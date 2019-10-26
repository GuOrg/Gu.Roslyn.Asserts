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
    /// For finding MetadataReferences in the Reference assemblies folder.
    /// Everything in this class is cargo culted. Don't know if there are any docs for this.
    /// </summary>
    public static class ReferenceAssembly
    {
        private static readonly ConcurrentDictionary<string, MetadataReference> CachedReferences = new ConcurrentDictionary<string, MetadataReference>();
        private static DirectoryInfo? directory;

        /// <summary>
        /// Gets or sets the reference assembly location to use.
        /// </summary>
        public static DirectoryInfo? Directory
        {
            get
            {
                return directory ?? (directory = GetDefault());

                static DirectoryInfo? GetDefault()
                {
                    var referenceAssemblies = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86), "Reference Assemblies");
                    if (System.IO.Directory.Exists(referenceAssemblies))
                    {
                        var expectedName = typeof(int).Assembly.GetName();
                        foreach (var mscorlib in System.IO.Directory.EnumerateFiles(referenceAssemblies, "mscorlib.dll", SearchOption.AllDirectories).OrderByDescending(x => File.GetCreationTimeUtc(x)))
                        {
                            var name = AssemblyName.GetAssemblyName(mscorlib);
                            if (expectedName.FullName == name.FullName)
                            {
                                return new DirectoryInfo(Path.GetDirectoryName(mscorlib));
                            }
                        }
                    }

                    return null;
                }
            }

            set
            {
                CachedReferences.Clear();
                NameFileMap.Clear();
                directory = value;
            }
        }

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

        private static class NameFileMap
        {
            private static ImmutableDictionary<string, FileInfo>? value;

            internal static ImmutableDictionary<string, FileInfo> Value => value ?? (value = Create());

            internal static void Clear()
            {
                value = null;
            }

            private static ImmutableDictionary<string, FileInfo> Create()
            {
                return Directory?.EnumerateFiles("*.dll", SearchOption.AllDirectories)
                                 .ToImmutableDictionary(
                                     x => Path.GetFileNameWithoutExtension(x.FullName),
                                     x => x) ??
                       ImmutableDictionary<string, FileInfo>.Empty;
            }
        }
    }
}
