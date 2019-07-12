namespace Gu.Roslyn.Asserts.Analyzers.Tests.IndicateErrorPositionTests
{
    using Microsoft.CodeAnalysis.CodeFixes;
    using Microsoft.CodeAnalysis.Diagnostics;
    using NUnit.Framework;

    public static class Diagnostics
    {
        private static readonly DiagnosticAnalyzer Analyzer = new ArgumentAnalyzer();
        private static readonly CodeFixProvider Fix = new IndicateErrorPositionFix();
        private static readonly ExpectedDiagnostic ExpectedDiagnostic = ExpectedDiagnostic.Create(GURA02IndicateErrorPosition.Descriptor);

        [Test]
        public static void RoslynAssertDiagnosticsTwoParams()
        {
            var code = @"
namespace RoslynSandbox
{
    using Gu.Roslyn.Asserts;
    using NUnit.Framework;

    public static class C
    {
        private static readonly PlaceholderAnalyzer Analyzer = new PlaceholderAnalyzer();

        [Test]
        public static void M()
        {
            var code1 = ""class C { }"";
            var code2 = ""class C { }"";
            RoslynAssert.Diagnostics(Analyzer, ↓code1, ↓code2);
        }
    }
}";

            RoslynAssert.NoFix(Analyzer, Fix, ExpectedDiagnostic, Code.PlaceholderAnalyzer, code);
        }

        [Test]
        public static void RoslynAssertDiagnosticsArray()
        {
            var code = @"
namespace RoslynSandbox
{
    using Gu.Roslyn.Asserts;
    using NUnit.Framework;

    public static class C
    {
        private static readonly PlaceholderAnalyzer Analyzer = new PlaceholderAnalyzer();

        [Test]
        public static void M()
        {
            var code1 = ""class C { }"";
            var code2 = ""class C { }"";
            RoslynAssert.Diagnostics(Analyzer, ↓new [] { code1, code2 });
        }
    }
}";

            RoslynAssert.NoFix(Analyzer, Fix, ExpectedDiagnostic, Code.PlaceholderAnalyzer, code);
        }

        [Test]
        public static void RoslynAssertCodeFixOneBefore()
        {
            var code = @"
namespace RoslynSandbox
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
            var code1 = ""class C { }"";
            var code2 = ""class C { }"";
            RoslynAssert.CodeFix(Analyzer, Fix, ↓new [] { code1, code2 }, string.Empty);
        }
    }
}";

            RoslynAssert.NoFix(Analyzer, Fix, ExpectedDiagnostic, Code.PlaceholderAnalyzer, Code.PlaceholderFix, code);
        }
    }
}
