// ReSharper disable PossibleMultipleEnumeration
namespace Gu.Roslyn.Asserts
{
    using System;
    using System.Collections.Generic;
    using System.Collections.Immutable;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;
    using System.Threading.Tasks;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.Diagnostics;

    public static partial class AnalyzerAssert
    {
        /// <summary>
        /// Verifies that <paramref name="codeWithErrorsIndicated"/> produces the expected diagnostics.
        /// </summary>
        /// <typeparam name="TAnalyzer">The type of the analyzer.</typeparam>
        /// <param name="codeWithErrorsIndicated">The code with error positions indicated.</param>
        public static void Diagnostics<TAnalyzer>(params string[] codeWithErrorsIndicated)
            where TAnalyzer : DiagnosticAnalyzer, new()
        {
            Diagnostics(new TAnalyzer(), codeWithErrorsIndicated);
        }

        /// <summary>
        /// Verifies that <paramref name="codeWithErrorsIndicated"/> produces the expected diagnostics.
        /// </summary>
        /// <param name="analyzerType">The type of the analyzer.</param>
        /// <param name="codeWithErrorsIndicated">The code with error positions indicated.</param>
        public static void Diagnostics(Type analyzerType, params string[] codeWithErrorsIndicated)
        {
            Diagnostics((DiagnosticAnalyzer)Activator.CreateInstance(analyzerType), codeWithErrorsIndicated);
        }

        /// <summary>
        /// Verifies that <paramref name="codeWithErrorsIndicated"/> produces the expected diagnostics.
        /// </summary>
        /// <param name="analyzer">The analyzer to apply.</param>
        /// <param name="codeWithErrorsIndicated">The code with error positions indicated.</param>
        public static void Diagnostics(DiagnosticAnalyzer analyzer, params string[] codeWithErrorsIndicated)
        {
            Diagnostics(analyzer, (IEnumerable<string>)codeWithErrorsIndicated);
        }

        /// <summary>
        /// Verifies that <paramref name="codeWithErrorsIndicated"/> produces the expected diagnostics.
        /// </summary>
        /// <param name="analyzer">The analyzer to apply.</param>
        /// <param name="codeWithErrorsIndicated">The code with error positions indicated.</param>
        public static void Diagnostics(DiagnosticAnalyzer analyzer, IEnumerable<string> codeWithErrorsIndicated)
        {
            try
            {
                DiagnosticsAsync(analyzer, codeWithErrorsIndicated).Wait();
            }
            catch (AggregateException e)
            {
                Fail.WithMessage(e.InnerExceptions[0].Message);
            }
        }

        /// <summary>
        /// Verifies that <paramref name="codeWithErrorsIndicated"/> produces the expected diagnostics.
        /// </summary>
        /// <param name="analyzer">The analyzer to apply.</param>
        /// <param name="codeWithErrorsIndicated">The code with error positions indicated.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        public static Task DiagnosticsAsync(DiagnosticAnalyzer analyzer, IEnumerable<string> codeWithErrorsIndicated)
        {
            return DiagnosticsWithMetaDataAsync(analyzer, codeWithErrorsIndicated, References);
        }

        /// <summary>
        /// Verifies that <paramref name="codeWithErrorsIndicated"/> produces the expected diagnostics.
        /// </summary>
        /// <param name="analyzer">The analyzer to apply.</param>
        /// <param name="codeWithErrorsIndicated">The code with error positions indicated.</param>
        /// <param name="references">The meta data references to use when compiling.</param>
        /// <returns>The meta data from the run..</returns>
        public static async Task<DiagnosticMetaData> DiagnosticsWithMetaDataAsync(DiagnosticAnalyzer analyzer, IEnumerable<string> codeWithErrorsIndicated, IEnumerable<MetadataReference> references)
        {
            var result = ExpectedDiagnostic.FromCode(analyzer, codeWithErrorsIndicated);
            var expecteds = result.ExpectedDiagnostics;
            if (expecteds.Count == 0)
            {
                Fail.WithMessage("Expected code to have at least one error position indicated with '↓'");
            }

            var data = await CodeFactory.GetDiagnosticsWithMetaDataAsync(analyzer, result.CleanedSources, references)
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

            return new DiagnosticMetaData(codeWithErrorsIndicated, result.CleanedSources, expecteds, data.Diagnostics, data.Solution);
        }

        /// <summary>
        /// Meta data from a call to GetAnalyzerDiagnosticsAsync
        /// </summary>
        [SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Global", Justification = "For debugging.")]
        public class DiagnosticMetaData
        {
            /// <summary>
            /// Initializes a new instance of the <see cref="DiagnosticMetaData"/> class.
            /// </summary>
            /// <param name="codeWithErrorsIndicated">The code with errors indicated</param>
            /// <param name="sources"><paramref name="codeWithErrorsIndicated"/> cleaned from indicators.</param>
            /// <param name="expectedDiagnostics">Info about the expected diagnostics.</param>
            /// <param name="actualDiagnostics">The diagnostics returned from Roslyn</param>
            /// <param name="solution">The solution the analysis was run on.</param>
            public DiagnosticMetaData(
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

            /// <summary>
            /// Gets the code with errors indicated
            /// </summary>
            public IEnumerable<string> CodeWithErrorsIndicated { get; }

            /// <summary>
            /// Gets the code that was analyzed. This is <see cref="CodeWithErrorsIndicated"/> with indicators stripped.
            /// </summary>
            public IReadOnlyList<string> Sources { get; }

            /// <summary>
            /// Gets the meta data about the expected diagnostics.
            /// </summary>
            public IReadOnlyList<ExpectedDiagnostic> ExpectedDiagnostics { get; }

            /// <summary>
            /// Gets the actual diagnostics returned from Roslyn.
            /// </summary>
            public IReadOnlyList<ImmutableArray<Diagnostic>> ActualDiagnostics { get; }

            /// <summary>
            /// Gets the solution the analysis was performed on.
            /// </summary>
            public Solution Solution { get; }
        }
    }
}