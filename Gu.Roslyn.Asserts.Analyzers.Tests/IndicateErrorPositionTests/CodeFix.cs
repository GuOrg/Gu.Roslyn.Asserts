namespace Gu.Roslyn.Asserts.Analyzers.Tests.IndicateErrorPositionTests
{
    using Microsoft.CodeAnalysis.CodeFixes;
    using Microsoft.CodeAnalysis.Diagnostics;
    using NUnit.Framework;

    public static class CodeFix
    {
        private static readonly DiagnosticAnalyzer Analyzer = new ArgumentAnalyzer();
        private static readonly CodeFixProvider Fix = new IndicateErrorPositionFix();
        private static readonly ExpectedDiagnostic ExpectedDiagnostic = ExpectedDiagnostic.Create(GURA02IndicateErrorPosition.Descriptor);

        [Test]
        public static void DiagnosticsOneParamWithoutPosition()
        {
            var before = @"
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
            var code = ""class C { }"";
            RoslynAssert.Diagnostics(Analyzer, ↓code);
        }
    }
}";

            var after = @"
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
            var code = ""↓class C { }"";
            RoslynAssert.Diagnostics(Analyzer, code);
        }
    }
}";
            var expectedDiagnostic = ExpectedDiagnostic.WithMessage("Indicate expected error position with ↓ (alt + 25).");
            RoslynAssert.CodeFix(Analyzer, Fix, expectedDiagnostic, new[] { Code.PlaceholderAnalyzer, before }, after);
        }

        [Test]
        public static void DiagnosticsOneParamWithoutPositionVerbatimString()
        {
            var before = @"
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
            var code = @""class C
{
}"";
            RoslynAssert.Diagnostics(Analyzer, ↓code);
        }
    }
}";

            var after = @"
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
            var code = @""↓class C
{
}"";
            RoslynAssert.Diagnostics(Analyzer, code);
        }
    }
}";
            var expectedDiagnostic = ExpectedDiagnostic.WithMessage("Indicate expected error position with ↓ (alt + 25).");
            RoslynAssert.CodeFix(Analyzer, Fix, expectedDiagnostic, new[] { Code.PlaceholderAnalyzer, before }, after);
        }

        [Test]
        public static void DiagnosticsTowParamWithoutPositionOneWithMatchingName()
        {
            var before = @"
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
            var code = ""class C { }"";
            RoslynAssert.Diagnostics(Analyzer, code1, ↓code);
        }
    }
}";

            var after = @"
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
            var code = ""↓class C { }"";
            RoslynAssert.Diagnostics(Analyzer, code1, code);
        }
    }
}";
            var expectedDiagnostic = ExpectedDiagnostic.WithMessage("Indicate expected error position with ↓ (alt + 25).");
            RoslynAssert.CodeFix(Analyzer, Fix, expectedDiagnostic, new[] { Code.PlaceholderAnalyzer, before }, after);
        }

        [Test]
        public static void DiagnosticsTowParamWithoutPositionOneLocalOneConst()
        {
            var before = @"
namespace RoslynSandbox
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
            var code = ""class C2 { }"";
            RoslynAssert.Diagnostics(Analyzer, C1, ↓code);
        }
    }
}";

            var after = @"
namespace RoslynSandbox
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
            var expectedDiagnostic = ExpectedDiagnostic.WithMessage("Indicate expected error position with ↓ (alt + 25).");
            RoslynAssert.CodeFix(Analyzer, Fix, expectedDiagnostic, new[] { Code.PlaceholderAnalyzer, before }, after);
        }

        [Test]
        public static void DiagnosticsTowParamWithoutPositionOneLocalOneInstance()
        {
            var before = @"
namespace RoslynSandbox
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
            var code2 = ""class C { }"";
            RoslynAssert.Diagnostics(Analyzer, this.code1, ↓code2);
        }
    }
}";

            var after = @"
namespace RoslynSandbox
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
            var expectedDiagnostic = ExpectedDiagnostic.WithMessage("Indicate expected error position with ↓ (alt + 25).");
            RoslynAssert.CodeFix(Analyzer, Fix, expectedDiagnostic, new[] { Code.PlaceholderAnalyzer, before }, after);
        }

        [Test]
        public static void CodeFixOneBefore()
        {
            var before = @"
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
            var before = ""class C { }"";
            RoslynAssert.CodeFix(Analyzer, Fix, ↓before, string.Empty);
        }
    }
}";

            var after = @"
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
            var before = ""↓class C { }"";
            RoslynAssert.CodeFix(Analyzer, Fix, before, string.Empty);
        }
    }
}";
            var expectedDiagnostic = ExpectedDiagnostic.WithMessage("Indicate expected error position with ↓ (alt + 25).");
            RoslynAssert.CodeFix(Analyzer, Fix, expectedDiagnostic, new[] { Code.PlaceholderAnalyzer, Code.PlaceholderFix, before }, after);
        }
    }
}
