namespace Gu.Roslyn.Asserts
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Microsoft.CodeAnalysis;

    /// <summary>
    /// Specify what default metadata reference to use when compiling the code in asserts.
    /// </summary>
    [AttributeUsage(AttributeTargets.Assembly)]
    public class MetadataReferencesAttribute_ : Attribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MetadataReferencesAttribute_"/> class.
        /// </summary>
        /// <param name="types">Specify types in assemblies for which metadata references will be included.</param>
        public MetadataReferencesAttribute_(params Type[] types)
        {
            this.MetadataReferences = types.Select(x => Gu.Roslyn.Asserts.MetadataReferences.CreateFromAssembly(x.Assembly))
                                           .ToArray();
        }

        /// <summary>
        /// Gets the metadata references to include in the workspaces used in tests.
        /// </summary>
        public IEnumerable<MetadataReference> MetadataReferences { get; }
    }
}
