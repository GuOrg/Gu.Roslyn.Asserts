namespace Gu.Roslyn.Asserts.Tests
{
    using System.Collections.Immutable;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.Diagnostics;

    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    internal class DummyAnalyzer : DiagnosticAnalyzer
    {
        public DummyAnalyzer(params DiagnosticDescriptor[] descriptors)
        {
            this.SupportedDiagnostics = ImmutableArray.Create(descriptors);
        }

        /// <inheritdoc/>
        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get; }

        public override void Initialize(AnalysisContext context)
        {
            context.RegisterSyntaxNodeAction(HandleIdentifierName, SyntaxKind.IdentifierName);
        }

        private static void HandleIdentifierName(SyntaxNodeAnalysisContext context)
        {
        }
    }
}