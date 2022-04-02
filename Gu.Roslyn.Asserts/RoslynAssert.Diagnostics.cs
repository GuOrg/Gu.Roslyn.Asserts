namespace Gu.Roslyn.Asserts
{
    using System.Collections.Generic;
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
        /// <param name="code">The code to analyze with <paramref name="analyzer"/>. Indicate diagnostic position with ↓ (alt + 25).</param>
        public static void Diagnostics(DiagnosticAnalyzer analyzer, params string[] code)
        {
            if (analyzer is null)
            {
                throw new System.ArgumentNullException(nameof(analyzer));
            }

            Diagnostics(
                analyzer,
                DiagnosticsAndSources.FromMarkup(analyzer, code),
                Settings.Default);
        }

        /// <summary>
        /// Verifies that <paramref name="code"/> produces the expected diagnostics.
        /// </summary>
        /// <param name="analyzer">The <see cref="DiagnosticAnalyzer"/> to check <paramref name="code"/> with.</param>
        /// <param name="expectedDiagnostic">The <see cref="ExpectedDiagnostic"/> with information about the expected <see cref="Diagnostic"/>. If <paramref name="analyzer"/> supports more than one <see cref="DiagnosticDescriptor.Id"/> this must be provided.</param>
        /// <param name="code">The code to analyze with <paramref name="analyzer"/>. Indicate diagnostic position with ↓ (alt + 25).</param>
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
        /// <param name="code">The code to analyze with <paramref name="analyzer"/>. Indicate diagnostic position with ↓ (alt + 25).</param>
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
        /// <param name="code">The code to analyze with <paramref name="analyzer"/>. Indicate diagnostic position with ↓ (alt + 25).</param>
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
                DiagnosticsAndSources.FromMarkup(analyzer, code),
                settings);
        }

        /// <summary>
        /// Verifies that <paramref name="code"/> produces the expected diagnostics.
        /// </summary>
        /// <param name="analyzer">The <see cref="DiagnosticAnalyzer"/> to check <paramref name="code"/> with.</param>
        /// <param name="code">The code to analyze with <paramref name="analyzer"/>. Indicate diagnostic position with ↓ (alt + 25).</param>
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
                DiagnosticsAndSources.FromMarkup(analyzer, code),
                settings);
        }

        /// <summary>
        /// Verifies that <paramref name="code"/> produces the expected diagnostics.
        /// </summary>
        /// <param name="analyzer">The <see cref="DiagnosticAnalyzer"/> to check <paramref name="code"/> with.</param>
        /// <param name="expectedDiagnostic">The <see cref="ExpectedDiagnostic"/> with information about the expected <see cref="Diagnostic"/>. If <paramref name="analyzer"/> supports more than one <see cref="DiagnosticDescriptor.Id"/> this must be provided.</param>
        /// <param name="code">The code to analyze with <paramref name="analyzer"/>. Indicate diagnostic position with ↓ (alt + 25).</param>
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
        /// <param name="code">The code to analyze with <paramref name="analyzer"/>. Indicate diagnostic position with ↓ (alt + 25).</param>
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
        /// <param name="diagnosticsAndSources">The code to analyze with <paramref name="analyzer"/>. Indicate diagnostic position with ↓ (alt + 25).</param>
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
            var diagnostics = Analyze.GetDiagnostics(analyzer, sln);
            VerifyDiagnostics(diagnosticsAndSources, diagnostics);
            NoDiagnostics(diagnostics.SelectMany(x => x.FilterCompilerDiagnostics(settings.AllowedCompilerDiagnostics)));
        }

        private static void VerifyDiagnostics(DiagnosticsAndSources diagnosticsAndSources, IReadOnlyList<ProjectDiagnostics> diagnostics)
        {
            VerifyDiagnostics(
                diagnosticsAndSources,
                diagnostics.SelectMany(x => x.AnalyzerDiagnostics).ToList(),
                diagnostics.SelectMany(x => x.All()).ToList());
        }
    }
}
