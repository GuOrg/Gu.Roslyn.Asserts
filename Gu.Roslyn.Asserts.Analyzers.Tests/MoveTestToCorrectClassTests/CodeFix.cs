namespace Gu.Roslyn.Asserts.Analyzers.Tests.MoveTestToCorrectClassTests
{
    using Microsoft.CodeAnalysis.CodeFixes;
    using Microsoft.CodeAnalysis.Diagnostics;
    using NUnit.Framework;

    public static class CodeFix
    {
        private static readonly DiagnosticAnalyzer Analyzer = new MethodDeclarationAnalyzer();
        private static readonly CodeFixProvider Fix = new MoveFix();
        private static readonly ExpectedDiagnostic ExpectedDiagnostic = ExpectedDiagnostic.Create(GURA06TestShouldBeInCorrectClass.Descriptor);

        [Test]
        public static void WhenMixCreatingClass()
        {
            var before = @"
namespace RoslynSandbox
{
    using Gu.Roslyn.Asserts;
    using NUnit.Framework;

    public static class Valid
    {
        private static readonly PlaceholderAnalyzer Analyzer = new PlaceholderAnalyzer();

        [Test]
        public static void M1()
        {
            var c = ""class C { }"";
            RoslynAssert.Valid(Analyzer, c);
        }

        [Test]
        public static void ↓M2()
        {
            var c = ""class C { }"";
            RoslynAssert.Diagnostics(Analyzer, c);
        }
    }
}";

            var valid = @"
namespace RoslynSandbox
{
    using Gu.Roslyn.Asserts;
    using NUnit.Framework;

    public static class Valid
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

            var diagnostics = @"
namespace RoslynSandbox
{
    using Gu.Roslyn.Asserts;
    using NUnit.Framework;

    public static class Diagnostics
    {
        private static readonly PlaceholderAnalyzer Analyzer = new PlaceholderAnalyzer();

        [Test]
        public static void M2()
        {
            var c = ""class C { }"";
            RoslynAssert.Diagnostics(Analyzer, c);
        }
    }
}";
            RoslynAssert.CodeFix(Analyzer, Fix, ExpectedDiagnostic, new[] { Code.PlaceholderAnalyzer, before }, new[] { Code.PlaceholderAnalyzer, valid, diagnostics }, suppressedDiagnostics: new[] { "CS1701" });
        }

        [Test]
        public static void WhenMixAndClassExists()
        {
            var before = @"
namespace RoslynSandbox
{
    using Gu.Roslyn.Asserts;
    using NUnit.Framework;

    public static class Valid
    {
        private static readonly PlaceholderAnalyzer Analyzer = new PlaceholderAnalyzer();

        [Test]
        public static void M1()
        {
            var c = ""class C { }"";
            RoslynAssert.Valid(Analyzer, c);
        }

        [Test]
        public static void ↓M2()
        {
            var c = ""class C { }"";
            RoslynAssert.Diagnostics(Analyzer, c);
        }
    }
}";

            var diagnosticsBefore = @"
namespace RoslynSandbox
{
    using Gu.Roslyn.Asserts;
    using NUnit.Framework;

    public static class Diagnostics
    {
        private static readonly PlaceholderAnalyzer Analyzer = new PlaceholderAnalyzer();

        [Test]
        public static void M1()
        {
            var c = ""class C { }"";
            RoslynAssert.Diagnostics(Analyzer, c);
        }
    }
}";

            var valid = @"
namespace RoslynSandbox
{
    using Gu.Roslyn.Asserts;
    using NUnit.Framework;

    public static class Valid
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

            var diagnosticsAfter = @"
namespace RoslynSandbox
{
    using Gu.Roslyn.Asserts;
    using NUnit.Framework;

    public static class Diagnostics
    {
        private static readonly PlaceholderAnalyzer Analyzer = new PlaceholderAnalyzer();

        [Test]
        public static void M1()
        {
            var c = ""class C { }"";
            RoslynAssert.Diagnostics(Analyzer, c);
        }

        [Test]
        public static void M2()
        {
            var c = ""class C { }"";
            RoslynAssert.Diagnostics(Analyzer, c);
        }
    }
}";
            RoslynAssert.CodeFix(Analyzer, Fix, new[] { Code.PlaceholderAnalyzer, before, diagnosticsBefore }, new[] { Code.PlaceholderAnalyzer, valid, diagnosticsAfter }, suppressedDiagnostics: new[] { "CS1701" });
        }
    }
}
