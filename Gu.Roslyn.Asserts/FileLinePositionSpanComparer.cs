namespace Gu.Roslyn.Asserts
{
    using System.Collections.Generic;
    using Microsoft.CodeAnalysis;

    /// <inheritdoc />
    internal class FileLinePositionSpanComparer : Comparer<FileLinePositionSpan>
    {
        /// <summary>
        /// The default instance.
        /// </summary>
        public new static readonly FileLinePositionSpanComparer Default = new FileLinePositionSpanComparer();

        /// <inheritdoc />
        public override int Compare(FileLinePositionSpan x, FileLinePositionSpan y)
        {
            if (x.Path != y.Path)
            {
                return string.CompareOrdinal(x.Path, y.Path);
            }

            return x.StartLinePosition.CompareTo(y.StartLinePosition);
        }
    }
}