namespace Gu.Roslyn.Asserts.Tests
{
    using System.IO;
    using System.Linq;
    using System.Threading.Tasks;
    using Microsoft.CodeAnalysis;
    using NUnit.Framework;

    public static class AnalyzeTests
    {
        private static readonly MetadataReference[] MetadataReferences = new MetadataReference[0];

        [Test]
        public static async Task GetDiagnosticsAsyncProjectFile()
        {
            Assert.AreEqual(true, ProjectFile.TryFind("Gu.Roslyn.Asserts.csproj", out FileInfo projectFile));
            var diagnostics = await Analyze.GetDiagnosticsAsync(new FieldNameMustNotBeginWithUnderscore(), projectFile, MetadataReferences).ConfigureAwait(false);
            CollectionAssert.IsEmpty(diagnostics.SelectMany(x => x));
        }

        [Test]
        public static async Task GetDiagnosticsAsyncSolutionFile()
        {
            var solutionFile = SolutionFile.Find("Gu.Roslyn.Asserts.sln");
            var diagnostics = await Analyze.GetDiagnosticsAsync(new FieldNameMustNotBeginWithUnderscore(), solutionFile, MetadataReferences)
                                           .ConfigureAwait(false);
            var expected = new[]
                           {
                               "ClassLibrary1Class1.cs(10,21): warning SA1309: Field '_value' must not begin with an underscore",
                               "ClassLibrary2Class1.cs(10,21): warning SA1309: Field '_value' must not begin with an underscore",
                           };
            CollectionAssert.AreEquivalent(expected, diagnostics.SelectMany(x => x).Select(SkipDirectory));
        }

        [Test]
        public static async Task GetDiagnosticsAsyncSolution()
        {
            var solutionFile = SolutionFile.Find("Gu.Roslyn.Asserts.sln");
            var expected = new[]
                           {
                               "ClassLibrary1Class1.cs(10,21): warning SA1309: Field '_value' must not begin with an underscore",
                               "ClassLibrary2Class1.cs(10,21): warning SA1309: Field '_value' must not begin with an underscore",
                           };

            var sln = CodeFactory.CreateSolution(solutionFile, MetadataReferences);
            var analyzer = new FieldNameMustNotBeginWithUnderscore();
            var diagnostics = await Analyze.GetDiagnosticsAsync(sln, analyzer).ConfigureAwait(false);
            CollectionAssert.AreEquivalent(expected, diagnostics.SelectMany(x => x).Select(SkipDirectory));

            diagnostics = Analyze.GetDiagnostics(analyzer, sln);
            CollectionAssert.AreEquivalent(expected, diagnostics.SelectMany(x => x).Select(SkipDirectory));
        }

        private static string SkipDirectory(Diagnostic diagnostic)
        {
            var text = diagnostic.ToString();
            return text.Substring(text.LastIndexOf('\\') + 1);
        }
    }
}
