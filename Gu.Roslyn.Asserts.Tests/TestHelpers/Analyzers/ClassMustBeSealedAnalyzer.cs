namespace Gu.Roslyn.Asserts.Tests
{
    using System.Collections.Immutable;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using Microsoft.CodeAnalysis.Diagnostics;

    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    internal class ClassMustBeSealedAnalyzer : DiagnosticAnalyzer
    {
        internal const string DiagnosticId = "ClassMustBeSealed";

        private static readonly DiagnosticDescriptor Descriptor = new(
            id: DiagnosticId,
            title: "This analyzer reports warnings if a class is not sealed",
            messageFormat: "Message format",
            category: "Category",
            defaultSeverity: DiagnosticSeverity.Warning,
            isEnabledByDefault: true);

        /// <inheritdoc/>
        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get; } = ImmutableArray.Create(Descriptor);

        public override void Initialize(AnalysisContext context)
        {
            context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.Analyze | GeneratedCodeAnalysisFlags.ReportDiagnostics);
            context.EnableConcurrentExecution();
            if (context is null)
            {
                throw new System.ArgumentNullException(nameof(context));
            }

            context.RegisterSyntaxNodeAction(HandleDeclaration, SyntaxKind.ClassDeclaration);
        }

        private static void HandleDeclaration(SyntaxNodeAnalysisContext context)
        {
            var classDeclaration = (ClassDeclarationSyntax)context.Node;
            if (!classDeclaration.Modifiers.Any(SyntaxKind.SealedKeyword))
            {
                context.ReportDiagnostic(Diagnostic.Create(Descriptor, classDeclaration.GetLocation()));
            }
        }
    }
}
