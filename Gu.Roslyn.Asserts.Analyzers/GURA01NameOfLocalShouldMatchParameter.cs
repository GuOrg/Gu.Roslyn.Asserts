namespace Gu.Roslyn.Asserts.Analyzers
{
    using Microsoft.CodeAnalysis;

    public static class GURA01NameOfLocalShouldMatchParameter
    {
        public const string DiagnosticId = "GURA01";

        public static readonly DiagnosticDescriptor Descriptor = new DiagnosticDescriptor(
            id: DiagnosticId,
            title: "Name of local should match parameter.",
            messageFormat: "Name of '{0}' should be '{1}'.",
            category: AnalyzerCategory.Ocd,
            defaultSeverity: DiagnosticSeverity.Hidden,
            isEnabledByDefault: true,
            description: "Name of local should match parameter for max consistency.");
    }
}
