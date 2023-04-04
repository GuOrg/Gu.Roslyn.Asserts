namespace Gu.Roslyn.Asserts.Tests;

using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

[DiagnosticAnalyzer(LanguageNames.CSharp)]
internal sealed class CallIdAnalyzer : DiagnosticAnalyzer
{
    internal const string DiagnosticId = "CallId";

    private static readonly DiagnosticDescriptor Descriptor = new(
        id: DiagnosticId,
        title: "This analyzer reports warnings if the extension method ID() is not called",
        messageFormat: "Message format",
        category: "Category",
        defaultSeverity: DiagnosticSeverity.Warning,
        isEnabledByDefault: true);

    /// <inheritdoc/>
    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get; } = ImmutableArray.Create(Descriptor);

    public override void Initialize(AnalysisContext context)
    {
        if (context is null)
        {
            throw new System.ArgumentNullException(nameof(context));
        }

        context.EnableConcurrentExecution();
        context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.Analyze | GeneratedCodeAnalysisFlags.ReportDiagnostics);
        context.RegisterSyntaxNodeAction(HandleDeclaration, SyntaxKind.ObjectCreationExpression);
    }

    private static void HandleDeclaration(SyntaxNodeAnalysisContext context)
    {
        if (context.Node is ObjectCreationExpressionSyntax objectCreation &&
            !CallsId(objectCreation))
        {
            context.ReportDiagnostic(Diagnostic.Create(Descriptor, objectCreation.GetLocation()));
        }

        static bool CallsId(ObjectCreationExpressionSyntax candidate)
        {
            return candidate.Parent is MemberAccessExpressionSyntax memberAccess &&
                   memberAccess.Parent is InvocationExpressionSyntax;
        }
    }
}
