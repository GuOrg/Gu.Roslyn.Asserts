namespace Gu.Roslyn.Asserts.Tests.WithMetadataReferencesAttribute
{
    using Gu.Roslyn.Asserts.Tests.WithMetadataReferencesAttribute.AnalyzersAndFixes;
    using NUnit.Framework;

    public partial class AnalyzerAssertTests
    {
        [Test]
        public void ProjectFileNoErrorAnalyzer()
        {
            var csproj = ProjectFile.Find("Gu.Roslyn.Asserts.csproj");
            var analyzer = new NoErrorAnalyzer();

            AnalyzerAssert.Valid<NoErrorAnalyzer>(csproj);
            AnalyzerAssert.Valid(typeof(NoErrorAnalyzer), csproj);
            AnalyzerAssert.Valid(analyzer, csproj);

            var descriptor = NoErrorAnalyzer.Descriptor;
            AnalyzerAssert.Valid<NoErrorAnalyzer>(descriptor, csproj);
            AnalyzerAssert.Valid(typeof(NoErrorAnalyzer), descriptor, csproj);
            AnalyzerAssert.Valid(analyzer, descriptor, csproj);
            AnalyzerAssert.Valid(analyzer, csproj, CodeFactory.DefaultCompilationOptions(analyzer, descriptor, null), AnalyzerAssert.MetadataReferences);
        }

        [Test]
        public void ClassLibrary1NoErrorAnalyzer()
        {
            var analyzer = new NoErrorAnalyzer();
            var csproj = ProjectFile.Find("ClassLibrary1.csproj");
            var descriptor = NoErrorAnalyzer.Descriptor;
            AnalyzerAssert.Valid<NoErrorAnalyzer>(descriptor, csproj);
            AnalyzerAssert.Valid(typeof(NoErrorAnalyzer), descriptor, csproj);
            AnalyzerAssert.Valid(analyzer, descriptor, csproj);
            AnalyzerAssert.Valid(analyzer, csproj, CodeFactory.DefaultCompilationOptions(analyzer, descriptor, null), AnalyzerAssert.MetadataReferences);
        }

        [Test]
        public void WpfAppNoErrorAnalyzer()
        {
            var analyzer = new NoErrorAnalyzer();
            var csproj = ProjectFile.Find("WpfApp1.csproj");
            var descriptor = NoErrorAnalyzer.Descriptor;
            AnalyzerAssert.Valid<NoErrorAnalyzer>(descriptor, csproj);
            AnalyzerAssert.Valid(typeof(NoErrorAnalyzer), descriptor, csproj);
            AnalyzerAssert.Valid(analyzer, descriptor, csproj);
            AnalyzerAssert.Valid(analyzer, csproj, CodeFactory.DefaultCompilationOptions(analyzer, descriptor, null), AnalyzerAssert.MetadataReferences);
        }

        [Explicit("Need to solve signing and InternalsVisibleTo")]
        [Test]
        public void SolutionFileNoErrorAnalyzer()
        {
            var sln = SolutionFile.Find("Gu.Roslyn.Asserts.sln");
            var analyzer = new NoErrorAnalyzer();
            AnalyzerAssert.Valid<NoErrorAnalyzer>(sln);
            AnalyzerAssert.Valid(typeof(NoErrorAnalyzer), sln);
            AnalyzerAssert.Valid(analyzer, sln);

            var descriptor = NoErrorAnalyzer.Descriptor;
            AnalyzerAssert.Valid<NoErrorAnalyzer>(descriptor, sln);
            AnalyzerAssert.Valid(typeof(NoErrorAnalyzer), descriptor, sln);
            AnalyzerAssert.Valid(analyzer, descriptor, sln);
        }

        [Explicit("Need to solve signing and InternalsVisibleTo")]
        [Test]
        public void SolutionNoErrorAnalyzer()
        {
            var sln = SolutionFile.Find("Gu.Roslyn.Asserts.sln");
            var solution = CodeFactory.CreateSolution(sln, new[] { new NoErrorAnalyzer() }, AnalyzerAssert.MetadataReferences);
            AnalyzerAssert.Valid<NoErrorAnalyzer>(solution);
            AnalyzerAssert.Valid(typeof(NoErrorAnalyzer), solution);
            AnalyzerAssert.Valid(new NoErrorAnalyzer(), solution);
        }
    }
}
