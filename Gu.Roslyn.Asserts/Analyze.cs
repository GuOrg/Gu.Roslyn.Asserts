namespace Gu.Roslyn.Asserts
{
    using System;
    using System.Collections.Generic;
    using System.Collections.Immutable;
    using System.IO;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.Diagnostics;

    /// <summary>
    /// Class exposing functionality for analyzing code.
    /// </summary>
    public static class Analyze
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
            var sln = CodeFactory.CreateSolution(sources, new[] { analyzer }, references);
            var results = await GetDiagnosticsAsync(analyzer, sln);

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
        public static Task<IReadOnlyList<ImmutableArray<Diagnostic>>> GetDiagnosticsAsync(DiagnosticAnalyzer analyzer, IEnumerable<string> sources, IEnumerable<MetadataReference> references)
        {
            var sln = CodeFactory.CreateSolution(sources, new[] { analyzer }, references);
            return GetDiagnosticsAsync(analyzer, sln);
        }

        /// <summary>
        /// Creates a solution and adds the <typeparamref name="TAnalyzer"/> as analyzer.
        /// Then compiles it and returns the diagnostics.
        /// </summary>
        /// <typeparam name="TAnalyzer">The type of the analyzer.</typeparam>
        /// <param name="code">
        /// The code to create the solution from.
        /// Can be a .cs, .csproj or .sln file
        /// </param>
        /// <param name="references">The <see cref="MetadataReference"/> to use when compiling.</param>
        /// <returns>A list with diagnostics per document.</returns>
        public static Task<IReadOnlyList<ImmutableArray<Diagnostic>>> GetDiagnosticsAsync<TAnalyzer>(FileInfo code, IEnumerable<MetadataReference> references)
            where TAnalyzer : DiagnosticAnalyzer, new()
        {
            return GetDiagnosticsAsync(new TAnalyzer(), code, references);
        }

        /// <summary>
        /// Creates a solution and adds the <paramref name="analyzerType"/> as analyzer.
        /// Then compiles it and returns the diagnostics.
        /// </summary>
        /// <param name="analyzerType">The type of the analyzer.</param>
        /// <param name="code">
        /// The code to create the solution from.
        /// Can be a .cs, .csproj or .sln file
        /// </param>
        /// <param name="references">The <see cref="MetadataReference"/> to use when compiling.</param>
        /// <returns>A list with diagnostics per document.</returns>
        public static Task<IReadOnlyList<ImmutableArray<Diagnostic>>> GetDiagnosticsAsync(Type analyzerType, FileInfo code, IEnumerable<MetadataReference> references)
        {
            return GetDiagnosticsAsync((DiagnosticAnalyzer)Activator.CreateInstance(analyzerType), code, references);
        }

        /// <summary>
        /// Creates a solution and adds the <paramref name="analyzer"/> as analyzer.
        /// Then compiles it and returns the diagnostics.
        /// </summary>
        /// <param name="analyzer">The analyzer to find diagnostics for.</param>
        /// <param name="code">
        /// The code to create the solution from.
        /// Can be a .cs, .csproj or .sln file
        /// </param>
        /// <param name="references">The <see cref="MetadataReference"/> to use when compiling.</param>
        /// <returns>A list with diagnostics per document.</returns>
        public static Task<IReadOnlyList<ImmutableArray<Diagnostic>>> GetDiagnosticsAsync(DiagnosticAnalyzer analyzer, FileInfo code, IEnumerable<MetadataReference> references)
        {
            var sln = CodeFactory.CreateSolution(code, new[] { analyzer }, references);
            return GetDiagnosticsAsync(analyzer, sln);
        }

        private static async Task<IReadOnlyList<ImmutableArray<Diagnostic>>> GetDiagnosticsAsync(DiagnosticAnalyzer analyzer, Solution sln)
        {
            var results = new List<ImmutableArray<Diagnostic>>();
            foreach (var project in sln.Projects)
            {
                var compilation = await project.GetCompilationAsync(CancellationToken.None)
                                               .ConfigureAwait(false);
                if (analyzer is PlaceholderAnalyzer)
                {
                    results.Add(compilation.GetDiagnostics(CancellationToken.None));
                }
                else
                {
                    var withAnalyzers = compilation.WithAnalyzers(
                        ImmutableArray.Create(analyzer),
                        project.AnalyzerOptions,
                        CancellationToken.None);
                    results.Add(await withAnalyzers.GetAnalyzerDiagnosticsAsync(CancellationToken.None)
                                                   .ConfigureAwait(false));
                }
            }

            return results;
        }

        /// <summary>
        /// The diagnostics and the solution the analysis was performed on.
        /// </summary>
        public class DiagnosticsWithMetaData
        {
            /// <summary>
            /// Initializes a new instance of the <see cref="DiagnosticsWithMetaData"/> class.
            /// </summary>
            /// <param name="solution">The solution the analysis was performed on.</param>
            /// <param name="diagnostics">The diagnostics returned from Roslyn.</param>
            public DiagnosticsWithMetaData(Solution solution, IReadOnlyList<ImmutableArray<Diagnostic>> diagnostics)
            {
                this.Solution = solution;
                this.Diagnostics = diagnostics;
            }

            /// <summary>
            /// Gets the solution the analysis was performed on.
            /// </summary>
            public Solution Solution { get; }

            /// <summary>
            /// Gets the diagnostics returned from Roslyn.
            /// </summary>
            public IReadOnlyList<ImmutableArray<Diagnostic>> Diagnostics { get; }
        }
    }
}