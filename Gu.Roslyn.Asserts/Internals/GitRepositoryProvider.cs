namespace Gu.Roslyn.Asserts.Internals;

using System;
using System.Linq;

/// <summary>
/// Parses the URLs of various git repository host sites.
/// </summary>
internal static class GitRepositoryProvider
{
    /// <summary>
    /// Factory function for a GitFile.
    /// </summary>
    /// <param name="uri">The url.</param>
    /// <returns>Parsed <see cref="GitFile"/> object.</returns>
    internal static GitFile ParseUrl(Uri uri)
    {
        return new GitFile(uri);
    }

    /// <summary>
    /// A git file.
    /// </summary>
    internal class GitFile
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="GitFile"/> class.
        /// </summary>
        /// <param name="uri">The url.</param>
        internal GitFile(Uri uri)
        {
            // sample URL
            // segment indices:  01111112222222222222222223333344444445555555555555555555555556666666666666666666666666666666666
            // https://github.com/GuOrg/Gu.Roslyn.Asserts/blob/master/Gu.Roslyn.Asserts.Tests/RoslynAssertCodeFixTests.Fail.cs
            var segments = uri.Segments;
            var path = string.Join(string.Empty, segments.Skip(5));
            this.Provider = uri.Host;
            var trim = new[] { '/' };
            this.User = segments[1].TrimEnd(trim);
            this.RepoName = segments[2].TrimEnd(trim);
            this.RepositoryUrl = new Uri(uri, string.Join(string.Empty, segments.Take(3)));
            this.Branch = segments[4].TrimEnd(trim);
            this.Path = path;
        }

        /// <summary>
        /// Gets the name of the provider, e.g. "github.com", "bitbucket.org".
        /// </summary>
        internal string Provider { get; }

        /// <summary>
        /// Gets the user owns the repository.
        /// </summary>
        internal string User { get; }

        /// <summary>
        /// Gets the name of the repository.
        /// </summary>
        internal string RepoName { get; }

        /// <summary>
        /// Gets the URL to the repository, usable with git clone.
        /// </summary>
        internal Uri RepositoryUrl { get; }

        /// <summary>
        /// Gets the name of a branch, or a commit SHA-1 hash.
        /// </summary>
        internal string Branch { get; }

        /// <summary>
        /// Gets the path to the file in the repo.
        /// </summary>
        internal string Path { get; }
    }
}
