namespace Gu.Roslyn.Asserts.Tests
{
    using System.IO;
    using System.Linq;
    using System.Threading.Tasks;
    using Microsoft.CodeAnalysis;
    using NUnit.Framework;

    public class AnalyzeTests
    {
        private static readonly MetadataReference[] MetadataReferences = new MetadataReference[0];

        [Test]
        public async Task AnalyzeProjectFile()
        {
            Assert.AreEqual(true, ProjectFile.TryFind("Gu.Roslyn.Asserts.csproj", out FileInfo projectFile));
            var diagnostics = await Analyze.GetDiagnosticsAsync(new FieldNameMustNotBeginWithUnderscore(), projectFile, MetadataReferences)
                                           .ConfigureAwait(false);
            CollectionAssert.IsEmpty(diagnostics.SelectMany(x => x));
        }

        [Test]
        public async Task AnalyzeSolutionFile()
        {
            Assert.AreEqual(true, SolutionFile.TryFind("Gu.Roslyn.Asserts.sln", out FileInfo solutionFile));
            var diagnostics = await Analyze.GetDiagnosticsAsync(new FieldNameMustNotBeginWithUnderscore(), solutionFile, MetadataReferences)
                                           .ConfigureAwait(false);
            var expected = new[]
                           {
                               "ClassLibrary1Class1.cs(8,21): warning SA1309: Field '_value' must not begin with an underscore",
                               "ClassLibrary2Class1.cs(8,21): warning SA1309: Field '_value' must not begin with an underscore",
                           };
            CollectionAssert.AreEquivalent(expected, diagnostics.SelectMany(x => x).Select(x => x.ToString()));
        }
    }
}
