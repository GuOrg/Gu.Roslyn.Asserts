namespace Gu.Roslyn.Asserts
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Xml.Linq;
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
        /// If code contains many namespaces one project per namespace is created.
        /// </summary>
        /// <param name="code">The sources as strings.</param>
        /// <param name="metadataReferences">The <see cref="MetadataReference"/> to use when compiling.</param>
        /// <returns>A list with diagnostics per document.</returns>
        public static Solution CreateSolution(string[] code, params MetadataReference[] metadataReferences)
        {
            return CreateSolution(code, (IReadOnlyList<MetadataReference>)metadataReferences);
        }

        /// <summary>
        /// Creates a solution for <paramref name="code"/>
        /// If code contains many namespaces one project per namespace is created.
        /// </summary>
        /// <param name="code">The sources as strings.</param>
        /// <param name="metadataReferences">The <see cref="MetadataReference"/> to use when compiling.</param>
        /// <returns>A list with diagnostics per document.</returns>
        public static Solution CreateSolution(IReadOnlyList<string> code, IReadOnlyList<MetadataReference> metadataReferences)
        {
            var solution = new AdhocWorkspace()
                .CurrentSolution;
            var byNamespaces = code.Select(c => new SourceMetaData(c))
                .GroupBy(c => c.Namespace);
            foreach (var byNamespace in byNamespaces)
            {
                var assemblyName = byNamespace.Key;
                var projectId = ProjectId.CreateNewId(assemblyName);
                solution = solution.AddProject(projectId, assemblyName, assemblyName, LanguageNames.CSharp)
                    .WithProjectCompilationOptions(
                        projectId,
                        new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary, allowUnsafe: true))
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
        /// <param name="code">The code to create the solution from.</param>
        /// <param name="analyzers">The analyzers to add diagnostic options for.</param>
        /// <param name="metadataReferences">The metadata references.</param>
        /// <returns>A <see cref="Solution"/></returns>
        public static Solution CreateSolution(IReadOnlyList<string> code, IReadOnlyList<DiagnosticAnalyzer> analyzers, IReadOnlyList<MetadataReference> metadataReferences)
        {
            var solution = new AdhocWorkspace()
                .CurrentSolution;
            var byNamespaces = code.Select(c => new SourceMetaData(c))
                                   .GroupBy(c => c.Namespace);
            var specificDiagnosticOptions = GetSpecificDiagnosticOptions(analyzers);
            foreach (var byNamespace in byNamespaces)
            {
                var assemblyName = byNamespace.Key;
                var projectId = ProjectId.CreateNewId(assemblyName);
                solution = solution.AddProject(projectId, assemblyName, assemblyName, LanguageNames.CSharp)
                                   .WithProjectCompilationOptions(
                                       projectId,
                                       new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary, allowUnsafe: true)
                                                .WithSpecificDiagnosticOptions(specificDiagnosticOptions))
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
                                       new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary, allowUnsafe: true)
                                           .WithSpecificDiagnosticOptions(GetSpecificDiagnosticOptions(analyzers)))
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
            var files = directory.EnumerateFiles(fileName);
            var count = files.Count();
            if (count == 0)
            {
                if (directory.Parent != null)
                {
                    return TryFindFileInParentDirectory(directory.Parent, fileName, out result);
                }

                result = null;
                return false;
            }

            if (count == 1)
            {
                result = files.Single();
                return true;
            }

            result = null;
            return false;
        }

        private static IReadOnlyCollection<KeyValuePair<string, ReportDiagnostic>> GetSpecificDiagnosticOptions(IReadOnlyList<DiagnosticAnalyzer> analyzers)
        {
            var diagnosticOptions = analyzers.SelectMany(a => a.SupportedDiagnostics)
                                             .ToDictionary(d => d.Id, d => ReportDiagnostic.Warn);
            diagnosticOptions.Add("AD0001", ReportDiagnostic.Error);
            return diagnosticOptions;
        }

        private static IEnumerable<FileInfo> GetCodeFilesInProject(FileInfo projectFile)
        {
            var doc = XDocument.Parse(File.ReadAllText(projectFile.FullName));
            var directory = projectFile.DirectoryName;
            var compiles = doc.Descendants(XName.Get("Compile", "http://schemas.microsoft.com/developer/msbuild/2003"))
                              .ToArray();
            if (compiles.Length == 0)
            {
                throw new InvalidOperationException("Parsing failed, no <Compile ... /> found.");
            }

            foreach (var compile in compiles)
            {
                var csFile = Path.Combine(directory, compile.Attribute("Include").Value);
                yield return new FileInfo(csFile);
            }
        }

        private struct SourceMetaData
        {
            public SourceMetaData(string code)
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