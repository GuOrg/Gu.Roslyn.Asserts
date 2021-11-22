namespace Gu.Roslyn.Asserts.Analyzers
{
    using System.Collections.Immutable;
    using Gu.Roslyn.AnalyzerExtensions;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using Microsoft.CodeAnalysis.Diagnostics;

    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    internal class MethodDeclarationAnalyzer : DiagnosticAnalyzer
    {
        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get; } = ImmutableArray.Create(
            Descriptors.GURA06TestShouldBeInCorrectClass);

        public override void Initialize(AnalysisContext context)
        {
            context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
            context.EnableConcurrentExecution();
            context.RegisterSyntaxNodeAction(c => Handle(c), SyntaxKind.MethodDeclaration);
        }

        private static void Handle(SyntaxNodeAnalysisContext context)
        {
            if (context.Node is MethodDeclarationSyntax methodDeclaration &&
                context.ContainingSymbol is IMethodSymbol method &&
                InvocationWalker.TryFindRoslynAssert(methodDeclaration, out var invocation))
            {
                if (invocation.TryGetMethodName(out var name) &&
                    name != method.ContainingType.Name &&
                    name != method.ContainingType.ContainingType?.Name)
                {
                    context.ReportDiagnostic(Diagnostic.Create(
                        Descriptors.GURA06TestShouldBeInCorrectClass,
                        methodDeclaration.Identifier.GetLocation(),
                        ImmutableDictionary<string, string?>.Empty.Add(nameof(IdentifierNameSyntax), name),
                        name));
                }
            }
        }
    }
}
