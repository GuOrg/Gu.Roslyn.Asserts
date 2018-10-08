namespace Gu.Roslyn.Asserts.Tests
{
    using System.Collections.Immutable;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.Diagnostics;

    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    internal class DuplicateIdAnalyzer : DiagnosticAnalyzer
    {
        internal static readonly DiagnosticDescriptor Descriptor1 = new DiagnosticDescriptor(
            "0",
            string.Empty,
            string.Empty,
            string.Empty,
            DiagnosticSeverity.Warning,
            isEnabledByDefault: true);

        internal static readonly DiagnosticDescriptor Descriptor2 = new DiagnosticDescriptor(
            "0",
            string.Empty,
            string.Empty,
            string.Empty,
            DiagnosticSeverity.Warning,
            isEnabledByDefault: true);

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get; } = ImmutableArray.Create(
            Descriptor1,
            Descriptor2);

        public override void Initialize(AnalysisContext context)
        {
        }
    }
}
