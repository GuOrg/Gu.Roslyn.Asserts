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
        /// Get the <see cref="MetadataReference"/> for <paramref name="assembly"/> and all assemblies referenced by <paramref name="assembly"/>
        /// </summary>
        /// <param name="assembly">The assembly.</param>
        /// <returns><see cref="MetadataReference"/>s.</returns>
        public static IEnumerable<MetadataReference> Transitive(Assembly assembly)
        {
            foreach (var a in RecursiveReferencedAssemblies(assembly))
            {
                yield return MetadataReference.CreateFromFile(a.Location);
            }
        }

        /// <summary>
        /// Get the metadata references specified with <see cref="MetadataReferenceAttribute"/> and <see cref="MetadataReferencesAttribute"/> in the test assembly.
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

            ////var displayNames = metadataReferences.Select(x => x.Display).ToArray();
            ////var distinct = displayNames.Distinct().OrderBy(x => x).ToArray();
            ////if (distinct.Length != displayNames.Length)
            ////{
            ////    var dupes = displayNames.Where(x => displayNames.Count(y => x == y) > 1);
            ////    throw AssertException.Create(
            ////        "Expected metadata references to be unique assemblies.\r\n" +
            ////        "The following appear more than once.\r\n" +
            ////        $"{string.Join(Environment.NewLine, dupes)}");
            ////}

            return new List<MetadataReference>(metadataReferences);
        }

        private static HashSet<Assembly> RecursiveReferencedAssemblies(Assembly a, HashSet<Assembly> assemblies = null)
        {
            assemblies = assemblies ?? new HashSet<Assembly>();
            if (assemblies.Add(a))
            {
                foreach (var referencedAssemblyName in a.GetReferencedAssemblies())
                {
                    var referencedAssembly = AppDomain.CurrentDomain.GetAssemblies()
                                                      .SingleOrDefault(x => x.GetName() == referencedAssemblyName) ??
                                             Assembly.Load(referencedAssemblyName);
                    RecursiveReferencedAssemblies(referencedAssembly, assemblies);
                }
            }

            return assemblies;
        }
    }
}