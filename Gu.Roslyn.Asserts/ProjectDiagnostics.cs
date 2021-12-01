namespace Gu.Roslyn.Asserts
{
    using System;
    using System.Collections.Immutable;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;

    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.Diagnostics;

    /// <summary>
    /// Diagnostics for a project compiled with an analyzer.
    /// </summary>
    public class ProjectDiagnostics
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ProjectDiagnostics"/> class.
        /// </summary>
        /// <param name="project">The <see cref="Project"/>.</param>
        /// <param name="compilerDiagnostics">The compiler errors.</param>
        /// <param name="analyzerDiagnostics">The diagnostics for the analyzer.</param>
        public ProjectDiagnostics(Project project, ImmutableArray<Diagnostic> compilerDiagnostics, ImmutableArray<Diagnostic> analyzerDiagnostics)
        {
            this.Project = project;
            this.CompilerDiagnostics = compilerDiagnostics;
            this.AnalyzerDiagnostics = analyzerDiagnostics;
        }

        /// <summary>
        /// The <see cref="Project"/>.
        /// </summary>
        public Project Project { get; }

        /// <summary>
        /// Gets the compiler errors.
        /// </summary>
        public ImmutableArray<Diagnostic> CompilerDiagnostics { get; }

        /// <summary>
        /// Gets the diagnostics for the analyzer.
        /// </summary>
        public ImmutableArray<Diagnostic> AnalyzerDiagnostics { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="ProjectDiagnostics"/> class.
        /// </summary>
        /// <param name="project">The <see cref="Project"/>.</param>
        /// <param name="analyzer">The <see cref="DiagnosticAnalyzer"/></param>
        public static async Task<ProjectDiagnostics> CreateAsync(Project project, DiagnosticAnalyzer analyzer)
        {
            var compilation = await project.GetCompilationAsync(CancellationToken.None).ConfigureAwait(false) ??
                              throw new InvalidOperationException("project.GetCompilationAsync() returned null");

            if (analyzer is PlaceholderAnalyzer placeholder)
            {
                var diagnostics = compilation.GetDiagnostics(CancellationToken.None);
                return new ProjectDiagnostics(
                    project,
                    diagnostics.Where(x => placeholder.SupportedDiagnostics.All(d => d.Id != x.Id)).ToImmutableArray(),
                    diagnostics.Where(x => placeholder.SupportedDiagnostics.All(d => d.Id == x.Id)).ToImmutableArray());
            }
            else
            {
                var withAnalyzers = compilation.WithAnalyzers(
                    ImmutableArray.Create(analyzer),
                    project.AnalyzerOptions,
                    CancellationToken.None);
                var analyzerDiagnostics = await withAnalyzers.GetAnalyzerDiagnosticsAsync(CancellationToken.None).ConfigureAwait(false);
                return new ProjectDiagnostics(
                    project,
                    compilation.GetDiagnostics(CancellationToken.None),
                    analyzerDiagnostics);
            }
        }
    }
}
