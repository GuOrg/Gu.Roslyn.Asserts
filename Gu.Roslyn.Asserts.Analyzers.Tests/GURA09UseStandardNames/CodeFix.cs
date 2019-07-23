namespace Gu.Roslyn.Asserts.Analyzers.Tests.GURA09UseStandardNames
{
    using Microsoft.CodeAnalysis.CodeFixes;
    using Microsoft.CodeAnalysis.Diagnostics;
    using NUnit.Framework;

    public static partial class CodeFix
    {
        private static readonly DiagnosticAnalyzer Analyzer = new MethodDeclarationAnalyzer();
        private static readonly CodeFixProvider Fix = new StandardNamesFix();
        private static readonly ExpectedDiagnostic ExpectedDiagnostic = ExpectedDiagnostic.Create(Descriptors.GURA09UseStandardNames);

        [Explicit("Temp suppress.")]
        [Test]
        public static void MethodNamedBar()
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
        public static void M1()
        {
            var c = @""
namespace N
{
    class C
    {
        void ↓Bar(int i) { }
    }
}"";
            RoslynAssert.Valid(Analyzer, c);
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
            var c = @""
namespace N
{
    class C
    {
        void M(int i) { }
    }
}"";
            RoslynAssert.Valid(Analyzer, c);
        }
    }
}";

            RoslynAssert.CodeFix(Analyzer, Fix, ExpectedDiagnostic, new[] { Code.PlaceholderAnalyzer, before }, new[] { Code.PlaceholderAnalyzer, after });
        }
    }
}
