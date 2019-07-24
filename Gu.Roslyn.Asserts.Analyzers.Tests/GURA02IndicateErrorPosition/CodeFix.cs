namespace Gu.Roslyn.Asserts.Analyzers.Tests.GURA02IndicateErrorPosition
{
    using Microsoft.CodeAnalysis.CodeFixes;
    using Microsoft.CodeAnalysis.Diagnostics;
    using NUnit.Framework;

    public static class CodeFix
    {
        private static readonly DiagnosticAnalyzer Analyzer = new InvocationAnalyzer();
        private static readonly CodeFixProvider Fix = new IndicateErrorPositionFix();
        private static readonly ExpectedDiagnostic ExpectedDiagnostic = ExpectedDiagnostic.Create(Descriptors.GURA02IndicateErrorPosition);

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
            RoslynAssert.CodeFix(Analyzer, Fix, ExpectedDiagnostic, new[] { Code.PlaceholderAnalyzer, before }, after);
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
            RoslynAssert.CodeFix(Analyzer, Fix, ExpectedDiagnostic, new[] { Code.PlaceholderAnalyzer, before }, after);
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
            RoslynAssert.CodeFix(Analyzer, Fix, ExpectedDiagnostic, new[] { Code.PlaceholderAnalyzer, before }, after);
        }

        [Test]
        public static void DiagnosticsTowParamWithoutPositionOneLocalOneConstField()
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
            RoslynAssert.CodeFix(Analyzer, Fix, ExpectedDiagnostic, new[] { Code.PlaceholderAnalyzer, before }, after);
        }

        [Test]
        public static void DiagnosticsTowParamWithoutPositionOneLocalOneInstanceField()
        {
            var before = @"
namespace N
{
    using Gu.Roslyn.Asserts;
    using NUnit.Framework;

    public class C
    {
        private static readonly PlaceholderAnalyzer Analyzer = new PlaceholderAnalyzer();
        private readonly string c1 = ""class C1 { }"";

        [Test]
        public void M()
        {
            var c2 = ↓""class C2 { }"";
            RoslynAssert.Diagnostics(Analyzer, this.c1, c2);
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
        private readonly string c1 = ""class C1 { }"";

        [Test]
        public void M()
        {
            var c2 = ""↓class C2 { }"";
            RoslynAssert.Diagnostics(Analyzer, this.c1, c2);
        }
    }
}";
            RoslynAssert.CodeFix(Analyzer, Fix, ExpectedDiagnostic, new[] { Code.PlaceholderAnalyzer, before }, after);
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
            RoslynAssert.CodeFix(Analyzer, Fix, ExpectedDiagnostic, new[] { Code.PlaceholderAnalyzer, Code.PlaceholderFix, before }, after);
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
            RoslynAssert.CodeFix(Analyzer, Fix, ExpectedDiagnostic, new[] { Code.PlaceholderAnalyzer, Code.PlaceholderFix, before }, after);
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
            RoslynAssert.CodeFix(Analyzer, Fix, ↓""class C1 { }"", ""class C2 { }"");
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
            RoslynAssert.CodeFix(Analyzer, Fix, ""↓class C1 { }"", ""class C2 { }"");
        }
    }
}";
            RoslynAssert.CodeFix(Analyzer, Fix, ExpectedDiagnostic, new[] { Code.PlaceholderAnalyzer, Code.PlaceholderFix, before }, after);
        }
    }
}
