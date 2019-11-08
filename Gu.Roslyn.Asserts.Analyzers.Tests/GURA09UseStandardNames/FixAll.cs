namespace Gu.Roslyn.Asserts.Analyzers.Tests.GURA09UseStandardNames
{
    using Microsoft.CodeAnalysis.CodeFixes;
    using Microsoft.CodeAnalysis.Diagnostics;
    using NUnit.Framework;

    public static class FixAll
    {
        private static readonly DiagnosticAnalyzer Analyzer = new MethodDeclarationAnalyzer();
        private static readonly CodeFixProvider Fix = new StandardNamesFix();
        private static readonly ExpectedDiagnostic ExpectedDiagnostic = ExpectedDiagnostic.Create(Descriptors.GURA09UseStandardNames);

        [Test]
        public static void FooWithPropertyBarValid()
        {
            var before = @"
namespace N
{
    using Gu.Roslyn.Asserts;
    using NUnit.Framework;

    public static class Valid
    {
        private static readonly PlaceholderAnalyzer Analyzer = new PlaceholderAnalyzer();

        [Test]
        public static void M()
        {
            var foo = @""
public class ↓Foo
{
    public int ↓Bar { get; set; }
}"";
            RoslynAssert.Valid(Analyzer, foo);
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
        public static void M()
        {
            var foo = @""
public class C
{
    public int P { get; set; }
}"";
            RoslynAssert.Valid(Analyzer, foo);
        }
    }
}";

            RoslynAssert.FixAll(Analyzer, Fix, ExpectedDiagnostic, new[] { Code.PlaceholderAnalyzer, before }, new[] { Code.PlaceholderAnalyzer, after });
        }

        [Test]
        public static void FooWithPropertyBarCodeFix()
        {
            var before = @"
namespace N
{
    using Gu.Roslyn.Asserts;
    using NUnit.Framework;

    public static class Valid
    {
        private static readonly PlaceholderAnalyzer Analyzer = new PlaceholderAnalyzer();
        private static readonly PlaceholderFix Fix = new PlaceholderFix();

        [Test]
        public static void M()
        {
            var before = @""
public class ↓Foo
{
    public int ↓Bar { get; set; }
}"";

            var after = @""
public class Foo
{
    public int Bar { get; set; }
}"";
            RoslynAssert.CodeFix(Analyzer, Fix, before, after);
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
        private static readonly PlaceholderFix Fix = new PlaceholderFix();

        [Test]
        public static void M()
        {
            var foo = @""
public class C
{
    public int P { get; set; }
}"";

            var after = @""
public class C
{
    public int P { get; set; }
}"";
            RoslynAssert.CodeFix(Analyzer, Fix, before, after);
        }
    }
}";

            RoslynAssert.FixAll(Analyzer, Fix, ExpectedDiagnostic, new[] { Code.PlaceholderAnalyzer, Code.PlaceholderFix, before }, new[] { Code.PlaceholderAnalyzer, Code.PlaceholderFix, after });
        }
    }
}
