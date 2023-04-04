namespace Gu.Roslyn.Asserts.Analyzers.Tests.GURA01NameShouldMatchParameter;

using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Diagnostics;
using NUnit.Framework;

public static class NoFix
{
    private static readonly DiagnosticAnalyzer Analyzer = new InvocationAnalyzer();
    private static readonly CodeFixProvider Fix = new RenameFix();
    private static readonly ExpectedDiagnostic ExpectedDiagnostic = ExpectedDiagnostic.Create(Descriptors.GURA01NameShouldMatchParameter);

    [Test]
    public static void WhenNameCollision()
    {
        var code = @"
namespace N
{
    using Gu.Roslyn.Asserts;
    using NUnit.Framework;

    public static class C
    {
        private static readonly PlaceholderAnalyzer Analyzer = new PlaceholderAnalyzer();

        [TestCase(""C { }"")]
        public static void M(string code)
        {
            var wrong = ""class C { }"";
            RoslynAssert.Valid(Analyzer, ↓wrong);
        }
    }
}";

        var expectedDiagnostic = ExpectedDiagnostic.WithMessage("Name of 'wrong' should be 'code'");
        RoslynAssert.NoFix(Analyzer, Fix, expectedDiagnostic, Code.PlaceholderAnalyzer, Code.PlaceholderFix, code);
    }
}
