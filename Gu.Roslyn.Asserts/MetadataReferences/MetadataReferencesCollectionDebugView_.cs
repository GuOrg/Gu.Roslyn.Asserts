namespace Gu.Roslyn.Asserts
{
    using System;
    using System.Diagnostics;
    using System.Linq;
    using Microsoft.CodeAnalysis;

    /// <summary>
    /// Debug view for <see cref="MetaDataReferencesCollection"/>.
    /// </summary>
    internal class MetadataReferencesCollectionDebugView_
    {
        private readonly MetaDataReferencesCollection metaDataReferences;

        /// <summary>
        /// Initializes a new instance of the <see cref="MetadataReferencesCollectionDebugView_"/> class.
        /// </summary>
        /// <param name="set">The set.</param>
        internal MetadataReferencesCollectionDebugView_(MetaDataReferencesCollection set)
        {
            this.metaDataReferences = set ?? throw new ArgumentNullException(nameof(set));
        }

        /// <summary>
        /// Gets the items.
        /// </summary>
        [DebuggerBrowsable(DebuggerBrowsableState.RootHidden)]
        internal MetadataReference[] Items => this.metaDataReferences.ToArray();
    }
}
