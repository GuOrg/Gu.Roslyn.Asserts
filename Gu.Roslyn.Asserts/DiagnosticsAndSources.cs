namespace Gu.Roslyn.Asserts
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.Diagnostics;

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
        /// <param name="code">The code to analyze.</param>
        public DiagnosticsAndSources(
            IReadOnlyList<ExpectedDiagnostic> expectedDiagnostics,
            IReadOnlyList<string> code)
        {
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
        /// <param name="code">The code with errors indicated.</param>
        /// <returns>An instance of <see cref="DiagnosticsAndSources"/>.</returns>
        public static DiagnosticsAndSources CreateFromCodeWithErrorsIndicated(DiagnosticAnalyzer analyzer, string code)
        {
            if (analyzer == null)
            {
                throw new ArgumentNullException(nameof(analyzer));
            }

            if (code == null)
            {
                throw new ArgumentNullException(nameof(code));
            }

            return CreateFromCodeWithErrorsIndicated(analyzer, new[] { code });
        }

        /// <summary>
        /// Get the expected diagnostics and cleaned sources.
        /// </summary>
        /// <param name="analyzer">The descriptor that is expected to produce diagnostics.</param>
        /// <param name="code">The code with errors indicated.</param>
        /// <returns>An instance of <see cref="DiagnosticsAndSources"/>.</returns>
        public static DiagnosticsAndSources CreateFromCodeWithErrorsIndicated(DiagnosticAnalyzer analyzer, IReadOnlyList<string> code)
        {
            if (analyzer == null)
            {
                throw new ArgumentNullException(nameof(analyzer));
            }

            if (code == null)
            {
                throw new ArgumentNullException(nameof(code));
            }

            if (analyzer.SupportedDiagnostics.Length > 1)
            {
                var message = "This can only be used for analyzers with one SupportedDiagnostics\r\n" +
                              "Prefer overload with ExpectedDiagnostic";
                throw new ArgumentException(message, nameof(analyzer));
            }

            return CreateFromCodeWithErrorsIndicated(analyzer.SupportedDiagnostics[0].Id, null, code);
        }

        /// <summary>
        /// Get the expected diagnostics and cleaned sources.
        /// </summary>
        /// <param name="descriptor">The descriptor that is expected to produce diagnostics.</param>
        /// <param name="code">The code with errors indicated.</param>
        /// <returns>An instance of <see cref="DiagnosticsAndSources"/>.</returns>
        public static DiagnosticsAndSources CreateFromCodeWithErrorsIndicated(DiagnosticDescriptor descriptor, IReadOnlyList<string> code)
        {
            if (descriptor == null)
            {
                throw new ArgumentNullException(nameof(descriptor));
            }

            if (code == null)
            {
                throw new ArgumentNullException(nameof(code));
            }

            return CreateFromCodeWithErrorsIndicated(descriptor.Id, null, code);
        }

        /// <summary>
        /// Get the expected diagnostics and cleaned sources.
        /// </summary>
        /// <param name="analyzerId">The descriptor diagnosticId that is expected to produce diagnostics.</param>
        /// <param name="message">The expected message for the diagnostics, can be null.</param>
        /// <param name="code">The code with errors indicated.</param>
        /// <returns>An instance of <see cref="DiagnosticsAndSources"/>.</returns>
        public static DiagnosticsAndSources CreateFromCodeWithErrorsIndicated(string analyzerId, string message, IReadOnlyList<string> code)
        {
            if (analyzerId == null)
            {
                throw new ArgumentNullException(nameof(analyzerId));
            }

            if (code == null)
            {
                throw new ArgumentNullException(nameof(code));
            }

            var diagnostics = new List<ExpectedDiagnostic>();
            var cleanedSources = new List<string>();
            foreach (var source in code)
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
        /// Either the <paramref name="expectedDiagnostic"/> or <paramref name="code"/> can have position or error position indicated but not both.
        /// </summary>
        /// <param name="expectedDiagnostic">The descriptor diagnosticId that is expected to produce diagnostics.</param>
        /// <param name="code">The code with errors indicated.</param>
        /// <returns>An instance of <see cref="DiagnosticsAndSources"/>.</returns>
        public static DiagnosticsAndSources Create(ExpectedDiagnostic expectedDiagnostic, string code)
        {
            if (expectedDiagnostic == null)
            {
                throw new ArgumentNullException(nameof(expectedDiagnostic));
            }

            if (code == null)
            {
                throw new ArgumentNullException(nameof(code));
            }

            return Create(expectedDiagnostic, new[] { code });
        }

        /// <summary>
        /// Get the expected diagnostics and cleaned sources.
        /// Either the <paramref name="expectedDiagnostic"/> or <paramref name="code"/> can have position or error position indicated but not both.
        /// </summary>
        /// <param name="expectedDiagnostic">The descriptor diagnosticId that is expected to produce diagnostics.</param>
        /// <param name="code">The code with errors indicated.</param>
        /// <returns>An instance of <see cref="DiagnosticsAndSources"/>.</returns>
        public static DiagnosticsAndSources Create(ExpectedDiagnostic expectedDiagnostic, IReadOnlyList<string> code)
        {
            if (expectedDiagnostic == null)
            {
                throw new ArgumentNullException(nameof(expectedDiagnostic));
            }

            if (code == null)
            {
                throw new ArgumentNullException(nameof(code));
            }

            if (HasErrorsIndicated(code))
            {
                if (expectedDiagnostic.HasPosition)
                {
                    var message = "Expected diagnostic has position indicated and the code has error position indicated with ↓\r\n" +
                                  "Use either:\r\n" +
                                  "a) Diagnostic with position and no error indicated in the code.\r\n" +
                                  "a) Diagnostic with position and no error indicated in the code.\r\n";
                    throw new InvalidOperationException(message);
                }

                return CreateFromCodeWithErrorsIndicated(expectedDiagnostic.Id, expectedDiagnostic.Message, code);
            }

            return new DiagnosticsAndSources(new[] { expectedDiagnostic }, code);
        }

        private static bool HasErrorsIndicated(IReadOnlyList<string> code)
        {
            foreach (var doc in code)
            {
                if (HasErrorsIndicated(doc))
                {
                    return true;
                }
            }

            return false;
        }

        private static bool HasErrorsIndicated(string code) => code.Contains('↓');
    }
}
