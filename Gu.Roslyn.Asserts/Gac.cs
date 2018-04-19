namespace Gu.Roslyn.Asserts
{
    using System;
    using System.Collections.Concurrent;
    using System.IO;
    using Microsoft.CodeAnalysis;

    /// <summary>
    /// For finding MetadataReferences in the GAC.
    /// </summary>
    public static class Gac
    {
        private static readonly Lazy<ConcurrentDictionary<string, FileInfo>> Cache = new Lazy<ConcurrentDictionary<string, FileInfo>>(Create);
        private static readonly ConcurrentDictionary<string, MetadataReference> Cachedreferences = new ConcurrentDictionary<string, MetadataReference>();

        /// <summary>
        /// Try get a <see cref="MetadataReference"/> from the GAC.
        /// </summary>
        /// <param name="name">Example 'System'</param>
        /// <param name="metadataReference">The <see cref="MetadataReference"/> if found.</param>
        /// <returns>A value indicating a reference was found.</returns>
        public static bool TryGet(string name, out MetadataReference metadataReference)
        {
            if (Cache.Value.TryGetValue(name, out var fileInfo))
            {
                metadataReference = Cachedreferences.GetOrAdd(fileInfo.FullName, x => MetadataReference.CreateFromFile(x));
            }

            metadataReference = null;
            return false;
        }

        private static ConcurrentDictionary<string, FileInfo> Create()
        {
            var gac = new ConcurrentDictionary<string, FileInfo>();
            var dir = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Windows), "Microsoft.NET\\assembly");
            if (Directory.Exists(dir))
            {
                var msil = Path.Combine(dir, "GAC_MSIL");
                if (Directory.Exists(msil))
                {
                    foreach (var file in Directory.EnumerateFiles(dir, "*.dll", SearchOption.AllDirectories))
                    {
                        gac.TryAdd(Path.GetFileNameWithoutExtension(file), new FileInfo(file));
                    }
                }
            }

            return gac;
        }
    }
}
