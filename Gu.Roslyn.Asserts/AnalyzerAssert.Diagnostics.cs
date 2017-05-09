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

    public static partial class AnalyzerAssert
    {
        public static void Diagnostics<TAnalyzer>(params string[] code)
            where TAnalyzer : DiagnosticAnalyzer, new()
        {
            Diagnostics(new TAnalyzer(), code);
        }

        public static void Diagnostics(Type analyzerType, params string[] code)
        {
            Diagnostics((DiagnosticAnalyzer)Activator.CreateInstance(analyzerType), code);
        }

        public static void Diagnostics(DiagnosticAnalyzer analyzer, params string[] code)
        {
            Diagnostics(analyzer, (IEnumerable<string>)code);
        }

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

        public static Task DiagnosticsAsync(DiagnosticAnalyzer analyzer, IEnumerable<string> codeWithErrorsIndicated)
        {
            return DiagnosticsWithMetaDataAsync(analyzer, codeWithErrorsIndicated, References);
        }

        public static async Task<DaignosticMetaData> DiagnosticsWithMetaDataAsync(DiagnosticAnalyzer analyzer, IEnumerable<string> codeWithErrorsIndicated, IEnumerable<MetadataReference> references)
        {
            (var expecteds, var sources) = ExpectedDiagnostic.FromCode(analyzer, codeWithErrorsIndicated);

            if (expecteds.Count == 0)
            {
                Fail.WithMessage("Expected code to have at least one error position indicated with '↓'");
            }

            var data = await CodeFactory.GetDiagnosticsWithMetaDataAsync(analyzer, sources, references)
                                               .ConfigureAwait(false);

            var actuals = data.Diagnostics
                              .SelectMany(x => x)
                              .OrderBy(d => d.Id)
                              .ThenBy(d => d.Location.GetMappedLineSpan(), FileLinePositionSpanComparer.Default)
                              .ToArray();
            if (expecteds.Count != actuals.Length)
            {
                Fail.WithMessage($"Expected count does not match actual.{Environment.NewLine}" +
                                 $"Expected: {expecteds.Count}{Environment.NewLine}" +
                                 $"Actual:   {actuals.Length}");
            }

            expecteds = expecteds.OrderBy(d => d.Analyzer.SupportedDiagnostics[0].Id)
                                 .ThenBy(d => d.Span, FileLinePositionSpanComparer.Default)
                                 .ToArray();
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

            return new DaignosticMetaData(codeWithErrorsIndicated, sources, expecteds, data.Diagnostics, data.Solution);
        }

        public class DaignosticMetaData
        {
            public DaignosticMetaData(
                IEnumerable<string> codeWithErrorsIndicated,
                IReadOnlyList<string> sources,
                IReadOnlyList<ExpectedDiagnostic> expectedDiagnostics,
                IReadOnlyList<ImmutableArray<Diagnostic>> actualDiagnostics,
                Solution solution)
            {
                this.CodeWithErrorsIndicated = codeWithErrorsIndicated;
                this.Sources = sources;
                this.ExpectedDiagnostics = expectedDiagnostics;
                this.ActualDiagnostics = actualDiagnostics;
                this.Solution = solution;
            }

            public IEnumerable<string> CodeWithErrorsIndicated { get; }

            public IReadOnlyList<string> Sources { get; }

            public IReadOnlyList<ExpectedDiagnostic> ExpectedDiagnostics { get; }

            public IReadOnlyList<ImmutableArray<Diagnostic>> ActualDiagnostics { get; }

            public Solution Solution { get; }
        }
    }
}