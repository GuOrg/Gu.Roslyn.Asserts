namespace Gu.Roslyn.Asserts
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Globalization;
    using System.Linq;
    using Gu.Roslyn.Asserts.Internals;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.Diagnostics;
    using Microsoft.CodeAnalysis.Text;

    /// <summary>
    /// Info about an expected diagnostic.
    /// </summary>
    [DebuggerDisplay("{Id} {Message} {Span}")]
    public class ExpectedDiagnostic
    {
        private static readonly FileLinePositionSpan NoPosition = new FileLinePositionSpan("MISSING", default(LinePosition), default(LinePosition));

        /// <summary>
        /// Initializes a new instance of the <see cref="ExpectedDiagnostic"/> class.
        /// </summary>
        /// <param name="analyzer"> The analyzer that is expected to report a diagnostic.</param>
        /// <param name="span"> The position of the expected diagnostic.</param>
        [Obsolete("To be removed use factory methods.")]
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
        /// Gets the expected message as text.
        /// </summary>
        public string Message { get; }

        /// <summary>
        /// Gets the position of the expected diagnostic.
        /// </summary>
        public FileLinePositionSpan Span { get; }

        /// <summary>
        /// Gets a value indicating whether the <see cref="Span"/> has path specified.
        /// If the test is for a single file path can be omitted and will be set to 'MISSING'.
        /// </summary>
        public bool HasPath => this.Span.Path != null &&
                               this.Span.Path != NoPosition.Path;

        /// <summary>
        /// Gets a value indicating whether this instance indicates error position.
        /// </summary>
        public bool HasPosition => this.Span.StartLinePosition != NoPosition.StartLinePosition ||
                                   this.Span.EndLinePosition != NoPosition.EndLinePosition ||
                                   this.Span.Path != NoPosition.Path;

        /// <summary>
        /// Gets the analyzer that is expected to report a diagnostic.
        /// </summary>
        [Obsolete("To be removed.")]
        public DiagnosticAnalyzer Analyzer { get; }

        /// <summary>
        /// Create a new instance of <see cref="ExpectedDiagnostic"/> and use the id from <paramref name="descriptor"/>.
        /// </summary>
        /// <param name="descriptor">The expected diagnostic id.</param>
        /// <returns>A new instance of <see cref="ExpectedDiagnostic"/>.</returns>
        public static ExpectedDiagnostic Create(DiagnosticDescriptor descriptor)
        {
            return new ExpectedDiagnostic(descriptor.Id, null, NoPosition);
        }

        /// <summary>
        /// Create a new instance of <see cref="ExpectedDiagnostic"/>.
        /// </summary>
        /// <param name="diagnosticId">The expected diagnostic id.</param>
        /// <returns>A new instance of <see cref="ExpectedDiagnostic"/>.</returns>
        public static ExpectedDiagnostic Create(string diagnosticId)
        {
            return new ExpectedDiagnostic(diagnosticId, null, NoPosition);
        }

        /// <summary>
        /// Create a new instance of <see cref="ExpectedDiagnostic"/>.
        /// </summary>
        /// <param name="diagnosticId">The expected diagnostic id.</param>
        /// <param name="message">The expected message.</param>
        /// <returns>A new instance of <see cref="ExpectedDiagnostic"/>.</returns>
        public static ExpectedDiagnostic Create(string diagnosticId, string message)
        {
            return new ExpectedDiagnostic(diagnosticId, message, NoPosition);
        }

        /// <summary>
        /// Create a new instance of <see cref="ExpectedDiagnostic"/>.
        /// </summary>
        /// <param name="diagnosticId">The expected diagnostic id.</param>
        /// <param name="line">The expected line number.</param>
        /// <param name="character">The expected character position.</param>
        /// <returns>A new instance of <see cref="ExpectedDiagnostic"/>.</returns>
        public static ExpectedDiagnostic Create(string diagnosticId, int line, int character)
        {
            return Create(diagnosticId, null, line, character);
        }

        /// <summary>
        /// Create a new instance of <see cref="ExpectedDiagnostic"/>.
        /// </summary>
        /// <param name="diagnosticId">The expected diagnostic id.</param>
        /// <param name="message">The expected message.</param>
        /// <param name="line">The expected line number.</param>
        /// <param name="character">The expected character position.</param>
        /// <returns>A new instance of <see cref="ExpectedDiagnostic"/>.</returns>
        public static ExpectedDiagnostic Create(string diagnosticId, string message, int line, int character)
        {
            var position = new LinePosition(line, character);
            return new ExpectedDiagnostic(diagnosticId, message, new FileLinePositionSpan(NoPosition.Path, position, position));
        }

        /// <summary>
        /// Create a new instance of <see cref="ExpectedDiagnostic"/>.
        /// </summary>
        /// <param name="diagnosticId">The expected diagnostic id.</param>
        /// <param name="message">The expected message.</param>
        /// <param name="path">The path of the file with the diagnostic.</param>
        /// <param name="line">The expected line number.</param>
        /// <param name="character">The expected character position.</param>
        /// <returns>A new instance of <see cref="ExpectedDiagnostic"/>.</returns>
        public static ExpectedDiagnostic Create(string diagnosticId, string message, string path, int line, int character)
        {
            var position = new LinePosition(line, character);
            return new ExpectedDiagnostic(diagnosticId, message, new FileLinePositionSpan(path, position, position));
        }

        /// <summary>
        /// Create a new instance of <see cref="ExpectedDiagnostic"/>.
        /// </summary>
        /// <param name="diagnosticId">The expected diagnostic id.</param>
        /// <param name="code">The code with error position indicated..</param>
        /// <param name="cleanedSources"><paramref name="code"/> without error indicator.</param>
        /// <returns>A new instance of <see cref="ExpectedDiagnostic"/>.</returns>
        public static ExpectedDiagnostic CreateFromCodeWithErrorsIndicated(string diagnosticId, string code, out string cleanedSources)
        {
            return CreateFromCodeWithErrorsIndicated(diagnosticId, null, code, out cleanedSources);
        }

        /// <summary>
        /// Create a new instance of <see cref="ExpectedDiagnostic"/>.
        /// </summary>
        /// <param name="diagnosticId">The expected diagnostic id.</param>
        /// <param name="message">The expected message.</param>
        /// <param name="code">The code with error position indicated..</param>
        /// <param name="cleanedSources"><paramref name="code"/> without error indicator.</param>
        /// <returns>A new instance of <see cref="ExpectedDiagnostic"/>.</returns>
        public static ExpectedDiagnostic CreateFromCodeWithErrorsIndicated(string diagnosticId, string message, string code, out string cleanedSources)
        {
            var positions = CodeReader.FindLinePositions(code).ToArray();
            if (positions.Length == 0)
            {
                throw new ArgumentException("Expected one error position indicated, was zero.", nameof(code));
            }

            if (positions.Length > 1)
            {
                throw new ArgumentException($"Expected one error position indicated, was {positions.Length}.", nameof(code));
            }

            cleanedSources = code.Replace("↓", string.Empty);
            var fileName = CodeReader.FileName(code);
            var position = positions[0];
            return new ExpectedDiagnostic(diagnosticId, message, new FileLinePositionSpan(fileName, position, position));
        }

        /// <summary>
        /// Create a new instance of <see cref="ExpectedDiagnostic"/>.
        /// </summary>
        /// <param name="diagnosticId">The expected diagnostic id.</param>
        /// <param name="message">The expected message.</param>
        /// <param name="codeWithErrorsIndicated">The code with error position indicated..</param>
        /// <param name="cleanedSources"><paramref name="codeWithErrorsIndicated"/> without errors indicated.</param>
        /// <returns>A new instance of <see cref="ExpectedDiagnostic"/>.</returns>
        public static IReadOnlyList<ExpectedDiagnostic> CreateManyFromCodeWithErrorsIndicated(string diagnosticId, string message, string codeWithErrorsIndicated, out string cleanedSources)
        {
            var positions = CodeReader.FindLinePositions(codeWithErrorsIndicated).ToArray();
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
            return this.Id == actual.Id &&
                   this.MessageMatches(actual) &&
                   this.PositionMatches(actual);
        }

        /// <summary>
        /// Check if message matches if <see cref="Message"/> is not null.
        /// </summary>
        /// <param name="actual">The actual diagnostic.</param>
        /// <returns>True if match.</returns>
        public bool MessageMatches(Diagnostic actual)
        {
            return this.Message == null ||
                   this.Message == actual.GetMessage(CultureInfo.InvariantCulture);
        }

        /// <summary>
        /// Check if position matches if specified.
        /// </summary>
        /// <param name="actual">The actual diagnostic.</param>
        /// <returns>True if match.</returns>
        public bool PositionMatches(Diagnostic actual)
        {
            if (!this.HasPosition)
            {
                return true;
            }

            var actualSpan = actual.Location.GetMappedLineSpan();
            if (this.Span.StartLinePosition != actualSpan.StartLinePosition)
            {
                return false;
            }

            if (this.HasPath &&
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
        /// Get a clone of this instance with updated <see cref="Message"/>.
        /// </summary>
        /// <param name="message">The expected message.</param>
        /// <returns>A new <see cref="ExpectedDiagnostic"/>.</returns>
        public ExpectedDiagnostic WithMessage(string message) => new ExpectedDiagnostic(this.Id, message, this.Span);

        /// <summary>
        /// Get a clone of this instance with updated <see cref="Span"/>.
        /// </summary>
        /// <param name="span">The expected position.</param>
        /// <returns>A new <see cref="ExpectedDiagnostic"/>.</returns>
        public ExpectedDiagnostic WithPosition(FileLinePositionSpan span) => new ExpectedDiagnostic(this.Id, this.Message, span);

        /// <summary>
        /// Create a new instance of <see cref="ExpectedDiagnostic"/> with position.
        /// </summary>
        /// <param name="codeWithErrorsIndicated">The code with error position indicated..</param>
        /// <param name="cleanedSources"><paramref name="codeWithErrorsIndicated"/> without errors indicated.</param>
        /// <returns>A new instance of <see cref="ExpectedDiagnostic"/>.</returns>
        public ExpectedDiagnostic WithPositionFromCodeWithErrorsIndicated(string codeWithErrorsIndicated, out string cleanedSources)
        {
            var positions = CodeReader.FindLinePositions(codeWithErrorsIndicated).ToArray();
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
            return new ExpectedDiagnostic(this.Id, this.Message, new FileLinePositionSpan(fileName, position, position));
        }

        /// <summary>
        /// Writes the diagnostic and the offending code.
        /// </summary>
        /// <returns>A string for use in assert exception.</returns>
        internal string ToString(IReadOnlyList<string> sources)
        {
            if (this.HasPosition)
            {
                var path = this.HasPath ? this.Span.Path : CodeReader.FileName(sources.Single());
                var match = sources.FirstOrDefault(x => CodeReader.FileName(x) == path && this.Span.Span.ExistsIn(x));
                var line = match != null ? CodeReader.GetLineWithErrorIndicated(match, this.Span.StartLinePosition) : string.Empty;
                return $"{this.Id} {this.Message}\r\n" +
                       $"  at line {this.Span.StartLinePosition.Line} and character {this.Span.StartLinePosition.Character} in file {path} | {line.TrimStart(' ')}";
            }

            return $"{this.Id} {this.Message}";
        }
    }
}
