namespace Gu.Roslyn.Asserts
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using Microsoft.CodeAnalysis;

    /// <summary>
    /// Helper for getting metadata references from <see cref="MetadataReferenceAttribute"/> and <see cref="MetadataReferencesAttribute"/>
    /// </summary>
    public static class MetadataReferences
    {
#pragma warning disable 169
        private static List<MetadataReference> metadataReferences;
#pragma warning restore 169

        /// <summary>
        /// Get the <see cref="MetadataReference"/> for <paramref name="typeInAssembly"/> and all assemblies referenced by <paramref name="typeInAssembly"/>
        /// </summary>
        /// <param name="typeInAssembly">A type in the assemblies.</param>
        /// <returns><see cref="MetadataReference"/>s.</returns>
        public static IEnumerable<MetadataReference> Transitive(params Type[] typeInAssembly)
        {
            return Transitive(typeInAssembly.Select(x => x.Assembly).ToArray());
        }

        /// <summary>
        /// Get the <see cref="MetadataReference"/> for <paramref name="assemblies"/> and all assemblies referenced by <paramref name="assemblies"/>
        /// </summary>
        /// <param name="assemblies">The assemblies.</param>
        /// <returns><see cref="MetadataReference"/>s.</returns>
        public static IEnumerable<MetadataReference> Transitive(params Assembly[] assemblies)
        {
            foreach (var a in RecursiveReferencedAssemblies(assemblies))
            {
                yield return MetadataReference.CreateFromFile(a.Location);
            }
        }

        /// <summary>
        /// Get the metadata references specified with <see cref="MetadataReferenceAttribute"/> and <see cref="MetadataReferencesAttribute"/> in the test assemblies.
        /// </summary>
        public static IReadOnlyList<MetadataReference> FromAttributes()
        {
            if (metadataReferences != null)
            {
                return new List<MetadataReference>(metadataReferences);
            }

            metadataReferences = new List<MetadataReference>();
            var assemblies = AppDomain.CurrentDomain.GetAssemblies();
            foreach (var assembly in assemblies)
            {
                var attributes = Attribute.GetCustomAttributes(assembly, typeof(MetadataReferenceAttribute));
                foreach (var attribute in attributes.Cast<MetadataReferenceAttribute>())
                {
                    metadataReferences.Add(attribute.MetadataReference);
                }
            }

            foreach (var assembly in assemblies)
            {
                var attribute = (MetadataReferencesAttribute)Attribute.GetCustomAttribute(assembly, typeof(MetadataReferencesAttribute));
                if (attribute != null)
                {
                    metadataReferences.AddRange(attribute.MetadataReferences);
                }
            }

            foreach (var assembly in assemblies)
            {
                var attributes = Attribute.GetCustomAttributes(assembly, typeof(TransitiveMetadataReferencesAttribute));
                foreach (var attribute in attributes.Cast<TransitiveMetadataReferencesAttribute>())
                {
                    metadataReferences.AddRange(attribute.MetadataReferences);
                }
            }

            return new List<MetadataReference>(metadataReferences);
        }

        private static HashSet<Assembly> RecursiveReferencedAssemblies(Assembly a, HashSet<Assembly> recursiveAssemblies = null)
        {
            recursiveAssemblies = recursiveAssemblies ?? new HashSet<Assembly>();
            if (recursiveAssemblies.Add(a))
            {
                foreach (var referencedAssemblyName in a.GetReferencedAssemblies())
                {
                    var referencedAssembly = AppDomain.CurrentDomain.GetAssemblies()
                                                      .SingleOrDefault(x => x.GetName() == referencedAssemblyName) ??
                                             Assembly.Load(referencedAssemblyName);
                    _ = RecursiveReferencedAssemblies(referencedAssembly, recursiveAssemblies);
                }
            }

            return recursiveAssemblies;
        }

        private static HashSet<Assembly> RecursiveReferencedAssemblies(Assembly[] assemblies, HashSet<Assembly> recursiveAssemblies = null)
        {
            recursiveAssemblies = recursiveAssemblies ?? new HashSet<Assembly>();
            foreach (var assembly in assemblies)
            {
                RecursiveReferencedAssemblies(assembly, recursiveAssemblies);
            }

            return recursiveAssemblies;
        }
    }
}
