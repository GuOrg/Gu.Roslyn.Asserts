namespace Gu.Roslyn.Asserts
{
    using System;
    using System.Collections.Generic;
    using System.Collections.Immutable;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;

    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CodeFixes;
    using Microsoft.CodeAnalysis.Diagnostics;

    /// <summary>
    /// Diagnostics for a project compiled with an analyzer.
    /// </summary>
    public class ProjectDiagnostics
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ProjectDiagnostics"/> class.
        /// </summary>
        /// <param name="analyzer">The <see cref="DiagnosticAnalyzer"/></param>
        /// <param name="project">The <see cref="Project"/>.</param>
        /// <param name="compilerDiagnostics">The compiler errors.</param>
        /// <param name="analyzerDiagnostics">The diagnostics for the analyzer.</param>
        public ProjectDiagnostics(DiagnosticAnalyzer analyzer, Project project, ImmutableArray<Diagnostic> compilerDiagnostics, ImmutableArray<Diagnostic> analyzerDiagnostics)
        {
            this.Analyzer = analyzer;
            this.Project = project;
            this.CompilerDiagnostics = compilerDiagnostics;
            this.AnalyzerDiagnostics = analyzerDiagnostics;
        }

        /// <summary>
        /// The <see cref="DiagnosticAnalyzer"/>.
        /// </summary>
        public DiagnosticAnalyzer Analyzer { get; }

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
        /// If both <see cref="CompilerDiagnostics"/> and <see cref="AnalyzerDiagnostics"/> are empty.
        /// </summary>
        public bool IsEmpty => this.CompilerDiagnostics.IsDefaultOrEmpty &&
                               this.AnalyzerDiagnostics.IsDefaultOrEmpty;

        /// <summary>
        /// Initializes a new instance of the <see cref="ProjectDiagnostics"/> class.
        /// </summary>
        /// <param name="analyzer">The <see cref="DiagnosticAnalyzer"/></param>
        /// <param name="project">The <see cref="Project"/>.</param>
        public static async Task<ProjectDiagnostics> CreateAsync(DiagnosticAnalyzer analyzer, Project project)
        {
            if (project is null)
            {
                throw new ArgumentNullException(nameof(project));
            }

            if (analyzer is null)
            {
                throw new ArgumentNullException(nameof(analyzer));
            }

            var compilation = await project.GetCompilationAsync(CancellationToken.None).ConfigureAwait(false) ??
                              throw new InvalidOperationException("project.GetCompilationAsync() returned null");

            if (analyzer is PlaceholderAnalyzer placeholder)
            {
                var diagnostics = compilation.GetDiagnostics(CancellationToken.None);
                return new ProjectDiagnostics(
                    analyzer,
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
                    analyzer,
                    project,
                    compilation.GetDiagnostics(CancellationToken.None),
                    analyzerDiagnostics);
            }
        }

        /// <summary>
        /// Gets <see cref="CompilerDiagnostics"/> and <see cref="AnalyzerDiagnostics"/>.
        /// </summary>
        public IEnumerable<Diagnostic> All()
        {
            foreach (var diagnostic in this.CompilerDiagnostics)
            {
                yield return diagnostic;
            }

            foreach (var diagnostic in this.AnalyzerDiagnostics)
            {
                yield return diagnostic;
            }
        }

        /// <summary>
        /// Gets <see cref="CompilerDiagnostics"/> filtered by <paramref name="allowedCompilerDiagnostics"/> and <see cref="AnalyzerDiagnostics"/>.
        /// </summary>
        internal IEnumerable<Diagnostic> Filter(AllowedCompilerDiagnostics allowedCompilerDiagnostics)
        {
            return allowedCompilerDiagnostics switch
            {
                AllowedCompilerDiagnostics.None => this.All(),
                AllowedCompilerDiagnostics.Warnings
                    => this.CompilerDiagnostics.Where(x => x.Severity == DiagnosticSeverity.Error)
                           .Concat(this.AnalyzerDiagnostics),
                AllowedCompilerDiagnostics.WarningsAndErrors => this.AnalyzerDiagnostics,
                _ => throw new ArgumentOutOfRangeException(nameof(allowedCompilerDiagnostics), allowedCompilerDiagnostics, null),
            };
        }

        /// <summary>
        /// Gets <see cref="CompilerDiagnostics"/> filtered by <paramref name="allowedCompilerDiagnostics"/> and <see cref="AnalyzerDiagnostics"/>.
        /// </summary>
        internal IEnumerable<Diagnostic> FilterCompilerDiagnostics(AllowedCompilerDiagnostics allowedCompilerDiagnostics)
        {
            return allowedCompilerDiagnostics switch
            {
                AllowedCompilerDiagnostics.None => this.CompilerDiagnostics,
                AllowedCompilerDiagnostics.Warnings
                    => this.CompilerDiagnostics.Where(x => x.Severity == DiagnosticSeverity.Error),
                AllowedCompilerDiagnostics.WarningsAndErrors => Enumerable.Empty<Diagnostic>(),
                _ => throw new ArgumentOutOfRangeException(nameof(allowedCompilerDiagnostics), allowedCompilerDiagnostics, null),
            };
        }

        internal IEnumerable<Diagnostic> FixableBy(CodeFixProvider fix)
        {
            foreach (var candidate in this.AnalyzerDiagnostics)
            {
                if (fix.FixableDiagnosticIds.Contains(candidate.Id))
                {
                    yield return candidate;
                }
            }

            foreach (var candidate in this.CompilerDiagnostics)
            {
                if (fix.FixableDiagnosticIds.Contains(candidate.Id))
                {
                    yield return candidate;
                }
            }
        }
    }
}
