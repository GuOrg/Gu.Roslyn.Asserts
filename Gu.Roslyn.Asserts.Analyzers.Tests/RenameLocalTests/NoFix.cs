namespace Gu.Roslyn.Asserts.Analyzers.Tests.RenameLocalTests
{
    using Microsoft.CodeAnalysis.CodeFixes;
    using Microsoft.CodeAnalysis.Diagnostics;
    using NUnit.Framework;

    public static class NoFix
    {
        private static readonly DiagnosticAnalyzer Analyzer = new ArgumentAnalyzer();
        private static readonly CodeFixProvider Fix = new RenameFix();

        private static readonly ExpectedDiagnostic ExpectedDiagnostic = ExpectedDiagnostic.Create(GURA01NameOfLocalShouldMatchParameter.Descriptor);

        [Test]
        public static void WhenNameCollision()
        {
            var code = @"
namespace RoslynSandbox
{
    using Microsoft.CodeAnalysis.Diagnostics;
    using Gu.Roslyn.Asserts;
    using NUnit.Framework;

    public static class C
    {
        private static readonly PlaceholderAnalyzer Analyzer = new PlaceholderAnalyzer();

        [TestCase(""C { }"")]
        public static void M(string code)
        {
            var wrong = ""class C { }"".AssertReplace(string.Empty, code);
            RoslynAssert.Valid(Analyzer, ↓wrong);
        }
    }
}";

            var expectedDiagnostic = ExpectedDiagnostic.WithMessage("Name of 'wrong' should be 'code'.");
            RoslynAssert.NoFix(Analyzer, Fix, expectedDiagnostic, Code.PlaceholderAnalyzer, Code.PlaceholderFix, code);
        }
    }
}
