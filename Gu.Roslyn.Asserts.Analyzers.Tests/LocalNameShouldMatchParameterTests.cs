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
        public static void WhenAnalyzer()
        {
            var before = @"
namespace RoslynSandbox
{
    using Microsoft.CodeAnalysis.Diagnostics;
    using Gu.Roslyn.Asserts;
    using NUnit.Framework;

    public static class C
    {
        [Test]
        public static void M()
        {
            var code = ""class Foo { }"";
            var wrong = (DiagnosticAnalyzer)null;
            RoslynAssert.Valid(↓wrong, code);
        }
    }
}";

            var after = @"
namespace RoslynSandbox
{
    using Microsoft.CodeAnalysis.Diagnostics;
    using Gu.Roslyn.Asserts;
    using NUnit.Framework;

    public static class C
    {
        [Test]
        public static void M()
        {
            var code = ""class Foo { }"";
            var analyzer = (DiagnosticAnalyzer)null;
            RoslynAssert.Valid(analyzer, code);
        }
    }
}";
            var expectedDiagnostic = ExpectedDiagnostic.WithMessage("Name of 'wrong' should be 'analyzer'.");
            RoslynAssert.CodeFix(Analyzer, Fix, expectedDiagnostic, before, after, fixTitle: "Rename to 'analyzer'");
        }

        [Test]
        public static void WhenOneParam()
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

        [Test]
        public static void DoNotWarnWhenTwoParams()
        {
            var code = @"
namespace RoslynSandbox
{
    using Microsoft.CodeAnalysis.Diagnostics;
    using Gu.Roslyn.Asserts;
    using NUnit.Framework;

    public static class C
    {
        [Test]
        public static void M()
        {
            var code1 = ""class Foo { }"";
            var code2 = ""class Foo { }"";
            RoslynAssert.Valid((DiagnosticAnalyzer)null, code1, code2);
        }
    }
}";

            RoslynAssert.Valid(Analyzer, code);
        }

        [Explicit("Temp ignore.")]
        [Test]
        public static void RunOnTestProject()
        {
            var sln = CodeFactory.CreateSolution(
                ProjectFile.Find("Gu.Roslyn.Asserts.Tests.csproj"),
                MetadataReferences.FromAttributes());
            RoslynAssert.Valid(Analyzer, sln);
        }
    }
}
