namespace Gu.Roslyn.Asserts
{
    using System.Collections;
    using System.Collections.Generic;
    using System.Diagnostics;
    using Microsoft.CodeAnalysis;

    /// <summary>
    /// A collection with set semantics only adding unique references.
    /// Determining if equal by checking the <see cref="PortableExecutableReference.FilePath"/> property.
    /// Reason for this class is to make sure explicit references with aliases are not overwritten by added transitive.
    /// This class has weird semantics and is not elegant but doing it like this to make the breaking change when changing from List&lt;MetadataReference&gt; minimal.
    /// </summary>
    [DebuggerTypeProxy(typeof(MetadataReferencesCollectionDebugView_))]
    [DebuggerDisplay("Count = {this.inner.Count}")]
    public class MetaDataReferencesCollection : IEnumerable<MetadataReference>
    {
        private readonly List<MetadataReference> inner = new List<MetadataReference>();

        /// <summary>
        /// Initializes a new instance of the <see cref="MetaDataReferencesCollection"/> class.
        /// </summary>
        public MetaDataReferencesCollection()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MetaDataReferencesCollection"/> class.
        /// </summary>
        public MetaDataReferencesCollection(IEnumerable<MetadataReference> list)
        {
            this.AddRange(list);
        }

        /// <summary>
        /// Add a collection of <see cref="MetadataReference"/>.
        /// </summary>
        /// <param name="items">The references to add.</param>
        public void AddRange(IEnumerable<MetadataReference> items)
        {
            foreach (var item in items)
            {
                this.Add(item);
            }
        }

        /// <summary>
        /// Adds the item if it does not exist in the collection.
        /// If the item exists the one with aliases wins.
        /// </summary>
        /// <param name="item">The item.</param>
        public bool Add(MetadataReference item)
        {
            for (var i = 0; i < this.inner.Count; i++)
            {
                var existing = this.inner[i];
                if (MetadataReferenceComparer.Equals(existing, item))
                {
                    if (item.Properties.Aliases.Length > 0)
                    {
                        this.inner[i] = item;
                        return true;
                    }

                    return false;
                }
            }

            this.inner.Add(item);
            return true;
        }

        /// <summary>
        /// Clear the collection.
        /// </summary>
        public void Clear() => this.inner.Clear();

        /// <inheritdoc />
        public IEnumerator<MetadataReference> GetEnumerator() => this.inner.GetEnumerator();

        /// <inheritdoc />
        IEnumerator IEnumerable.GetEnumerator() => ((IEnumerable)this.inner).GetEnumerator();
    }
}
