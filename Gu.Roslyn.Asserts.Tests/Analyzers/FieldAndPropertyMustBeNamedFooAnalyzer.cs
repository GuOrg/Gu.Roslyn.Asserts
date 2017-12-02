namespace Gu.Roslyn.Asserts.Tests
{
    using System.Collections.Immutable;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using Microsoft.CodeAnalysis.Diagnostics;

    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    internal class FieldAndPropertyMustBeNamedFooAnalyzer : DiagnosticAnalyzer
    {
        public const string FieldDiagnosticId = "Field";
        public const string PropertyDiagnosticId = "Property";

        private static readonly DiagnosticDescriptor FieldDescriptor = new DiagnosticDescriptor(
            id: FieldDiagnosticId,
            title: "The field must be named foo.",
            messageFormat: "Message format.",
            category: "Category",
            defaultSeverity: DiagnosticSeverity.Warning,
            isEnabledByDefault: true);

        private static readonly DiagnosticDescriptor PropertyDescriptor = new DiagnosticDescriptor(
            id: PropertyDiagnosticId,
            title: "The Property must be named Foo.",
            messageFormat: "Message format.",
            category: "Category",
            defaultSeverity: DiagnosticSeverity.Warning,
            isEnabledByDefault: true);

        /// <inheritdoc/>
        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get; } = ImmutableArray.Create(
            FieldDescriptor,
            PropertyDescriptor);

        public override void Initialize(AnalysisContext context)
        {
            context.RegisterSyntaxNodeAction(HandleDeclaration, SyntaxKind.FieldDeclaration, SyntaxKind.PropertyDeclaration);
        }

        private static void HandleDeclaration(SyntaxNodeAnalysisContext context)
        {
            if (context.Node is FieldDeclarationSyntax fieldDeclaration &&
                fieldDeclaration.Declaration.Variables[0].Identifier.ValueText != "foo")
            {
                context.ReportDiagnostic(Diagnostic.Create(FieldDescriptor, fieldDeclaration.Declaration.Variables[0].GetLocation()));
            }

            if (context.Node is PropertyDeclarationSyntax propertyDeclaration &&
                propertyDeclaration.Identifier.ValueText != "foo")
            {
                context.ReportDiagnostic(Diagnostic.Create(PropertyDescriptor, propertyDeclaration.Identifier.GetLocation()));
            }
        }
    }
}