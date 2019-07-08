namespace Gu.Roslyn.Asserts.Tests.Net472WithAttributes
{
    using Gu.Roslyn.Asserts.Tests.Net472WithAttributes.AnalyzersAndFixes;
    using NUnit.Framework;

    public partial class RoslynAssertTests
    {
        [Test]
        public void ProjectFileNoErrorAnalyzer()
        {
            var csproj = ProjectFile.Find("Gu.Roslyn.Asserts.csproj");
            var analyzer = new NoErrorAnalyzer();

            RoslynAssert.Valid(typeof(NoErrorAnalyzer), csproj);
            RoslynAssert.Valid(analyzer, csproj);

            var descriptor = NoErrorAnalyzer.Descriptor;
            RoslynAssert.Valid(typeof(NoErrorAnalyzer), descriptor, csproj);
            RoslynAssert.Valid(analyzer, descriptor, csproj);
            RoslynAssert.Valid(analyzer, csproj, CodeFactory.DefaultCompilationOptions(analyzer, descriptor, null), RoslynAssert.MetadataReferences);
        }

        [Test]
        public void ClassLibrary1NoErrorAnalyzer()
        {
            var analyzer = new NoErrorAnalyzer();
            var csproj = ProjectFile.Find("ClassLibrary1.csproj");
            var descriptor = NoErrorAnalyzer.Descriptor;
            RoslynAssert.Valid(typeof(NoErrorAnalyzer), descriptor, csproj);
            RoslynAssert.Valid(analyzer, descriptor, csproj);
            RoslynAssert.Valid(analyzer, csproj, CodeFactory.DefaultCompilationOptions(analyzer, descriptor, null), RoslynAssert.MetadataReferences);
        }

        [Test]
        public void WpfAppNoErrorAnalyzer()
        {
            var analyzer = new NoErrorAnalyzer();
            var csproj = ProjectFile.Find("WpfApp1.csproj");
            var descriptor = NoErrorAnalyzer.Descriptor;
            RoslynAssert.Valid(typeof(NoErrorAnalyzer), descriptor, csproj);
            RoslynAssert.Valid(analyzer, descriptor, csproj);
            RoslynAssert.Valid(analyzer, csproj, CodeFactory.DefaultCompilationOptions(analyzer, descriptor, null), RoslynAssert.MetadataReferences);
        }

        [Explicit("Need to solve signing and InternalsVisibleTo")]
        [Test]
        public void SolutionFileNoErrorAnalyzer()
        {
            var sln = SolutionFile.Find("Gu.Roslyn.Asserts.sln");
            var analyzer = new NoErrorAnalyzer();
            RoslynAssert.Valid(typeof(NoErrorAnalyzer), sln);
            RoslynAssert.Valid(analyzer, sln);

            var descriptor = NoErrorAnalyzer.Descriptor;
            RoslynAssert.Valid(typeof(NoErrorAnalyzer), descriptor, sln);
            RoslynAssert.Valid(analyzer, descriptor, sln);
        }

        [Explicit("Need to solve signing and InternalsVisibleTo")]
        [Test]
        public void SolutionNoErrorAnalyzer()
        {
            var sln = SolutionFile.Find("Gu.Roslyn.Asserts.sln");
            var solution = CodeFactory.CreateSolution(sln, new[] { new NoErrorAnalyzer() }, RoslynAssert.MetadataReferences);
            RoslynAssert.Valid(typeof(NoErrorAnalyzer), solution);
            RoslynAssert.Valid(new NoErrorAnalyzer(), solution);
        }
    }
}
