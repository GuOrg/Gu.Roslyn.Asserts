namespace Gu.Roslyn.Asserts.Tests
{
    using NUnit.Framework;

    public class SolutionFileTests
    {
        [Test]
        public void FindSolutionFile()
        {
            Assert.AreEqual(true, SolutionFile.TryFind("Gu.Roslyn.Asserts.sln", out var sln));
            Assert.AreEqual("Gu.Roslyn.Asserts.sln", sln.Name);
            sln = SolutionFile.Find("Gu.Roslyn.Asserts.sln");
            Assert.AreEqual("Gu.Roslyn.Asserts.sln", sln.Name);
        }
    }
}