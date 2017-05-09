namespace Gu.Roslyn.Asserts
{
    using System.Collections.Immutable;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.Diagnostics;

    /// <summary>
    /// 
    /// </summary>
    public class PlaceholderAnalyzer : DiagnosticAnalyzer
    {
        public PlaceholderAnalyzer(string id)
        {
            this.SupportedDiagnostics = ImmutableArray.Create(new DiagnosticDescriptor(
                id: id,
                title: "Placeholder",
                messageFormat: "Placeholder",
                category: "Placeholder",
                defaultSeverity: DiagnosticSeverity.Warning,
                isEnabledByDefault: true));
        }

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get; }

        public override void Initialize(AnalysisContext context)
        {
            // nop
        }
    }
}