namespace Gu.Roslyn.Asserts.Tests
{
    using System;
    using System.Collections.Immutable;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using Microsoft.CodeAnalysis.Diagnostics;

    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    internal sealed class FieldNameMustNotBeginWithUnderscoreDifferentDiagnosticsForPublic : DiagnosticAnalyzer
    {
        internal const string Id1 = "SA1309a";
        internal const string Id2 = "SA1309b";

        private static readonly DiagnosticDescriptor Descriptor1 = new(
            Id1,
            "Field names must not begin with underscore",
            "Field '{0}' must not begin with an underscore",
            "Naming",
            DiagnosticSeverity.Warning,
            isEnabledByDefault: true);

        private static readonly DiagnosticDescriptor Descriptor2 = new(
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
            context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
            context.EnableConcurrentExecution();
            context.RegisterSyntaxNodeAction(FieldDeclarationAction, SyntaxKind.FieldDeclaration);
        }

        private static void HandleFieldDeclaration(SyntaxNodeAnalysisContext context)
        {
            var syntax = (FieldDeclarationSyntax)context.Node;
            var variables = syntax.Declaration?.Variables;
            if (variables is null)
            {
                return;
            }

            foreach (var variableDeclarator in variables.Value)
            {
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
                if (context.ContainingSymbol.DeclaredAccessibility == Accessibility.Public)
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
