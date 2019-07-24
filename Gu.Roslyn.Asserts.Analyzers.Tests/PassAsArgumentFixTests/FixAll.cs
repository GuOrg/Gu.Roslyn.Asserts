namespace Gu.Roslyn.Asserts.Analyzers.Tests.PassAsArgumentFixTests
{
    using Microsoft.CodeAnalysis.CodeFixes;
    using NUnit.Framework;

    public static class FixAll
    {
        private static readonly CodeFixProvider Fix = new PassAsArgumentFix();
        private static readonly ExpectedDiagnostic ExpectedDiagnostic = ExpectedDiagnostic.Create("CS0618");

        [Test]
        public static void ValidCreateField()
        {
            var before = @"
namespace N
{
    using Gu.Roslyn.Asserts;
    using NUnit.Framework;

    public static class Valid
    {
        [Test]
        public static void M1()
        {
            var code = ""class C { }"";
            ↓RoslynAssert.Valid<PlaceholderAnalyzer>(code);
        }

        [Test]
        public static void M2()
        {
            var code = ""class C { }"";
            ↓RoslynAssert.Valid<PlaceholderAnalyzer>(code);
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
            var code = ""class C { }"";
            RoslynAssert.Valid(Analyzer, code);
        }

        [Test]
        public static void M2()
        {
            var code = ""class C { }"";
            RoslynAssert.Valid(Analyzer, code);
        }
    }
}";
            RoslynAssert.FixAll(Fix, ExpectedDiagnostic, new[] { Code.PlaceholderAnalyzer, before }, after);
        }

        [Test]
        public static void CodeFixCreateAndUseFields()
        {
            var before = @"
namespace N
{
    using Gu.Roslyn.Asserts;
    using NUnit.Framework;

    public static class C
    {
        [Test]
        public static void M1()
        {
            var before = ""class C { }"";
            var after = ""class C { }"";
            ↓RoslynAssert.CodeFix<PlaceholderAnalyzer, PlaceholderFix>(ExpectedDiagnostic.Create(""123""), before, after);
        }

        [Test]
        public static void M2()
        {
            var before = ""class C { }"";
            var after = ""class C { }"";
            ↓RoslynAssert.CodeFix<PlaceholderAnalyzer, PlaceholderFix>(ExpectedDiagnostic.Create(""123""), before, after);
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
        private static readonly PlaceholderFix Fix = new PlaceholderFix();

        [Test]
        public static void M1()
        {
            var before = ""class C { }"";
            var after = ""class C { }"";
            RoslynAssert.CodeFix(Analyzer, Fix, ExpectedDiagnostic.Create(""123""), before, after);
        }

        [Test]
        public static void M2()
        {
            var before = ""class C { }"";
            var after = ""class C { }"";
            RoslynAssert.CodeFix(Analyzer, Fix, ExpectedDiagnostic.Create(""123""), before, after);
        }
    }
}";
            RoslynAssert.FixAll(Fix, ExpectedDiagnostic, new[] { Code.PlaceholderFix, Code.PlaceholderAnalyzer, before }, after);
        }
    }
}
