namespace Gu.Roslyn.Asserts.Analyzers.Tests.GURA08aShouldBeInternal
{
    using NUnit.Framework;

    public static class Valid
    {
        private static readonly DiagnosticAssert Assert = RoslynAssert.Create<ObjectCreationAnalyzer>(Descriptors.GURA08aShouldBeInternal);

        [Test]
        public static void CodeRefactoringProvider()
        {
            var refactoring = @"
namespace N
{
    using System.Threading.Tasks;
    using Microsoft.CodeAnalysis.CodeRefactorings;

    public class PlaceholderRefactoring : CodeRefactoringProvider
    {
        public override Task ComputeRefactoringsAsync(CodeRefactoringContext context) => Task.CompletedTask;
    }
}";
            Assert.Valid(refactoring);
        }

        [Test]
        public static void EmptyClass()
        {
            var c = @"
namespace N
{
    public class C
    {
    }
}";
            Assert.Valid(c);
        }

        [Test]
        public static void RoslynAssertDiagnostics()
        {
            var analyzer = @"
namespace N
{
    using System.Collections.Immutable;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.Diagnostics;

    internal class Analyzer : DiagnosticAnalyzer
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
            var diagnostics = @"
namespace N
{
    using Gu.Roslyn.Asserts;
    using NUnit.Framework;

    public static class Diagnostics
    {
        private static readonly Analyzer Analyzer = new Analyzer();

        [TestCase(""C2 { }"")]
        public static void M(string declaration)
        {
            var c1 = ""class C1 { }"";
            var code = ""class C2 { }"".AssertReplace(""C2 { }"", declaration);
            RoslynAssert.Diagnostics(Analyzer, c1, code);
        }
    }
}";
            Assert.Valid(analyzer, diagnostics);
        }

        [Test]
        public static void DiagnosticGenericAnalyzer()
        {
            var analyzer = @"
namespace N
{
    using System.Collections.Immutable;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.Diagnostics;

    internal class Analyzer : DiagnosticAnalyzer
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

            var diagnostics = @"
namespace N
{
    using Microsoft.CodeAnalysis.Diagnostics;

    using Gu.Roslyn.Asserts;
    using NUnit.Framework;

    [TestFixture(typeof(Analyzer))]
    public static class Diagnostics<T>
        where T : DiagnosticAnalyzer, new()
    {
        private static readonly DiagnosticAnalyzer Analyzer = new T();

        [TestCase(""C2 { }"")]
        public static void M(string declaration)
        {
            var c1 = ""class C1 { }"";
            var code = ""class C2 { }"".AssertReplace(""C2 { }"", declaration);
            RoslynAssert.Diagnostics(Analyzer, c1, code);
        }
    }
}";
            Assert.Valid(analyzer, diagnostics);
        }
    }
}
