namespace Gu.Roslyn.Asserts
{
    using Microsoft.CodeAnalysis.Diagnostics;

    public class ExpectedDiagnostic
    {
        public ExpectedDiagnostic(DiagnosticAnalyzer analyzer, string fileName, int spanStart)
        {
            this.Analyzer = analyzer;
            this.FileName = fileName;
            this.SpanStart = spanStart;
        }

        public DiagnosticAnalyzer Analyzer { get; }

        public string FileName { get; }

        public int SpanStart { get; }
    }
}
