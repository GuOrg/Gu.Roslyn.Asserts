namespace Gu.Roslyn.Asserts.Tests
{
    using System.IO;
    using Gu.Roslyn.Asserts.Internals;
    using NUnit.Framework;

    [TestFixture]
    public class GitClientTests
    {
        [Test]
        public void Basic()
        {
            var git = new GitClient();
            string tempDirectory = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
            var directoryInfo = Directory.CreateDirectory(tempDirectory);
            git.Clone(
                new System.Uri("https://github.com/GuOrg/Gu.Inject"),
                directoryInfo.FullName,
                GitClient.CloneFlags.Shallow);
        }
    }
}
