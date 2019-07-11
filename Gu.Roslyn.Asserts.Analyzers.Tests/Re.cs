namespace Gu.Roslyn.Asserts.Analyzers.Tests
{
    using Microsoft.CodeAnalysis.CodeFixes;
    using Microsoft.CodeAnalysis.Diagnostics;
    using NUnit.Framework;

    public static class LocalNameShouldMatchParameterTests
    {
        private static readonly DiagnosticAnalyzer Analyzer = new LocalNameShouldMatchParameter();
        private static readonly CodeFixProvider Fix = new RenameLocalFix();
        private static readonly ExpectedDiagnostic ExpectedDiagnostic = ExpectedDiagnostic.Create(LocalNameShouldMatchParameter.Descriptor);

        [Test]
        public static void LocalNameShouldMatchParameterFix()
        {
            var before = @"
namespace RoslynSandbox
{
    using Gu.Roslyn.Asserts;
    using NUnit.Framework;

    public static class C
    {
        [Test]
        public static void M()
        {
            var foo = ""class Foo { }"";
            RoslynAssert.Valid(null, ↓foo);
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
        [Test]
        public static void M()
        {
            var code = ""class Foo { }"";
            RoslynAssert.Valid(null, code);
        }
    }
}";
            var expectedDiagnostic = ExpectedDiagnostic.WithMessage("Name of 'foo' should be 'code'.");
            RoslynAssert.CodeFix(Analyzer, Fix, expectedDiagnostic, before, after);
        }
    }
}
