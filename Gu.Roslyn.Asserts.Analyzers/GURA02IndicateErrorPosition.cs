namespace Gu.Roslyn.Asserts.Analyzers
{
    using Microsoft.CodeAnalysis;

    public static class GURA02IndicateErrorPosition
    {
        public const string DiagnosticId = "GURA02";

        public static readonly DiagnosticDescriptor Descriptor = new DiagnosticDescriptor(
            id: DiagnosticId,
            title: "Indicate error position.",
            messageFormat: "Indicate error position with ↓ (alt + 25).",
            category: AnalyzerCategory.Correctness,
            defaultSeverity: DiagnosticSeverity.Warning,
            isEnabledByDefault: true,
            description: "Indicate error position with ↓ (alt + 25).");
    }
}
