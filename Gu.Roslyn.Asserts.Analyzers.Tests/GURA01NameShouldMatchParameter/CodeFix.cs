namespace Gu.Roslyn.Asserts.Analyzers.Tests.GURA01NameShouldMatchParameter
{
    using Microsoft.CodeAnalysis.CodeFixes;
    using Microsoft.CodeAnalysis.Diagnostics;
    using NUnit.Framework;

    public static class CodeFix
    {
        private static readonly DiagnosticAnalyzer Analyzer = new InvocationAnalyzer();
        private static readonly CodeFixProvider Fix = new RenameFix();
        private static readonly ExpectedDiagnostic ExpectedDiagnostic = ExpectedDiagnostic.Create(Descriptors.GURA01NameShouldMatchParameter);

        [Test]
        public static void LocalAnalyzer()
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
            var code = ""class C { }"";
            var wrong = new PlaceholderAnalyzer();
            RoslynAssert.Valid(↓wrong, code);
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
            var code = ""class C { }"";
            var analyzer = new PlaceholderAnalyzer();
            RoslynAssert.Valid(analyzer, code);
        }
    }
}";
            var expectedDiagnostic = ExpectedDiagnostic.WithMessage("Name of 'wrong' should be 'analyzer'");
            RoslynAssert.CodeFix(Analyzer, Fix, expectedDiagnostic, new[] { Code.PlaceholderAnalyzer, before }, after, fixTitle: "Rename to 'analyzer'");
        }

        [Test]
        public static void StaticFieldAnalyzer()
        {
            var before = @"
namespace N
{
    using Gu.Roslyn.Asserts;
    using NUnit.Framework;

    public static class C
    {
        private static readonly PlaceholderAnalyzer Wrong = new PlaceholderAnalyzer();

        [Test]
        public static void M()
        {
            var code = ""class C { }"";
            RoslynAssert.Valid(↓Wrong, code);
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
            RoslynAssert.Valid(Analyzer, code);
        }
    }
}";
            var expectedDiagnostic = ExpectedDiagnostic.WithMessage("Name of 'Wrong' should be 'Analyzer'");
            RoslynAssert.CodeFix(Analyzer, Fix, expectedDiagnostic, new[] { Code.PlaceholderAnalyzer, before }, after, fixTitle: "Rename to 'Analyzer'");
        }

        [Test]
        public static void RoslynAssertValidOneParam()
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
            var wrong = ""class C { }"";
            RoslynAssert.Valid(Analyzer, ↓wrong);
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
            RoslynAssert.Valid(Analyzer, code);
        }
    }
}";
            var expectedDiagnostic = ExpectedDiagnostic.WithMessage("Name of 'wrong' should be 'code'");
            RoslynAssert.CodeFix(Analyzer, Fix, expectedDiagnostic, new[] { Code.PlaceholderAnalyzer, before }, after);
        }

        [Test]
        public static void RoslynAssertValidFieldAndLocal()
        {
            var before = @"
namespace N
{
    using Gu.Roslyn.Asserts;
    using NUnit.Framework;

    public static class C
    {
        private static readonly PlaceholderAnalyzer Analyzer = new PlaceholderAnalyzer();

        const string C1 = ""class C1 { }"";

        [Test]
        public static void M()
        {
            var wrong = ""class C { }"";
            RoslynAssert.Valid(Analyzer, C1, ↓wrong);
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

        const string C1 = ""class C1 { }"";

        [Test]
        public static void M()
        {
            var code = ""class C { }"";
            RoslynAssert.Valid(Analyzer, C1, code);
        }
    }
}";
            var expectedDiagnostic = ExpectedDiagnostic.WithMessage("Name of 'wrong' should be 'code'");
            RoslynAssert.CodeFix(Analyzer, Fix, expectedDiagnostic, new[] { Code.PlaceholderAnalyzer, before }, after);
        }

        [Test]
        public static void RoslynAssertValidLocalAndField()
        {
            var before = @"
namespace N
{
    using Gu.Roslyn.Asserts;
    using NUnit.Framework;

    public static class C
    {
        private static readonly PlaceholderAnalyzer Analyzer = new PlaceholderAnalyzer();

        const string C1 = ""class C1 { }"";

        [Test]
        public static void M()
        {
            var wrong = ""class C { }"";
            RoslynAssert.Valid(Analyzer, ↓wrong, C1);
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

        const string C1 = ""class C1 { }"";

        [Test]
        public static void M()
        {
            var code = ""class C { }"";
            RoslynAssert.Valid(Analyzer, code, C1);
        }
    }
}";
            var expectedDiagnostic = ExpectedDiagnostic.WithMessage("Name of 'wrong' should be 'code'");
            RoslynAssert.CodeFix(Analyzer, Fix, expectedDiagnostic, new[] { Code.PlaceholderAnalyzer, before }, after);
        }

        [Test]
        public static void WhenOneBeforeHasPosition()
        {
            var before = @"
namespace N
{
    using Microsoft.CodeAnalysis.CodeFixes;
    using Microsoft.CodeAnalysis.Diagnostics;
    using Gu.Roslyn.Asserts;
    using NUnit.Framework;

    public static class C
    {
        [Test]
        public static void M()
        {
            var c1 = ""class C1 { }"";
            var code2 = ""class ↓C2 { }"";
            var after = ""class C2 { }"";
            RoslynAssert.CodeFix((DiagnosticAnalyzer)null, (CodeFixProvider)null, new [] { c1, code2 }, after);
        }
    }
}";

            var after = @"
namespace N
{
    using Microsoft.CodeAnalysis.CodeFixes;
    using Microsoft.CodeAnalysis.Diagnostics;
    using Gu.Roslyn.Asserts;
    using NUnit.Framework;

    public static class C
    {
        [Test]
        public static void M()
        {
            var c1 = ""class C1 { }"";
            var before = ""class ↓C2 { }"";
            var after = ""class C2 { }"";
            RoslynAssert.CodeFix((DiagnosticAnalyzer)null, (CodeFixProvider)null, new [] { c1, before }, after);
        }
    }
}";
            var expectedDiagnostic = ExpectedDiagnostic.WithMessage("Name of 'code2' should be 'before'");
            var diagnosticsAndSources = new DiagnosticsAndSources(new[] { expectedDiagnostic }, new[] { before });
            RoslynAssert.CodeFix(Analyzer, Fix, diagnosticsAndSources, new[] { after }, fixTitle: "Rename to 'before'", settings: Settings.Default.WithAllowedCompilerDiagnostics(AllowedCompilerDiagnostics.Warnings));
        }

        [Test]
        public static void WhenOneCodeHasPosition()
        {
            var before = @"
namespace N
{
    using Microsoft.CodeAnalysis.Diagnostics;
    using Gu.Roslyn.Asserts;
    using NUnit.Framework;

    public static class C
    {
        [Test]
        public static void M()
        {
            var c1 = ""class C1 { }"";
            var wrong = ""class ↓C2 { }"";
            RoslynAssert.Diagnostics((DiagnosticAnalyzer)null, new [] { c1, wrong });
        }
    }
}";

            var after = @"
namespace N
{
    using Microsoft.CodeAnalysis.Diagnostics;
    using Gu.Roslyn.Asserts;
    using NUnit.Framework;

    public static class C
    {
        [Test]
        public static void M()
        {
            var c1 = ""class C1 { }"";
            var code = ""class ↓C2 { }"";
            RoslynAssert.Diagnostics((DiagnosticAnalyzer)null, new [] { c1, code });
        }
    }
}";
            var expectedDiagnostic = ExpectedDiagnostic.WithMessage("Name of 'wrong' should be 'code'");
            var diagnosticsAndSources = new DiagnosticsAndSources(new[] { expectedDiagnostic }, new[] { before });
            RoslynAssert.CodeFix(Analyzer, Fix, diagnosticsAndSources, new[] { after }, fixTitle: "Rename to 'code'", settings: Settings.Default.WithAllowedCompilerDiagnostics(AllowedCompilerDiagnostics.Warnings));
        }

        [Test]
        public static void WhenAssertReplace()
        {
            var before = @"
namespace N
{
    using Gu.Roslyn.Asserts;
    using NUnit.Framework;

    public static class C
    {
        private static readonly PlaceholderAnalyzer Analyzer = new PlaceholderAnalyzer();

        [TestCase(""C { }"")]
        public static void M(string declaration)
        {
            var wrong = ""class C { }"".AssertReplace(""class C { }"", declaration);
            RoslynAssert.Valid(Analyzer, ↓wrong);
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

        [TestCase(""C { }"")]
        public static void M(string declaration)
        {
            var code = ""class C { }"".AssertReplace(""class C { }"", declaration);
            RoslynAssert.Valid(Analyzer, code);
        }
    }
}";

            RoslynAssert.CodeFix(Analyzer, Fix, ExpectedDiagnostic, new[] { Code.PlaceholderAnalyzer, Code.PlaceholderFix, before }, after);
        }

        [Test]
        public static void WhenLocalNameMatchesCode()
        {
            var before = @"
namespace N
{
    using Gu.Roslyn.Asserts;
    using NUnit.Framework;

    public static class C
    {
        private static readonly PlaceholderAnalyzer Analyzer = new PlaceholderAnalyzer();

        private const string C1 = @""
namespace N
{
    public class C1 { }
}"";

        [Test]
        public static void M()
        {
            var c2 = ""class C2 { }"";
            RoslynAssert.Valid(Analyzer, C1, ↓c2);
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

        private const string C1 = @""
namespace N
{
    public class C1 { }
}"";

        [Test]
        public static void M()
        {
            var code = ""class C2 { }"";
            RoslynAssert.Valid(Analyzer, C1, code);
        }
    }
}";
            RoslynAssert.CodeFix(Analyzer, Fix, ExpectedDiagnostic, new[] { Code.PlaceholderAnalyzer, before }, after);
        }
    }
}
