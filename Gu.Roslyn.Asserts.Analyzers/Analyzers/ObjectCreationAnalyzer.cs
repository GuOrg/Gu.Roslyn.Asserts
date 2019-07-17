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
    public class ObjectCreationAnalyzer : DiagnosticAnalyzer
    {
        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get; } = ImmutableArray.Create(
            Descriptors.ShouldBeInternal,
            Descriptors.ShouldBePublic);

        public override void Initialize(AnalysisContext context)
        {
            context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
            context.EnableConcurrentExecution();
            context.RegisterSyntaxNodeAction(c => Handle(c), SyntaxKind.ObjectCreationExpression);
        }

        private static void Handle(SyntaxNodeAnalysisContext context)
        {
            if (context.Node is ObjectCreationExpressionSyntax objectCreation &&
                context.SemanticModel.TryGetType(objectCreation, context.CancellationToken, out var type) &&
                type.Locations.Any(x => x.IsInSource))
            {
                if (type.IsAssignableTo(KnownSymbol.CodeFixProvider, context.Compilation) ||
                    type.IsAssignableTo(KnownSymbol.CodeRefactoringProvider, context.Compilation) ||
                    type.IsAssignableTo(KnownSymbol.DiagnosticAnalyzer, context.Compilation))
                {
                    if (type.DeclaredAccessibility != Accessibility.Internal)
                    {
                        context.ReportDiagnostic(
                            Diagnostic.Create(
                                Descriptors.ShouldBeInternal,
                                objectCreation.Type.GetLocation(),
                                $"{type.ToMinimalDisplayString(context.SemanticModel, context.Node.SpanStart)}"));
                    }
                    else if (type.DeclaredAccessibility != Accessibility.Public)
                    {
                        context.ReportDiagnostic(
                            Diagnostic.Create(
                                Descriptors.ShouldBePublic,
                                objectCreation.Type.GetLocation(),
                                $"{type.ToMinimalDisplayString(context.SemanticModel, context.Node.SpanStart)}"));
                    }
                }
            }
        }
    }
}
