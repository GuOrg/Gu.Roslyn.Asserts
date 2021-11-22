namespace Gu.Roslyn.Asserts.Tests
{
    using System.Collections.Immutable;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using Microsoft.CodeAnalysis.Diagnostics;

    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    internal class PropertyMustBeNamedValueAnalyzer : DiagnosticAnalyzer
    {
        internal const string DiagnosticId = "PropertyMustBeNamedValueAnalyzer";

        internal static readonly DiagnosticDescriptor PropertyDescriptor = new(
            id: DiagnosticId,
            title: "The Property must be named Value",
            messageFormat: "Message format",
            category: "Category",
            defaultSeverity: DiagnosticSeverity.Warning,
            isEnabledByDefault: true);

        /// <inheritdoc/>
        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get; } = ImmutableArray.Create(
            PropertyDescriptor);

        public override void Initialize(AnalysisContext context)
        {
            if (context is null)
            {
                throw new System.ArgumentNullException(nameof(context));
            }

            context.EnableConcurrentExecution();
            context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.Analyze | GeneratedCodeAnalysisFlags.ReportDiagnostics);
            context.RegisterSyntaxNodeAction(HandleDeclaration, SyntaxKind.PropertyDeclaration);
        }

        private static void HandleDeclaration(SyntaxNodeAnalysisContext context)
        {
            if (context.ContainingSymbol is IPropertySymbol property &&
                property.Name != "Value" &&
                context.Node is PropertyDeclarationSyntax propertyDeclaration)
            {
                context.ReportDiagnostic(Diagnostic.Create(PropertyDescriptor, propertyDeclaration.Identifier.GetLocation()));
            }
        }
    }
}
