namespace Gu.Roslyn.Asserts
{
    using System.Collections.Immutable;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.Diagnostics;

    /// <summary>
    /// A dummy analyzer for indicating position of diagnostics produced by analyzers that are not in our code.
    /// </summary>
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class PlaceholderAnalyzer : DiagnosticAnalyzer
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PlaceholderAnalyzer"/> class.
        /// </summary>
        /// <param name="id">The diagnosticId of the expected diagnostics.</param>
        public PlaceholderAnalyzer(string id)
        {
            this.SupportedDiagnostics = ImmutableArray.Create(
                new DiagnosticDescriptor(
                    id: id,
                    title: "Placeholder",
                    messageFormat: "Placeholder",
                    category: "Placeholder",
                    defaultSeverity: DiagnosticSeverity.Warning,
                    isEnabledByDefault: true));
        }

        /// <inheritdoc />
        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get; }

        /// <inheritdoc />
        public override void Initialize(AnalysisContext context)
        {
            // nop
        }
    }
}
