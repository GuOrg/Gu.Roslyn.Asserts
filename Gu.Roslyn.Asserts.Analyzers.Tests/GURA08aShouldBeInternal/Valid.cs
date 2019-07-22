namespace Gu.Roslyn.Asserts.Analyzers.Tests.GURA08aShouldBeInternal
{
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.Diagnostics;
    using NUnit.Framework;

    public static class Valid
    {
        private static readonly DiagnosticAnalyzer Analyzer = new ObjectCreationAnalyzer();
        private static readonly DiagnosticDescriptor Descriptor = Descriptors.GURA08bShouldBePublic;

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
