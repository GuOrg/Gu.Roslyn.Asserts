namespace Gu.Roslyn.Asserts
{
    using System;
    using System.Diagnostics;
    using System.Linq;
    using Microsoft.CodeAnalysis;

    /// <summary>
    /// Debug view for <see cref="MetadataReferencesCollection"/>.
    /// </summary>
    internal class MetadataReferencesCollectionDebugView
    {
        private readonly MetadataReferencesCollection metaDataReferences;

        /// <summary>
        /// Initializes a new instance of the <see cref="MetadataReferencesCollectionDebugView"/> class.
        /// </summary>
        /// <param name="set">The set.</param>
        internal MetadataReferencesCollectionDebugView(MetadataReferencesCollection set)
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
