namespace Gu.Roslyn.Asserts
{
    using System;
    using System.Collections.Generic;
    using Microsoft.CodeAnalysis;

    /// <summary>
    /// An <see cref="IEqualityComparer{T}"/> for <see cref="MetadataReference"/>.
    /// </summary>
    internal sealed class MetadataReferenceComparer : IEqualityComparer<MetadataReference>
    {
        /// <summary>
        /// The default instance.
        /// </summary>
        internal static readonly MetadataReferenceComparer Default = new MetadataReferenceComparer();
        private static readonly StringComparer OrdinalIgnoreCase = StringComparer.OrdinalIgnoreCase;

        /// <inheritdoc />
        bool IEqualityComparer<MetadataReference>.Equals(MetadataReference x, MetadataReference y) => Equals(x, y);

        /// <inheritdoc />
        public int GetHashCode(MetadataReference obj)
        {
            if (obj is PortableExecutableReference portable)
            {
                return OrdinalIgnoreCase.GetHashCode(portable.FilePath);
            }

            return obj.GetHashCode();
        }

        /// <summary>
        /// Suppressing this with Obsolete to it is not used by mistake.
        /// </summary>
        /// <param name="x">The first.</param>
        /// <param name="y">The other.</param>
        /// <returns>object.Equals().</returns>
        [Obsolete("Don't use this", error: true)]
        internal new static bool Equals(object x, object y) => object.Equals(x, y);

        /// <summary>
        /// Compare two <see cref="MetadataReference"/>.
        /// Compare by
        /// - FilePath if they are <see cref="PortableExecutableReference"/>
        /// - object.Equals() if not.
        /// </summary>
        /// <param name="x">The first <see cref="MetadataReference"/>.</param>
        /// <param name="y">The other <see cref="MetadataReferences"/>.</param>
        /// <returns>True if x and y are found equal.</returns>
        internal static bool Equals(MetadataReference x, MetadataReference y)
        {
            if (x is null && y is null)
            {
                return true;
            }

            if (x is null || y is null)
            {
                return false;
            }

            if (x is PortableExecutableReference xp &&
                y is PortableExecutableReference yp)
            {
                return OrdinalIgnoreCase.Equals(xp.FilePath, yp.FilePath);
            }

            return object.Equals(x, y);
        }
    }
}
