namespace Gu.Roslyn.Asserts.Analyzers
{
    using Microsoft.CodeAnalysis;

    public static class GURA07TestClassShouldBePublicStatic
    {
        public const string DiagnosticId = "GURA07";

        public static readonly DiagnosticDescriptor Descriptor = new DiagnosticDescriptor(
            id: DiagnosticId,
            title: "Test class should be public static.",
            messageFormat: "'{0}' should be public static.",
            category: AnalyzerCategory.Ocd,
            defaultSeverity: DiagnosticSeverity.Hidden,
            isEnabledByDefault: true,
            description: "Test class should be public static.");
    }
}
