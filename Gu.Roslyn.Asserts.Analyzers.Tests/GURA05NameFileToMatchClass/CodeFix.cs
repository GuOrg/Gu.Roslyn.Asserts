namespace Gu.Roslyn.Asserts.Analyzers.Tests.GURA05NameFileToMatchClass
{
    using NUnit.Framework;

    public static class CodeFix
    {
        private static readonly DiagnosticFixAssert Assert = RoslynAssert.Create<ClassDeclarationAnalyzer, RenameFix>(
            ExpectedDiagnostic.Create(Descriptors.GURA04NameClassToMatchAsserts));

        [Test]
        public static void WhenValidCode()
        {
            var before = @"
namespace N
{
    using Gu.Roslyn.Asserts;
    using NUnit.Framework;

    internal static class ↓ValidCode
    {
        private static readonly PlaceholderAnalyzer Analyzer = new PlaceholderAnalyzer();

        [Test]
        public static void M1()
        {
            var c = ""class C { }"";
            RoslynAssert.Valid(Analyzer, c);
        }
    }
}";

            var after = @"
namespace N
{
    using Gu.Roslyn.Asserts;
    using NUnit.Framework;

    internal static class Valid
    {
        private static readonly PlaceholderAnalyzer Analyzer = new PlaceholderAnalyzer();

        [Test]
        public static void M1()
        {
            var c = ""class C { }"";
            RoslynAssert.Valid(Analyzer, c);
        }
    }
}";

            Assert.CodeFix(new[] { Code.PlaceholderAnalyzer, before }, new[] { Code.PlaceholderAnalyzer, after });
        }
    }
}
