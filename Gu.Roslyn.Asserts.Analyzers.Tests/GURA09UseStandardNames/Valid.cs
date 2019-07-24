namespace Gu.Roslyn.Asserts.Analyzers.Tests.GURA09UseStandardNames
{
    using Microsoft.CodeAnalysis.Diagnostics;
    using NUnit.Framework;

    public static class Valid
    {
        private static readonly DiagnosticAnalyzer Analyzer = new ClassDeclarationAnalyzer();

        [Test]
        public static void SimpleClass()
        {
            var code = @"
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
            var c = @""
namespace N
{
    class C
    {
        private readonly int f;

        C(int f, int p)
        {
            this.f = f;
            this.P = p;
        }

        public int P { get; }
    }
}"";
            RoslynAssert.Valid(Analyzer, c);
        }
    }
}";
            RoslynAssert.Valid(Analyzer, Code.PlaceholderAnalyzer, code);
        }

        [Test]
        public static void StringLiteral()
        {
            var code = @"
namespace N
{
    using Gu.Roslyn.Asserts;
    using NUnit.Framework;

    public static class Valid
    {
        private static readonly PlaceholderAnalyzer Analyzer = new PlaceholderAnalyzer();

        [Test]
        public static void FooTest()
        {
            var c = @""
class C
{
    const string Text = """"Activator.CreateInstance(typeof(T), \""""foo\"""")  """";
}"";
            RoslynAssert.Valid(Analyzer, c);
        }
    }
}";

            RoslynAssert.Valid(Analyzer, new[] { Code.PlaceholderAnalyzer, code });
        }
    }
}
