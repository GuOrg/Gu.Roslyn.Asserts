namespace Gu.Roslyn.Asserts.Analyzers.Tests.GURA02IndicateErrorPosition
{
    using Microsoft.CodeAnalysis.CodeFixes;
    using Microsoft.CodeAnalysis.Diagnostics;
    using NUnit.Framework;

    public static class FixAll
    {
        private static readonly DiagnosticAnalyzer Analyzer = new InvocationAnalyzer();
        private static readonly CodeFixProvider Fix = new IndicateErrorPositionFix();
        private static readonly ExpectedDiagnostic ExpectedDiagnostic = ExpectedDiagnostic.Create(Descriptors.GURA02IndicateErrorPosition);

        [Test]
        public static void CodeFixLocals()
        {
            var before = @"
namespace N
{
    using Gu.Roslyn.Asserts;
    using NUnit.Framework;

    public static class C
    {
        private static readonly PlaceholderAnalyzer Analyzer = new PlaceholderAnalyzer();
        private static readonly PlaceholderFix Fix = new PlaceholderFix();

        [Test]
        public static void M()
        {
            var c1 = ↓""class C1 { }"";
            var c2 = ↓""class C2 { }"";
            var after = ""class C { }"";
            RoslynAssert.CodeFix(Analyzer, Fix, new [] { c1, c2 }, after);
        }
    }
}";

            var after = @"
namespace N
{
    using Gu.Roslyn.Asserts;
    using NUnit.Framework;

    public static class C
    {
        private static readonly PlaceholderAnalyzer Analyzer = new PlaceholderAnalyzer();
        private static readonly PlaceholderFix Fix = new PlaceholderFix();

        [Test]
        public static void M()
        {
            var c1 = ""↓class C1 { }"";
            var c2 = ""class C2 { }"";
            var after = ""class C { }"";
            RoslynAssert.CodeFix(Analyzer, Fix, new [] { c1, c2 }, after);
        }
    }
}";
            RoslynAssert.FixAll(Analyzer, Fix, ExpectedDiagnostic, new[] { Code.PlaceholderAnalyzer, Code.PlaceholderFix, before }, after);
        }
    }
}