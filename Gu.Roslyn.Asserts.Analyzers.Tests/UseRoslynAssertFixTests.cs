namespace Gu.Roslyn.Asserts.Analyzers.Tests
{
    using Microsoft.CodeAnalysis.CodeFixes;
    using NUnit.Framework;

    public static class UseRoslynAssertFixTests
    {
        private static readonly CodeFixProvider Fix = new UseRoslynAssertFix();

        [Test]
        public static void ChangeToRoslynAssert()
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
            ↓AnalyzerAssert.Valid(null, string.Empty);
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
            RoslynAssert.Valid(null, string.Empty);
        }
    }
}";
            var expectedDiagnostic = ExpectedDiagnostic.Create("CS0103");
            RoslynAssert.CodeFix(Fix, expectedDiagnostic, before, after, suppressedDiagnostics: new[] { "CS8019" });
        }

        [Test]
        public static void ChangeToRoslynAssertFullyQualified()
        {
            var before = @"
namespace RoslynSandbox
{
    using NUnit.Framework;

    public static class C
    {
        [Test]
        public static void M()
        {
            ↓Gu.Roslyn.Asserts.AnalyzerAssert.Valid(null, string.Empty);
        }
    }
}";

            var after = @"
namespace RoslynSandbox
{
    using NUnit.Framework;

    public static class C
    {
        [Test]
        public static void M()
        {
            Gu.Roslyn.Asserts.RoslynAssert.Valid(null, string.Empty);
        }
    }
}";
            var expectedDiagnostic = ExpectedDiagnostic.Create("CS0234");
            RoslynAssert.CodeFix(Fix, expectedDiagnostic, before, after);
        }

        [Test]
        public static void AssertTests()
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
            ↓AnalyzerAssert.Valid(null, string.Empty);
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
            RoslynAssert.Valid(null, string.Empty);
        }
    }
}";
            var expectedDiagnostic = ExpectedDiagnostic.Create("CS0103");
            RoslynAssert.CodeFix(Fix, expectedDiagnostic, before, after, suppressedDiagnostics: new[] { "CS8019" });
            RoslynAssert.CodeFix(Fix, expectedDiagnostic, new[] { before }, after, suppressedDiagnostics: new[] { "CS8019" });
        }
    }
}
