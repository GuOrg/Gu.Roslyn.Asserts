namespace Gu.Roslyn.Asserts
{
    using System;
    using System.IO;
    using System.Linq;
    using System.Reflection;

    public static class ProjectFile
    {
        /// <summary>
        /// Searches parent directories for <paramref name="dllFile"/>
        /// </summary>
        /// <param name="dllFile">Ex Foo.dll</param>
        /// <param name="result">The <see cref="File"/> if found.</param>
        /// <returns>A value indicating if a file was found.</returns>
        public static bool TryFind(FileInfo dllFile, out FileInfo result)
        {
            result = null;
            if (SolutionFile.TryFind(Assembly.GetCallingAssembly(), out var sln))
            {
                var projectFileName = Path.GetFileNameWithoutExtension(dllFile.FullName) + ".csproj";
                //// ReSharper disable once PossibleNullReferenceException
                result = sln.Directory.EnumerateFiles(projectFileName, SearchOption.AllDirectories).FirstOrDefault();
            }

            return result != null;
        }

        /// <summary>
        /// Searches parent directories for <paramref name="projectFile"/>
        /// </summary>
        /// <param name="projectFile">Ex Foo.csproj</param>
        /// <param name="result">The <see cref="File"/> if found.</param>
        /// <returns>A value indicating if a file was found.</returns>
        public static bool TryFind(string projectFile, out FileInfo result)
        {
            result = null;
            if (SolutionFile.TryFind(Assembly.GetCallingAssembly(), out var sln))
            {
                // ReSharper disable once PossibleNullReferenceException
                result = sln.Directory.EnumerateFiles(projectFile, SearchOption.AllDirectories).FirstOrDefault();
            }

            return result != null;
        }

        /// <summary>
        /// Searches parent directories for <paramref name="projectFile"/>
        /// </summary>
        /// <param name="projectFile">Ex Foo.csproj</param>
        /// <returns>The project file.</returns>
        public static FileInfo Find(string projectFile)
        {
            if (SolutionFile.TryFind(Assembly.GetCallingAssembly(), out var sln))
            {
                // ReSharper disable once PossibleNullReferenceException
                var result = sln.Directory.EnumerateFiles(projectFile, SearchOption.AllDirectories).FirstOrDefault();
                if (result == null)
                {
                    throw new InvalidOperationException("Did not find a file named: " + projectFile);
                }

                return result;
            }

            throw new InvalidOperationException("Did not find a sln for: " + Assembly.GetCallingAssembly());
        }
    }
}