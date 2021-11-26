namespace Gu.Roslyn.Asserts
{
    using System;
    using System.Collections.Immutable;
    using System.Diagnostics.CodeAnalysis;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using System.Text.RegularExpressions;

    using Microsoft.CodeAnalysis;

    /// <summary>
    /// Helper methods for working with .sln files.
    /// </summary>
    public static class SolutionFile
    {
        /// <summary>
        /// Searches parent directories for <paramref name="assembly"/> the first file matching *.sln.
        /// </summary>
        /// <param name="assembly">The <see cref="Assembly"/>.</param>
        /// <param name="sln">The <see cref="File"/> if found.</param>
        /// <returns>A value indicating if a file was found.</returns>
        public static bool TryFind(Assembly assembly, [NotNullWhen(true)] out FileInfo? sln)
        {
            if (assembly is null)
            {
                throw new ArgumentNullException(nameof(assembly));
            }

            sln = null;
            return assembly.CodeBase is { } codeBase &&
                   new FileInfo(new Uri(codeBase, UriKind.Absolute).LocalPath) is { Directory: { } directory } &&
                   CodeFactory.TryFindFileInParentDirectory(directory, "*.sln", out sln);
        }

        /// <summary>
        /// Searches parent directories for <paramref name="name"/> the first file matching Foo.sln.
        /// </summary>
        /// <param name="name">The assembly.</param>
        /// <param name="sln">The <see cref="File"/> if found.</param>
        /// <returns>A value indicating if a file was found.</returns>
        public static bool TryFind(string name, [NotNullWhen(true)] out FileInfo? sln)
        {
            sln = null;
            return Assembly.GetCallingAssembly().CodeBase is { } codeBase &&
                   new FileInfo(new Uri(codeBase, UriKind.Absolute).LocalPath) is { Directory: { } directory } &&
                   CodeFactory.TryFindFileInParentDirectory(directory, name, out sln);
        }

        /// <summary>
        /// Searches parent directories for <paramref name="name"/> the first file matching Foo.sln.
        /// </summary>
        /// <param name="name">The name of the sln file.</param>
        /// <returns>The solution file.</returns>
        public static FileInfo Find(string name)
        {
            if (Assembly.GetCallingAssembly().CodeBase is { } codeBase &&
                new FileInfo(new Uri(codeBase, UriKind.Absolute).LocalPath) is { Directory: { } directory } &&
                CodeFactory.TryFindFileInParentDirectory(directory, name, out var sln))
            {
                return sln;
            }

            throw new InvalidOperationException("Did not find a .sln file named: " + name);
        }

        /// <summary>
        /// Searches parent directories for <paramref name="assembly"/> and return the the first .sln file found.
        /// </summary>
        /// <param name="assembly">The <see cref="Assembly"/>.</param>
        /// <returns>The solution file.</returns>
        public static FileInfo Find(Assembly assembly)
        {
            if (assembly is null)
            {
                throw new ArgumentNullException(nameof(assembly));
            }

            if (TryFind(assembly, out var sln))
            {
                return sln;
            }

            if (assembly.CodeBase is { } codebase)
            {
                throw new InvalidOperationException($"Did not find a .sln file in recursive parent directories of assembly.CodeBase: {new Uri(codebase, UriKind.Absolute).LocalPath}");
            }

            throw new InvalidOperationException($"Did not find a .sln file in recursive parent directories of assembly.CodeBase is null");
        }

        /// <summary>
        /// Parse a <see cref="SolutionInfo"/> from <paramref name="sln"/>.
        /// </summary>
        /// <param name="sln">The solution file.</param>
        /// <returns>A <see cref="SolutionInfo"/>.</returns>
        public static SolutionInfo ParseInfo(FileInfo sln)
        {
            if (sln is null)
            {
                throw new ArgumentNullException(nameof(sln));
            }

            var contents = File.ReadAllText(sln.FullName);
            var builder = ImmutableDictionary.CreateBuilder<ProjectId, FileInfo>();
            foreach (Match? match in Regex.Matches(contents, @"Project\(""[^ ""]+""\) = ""(?<name>\w+(\.\w+)*)\"", ?""(?<path>\w+(\.\w+)*(\\\w+(\.\w+)*)*.csproj)", RegexOptions.ExplicitCapture))
            {
                if (match is null)
                {
                    throw new FormatException($"Error parsing {sln}");
                }

                var projectFile = new FileInfo(Path.Combine(sln.DirectoryName, match.Groups["path"].Value));
                builder.Add(ProjectId.CreateNewId(projectFile.FullName), projectFile);
            }

            var idFileMap = builder.ToImmutable();
            return SolutionInfo.Create(
                SolutionId.CreateNewId(sln.FullName),
                VersionStamp.Create(sln.LastWriteTimeUtc),
                sln.FullName,
                idFileMap.Keys.Select(x => ProjectFile.ParseInfo(x, idFileMap)));
        }
    }
}
