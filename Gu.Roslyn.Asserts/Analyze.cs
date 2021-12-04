namespace Gu.Roslyn.Asserts
{
    using System;
    using System.Collections.Generic;
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
        /// <param name="code">The sources as strings.</param>
        /// <param name="settings">The <see cref="Settings"/>.</param>
        /// <returns>A list with diagnostics per document.</returns>
        public static IReadOnlyList<ProjectDiagnostics> GetDiagnostics(DiagnosticAnalyzer analyzer, IReadOnlyList<string> code, Settings? settings = null)
        {
            var sln = CodeFactory.CreateSolution(code, settings ?? Settings.Default);
            return GetDiagnostics(analyzer, sln);
        }

        /// <summary>
        /// Creates a solution and adds the <paramref name="analyzer"/> as analyzer.
        /// Then compiles it and returns the diagnostics.
        /// </summary>
        /// <param name="analyzer">The analyzer to find diagnostics for.</param>
        /// <param name="code">The sources as strings.</param>
        /// <param name="settings">The <see cref="Settings"/>.</param>
        /// <returns>A list with diagnostics per document.</returns>
        public static IReadOnlyList<ProjectDiagnostics> GetDiagnostics(DiagnosticAnalyzer analyzer, string code, Settings? settings = null)
        {
            var sln = CodeFactory.CreateSolution(code, settings ?? Settings.Default);
            return GetDiagnostics(analyzer, sln);
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
        public static IReadOnlyList<ProjectDiagnostics> GetDiagnostics(DiagnosticAnalyzer analyzer, FileInfo code, Settings? settings = null)
        {
            var sln = CodeFactory.CreateSolution(code, settings);
            return GetDiagnostics(analyzer, sln);
        }

        /// <summary>
        /// Creates a solution, compiles it and returns the diagnostics.
        /// </summary>
        /// <param name="project">The project.</param>
        /// <param name="analyzer">The <see cref="DiagnosticAnalyzer"/> to check <paramref name="project"/> with.</param>
        /// <returns>A list with diagnostics per document.</returns>
        public static Task<ProjectDiagnostics> GetDiagnosticsAsync(DiagnosticAnalyzer analyzer, Project project)
        {
            if (project is null)
            {
                throw new ArgumentNullException(nameof(project));
            }

            if (analyzer is null)
            {
                throw new ArgumentNullException(nameof(analyzer));
            }

            return ProjectDiagnostics.CreateAsync(analyzer, project);
        }

        /// <summary>
        /// Creates a solution, compiles it and returns the diagnostics.
        /// </summary>
        /// <param name="project">The project.</param>
        /// <param name="analyzer">The <see cref="DiagnosticAnalyzer"/> to check <paramref name="project"/> with.</param>
        /// <returns>A list with diagnostics per document.</returns>
        public static ProjectDiagnostics GetDiagnostics(DiagnosticAnalyzer analyzer, Project project)
        {
            if (project is null)
            {
                throw new ArgumentNullException(nameof(project));
            }

            if (analyzer is null)
            {
                throw new ArgumentNullException(nameof(analyzer));
            }

            return ProjectDiagnostics.CreateAsync(analyzer, project).GetAwaiter().GetResult();
        }

        /// <summary>
        /// Creates a solution, compiles it and returns the diagnostics.
        /// </summary>
        /// <param name="solution">The solution.</param>
        /// <returns>A list with diagnostics per document.</returns>
        public static async Task<IReadOnlyList<Diagnostic>> GetAllDiagnosticsAsync(Solution solution)
        {
            if (solution is null)
            {
                throw new ArgumentNullException(nameof(solution));
            }

            var results = new List<Diagnostic>();
            foreach (var project in solution.Projects)
            {
                var compilation = await project.GetCompilationAsync(CancellationToken.None)
                                               .ConfigureAwait(false) ??
                                  throw new InvalidOperationException("project.GetCompilationAsync() returned null");
                results.AddRange(compilation.GetDiagnostics(CancellationToken.None));
            }

            return results;
        }

        /// <summary>
        /// Creates a solution, compiles it and returns all diagnostics.
        /// </summary>
        /// <param name="solution">The solution.</param>
        /// <returns>A list with diagnostics.</returns>
        public static IReadOnlyList<Diagnostic> GetAllDiagnostics(Solution solution)
        {
            if (solution is null)
            {
                throw new ArgumentNullException(nameof(solution));
            }

            var results = new List<Diagnostic>();
            foreach (var project in solution.Projects)
            {
                var compilation = project.GetCompilationAsync(CancellationToken.None).GetAwaiter().GetResult() ??
                                  throw new InvalidOperationException("project.GetCompilationAsync() returned null");
                results.AddRange(compilation.GetDiagnostics(CancellationToken.None));
            }

            return results;
        }

        /// <summary>
        /// Creates a solution, compiles it and returns the diagnostics.
        /// </summary>
        /// <param name="solution">The solution.</param>
        /// <param name="analyzer">The <see cref="DiagnosticAnalyzer"/> to check <paramref name="solution"/> with.</param>
        /// <returns>A list with diagnostics per document.</returns>
        public static async Task<IReadOnlyList<ProjectDiagnostics>> GetDiagnosticsAsync(DiagnosticAnalyzer analyzer, Solution solution)
        {
            if (solution is null)
            {
                throw new ArgumentNullException(nameof(solution));
            }

            if (analyzer is null)
            {
                throw new ArgumentNullException(nameof(analyzer));
            }

            var results = new List<ProjectDiagnostics>();
            foreach (var project in solution.Projects)
            {
                results.Add(await ProjectDiagnostics.CreateAsync(analyzer, project).ConfigureAwait(false));
            }

            return results;
        }

        /// <summary>
        /// Creates a solution, compiles it and returns the diagnostics.
        /// </summary>
        /// <param name="analyzer">The <see cref="DiagnosticAnalyzer"/> to check <paramref name="solution"/> with.</param>
        /// <param name="solution">The solution.</param>
        /// <returns>A list with diagnostics per document.</returns>
        public static IReadOnlyList<ProjectDiagnostics> GetDiagnostics(DiagnosticAnalyzer analyzer, Solution solution)
        {
            if (analyzer is null)
            {
                throw new ArgumentNullException(nameof(analyzer));
            }

            if (solution is null)
            {
                throw new ArgumentNullException(nameof(solution));
            }

            var results = new List<ProjectDiagnostics>();
            foreach (var project in solution.Projects)
            {
                results.Add(ProjectDiagnostics.CreateAsync(analyzer, project).GetAwaiter().GetResult());
            }

            return results;
        }
    }
}
