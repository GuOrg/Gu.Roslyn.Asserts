namespace Gu.Roslyn.Asserts.Tests
{
    using Microsoft.CodeAnalysis;

    // ReSharper disable once InconsistentNaming
    public static class Descriptors
    {
        public static readonly DiagnosticDescriptor Id1 = new DiagnosticDescriptor(
            "1",
            "Title: 1",
            "Message: 1",
            "Description: 1",
            DiagnosticSeverity.Warning,
            isEnabledByDefault: true);

        public static readonly DiagnosticDescriptor Id1Duplicate = new DiagnosticDescriptor(
            "1",
            "Title: 1 (dupe)",
            "Message: 1 (dupe)",
            "Description: 1 (dupe)",
            DiagnosticSeverity.Warning,
            isEnabledByDefault: true);

        public static readonly DiagnosticDescriptor Id2 = new DiagnosticDescriptor(
            "2",
            "Title: 2",
            "Message: 2",
            "Description: 2",
            DiagnosticSeverity.Warning,
            isEnabledByDefault: true);

        public static readonly DiagnosticDescriptor IdWithNoFix = new DiagnosticDescriptor(
            "IdWithNoFix",
            "Title: IdWithNoFix",
            "Message: IdWithNoFix",
            "Description: IdWithNoFix",
            DiagnosticSeverity.Warning,
            isEnabledByDefault: true);
    }
}
