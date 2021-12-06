namespace Gu.Roslyn.Asserts
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.Diagnostics;

    /// <summary>
    /// Expected diagnostics and code.
    /// </summary>
    public partial class DiagnosticsAndSources
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DiagnosticsAndSources"/> class.
        /// </summary>
        /// <param name="expectedDiagnostics">The expected diagnostics that were indicated in the code.</param>
        /// <param name="code">The code to analyze.</param>
        public DiagnosticsAndSources(
            IReadOnlyList<ExpectedDiagnostic> expectedDiagnostics,
            IReadOnlyList<string> code)
        {
            if (expectedDiagnostics is null)
            {
                throw new ArgumentNullException(nameof(expectedDiagnostics));
            }

            if (code is null)
            {
                throw new ArgumentNullException(nameof(code));
            }

            if (code.Count > 1 &&
                expectedDiagnostics.Any(x => !x.HasPath))
            {
                throw new InvalidOperationException("Expected diagnostic must specify path when more than one document is tested.\r\n" +
                                                    "Either specify path or indicate expected error position with ↓");
            }

            this.ExpectedDiagnostics = expectedDiagnostics;
            this.Code = code;
        }

        /// <summary>
        /// Gets the expected diagnostics that were indicated in the code.
        /// </summary>
        public IReadOnlyList<ExpectedDiagnostic> ExpectedDiagnostics { get; }

        /// <summary>
        /// Gets the <see cref="Code"/> with to analyze.
        /// </summary>
        public IReadOnlyList<string> Code { get; }

        /// <summary>
        /// Get the expected diagnostics and cleaned sources.
        /// </summary>
        /// <param name="analyzer">The descriptor that is expected to produce diagnostics.</param>
        /// <param name="markup">The code with diagnostic positions indicated with ↓ (alt + 25).</param>
        /// <returns>An instance of <see cref="DiagnosticsAndSources"/>.</returns>
        public static DiagnosticsAndSources FromMarkup(DiagnosticAnalyzer analyzer, string markup)
        {
            if (analyzer is null)
            {
                throw new ArgumentNullException(nameof(analyzer));
            }

            if (markup is null)
            {
                throw new ArgumentNullException(nameof(markup));
            }

            return FromMarkup(analyzer, new[] { markup });
        }

        /// <summary>
        /// Get the expected diagnostics and cleaned sources.
        /// </summary>
        /// <param name="analyzer">The descriptor that is expected to produce diagnostics.</param>
        /// <param name="markup">The code with diagnostic positions indicated with ↓ (alt + 25).</param>
        /// <returns>An instance of <see cref="DiagnosticsAndSources"/>.</returns>
        public static DiagnosticsAndSources FromMarkup(DiagnosticAnalyzer analyzer, IReadOnlyList<string> markup)
        {
            if (analyzer is null)
            {
                throw new ArgumentNullException(nameof(analyzer));
            }

            if (markup is null)
            {
                throw new ArgumentNullException(nameof(markup));
            }

            RoslynAssert.VerifySingleSupportedDiagnostic(analyzer, out var descriptor);
            return FromMarkup(descriptor.Id, null, markup);
        }

        /// <summary>
        /// Get the expected diagnostics and cleaned sources.
        /// </summary>
        /// <param name="descriptor">The descriptor that is expected to produce diagnostics.</param>
        /// <param name="markup">The code with diagnostic positions indicated with ↓ (alt + 25).</param>
        /// <returns>An instance of <see cref="DiagnosticsAndSources"/>.</returns>
        public static DiagnosticsAndSources FromMarkup(DiagnosticDescriptor descriptor, IReadOnlyList<string> markup)
        {
            if (descriptor is null)
            {
                throw new ArgumentNullException(nameof(descriptor));
            }

            if (markup is null)
            {
                throw new ArgumentNullException(nameof(markup));
            }

            return FromMarkup(descriptor.Id, null, markup);
        }

        /// <summary>
        /// Get the expected diagnostics and cleaned sources.
        /// </summary>
        /// <param name="analyzerId">The descriptor diagnosticId that is expected to produce diagnostics.</param>
        /// <param name="message">The expected message for the diagnostics, can be null.</param>
        /// <param name="markup">The code with diagnostic positions indicated with ↓ (alt + 25).</param>
        /// <returns>An instance of <see cref="DiagnosticsAndSources"/>.</returns>
        public static DiagnosticsAndSources FromMarkup(string analyzerId, string? message, IReadOnlyList<string> markup)
        {
            if (analyzerId is null)
            {
                throw new ArgumentNullException(nameof(analyzerId));
            }

            if (markup is null)
            {
                throw new ArgumentNullException(nameof(markup));
            }

            var diagnostics = new List<ExpectedDiagnostic>();
            var cleanedSources = new List<string>();
            foreach (var source in markup)
            {
                var positions = CodeReader.FindLinePositions(source).ToArray();
                if (positions.Length == 0)
                {
                    cleanedSources.Add(source);
                    continue;
                }

                cleanedSources.Add(source.Replace("↓", string.Empty));
                var fileName = CodeReader.FileName(source);
                diagnostics.AddRange(positions.Select(p => new ExpectedDiagnostic(analyzerId, message, new FileLinePositionSpan(fileName, p, p))));
            }

            if (diagnostics.Count == 0)
            {
                throw new InvalidOperationException("Expected code to have at least one error position indicated with '↓'");
            }

            return new DiagnosticsAndSources(diagnostics, cleanedSources);
        }

        /// <summary>
        /// Get the expected diagnostics and cleaned sources.
        /// Either the <paramref name="expectedDiagnostic"/> or <paramref name="markup"/> can have position or error position indicated but not both.
        /// </summary>
        /// <param name="expectedDiagnostic">The descriptor diagnosticId that is expected to produce diagnostics.</param>
        /// <param name="markup">The code with diagnostic positions indicated with ↓ (alt + 25).</param>
        /// <returns>An instance of <see cref="DiagnosticsAndSources"/>.</returns>
        public static DiagnosticsAndSources Create(ExpectedDiagnostic expectedDiagnostic, string markup)
        {
            if (expectedDiagnostic is null)
            {
                throw new ArgumentNullException(nameof(expectedDiagnostic));
            }

            if (markup is null)
            {
                throw new ArgumentNullException(nameof(markup));
            }

            return Create(expectedDiagnostic, new[] { markup });
        }

        /// <summary>
        /// Get the expected diagnostics and cleaned sources.
        /// Either the <paramref name="expectedDiagnostic"/> or <paramref name="codeOrMarkup"/> can have position or error position indicated but not both.
        /// </summary>
        /// <param name="expectedDiagnostic">The descriptor diagnosticId that is expected to produce diagnostics.</param>
        /// <param name="codeOrMarkup">The code with errors indicated.</param>
        /// <returns>An instance of <see cref="DiagnosticsAndSources"/>.</returns>
        public static DiagnosticsAndSources Create(ExpectedDiagnostic expectedDiagnostic, IReadOnlyList<string> codeOrMarkup)
        {
            if (expectedDiagnostic is null)
            {
                throw new ArgumentNullException(nameof(expectedDiagnostic));
            }

            if (codeOrMarkup is null)
            {
                throw new ArgumentNullException(nameof(codeOrMarkup));
            }

            if (HasPositionsIndicated(codeOrMarkup))
            {
                if (expectedDiagnostic.HasPosition)
                {
                    var message = "Expected diagnostic has position indicated and the code has error position indicated with ↓\r\n" +
                                  "Use either:\r\n" +
                                  "a) Expected error position indicated with ↓ in the code.\r\n" +
                                  "b) ExpectedDiagnostic with expected error position.\r\n";
                    throw new InvalidOperationException(message);
                }

                return FromMarkup(expectedDiagnostic.Id, expectedDiagnostic.Message, codeOrMarkup);
            }

            return new DiagnosticsAndSources(new[] { expectedDiagnostic }, codeOrMarkup);
        }

        private static bool HasPositionsIndicated(IReadOnlyList<string> code)
        {
            foreach (var doc in code)
            {
                if (HasPositionsIndicated(doc))
                {
                    return true;
                }
            }

            return false;
        }

        private static bool HasPositionsIndicated(string code) => code.Contains('↓');
    }
}
