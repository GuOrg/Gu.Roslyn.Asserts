namespace Gu.Roslyn.Asserts.Analyzers.Tests.GURA11ChainAssertReplace
{
    using Microsoft.CodeAnalysis.CodeFixes;
    using Microsoft.CodeAnalysis.Diagnostics;
    using NUnit.Framework;

    public static class CodeFix
    {
        private static readonly DiagnosticAnalyzer Analyzer = new InvocationAnalyzer();
        private static readonly CodeFixProvider Fix = new ChainAssertReplaceFix();
        private static readonly ExpectedDiagnostic ExpectedDiagnostic = ExpectedDiagnostic.Create(Descriptors.GURA11ChainAssertReplace);

        [Test]
        public static void Local()
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
            var code = ""class C2 { }"";
            code = code.↓AssertReplace(""C2 { }"", declaration);
            RoslynAssert.Diagnostics(Analyzer, C1, code);
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
            var code = ""class C2 { }"".AssertReplace(""C2 { }"", declaration);
            RoslynAssert.Diagnostics(Analyzer, C1, code);
        }
    }
}";
            RoslynAssert.CodeFix(Analyzer, Fix, ExpectedDiagnostic, new[] { Code.PlaceholderAnalyzer, before }, after);
        }
    }
}
