namespace Gu.Roslyn.Asserts.Analyzers
{
    using Microsoft.CodeAnalysis;

    public static class GURA06TestShouldBeInCorrectClass
    {
        public const string DiagnosticId = "GURA06";

        public static readonly DiagnosticDescriptor Descriptor = new DiagnosticDescriptor(
            id: DiagnosticId,
            title: "Move test to correct class.",
            messageFormat: "Move to '{0}'.",
            category: AnalyzerCategory.Ocd,
            defaultSeverity: DiagnosticSeverity.Hidden,
            isEnabledByDefault: true,
            description: "Move test to correct class.");
    }
}
