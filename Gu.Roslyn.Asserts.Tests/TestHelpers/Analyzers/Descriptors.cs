namespace Gu.Roslyn.Asserts.Tests
{
    using Microsoft.CodeAnalysis;

    // ReSharper disable once InconsistentNaming
    public static class Descriptors
    {
        public static readonly DiagnosticDescriptor Id1 = new DiagnosticDescriptor(
            "ID1",
            "Title: ID1",
            "Message: ID1",
            "Description: ID1",
            DiagnosticSeverity.Warning,
            isEnabledByDefault: true);

        public static readonly DiagnosticDescriptor Id1Duplicate = new DiagnosticDescriptor(
            "ID1",
            "Title: ID1 (dupe)",
            "Message: ID1 (dupe)",
            "Description: 1 (dupe)",
            DiagnosticSeverity.Warning,
            isEnabledByDefault: true);

        public static readonly DiagnosticDescriptor Id2 = new DiagnosticDescriptor(
            "ID2",
            "Title: ID2",
            "Message: ID2",
            "Description: ID2",
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
