namespace Gu.Roslyn.Asserts.Tests
{
    using System.Linq;
    using NUnit.Framework;

    public static class SolutionFileTests
    {
        [Test]
        public static void TryFind()
        {
            Assert.AreEqual(true, SolutionFile.TryFind("Gu.Roslyn.Asserts.sln", out var sln));
            Assert.AreEqual("Gu.Roslyn.Asserts.sln", sln!.Name);
            sln = SolutionFile.Find("Gu.Roslyn.Asserts.sln");
            Assert.AreEqual("Gu.Roslyn.Asserts.sln", sln.Name);
        }

        [Test]
        public static void Find()
        {
            var sln = SolutionFile.Find("Gu.Roslyn.Asserts.sln");
            Assert.AreEqual("Gu.Roslyn.Asserts.sln", sln.Name);
        }

        [Test]
        public static void ParseInfo()
        {
            var file = SolutionFile.Find("Gu.Roslyn.Asserts.sln");
            var sln = SolutionFile.ParseInfo(file);
            var expected = new[]
            {
                "Gu.Roslyn.Asserts",
                "Gu.Roslyn.Asserts.Analyzers",
                "Gu.Roslyn.Asserts.Analyzers.Tests",
                "Gu.Roslyn.Asserts.Analyzers.Vsix",
                "Gu.Roslyn.Asserts.Tests",
                "AstView",
                "WpfApp1",
                "ClassLibrary2",
                "ClassLibrary1",
            };
            CollectionAssert.AreEquivalent(expected, sln.Projects.Select(x => x.Name));
            var assertsProject = sln.Projects.Single(x => x.Name == "Gu.Roslyn.Asserts");

            var analyzersProject = sln.Projects.Single(x => x.Name == "Gu.Roslyn.Asserts.Analyzers");
            CollectionAssert.IsEmpty(analyzersProject.ProjectReferences);
            var testProject = sln.Projects.Single(x => x.Name == "Gu.Roslyn.Asserts.Tests");
            CollectionAssert.AreEqual(new[] { assertsProject.Id }, testProject.ProjectReferences.Select(x => x.ProjectId).ToArray());
        }
    }
}
