namespace Gu.Roslyn.Asserts
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using Microsoft.CodeAnalysis;

    /// <summary>
    /// Specify what default metadata reference to use.
    /// </summary>
    [AttributeUsage(AttributeTargets.Assembly)]
    public class MetadataReferencesAttribute : Attribute
    {
        private static IReadOnlyList<MetadataReference> metadataReferences;

        /// <summary>
        /// Initializes a new instance of the <see cref="MetadataReferencesAttribute"/> class.
        /// </summary>
        /// <param name="types">Specify types in assemblies for which metadata references will be included.</param>
        public MetadataReferencesAttribute(params Type[] types)
            : this(types.Select(x => x.GetTypeInfo().Assembly).ToArray())
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MetadataReferencesAttribute"/> class.
        /// </summary>
        /// <param name="assemblies">Specify assemblies for which metadata references will be included.</param>
        public MetadataReferencesAttribute(params Assembly[] assemblies)
#if NET46
            // ReSharper disable once CoVariantArrayConversion
            : this(assemblies.Select(x => MetadataReference.CreateFromFile(x.Location)).ToArray())
#else
            : this(Array.Empty<MetadataReference>())
#endif
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MetadataReferencesAttribute"/> class.
        /// </summary>
        /// <param name="metadataReferences">Specify metadata references to  be included.</param>
        public MetadataReferencesAttribute(params MetadataReference[] metadataReferences)
        {
            this.MetadataReferences = metadataReferences ?? throw new ArgumentNullException(nameof(metadataReferences), "This only works for Net46");
        }

        /// <summary>
        /// Gets the metadata references to include in the workspaces used in tests.
        /// </summary>
        public IReadOnlyList<MetadataReference> MetadataReferences { get; }

        /// <summary>
        /// Get the metadata references specified in the test assembly.
        /// </summary>
        public static List<MetadataReference> GetMetadataReferences()
        {
#if NET46
            if (metadataReferences != null)
            {
                return new List<MetadataReference>(metadataReferences);
            }

            foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                var attribute = (MetadataReferencesAttribute)GetCustomAttribute(assembly, typeof(MetadataReferencesAttribute));
                if (attribute != null)
                {
                    metadataReferences = attribute.MetadataReferences;
                    return new List<MetadataReference>(metadataReferences);
                }
            }

            metadataReferences = new MetadataReference[0];
            return new List<MetadataReference>();
#else
            return new List<MetadataReference>();
#endif
        }
    }
}
