namespace Gu.Roslyn.Asserts.Tests.WithMetadataReferencesAttribute
{
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using Microsoft.CodeAnalysis;

    public class MetadataReferenceComparer : IComparer<MetadataReference>, IComparer
    {
        public static readonly MetadataReferenceComparer Default = new MetadataReferenceComparer();

        public int Compare(MetadataReference x, MetadataReference y)
        {
            if (ReferenceEquals(x, y))
            {
                return 0;
            }

            if (x == null || y == null)
            {
                return 1;
            }

            if (x.Display != y.Display)
            {
                return -1;
            }

            return x.Properties.Aliases.SequenceEqual(y.Properties.Aliases)
                ? 0
                : 1;
        }

        int IComparer.Compare(object x, object y) => this.Compare((MetadataReference)x, (MetadataReference)y);
    }
}