namespace Gu.Roslyn.Asserts
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Gu.Roslyn.Asserts.Internals;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CodeFixes;
    using Microsoft.CodeAnalysis.Diagnostics;

    /// <summary>
    /// The RoslynAssert class contains a collection of static methods used for assertions on the behavior of analyzers and code fixes.
    /// </summary>
    public static partial class RoslynAssert
    {
        /// <summary>
        /// Check that the <paramref name="analyzer"/> exports <paramref name="expectedDiagnostic"/>.
        /// </summary>
        /// <param name="analyzer">The <see cref="DiagnosticAnalyzer"/>.</param>
        /// <param name="expectedDiagnostic">The <see cref="ExpectedDiagnostic"/>.</param>
        public static void VerifyAnalyzerSupportsDiagnostic(DiagnosticAnalyzer analyzer, ExpectedDiagnostic expectedDiagnostic)
        {
            if (analyzer is null)
            {
                throw new ArgumentNullException(nameof(analyzer));
            }

            if (expectedDiagnostic is null)
            {
                throw new ArgumentNullException(nameof(expectedDiagnostic));
            }

            VerifyAnalyzerSupportsDiagnostic(analyzer, expectedDiagnostic.Id);
        }

        /// <summary>
        /// Check that the <paramref name="analyzer"/> exports <paramref name="descriptor"/>.
        /// </summary>
        /// <param name="analyzer">The <see cref="DiagnosticAnalyzer"/>.</param>
        /// <param name="descriptor">The <see cref="DiagnosticDescriptor"/>.</param>
        public static void VerifyAnalyzerSupportsDiagnostic(DiagnosticAnalyzer analyzer, DiagnosticDescriptor descriptor)
        {
            if (analyzer is null)
            {
                throw new ArgumentNullException(nameof(analyzer));
            }

            if (descriptor is null)
            {
                throw new ArgumentNullException(nameof(descriptor));
            }

            VerifyAnalyzerSupportsDiagnostic(analyzer, descriptor.Id);
        }

        /// <summary>
        /// Check that the <paramref name="analyzer"/> exports <paramref name="expectedDiagnostics"/>.
        /// </summary>
        /// <param name="analyzer">The <see cref="DiagnosticAnalyzer"/>.</param>
        /// <param name="expectedDiagnostics">The <see cref="IReadOnlyList{ExpectedDiagnostic}"/>.</param>
        internal static void VerifyAnalyzerSupportsDiagnostics(DiagnosticAnalyzer analyzer, IReadOnlyList<ExpectedDiagnostic> expectedDiagnostics)
        {
            foreach (var expectedDiagnostic in expectedDiagnostics)
            {
                VerifyAnalyzerSupportsDiagnostic(analyzer, expectedDiagnostic.Id);
            }
        }

        /// <summary>
        /// Check that the <paramref name="suppressor"/> supports <paramref name="expectedDiagnostics"/>.
        /// </summary>
        /// <param name="suppressor">The <see cref="DiagnosticSuppressor"/>.</param>
        /// <param name="expectedDiagnostics">The <see cref="IReadOnlyList{ExpectedDiagnostic}"/>.</param>
        internal static void VerifySuppressorSupportsDiagnostics(DiagnosticSuppressor suppressor, IReadOnlyList<ExpectedDiagnostic> expectedDiagnostics)
        {
            foreach (var expectedDiagnostic in expectedDiagnostics)
            {
                VerifySuppressorSupportsDiagnostic(suppressor, expectedDiagnostic.Id);
            }
        }

        /// <summary>
        /// Check that the <paramref name="analyzer"/> exports <paramref name="descriptors"/>.
        /// </summary>
        /// <param name="analyzer">The <see cref="DiagnosticAnalyzer"/>.</param>
        /// <param name="descriptors">The <see cref="IReadOnlyList{DiagnosticDescriptor}"/>.</param>
        internal static void VerifyAnalyzerSupportsDiagnostics(DiagnosticAnalyzer analyzer, IReadOnlyList<DiagnosticDescriptor> descriptors)
        {
            foreach (var expectedDiagnostic in descriptors)
            {
                VerifyAnalyzerSupportsDiagnostic(analyzer, expectedDiagnostic.Id);
            }
        }

        /// <summary>
        /// Check that the analyzer supports a diagnostic with <paramref name="descriptor"/>.
        /// </summary>
        /// <param name="analyzer">The <see cref="DiagnosticAnalyzer"/>.</param>
        /// <param name="descriptor">The descriptor of the supported diagnostic.</param>
        internal static void VerifySingleSupportedDiagnostic(DiagnosticAnalyzer analyzer, out DiagnosticDescriptor descriptor)
        {
            if (analyzer.SupportedDiagnostics.Length == 0)
            {
                var message = $"{analyzer.GetType().Name}.SupportedDiagnostics returns an empty array.";
                throw new AssertException(message);
            }

            if (analyzer.SupportedDiagnostics.Length > 1)
            {
                var message = "This can only be used for analyzers with one SupportedDiagnostics.\r\n" +
                              "Prefer overload with ExpectedDiagnostic.";
                throw new AssertException(message);
            }

            descriptor = analyzer.SupportedDiagnostics[0];
            if (descriptor is null)
            {
                var message = $"{analyzer.GetType().Name}.SupportedDiagnostics[0] returns null.";
                throw new AssertException(message);
            }
        }

        /// <summary>
        /// Check that the suppressor supports a diagnostic with <paramref name="descriptor"/>.
        /// </summary>
        /// <param name="suppressor">The <see cref="DiagnosticSuppressor"/>.</param>
        /// <param name="descriptor">The descriptor of the supported suppression.</param>
        internal static void VerifySingleSupportedSuppression(DiagnosticSuppressor suppressor, out SuppressionDescriptor descriptor)
        {
            if (suppressor.SupportedSuppressions.Length == 0)
            {
                var message = $"{suppressor.GetType().Name}.SupportedSuppressions returns an empty array.";
                throw new AssertException(message);
            }

            if (suppressor.SupportedSuppressions.Select(x => x.SuppressedDiagnosticId).Distinct().Count() > 1)
            {
                var message = "This can only be used for analyzers with one SuppressedDiagnosticId.\r\n" +
                              "Prefer overload with ExpectedDiagnostic.";
                throw new AssertException(message);
            }

            descriptor = suppressor.SupportedSuppressions[0];
            if (descriptor is null)
            {
                var message = $"{suppressor.GetType().Name}.SupportedDiagnostics[0] returns null.";
                throw new AssertException(message);
            }
        }

        /// <summary>
        /// Check that the analyzer supports a diagnostic with <paramref name="expectedId"/>.
        /// </summary>
        /// <param name="analyzer">The <see cref="DiagnosticAnalyzer"/>.</param>
        /// <param name="expectedId">The descriptor of the supported diagnostic.</param>
        internal static void VerifyAnalyzerSupportsDiagnostic(DiagnosticAnalyzer analyzer, string expectedId)
        {
            if (!analyzer.SupportedDiagnostics.Any(x => x.Id == expectedId))
            {
                var message = $"{analyzer.GetType().Name} does not produce a diagnostic with ID '{expectedId}'.{Environment.NewLine}" +
                              $"{analyzer.GetType().Name}.{nameof(analyzer.SupportedDiagnostics)}: {Format(analyzer.SupportedDiagnostics)}.{Environment.NewLine}" +
                              $"The expected diagnostic is: '{expectedId}'.";
                throw new AssertException(message);
            }
        }

        /// <summary>
        /// Check that the suppressor supports a diagnostic with <paramref name="expectedId"/>.
        /// </summary>
        /// <param name="suppressor">The <see cref="DiagnosticSuppressor"/>.</param>
        /// <param name="expectedId">The descriptor of the supported diagnostic.</param>
        internal static void VerifySuppressorSupportsDiagnostic(DiagnosticSuppressor suppressor, string expectedId)
        {
            if (!suppressor.SupportedSuppressions.Any(x => x.SuppressedDiagnosticId == expectedId))
            {
                var message = $"{suppressor.GetType().Name} does not supppress a diagnostic with ID '{expectedId}'.{Environment.NewLine}" +
                              $"{suppressor.GetType().Name}.{nameof(suppressor.SupportedSuppressions)}: {Format(suppressor.SupportedSuppressions)}.{Environment.NewLine}" +
                              $"The expected diagnostic is: '{expectedId}'.";
                throw new AssertException(message);
            }
        }

        private static void VerifyDiagnostics(
            DiagnosticsAndSources diagnosticsAndSources,
            IReadOnlyList<Diagnostic> selectedDiagnostics,
            IReadOnlyList<Diagnostic> allDiagnostics)
        {
            var expectedDiagnostics = diagnosticsAndSources.ExpectedDiagnostics;

            if (expectedDiagnostics.Count == 0)
            {
                throw new AssertException("Expected code to have at least one error position indicated with '↓'");
            }

            if (AnyMatch(expectedDiagnostics, selectedDiagnostics))
            {
                return;
            }

            if (selectedDiagnostics.TrySingle(out var single) &&
                expectedDiagnostics.Count == 1 &&
                expectedDiagnostics[0].Id == single.Id)
            {
                if (expectedDiagnostics[0].PositionMatches(single) &&
                    !expectedDiagnostics[0].MessageMatches(single))
                {
                    CodeAssert.AreEqual(expectedDiagnostics[0].Message!, single.GetMessage(CultureInfo.InvariantCulture), "Expected and actual messages do not match.");
                }
            }

            var error = StringBuilderPool.Borrow();
            error.AppendLine("Expected and actual diagnostics do not match.")
                 .AppendLine("Expected:");
            foreach (var expected in expectedDiagnostics.OrderBy(x => x.Span.StartLinePosition))
            {
                error.AppendLine(expected.ToString(diagnosticsAndSources.Code, "  "));
            }

            if (allDiagnostics.Count == 0)
            {
                error.AppendLine("Actual: <no diagnostics>");
            }
            else
            {
                error.AppendLine("Actual:");
                foreach (var diagnostic in allDiagnostics.OrderBy(x => x.Location.SourceSpan.Start))
                {
                    error.AppendLine(diagnostic.ToErrorString("  "));
                }
            }

            throw new AssertException(error.Return());

            static bool AnyMatch(IReadOnlyList<ExpectedDiagnostic> expectedDiagnostics, IReadOnlyList<Diagnostic> diagnostics)
            {
                foreach (var diagnostic in diagnostics)
                {
                    if (expectedDiagnostics.Any(x => x.Matches(diagnostic)))
                    {
                        continue;
                    }

                    return false;
                }

                foreach (var expected in expectedDiagnostics)
                {
                    if (diagnostics.Any(x => expected.Matches(x)))
                    {
                        continue;
                    }

                    return false;
                }

                return true;
            }
        }

        private static void VerifyCodeFixSupportsAnalyzer(DiagnosticAnalyzer analyzer, CodeFixProvider fix)
        {
            if (!analyzer.SupportedDiagnostics.Select(d => d.Id).Intersect(fix.FixableDiagnosticIds).Any())
            {
                var message = $"{analyzer.GetType().Name} does not produce diagnostics fixable by {fix.GetType().Name}.{Environment.NewLine}" +
                              $"{analyzer.GetType().Name}.{nameof(analyzer.SupportedDiagnostics)}: {Format(analyzer.SupportedDiagnostics)}.{Environment.NewLine}" +
                              $"{fix.GetType().Name}.{nameof(fix.FixableDiagnosticIds)}: {Format(fix.FixableDiagnosticIds)}.";
                throw new AssertException(message);
            }
        }

        private static string Format(IEnumerable<DiagnosticDescriptor> supportedDiagnostics)
        {
            return Format(supportedDiagnostics.Select(x => x.Id));
        }

        private static string Format(IEnumerable<SuppressionDescriptor> supportedSuppressions)
        {
            return Format(supportedSuppressions.Select(x => x.SuppressedDiagnosticId));
        }

        private static string Format(IEnumerable<string> ids)
        {
            // ReSharper disable PossibleMultipleEnumeration
            return ids.TrySingle(out var single)
                ? $"'{single}'"
                : $"{{{string.Join(", ", ids)}}}";
            //// ReSharper restore PossibleMultipleEnumeration
        }

        private static async Task AreEqualAsync(IReadOnlyList<string> expected, Solution actual, string? messageHeader)
        {
            var actualCount = actual.Projects.SelectMany(x => x.Documents).Count();
            if (expected.Count != actualCount)
            {
                throw new AssertException($"Expected {expected.Count} documents the fixed solution has {actualCount} documents.");
            }

            foreach (var project in actual.Projects)
            {
                foreach (var document in project.Documents)
                {
                    var fixedSource = await CodeReader.GetStringFromDocumentAsync(document, CancellationToken.None).ConfigureAwait(false);
                    CodeAssert.AreEqual(FindExpected(fixedSource), fixedSource, messageHeader);
                }
            }

            string FindExpected(string fixedSource)
            {
                var fixedNamespace = CodeReader.Namespace(fixedSource);
                var fixedFileName = CodeReader.FileName(fixedSource);
                var match = expected.FirstOrDefault(x => x == fixedSource);
                if (match != null)
                {
                    return match;
                }

                foreach (var candidate in expected)
                {
                    if (CodeReader.Namespace(candidate) == fixedNamespace &&
                        CodeReader.FileName(candidate) == fixedFileName)
                    {
                        return candidate;
                    }
                }

                throw new AssertException($"The fixed solution contains a document {fixedFileName} in namespace {fixedNamespace} that is not in the expected documents.");
            }
        }
    }
}
