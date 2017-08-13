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
            if (x.Display != y.Display)
            {
                return -1;
            }

            return x.Properties.Aliases.SequenceEqual(y.Properties.Aliases)
                ? 0
                : 1;
        }

        int IComparer.Compare(object x, object y) => Compare((MetadataReference) x, (MetadataReference) y);
    }
}