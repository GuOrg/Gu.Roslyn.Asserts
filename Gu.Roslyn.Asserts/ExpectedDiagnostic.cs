namespace Gu.Roslyn.Asserts
{
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.Diagnostics;

    /// <summary>
    /// Info about an expected diagnostic.
    /// </summary>
    public class ExpectedDiagnostic
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ExpectedDiagnostic"/> class.
        /// </summary>
        /// <param name="analyzer"> The analyzer that is expected to report a diagnostic.</param>
        /// <param name="span"> The position of the expected diagnostic.</param>
        public ExpectedDiagnostic(DiagnosticAnalyzer analyzer, FileLinePositionSpan span)
        {
            this.Analyzer = analyzer;
            this.Span = span;
        }

        /// <summary>
        /// Gets the analyzer that is expected to report a diagnostic.
        /// </summary>
        public DiagnosticAnalyzer Analyzer { get; }

        /// <summary>
        /// Gets the position of the expected diagnostic.
        /// </summary>
        public FileLinePositionSpan Span { get; }

        /// <summary>
        /// Get the expected diagnostics and cleaned sources.
        /// </summary>
        /// <param name="analyzer">The analyzer that is expected to produce diagnostics.</param>
        /// <param name="codeWithErrorsIndicated">The code with errors indicated.</param>
        /// <returns>An instance of <see cref="DiagnosticsAndSources"/>.</returns>
        public static DiagnosticsAndSources FromCode(DiagnosticAnalyzer analyzer, IReadOnlyList<string> codeWithErrorsIndicated)
        {
            var diagnostics = new List<ExpectedDiagnostic>();
            var cleanedSources = new List<string>();
            foreach (var source in codeWithErrorsIndicated)
            {
                var positions = CodeReader.FindDiagnosticsPositions(source).ToArray();
                if (positions.Length == 0)
                {
                    cleanedSources.Add(source);
                    continue;
                }

                cleanedSources.Add(source.Replace("↓", string.Empty));
                var fileName = CodeReader.FileName(source);
                diagnostics.AddRange(positions.Select(p => new ExpectedDiagnostic(analyzer, new FileLinePositionSpan(fileName, p, p))));
            }

            return new DiagnosticsAndSources(diagnostics, codeWithErrorsIndicated, cleanedSources);
        }

        /// <summary>
        /// Expected diagnostics and code.
        /// </summary>
        [SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Global", Justification = "For debugging.")]
        public class DiagnosticsAndSources
        {
            /// <summary>
            /// Initializes a new instance of the <see cref="DiagnosticsAndSources"/> class.
            /// </summary>
            /// <param name="expectedDiagnostics">The expected diagnostics that were indicated in the code.</param>
            /// <param name="codeWithErrorsIndicated">The code with positions for diagnostics indicated.</param>
            /// <param name="cleanedSources">The <paramref name="codeWithErrorsIndicated"/> with indicators removed.</param>
            public DiagnosticsAndSources(
                IReadOnlyList<ExpectedDiagnostic> expectedDiagnostics,
                IReadOnlyList<string> codeWithErrorsIndicated,
                IReadOnlyList<string> cleanedSources)
            {
                this.ExpectedDiagnostics = expectedDiagnostics;
                this.CodeWithErrorsIndicated = codeWithErrorsIndicated;
                this.CleanedSources = cleanedSources;
            }

            /// <summary>
            /// Gets the expected diagnostics that were indicated in the code.
            /// </summary>
            public IReadOnlyList<ExpectedDiagnostic> ExpectedDiagnostics { get; }

            /// <summary>
            /// Gets the code with positions for diagnostics indicated.
            /// </summary>
            public IReadOnlyList<string> CodeWithErrorsIndicated { get; }

            /// <summary>
            /// Gets the <see cref="CodeWithErrorsIndicated"/> with indicators removed.
            /// </summary>
            public IReadOnlyList<string> CleanedSources { get; }
        }
    }
}
