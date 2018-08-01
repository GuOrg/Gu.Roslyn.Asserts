namespace Gu.Roslyn.Asserts.Tests
{
    using System.Collections.Immutable;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.Diagnostics;

    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    internal class SupportedDiagnosticsAnalyzer : DiagnosticAnalyzer
    {
        public SupportedDiagnosticsAnalyzer(ImmutableArray<DiagnosticDescriptor> supportedDiagnostics)
        {
            this.SupportedDiagnostics = supportedDiagnostics;
        }

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get; }

        public override void Initialize(AnalysisContext context)
        {
        }
    }
}