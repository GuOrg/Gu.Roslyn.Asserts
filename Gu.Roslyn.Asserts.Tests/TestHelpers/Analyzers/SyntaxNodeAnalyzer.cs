namespace Gu.Roslyn.Asserts.Tests
{
    using System.Collections.Immutable;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.Diagnostics;

    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    internal class SyntaxNodeAnalyzer : DiagnosticAnalyzer
    {
        private readonly SyntaxKind[] kinds;

        internal SyntaxNodeAnalyzer(params DiagnosticDescriptor[] descriptors)
            : this(descriptors, SyntaxKind.IdentifierName)
        {
        }

        internal SyntaxNodeAnalyzer(DiagnosticDescriptor[] descriptors, params SyntaxKind[] kinds)
        {
            this.kinds = kinds;
            this.SupportedDiagnostics = ImmutableArray.Create(descriptors);
        }

        /// <inheritdoc/>
        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get; }

        public override void Initialize(AnalysisContext context)
        {
            context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.Analyze | GeneratedCodeAnalysisFlags.ReportDiagnostics);
            context.EnableConcurrentExecution();
            context.RegisterSyntaxNodeAction(this.HandleIdentifierName, this.kinds);
        }

        private void HandleIdentifierName(SyntaxNodeAnalysisContext context)
        {
            foreach (var diagnostic in this.SupportedDiagnostics)
            {
                context.ReportDiagnostic(Diagnostic.Create(diagnostic, context.Node.GetLocation()));
            }
        }
    }
}
