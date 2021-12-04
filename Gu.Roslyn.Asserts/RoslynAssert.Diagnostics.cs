namespace Gu.Roslyn.Asserts
{
    using System;
    using System.Collections.Generic;
    using System.Collections.Immutable;
    using System.Globalization;
    using System.Linq;
    using Gu.Roslyn.Asserts.Internals;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.Diagnostics;

    public static partial class RoslynAssert
    {
        /// <summary>
        /// Verifies that <paramref name="code"/> produces the expected diagnostics.
        /// </summary>
        /// <param name="analyzer">The <see cref="DiagnosticAnalyzer"/> to check <paramref name="code"/> with.</param>
        /// <param name="code">The code to analyze with <paramref name="analyzer"/>. Indicate error position with ↓ (alt + 25).</param>
        public static void Diagnostics(DiagnosticAnalyzer analyzer, params string[] code)
        {
            if (analyzer is null)
            {
                throw new System.ArgumentNullException(nameof(analyzer));
            }

            Diagnostics(
                analyzer,
                DiagnosticsAndSources.CreateFromCodeWithErrorsIndicated(analyzer, code),
                Settings.Default);
        }

        /// <summary>
        /// Verifies that <paramref name="code"/> produces the expected diagnostics.
        /// </summary>
        /// <param name="analyzer">The <see cref="DiagnosticAnalyzer"/> to check <paramref name="code"/> with.</param>
        /// <param name="expectedDiagnostic">The <see cref="ExpectedDiagnostic"/> with information about the expected <see cref="Diagnostic"/>. If <paramref name="analyzer"/> supports more than one <see cref="DiagnosticDescriptor.Id"/> this must be provided.</param>
        /// <param name="code">The code to analyze with <paramref name="analyzer"/>. Indicate error position with ↓ (alt + 25).</param>
        public static void Diagnostics(DiagnosticAnalyzer analyzer, ExpectedDiagnostic expectedDiagnostic, params string[] code)
        {
            if (analyzer is null)
            {
                throw new System.ArgumentNullException(nameof(analyzer));
            }

            if (expectedDiagnostic is null)
            {
                throw new System.ArgumentNullException(nameof(expectedDiagnostic));
            }

            Diagnostics(
                analyzer,
                DiagnosticsAndSources.Create(expectedDiagnostic, code),
                Settings.Default);
        }

        /// <summary>
        /// Verifies that <paramref name="code"/> produces the expected diagnostics.
        /// </summary>
        /// <param name="analyzer">The <see cref="DiagnosticAnalyzer"/> to check <paramref name="code"/> with.</param>
        /// <param name="expectedDiagnostics">The expected diagnostics.</param>
        /// <param name="code">The code to analyze with <paramref name="analyzer"/>. Indicate error position with ↓ (alt + 25).</param>
        public static void Diagnostics(DiagnosticAnalyzer analyzer, IReadOnlyList<ExpectedDiagnostic> expectedDiagnostics, params string[] code)
        {
            if (analyzer is null)
            {
                throw new System.ArgumentNullException(nameof(analyzer));
            }

            if (expectedDiagnostics is null)
            {
                throw new System.ArgumentNullException(nameof(expectedDiagnostics));
            }

            Diagnostics(
                analyzer,
                new DiagnosticsAndSources(expectedDiagnostics, code),
                Settings.Default);
        }

        /// <summary>
        /// Verifies that <paramref name="code"/> produces the expected diagnostics.
        /// </summary>
        /// <param name="analyzer">The <see cref="DiagnosticAnalyzer"/> to check <paramref name="code"/> with.</param>
        /// <param name="code">The code to analyze with <paramref name="analyzer"/>. Indicate error position with ↓ (alt + 25).</param>
        /// <param name="settings">The <see cref="Settings"/>.</param>
        public static void Diagnostics(
            DiagnosticAnalyzer analyzer,
            string code,
            Settings? settings = null)
        {
            if (analyzer is null)
            {
                throw new System.ArgumentNullException(nameof(analyzer));
            }

            if (code is null)
            {
                throw new System.ArgumentNullException(nameof(code));
            }

            Diagnostics(
                analyzer,
                DiagnosticsAndSources.CreateFromCodeWithErrorsIndicated(analyzer, code),
                settings);
        }

        /// <summary>
        /// Verifies that <paramref name="code"/> produces the expected diagnostics.
        /// </summary>
        /// <param name="analyzer">The <see cref="DiagnosticAnalyzer"/> to check <paramref name="code"/> with.</param>
        /// <param name="code">The code to analyze with <paramref name="analyzer"/>. Indicate error position with ↓ (alt + 25).</param>
        /// <param name="settings">The <see cref="Settings"/>.</param>
        public static void Diagnostics(
            DiagnosticAnalyzer analyzer,
            IReadOnlyList<string> code,
            Settings? settings = null)
        {
            if (analyzer is null)
            {
                throw new System.ArgumentNullException(nameof(analyzer));
            }

            if (code is null)
            {
                throw new System.ArgumentNullException(nameof(code));
            }

            Diagnostics(
                analyzer,
                DiagnosticsAndSources.CreateFromCodeWithErrorsIndicated(analyzer, code),
                settings);
        }

        /// <summary>
        /// Verifies that <paramref name="code"/> produces the expected diagnostics.
        /// </summary>
        /// <param name="analyzer">The <see cref="DiagnosticAnalyzer"/> to check <paramref name="code"/> with.</param>
        /// <param name="expectedDiagnostic">The <see cref="ExpectedDiagnostic"/> with information about the expected <see cref="Diagnostic"/>. If <paramref name="analyzer"/> supports more than one <see cref="DiagnosticDescriptor.Id"/> this must be provided.</param>
        /// <param name="code">The code to analyze with <paramref name="analyzer"/>. Indicate error position with ↓ (alt + 25).</param>
        /// <param name="settings">The <see cref="Settings"/>.</param>
        public static void Diagnostics(
            DiagnosticAnalyzer analyzer,
            ExpectedDiagnostic expectedDiagnostic,
            string code,
            Settings? settings = null)
        {
            if (analyzer is null)
            {
                throw new System.ArgumentNullException(nameof(analyzer));
            }

            if (expectedDiagnostic is null)
            {
                throw new System.ArgumentNullException(nameof(expectedDiagnostic));
            }

            if (code is null)
            {
                throw new System.ArgumentNullException(nameof(code));
            }

            Diagnostics(
                analyzer,
                DiagnosticsAndSources.Create(expectedDiagnostic, code),
                settings);
        }

        /// <summary>
        /// Verifies that <paramref name="code"/> produces the expected diagnostics.
        /// </summary>
        /// <param name="analyzer">The <see cref="DiagnosticAnalyzer"/> to check <paramref name="code"/> with.</param>
        /// <param name="expectedDiagnostic">The <see cref="ExpectedDiagnostic"/> with information about the expected <see cref="Diagnostic"/>. If <paramref name="analyzer"/> supports more than one <see cref="DiagnosticDescriptor.Id"/> this must be provided.</param>
        /// <param name="code">The code to analyze with <paramref name="analyzer"/>. Indicate error position with ↓ (alt + 25).</param>
        /// <param name="settings">The <see cref="Settings"/>.</param>
        public static void Diagnostics(
            DiagnosticAnalyzer analyzer,
            ExpectedDiagnostic expectedDiagnostic,
            IReadOnlyList<string> code,
            Settings? settings = null)
        {
            if (analyzer is null)
            {
                throw new System.ArgumentNullException(nameof(analyzer));
            }

            if (expectedDiagnostic is null)
            {
                throw new System.ArgumentNullException(nameof(expectedDiagnostic));
            }

            if (code is null)
            {
                throw new System.ArgumentNullException(nameof(code));
            }

            Diagnostics(
                analyzer,
                DiagnosticsAndSources.Create(expectedDiagnostic, code),
                settings);
        }

        /// <summary>
        /// Verifies that <paramref name="diagnosticsAndSources"/> produces the expected diagnostics.
        /// </summary>
        /// <param name="analyzer">The <see cref="DiagnosticAnalyzer"/> to check <paramref name="diagnosticsAndSources"/> with.</param>
        /// <param name="diagnosticsAndSources">The code to analyze with <paramref name="analyzer"/>. Indicate error position with ↓ (alt + 25).</param>
        /// <param name="settings">The <see cref="Settings"/>.</param>
        public static void Diagnostics(
            DiagnosticAnalyzer analyzer,
            DiagnosticsAndSources diagnosticsAndSources,
            Settings? settings = null)
        {
            if (analyzer is null)
            {
                throw new System.ArgumentNullException(nameof(analyzer));
            }

            if (diagnosticsAndSources is null)
            {
                throw new System.ArgumentNullException(nameof(diagnosticsAndSources));
            }

            settings ??= Settings.Default;
            VerifyAnalyzerSupportsDiagnostics(analyzer, diagnosticsAndSources.ExpectedDiagnostics);
            var sln = CodeFactory.CreateSolution(
                analyzer,
                diagnosticsAndSources,
                settings);
            var diagnostics = Analyze.GetDiagnosticsAsync(analyzer, sln).GetAwaiter().GetResult();
            VerifyDiagnostics(diagnosticsAndSources, diagnostics);
            NoDiagnostics(diagnostics.SelectMany(x => x.FilterCompilerDiagnostics(settings.AllowCompilationDiagnostics)));
        }

        private static void VerifyDiagnostics(DiagnosticsAndSources diagnosticsAndSources, IReadOnlyList<ProjectDiagnostics> diagnostics, string? expectedMessage = null)
        {
            if (diagnosticsAndSources.ExpectedDiagnostics.Count == 0)
            {
                throw new AssertException("Expected code to have at least one error position indicated with '↓'");
            }

            if (AnyMatch(diagnosticsAndSources.ExpectedDiagnostics, diagnostics.SelectMany(x => x.AnalyzerDiagnostics)))
            {
                if (expectedMessage != null)
                {
                    foreach (var actual in diagnostics.SelectMany(x => x.AnalyzerDiagnostics))
                    {
                        var actualMessage = actual.GetMessage(CultureInfo.InvariantCulture);
                        TextAssert.AreEqual(expectedMessage, actualMessage, $"Expected and actual diagnostic message for the diagnostic {actual} does not match");
                    }
                }

                return;
            }

            var error = StringBuilderPool.Borrow();
            if (diagnostics.SelectMany(x => x.AnalyzerDiagnostics).TrySingle(out var single) &&
                diagnosticsAndSources.ExpectedDiagnostics.Count == 1 &&
                diagnosticsAndSources.ExpectedDiagnostics[0].Id == single.Id)
            {
                if (diagnosticsAndSources.ExpectedDiagnostics[0].PositionMatches(single) &&
                    !diagnosticsAndSources.ExpectedDiagnostics[0].MessageMatches(single))
                {
                    CodeAssert.AreEqual(diagnosticsAndSources.ExpectedDiagnostics[0].Message!, single.GetMessage(CultureInfo.InvariantCulture), "Expected and actual messages do not match.");
                }
            }

            var allDiagnostics = diagnostics.SelectMany(x => x.All()).ToArray();
            error.AppendLine("Expected and actual diagnostics do not match.")
                 .AppendLine("Expected:");
            foreach (var expected in diagnosticsAndSources.ExpectedDiagnostics.OrderBy(x => x.Span.StartLinePosition))
            {
                error.AppendLine(expected.ToString(diagnosticsAndSources.Code, "  "));
            }

            if (allDiagnostics.Length == 0)
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

            static bool AnyMatch(IReadOnlyList<ExpectedDiagnostic> expectedDiagnostics, IEnumerable<Diagnostic> diagnostics)
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
    }
}
