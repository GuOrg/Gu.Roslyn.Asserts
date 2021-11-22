namespace Gu.Roslyn.Asserts.Tests
{
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using NUnit.Framework;

    public static class ProjectFileTests
    {
        [Test]
        public static void TryFindFromDll()
        {
            var dll = new FileInfo(Assembly.GetExecutingAssembly().Location);
            Assert.AreEqual(true, ProjectFile.TryFind(dll, out var projectFile));
            Assert.AreEqual(Path.GetFileNameWithoutExtension(dll.FullName) + ".csproj", projectFile!.Name);
        }

        [TestCase("Gu.Roslyn.Asserts.Tests.csproj")]
        [TestCase("WpfApp1.csproj")]
        public static void TryFindByName(string name)
        {
            Assert.AreEqual(true, ProjectFile.TryFind(name, out var projectFile));
            Assert.AreEqual(name, projectFile!.Name);
        }

        [TestCase("Gu.Roslyn.Asserts.Tests.csproj")]
        [TestCase("WpfApp1.csproj")]
        public static void Find(string name)
        {
            var projectFile = ProjectFile.Find(name);
            Assert.AreEqual(name, projectFile.Name);
        }

        [Test]
        public static void ParseInfo()
        {
            var file = ProjectFile.Find("ClassLibrary1.csproj");
            var csproj = ProjectFile.ParseInfo(file);
            var expected = new[]
            {
                "AllowCompilationErrors.cs",
                "AssemblyAttributes.cs",
                "ClassLibrary1Class1.cs",
                "AssemblyInfo.cs",
            };
            CollectionAssert.AreEquivalent(expected, csproj.Documents.Select(x => x.Name));
            foreach (var document in csproj.Documents)
            {
                Assert.AreEqual(true, File.Exists(document.FilePath));
            }
        }
    }
}
