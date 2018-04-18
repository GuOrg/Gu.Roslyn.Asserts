namespace Gu.Roslyn.Asserts.Tests
{
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
    }
}
