namespace Gu.Roslyn.Asserts.Analyzers.Tests.GURA09UseStandardNames
{
    using NUnit.Framework;

    public static partial class CodeFix
    {
        public static class Property
        {
            [TestCase("public int ↓Foo { get; }", "public int P { get; }")]
            [TestCase("public int ↓A { get; }", "public int P { get; }")]
            [TestCase("public int ↓Foo { get; set; }", "public int P { get; set; }")]
            public static void Single(string declarationBefore, string declarationAfter)
            {
                var before = @"
namespace N
{
    using Gu.Roslyn.Asserts;
    using NUnit.Framework;

    public static class Valid
    {
        private static readonly PlaceholderAnalyzer Analyzer = new PlaceholderAnalyzer();

        [Test]
        public static void M1()
        {
            var c = @""
namespace N
{
    class C
    {
        public int ↓Foo { get; }
    }
}"";
            RoslynAssert.Valid(Analyzer, c);
        }
    }
}".AssertReplace("public int ↓Foo { get; }", declarationBefore);

                var after = @"
namespace N
{
    using Gu.Roslyn.Asserts;
    using NUnit.Framework;

    public static class Valid
    {
        private static readonly PlaceholderAnalyzer Analyzer = new PlaceholderAnalyzer();

        [Test]
        public static void M1()
        {
            var c = @""
namespace N
{
    class C
    {
        public int P { get; }
    }
}"";
            RoslynAssert.Valid(Analyzer, c);
        }
    }
}".AssertReplace("public int P { get; }", declarationAfter);

                RoslynAssert.CodeFix(Analyzer, Fix, ExpectedDiagnostic, new[] { Code.PlaceholderAnalyzer, before }, new[] { Code.PlaceholderAnalyzer, after });
            }
        }
    }
}
