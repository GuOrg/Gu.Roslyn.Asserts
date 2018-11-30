namespace Gu.Roslyn.Asserts
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Microsoft.CodeAnalysis;

    /// <summary>
    /// Specify what default metadata reference to use.
    /// </summary>
    [AttributeUsage(AttributeTargets.Assembly,  AllowMultiple = true)]
    public class TransitiveMetadataReferencesAttribute : Attribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TransitiveMetadataReferencesAttribute"/> class.
        /// </summary>
        /// <param name="types">Specify a type for which <see cref="MetadataReference"/> for the assembly and all transitive dependencies will be added.</param>
        public TransitiveMetadataReferencesAttribute(params Type[] types)
        {
            this.MetadataReferences = types.SelectMany(t => Asserts.MetadataReferences.Transitive(t.Assembly))
                                           .ToList();
        }

        /// <summary>
        /// Gets the metadata references to include in the workspaces used in tests.
        /// </summary>
        public IEnumerable<MetadataReference> MetadataReferences { get; }
    }
}
