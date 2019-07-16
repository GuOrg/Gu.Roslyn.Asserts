namespace Gu.Roslyn.Asserts.Analyzers.Tests.TestClassShouldBePublicStaticTests
{
    using Microsoft.CodeAnalysis.CodeFixes;
    using Microsoft.CodeAnalysis.Diagnostics;
    using NUnit.Framework;

    public static class CodeFix
    {
        private static readonly DiagnosticAnalyzer Analyzer = new ClassDeclarationAnalyzer();
        private static readonly CodeFixProvider Fix = new PublicStaticFix();
        private static readonly ExpectedDiagnostic ExpectedDiagnostic = ExpectedDiagnostic.Create(GURA07TestClassShouldBePublicStatic.Descriptor);

        [Test]
        public static void WhenInternalStatic()
        {
            var before = @"
namespace RoslynSandbox
{
    using Gu.Roslyn.Asserts;
    using NUnit.Framework;

    internal static class ↓Valid
    {
        private static readonly PlaceholderAnalyzer Analyzer = new PlaceholderAnalyzer();

        [Test]
        internal static void M1()
        {
            var c = ""class C { }"";
            RoslynAssert.Valid(Analyzer, c);
        }
    }
}";

            var after = @"
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

            RoslynAssert.CodeFix(Analyzer, Fix, ExpectedDiagnostic, new[] { Code.PlaceholderAnalyzer, before }, new[] { Code.PlaceholderAnalyzer, after }, suppressedDiagnostics: new[] { "CS1701" });
        }

        [Test]
        public static void WhenExplicitInternal()
        {
            var before = @"
namespace RoslynSandbox
{
    using Gu.Roslyn.Asserts;
    using NUnit.Framework;

    internal class ↓Valid
    {
        private readonly PlaceholderAnalyzer Analyzer = new PlaceholderAnalyzer();

        [Test]
        internal void M1()
        {
            var c = ""class C { }"";
            RoslynAssert.Valid(Analyzer, c);
        }
    }
}";

            var after = @"
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

            RoslynAssert.CodeFix(Analyzer, Fix, ExpectedDiagnostic, new[] { Code.PlaceholderAnalyzer, before }, new[] { Code.PlaceholderAnalyzer, after }, suppressedDiagnostics: new[] { "CS1701" });
        }

        [Test]
        public static void WhenImplicitnternalPrivate()
        {
            var before = @"
namespace RoslynSandbox
{
    using Gu.Roslyn.Asserts;
    using NUnit.Framework;

    class ↓Valid
    {
        private readonly PlaceholderAnalyzer Analyzer = new PlaceholderAnalyzer();

        [Test]
        void M1()
        {
            var c = ""class C { }"";
            RoslynAssert.Valid(Analyzer, c);
        }
    }
}";

            var after = @"
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

            RoslynAssert.CodeFix(Analyzer, Fix, ExpectedDiagnostic, new[] { Code.PlaceholderAnalyzer, before }, new[] { Code.PlaceholderAnalyzer, after }, suppressedDiagnostics: new[] { "CS1701" });
        }
    }
}
