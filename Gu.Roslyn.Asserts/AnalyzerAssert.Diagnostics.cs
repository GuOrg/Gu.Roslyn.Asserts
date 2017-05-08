namespace Gu.Roslyn.Asserts
{
    using System;
    using System.Collections.Generic;
    using System.Collections.Immutable;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.Diagnostics;

    public partial class AnalyzerAssert
    {
        public static void Diagnostics(DiagnosticAnalyzer analyzer, IEnumerable<string> code)
        {
            try
            {
                DiagnosticsAsync(analyzer, code).Wait();
            }
            catch (AggregateException e)
            {
                Fail.WithMessage(e.InnerExceptions[0].Message);
            }
        }

        public static async Task DiagnosticsAsync(DiagnosticAnalyzer analyzer, IEnumerable<string> code)
        {
            (IReadOnlyList<ExpectedDiagnostic> expecteds, IEnumerable<string> sources) = ExpectedDiagnostic.FromCode(analyzer,code);
            if (expecteds.Count == 0)
            {
                Fail.WithMessage("Expected code to have at least one error position indicated with '↓'");
            }

            var sln = CodeFactory.CreateSolution(sources, References);
            var results = new List<ImmutableArray<Diagnostic>>();
            foreach (var project in sln.Projects)
            {
                var compilation = await project.GetCompilationAsync(CancellationToken.None)
                    .ConfigureAwait(false);

                var withAnalyzers = compilation.WithAnalyzers(
                    ImmutableArray.Create(analyzer),
                    project.AnalyzerOptions,
                    CancellationToken.None);
                results.Add(await withAnalyzers.GetAnalyzerDiagnosticsAsync(CancellationToken.None)
                    .ConfigureAwait(false));
            }

            var actuals = results.SelectMany(x => x).ToArray();
            if (expecteds.Count != actuals.Length)
            {
                Fail.WithMessage($"Expected count does not match actual.{Environment.NewLine}" +
                                 $"Expected: {expecteds.Count}{Environment.NewLine}" +
                                 $"Actual:   {actuals.Length}");
            }

            for (var i = 0; i < expecteds.Count; i++)
            {
                var expected = expecteds[i];
                var actual = actuals[i];
                if (expected.Analyzer.SupportedDiagnostics[0].Id != actual.Id)
                {
                    Fail.WithMessage($"Expected id does not match actual.{Environment.NewLine}" +
                                     $"Expected: {expected.Analyzer.SupportedDiagnostics[0].Id}{Environment.NewLine}" +
                                     $"Actual:   {actual.Id}");
                }

                var actualSpan = actual.Location.GetMappedLineSpan();
                if (expected.Span.Path != actualSpan.Path)
                {
                    Fail.WithMessage($"Expected id does not match actual.{Environment.NewLine}" +
                                     $"Expected: {expected.Span.Path}" +
                                     $"Actual:   {actual.Location.SourceTree.FilePath}");
                }

                if (expected.Span.StartLinePosition != actualSpan.StartLinePosition)
                {
                    Fail.WithMessage($"Expected id does not match actual.{Environment.NewLine}" +
                                     $"Expected: {expected.Span.Path}" +
                                     $"Actual:   {actual.Location.SourceTree.FilePath}");
                }
            }
        }
    }
}