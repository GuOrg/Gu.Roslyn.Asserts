namespace Gu.Roslyn.Asserts.Analyzers.Tests.GURA09UseStandardNames
{
    using NUnit.Framework;

    public static class NoFix
    {
        private static readonly DiagnosticFixAssert Assert = RoslynAssert.Create<InvocationAnalyzer, StandardNamesFix>(
             ExpectedDiagnostic.Create(Descriptors.GURA09UseStandardNames));

        [Test]
        public static void ParameterNameFoo()
        {
            var code = @"
namespace N
{
    using Gu.Roslyn.Asserts;
    using NUnit.Framework;

    public static class Valid
    {
        private static readonly PlaceholderAnalyzer Analyzer = new PlaceholderAnalyzer();

        [Test]
        public static void M()
        {
            var c = @""
namespace N
{
    class C
    {
        c(int ↓foo) { }
    }
}"";
            RoslynAssert.Valid(Analyzer, c);
        }
    }
}";
            Assert.NoFix(Code.PlaceholderAnalyzer, code);
        }
    }
}
