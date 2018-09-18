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

                if (assembly.GetCustomAttribute<MetadataReferencesAttribute>() is MetadataReferencesAttribute attribute)
                {
                    set.UnionWith(attribute.MetadataReferences);
                }
            }

            metadataReferences = new List<MetadataReference>(set);
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

        private class MetadataReferenceComparer : IEqualityComparer<MetadataReference>
        {
            public static readonly MetadataReferenceComparer Default = new MetadataReferenceComparer();
            private static readonly StringComparer OrdinalIgnoreCase = StringComparer.OrdinalIgnoreCase;

            public bool Equals(MetadataReference x, MetadataReference y)
            {
                if (x == null && y == null)
                {
                    return true;
                }

                if (x == null || y == null)
                {
                    return false;
                }

                if (x is PortableExecutableReference xp &&
                    y is PortableExecutableReference yp)
                {
                    return OrdinalIgnoreCase.Equals(xp.FilePath, yp.FilePath);
                }

                return object.Equals(x, y);
            }

            public int GetHashCode(MetadataReference obj)
            {
                if (obj is PortableExecutableReference portable)
                {
                    return OrdinalIgnoreCase.GetHashCode(portable);
                }

                return obj.GetHashCode();
            }
        }
    }
}
