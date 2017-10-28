namespace Gu.Roslyn.Asserts.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Collections.Immutable;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using Microsoft.CodeAnalysis.Diagnostics;

    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    internal class SyntaxNodeAnalyzer : DiagnosticAnalyzer
    {
        public const string DiagnosticId = "123";
        private readonly SyntaxKind[] syntaxKinds;
        private readonly List<SyntaxNodeAnalysisContext> contexts = new List<SyntaxNodeAnalysisContext>();

        private static readonly DiagnosticDescriptor Descriptor = new DiagnosticDescriptor(
            DiagnosticId,
            "SyntaxNodeAnalyzer",
            "SyntaxNodeAnalyzer",
            "SyntaxNodeAnalyzer",
            DiagnosticSeverity.Warning,
            isEnabledByDefault: true);

        public SyntaxNodeAnalyzer(params SyntaxKind[] syntaxKinds)
        {
            this.syntaxKinds = syntaxKinds;
        }

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get; } = ImmutableArray.Create(Descriptor);

        public IReadOnlyList<SyntaxNodeAnalysisContext> Contexts => this.contexts;

        /// <inheritdoc/>
        public override void Initialize(AnalysisContext context)
        {
            context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
            context.EnableConcurrentExecution();
            context.RegisterSyntaxNodeAction(this.Handle, this.syntaxKinds);
        }

        private void Handle(SyntaxNodeAnalysisContext context)
        {
           this.contexts.Add(context);
        }
    }
}