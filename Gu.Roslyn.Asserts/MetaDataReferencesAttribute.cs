namespace Gu.Roslyn.Asserts
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;
    using System.Reflection;
    using Microsoft.CodeAnalysis;

    /// <summary>
    /// Specify what default metadata reference to use.
    /// </summary>
    [AttributeUsage(AttributeTargets.Assembly, AllowMultiple = false)]
    public class MetadataReferencesAttribute : Attribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MetadataReferencesAttribute"/> class.
        /// </summary>
        /// <param name="types">Specify types in assemblies for which metadata references will be included.</param>
        public MetadataReferencesAttribute(params Type[] types)
        {
            this.MetadataReferences = types.Select(x => MetadataReference.CreateFromFile(x.Assembly.Location))
                                           .ToArray();
        }

        /// <summary>
        /// Gets the metadata references to include in the workspaces used in tests.
        /// </summary>
        public IReadOnlyList<MetadataReference> MetadataReferences { get; }
    }
}
