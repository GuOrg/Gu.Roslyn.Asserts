namespace Gu.Roslyn.Asserts
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Text.RegularExpressions;
    using System.Xml.Linq;
    using Gu.Roslyn.Asserts.Internals;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.Diagnostics;
    using Microsoft.CodeAnalysis.Text;

    /// <summary>
    /// A helper for creating projects and solutions from strings of code.
    /// </summary>
    public static class CodeFactory
    {
        /// <summary>
        /// Creates a solution for <paramref name="code"/>
        /// </summary>
        /// <param name="code">The sources as strings.</param>
        /// <param name="metadataReferences">The <see cref="MetadataReference"/> to use when compiling.</param>
        /// <returns>A list with diagnostics per document.</returns>
        public static Solution CreateSolution(string code, params MetadataReference[] metadataReferences)
        {
            return CreateSolution(new[] { code }, (IReadOnlyList<MetadataReference>)metadataReferences);
        }

        /// <summary>
        /// Creates a solution for <paramref name="code"/>
        /// Each unique namespace in <paramref name="code"/> is added as a project.
        /// </summary>
        /// <param name="code">The sources as strings.</param>
        /// <param name="metadataReferences">The <see cref="MetadataReference"/> to use when compiling.</param>
        /// <returns>A list with diagnostics per document.</returns>
        public static Solution CreateSolution(IReadOnlyList<string> code, params MetadataReference[] metadataReferences)
        {
            return CreateSolution(code, (IReadOnlyList<MetadataReference>)metadataReferences);
        }

        /// <summary>
        /// Creates a solution for <paramref name="code"/>
        /// </summary>
        /// <param name="code">The sources as strings.</param>
        /// <param name="metadataReferences">The <see cref="MetadataReference"/> to use when compiling.</param>
        /// <returns>A list with diagnostics per document.</returns>
        public static Solution CreateSolution(string code, IReadOnlyList<MetadataReference> metadataReferences)
        {
            return CreateSolution(new[] { code }, metadataReferences);
        }

        /// <summary>
        /// Creates a solution for <paramref name="code"/>
        /// Each unique namespace in <paramref name="code"/> is added as a project.
        /// </summary>
        /// <param name="code">The sources as strings.</param>
        /// <param name="metadataReferences">The <see cref="MetadataReference"/> to use when compiling.</param>
        /// <returns>A list with diagnostics per document.</returns>
        public static Solution CreateSolution(IReadOnlyList<string> code, IReadOnlyList<MetadataReference> metadataReferences)
        {
            var compilationOptions = new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary, allowUnsafe: true);
            return CreateSolution(code, compilationOptions, metadataReferences);
        }

        /// <summary>
        /// Create a Solution with diagnostic options set to warning for all supported diagnostics in <paramref name="analyzers"/>
        /// </summary>
        /// <param name="code">The code to create the solution from.</param>
        /// <param name="analyzers">The analyzers to add diagnostic options for.</param>
        /// <param name="metadataReferences">The metadata references.</param>
        /// <returns>A <see cref="Solution"/></returns>
        public static Solution CreateSolution(string code, IReadOnlyList<DiagnosticAnalyzer> analyzers, IReadOnlyList<MetadataReference> metadataReferences = null)
        {
            return CreateSolution(new[] { code }, analyzers, metadataReferences);
        }

        /// <summary>
        /// Create a Solution with diagnostic options set to warning for all supported diagnostics in <paramref name="analyzers"/>
        /// Each unique namespace in <paramref name="code"/> is added as a project.
        /// </summary>
        /// <param name="code">The code to create the solution from.</param>
        /// <param name="analyzers">The analyzers to add diagnostic options for.</param>
        /// <param name="metadataReferences">The metadata references.</param>
        /// <returns>A <see cref="Solution"/></returns>
        public static Solution CreateSolution(IReadOnlyList<string> code, IReadOnlyList<DiagnosticAnalyzer> analyzers, IReadOnlyList<MetadataReference> metadataReferences = null)
        {
            var compilationOptions = new CSharpCompilationOptions(
                OutputKind.DynamicallyLinkedLibrary,
                allowUnsafe: true,
                specificDiagnosticOptions: GetSpecificDiagnosticOptions(analyzers, null));
            return CreateSolution(code, compilationOptions, metadataReferences);
        }

        /// <summary>
        /// Create a <see cref="Solution"/> for <paramref name="code"/>
        /// </summary>
        /// <param name="code">The code to create the solution from.</param>
        /// <param name="compilationOptions">The <see cref="CSharpCompilationOptions"/>.</param>
        /// <param name="metadataReferences">The metadata references.</param>
        /// <returns>A <see cref="Solution"/></returns>
        public static Solution CreateSolution(string code, CSharpCompilationOptions compilationOptions, IReadOnlyList<MetadataReference> metadataReferences = null)
        {
            return CreateSolution(new[] { code }, compilationOptions, metadataReferences);
        }

        /// <summary>
        /// Create a <see cref="Solution"/> for <paramref name="code"/>
        /// Each unique namespace in <paramref name="code"/> is added as a project.
        /// </summary>
        /// <param name="code">The code to create the solution from.</param>
        /// <param name="compilationOptions">The <see cref="CSharpCompilationOptions"/>.</param>
        /// <param name="metadataReferences">The metadata references.</param>
        /// <returns>A <see cref="Solution"/></returns>
        public static Solution CreateSolution(IEnumerable<string> code, CSharpCompilationOptions compilationOptions, IEnumerable<MetadataReference> metadataReferences = null)
        {
            var solution = new AdhocWorkspace().CurrentSolution;
            var byNamespaces = code.Select(c => new SourceMetadata(c))
                                   .GroupBy(c => c.Namespace);
            foreach (var byNamespace in byNamespaces)
            {
                var assemblyName = byNamespace.Key;
                var projectId = ProjectId.CreateNewId(assemblyName);
                solution = solution.AddProject(projectId, assemblyName, assemblyName, LanguageNames.CSharp)
                                   .WithProjectCompilationOptions(
                                       projectId,
                                       compilationOptions)
                                   .AddMetadataReferences(projectId, metadataReferences ?? Enumerable.Empty<MetadataReference>());
                foreach (var file in byNamespace)
                {
                    var documentId = DocumentId.CreateNewId(projectId);
                    solution = solution.AddDocument(documentId, file.FileName, file.Code);
                }
            }

            return solution;
        }

        /// <summary>
        /// Create a Solution with diagnostic options set to warning for all supported diagnostics in <paramref name="analyzers"/>
        /// </summary>
        /// <param name="code">
        /// The code to create the solution from.
        /// Can be a .cs, .csproj or .sln file
        /// </param>
        /// <param name="analyzers">The analyzers to add diagnostic options for.</param>
        /// <param name="metadataReferences">The metadata references.</param>
        /// <returns>A <see cref="Solution"/></returns>
        public static Solution CreateSolution(FileInfo code, IReadOnlyList<DiagnosticAnalyzer> analyzers, IReadOnlyList<MetadataReference> metadataReferences)
        {
            var compilationOptions = new CSharpCompilationOptions(
                OutputKind.DynamicallyLinkedLibrary,
                allowUnsafe: true,
                specificDiagnosticOptions: GetSpecificDiagnosticOptions(analyzers, null));
            return CreateSolution(code, analyzers, compilationOptions, metadataReferences);
        }

        /// <summary>
        /// Create a Solution with diagnostic options set to warning for all supported diagnostics in <paramref name="analyzers"/>
        /// </summary>
        /// <param name="code">
        /// The code to create the solution from.
        /// Can be a .cs, .csproj or .sln file
        /// </param>
        /// <param name="analyzers">The analyzers to add diagnostic options for.</param>
        /// <param name="compilationOptions">The <see cref="CompilationOptions"/> to use when compiling.</param>
        /// <param name="metadataReferences">The metadata references.</param>
        /// <returns>A <see cref="Solution"/></returns>
        public static Solution CreateSolution(FileInfo code, IReadOnlyList<DiagnosticAnalyzer> analyzers, CompilationOptions compilationOptions, IReadOnlyList<MetadataReference> metadataReferences)
        {
            if (string.Equals(code.Extension, ".cs", StringComparison.OrdinalIgnoreCase))
            {
                return CreateSolution(new[] { File.ReadAllText(code.FullName) }, analyzers, metadataReferences);
            }

            if (string.Equals(code.Extension, ".csproj", StringComparison.OrdinalIgnoreCase))
            {
                var solution = new AdhocWorkspace().CurrentSolution;
                var assemblyName = Path.GetFileNameWithoutExtension(code.FullName);
                var projectId = ProjectId.CreateNewId(assemblyName);
                solution = solution.AddProject(projectId, assemblyName, assemblyName, LanguageNames.CSharp)
                                   .WithProjectCompilationOptions(
                                       projectId,
                                       compilationOptions)
                                   .AddMetadataReferences(projectId, metadataReferences ?? Enumerable.Empty<MetadataReference>());
                foreach (var file in GetCodeFilesInProject(code))
                {
                    var documentId = DocumentId.CreateNewId(projectId);
                    using (var stream = File.OpenRead(file.FullName))
                    {
                        solution = solution.AddDocument(documentId, file.Name, SourceText.From(stream));
                    }
                }

                return solution;
            }

            if (string.Equals(code.Extension, ".sln", StringComparison.OrdinalIgnoreCase))
            {
                var sln = File.ReadAllText(code.FullName);
                var solution = new AdhocWorkspace().CurrentSolution;
                var matches = Regex.Matches(sln, @"Project\(""[^ ""]+""\) = ""(?<name>\w+(\.\w+)*)\"", ?""(?<path>\w+(\.\w+)*(\\\w+(\.\w+)*)*.csproj)", RegexOptions.ExplicitCapture);
                foreach (Match match in matches)
                {
                    var assemblyName = match.Groups["name"].Value;
                    var projectId = ProjectId.CreateNewId(assemblyName);
                    solution = solution.AddProject(projectId, assemblyName, assemblyName, LanguageNames.CSharp)
                                       .WithProjectCompilationOptions(
                                           projectId,
                                           compilationOptions)
                                       .AddMetadataReferences(projectId, metadataReferences ?? Enumerable.Empty<MetadataReference>());
                    foreach (var file in GetCodeFilesInProject(new FileInfo(Path.Combine(code.DirectoryName, match.Groups["path"].Value))))
                    {
                        var documentId = DocumentId.CreateNewId(projectId);
                        using (var stream = File.OpenRead(file.FullName))
                        {
                            solution = solution.AddDocument(documentId, file.Name, SourceText.From(stream));
                        }
                    }
                }

                return solution;
            }

            throw new NotSupportedException($"Cannot create a solution from {code.FullName}");
        }

        /// <summary>
        /// Searches parent directories for <paramref name="dllFile"/>
        /// </summary>
        /// <param name="dllFile">Ex Foo.dll</param>
        /// <param name="result">The <see cref="File"/> if found.</param>
        /// <returns>A value indicating if a file was found.</returns>
        public static bool TryFindProjectFile(FileInfo dllFile, out FileInfo result)
        {
            var projectFileName = Path.GetFileNameWithoutExtension(dllFile.FullName) + ".csproj";
            return TryFindFileInParentDirectory(dllFile.Directory, projectFileName, out result);
        }

        /// <summary>
        /// Searches parent directories for <paramref name="fileName"/>
        /// </summary>
        /// <param name="directory">The directory to start in.</param>
        /// <param name="fileName">Ex Foo.csproj</param>
        /// <param name="result">The <see cref="File"/> if found.</param>
        /// <returns>A value indicating if a file was found.</returns>
        public static bool TryFindFileInParentDirectory(DirectoryInfo directory, string fileName, out FileInfo result)
        {
            if (directory.EnumerateFiles(fileName).TryGetSingle(out result))
            {
                return true;
            }

            if (directory.Parent != null)
            {
                return TryFindFileInParentDirectory(directory.Parent, fileName, out result);
            }

            result = null;
            return false;
        }

        private static IReadOnlyCollection<KeyValuePair<string, ReportDiagnostic>> GetSpecificDiagnosticOptions(IEnumerable<DiagnosticAnalyzer> analyzers, IEnumerable<string> suppressed)
        {
            ReportDiagnostic WarnOrError(DiagnosticSeverity severity)
            {
                switch (severity)
                {
                    case DiagnosticSeverity.Error:
                        return ReportDiagnostic.Error;
                    case DiagnosticSeverity.Hidden:
                    case DiagnosticSeverity.Info:
                    case DiagnosticSeverity.Warning:
                        return ReportDiagnostic.Warn;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }

            var diagnosticOptions = analyzers.SelectMany(a => a.SupportedDiagnostics)
                                             .ToDictionary(d => d.Id, d => WarnOrError(d.DefaultSeverity));
            diagnosticOptions.Add("AD0001", ReportDiagnostic.Error);
            if (suppressed != null)
            {
                foreach (var id in suppressed)
                {
                    diagnosticOptions.Add(id, ReportDiagnostic.Suppress);
                }
            }

            return diagnosticOptions;
        }

        private static IEnumerable<FileInfo> GetCodeFilesInProject(FileInfo projectFile)
        {
            var doc = XDocument.Parse(File.ReadAllText(projectFile.FullName));
            var directory = projectFile.DirectoryName;
            var root = doc.Root;
            if (root?.Name == "Project" && root.Attribute("Sdk")?.Value == "Microsoft.NET.Sdk")
            {
                foreach (var csFile in projectFile.Directory
                                                  .EnumerateFiles("*.cs", SearchOption.TopDirectoryOnly))
                {
                    yield return csFile;
                }

                foreach (var dir in projectFile.Directory
                                               .EnumerateDirectories("*", SearchOption.TopDirectoryOnly)
                                               .Where(dir => !string.Equals(dir.Name, "bin", StringComparison.OrdinalIgnoreCase))
                                               .Where(dir => !string.Equals(dir.Name, "obj", StringComparison.OrdinalIgnoreCase)))
                {
                    foreach (var nestedFile in dir.EnumerateFiles("*.cs", SearchOption.AllDirectories))
                    {
                        yield return nestedFile;
                    }
                }
            }
            else
            {
                var compiles = doc.Descendants(XName.Get("Compile", "http://schemas.microsoft.com/developer/msbuild/2003"))
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

                    var csFile = Path.Combine(directory, include);
                    yield return new FileInfo(csFile);
                }
            }
        }

        private struct SourceMetadata
        {
            public SourceMetadata(string code)
            {
                this.Code = code;
                this.FileName = CodeReader.FileName(code);
                this.Namespace = CodeReader.Namespace(code);
            }

            internal string Code { get; }

            internal string FileName { get; }

            internal string Namespace { get; }
        }
    }
}