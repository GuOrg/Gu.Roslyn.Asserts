namespace Gu.Roslyn.Asserts
{
    using System.Collections.Generic;
    using System.Linq;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.Diagnostics;

    public class ExpectedDiagnostic
    {
        public ExpectedDiagnostic(DiagnosticAnalyzer analyzer, FileLinePositionSpan span)
        {
            this.Analyzer = analyzer;
            this.Span = span;
        }

        public DiagnosticAnalyzer Analyzer { get; }

        public FileLinePositionSpan Span { get; }

        public static (IReadOnlyList<ExpectedDiagnostic>, IReadOnlyList<string>) FromCode(DiagnosticAnalyzer analyzer, IEnumerable<string> sources)
        {
            var diagnostics = new List<ExpectedDiagnostic>();
            var cleanedSources = new List<string>();
            foreach (var source in sources)
            {
                var positions = CodeReader.FindDiagnosticsPositions(source).ToArray();
                if (positions.Length == 0)
                {
                    cleanedSources.Add(source);
                    continue;
                }

                cleanedSources.Add(source.Replace("↓", string.Empty));
                var fileName = CodeReader.FileName(source);
                diagnostics.AddRange(positions.Select(p => new ExpectedDiagnostic(analyzer, new FileLinePositionSpan(fileName, p, p))));
            }

            return (diagnostics, cleanedSources);
        }
    }
}
