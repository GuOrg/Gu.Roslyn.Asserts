namespace Gu.Roslyn.Asserts.Analyzers.Tests
{
    public static class Code
    {
        public const string PlaceholderAnalyzer = @"
namespace RoslynSandbox
{
    using System.Collections.Immutable;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.Diagnostics;

    public class PlaceholderAnalyzer : DiagnosticAnalyzer
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

        public const string PlaceholderFix = @"
namespace RoslynSandbox
{
    using System.Collections.Immutable;
    using System.Threading.Tasks;
    using Microsoft.CodeAnalysis.CodeFixes;

    public class PlaceholderFix : CodeFixProvider
    {
        public override ImmutableArray<string> FixableDiagnosticIds { get; }

        public override FixAllProvider GetFixAllProvider() => null;

        public override Task RegisterCodeFixesAsync(CodeFixContext context) => Task.CompletedTask;
    }
}";

        public const string PlaceholderRefactoring = @"
namespace RoslynSandbox
{
    using System.Threading.Tasks;
    using Microsoft.CodeAnalysis.CodeRefactorings;

    public class PlaceholderRefactoring : CodeRefactoringProvider
    {
        public override Task ComputeRefactoringsAsync(CodeRefactoringContext context) => Task.CompletedTask;
    }
}";
    }
}
