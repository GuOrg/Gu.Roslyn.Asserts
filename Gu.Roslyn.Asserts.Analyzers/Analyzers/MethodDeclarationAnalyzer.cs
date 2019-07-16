namespace Gu.Roslyn.Asserts.Analyzers
{
    using System.Collections.Immutable;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using Microsoft.CodeAnalysis.Diagnostics;

    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class MethodDeclarationAnalyzer : DiagnosticAnalyzer
    {
        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get; } = ImmutableArray.Create(
            Descriptors.TestShouldBeInCorrectClass);

        public override void Initialize(AnalysisContext context)
        {
            context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
            context.EnableConcurrentExecution();
            context.RegisterSyntaxNodeAction(c => Handle(c), SyntaxKind.MethodDeclaration);
        }

        private static void Handle(SyntaxNodeAnalysisContext context)
        {
            if (context.Node is MethodDeclarationSyntax methodDeclaration &&
                context.ContainingSymbol is IMethodSymbol method)
            {
                switch (method.ContainingType.Name)
                {
                    case "CodeFix":
                    case "Diagnostics":
                    case "Valid":
                        if (InvocationWalker.TryFindName(methodDeclaration, out var name) &&
                            name != method.ContainingType.Name)
                        {
                            context.ReportDiagnostic(Diagnostic.Create(
                                Descriptors.TestShouldBeInCorrectClass,
                                methodDeclaration.Identifier.GetLocation(),
                                ImmutableDictionary<string, string>.Empty.Add(nameof(IdentifierNameSyntax), name),
                                name));
                        }

                        break;
                }
            }
        }
    }
}
