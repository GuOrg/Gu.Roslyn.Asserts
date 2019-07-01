namespace Gu.Roslyn.Asserts.Tests
{
    using System;
    using System.Collections.Immutable;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using Microsoft.CodeAnalysis.Diagnostics;

    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    internal class FieldNameMustNotBeginWithUnderscoreReportsTwo : DiagnosticAnalyzer
    {
        public const string DiagnosticId1 = "SA1309_1";
        public const string DiagnosticId2 = "SA2309_2";

        internal static readonly DiagnosticDescriptor Descriptor1 = new DiagnosticDescriptor(
            DiagnosticId1,
            "Field names must not begin with underscore",
            "Field '{0}' must not begin with an underscore 1",
            "Naming",
            DiagnosticSeverity.Warning,
            isEnabledByDefault: true);

        internal static readonly DiagnosticDescriptor Descriptor2 = new DiagnosticDescriptor(
            DiagnosticId2,
            "Field names must not begin with underscore",
            "Field '{0}' must not begin with an underscore 2",
            "Naming",
            DiagnosticSeverity.Warning,
            isEnabledByDefault: true);

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get; } = ImmutableArray.Create(Descriptor1, Descriptor2);

        /// <inheritdoc/>
        public override void Initialize(AnalysisContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            context.EnableConcurrentExecution();
            context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.Analyze | GeneratedCodeAnalysisFlags.ReportDiagnostics);
            context.RegisterSyntaxNodeAction(c => Handle(c), SyntaxKind.FieldDeclaration);
        }

        private static void Handle(SyntaxNodeAnalysisContext context)
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
                context.ReportDiagnostic(Diagnostic.Create(Descriptor1, identifier.GetLocation(), name));
                context.ReportDiagnostic(Diagnostic.Create(Descriptor2, identifier.GetLocation(), name));
            }
        }
    }
}
