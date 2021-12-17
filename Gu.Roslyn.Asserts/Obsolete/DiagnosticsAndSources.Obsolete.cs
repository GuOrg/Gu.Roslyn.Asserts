namespace Gu.Roslyn.Asserts
{
    using System;
    using System.Collections.Generic;
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
        public static DiagnosticsAndSources CreateFromCodeWithErrorsIndicated(DiagnosticAnalyzer analyzer, string code) => FromMarkup(analyzer, code);

        /// <summary>
        /// Get the expected diagnostics and cleaned sources.
        /// </summary>
        /// <param name="analyzer">The descriptor that is expected to produce diagnostics.</param>
        /// <param name="code">The code with errors indicated.</param>
        /// <returns>An instance of <see cref="DiagnosticsAndSources"/>.</returns>
        [Obsolete("Use FromMarkup()")]
        public static DiagnosticsAndSources CreateFromCodeWithErrorsIndicated(DiagnosticAnalyzer analyzer, IReadOnlyList<string> code) => FromMarkup(analyzer, code);

        /// <summary>
        /// Get the expected diagnostics and cleaned sources.
        /// </summary>
        /// <param name="descriptor">The descriptor that is expected to produce diagnostics.</param>
        /// <param name="code">The code with errors indicated.</param>
        /// <returns>An instance of <see cref="DiagnosticsAndSources"/>.</returns>
        [Obsolete("Use FromMarkup()")]
        public static DiagnosticsAndSources CreateFromCodeWithErrorsIndicated(DiagnosticDescriptor descriptor, IReadOnlyList<string> code) => FromMarkup(descriptor, code);

        /// <summary>
        /// Get the expected diagnostics and cleaned sources.
        /// </summary>
        /// <param name="analyzerId">The descriptor diagnosticId that is expected to produce diagnostics.</param>
        /// <param name="message">The expected message for the diagnostics, can be null.</param>
        /// <param name="code">The code with errors indicated.</param>
        /// <returns>An instance of <see cref="DiagnosticsAndSources"/>.</returns>
        [Obsolete("Use FromMarkup()")]
        public static DiagnosticsAndSources CreateFromCodeWithErrorsIndicated(string analyzerId, string? message, IReadOnlyList<string> code) => FromMarkup(analyzerId, message, code);
    }
}
