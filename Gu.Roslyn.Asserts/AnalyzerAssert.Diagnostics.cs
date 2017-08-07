namespace Gu.Roslyn.Asserts
{
    using System;
    using System.Collections.Generic;
    using System.Collections.Immutable;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;
    using System.Threading.Tasks;
    using Gu.Roslyn.Asserts.Internals;
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
            Diagnostics(new TAnalyzer(), null, codeWithErrorsIndicated);
        }

        /// <summary>
        /// Verifies that <paramref name="codeWithErrorsIndicated"/> produces the expected diagnostics.
        /// </summary>
        /// <typeparam name="TAnalyzer">The type of the analyzer.</typeparam>
        /// <param name="expectedMessage">The expected message in the diagnostic produced by the analyzer.</param>
        /// <param name="codeWithErrorsIndicated">The code with error positions indicated.</param>
        public static void Diagnostics<TAnalyzer>(ExpectedMessage expectedMessage, params string[] codeWithErrorsIndicated)
            where TAnalyzer : DiagnosticAnalyzer, new()
        {
            Diagnostics(new TAnalyzer(), expectedMessage, codeWithErrorsIndicated);
        }

        /// <summary>
        /// Verifies that <paramref name="codeWithErrorsIndicated"/> produces the expected diagnostics.
        /// </summary>
        /// <param name="analyzerType">The type of the analyzer.</param>
        /// <param name="codeWithErrorsIndicated">The code with error positions indicated.</param>
        public static void Diagnostics(Type analyzerType, params string[] codeWithErrorsIndicated)
        {
            Diagnostics((DiagnosticAnalyzer)Activator.CreateInstance(analyzerType), null, codeWithErrorsIndicated);
        }

        /// <summary>
        /// Verifies that <paramref name="codeWithErrorsIndicated"/> produces the expected diagnostics.
        /// </summary>
        /// <param name="analyzerType">The type of the analyzer.</param>
        /// <param name="expectedMessage">The expected message in the diagnostic produced by the analyzer.</param>
        /// <param name="codeWithErrorsIndicated">The code with error positions indicated.</param>
        public static void Diagnostics(Type analyzerType, ExpectedMessage expectedMessage, params string[] codeWithErrorsIndicated)
        {
            Diagnostics((DiagnosticAnalyzer)Activator.CreateInstance(analyzerType), expectedMessage, codeWithErrorsIndicated);
        }

        /// <summary>
        /// Verifies that <paramref name="codeWithErrorsIndicated"/> produces the expected diagnostics.
        /// </summary>
        /// <param name="analyzer">The analyzer to apply.</param>
        /// <param name="codeWithErrorsIndicated">The code with error positions indicated.</param>
        public static void Diagnostics(DiagnosticAnalyzer analyzer, params string[] codeWithErrorsIndicated)
        {
            Diagnostics(analyzer, codeWithErrorsIndicated, null);
        }

        /// <summary>
        /// Verifies that <paramref name="codeWithErrorsIndicated"/> produces the expected diagnostics.
        /// </summary>
        /// <param name="analyzer">The analyzer to apply.</param>
        /// <param name="expectedMessage">The expected message in the diagnostic produced by the analyzer.</param>
        /// <param name="codeWithErrorsIndicated">The code with error positions indicated.</param>
        public static void Diagnostics(DiagnosticAnalyzer analyzer, ExpectedMessage expectedMessage, params string[] codeWithErrorsIndicated)
        {
            Diagnostics(analyzer, codeWithErrorsIndicated, expectedMessage);
        }

        /// <summary>
        /// Verifies that <paramref name="codeWithErrorsIndicated"/> produces the expected diagnostics.
        /// </summary>
        /// <param name="analyzer">The analyzer to apply.</param>
        /// <param name="codeWithErrorsIndicated">The code with error positions indicated.</param>
        /// <param name="expectedMessage">The expected message in the diagnostic produced by the analyzer.</param>
        public static void Diagnostics(DiagnosticAnalyzer analyzer, IReadOnlyList<string> codeWithErrorsIndicated, ExpectedMessage expectedMessage = null)
        {
            DiagnosticsAsync(analyzer, codeWithErrorsIndicated, expectedMessage).GetAwaiter().GetResult();
        }

        /// <summary>
        /// Verifies that <paramref name="codeWithErrorsIndicated"/> produces the expected diagnostics.
        /// </summary>
        /// <param name="analyzer">The analyzer to apply.</param>
        /// <param name="codeWithErrorsIndicated">The code with error positions indicated.</param>
        /// <param name="expectedMessage">The expected message in the diagnostic produced by the analyzer.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        public static Task DiagnosticsAsync(DiagnosticAnalyzer analyzer, IReadOnlyList<string> codeWithErrorsIndicated, ExpectedMessage expectedMessage = null)
        {
            return DiagnosticsWithMetaDataAsync(analyzer, codeWithErrorsIndicated, MetadataReference, expectedMessage);
        }

        /// <summary>
        /// Verifies that <paramref name="codeWithErrorsIndicated"/> produces the expected diagnostics.
        /// </summary>
        /// <param name="analyzer">The analyzer to apply.</param>
        /// <param name="codeWithErrorsIndicated">The code with error positions indicated.</param>
        /// <param name="metadataReferences">The meta data metadataReferences to use when compiling.</param>
        /// <param name="expectedMessage">The expected message in the diagnostic produced by the analyzer.</param>
        /// <returns>The meta data from the run..</returns>
        public static async Task<DiagnosticsMetaData> DiagnosticsWithMetaDataAsync(DiagnosticAnalyzer analyzer, IReadOnlyList<string> codeWithErrorsIndicated, IReadOnlyList<MetadataReference> metadataReferences, ExpectedMessage expectedMessage = null)
        {
            var expectedDiagnosticsAndSources = ExpectedDiagnostic.FromCode(analyzer, codeWithErrorsIndicated);
            if (expectedDiagnosticsAndSources.ExpectedDiagnostics.Count == 0)
            {
                throw Fail.CreateException("Expected code to have at least one error position indicated with '↓'");
            }

            var data = await Analyze.GetDiagnosticsWithMetaDataAsync(analyzer, expectedDiagnosticsAndSources.CleanedSources, metadataReferences)
                                    .ConfigureAwait(false);

            var expecteds = new HashSet<IdAndPosition>(expectedDiagnosticsAndSources.ExpectedDiagnostics.Select(IdAndPosition.Create));

            var actuals = data.Diagnostics
                              .SelectMany(x => x)
                              .Select(IdAndPosition.Create)
                              .ToArray();

            if (expecteds.SetEquals(actuals))
            {
                if (expectedMessage != null)
                {
                    foreach (var actual in data.Diagnostics.SelectMany(x => x))
                    {
                        expectedMessage.AssertIsMatch(actual);
                    }
                }

                return new DiagnosticsMetaData(codeWithErrorsIndicated, expectedDiagnosticsAndSources.CleanedSources, expectedDiagnosticsAndSources.ExpectedDiagnostics, data.Diagnostics, data.Solution);
            }

            var error = StringBuilderPool.Borrow();
            error.AppendLine("Expected and actual diagnostics do not match.");
            var unMatchedExpecteds = expecteds.Except(actuals).ToArray();
            for (var i = 0; i < unMatchedExpecteds.Length; i++)
            {
                error.Append(i == 0 ? "Expected: " : "            ");
                var expected = unMatchedExpecteds[i];
                error.AppendLine(expected.ToString(expectedDiagnosticsAndSources.CleanedSources));
            }

            if (actuals.Length == 0)
            {
                error.AppendLine("Actual:   <no errors>");
            }

            var unmatchedActuals = actuals.Except(expecteds).ToArray();
            if (actuals.Length > 0 && unmatchedActuals.Length == 0)
            {
                error.AppendLine("Actual:   <missing>");
            }

            for (var i = 0; i < unmatchedActuals.Length; i++)
            {
                error.Append(i == 0 ? "Actual:   " : "            ");
                var actual = unmatchedActuals[i];
                error.AppendLine(actual.ToString(expectedDiagnosticsAndSources.CleanedSources));
            }

            throw Fail.CreateException(StringBuilderPool.ReturnAndGetText(error));
        }

        /// <summary>
        /// Meta data from a call to GetAnalyzerDiagnosticsAsync
        /// </summary>
        [SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Global", Justification = "For debugging.")]
        public class DiagnosticsMetaData
        {
            /// <summary>
            /// Initializes a new instance of the <see cref="DiagnosticsMetaData"/> class.
            /// </summary>
            /// <param name="codeWithErrorsIndicated">The code with errors indicated</param>
            /// <param name="sources"><paramref name="codeWithErrorsIndicated"/> cleaned from indicators.</param>
            /// <param name="expectedDiagnostics">Info about the expected diagnostics.</param>
            /// <param name="actualDiagnostics">The diagnostics returned from Roslyn</param>
            /// <param name="solution">The solution the analysis was run on.</param>
            public DiagnosticsMetaData(
                IReadOnlyList<string> codeWithErrorsIndicated,
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
            public IReadOnlyList<string> CodeWithErrorsIndicated { get; }

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