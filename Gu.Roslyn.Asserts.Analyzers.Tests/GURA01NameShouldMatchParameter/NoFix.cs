namespace Gu.Roslyn.Asserts.Analyzers.Tests.GURA01NameShouldMatchParameter
{
    using NUnit.Framework;

    public static class NoFix
    {
        private static readonly DiagnosticFixAssert Assert = RoslynAssert.Create<InvocationAnalyzer, RenameFix>();
        private static readonly ExpectedDiagnostic ExpectedDiagnostic = ExpectedDiagnostic.Create(Descriptors.GURA01NameShouldMatchParameter);

        [Test]
        public static void WhenNameCollision()
        {
            var code = @"
namespace N
{
    using Gu.Roslyn.Asserts;
    using NUnit.Framework;

    public static class C
    {
        private static readonly PlaceholderAnalyzer Analyzer = new PlaceholderAnalyzer();

        [TestCase(""C { }"")]
        public static void M(string code)
        {
            var wrong = ""class C { }"";
            RoslynAssert.Valid(Analyzer, ↓wrong);
        }
    }
}";

            var expectedDiagnostic = ExpectedDiagnostic.WithMessage("Name of 'wrong' should be 'code'.");
            Assert.NoFix(expectedDiagnostic, Code.PlaceholderAnalyzer, Code.PlaceholderFix, code);
        }
    }
}
