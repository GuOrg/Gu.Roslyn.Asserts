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
        /// <param name="code">The sources as strings.</param>
        /// <param name="settings">The <see cref="Settings"/>.</param>
        /// <returns>A list with diagnostics per document.</returns>
        public static IReadOnlyList<ImmutableArray<Diagnostic>> GetDiagnostics(DiagnosticAnalyzer analyzer, IReadOnlyList<string> code, Settings? settings = null)
        {
            var sln = CodeFactory.CreateSolution(code, settings ?? Settings.Default);
            return GetDiagnostics(sln, analyzer);
        }

        /// <summary>
        /// Creates a solution and adds the <paramref name="analyzer"/> as analyzer.
        /// Then compiles it and returns the diagnostics.
        /// </summary>
        /// <param name="analyzer">The analyzer to find diagnostics for.</param>
        /// <param name="code">The sources as strings.</param>
        /// <param name="settings">The <see cref="Settings"/>.</param>
        /// <returns>A list with diagnostics per document.</returns>
        public static IReadOnlyList<ImmutableArray<Diagnostic>> GetDiagnostics(DiagnosticAnalyzer analyzer, string code, Settings? settings = null)
        {
            var sln = CodeFactory.CreateSolution(code, settings ?? Settings.Default);
            return GetDiagnostics(sln, analyzer);
        }

        /// <summary>
        /// Creates a solution and adds the <paramref name="analyzer"/> as analyzer.
        /// Then compiles it and returns the diagnostics.
        /// </summary>
        /// <param name="analyzer">The analyzer to find diagnostics for.</param>
        /// <param name="code">
        /// The code to create the solution from.
        /// Can be a .cs, .csproj or .solution file.
        /// </param>
        /// <param name="settings">The <see cref="Settings"/>.</param>
        /// <returns>A list with diagnostics per document.</returns>
        public static IReadOnlyList<ImmutableArray<Diagnostic>> GetDiagnostics(DiagnosticAnalyzer analyzer, FileInfo code, Settings? settings = null)
        {
            var sln = CodeFactory.CreateSolution(code, settings);
            return GetDiagnostics(sln, analyzer);
        }

        /// <summary>
        /// Creates a solution, compiles it and returns the diagnostics.
        /// </summary>
        /// <param name="solution">The solution.</param>
        /// <returns>A list with diagnostics per document.</returns>
        public static async Task<IReadOnlyList<ImmutableArray<Diagnostic>>> GetDiagnosticsAsync(Solution solution)
        {
            if (solution is null)
            {
                throw new ArgumentNullException(nameof(solution));
            }

            var results = new List<ImmutableArray<Diagnostic>>();
            foreach (var project in solution.Projects)
            {
                var compilation = await project.GetCompilationAsync(CancellationToken.None)
                                               .ConfigureAwait(false);
                results.Add(compilation!.GetDiagnostics(CancellationToken.None));
            }

            return results;
        }

        /// <summary>
        /// Creates a solution, compiles it and returns the diagnostics.
        /// </summary>
        /// <param name="project">The project.</param>
        /// <param name="analyzer">The <see cref="DiagnosticAnalyzer"/> to check <paramref name="project"/> with.</param>
        /// <returns>A list with diagnostics per document.</returns>
        public static Task<ProjectDiagnostics> GetDiagnosticsAsync(Project project, DiagnosticAnalyzer analyzer)
        {
            if (project is null)
            {
                throw new ArgumentNullException(nameof(project));
            }

            if (analyzer is null)
            {
                throw new ArgumentNullException(nameof(analyzer));
            }

            return ProjectDiagnostics.CreateAsync(project, analyzer);
        }

        /// <summary>
        /// Creates a solution, compiles it and returns the diagnostics.
        /// </summary>
        /// <param name="solution">The solution.</param>
        /// <returns>A list with diagnostics per document.</returns>
        public static IReadOnlyList<ImmutableArray<Diagnostic>> GetDiagnostics(Solution solution)
        {
            if (solution is null)
            {
                throw new ArgumentNullException(nameof(solution));
            }

            var results = new List<ImmutableArray<Diagnostic>>();
            foreach (var project in solution.Projects)
            {
                var compilation = project.GetCompilationAsync(CancellationToken.None).GetAwaiter().GetResult();
                results.Add(compilation!.GetDiagnostics(CancellationToken.None));
            }

            return results;
        }

        /// <summary>
        /// Creates a solution, compiles it and returns the diagnostics fixable by <paramref name="fix"/>.
        /// </summary>
        /// <param name="solution">The solution.</param>
        /// <param name="analyzer">The <see cref="DiagnosticAnalyzer"/> to check <paramref name="solution"/> with.</param>
        /// <param name="fix">The code fix to use when filtering the diagnostics.</param>
        /// <returns>A list with all fixable diagnostics.</returns>
        public static async Task<IReadOnlyList<Diagnostic>> GetFixableDiagnosticsAsync(Solution solution, DiagnosticAnalyzer analyzer, CodeFixProvider fix)
        {
            if (solution is null)
            {
                throw new ArgumentNullException(nameof(solution));
            }

            if (analyzer is null)
            {
                throw new ArgumentNullException(nameof(analyzer));
            }

            var fixableDiagnostics = new List<Diagnostic>();
            foreach (var project in solution.Projects)
            {
                var compilation = await project.GetCompilationAsync(CancellationToken.None)
                                               .ConfigureAwait(false);
                if (analyzer is PlaceholderAnalyzer)
                {
                    fixableDiagnostics.AddRange(compilation!.GetDiagnostics(CancellationToken.None).Where(d => fix.FixableDiagnosticIds.Contains(d.Id)));
                }
                else
                {
                    var withAnalyzers = compilation!.WithAnalyzers(
                        ImmutableArray.Create(analyzer),
                        project.AnalyzerOptions,
                        CancellationToken.None);
                    var diagnostics = await withAnalyzers.GetAnalyzerDiagnosticsAsync(CancellationToken.None)
                                                         .ConfigureAwait(false);
                    fixableDiagnostics.AddRange(diagnostics.Where(d => fix.FixableDiagnosticIds.Contains(d.Id)));
                }
            }

            return fixableDiagnostics;
        }

        /// <summary>
        /// Creates a solution, compiles it and returns the diagnostics.
        /// </summary>
        /// <param name="solution">The solution.</param>
        /// <param name="analyzer">The <see cref="DiagnosticAnalyzer"/> to check <paramref name="solution"/> with.</param>
        /// <returns>A list with diagnostics per document.</returns>
        public static async Task<IReadOnlyList<ImmutableArray<Diagnostic>>> GetDiagnosticsAsync(Solution solution, DiagnosticAnalyzer analyzer)
        {
            if (solution is null)
            {
                throw new ArgumentNullException(nameof(solution));
            }

            if (analyzer is null)
            {
                throw new ArgumentNullException(nameof(analyzer));
            }

            var results = new List<ImmutableArray<Diagnostic>>();
            foreach (var project in solution.Projects)
            {
                var compilation = await project.GetCompilationAsync(CancellationToken.None)
                                               .ConfigureAwait(false);
                if (analyzer is PlaceholderAnalyzer)
                {
                    results.Add(compilation!.GetDiagnostics(CancellationToken.None));
                }
                else
                {
                    var withAnalyzers = compilation!.WithAnalyzers(
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
        /// <param name="analyzer">The <see cref="DiagnosticAnalyzer"/> to check <paramref name="solution"/> with.</param>
        /// <returns>A list with diagnostics per document.</returns>
        public static IReadOnlyList<ImmutableArray<Diagnostic>> GetDiagnostics(Solution solution, DiagnosticAnalyzer analyzer)
        {
            return GetDiagnostics(analyzer, solution);
        }

        /// <summary>
        /// Creates a solution, compiles it and returns the diagnostics.
        /// </summary>
        /// <param name="analyzer">The <see cref="DiagnosticAnalyzer"/> to check <paramref name="solution"/> with.</param>
        /// <param name="solution">The solution.</param>
        /// <returns>A list with diagnostics per document.</returns>
        public static IReadOnlyList<ImmutableArray<Diagnostic>> GetDiagnostics(DiagnosticAnalyzer analyzer, Solution solution)
        {
            if (analyzer is null)
            {
                throw new ArgumentNullException(nameof(analyzer));
            }

            if (solution is null)
            {
                throw new ArgumentNullException(nameof(solution));
            }

            var results = new List<ImmutableArray<Diagnostic>>();
            foreach (var project in solution.Projects)
            {
                var compilation = project.GetCompilationAsync(CancellationToken.None).GetAwaiter().GetResult();
                if (analyzer is PlaceholderAnalyzer)
                {
                    results.Add(compilation!.GetDiagnostics(CancellationToken.None));
                }
                else
                {
                    var withAnalyzers = compilation!.WithAnalyzers(
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
        /// <param name="analyzer">The <see cref="DiagnosticAnalyzer"/> to check <paramref name="project"/> with.</param>
        /// <returns>A list with diagnostics per document.</returns>
        public static IReadOnlyList<ImmutableArray<Diagnostic>> GetDiagnostics(Project project, DiagnosticAnalyzer analyzer)
        {
            if (project is null)
            {
                throw new ArgumentNullException(nameof(project));
            }

            if (analyzer is null)
            {
                throw new ArgumentNullException(nameof(analyzer));
            }

            var results = new List<ImmutableArray<Diagnostic>>();
            var compilation = project.GetCompilationAsync(CancellationToken.None).GetAwaiter().GetResult();
            if (analyzer is PlaceholderAnalyzer)
            {
                results.Add(compilation!.GetDiagnostics(CancellationToken.None));
            }
            else
            {
                var withAnalyzers = compilation!.WithAnalyzers(
                    ImmutableArray.Create(analyzer),
                    project.AnalyzerOptions,
                    CancellationToken.None);
                results.Add(withAnalyzers.GetAnalyzerDiagnosticsAsync(CancellationToken.None).GetAwaiter().GetResult());
            }

            return results;
        }

        /// <summary>
        /// Get the diagnostics for <paramref name="analyzer"/> and compiler errors.
        /// </summary>
        /// <param name="analyzer">The <see cref="DiagnosticAnalyzer"/>.</param>
        /// <param name="solution">The <see cref="Solution"/>.</param>
        /// <returns>The <see cref="DiagnosticsAndErrors"/>.</returns>
        internal static DiagnosticsAndErrors GetDiagnosticsAndErrors(DiagnosticAnalyzer analyzer, Solution solution)
        {
            var errors = new List<ImmutableArray<Diagnostic>>();
            var analyzerDiagnostics = new List<ImmutableArray<Diagnostic>>();

            foreach (var project in solution.Projects)
            {
                var compilation = project.GetCompilationAsync(CancellationToken.None).GetAwaiter().GetResult() ??
                                  throw new InvalidOperationException("project.GetCompilationAsync() returned null");

                if (analyzer is PlaceholderAnalyzer placeholder)
                {
                    var diagnostics = compilation.GetDiagnostics(CancellationToken.None);
                    errors.Add(diagnostics.Where(x => placeholder.SupportedDiagnostics.All(d => d.Id != x.Id)).ToImmutableArray());
                    analyzerDiagnostics.Add(diagnostics.Where(x => placeholder.SupportedDiagnostics.All(d => d.Id == x.Id)).ToImmutableArray());
                }
                else
                {
                    errors.Add(compilation.GetDiagnostics(CancellationToken.None));
                    var withAnalyzers = compilation.WithAnalyzers(
                        ImmutableArray.Create(analyzer),
                        project.AnalyzerOptions,
                        CancellationToken.None);
                    analyzerDiagnostics.Add(withAnalyzers.GetAnalyzerDiagnosticsAsync(CancellationToken.None).GetAwaiter().GetResult());
                }
            }

            return new DiagnosticsAndErrors(errors, analyzerDiagnostics);
        }

        /// <summary>
        /// Diagnostics for the analyzer and compiler errors.
        /// </summary>
        internal class DiagnosticsAndErrors
        {
            /// <summary>
            /// Gets the compiler errors.
            /// </summary>
            internal readonly IReadOnlyList<ImmutableArray<Diagnostic>> Errors;

            /// <summary>
            /// Gets the diagnostics for the analyzer.
            /// </summary>
            internal readonly IReadOnlyList<ImmutableArray<Diagnostic>> AnalyzerDiagnostics;

            /// <summary>
            /// Initializes a new instance of the <see cref="DiagnosticsAndErrors"/> class.
            /// </summary>
            /// <param name="errors">The compiler errors.</param>
            /// <param name="analyzerDiagnostics">The diagnostics for the analyzer.</param>
            internal DiagnosticsAndErrors(IReadOnlyList<ImmutableArray<Diagnostic>> errors, IReadOnlyList<ImmutableArray<Diagnostic>> analyzerDiagnostics)
            {
                this.Errors = errors;
                this.AnalyzerDiagnostics = analyzerDiagnostics;
            }
        }
    }
}
