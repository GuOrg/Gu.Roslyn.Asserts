namespace Gu.Roslyn.Asserts.Tests
{
    using System.Collections.Immutable;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.Diagnostics;

    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    internal class NopAnalyzer : DiagnosticAnalyzer
    {
        private readonly SyntaxKind[] kinds;

        internal NopAnalyzer()
            : this(new[] { Descriptors.Id1 })
        {
        }

        internal NopAnalyzer(params DiagnosticDescriptor[] descriptors)
            : this(descriptors, SyntaxKind.IdentifierName)
        {
        }

        internal NopAnalyzer(DiagnosticDescriptor[] descriptors, params SyntaxKind[] kinds)
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
            context.RegisterSyntaxNodeAction(x => Handle(x), this.kinds);
        }

        private static void Handle(SyntaxNodeAnalysisContext context)
        {
        }
    }
}
