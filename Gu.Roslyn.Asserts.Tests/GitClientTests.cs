namespace Gu.Roslyn.Asserts.Tests
{
    using System.IO;
    using Gu.Roslyn.Asserts.Internals;
    using NUnit.Framework;

    public static class GitClientTests
    {
        [Test]
        public static void Basic()
        {
            var git = new GitClient();
            var tempDirectory = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
            var directoryInfo = Directory.CreateDirectory(tempDirectory);
            git.Clone(
                new System.Uri("https://github.com/GuOrg/Gu.Inject"),
                directoryInfo.FullName,
                GitClient.CloneFlags.Shallow);
        }
    }
}
