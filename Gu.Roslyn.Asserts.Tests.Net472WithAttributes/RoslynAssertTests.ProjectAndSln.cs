namespace Gu.Roslyn.Asserts.Tests.Net472WithAttributes
{
    using Gu.Roslyn.Asserts.Tests.Net472WithAttributes.AnalyzersAndFixes;
    using NUnit.Framework;

    public partial class RoslynAssertTests
    {
        [Test]
        public void ProjectFileNoErrorAnalyzer()
        {
            var code = ProjectFile.Find("Gu.Roslyn.Asserts.csproj");
            var analyzer = new NoErrorAnalyzer();

            RoslynAssert.Valid(typeof(NoErrorAnalyzer), code);
            RoslynAssert.Valid(analyzer, code);

            var descriptor = NoErrorAnalyzer.Descriptor;
            RoslynAssert.Valid(typeof(NoErrorAnalyzer), descriptor, code);
            RoslynAssert.Valid(analyzer, descriptor, code);
            RoslynAssert.Valid(analyzer, code, compilationOptions: CodeFactory.DefaultCompilationOptions(analyzer, descriptor, null));
        }

        [Test]
        public void ClassLibrary1NoErrorAnalyzer()
        {
            var analyzer = new NoErrorAnalyzer();
            var code = ProjectFile.Find("ClassLibrary1.csproj");
            var descriptor = NoErrorAnalyzer.Descriptor;
            RoslynAssert.Valid(typeof(NoErrorAnalyzer), descriptor, code);
            RoslynAssert.Valid(analyzer, descriptor, code);
            RoslynAssert.Valid(analyzer, code, compilationOptions: CodeFactory.DefaultCompilationOptions(analyzer, descriptor, null));
        }

        [Test]
        public void WpfAppNoErrorAnalyzer()
        {
            var analyzer = new NoErrorAnalyzer();
            var code = ProjectFile.Find("WpfApp1.csproj");
            var descriptor = NoErrorAnalyzer.Descriptor;
            RoslynAssert.Valid(typeof(NoErrorAnalyzer), descriptor, code);
            RoslynAssert.Valid(analyzer, descriptor, code);
            RoslynAssert.Valid(analyzer, code, compilationOptions: CodeFactory.DefaultCompilationOptions(analyzer, descriptor, null));
        }

        [Explicit("Need to solve signing and InternalsVisibleTo")]
        [Test]
        public void SolutionFileNoErrorAnalyzer()
        {
            var code = SolutionFile.Find("Gu.Roslyn.Asserts.sln");
            var analyzer = new NoErrorAnalyzer();
            RoslynAssert.Valid(typeof(NoErrorAnalyzer), code);
            RoslynAssert.Valid(analyzer, code);

            var descriptor = NoErrorAnalyzer.Descriptor;
            RoslynAssert.Valid(typeof(NoErrorAnalyzer), descriptor, code);
            RoslynAssert.Valid(analyzer, descriptor, code);
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
