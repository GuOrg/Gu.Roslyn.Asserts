namespace Gu.Roslyn.Asserts.Analyzers.Tests
{
    using Microsoft.CodeAnalysis.CodeFixes;
    using NUnit.Framework;

    public static class RenameFixTests
    {
        private static readonly CodeFixProvider Fix = new RenameObsoleteFix();

        [Test]
        public static void ChangeToRoslynAssert()
        {
            var before = @"
namespace N
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
namespace N
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
namespace N
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
namespace N
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

        [TestCase("code")]
        [TestCase("codeWithErrorsIndicated")]
        public static void ChangeParameterNameToBefore(string name)
        {
            var before = @"
namespace N
{
    using Gu.Roslyn.Asserts;
    using NUnit.Framework;

    public static class C
    {
        [Test]
        public static void M()
        {
            RoslynAssert.CodeFix(analyzer: null, fix: null, code: string.Empty, after: string.Empty);
        }
    }
}".AssertReplace("code", name);

            var after = @"
namespace N
{
    using Gu.Roslyn.Asserts;
    using NUnit.Framework;

    public static class C
    {
        [Test]
        public static void M()
        {
            RoslynAssert.CodeFix(analyzer: null, fix: null, before: string.Empty, after: string.Empty);
        }
    }
}";
            var expectedDiagnostic = ExpectedDiagnostic.Create("CS1739");
            RoslynAssert.CodeFix(Fix, expectedDiagnostic, before, after);
        }

        [TestCase("fixedCode")]
        [TestCase("fixedcode")]
        public static void ChangeParameterNameToAfter(string name)
        {
            var before = @"
namespace N
{
    using Gu.Roslyn.Asserts;
    using NUnit.Framework;

    public static class C
    {
        [Test]
        public static void M()
        {
            RoslynAssert.CodeFix(analyzer: null, fix: null, before: string.Empty, fixedCode: string.Empty);
        }
    }
}".AssertReplace("fixedCode", name);

            var after = @"
namespace N
{
    using Gu.Roslyn.Asserts;
    using NUnit.Framework;

    public static class C
    {
        [Test]
        public static void M()
        {
            RoslynAssert.CodeFix(analyzer: null, fix: null, before: string.Empty, after: string.Empty);
        }
    }
}";
            var expectedDiagnostic = ExpectedDiagnostic.Create("CS1739");
            RoslynAssert.CodeFix(Fix, expectedDiagnostic, before, after);
        }

        [Test]
        public static void AssertTests()
        {
            var before = @"
namespace N
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
namespace N
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
