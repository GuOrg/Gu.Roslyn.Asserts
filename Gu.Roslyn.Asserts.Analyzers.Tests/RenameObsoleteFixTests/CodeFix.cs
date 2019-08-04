namespace Gu.Roslyn.Asserts.Analyzers.Tests.RenameFixTests
{
    using Microsoft.CodeAnalysis.CodeFixes;
    using NUnit.Framework;

    public static partial class CodeFix
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
        private static readonly PlaceholderAnalyzer Analyzer = new PlaceholderAnalyzer();

        [Test]
        public static void M()
        {
            ↓AnalyzerAssert.Valid(Analyzer, string.Empty);
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
            RoslynAssert.Valid(Analyzer, string.Empty);
        }
    }
}";
            var expectedDiagnostic = ExpectedDiagnostic.Create("CS0103");
            RoslynAssert.CodeFix(Fix, expectedDiagnostic, new[] { Code.PlaceholderAnalyzer, before }, after, suppressWarnings: new[] { "CS8019" });
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
        private static readonly PlaceholderAnalyzer Analyzer = new PlaceholderAnalyzer();

        [Test]
        public static void M()
        {
            ↓Gu.Roslyn.Asserts.AnalyzerAssert.Valid(Analyzer, string.Empty);
        }
    }
}";

            var after = @"
namespace N
{
    using NUnit.Framework;

    public static class C
    {
        private static readonly PlaceholderAnalyzer Analyzer = new PlaceholderAnalyzer();

        [Test]
        public static void M()
        {
            Gu.Roslyn.Asserts.RoslynAssert.Valid(Analyzer, string.Empty);
        }
    }
}";
            var expectedDiagnostic = ExpectedDiagnostic.Create("CS0234");
            RoslynAssert.CodeFix(Fix, expectedDiagnostic, new[] { Code.PlaceholderAnalyzer, before }, after);
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
        public static void ChangeParameterNameToSuppressWarnings()
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
            var code = ""class C { }"";
            RoslynAssert.Valid(Analyzer, code, ↓suppressedDiagnostics: new[] { ""CS1701"" });
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
            var code = ""class C { }"";
            RoslynAssert.Valid(Analyzer, code, suppressWarnings: new[] { ""CS1701"" });
        }
    }
}";
            var expectedDiagnostic = ExpectedDiagnostic.Create("CS1739");
            RoslynAssert.CodeFix(Fix, expectedDiagnostic, new[] { Code.PlaceholderAnalyzer, before }, after);
        }
    }
}
