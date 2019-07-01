namespace Gu.Roslyn.Asserts.Tests.Net46WithAttributes.AnalyzersAndFixes
{
    using System.Collections.Immutable;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.Diagnostics;

    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    internal class NoErrorAnalyzer : DiagnosticAnalyzer
    {
        public const string DiagnosticId = "NoError";

        internal static readonly DiagnosticDescriptor Descriptor = new DiagnosticDescriptor(
            id: DiagnosticId,
            title: "This analyzer never reports an error.",
            messageFormat: "Message format.",
            category: "Category",
            defaultSeverity: DiagnosticSeverity.Warning,
            isEnabledByDefault: true);

        /// <inheritdoc/>
        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get; } = ImmutableArray.Create(Descriptor);

        public override void Initialize(AnalysisContext context)
        {
            if (context == null)
            {
                throw new System.ArgumentNullException(nameof(context));
            }

            context.EnableConcurrentExecution();
            context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.Analyze | GeneratedCodeAnalysisFlags.ReportDiagnostics);
            context.RegisterSyntaxNodeAction(HandleIdentifierName, SyntaxKind.IdentifierName);
        }

        private static void HandleIdentifierName(SyntaxNodeAnalysisContext context)
        {
        }
    }
}
