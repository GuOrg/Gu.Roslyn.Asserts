namespace Gu.Roslyn.Asserts.Internals
{
    using System.Collections.Generic;
    using Microsoft.CodeAnalysis;

    internal sealed class LocationComparer : IComparer<Location>
    {
        internal static readonly LocationComparer BySourceSpan = new LocationComparer();

        private LocationComparer()
        {
        }

        public int Compare(Location x, Location y)
        {
            if (object.ReferenceEquals(x, y))
            {
                return 0;
            }

            if (x == null)
            {
                return -1;
            }

            if (y == null)
            {
                return 1;
            }

            return x.SourceSpan.CompareTo(y.SourceSpan);
        }
    }
}
