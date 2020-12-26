namespace Gu.Roslyn.Asserts.Analyzers
{
    using System.Collections.Immutable;
    using System.Linq;
    using Gu.Roslyn.AnalyzerExtensions;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using Microsoft.CodeAnalysis.Diagnostics;

    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    internal class ObjectCreationAnalyzer : DiagnosticAnalyzer
    {
        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get; } = ImmutableArray.Create(
            Descriptors.GURA08aShouldBeInternal,
            Descriptors.GURA08bShouldBePublic);

        public override void Initialize(AnalysisContext context)
        {
            context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
            context.EnableConcurrentExecution();
            context.RegisterSyntaxNodeAction(c => Handle(c), SyntaxKind.ObjectCreationExpression);
        }

        private static void Handle(SyntaxNodeAnalysisContext context)
        {
            if (context.Node is ObjectCreationExpressionSyntax objectCreation &&
                context.SemanticModel.TryGetNamedType(objectCreation, context.CancellationToken, out var type) &&
                type.Locations.Any(x => x.IsInSource))
            {
                if (type.IsAssignableTo(KnownSymbols.CodeFixProvider, context.Compilation) ||
                    type.IsAssignableTo(KnownSymbols.CodeRefactoringProvider, context.Compilation) ||
                    type.IsAssignableTo(KnownSymbols.DiagnosticAnalyzer, context.Compilation))
                {
                    switch (type.DeclaredAccessibility)
                    {
                        case Accessibility.Internal:
                            context.ReportDiagnostic(
                                Diagnostic.Create(
                                    Descriptors.GURA08bShouldBePublic,
                                    objectCreation.Type.GetLocation(),
                                    $"{type.ToMinimalDisplayString(context.SemanticModel, context.Node.SpanStart)}"));
                            break;
                        case Accessibility.Public:
                            context.ReportDiagnostic(
                                Diagnostic.Create(
                                    Descriptors.GURA08aShouldBeInternal,
                                    objectCreation.Type.GetLocation(),
                                    $"{type.ToMinimalDisplayString(context.SemanticModel, context.Node.SpanStart)}"));
                            break;
                    }
                }
            }
        }
    }
}
