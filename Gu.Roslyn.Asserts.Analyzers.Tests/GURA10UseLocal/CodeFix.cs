namespace Gu.Roslyn.Asserts.Analyzers.Tests.GURA10UseLocal
{
    using NUnit.Framework;

    public static class CodeFix
    {
        private static readonly DiagnosticFixAssert Assert = RoslynAssert.Create<InvocationAnalyzer, CopyToLocalFix>(
            ExpectedDiagnostic.Create(Descriptors.GURA10UseLocal));

        [Test]
        public static void StaticField()
        {
            var before = @"
namespace N
{
    using Gu.Roslyn.Asserts;
    using NUnit.Framework;

    public static class Diagnostics
    {
        private static readonly PlaceholderAnalyzer Analyzer = new PlaceholderAnalyzer();
        private static readonly string C1 = ""class C1 { }"";

        [TestCase(""C2 { }"")]
        public static void M(string declaration)
        {
            var code = ""class C2 { }"".AssertReplace(""C2 { }"", declaration);
            RoslynAssert.Diagnostics(Analyzer, ↓C1, code);
        }
    }
}";

            var after = @"
namespace N
{
    using Gu.Roslyn.Asserts;
    using NUnit.Framework;

    public static class Diagnostics
    {
        private static readonly PlaceholderAnalyzer Analyzer = new PlaceholderAnalyzer();
        private static readonly string C1 = ""class C1 { }"";

        [TestCase(""C2 { }"")]
        public static void M(string declaration)
        {
            var c1 = ""class C1 { }"";
            var code = ""class C2 { }"".AssertReplace(""C2 { }"", declaration);
            RoslynAssert.Diagnostics(Analyzer, c1, code);
        }
    }
}";
            Assert.CodeFix(new[] { Code.PlaceholderAnalyzer, before }, after);
        }
    }
}
