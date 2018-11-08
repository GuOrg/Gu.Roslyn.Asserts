namespace Gu.Roslyn.Asserts
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Immutable;
    using System.IO;
    using System.Linq;
    using Microsoft.CodeAnalysis;

    /// <summary>
    /// For finding MetadataReferences in the GAC.
    /// </summary>
    public static class Gac
    {
        private static readonly Lazy<ImmutableDictionary<string, FileInfo>> Cache = new Lazy<ImmutableDictionary<string, FileInfo>>(Create);
        private static readonly ConcurrentDictionary<string, MetadataReference> CachedReferences = new ConcurrentDictionary<string, MetadataReference>();

        /// <summary>
        /// Try get a <see cref="MetadataReference"/> from the GAC.
        /// </summary>
        /// <param name="name">Example 'System'.</param>
        /// <param name="metadataReference">The <see cref="MetadataReference"/> if found.</param>
        /// <returns>A value indicating a reference was found.</returns>
        public static bool TryGet(string name, out MetadataReference metadataReference)
        {
            if (Cache.Value.TryGetValue(name, out var fileInfo))
            {
                metadataReference = CachedReferences.GetOrAdd(fileInfo.FullName, x => MetadataReference.CreateFromFile(x));
                return metadataReference != null;
            }

            metadataReference = null;
            return false;
        }

        private static ImmutableDictionary<string, FileInfo> Create()
        {
            var dir = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Windows), "Microsoft.NET\\assembly\\GAC_MSIL");
            if (Directory.Exists(dir))
            {
                return Directory.EnumerateFiles(dir, "*.dll", SearchOption.AllDirectories)
                                .GroupBy(x => Path.GetFileNameWithoutExtension(x))
                                .ToImmutableDictionary(
                                    x => x.Key,
                                    x => new FileInfo(x.Last()));
            }

            return ImmutableDictionary<string, FileInfo>.Empty;
        }
    }
}
