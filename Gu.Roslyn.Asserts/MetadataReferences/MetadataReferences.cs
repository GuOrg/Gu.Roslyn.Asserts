namespace Gu.Roslyn.Asserts
{
    using System;
    using System.Collections.Generic;
    using System.Collections.Immutable;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;

    /// <summary>
    /// Helper for getting meta data references from <see cref="MetadataReferenceAttribute"/> and <see cref="MetadataReferencesAttribute"/>.
    /// </summary>
    public static class MetadataReferences
    {
        private static ImmutableArray<MetadataReference> fromAttributes;

        /// <summary>
        /// Get the meta data references specified with <see cref="MetadataReferenceAttribute"/> and <see cref="MetadataReferencesAttribute"/> in the test assemblies.
        /// </summary>
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

                if (assembly.GetCustomAttribute<MetadataReferencesAttribute>() is MetadataReferencesAttribute attribute)
                {
                    set.UnionWith(attribute.MetadataReferences);
                }
            }

            fromAttributes = ImmutableArray.CreateRange(set);
            return fromAttributes;
        }

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
        /// Create a binary reference from strings.
        /// This is useful when testing for example deriving from a base class not in source.
        /// </summary>
        /// <param name="code">The code to create a dll project from.</param>
        /// <returns>A <see cref="MetadataReference"/>.</returns>
        public static MetadataReference CreateBinary(params string[] code)
        {
            var sln = CodeFactory.CreateSolutionWithOneProject(
                code,
                new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary, allowUnsafe: true),
#pragma warning disable CS0618 // Suppress until removed. Will be replaced with Metadatareferences.FromAttributes()
                RoslynAssert.MetadataReferences);
#pragma warning restore CS0618 // Suppress until removed. Will be replaced with Metadatareferences.FromAttributes()
            RoslynAssert.NoCompilerErrors(sln);

            using (var ms = new MemoryStream())
            {
                _ = sln.Projects.Single().GetCompilationAsync().Result.Emit(ms);
                ms.Position = 0;
                return MetadataReference.CreateFromStream(ms);
            }
        }

        /// <summary>
        /// Get the <see cref="MetadataReference"/> for <paramref name="typesInAssemblies"/> and all assemblies referenced by <paramref name="typesInAssemblies"/>.
        /// </summary>
        /// <param name="typesInAssemblies">A type in the assemblies.</param>
        /// <returns><see cref="MetadataReference"/>s.</returns>
        public static IEnumerable<MetadataReference> Transitive(params Type[] typesInAssemblies)
        {
            return Transitive(typesInAssemblies.SelectMany(t => Assemblies(t)).ToArray());

            static IEnumerable<Assembly> Assemblies(Type type)
            {
                yield return type.Assembly;
                if (type.IsGenericType)
                {
                    foreach (var genericArgument in type.GetGenericArguments())
                    {
                        yield return genericArgument.Assembly;
                    }
                }
            }
        }

        /// <summary>
        /// Get the <see cref="MetadataReference"/> for <paramref name="assemblies"/> and all assemblies referenced by <paramref name="assemblies"/>.
        /// </summary>
        /// <param name="assemblies">The assemblies.</param>
        /// <returns><see cref="MetadataReference"/>s.</returns>
        public static IEnumerable<MetadataReference> Transitive(params Assembly[] assemblies)
        {
            foreach (var assembly in RecursiveReferencedAssemblies(assemblies))
            {
                yield return CreateFromAssembly(assembly);
            }
        }

        private static HashSet<Assembly> RecursiveReferencedAssemblies(Assembly a, HashSet<Assembly> recursiveAssemblies = null)
        {
            recursiveAssemblies ??= new HashSet<Assembly>();
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
            recursiveAssemblies ??= new HashSet<Assembly>();
            foreach (var assembly in assemblies)
            {
                RecursiveReferencedAssemblies(assembly, recursiveAssemblies);
            }

            return recursiveAssemblies;
        }
    }
}
