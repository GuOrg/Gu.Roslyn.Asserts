namespace Gu.Roslyn.Asserts
{
    using System;
    using System.Collections.Generic;
    using System.Collections.Immutable;
    using System.IO;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CodeFixes;
    using Microsoft.CodeAnalysis.CSharp;
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
        /// <param name="compilationOptions">The <see cref="CSharpCompilationOptions"/> to use.</param>
        /// <param name="references">The <see cref="MetadataReference"/> to use when compiling.</param>
        /// <returns>A list with diagnostics per document.</returns>
        [Obsolete("To be removed.")]
        public static async Task<DiagnosticsWithMetadata> GetDiagnosticsWithMetadataAsync(DiagnosticAnalyzer analyzer, IReadOnlyList<string> sources, CSharpCompilationOptions compilationOptions, IEnumerable<MetadataReference> references)
        {
            var sln = CodeFactory.CreateSolution(sources, compilationOptions, references);
            var diagnostics = await GetDiagnosticsAsync(sln, analyzer).ConfigureAwait(false);
            return new DiagnosticsWithMetadata(sln, diagnostics);
        }

        /// <summary>
        /// Creates a solution and adds the <paramref name="analyzer"/> as analyzer.
        /// Then compiles it and returns the diagnostics.
        /// </summary>
        /// <param name="analyzer">The analyzer to find diagnostics for.</param>
        /// <param name="sources">The sources as strings.</param>
        /// <param name="references">The <see cref="MetadataReference"/> to use when compiling.</param>
        /// <returns>A list with diagnostics per document.</returns>
        [Obsolete("To be removed.")]
        public static async Task<DiagnosticsWithMetadata> GetDiagnosticsWithMetadataAsync(DiagnosticAnalyzer analyzer, IReadOnlyList<string> sources, IReadOnlyList<MetadataReference> references)
        {
            var sln = CodeFactory.CreateSolution(sources, new[] { analyzer }, references);
            var diagnostics = await GetDiagnosticsAsync(sln, analyzer).ConfigureAwait(false);
            return new DiagnosticsWithMetadata(sln, diagnostics);
        }

        /// <summary>
        /// Creates a solution and adds the <paramref name="analyzer"/> as analyzer.
        /// Then compiles it and returns the diagnostics.
        /// </summary>
        /// <param name="analyzer">The analyzer to find diagnostics for.</param>
        /// <param name="sources">The sources as strings.</param>
        /// <param name="metadataReferences">The <see cref="MetadataReference"/> to use when compiling.</param>
        /// <returns>A list with diagnostics per document.</returns>
        public static Task<IReadOnlyList<ImmutableArray<Diagnostic>>> GetDiagnosticsAsync(DiagnosticAnalyzer analyzer, IReadOnlyList<string> sources, IReadOnlyList<MetadataReference> metadataReferences)
        {
            return GetDiagnosticsAsync(
                analyzer,
                sources,
                CodeFactory.DefaultCompilationOptions(analyzer, null),
                metadataReferences);
        }

        /// <summary>
        /// Creates a solution and adds the <paramref name="analyzer"/> as analyzer.
        /// Then compiles it and returns the diagnostics.
        /// </summary>
        /// <param name="analyzer">The analyzer to find diagnostics for.</param>
        /// <param name="sources">The sources as strings.</param>
        /// <param name="compilationOptions">The <see cref="CSharpCompilationOptions"/> to use.</param>
        /// <param name="metadataReferences">The <see cref="MetadataReference"/> to use when compiling.</param>
        /// <returns>A list with diagnostics per document.</returns>
        public static IReadOnlyList<ImmutableArray<Diagnostic>> GetDiagnostics(DiagnosticAnalyzer analyzer, IReadOnlyList<string> sources, CSharpCompilationOptions compilationOptions, IReadOnlyList<MetadataReference> metadataReferences)
        {
            var sln = CodeFactory.CreateSolution(sources, compilationOptions, metadataReferences);
            return GetDiagnostics(sln, analyzer);
        }

        /// <summary>
        /// Creates a solution and adds the <paramref name="analyzer"/> as analyzer.
        /// Then compiles it and returns the diagnostics.
        /// </summary>
        /// <param name="analyzer">The analyzer to find diagnostics for.</param>
        /// <param name="sources">The sources as strings.</param>
        /// <param name="compilationOptions">The <see cref="CSharpCompilationOptions"/> to use.</param>
        /// <param name="metadataReferences">The <see cref="MetadataReference"/> to use when compiling.</param>
        /// <returns>A list with diagnostics per document.</returns>
        public static Task<IReadOnlyList<ImmutableArray<Diagnostic>>> GetDiagnosticsAsync(DiagnosticAnalyzer analyzer, IReadOnlyList<string> sources, CSharpCompilationOptions compilationOptions, IReadOnlyList<MetadataReference> metadataReferences)
        {
            if (analyzer == null)
            {
                throw new ArgumentNullException(nameof(analyzer));
            }

            var sln = CodeFactory.CreateSolution(sources, compilationOptions, metadataReferences);
            return GetDiagnosticsAsync(sln, analyzer);
        }

        /// <summary>
        /// Creates a solution and adds the <typeparamref name="TAnalyzer"/> as analyzer.
        /// Then compiles it and returns the diagnostics.
        /// </summary>
        /// <typeparam name="TAnalyzer">The type of the analyzer.</typeparam>
        /// <param name="code">
        /// The code to create the solution from.
        /// Can be a .cs, .csproj or .solution file
        /// </param>
        /// <param name="references">The <see cref="MetadataReference"/> to use when compiling.</param>
        /// <returns>A list with diagnostics per document.</returns>
        public static Task<IReadOnlyList<ImmutableArray<Diagnostic>>> GetDiagnosticsAsync<TAnalyzer>(FileInfo code, IReadOnlyList<MetadataReference> references)
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
        /// Can be a .cs, .csproj or .solution file
        /// </param>
        /// <param name="metadataReferences">The <see cref="MetadataReference"/> to use when compiling.</param>
        /// <returns>A list with diagnostics per document.</returns>
        public static Task<IReadOnlyList<ImmutableArray<Diagnostic>>> GetDiagnosticsAsync(Type analyzerType, FileInfo code, IReadOnlyList<MetadataReference> metadataReferences)
        {
            return GetDiagnosticsAsync((DiagnosticAnalyzer)Activator.CreateInstance(analyzerType), code, metadataReferences);
        }

        /// <summary>
        /// Creates a solution and adds the <paramref name="analyzer"/> as analyzer.
        /// Then compiles it and returns the diagnostics.
        /// </summary>
        /// <param name="analyzer">The analyzer to find diagnostics for.</param>
        /// <param name="code">
        /// The code to create the solution from.
        /// Can be a .cs, .csproj or .solution file
        /// </param>
        /// <param name="metadataReferences">The <see cref="MetadataReference"/> to use when compiling.</param>
        /// <returns>A list with diagnostics per document.</returns>
        public static Task<IReadOnlyList<ImmutableArray<Diagnostic>>> GetDiagnosticsAsync(DiagnosticAnalyzer analyzer, FileInfo code, IReadOnlyList<MetadataReference> metadataReferences)
        {
            var sln = CodeFactory.CreateSolution(code, new[] { analyzer }, metadataReferences);
            return GetDiagnosticsAsync(sln, analyzer);
        }

        /// <summary>
        /// Creates a solution and adds the <paramref name="analyzer"/> as analyzer.
        /// Then compiles it and returns the diagnostics.
        /// </summary>
        /// <param name="analyzer">The analyzer to find diagnostics for.</param>
        /// <param name="code">
        /// The code to create the solution from.
        /// Can be a .cs, .csproj or .solution file
        /// </param>
        /// <param name="compilationOptions">The <see cref="CSharpCompilationOptions"/> to use.</param>
        /// <param name="metadataReferences">The <see cref="MetadataReference"/> to use when compiling.</param>
        /// <returns>A list with diagnostics per document.</returns>
        public static Task<IReadOnlyList<ImmutableArray<Diagnostic>>> GetDiagnosticsAsync(DiagnosticAnalyzer analyzer, FileInfo code, CSharpCompilationOptions compilationOptions, IReadOnlyList<MetadataReference> metadataReferences)
        {
            var sln = CodeFactory.CreateSolution(code, compilationOptions, metadataReferences);
            return GetDiagnosticsAsync(sln, analyzer);
        }

        /// <summary>
        /// Creates a solution, compiles it and returns the diagnostics.
        /// </summary>
        /// <param name="solution">The solution.</param>
        /// <returns>A list with diagnostics per document.</returns>
        public static async Task<IReadOnlyList<ImmutableArray<Diagnostic>>> GetDiagnosticsAsync(Solution solution)
        {
            var results = new List<ImmutableArray<Diagnostic>>();
            foreach (var project in solution.Projects)
            {
                var compilation = await project.GetCompilationAsync(CancellationToken.None)
                                               .ConfigureAwait(false);
                results.Add(compilation.GetDiagnostics(CancellationToken.None));
            }

            return results;
        }

        /// <summary>
        /// Creates a solution, compiles it and returns the diagnostics.
        /// </summary>
        /// <param name="solution">The solution.</param>
        /// <returns>A list with diagnostics per document.</returns>
        public static IReadOnlyList<ImmutableArray<Diagnostic>> GetDiagnostics(Solution solution)
        {
            var results = new List<ImmutableArray<Diagnostic>>();
            foreach (var project in solution.Projects)
            {
                var compilation = project.GetCompilationAsync(CancellationToken.None).GetAwaiter().GetResult();
                results.Add(compilation.GetDiagnostics(CancellationToken.None));
            }

            return results;
        }

        /// <summary>
        /// Creates a solution, compiles it and returns the diagnostics fixable by <paramref name="codeFix"/>.
        /// </summary>
        /// <param name="solution">The solution.</param>
        /// <param name="analyzer">The analyzer.</param>
        /// <param name="codeFix">The code fix to use when filtering the diagnostics.</param>
        /// <returns>A list with all fixable diagnostics.</returns>
        public static async Task<IReadOnlyList<Diagnostic>> GetFixableDiagnosticsAsync(Solution solution, DiagnosticAnalyzer analyzer, CodeFixProvider codeFix)
        {
            var fixableDiagnostics = new List<Diagnostic>();
            foreach (var project in solution.Projects)
            {
                var compilation = await project.GetCompilationAsync(CancellationToken.None)
                                               .ConfigureAwait(false);
                if (analyzer is PlaceholderAnalyzer)
                {
                    fixableDiagnostics.AddRange(compilation.GetDiagnostics(CancellationToken.None).Where(d => codeFix.FixableDiagnosticIds.Contains(d.Id)));
                }
                else
                {
                    var withAnalyzers = compilation.WithAnalyzers(
                        ImmutableArray.Create(analyzer),
                        project.AnalyzerOptions,
                        CancellationToken.None);
                    var diagnostics = await withAnalyzers.GetAnalyzerDiagnosticsAsync(CancellationToken.None)
                                                         .ConfigureAwait(false);
                    fixableDiagnostics.AddRange(diagnostics.Where(d => codeFix.FixableDiagnosticIds.Contains(d.Id)));
                }
            }

            return fixableDiagnostics;
        }

        /// <summary>
        /// Creates a solution, compiles it and returns the diagnostics.
        /// </summary>
        /// <param name="solution">The solution.</param>
        /// <param name="analyzer">The analyzer.</param>
        /// <returns>A list with diagnostics per document.</returns>
        public static async Task<IReadOnlyList<ImmutableArray<Diagnostic>>> GetDiagnosticsAsync(Solution solution, DiagnosticAnalyzer analyzer)
        {
            var results = new List<ImmutableArray<Diagnostic>>();
            foreach (var project in solution.Projects)
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
        /// Creates a solution, compiles it and returns the diagnostics.
        /// </summary>
        /// <param name="solution">The solution.</param>
        /// <param name="analyzer">The analyzer.</param>
        /// <returns>A list with diagnostics per document.</returns>
        public static IReadOnlyList<ImmutableArray<Diagnostic>> GetDiagnostics(Solution solution, DiagnosticAnalyzer analyzer)
        {
            return GetDiagnostics(analyzer, solution);
        }

        /// <summary>
        /// Creates a solution, compiles it and returns the diagnostics.
        /// </summary>
        /// <param name="analyzer">The analyzer.</param>
        /// <param name="solution">The solution.</param>
        /// <returns>A list with diagnostics per document.</returns>
        public static IReadOnlyList<ImmutableArray<Diagnostic>> GetDiagnostics(DiagnosticAnalyzer analyzer, Solution solution)
        {
            var results = new List<ImmutableArray<Diagnostic>>();
            foreach (var project in solution.Projects)
            {
                var compilation = project.GetCompilationAsync(CancellationToken.None).GetAwaiter().GetResult();
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
                    results.Add(withAnalyzers.GetAnalyzerDiagnosticsAsync(CancellationToken.None).GetAwaiter().GetResult());
                }
            }

            return results;
        }

        /// <summary>
        /// Creates a solution, compiles it and returns the diagnostics.
        /// </summary>
        /// <param name="project">The project.</param>
        /// <param name="analyzer">The analyzer.</param>
        /// <returns>A list with diagnostics per document.</returns>
        public static async Task<IReadOnlyList<ImmutableArray<Diagnostic>>> GetDiagnosticsAsync(Project project, DiagnosticAnalyzer analyzer)
        {
            var results = new List<ImmutableArray<Diagnostic>>();
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

            return results;
        }

        /// <summary>
        /// Creates a solution, compiles it and returns the diagnostics.
        /// </summary>
        /// <param name="project">The project.</param>
        /// <param name="analyzer">The analyzer.</param>
        /// <returns>A list with diagnostics per document.</returns>
        public static IReadOnlyList<ImmutableArray<Diagnostic>> GetDiagnostics(Project project, DiagnosticAnalyzer analyzer)
        {
            var results = new List<ImmutableArray<Diagnostic>>();
            var compilation = project.GetCompilationAsync(CancellationToken.None).GetAwaiter().GetResult();
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
                results.Add(withAnalyzers.GetAnalyzerDiagnosticsAsync(CancellationToken.None).GetAwaiter().GetResult());
            }

            return results;
        }

        /// <summary>
        /// The diagnostics and the solution the analysis was performed on.
        /// </summary>
        [Obsolete("To be removed, no idea how this class got the name it has. Guessing refactoring accident.")]
        public class DiagnosticsWithMetadata
        {
            /// <summary>
            /// Initializes a new instance of the <see cref="DiagnosticsWithMetadata"/> class.
            /// </summary>
            /// <param name="solution">The solution the analysis was performed on.</param>
            /// <param name="diagnostics">The diagnostics returned from Roslyn.</param>
            public DiagnosticsWithMetadata(Solution solution, IReadOnlyList<ImmutableArray<Diagnostic>> diagnostics)
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
