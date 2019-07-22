namespace Gu.Roslyn.Asserts.Analyzers.Tests.GURA08bShouldBePublic
{
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.Diagnostics;
    using NUnit.Framework;

    public static class Valid
    {
        private static readonly DiagnosticAnalyzer Analyzer = new ObjectCreationAnalyzer();
        private static readonly DiagnosticDescriptor Descriptor = Descriptors.GURA08aShouldBeInternal;

        [Test]
        public static void DiagnosticAnalyzer()
        {
            var diagnostics = @"
namespace N
{
    using Gu.Roslyn.Asserts;
    using NUnit.Framework;

    public static class Diagnostics
    {
        private static readonly PlaceholderAnalyzer Analyzer = new PlaceholderAnalyzer();

        [TestCase(""C2 { }"")]
        public static void M(string declaration)
        {
            var c1 = ""class C1 { }"";
            var code = ""↓class C2 { }"".AssertReplace(""C2 { }"", declaration);
            RoslynAssert.Diagnostics(Analyzer, c1, code);
        }
    }
}";
            RoslynAssert.Valid(Analyzer, Descriptor, Code.PlaceholderAnalyzer, diagnostics);
        }

        [Test]
        public static void CodeFixProvider()
        {
            var diagnostics = @"
namespace N
{
    using Gu.Roslyn.Asserts;
    using NUnit.Framework;

    public static class Diagnostics
    {
        private static readonly PlaceholderAnalyzer Analyzer = new PlaceholderAnalyzer();
        private static readonly PlaceholderFix Fix = new PlaceholderFix();

        [TestCase(""C2 { }"")]
        public static void M(string declaration)
        {
            var c1 = ""class C1 { }"";
            var code = ""↓class C2 { }"".AssertReplace(""C2 { }"", declaration);
            RoslynAssert.Diagnostics(Analyzer, c1, code);
        }
    }
}";
            RoslynAssert.Valid(Analyzer, Descriptor, Code.PlaceholderAnalyzer, Code.PlaceholderFix, diagnostics);
        }

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
            RoslynAssert.Valid(Analyzer, Descriptor, refactoring);
        }

        [Test]
        public static void RandomClass()
        {
            var c = @"
namespace N
{
    using System.Collections.Immutable;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.Diagnostics;

    public class C
    {
    }
}";
            RoslynAssert.Valid(Analyzer, Descriptor, c);
        }
    }
}
