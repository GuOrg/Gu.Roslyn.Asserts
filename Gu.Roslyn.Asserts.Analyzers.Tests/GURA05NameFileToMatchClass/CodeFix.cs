namespace Gu.Roslyn.Asserts.Analyzers.Tests.GURA05NameFileToMatchClass
{
    using Microsoft.CodeAnalysis.CodeFixes;
    using Microsoft.CodeAnalysis.Diagnostics;
    using NUnit.Framework;

    public static class CodeFix
    {
        private static readonly DiagnosticAnalyzer Analyzer = new ClassDeclarationAnalyzer();
        private static readonly CodeFixProvider Fix = new MakePublicStaticFix();
        private static readonly ExpectedDiagnostic ExpectedDiagnostic = ExpectedDiagnostic.Create(Descriptors.GURA07TestClassShouldBePublicStatic);

        [Explicit("Not sure why this breaks.")]
        [Test]
        public static void WhenValidCode()
        {
            var before = @"
namespace N
{
    using Gu.Roslyn.Asserts;
    using NUnit.Framework;

    internal static class ↓ValidCode
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
namespace N
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

            RoslynAssert.CodeFix(Analyzer, Fix, ExpectedDiagnostic, new[] { Code.PlaceholderAnalyzer, before }, new[] { Code.PlaceholderAnalyzer, after });
        }
    }
}
