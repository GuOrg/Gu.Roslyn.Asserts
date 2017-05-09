namespace Gu.Roslyn.Asserts
{
    using System.Collections.Generic;
    using System.Collections.Immutable;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.Diagnostics;

    public static class CodeFactory
    {
        /// <summary>
        /// Creates a solution and adds the <paramref name="analyzer"/> as analyzer.
        /// Then compiles it and returns the diagnostics.
        /// </summary>
        /// <param name="analyzer">The analyzer to find diagnostics for.</param>
        /// <param name="sources">The sources as strings.</param>
        /// <param name="references">The <see cref="MetadataReference"/> to use when compiling.</param>
        /// <returns>A list with diagnostics per document.</returns>
        public static async Task<DiagnosticsWithMetaData> GetDiagnosticsWithMetaDataAsync(DiagnosticAnalyzer analyzer, IEnumerable<string> sources, IEnumerable<MetadataReference> references)
        {
            var sln = CreateSolution(sources, new[] { analyzer }, references);
            var results = new List<ImmutableArray<Diagnostic>>();
            foreach (var project in sln.Projects)
            {
                var compilation = await project.GetCompilationAsync(CancellationToken.None)
                                               .ConfigureAwait(false);

                var withAnalyzers = compilation.WithAnalyzers(
                    ImmutableArray.Create(analyzer),
                    project.AnalyzerOptions,
                    CancellationToken.None);
                results.Add(await withAnalyzers.GetAnalyzerDiagnosticsAsync(CancellationToken.None)
                                               .ConfigureAwait(false));
            }

            return new DiagnosticsWithMetaData(sln, results);
        }

        /// <summary>
        /// Creates a solution and adds the <paramref name="analyzer"/> as analyzer.
        /// Then compiles it and returns the diagnostics.
        /// </summary>
        /// <param name="analyzer">The analyzer to find diagnostics for.</param>
        /// <param name="sources">The sources as strings.</param>
        /// <param name="references">The <see cref="MetadataReference"/> to use when compiling.</param>
        /// <returns>A list with diagnostics per document.</returns>
        public static async Task<IReadOnlyList<ImmutableArray<Diagnostic>>> GetDiagnosticsAsync(DiagnosticAnalyzer analyzer, IEnumerable<string> sources, IEnumerable<MetadataReference> references)
        {
            var sln = CreateSolution(sources, new[] { analyzer }, references);
            var results = new List<ImmutableArray<Diagnostic>>();
            foreach (var project in sln.Projects)
            {
                var compilation = await project.GetCompilationAsync(CancellationToken.None)
                                               .ConfigureAwait(false);

                var withAnalyzers = compilation.WithAnalyzers(
                    ImmutableArray.Create(analyzer),
                    project.AnalyzerOptions,
                    CancellationToken.None);
                results.Add(await withAnalyzers.GetAnalyzerDiagnosticsAsync(CancellationToken.None)
                                               .ConfigureAwait(false));
            }

            return results;
        }

        public static Solution CreateSolution(string[] code, params MetadataReference[] metadataReferences)
        {
            return CreateSolution(code, (IEnumerable<MetadataReference>)metadataReferences);
        }

        public static Solution CreateSolution(IEnumerable<string> code, IEnumerable<MetadataReference> metadataReferences)
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
        public static Solution CreateSolution(IEnumerable<string> code, IEnumerable<DiagnosticAnalyzer> analyzers, IEnumerable<MetadataReference> metadataReferences)
        {
            var solution = new AdhocWorkspace()
                .CurrentSolution;
            var byNamespaces = code.Select(c => new SourceMetaData(c))
                                   .GroupBy(c => c.Namespace);

            var diagnosticOptions = analyzers.SelectMany(a => a.SupportedDiagnostics)
                                             .ToDictionary(d => d.Id, d => ReportDiagnostic.Warn);
            diagnosticOptions.Add("AD0001", ReportDiagnostic.Error);
            foreach (var byNamespace in byNamespaces)
            {
                var assemblyName = byNamespace.Key;
                var projectId = ProjectId.CreateNewId(assemblyName);
                solution = solution.AddProject(projectId, assemblyName, assemblyName, LanguageNames.CSharp)
                                   .WithProjectCompilationOptions(
                                       projectId,
                                       new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary, allowUnsafe: true)
                                                .WithSpecificDiagnosticOptions(diagnosticOptions))
                                   .AddMetadataReferences(projectId, metadataReferences ?? Enumerable.Empty<MetadataReference>());
                foreach (var file in byNamespace)
                {
                    var documentId = DocumentId.CreateNewId(projectId);
                    solution = solution.AddDocument(documentId, file.FileName, file.Code);
                }
            }

            return solution;
        }

        public class DiagnosticsWithMetaData
        {
            public DiagnosticsWithMetaData(Solution solution, List<ImmutableArray<Diagnostic>> diagnostics)
            {
                this.Solution = solution;
                this.Diagnostics = diagnostics;
            }

            public Solution Solution { get; }

            public IReadOnlyList<ImmutableArray<Diagnostic>> Diagnostics { get; }
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