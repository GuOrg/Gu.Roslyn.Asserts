namespace Gu.Roslyn.Asserts.Tests
{
    using System.Collections.Generic;
    using System.Collections.Immutable;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.Diagnostics;

    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    internal sealed class SymbolAnalyzer : DiagnosticAnalyzer
    {
        private static readonly DiagnosticDescriptor Descriptor = new(
            "123",
            "SymbolAnalyzer",
            "SymbolAnalyzer",
            "SymbolAnalyzer",
            DiagnosticSeverity.Warning,
            isEnabledByDefault: true);

        private readonly SymbolKind[] kinds;
        private readonly List<SymbolAnalysisContext> contexts = new();

        internal SymbolAnalyzer(params SymbolKind[] kinds)
        {
            this.kinds = kinds;
        }

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get; } = ImmutableArray.Create(Descriptor);

        internal IReadOnlyList<SymbolAnalysisContext> Contexts => this.contexts;

        /// <inheritdoc/>
        public override void Initialize(AnalysisContext context)
        {
            context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
            context.EnableConcurrentExecution();
            context.RegisterSymbolAction(this.Handle, this.kinds);
        }

        private void Handle(SymbolAnalysisContext context)
        {
            this.contexts.Add(context);
        }
    }
}
