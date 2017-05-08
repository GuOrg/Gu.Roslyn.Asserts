namespace Gu.Roslyn.Asserts
{
    using System.Collections.Generic;
    using Microsoft.CodeAnalysis;

    internal class FileLinePositionSpanComparer : IComparer<FileLinePositionSpan>
    {
        public static readonly FileLinePositionSpanComparer Default = new FileLinePositionSpanComparer();

        public int Compare(FileLinePositionSpan x, FileLinePositionSpan y)
        {
            if (x.Path != y.Path)
            {
                return string.CompareOrdinal(x.Path, y.Path);
            }

            return x.StartLinePosition.CompareTo(y.StartLinePosition);
        }
    }
}