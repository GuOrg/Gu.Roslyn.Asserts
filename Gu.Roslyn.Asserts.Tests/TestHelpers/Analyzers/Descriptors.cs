namespace Gu.Roslyn.Asserts.Tests
{
    using Microsoft.CodeAnalysis;

    // ReSharper disable once InconsistentNaming
    public static class Descriptors
    {
        public static readonly DiagnosticDescriptor Id1234 = new DiagnosticDescriptor(
            "ID1234",
            "Title: ID1234",
            "Message: ID1234",
            "Description: ID1234",
            DiagnosticSeverity.Warning,
            isEnabledByDefault: true);

        public static readonly DiagnosticDescriptor Id1234Duplicate = new DiagnosticDescriptor(
            "ID1234",
            "Title: ID1234 (dupe)",
            "Message: ID1234 (dupe)",
            "Description: ID1234 (dupe)",
            DiagnosticSeverity.Warning,
            isEnabledByDefault: true);
    }
}
