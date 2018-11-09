namespace Gu.Roslyn.Asserts
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Immutable;
    using System.IO;
    using System.Reflection;
    using Microsoft.CodeAnalysis;

    /// <summary>
    /// For finding MetadataReferences in the Reference assemblies folder.
    /// Everything in this class is cargo culted. Don't know if there are any docs for this.
    /// </summary>
    public static class ReferenceAssembly
    {
        private static readonly Lazy<ImmutableDictionary<string, FileInfo>> NameFileMap = new Lazy<ImmutableDictionary<string, FileInfo>>(Create);
        private static readonly ConcurrentDictionary<string, MetadataReference> CachedReferences = new ConcurrentDictionary<string, MetadataReference>();

        /// <summary>
        /// Try get a <see cref="MetadataReference"/> from the C:\Program Files (x86)\Reference Assemblies\Microsoft\Framework\.NETFramework\{version}.
        /// </summary>
        /// <param name="assembly">Example 'System'.</param>
        /// <param name="metadataReference">The <see cref="MetadataReference"/> if found.</param>
        /// <returns>A value indicating a reference was found.</returns>
        public static bool TryGet(Assembly assembly, out MetadataReference metadataReference)
        {
            if (NameFileMap.Value.TryGetValue(Path.GetFileNameWithoutExtension(assembly.Location), out var dllFile))
            {
                metadataReference = CachedReferences.GetOrAdd(dllFile.FullName, x => MetadataReference.CreateFromFile(x));
                return metadataReference != null;
            }

            metadataReference = null;
            return false;
        }

        /// <summary>
        /// Try get a <see cref="MetadataReference"/> from the C:\Program Files (x86)\Reference Assemblies\Microsoft\Framework\.NETFramework\{version}.
        /// </summary>
        /// <param name="name">Example 'System'.</param>
        /// <param name="metadataReference">The <see cref="MetadataReference"/> if found.</param>
        /// <returns>A value indicating a reference was found.</returns>
        public static bool TryGet(string name, out MetadataReference metadataReference)
        {
            if (NameFileMap.Value.TryGetValue(Path.GetFileNameWithoutExtension(name), out var dllFile))
            {
                metadataReference = CachedReferences.GetOrAdd(dllFile.FullName, x => MetadataReference.CreateFromFile(x));
                return metadataReference != null;
            }

            metadataReference = null;
            return false;
        }

        private static ImmutableDictionary<string, FileInfo> Create()
        {
            var referenceAssemblies = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86), "Reference Assemblies");
            if (Directory.Exists(referenceAssemblies))
            {
                var assemblyName = typeof(int).Assembly.GetName();
                foreach (var mscorlib in Directory.EnumerateFiles(referenceAssemblies, "mscorlib.dll", SearchOption.AllDirectories))
                {
                    if (assemblyName.FullName == AssemblyName.GetAssemblyName(mscorlib).FullName)
                    {
                        return Directory.EnumerateFiles(Path.GetDirectoryName(mscorlib), "*.dll", SearchOption.AllDirectories)
                                        .ToImmutableDictionary(
                                            x => Path.GetFileNameWithoutExtension(x),
                                            x => new FileInfo(x));
                    }
                }
            }

            return ImmutableDictionary<string, FileInfo>.Empty;
        }
    }
}
