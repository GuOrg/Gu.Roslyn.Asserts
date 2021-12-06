namespace Gu.Roslyn.Asserts
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.Diagnostics;

    public partial class DiagnosticsAndSources
    {

        /// <summary>
        /// Get the expected diagnostics and cleaned sources.
        /// </summary>
        /// <param name="analyzer">The descriptor that is expected to produce diagnostics.</param>
        /// <param name="code">The code with errors indicated.</param>
        /// <returns>An instance of <see cref="DiagnosticsAndSources"/>.</returns>
        [Obsolete("Use FromMarkup()")]
        public static DiagnosticsAndSources CreateFromCodeWithErrorsIndicated(DiagnosticAnalyzer analyzer, string code)
        {
            if (analyzer is null)
            {
                throw new ArgumentNullException(nameof(analyzer));
            }

            if (code is null)
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
        [Obsolete("Use FromMarkup()")]
        public static DiagnosticsAndSources CreateFromCodeWithErrorsIndicated(DiagnosticAnalyzer analyzer, IReadOnlyList<string> code)
        {
            if (analyzer is null)
            {
                throw new ArgumentNullException(nameof(analyzer));
            }

            if (code is null)
            {
                throw new ArgumentNullException(nameof(code));
            }

            RoslynAssert.VerifySingleSupportedDiagnostic(analyzer, out var descriptor);
            return CreateFromCodeWithErrorsIndicated(descriptor.Id, null, code);
        }

        /// <summary>
        /// Get the expected diagnostics and cleaned sources.
        /// </summary>
        /// <param name="descriptor">The descriptor that is expected to produce diagnostics.</param>
        /// <param name="code">The code with errors indicated.</param>
        /// <returns>An instance of <see cref="DiagnosticsAndSources"/>.</returns>
        [Obsolete("Use FromMarkup()")]
        public static DiagnosticsAndSources CreateFromCodeWithErrorsIndicated(DiagnosticDescriptor descriptor, IReadOnlyList<string> code)
        {
            if (descriptor is null)
            {
                throw new ArgumentNullException(nameof(descriptor));
            }

            if (code is null)
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
        [Obsolete("Use FromMarkup()")]
        public static DiagnosticsAndSources CreateFromCodeWithErrorsIndicated(string analyzerId, string? message, IReadOnlyList<string> code)
        {
            if (analyzerId is null)
            {
                throw new ArgumentNullException(nameof(analyzerId));
            }

            if (code is null)
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

    }
}
