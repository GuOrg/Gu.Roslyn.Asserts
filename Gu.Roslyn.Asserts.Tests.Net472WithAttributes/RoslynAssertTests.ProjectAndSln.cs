namespace Gu.Roslyn.Asserts.Tests.Net472WithAttributes
{
    using Gu.Roslyn.Asserts.Tests.Net472WithAttributes.AnalyzersAndFixes;
    using NUnit.Framework;

    public partial class RoslynAssertTests
    {
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
    }
}
