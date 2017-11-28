namespace Gu.Roslyn.Asserts.Tests
{
    using System.Collections.Generic;
    using System.Collections.Immutable;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.Diagnostics;

    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    internal class SymbolAnalyzer : DiagnosticAnalyzer
    {
        private static readonly DiagnosticDescriptor Descriptor = new DiagnosticDescriptor(
            "123",
            "SyntaxNodeAnalyzer",
            "SyntaxNodeAnalyzer",
            "SyntaxNodeAnalyzer",
            DiagnosticSeverity.Warning,
            isEnabledByDefault: true);

        private readonly SymbolKind[] kinds;
        private readonly List<SymbolAnalysisContext> contexts = new List<SymbolAnalysisContext>();

        public SymbolAnalyzer(params SymbolKind[] kinds)
        {
            this.kinds = kinds;
        }

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get; } = ImmutableArray.Create(Descriptor);

        public IReadOnlyList<SymbolAnalysisContext> Contexts => this.contexts;

        /// <inheritdoc/>
        public override void Initialize(AnalysisContext context)
        {
            context.RegisterSymbolAction(this.Handle, this.kinds);
        }

        private void Handle(SymbolAnalysisContext context)
        {
            this.contexts.Add(context);
        }
    }
}