namespace Gu.Roslyn.Asserts.Tests
{
    using System;
    using System.Collections.Immutable;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.Diagnostics;

    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    internal class ThrowingAnalyzer : DiagnosticAnalyzer
    {
        private readonly SyntaxKind[] kinds;

        internal ThrowingAnalyzer()
            : this(new[] { Descriptors.Id1 })
        {
        }

        internal ThrowingAnalyzer(params DiagnosticDescriptor[] descriptors)
            : this(descriptors, SyntaxKind.IdentifierName)
        {
        }

        internal ThrowingAnalyzer(DiagnosticDescriptor[] descriptors, params SyntaxKind[] kinds)
        {
            this.kinds = kinds;
            this.SupportedDiagnostics = descriptors.Length == 0 ? ImmutableArray.Create(Descriptors.Id1) : ImmutableArray.Create(descriptors);
        }

        /// <inheritdoc/>
        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get; }

        public override void Initialize(AnalysisContext context)
        {
            context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.Analyze | GeneratedCodeAnalysisFlags.ReportDiagnostics);
            context.EnableConcurrentExecution();
            context.RegisterSyntaxNodeAction(x => throw new InvalidOperationException("Analyzer threw this."), this.kinds);
        }
    }
}
