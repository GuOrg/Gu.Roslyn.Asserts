namespace Gu.Roslyn.Asserts.Analyzers.Tests.GURA09UseStandardNames;

using NUnit.Framework;

public static class Valid
{
    private static readonly DiagnosticAssert Assert = RoslynAssert.Create<InvocationAnalyzer>();

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
            var code = @""
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
            RoslynAssert.Valid(Analyzer, code);
        }
    }
}";
        Assert.Valid(Code.PlaceholderAnalyzer, code);
    }

    [Test]
    public static void DiagnosticAnalyzer()
    {
        var code = @"
namespace N
{
    using System.Collections.Immutable;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.Diagnostics;

    public class Analyzer : DiagnosticAnalyzer
    {
        /// <inheritdoc/>
        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get; }

        public override void Initialize(AnalysisContext context)
        {
        }

        private static void Handle(SyntaxNodeAnalysisContext context)
        {
        }
    }
}";
        Assert.Valid(Code.PlaceholderAnalyzer, code);
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
            var code = @""
class C
{
    const string Text = """"Activator.CreateInstance(typeof(T), \""""foo\"""")"""";
}"";
            RoslynAssert.Valid(Analyzer, code);
        }
    }
}";

        Assert.Valid(Code.PlaceholderAnalyzer, code);
    }
}
