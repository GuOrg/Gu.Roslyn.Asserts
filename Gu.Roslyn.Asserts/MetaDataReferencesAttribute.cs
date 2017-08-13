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
#pragma warning disable 169
        private static List<MetadataReference> metadataReferences;
#pragma warning restore 169

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
        [SuppressMessage("ReSharper", "UnusedParameter.Local")]
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

            metadataReferences = new List<MetadataReference>();
            var assemblies = AppDomain.CurrentDomain.GetAssemblies();
            foreach (var assembly in assemblies)
            {
                var attributes = GetCustomAttributes(assembly, typeof(MetadataReferenceAttribute));
                if (attributes != null)
                {
                    foreach (MetadataReferenceAttribute attribute in attributes)
                    {
                        metadataReferences.Add(attribute.MetadataReference);
                    }
                }
            }

            foreach (var assembly in assemblies)
            {
                var attribute = (MetadataReferencesAttribute)GetCustomAttribute(assembly, typeof(MetadataReferencesAttribute));
                if (attribute != null)
                {
                    metadataReferences.AddRange(attribute.MetadataReferences);
                }
            }

            var displayNames = metadataReferences.Select(x => x.Display).ToArray();
            var distinct = displayNames.Distinct().OrderBy(x => x).ToArray();
            if (distinct.Length != displayNames.Length)
            {
                var dupes = displayNames.Where(x => displayNames.Count(y => x == y) > 1);
                throw AssertException.Create(
                    "Expected metadata references to be unique assemblies.\r\n" +
                    "The following appear more than once.\r\n" +
                    $"{string.Join(Environment.NewLine, dupes)}");
            }

            return new List<MetadataReference>(metadataReferences);
#else
            return new List<MetadataReference>();
#endif
        }
    }
}
