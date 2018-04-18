namespace Gu.Roslyn.Asserts
{
    using System;
    using System.IO;
    using System.Reflection;

    public static class SolutionFile
    {
        /// <summary>
        /// Searches parent directories for <paramref name="assembly"/> the first file matching *.sln
        /// </summary>
        /// <param name="assembly">The assembly</param>
        /// <param name="sln">The <see cref="File"/> if found.</param>
        /// <returns>A value indicating if a file was found.</returns>
        public static bool TryFind(Assembly assembly, out FileInfo sln)
        {
            if (assembly?.CodeBase == null)
            {
                sln = null;
                return false;
            }

            var dll = new FileInfo(new Uri(assembly.CodeBase, UriKind.Absolute).LocalPath);
            return CodeFactory.TryFindFileInParentDirectory(dll.Directory, "*.sln", out sln);
        }

        /// <summary>
        /// Searches parent directories for <paramref name="name"/> the first file matching Foo.sln
        /// </summary>
        /// <param name="name">The assembly</param>
        /// <param name="sln">The <see cref="File"/> if found.</param>
        /// <returns>A value indicating if a file was found.</returns>
        public static bool TryFind(string name, out FileInfo sln)
        {
            var assembly = Assembly.GetCallingAssembly();
            var dll = new FileInfo(new Uri(assembly.CodeBase, UriKind.Absolute).LocalPath);
            return CodeFactory.TryFindFileInParentDirectory(dll.Directory, name, out sln);
        }

        /// <summary>
        /// Searches parent directories for <paramref name="name"/> the first file matching Foo.sln
        /// </summary>
        /// <param name="name">The assembly</param>
        /// <returns>The solution file.</returns>
        public static FileInfo Find(string name)
        {
            var assembly = Assembly.GetCallingAssembly();
            var dll = new FileInfo(new Uri(assembly.CodeBase, UriKind.Absolute).LocalPath);
            if (CodeFactory.TryFindFileInParentDirectory(dll.Directory, name, out var sln))
            {
                return sln;
            }

            throw new InvalidOperationException("Did not find a file named: " + name);
        }
    }
}
