namespace Gu.Roslyn.Asserts.Analyzers
{
    using Microsoft.CodeAnalysis;

    internal static class Descriptors
    {
        internal static readonly DiagnosticDescriptor GURA01NameShouldMatchParameter = new DiagnosticDescriptor(
            id: "GURA01",
            title: "Name of local should match parameter.",
            messageFormat: "Name of '{0}' should be '{1}'.",
            category: AnalyzerCategory.Ocd,
            defaultSeverity: DiagnosticSeverity.Hidden,
            isEnabledByDefault: false,
            description: "Name of local should match parameter for max consistency.");

        internal static readonly DiagnosticDescriptor GURA02IndicateErrorPosition = new DiagnosticDescriptor(
            id: "GURA02",
            title: "Indicate position.",
            messageFormat: "Indicate position with ↓ (alt + 25).",
            category: AnalyzerCategory.Correctness,
            defaultSeverity: DiagnosticSeverity.Warning,
            isEnabledByDefault: true,
            description: "Indicate position with ↓ (alt + 25).");

        internal static readonly DiagnosticDescriptor GURA03NameShouldMatchCode = new DiagnosticDescriptor(
            id: "GURA03",
            title: "Name should match code.",
            messageFormat: "Name of '{0}' should be '{1}'.",
            category: AnalyzerCategory.Ocd,
            defaultSeverity: DiagnosticSeverity.Hidden,
            isEnabledByDefault: false,
            description: "Name should match code.");

        internal static readonly DiagnosticDescriptor GURA04NameClassToMatchAsserts = new DiagnosticDescriptor(
            id: "GURA04",
            title: "Name of class should match asserts.",
            messageFormat: "Name of '{0}' should be '{1}'.",
            category: AnalyzerCategory.Ocd,
            defaultSeverity: DiagnosticSeverity.Hidden,
            isEnabledByDefault: false,
            description: "Name of class should match asserts.");

        internal static readonly DiagnosticDescriptor GURA05NameFileToMatchClass = new DiagnosticDescriptor(
            id: "GURA05",
            title: "Name file to match class.",
            messageFormat: "Name of '{0}' should be '{1}'.",
            category: AnalyzerCategory.Ocd,
            defaultSeverity: DiagnosticSeverity.Hidden,
            isEnabledByDefault: false,
            description: "Name file to match class.");

        internal static readonly DiagnosticDescriptor GURA06TestShouldBeInCorrectClass = new DiagnosticDescriptor(
            id: "GURA06",
            title: "Move test to correct class.",
            messageFormat: "Move to '{0}'.",
            category: AnalyzerCategory.Ocd,
            defaultSeverity: DiagnosticSeverity.Hidden,
            isEnabledByDefault: false,
            description: "Move test to correct class.");

        internal static readonly DiagnosticDescriptor GURA07TestClassShouldBePublicStatic = new DiagnosticDescriptor(
            id: "GURA07",
            title: "Test class should be internal static.",
            messageFormat: "'{0}' should be internal static.",
            category: AnalyzerCategory.Ocd,
            defaultSeverity: DiagnosticSeverity.Hidden,
            isEnabledByDefault: false,
            description: "Test class should be internal static.");

        internal static readonly DiagnosticDescriptor GURA08aShouldBeInternal = new DiagnosticDescriptor(
            id: "GURA08a",
            title: "Should be internal.",
            messageFormat: "'{0}' should be internal.",
            category: AnalyzerCategory.Ocd,
            defaultSeverity: DiagnosticSeverity.Info,
            isEnabledByDefault: false,
            description: "Should be internal.");

        internal static readonly DiagnosticDescriptor GURA08bShouldBePublic = new DiagnosticDescriptor(
            id: "GURA08b",
            title: "Should be internal.",
            messageFormat: "'{0}' should be internal.",
            category: AnalyzerCategory.Ocd,
            defaultSeverity: DiagnosticSeverity.Hidden,
            isEnabledByDefault: false,
            description: "Should be internal.");

        internal static readonly DiagnosticDescriptor GURA09UseStandardNames = new DiagnosticDescriptor(
            id: "GURA09",
            title: "Use standard names in test code.",
            messageFormat: "{0}",
            category: AnalyzerCategory.Ocd,
            defaultSeverity: DiagnosticSeverity.Hidden,
            isEnabledByDefault: false,
            description: "Use standard names for things that do not have particular meaning in test code.");

        internal static readonly DiagnosticDescriptor GURA10UseLocal = new DiagnosticDescriptor(
            id: "GURA10",
            title: "Move to local.",
            messageFormat: "Copy {0} to a local.",
            category: AnalyzerCategory.Ocd,
            defaultSeverity: DiagnosticSeverity.Hidden,
            isEnabledByDefault: false,
            description: "Move to local for self contained tests.");
    }
}
