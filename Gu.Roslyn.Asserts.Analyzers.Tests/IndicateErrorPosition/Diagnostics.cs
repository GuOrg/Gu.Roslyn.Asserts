namespace Gu.Roslyn.Asserts.Analyzers.Tests.IndicateErrorPosition
{
    using Microsoft.CodeAnalysis.Diagnostics;
    using NUnit.Framework;

    [Explicit("Temp suppress.")]
    public static class Diagnostics
    {
        private static readonly DiagnosticAnalyzer Analyzer = new ArgumentAnalyzer();
        private static readonly ExpectedDiagnostic ExpectedDiagnostic = ExpectedDiagnostic.Create(Descriptors.IndicateErrorPosition);
        private static readonly string[] suppressWarnings = { "CS1701" };

        [Test]
        public static void DiagnosticsOneArgument()
        {
            var code = @"
namespace N
{
    using Gu.Roslyn.Asserts;
    using NUnit.Framework;

    public static class C
    {
        private static readonly PlaceholderAnalyzer Analyzer = new PlaceholderAnalyzer();

        [Test]
        public static void M()
        {
            var c1 = ↓""class C1 { }"";
            RoslynAssert.Diagnostics(Analyzer, ↓c1);
        }
    }
}";

            RoslynAssert.Diagnostics(Analyzer, ExpectedDiagnostic, new[] { Code.PlaceholderAnalyzer, code }, suppressWarnings: suppressWarnings);
        }

        [Test]
        public static void DiagnosticsTwoParams()
        {
            var code = @"
namespace N
{
    using Gu.Roslyn.Asserts;
    using NUnit.Framework;

    public static class C
    {
        private static readonly PlaceholderAnalyzer Analyzer = new PlaceholderAnalyzer();

        [Test]
        public static void M()
        {
            var c1 = ↓""class C1 { }"";
            var c2 = ↓""class C2 { }"";
            RoslynAssert.Diagnostics(Analyzer, ↓c1, ↓c2);
        }
    }
}";

            RoslynAssert.Diagnostics(Analyzer, ExpectedDiagnostic, new[] { Code.PlaceholderAnalyzer, code }, suppressWarnings: suppressWarnings);
        }

        [Test]
        public static void DiagnosticsArray()
        {
            var code = @"
namespace N
{
    using Gu.Roslyn.Asserts;
    using NUnit.Framework;

    public static class C
    {
        private static readonly PlaceholderAnalyzer Analyzer = new PlaceholderAnalyzer();

        [Test]
        public static void M()
        {
            var c1 = ↓""class C1 { }"";
            var c2 = ↓""class C2 { }"";
            RoslynAssert.Diagnostics(Analyzer, new [] { ↓c1, ↓c2 });
        }
    }
}";

            RoslynAssert.Diagnostics(Analyzer, ExpectedDiagnostic, new[] { Code.PlaceholderAnalyzer, code }, suppressWarnings: suppressWarnings);
        }

        [Test]
        public static void CodeFixArray()
        {
            var code = @"
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
            RoslynAssert.CodeFix(Analyzer, Fix, new [] { ↓c1, ↓c2 }, string.Empty);
        }
    }
}";

            RoslynAssert.Diagnostics(Analyzer, ExpectedDiagnostic, new[] { Code.PlaceholderAnalyzer, Code.PlaceholderFix, code }, suppressWarnings: suppressWarnings);
        }
    }
}
