namespace Gu.Roslyn.Asserts
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Diagnostics.CodeAnalysis;
    using System.Globalization;
    using System.Linq;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.Diagnostics;
    using Microsoft.CodeAnalysis.Text;

    /// <summary>
    /// Info about an expected diagnostic.
    /// </summary>
    [DebuggerDisplay("{Id} {Message} {Span}")]
    public class ExpectedDiagnostic
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ExpectedDiagnostic"/> class.
        /// </summary>
        /// <param name="analyzer"> The analyzer that is expected to report a diagnostic.</param>
        /// <param name="span"> The position of the expected diagnostic.</param>
        [Obsolete("To be removed")]
        public ExpectedDiagnostic(DiagnosticAnalyzer analyzer, FileLinePositionSpan span)
        {
            this.Analyzer = analyzer;
            this.Message = null;
            this.Id = analyzer.SupportedDiagnostics[0].Id;
            this.Span = span;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ExpectedDiagnostic"/> class.
        /// </summary>
        /// <param name="id">The expected diagnostic ID, required.</param>
        /// <param name="message">The expected message, can be null. If null it is not checked in asserts.</param>
        /// <param name="span"> The position of the expected diagnostic.</param>
        public ExpectedDiagnostic(string id, string message, FileLinePositionSpan span)
        {
            this.Id = id ?? throw new ArgumentNullException(nameof(id));
            this.Message = message;
            this.Span = span;
        }

        /// <summary>
        /// Gets the expected diagnostic ID.
        /// </summary>
        public string Id { get; }

        /// <summary>
        /// Gets the expected message as text
        /// </summary>
        public string Message { get; }

        /// <summary>
        /// Gets the position of the expected diagnostic.
        /// </summary>
        public FileLinePositionSpan Span { get; }

        /// <summary>
        /// Gets the analyzer that is expected to report a diagnostic.
        /// </summary>
        [Obsolete("To be removed.")]
        public DiagnosticAnalyzer Analyzer { get; }

        public static ExpectedDiagnostic Create(string diagnosticId, int line, int character)
        {
            return Create(diagnosticId, null, line, character);
        }

        public static ExpectedDiagnostic Create(string diagnosticId, string message, int line, int character)
        {
            var position = new LinePosition(line, character);
            return new ExpectedDiagnostic(diagnosticId, message, new FileLinePositionSpan(null, position, position));
        }

        public static ExpectedDiagnostic Create(string diagnosticId, string codeWithErrorsIndicated, out string cleanedSources)
        {
            return Create(diagnosticId, null, codeWithErrorsIndicated, out cleanedSources);
        }

        public static ExpectedDiagnostic Create(string diagnosticId, string message, string codeWithErrorsIndicated, out string cleanedSources)
        {
            var positions = CodeReader.FindDiagnosticsPositions(codeWithErrorsIndicated).ToArray();
            if (positions.Length == 0)
            {
                throw new ArgumentException("Expected one error position indicated, was zero.", nameof(codeWithErrorsIndicated));
            }

            if (positions.Length > 1)
            {
                throw new ArgumentException($"Expected one error position indicated, was {positions.Length}.", nameof(codeWithErrorsIndicated));
            }

            cleanedSources = codeWithErrorsIndicated.Replace("↓", string.Empty);
            var fileName = CodeReader.FileName(codeWithErrorsIndicated);
            var position = positions[0];
            return new ExpectedDiagnostic(diagnosticId, message, new FileLinePositionSpan(fileName, position, position));
        }

        public static IReadOnlyList<ExpectedDiagnostic> CreateMany(string diagnosticId, string message, string codeWithErrorsIndicated, out string cleanedSources)
        {
            var positions = CodeReader.FindDiagnosticsPositions(codeWithErrorsIndicated).ToArray();
            if (positions.Length == 0)
            {
                throw new ArgumentException("Expected one error position indicated, was zero.", nameof(codeWithErrorsIndicated));
            }

            cleanedSources = codeWithErrorsIndicated.Replace("↓", string.Empty);
            var fileName = CodeReader.FileName(codeWithErrorsIndicated);
            return positions.Select(p => new ExpectedDiagnostic(diagnosticId, message, new FileLinePositionSpan(fileName, p, p)))
                            .ToArray();
        }

        /// <summary>
        /// Check if Id, Span and Message matches.
        /// If Message is nu it is not checked.
        /// </summary>
        public bool Matches(Diagnostic actual)
        {
            if (this.Id != actual.Id)
            {
                return false;
            }

            if (this.Message != null &&
                this.Message != actual.GetMessage(CultureInfo.InvariantCulture))
            {
                return false;
            }

            var actualSpan = actual.Location.GetMappedLineSpan();
            if (this.Span.StartLinePosition != actualSpan.StartLinePosition)
            {
                return false;
            }

            if (this.Span.Path != null &&
                this.Span.Path != actualSpan.Path)
            {
                return false;
            }

            if (this.Span.StartLinePosition != this.Span.EndLinePosition)
            {
                return this.Span.EndLinePosition == actualSpan.EndLinePosition;
            }

            return true;
        }

        /// <summary>
        /// Get the expected diagnostics and cleaned sources.
        /// </summary>
        /// <param name="analyzer">The analyzer that is expected to produce diagnostics.</param>
        /// <param name="codeWithErrorsIndicated">The code with errors indicated.</param>
        /// <returns>An instance of <see cref="DiagnosticsAndSources"/>.</returns>
        internal static DiagnosticsAndSources CreateDiagnosticsAndSources(DiagnosticAnalyzer analyzer, IReadOnlyList<string> codeWithErrorsIndicated)
        {
            if (analyzer.SupportedDiagnostics.Length > 1)
            {
                throw new ArgumentException("This can only be used for analyzers with one SupportedDiagnostics", nameof(analyzer));
            }

            return CreateDiagnosticsAndSources(analyzer.SupportedDiagnostics[0].Id, null, codeWithErrorsIndicated);
        }

        /// <summary>
        /// Get the expected diagnostics and cleaned sources.
        /// </summary>
        /// <param name="analyzerId">The analyzer diagnosticId that is expected to produce diagnostics.</param>
        /// <param name="message">The expected message for the diagnostics, can be null.</param>
        /// <param name="codeWithErrorsIndicated">The code with errors indicated.</param>
        /// <returns>An instance of <see cref="DiagnosticsAndSources"/>.</returns>
        internal static DiagnosticsAndSources CreateDiagnosticsAndSources(string analyzerId, string message, IReadOnlyList<string> codeWithErrorsIndicated)
        {
            if (analyzerId == null)
            {
                throw new ArgumentNullException(nameof(analyzerId));
            }

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
                diagnostics.AddRange(positions.Select(p => new ExpectedDiagnostic(analyzerId, message, new FileLinePositionSpan(fileName, p, p))));
            }

            return new DiagnosticsAndSources(diagnostics, codeWithErrorsIndicated, cleanedSources);
        }

        /// <summary>
        /// Writes the diagnostic and the offending code.
        /// </summary>
        /// <returns>A string for use in assert exception</returns>
        internal string ToString(IReadOnlyList<string> sources)
        {
            var path = this.Span.Path;
            var match = sources.SingleOrDefault(x => CodeReader.FileName(x) == path);
            var line = match != null ? CodeReader.GetLineWithErrorIndicated(match, this.Span.StartLinePosition) : string.Empty;
            return $"{this.Id} {this.Message}\r\n" +
                   $"  at line {this.Span.StartLinePosition.Line} and character {this.Span.StartLinePosition.Character} in file {this.Span.Path} | {line.TrimStart(' ')}";
        }

        /// <summary>
        /// Expected diagnostics and code.
        /// </summary>
        [SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Global", Justification = "For debugging.")]
        internal class DiagnosticsAndSources
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
