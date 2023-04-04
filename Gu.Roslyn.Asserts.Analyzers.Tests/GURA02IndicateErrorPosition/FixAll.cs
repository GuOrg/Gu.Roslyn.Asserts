namespace Gu.Roslyn.Asserts.Analyzers.Tests.GURA02IndicateErrorPosition;

using NUnit.Framework;

public static class FixAll
{
    private static readonly DiagnosticFixAssert Assert = RoslynAssert.Create<InvocationAnalyzer, IndicateErrorPositionFix>(
        ExpectedDiagnostic.Create(Descriptors.GURA02IndicateErrorPosition));

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
        Assert.FixAll(new[] { Code.PlaceholderAnalyzer, Code.PlaceholderFix, before }, after);
    }
}
