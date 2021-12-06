namespace Gu.Roslyn.Asserts
{
    using System;
    using System.Collections.Generic;
    using System.Collections.Immutable;
    using System.Linq;
    using System.Reflection;
    using Microsoft.CodeAnalysis;

    public static partial class MetadataReferences
    {
        /// <summary>
        /// Get the meta data references specified with <see cref="MetadataReferenceAttribute"/> and <see cref="MetadataReferencesAttribute"/> in the test assemblies.
        /// </summary>
        [Obsolete("Use Settings.Default")]
        public static ImmutableArray<MetadataReference> FromAttributes()
        {
            if (!fromAttributes.IsDefault)
            {
                return fromAttributes;
            }

            var set = new HashSet<MetadataReference>(MetadataReferenceComparer.Default);
            foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                var attributes = Attribute.GetCustomAttributes(assembly, typeof(MetadataReferenceAttribute));
                foreach (var single in attributes.Cast<MetadataReferenceAttribute>())
                {
                    set.Add(single.MetadataReference);
                }

                attributes = Attribute.GetCustomAttributes(assembly, typeof(TransitiveMetadataReferencesAttribute));
                foreach (var transitive in attributes.Cast<TransitiveMetadataReferencesAttribute>())
                {
                    set.UnionWith(transitive.MetadataReferences);
                }

                if (assembly.GetCustomAttribute<MetadataReferencesAttribute>() is { } attribute)
                {
                    set.UnionWith(attribute.MetadataReferences);
                }
            }

            fromAttributes = ImmutableArray.CreateRange(set);
            return fromAttributes;
        }

        /// <summary>
        /// Create a binary reference from strings.
        /// This is useful when testing for example deriving from a base class not in source.
        /// </summary>
        /// <param name="code">The code to create a dll project from.</param>
        /// <returns>A <see cref="MetadataReference"/>.</returns>
        [Obsolete("Use MetadataReference.Compile()")]
        public static MetadataReference CreateBinary(params string[] code) => BinaryReference.Compile(code);
    }
}
