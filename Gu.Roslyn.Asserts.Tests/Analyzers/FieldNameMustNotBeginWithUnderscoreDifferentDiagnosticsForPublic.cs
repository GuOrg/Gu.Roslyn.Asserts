namespace Gu.Roslyn.Asserts.Tests
{
    using System;
    using System.Collections.Immutable;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using Microsoft.CodeAnalysis.Diagnostics;

    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    internal class FieldNameMustNotBeginWithUnderscoreDifferentDiagnosticsForPublic : DiagnosticAnalyzer
    {
        public const string Id1 = "ID1";
        public const string Id2 = "ID2";

        private static readonly DiagnosticDescriptor Descriptor1 = new DiagnosticDescriptor(
            Id1,
            "Field names must not begin with underscore",
            "Field '{0}' must not begin with an underscore",
            "Naming",
            DiagnosticSeverity.Warning,
            isEnabledByDefault: true);

        private static readonly DiagnosticDescriptor Descriptor2 = new DiagnosticDescriptor(
            Id2,
            "Field names must not begin with underscore",
            "Field '{0}' must not begin with an underscore",
            "Naming",
            DiagnosticSeverity.Warning,
            isEnabledByDefault: true);

        private static readonly Action<SyntaxNodeAnalysisContext> FieldDeclarationAction = HandleFieldDeclaration;

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get; } = ImmutableArray.Create(Descriptor1, Descriptor2);

        /// <inheritdoc/>
        public override void Initialize(AnalysisContext context)
        {
            context.RegisterSyntaxNodeAction(FieldDeclarationAction, SyntaxKind.FieldDeclaration);
        }

        private static void HandleFieldDeclaration(SyntaxNodeAnalysisContext context)
        {
            var syntax = (FieldDeclarationSyntax)context.Node;
            var variables = syntax.Declaration?.Variables;
            if (variables == null)
            {
                return;
            }

            foreach (VariableDeclaratorSyntax variableDeclarator in variables.Value)
            {
                if (variableDeclarator == null)
                {
                    continue;
                }

                var identifier = variableDeclarator.Identifier;
                if (identifier.IsMissing)
                {
                    continue;
                }

                if (!identifier.ValueText.StartsWith("_", StringComparison.Ordinal))
                {
                    continue;
                }

                var name = identifier.ValueText;
                if (context.SemanticModel.GetDeclaredSymbol(variableDeclarator, context.CancellationToken).DeclaredAccessibility == Accessibility.Public)
                {
                    context.ReportDiagnostic(Diagnostic.Create(Descriptor1, identifier.GetLocation(), name));
                }
                else
                {
                    context.ReportDiagnostic(Diagnostic.Create(Descriptor2, identifier.GetLocation(), name));
                }
            }
        }
    }
}