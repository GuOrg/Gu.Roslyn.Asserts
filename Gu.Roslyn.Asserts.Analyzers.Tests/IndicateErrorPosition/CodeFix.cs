namespace Gu.Roslyn.Asserts.Analyzers.Tests.IndicateErrorPosition
{
    using Microsoft.CodeAnalysis.CodeFixes;
    using Microsoft.CodeAnalysis.Diagnostics;
    using NUnit.Framework;

    public static class CodeFix
    {
        private static readonly DiagnosticAnalyzer Analyzer = new ArgumentAnalyzer();
        private static readonly CodeFixProvider Fix = new IndicateErrorPositionFix();
        private static readonly ExpectedDiagnostic ExpectedDiagnostic = ExpectedDiagnostic.Create(Descriptors.IndicateErrorPosition);

        [Test]
        public static void DiagnosticsOneParamWithoutPosition()
        {
            var before = @"
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
            var code = ↓""class C { }"";
            RoslynAssert.Diagnostics(Analyzer, code);
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

        [Test]
        public static void M()
        {
            var code = ""↓class C { }"";
            RoslynAssert.Diagnostics(Analyzer, code);
        }
    }
}";
            var expectedDiagnostic = ExpectedDiagnostic.WithMessage("Indicate expected position with ↓ (alt + 25).");
            RoslynAssert.CodeFix(Analyzer, Fix, expectedDiagnostic, new[] { Code.PlaceholderAnalyzer, before }, after);
        }

        [Test]
        public static void DiagnosticsOneParamWithoutPositionVerbatimString()
        {
            var before = @"
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
            var code = ↓@""class C
{
}"";
            RoslynAssert.Diagnostics(Analyzer, code);
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

        [Test]
        public static void M()
        {
            var code = @""↓class C
{
}"";
            RoslynAssert.Diagnostics(Analyzer, code);
        }
    }
}";
            var expectedDiagnostic = ExpectedDiagnostic.WithMessage("Indicate expected position with ↓ (alt + 25).");
            RoslynAssert.CodeFix(Analyzer, Fix, expectedDiagnostic, new[] { Code.PlaceholderAnalyzer, before }, after);
        }

        [Test]
        public static void DiagnosticsTowParamWithoutPositionOneWithMatchingName()
        {
            var before = @"
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
            var c1 = ""class C1 { }"";
            var code = ↓""class C2 { }"";
            RoslynAssert.Diagnostics(Analyzer, c1, code);
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

        [Test]
        public static void M()
        {
            var c1 = ""class C1 { }"";
            var code = ""↓class C2 { }"";
            RoslynAssert.Diagnostics(Analyzer, c1, code);
        }
    }
}";
            var expectedDiagnostic = ExpectedDiagnostic.WithMessage("Indicate expected position with ↓ (alt + 25).");
            RoslynAssert.CodeFix(Analyzer, Fix, expectedDiagnostic, new[] { Code.PlaceholderAnalyzer, before }, after);
        }

        [Test]
        public static void DiagnosticsTowParamWithoutPositionOneLocalOneConst()
        {
            var before = @"
namespace N
{
    using Gu.Roslyn.Asserts;
    using NUnit.Framework;

    public static class C
    {
        private static readonly PlaceholderAnalyzer Analyzer = new PlaceholderAnalyzer();
        const string C1 = ""class C1 { }"";

        [Test]
        public static void M()
        {
            var code = ↓""class C2 { }"";
            RoslynAssert.Diagnostics(Analyzer, C1, code);
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
        const string C1 = ""class C1 { }"";

        [Test]
        public static void M()
        {
            var code = ""↓class C2 { }"";
            RoslynAssert.Diagnostics(Analyzer, C1, code);
        }
    }
}";
            var expectedDiagnostic = ExpectedDiagnostic.WithMessage("Indicate expected position with ↓ (alt + 25).");
            RoslynAssert.CodeFix(Analyzer, Fix, expectedDiagnostic, new[] { Code.PlaceholderAnalyzer, before }, after);
        }

        [Test]
        public static void DiagnosticsTowParamWithoutPositionOneLocalOneInstance()
        {
            var before = @"
namespace N
{
    using Gu.Roslyn.Asserts;
    using NUnit.Framework;

    public class C
    {
        private static readonly PlaceholderAnalyzer Analyzer = new PlaceholderAnalyzer();
        private readonly string code1 = ""class C { }"";

        [Test]
        public void M()
        {
            var code2 = ↓""class C { }"";
            RoslynAssert.Diagnostics(Analyzer, this.code1, code2);
        }
    }
}";

            var after = @"
namespace N
{
    using Gu.Roslyn.Asserts;
    using NUnit.Framework;

    public class C
    {
        private static readonly PlaceholderAnalyzer Analyzer = new PlaceholderAnalyzer();
        private readonly string code1 = ""class C { }"";

        [Test]
        public void M()
        {
            var code2 = ""↓class C { }"";
            RoslynAssert.Diagnostics(Analyzer, this.code1, code2);
        }
    }
}";
            var expectedDiagnostic = ExpectedDiagnostic.WithMessage("Indicate expected position with ↓ (alt + 25).");
            RoslynAssert.CodeFix(Analyzer, Fix, expectedDiagnostic, new[] { Code.PlaceholderAnalyzer, before }, after);
        }

        [Test]
        public static void CodeFixLocal()
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
            var before = ↓""class C { }"";
            var after = ""class C { }"";
            RoslynAssert.CodeFix(Analyzer, Fix, before, after);
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
            var before = ""↓class C { }"";
            var after = ""class C { }"";
            RoslynAssert.CodeFix(Analyzer, Fix, before, after);
        }
    }
}";
            var expectedDiagnostic = ExpectedDiagnostic.WithMessage("Indicate expected position with ↓ (alt + 25).");
            RoslynAssert.CodeFix(Analyzer, Fix, expectedDiagnostic, new[] { Code.PlaceholderAnalyzer, Code.PlaceholderFix, before }, after);
        }

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
            var expectedDiagnostic = ExpectedDiagnostic.WithMessage("Indicate expected position with ↓ (alt + 25).");
            RoslynAssert.FixAll(Analyzer, Fix, expectedDiagnostic, new[] { Code.PlaceholderAnalyzer, Code.PlaceholderFix, before }, after);
        }

        [Test]
        public static void CodeFixLocalsWithBefore()
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
            var c1 = ""class C1 { }"";
            var before = ↓""class C2 { }"";
            var after = ""class C { }"";
            RoslynAssert.CodeFix(Analyzer, Fix, new [] { c1, before }, after);
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
            var c1 = ""class C1 { }"";
            var before = ""↓class C2 { }"";
            var after = ""class C { }"";
            RoslynAssert.CodeFix(Analyzer, Fix, new [] { c1, before }, after);
        }
    }
}";
            var expectedDiagnostic = ExpectedDiagnostic.WithMessage("Indicate expected position with ↓ (alt + 25).");
            RoslynAssert.FixAll(Analyzer, Fix, expectedDiagnostic, new[] { Code.PlaceholderAnalyzer, Code.PlaceholderFix, before }, after);
        }

        [Test]
        public static void CodeFixLiterals()
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
            RoslynAssert.CodeFix(Analyzer, Fix, ↓""class C { }"", ""class C { }"");
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
            RoslynAssert.CodeFix(Analyzer, Fix, ""↓class C { }"", ""class C { }"");
        }
    }
}";
            var expectedDiagnostic = ExpectedDiagnostic.WithMessage("Indicate expected position with ↓ (alt + 25).");
            RoslynAssert.FixAll(Analyzer, Fix, expectedDiagnostic, new[] { Code.PlaceholderAnalyzer, Code.PlaceholderFix, before }, after);
        }
    }
}
