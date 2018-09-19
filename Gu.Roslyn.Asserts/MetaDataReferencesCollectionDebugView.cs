namespace Gu.Roslyn.Asserts
{
    using System;
    using System.Diagnostics;
    using System.Linq;
    using Microsoft.CodeAnalysis;

    /// <summary>
    /// Debug view for <see cref="MetaDataReferencesCollection"/>
    /// </summary>
    internal class MetaDataReferencesCollectionDebugView
    {
        private readonly MetaDataReferencesCollection metaDataReferences;

        /// <summary>
        /// Initializes a new instance of the <see cref="MetaDataReferencesCollectionDebugView"/> class.
        /// </summary>
        /// <param name="set">The set.</param>
        public MetaDataReferencesCollectionDebugView(MetaDataReferencesCollection set)
        {
            this.metaDataReferences = set ?? throw new ArgumentNullException(nameof(set));
        }

        /// <summary>
        /// Gets the items.
        /// </summary>
        [DebuggerBrowsable(DebuggerBrowsableState.RootHidden)]
        public MetadataReference[] Items => this.metaDataReferences.ToArray();
    }
}
