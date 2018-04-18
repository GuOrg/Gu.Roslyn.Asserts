namespace Gu.Roslyn.Asserts
{
    using System;
    using System.Collections.Generic;
    using System.Collections.Immutable;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using System.Text;
    using System.Text.RegularExpressions;
    using System.Xml.Linq;
    using Microsoft.CodeAnalysis;

    public static class ProjectFile
    {
        /// <summary>
        /// Searches parent directories for <paramref name="dllFile"/>
        /// </summary>
        /// <param name="dllFile">Ex Foo.dll</param>
        /// <param name="result">The <see cref="File"/> if found.</param>
        /// <returns>A value indicating if a file was found.</returns>
        public static bool TryFind(FileInfo dllFile, out FileInfo result)
        {
            result = null;
            if (SolutionFile.TryFind(Assembly.GetCallingAssembly(), out var sln))
            {
                var projectFileName = Path.GetFileNameWithoutExtension(dllFile.FullName) + ".csproj";
                //// ReSharper disable once PossibleNullReferenceException
                result = sln.Directory.EnumerateFiles(projectFileName, SearchOption.AllDirectories).FirstOrDefault();
            }

            return result != null;
        }

        /// <summary>
        /// Searches parent directories for <paramref name="projectFile"/>
        /// </summary>
        /// <param name="projectFile">Ex Foo.csproj</param>
        /// <param name="result">The <see cref="File"/> if found.</param>
        /// <returns>A value indicating if a file was found.</returns>
        public static bool TryFind(string projectFile, out FileInfo result)
        {
            result = null;
            if (SolutionFile.TryFind(Assembly.GetCallingAssembly(), out var sln))
            {
                // ReSharper disable once PossibleNullReferenceException
                result = sln.Directory.EnumerateFiles(projectFile, SearchOption.AllDirectories).FirstOrDefault();
            }

            return result != null;
        }

        /// <summary>
        /// Searches parent directories for <paramref name="projectFile"/>
        /// </summary>
        /// <param name="projectFile">Ex Foo.csproj</param>
        /// <returns>The project file.</returns>
        public static FileInfo Find(string projectFile)
        {
            if (SolutionFile.TryFind(Assembly.GetCallingAssembly(), out var sln))
            {
                // ReSharper disable once PossibleNullReferenceException
                var result = sln.Directory.EnumerateFiles(projectFile, SearchOption.AllDirectories).FirstOrDefault();
                if (result == null)
                {
                    throw new InvalidOperationException("Did not find a file named: " + projectFile);
                }

                return result;
            }

            throw new InvalidOperationException("Did not find a sln for: " + Assembly.GetCallingAssembly());
        }

        public static ProjectInfo ParseInfo(FileInfo csproj)
        {
            return ParseInfo(csproj, ProjectId.CreateNewId(csproj.FullName), out _);
        }

        internal static ProjectInfo ParseInfo(ProjectId projectId, ImmutableDictionary<ProjectId, FileInfo> idFileMap)
        {
            var csproj = idFileMap[projectId];
            return ParseInfo(csproj, projectId, out var content).WithProjectReferences(GetProjectReferences());

            IReadOnlyList<ProjectReference> GetProjectReferences()
            {
                var root = content.Root;
                if (root == null)
                {
                    return new ProjectReference[0];
                }

                return root.Descendants(XName.Get("ProjectReference"))
                           .Select(e => e.Attribute("Include")?.Value)
                           .Where(x => x != null)
                           .Select(FindReference)
                           .ToArray();
            }

            ProjectReference FindReference(string reference)
            {
                var fileInfo = new FileInfo(Path.Combine(csproj.Directory.FullName, reference));
                foreach (var kvp in idFileMap)
                {
                    if (kvp.Value.FullName == fileInfo.FullName)
                    {
                        return new ProjectReference(kvp.Key);
                    }
                }

                throw new InvalidOperationException($"Did not find project id for {reference}");
            }
        }

        private static ProjectInfo ParseInfo(FileInfo csproj, ProjectId projectId, out XDocument content)
        {
            var name = Path.GetFileNameWithoutExtension(csproj.FullName);
            var xml = System.IO.File.ReadAllText(csproj.FullName);
            var xdoc = content = XDocument.Parse(xml);
            return ProjectInfo.Create(
                projectId,
                VersionStamp.Create(csproj.LastWriteTimeUtc),
                name,
                name,
                LanguageNames.CSharp,
                csproj.FullName,
                documents: GetDocuments());

            IEnumerable<DocumentInfo> GetDocuments()
            {
                // Is Sdk project
                if (Regex.IsMatch(xml, @"<TargetFrameworks?\b"))
                {
                    foreach (var csFile in csproj.Directory.EnumerateFiles("*.cs", SearchOption.TopDirectoryOnly))
                    {
                        yield return CreateDocumentInfo(csFile);
                    }

                    foreach (var dir in csproj.Directory.EnumerateDirectories("*", SearchOption.TopDirectoryOnly)
                                                 .Where(dir => !string.Equals(dir.Name, "bin", StringComparison.OrdinalIgnoreCase))
                                                 .Where(dir => !string.Equals(dir.Name, "obj", StringComparison.OrdinalIgnoreCase)))
                    {
                        foreach (var nestedFile in dir.EnumerateFiles("*.cs", SearchOption.AllDirectories))
                        {
                            yield return CreateDocumentInfo(nestedFile);
                        }
                    }
                }
                else
                {
                    var compiles = xdoc.Descendants(XName.Get("Compile", "http://schemas.microsoft.com/developer/msbuild/2003"))
                                       .ToArray();
                    if (compiles.Length == 0)
                    {
                        throw new InvalidOperationException("Parsing failed, no <Compile ... /> found.");
                    }

                    foreach (var compile in compiles)
                    {
                        var include = compile.Attribute("Include")?.Value;
                        if (include == null)
                        {
                            throw new InvalidOperationException("Parsing failed, no Include found.");
                        }

                        var csFile = Path.Combine(csproj.Directory.FullName, include);
                        yield return CreateDocumentInfo(new FileInfo(csFile));
                    }
                }
            }

            DocumentInfo CreateDocumentInfo(FileInfo file)
            {
                return DocumentInfo.Create(
                    DocumentId.CreateNewId(projectId, file.Name),
                    file.Name,
                    sourceCodeKind: SourceCodeKind.Regular,
                    filePath: file.FullName,
                    isGenerated: file.Name.EndsWith(".g.cs"),
                    loader: new FileTextLoader(file.FullName, Encoding.UTF8));
            }
        }
    }
}
