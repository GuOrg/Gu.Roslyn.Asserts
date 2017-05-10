namespace Gu.Roslyn.Asserts.Tests
{
    using System;
    using System.IO;
    using System.Reflection;
    using NUnit.Framework;

    public class CodeFactoryTests
    {
        [Test]
        public void TryFindFileInParentDirectory()
        {
            var codeBase = Assembly.GetExecutingAssembly().CodeBase;
            var dllFile = new Uri(codeBase, UriKind.Absolute).LocalPath;
            var directory = new DirectoryInfo(Path.GetDirectoryName(dllFile));
            Assert.AreEqual(true, CodeFactory.TryFindFileInParentDirectory(directory, "Gu.Roslyn.Asserts.Tests.csproj", out FileInfo projectFile));
            Assert.AreEqual("Gu.Roslyn.Asserts.Tests.csproj", projectFile.Name);
        }
    }
}