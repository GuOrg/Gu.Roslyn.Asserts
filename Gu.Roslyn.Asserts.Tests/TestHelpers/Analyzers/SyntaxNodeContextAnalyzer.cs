namespace Gu.Roslyn.Asserts.Tests
{
    using System.Collections.Generic;
    using System.Collections.Immutable;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.Diagnostics;

    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    internal sealed class SyntaxNodeContextAnalyzer : DiagnosticAnalyzer
    {
        private static readonly DiagnosticDescriptor Descriptor = new(
            "12345",
            "SyntaxNodeAnalyzer",
            "SyntaxNodeAnalyzer",
            "SyntaxNodeAnalyzer",
            DiagnosticSeverity.Warning,
            isEnabledByDefault: true);

        private readonly SyntaxKind[] kinds;
        private readonly List<SyntaxNodeAnalysisContext> contexts = new();

        internal SyntaxNodeContextAnalyzer(params SyntaxKind[] kinds)
        {
            this.kinds = kinds;
        }

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get; } =
            ImmutableArray.Create(Descriptor);

        internal IReadOnlyList<SyntaxNodeAnalysisContext> Contexts => this.contexts;

        /// <inheritdoc/>
        public override void Initialize(AnalysisContext context)
        {
            context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
            context.EnableConcurrentExecution();
            context.RegisterSyntaxNodeAction(this.Handle, this.kinds);
        }

        private void Handle(SyntaxNodeAnalysisContext context)
        {
            this.contexts.Add(context);
        }
    }
}
