namespace Gu.Roslyn.Asserts.Tests
{
    using System.Linq;
    using NUnit.Framework;

    public class SolutionFileTests
    {
        [Test]
        public void TryFind()
        {
            Assert.AreEqual(true, SolutionFile.TryFind("Gu.Roslyn.Asserts.sln", out var sln));
            Assert.AreEqual("Gu.Roslyn.Asserts.sln", sln.Name);
            sln = SolutionFile.Find("Gu.Roslyn.Asserts.sln");
            Assert.AreEqual("Gu.Roslyn.Asserts.sln", sln.Name);
        }

        [Test]
        public void Find()
        {
            var sln = SolutionFile.Find("Gu.Roslyn.Asserts.sln");
            Assert.AreEqual("Gu.Roslyn.Asserts.sln", sln.Name);
        }

        [Test]
        public void ParseInfo()
        {
            var file = SolutionFile.Find("Gu.Roslyn.Asserts.sln");
            var sln = SolutionFile.ParseInfo(file);
            var expected = new[]
                           {
                               "Gu.Roslyn.Asserts.Tests.WithMetadataReferencesAttribute",
                               "Gu.Roslyn.Asserts",
                               "WpfApp1",
                               "Gu.Roslyn.Asserts.Tests",
                               "ClassLibrary2",
                               "ClassLibrary1",
                           };
            CollectionAssert.AreEquivalent(expected, sln.Projects.Select(x => x.Name));
            var assertsProject = sln.Projects.Single(x => x.Name == "Gu.Roslyn.Asserts");
            CollectionAssert.IsEmpty(assertsProject.ProjectReferences);

            var testProject = sln.Projects.Single(x => x.Name == "Gu.Roslyn.Asserts.Tests");
            CollectionAssert.AreEqual(new[] { assertsProject.Id }, testProject.ProjectReferences.Select(x => x.ProjectId).ToArray());
        }
    }
}
