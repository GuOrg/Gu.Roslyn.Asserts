namespace Gu.Roslyn.Asserts.Tests
{
    using System.Collections.Immutable;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.Diagnostics;

    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    internal class IdentifierNameAnalyzer : DiagnosticAnalyzer
    {
        internal IdentifierNameAnalyzer(params DiagnosticDescriptor[] descriptors)
        {
            this.SupportedDiagnostics = ImmutableArray.Create(descriptors);
        }

        /// <inheritdoc/>
        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get; }

        public override void Initialize(AnalysisContext context)
        {
            if (context == null)
            {
                throw new System.ArgumentNullException(nameof(context));
            }

            context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.Analyze | GeneratedCodeAnalysisFlags.ReportDiagnostics);
            context.EnableConcurrentExecution();
            context.RegisterSyntaxNodeAction(HandleIdentifierName, SyntaxKind.IdentifierName);
        }

        private static void HandleIdentifierName(SyntaxNodeAnalysisContext context)
        {
        }
    }
}
