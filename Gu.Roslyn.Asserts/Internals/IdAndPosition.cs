namespace Gu.Roslyn.Asserts.Internals
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using Microsoft.CodeAnalysis;

    /// <summary>
    /// Metadata about a diagnostic
    /// </summary>
    [DebuggerDisplay("{this.Id} {this.Span}")]
    internal struct IdAndPosition : IEquatable<IdAndPosition>
    {
        private IdAndPosition(string id, FileLinePositionSpan span)
        {
            this.Id = id;
            this.Span = span;
        }

        /// <summary>
        /// Gets the id of the diagnostic.
        /// </summary>
        public string Id { get; }

        /// <summary>
        /// Gets the position of the diagnostic.
        /// </summary>
        public FileLinePositionSpan Span { get; }

        public static bool operator ==(IdAndPosition left, IdAndPosition right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(IdAndPosition left, IdAndPosition right)
        {
            return !left.Equals(right);
        }

        /// <summary>
        /// Create an instance of <see cref="IdAndPosition"/>
        /// </summary>
        /// <returns>An instance of <see cref="IdAndPosition"/></returns>
        public static IdAndPosition Create(Diagnostic diagnostic)
        {
            return new IdAndPosition(diagnostic.Id, diagnostic.Location.GetMappedLineSpan());
        }

        /// <summary>
        /// Create an instance of <see cref="IdAndPosition"/>
        /// </summary>
        /// <returns>An instance of <see cref="IdAndPosition"/></returns>
        public static IdAndPosition Create(ExpectedDiagnostic x)
        {
            return new IdAndPosition(x.Id, x.Span);
        }

        /// <inheritdoc />
        public bool Equals(IdAndPosition other)
        {
            bool EndPositionsEquals(FileLinePositionSpan x, FileLinePositionSpan y)
            {
                if (x.StartLinePosition == x.EndLinePosition ||
                    y.StartLinePosition == y.EndLinePosition)
                {
                    return true;
                }

                return x.EndLinePosition == y.EndLinePosition;
            }

            return string.Equals(this.Id, other.Id) &&
                   string.Equals(this.Span.Path, other.Span.Path) &&
                   this.Span.StartLinePosition == other.Span.StartLinePosition &&
                   EndPositionsEquals(this.Span, other.Span);
        }

        /// <inheritdoc />
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
            {
                return false;
            }

            return obj is IdAndPosition && this.Equals((IdAndPosition)obj);
        }

        /// <inheritdoc />
        public override int GetHashCode()
        {
            unchecked
            {
                return (this.Id.GetHashCode() * 397) ^
                       this.Span.Path.GetHashCode() ^
                       this.Span.StartLinePosition.GetHashCode();
            }
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
            return $"{this.Id} at line {this.Span.StartLinePosition.Line} and character {this.Span.StartLinePosition.Character} in file {path} |{line}";
        }
    }
}