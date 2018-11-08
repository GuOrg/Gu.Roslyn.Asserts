namespace Gu.Roslyn.Asserts
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using Microsoft.CodeAnalysis;

    /// <summary>
    /// Helper for getting metadata references from <see cref="MetadataReferenceAttribute"/> and <see cref="MetadataReferencesAttribute"/>.
    /// </summary>
    public static class MetadataReferences
    {
#pragma warning disable 169
        private static List<MetadataReference> metadataReferences;
#pragma warning restore 169

        /// <summary>
        /// Create a <see cref="MetadataReference"/> for the <paramref name="assembly"/>.
        /// Checks reference assemblies first.
        /// </summary>
        /// <param name="assembly">An <see cref="Assembly"/>.</param>
        /// <returns>A <see cref="MetadataReference"/>.</returns>
        public static MetadataReference CreateFromAssembly(Assembly assembly)
        {
            if (ReferenceAssembly.TryGet(assembly, out var reference))
            {
                return reference;
            }

            return MetadataReference.CreateFromFile(assembly.Location);
        }

        /// <summary>
        /// Create a <see cref="MetadataReference"/> for the <paramref name="assemblyFile"/>.
        /// Checks reference assemblies first.
        /// </summary>
        /// <param name="assemblyFile">An <see cref="Assembly"/>.</param>
        /// <returns>A <see cref="MetadataReference"/>.</returns>
        public static MetadataReference CreateFromFile(string assemblyFile)
        {
            if (ReferenceAssembly.TryGet(assemblyFile, out var reference))
            {
                return reference;
            }

            return MetadataReference.CreateFromFile(assemblyFile);
        }

        /// <summary>
        /// Get the <see cref="MetadataReference"/> for <paramref name="typeInAssembly"/> and all assemblies referenced by <paramref name="typeInAssembly"/>.
        /// </summary>
        /// <param name="typeInAssembly">A type in the assemblies.</param>
        /// <returns><see cref="MetadataReference"/>s.</returns>
        public static IEnumerable<MetadataReference> Transitive(params Type[] typeInAssembly)
        {
            return Transitive(typeInAssembly.Select(x => x.Assembly).ToArray());
        }

        /// <summary>
        /// Get the <see cref="MetadataReference"/> for <paramref name="assemblies"/> and all assemblies referenced by <paramref name="assemblies"/>.
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
        public static IEnumerable<MetadataReference> FromAttributes()
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
                    if (TryGetOrLoad(referencedAssemblyName, out var referencedAssembly))
                    {
                        _ = RecursiveReferencedAssemblies(referencedAssembly, recursiveAssemblies);
                    }
                }
            }

            return recursiveAssemblies;

            bool TryGetOrLoad(AssemblyName name, out Assembly result)
            {
                var assemblies = AppDomain.CurrentDomain.GetAssemblies();
                result = assemblies.SingleOrDefault(x => AssemblyName.ReferenceMatchesDefinition(x.GetName(), name));
                if (result != null)
                {
                    return true;
                }

                try
                {
                    result = Assembly.Load(name);
                    return true;
                }
                catch
                {
                    return false;
                }
            }
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
