namespace Gu.Roslyn.Asserts
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Immutable;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using Microsoft.CodeAnalysis;

    /// <summary>
    /// For finding MetadataReferences in the GAC.
    /// </summary>
    public static class ReferenceAssembly
    {
        private static readonly Lazy<ImmutableDictionary<string, ImmutableArray<FileInfo>>> Cache = new Lazy<ImmutableDictionary<string, ImmutableArray<FileInfo>>>(Create);
        private static readonly ConcurrentDictionary<string, MetadataReference> CachedReferences = new ConcurrentDictionary<string, MetadataReference>();

        /// <summary>
        /// Try get a <see cref="MetadataReference"/> from the C:\Program Files (x86)\Reference Assemblies\Microsoft\Framework\.NETFramework\{version}.
        /// </summary>
        /// <param name="assembly">Example 'System'.</param>
        /// <param name="metadataReference">The <see cref="MetadataReference"/> if found.</param>
        /// <returns>A value indicating a reference was found.</returns>
        public static bool TryGet(Assembly assembly, out MetadataReference metadataReference)
        {
            if (Cache.Value.TryGetValue(Path.GetFileNameWithoutExtension(assembly.Location), out var files))
            {
                var assemblyName = assembly.GetName();
                foreach (var file in files)
                {
                    var candidate = AssemblyName.GetAssemblyName(file.FullName);
                    if (assemblyName.FullName == candidate.FullName)
                    {
                        metadataReference = CachedReferences.GetOrAdd(file.FullName, x => MetadataReference.CreateFromFile(x));
                        return metadataReference != null;
                    }
                }

                metadataReference = CachedReferences.GetOrAdd(files.Last().FullName, x => MetadataReference.CreateFromFile(x));
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
            if (Cache.Value.TryGetValue(Path.GetFileNameWithoutExtension(name), out var files))
            {
                metadataReference = CachedReferences.GetOrAdd(files.Last().FullName, x => MetadataReference.CreateFromFile(x));
                return metadataReference != null;
            }

            metadataReference = null;
            return false;
        }

        private static ImmutableDictionary<string, ImmutableArray<FileInfo>> Create()
        {
            var dir = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86), "Reference Assemblies");
            if (Directory.Exists(dir))
            {
                return Directory.EnumerateFiles(dir, "*.dll", SearchOption.AllDirectories)
                                .GroupBy(x => Path.GetFileNameWithoutExtension(x))
                                .ToImmutableDictionary(
                                    x => x.Key,
                                    x => x.Select(f => new FileInfo(f))
                                          .OrderByDescending(f => f.CreationTimeUtc)
                                          .ToImmutableArray());
            }

            return ImmutableDictionary<string, ImmutableArray<FileInfo>>.Empty;
        }
    }
}
