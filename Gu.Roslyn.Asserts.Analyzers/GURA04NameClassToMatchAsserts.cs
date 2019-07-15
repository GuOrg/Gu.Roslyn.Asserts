namespace Gu.Roslyn.Asserts.Analyzers
{
    using Microsoft.CodeAnalysis;

    public static class GURA04NameClassToMatchAsserts
    {
        public const string DiagnosticId = "GURA04";

        public static readonly DiagnosticDescriptor Descriptor = new DiagnosticDescriptor(
            id: DiagnosticId,
            title: "Name of class should match asserts.",
            messageFormat: "Name of '{0}' should be '{1}'.",
            category: AnalyzerCategory.Ocd,
            defaultSeverity: DiagnosticSeverity.Hidden,
            isEnabledByDefault: true,
            description: "Name of class should match asserts.");
    }
}
