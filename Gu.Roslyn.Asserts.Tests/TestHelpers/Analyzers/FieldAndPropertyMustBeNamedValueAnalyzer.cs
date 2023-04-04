namespace Gu.Roslyn.Asserts.Tests
{
    using System.Collections.Immutable;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using Microsoft.CodeAnalysis.Diagnostics;

    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    internal sealed class FieldAndPropertyMustBeNamedValueAnalyzer : DiagnosticAnalyzer
    {
        internal const string FieldDiagnosticId = "Field";
        internal const string PropertyDiagnosticId = "Property";

        internal static readonly DiagnosticDescriptor FieldDescriptor = new(
            id: FieldDiagnosticId,
            title: "The field must be named Value",
            messageFormat: "Message format",
            category: "Category",
            defaultSeverity: DiagnosticSeverity.Warning,
            isEnabledByDefault: true);

        internal static readonly DiagnosticDescriptor PropertyDescriptor = new(
            id: PropertyDiagnosticId,
            title: "The Property must be named Value",
            messageFormat: "Message format",
            category: "Category",
            defaultSeverity: DiagnosticSeverity.Warning,
            isEnabledByDefault: true);

        /// <inheritdoc/>
        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get; } = ImmutableArray.Create(
            FieldDescriptor,
            PropertyDescriptor);

        public override void Initialize(AnalysisContext context)
        {
            if (context is null)
            {
                throw new System.ArgumentNullException(nameof(context));
            }

            context.EnableConcurrentExecution();
            context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.Analyze | GeneratedCodeAnalysisFlags.ReportDiagnostics);
            context.RegisterSyntaxNodeAction(HandleDeclaration, SyntaxKind.FieldDeclaration, SyntaxKind.PropertyDeclaration);
        }

        private static void HandleDeclaration(SyntaxNodeAnalysisContext context)
        {
            if (context.ContainingSymbol is IFieldSymbol field &&
                field.Name != "value" &&
                context.Node is FieldDeclarationSyntax fieldDeclaration)
            {
                context.ReportDiagnostic(Diagnostic.Create(FieldDescriptor, fieldDeclaration.Declaration.Variables[0].GetLocation()));
            }

            if (context.ContainingSymbol is IPropertySymbol property &&
                property.Name != "Value" &&
                context.Node is PropertyDeclarationSyntax propertyDeclaration)
            {
                context.ReportDiagnostic(Diagnostic.Create(PropertyDescriptor, propertyDeclaration.Identifier.GetLocation()));
            }
        }
    }
}
