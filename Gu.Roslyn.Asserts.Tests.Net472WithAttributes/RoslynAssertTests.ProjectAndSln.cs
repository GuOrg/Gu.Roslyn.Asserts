namespace Gu.Roslyn.Asserts.Tests.Net472WithAttributes
{
    using Gu.Roslyn.Asserts.Tests.Net472WithAttributes.AnalyzersAndFixes;
    using NUnit.Framework;

    public partial class RoslynAssertTests
    {
        [Test]
        public void ProjectFileNopAnalyzer()
        {
            var code = ProjectFile.Find("Gu.Roslyn.Asserts.csproj");
            var analyzer = new NopAnalyzer();

            RoslynAssert.Valid(typeof(NopAnalyzer), code);
            RoslynAssert.Valid(analyzer, code);

            var descriptor = NopAnalyzer.Descriptor;
            RoslynAssert.Valid(typeof(NopAnalyzer), descriptor, code);
            RoslynAssert.Valid(analyzer, descriptor, code);
            RoslynAssert.Valid(analyzer, code, compilationOptions: CodeFactory.DefaultCompilationOptions(analyzer, descriptor, null));
        }

        [Test]
        public void ClassLibrary1NopAnalyzer()
        {
            var analyzer = new NopAnalyzer();
            var code = ProjectFile.Find("ClassLibrary1.csproj");
            var descriptor = NopAnalyzer.Descriptor;
            RoslynAssert.Valid(typeof(NopAnalyzer), descriptor, code);
            RoslynAssert.Valid(analyzer, descriptor, code);
            RoslynAssert.Valid(analyzer, code, compilationOptions: CodeFactory.DefaultCompilationOptions(analyzer, descriptor, null));
        }

        [Test]
        public void WpfAppNopAnalyzer()
        {
            var analyzer = new NopAnalyzer();
            var code = ProjectFile.Find("WpfApp1.csproj");
            var descriptor = NopAnalyzer.Descriptor;
            RoslynAssert.Valid(typeof(NopAnalyzer), descriptor, code);
            RoslynAssert.Valid(analyzer, descriptor, code);
            RoslynAssert.Valid(analyzer, code, compilationOptions: CodeFactory.DefaultCompilationOptions(analyzer, descriptor, null));
        }

        [Explicit("Need to solve signing and InternalsVisibleTo")]
        [Test]
        public void SolutionFileNopAnalyzer()
        {
            var code = SolutionFile.Find("Gu.Roslyn.Asserts.sln");
            var analyzer = new NopAnalyzer();
            RoslynAssert.Valid(typeof(NopAnalyzer), code);
            RoslynAssert.Valid(analyzer, code);

            var descriptor = NopAnalyzer.Descriptor;
            RoslynAssert.Valid(typeof(NopAnalyzer), descriptor, code);
            RoslynAssert.Valid(analyzer, descriptor, code);
        }

        [Explicit("Need to solve signing and InternalsVisibleTo")]
        [Test]
        public void SolutionNopAnalyzer()
        {
            var sln = SolutionFile.Find("Gu.Roslyn.Asserts.sln");
            var solution = CodeFactory.CreateSolution(sln, new[] { new NopAnalyzer() }, RoslynAssert.MetadataReferences);
            RoslynAssert.Valid(typeof(NopAnalyzer), solution);
            RoslynAssert.Valid(new NopAnalyzer(), solution);
        }
    }
}
