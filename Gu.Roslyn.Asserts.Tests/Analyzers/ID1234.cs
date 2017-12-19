namespace Gu.Roslyn.Asserts.Tests
{
    using Microsoft.CodeAnalysis;

    // ReSharper disable once InconsistentNaming
    public static class ID1234
    {
        public static readonly DiagnosticDescriptor Descriptor = new DiagnosticDescriptor(
            "ID1234",
            "ID1234",
            "ID1234",
            "ID1234",
            DiagnosticSeverity.Warning,
            isEnabledByDefault: true);
    }
}