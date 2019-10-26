namespace Gu.Roslyn.Asserts.Internals
{
    using System.Collections.Generic;
    using Microsoft.CodeAnalysis;

    /// <inheritdoc />
    internal sealed class LocationComparer : IComparer<Location>
    {
        /// <summary>
        /// The default instance.
        /// </summary>
        internal static readonly LocationComparer BySourceSpan = new LocationComparer();

        private LocationComparer()
        {
        }

        /// <inheritdoc />
        public int Compare(Location x, Location y)
        {
            if (ReferenceEquals(x, y))
            {
                return 0;
            }

            if (x is null)
            {
                return -1;
            }

            if (y is null)
            {
                return 1;
            }

            return x.SourceSpan.CompareTo(y.SourceSpan);
        }
    }
}
