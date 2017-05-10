// ReSharper disable PossibleMultipleEnumeration
namespace Gu.Roslyn.Asserts
{
    using System;
    using System.Collections.Generic;
    using System.Collections.Immutable;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;
    using System.Text;
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
                throw Fail.CreateException(e.InnerExceptions[0].Message);
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
            return DiagnosticsWithMetaDataAsync(analyzer, codeWithErrorsIndicated, MetadataReference);
        }

        /// <summary>
        /// Verifies that <paramref name="codeWithErrorsIndicated"/> produces the expected diagnostics.
        /// </summary>
        /// <param name="analyzer">The analyzer to apply.</param>
        /// <param name="codeWithErrorsIndicated">The code with error positions indicated.</param>
        /// <param name="references">The meta data references to use when compiling.</param>
        /// <returns>The meta data from the run..</returns>
        public static async Task<DiagnosticsMetaData> DiagnosticsWithMetaDataAsync(DiagnosticAnalyzer analyzer, IEnumerable<string> codeWithErrorsIndicated, IEnumerable<MetadataReference> references)
        {
            var expectedDiagnosticsAndSources = ExpectedDiagnostic.FromCode(analyzer, codeWithErrorsIndicated);
            if (expectedDiagnosticsAndSources.ExpectedDiagnostics.Count == 0)
            {
                throw Fail.CreateException("Expected code to have at least one error position indicated with '↓'");
            }

            var data = await Analyze.GetDiagnosticsWithMetaDataAsync(analyzer, expectedDiagnosticsAndSources.CleanedSources, references)
                                    .ConfigureAwait(false);

            var expecteds = new HashSet<IdAndPosition>(expectedDiagnosticsAndSources.ExpectedDiagnostics.Select(x => new IdAndPosition(x.Analyzer.SupportedDiagnostics[0].Id, x.Span)));

            var actuals = data.Diagnostics
                              .SelectMany(x => x)
                              .Select(x => new IdAndPosition(x.Id, x.Location.GetMappedLineSpan()))
                              .ToArray();

            if (expecteds.SetEquals(actuals))
            {
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

            var unmatchedActuals = actuals.Except(expecteds).ToArray();
            for (var i = 0; i < unmatchedActuals.Length; i++)
            {
                error.Append(i == 0 ? "Actual:   " : "            ");
                var actual = unmatchedActuals[i];
                error.AppendLine(actual.ToString(expectedDiagnosticsAndSources.CleanedSources));
            }

            throw Fail.CreateException(StringBuilderPool.ReturnAndGetText(error));
        }

        private struct IdAndPosition : IEquatable<IdAndPosition>
        {
            public IdAndPosition(string id, FileLinePositionSpan span)
            {
                this.Id = id;
                this.Span = span;
            }

            public string Id { get; }

            public FileLinePositionSpan Span { get; }

            public static bool operator ==(IdAndPosition left, IdAndPosition right)
            {
                return left.Equals(right);
            }

            public static bool operator !=(IdAndPosition left, IdAndPosition right)
            {
                return !left.Equals(right);
            }

            public bool Equals(IdAndPosition other)
            {
                bool EndPositionsEquals(FileLinePositionSpan x, FileLinePositionSpan y)
                {
                    if (x.StartLinePosition == x.EndLinePosition ||
                        y.StartLinePosition == y.EndLinePosition)
                    {
                        return true;
                    }

                    return x.EndLinePosition == y.EndLinePosition;
                }

                return string.Equals(this.Id, other.Id) &&
                       string.Equals(this.Span.Path, other.Span.Path) &&
                       this.Span.StartLinePosition == other.Span.StartLinePosition &&
                       EndPositionsEquals(this.Span, other.Span);
            }

            public override bool Equals(object obj)
            {
                if (ReferenceEquals(null, obj))
                {
                    return false;
                }

                return obj is IdAndPosition && this.Equals((IdAndPosition)obj);
            }

            public override int GetHashCode()
            {
                unchecked
                {
                    return (this.Id.GetHashCode() * 397) ^
                           this.Span.Path.GetHashCode() ^
                           this.Span.StartLinePosition.GetHashCode();
                }
            }

            internal string ToString(IReadOnlyList<string> sources)
            {
                var path = this.Span.Path;
                var match = sources.SingleOrDefault(x => CodeReader.FileName(x) == path);
                var line = match != null ? CodeReader.GetLineWithErrorIndicated(match, this.Span.StartLinePosition) : string.Empty;
                return $"{this.Id} at line {this.Span.StartLinePosition.Line} and character {this.Span.StartLinePosition.Character} in file {path} |{line}";
            }
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