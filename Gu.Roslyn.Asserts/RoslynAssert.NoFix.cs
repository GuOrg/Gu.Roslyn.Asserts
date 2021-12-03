namespace Gu.Roslyn.Asserts
{
    using System.Collections.Generic;
    using System.Collections.Immutable;
    using System.Linq;

    using Gu.Roslyn.Asserts.Internals;

    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CodeFixes;
    using Microsoft.CodeAnalysis.Diagnostics;

    public static partial class RoslynAssert
    {
        /// <summary>
        /// Verifies that
        /// 1. <paramref name="code"/> produces the expected diagnostics
        /// 2. The code fix does not change the code.
        /// </summary>
        /// <param name="analyzer">The <see cref="DiagnosticAnalyzer"/> to check <paramref name="code"/> with.</param>
        /// <param name="fix">The <see cref="CodeFixProvider"/> to apply on the <see cref="Diagnostic"/> reported.</param>
        /// <param name="code">The code to analyze with <paramref name="analyzer"/>. Indicate error position with ↓ (alt + 25).</param>
        public static void NoFix(DiagnosticAnalyzer analyzer, CodeFixProvider fix, params string[] code)
        {
            if (analyzer is null)
            {
                throw new System.ArgumentNullException(nameof(analyzer));
            }

            if (fix is null)
            {
                throw new System.ArgumentNullException(nameof(fix));
            }

            NoFix(
                analyzer,
                fix,
                DiagnosticsAndSources.CreateFromCodeWithErrorsIndicated(analyzer, code));
        }

        /// <summary>
        /// Verifies that
        /// 1. <paramref name="code"/> produces the expected diagnostics
        /// 2. The code fix does not change the code.
        /// </summary>
        /// <param name="analyzer">The <see cref="DiagnosticAnalyzer"/> to check <paramref name="code"/> with.</param>
        /// <param name="fix">The <see cref="CodeFixProvider"/> to apply on the <see cref="Diagnostic"/> reported.</param>
        /// <param name="expectedDiagnostic">The <see cref="ExpectedDiagnostic"/> with information about the expected <see cref="Diagnostic"/>. If <paramref name="analyzer"/> supports more than one <see cref="DiagnosticDescriptor.Id"/> this must be provided.</param>
        /// <param name="code">The code to analyze with <paramref name="analyzer"/>. Indicate error position with ↓ (alt + 25).</param>
        public static void NoFix(DiagnosticAnalyzer analyzer, CodeFixProvider fix, ExpectedDiagnostic expectedDiagnostic, params string[] code)
        {
            if (analyzer is null)
            {
                throw new System.ArgumentNullException(nameof(analyzer));
            }

            if (fix is null)
            {
                throw new System.ArgumentNullException(nameof(fix));
            }

            if (expectedDiagnostic is null)
            {
                throw new System.ArgumentNullException(nameof(expectedDiagnostic));
            }

            NoFix(
                analyzer,
                fix,
                DiagnosticsAndSources.Create(expectedDiagnostic, code));
        }

        /// <summary>
        /// Verifies that
        /// 1. <paramref name="code"/> produces the expected diagnostics
        /// 2. The code fix does not change the code.
        /// </summary>
        /// <param name="analyzer">The <see cref="DiagnosticAnalyzer"/> to check <paramref name="code"/> with.</param>
        /// <param name="fix">The <see cref="CodeFixProvider"/> to apply on the <see cref="Diagnostic"/> reported.</param>
        /// <param name="expectedDiagnostic">The <see cref="ExpectedDiagnostic"/> with information about the expected <see cref="Diagnostic"/>. If <paramref name="analyzer"/> supports more than one <see cref="DiagnosticDescriptor.Id"/> this must be provided.</param>
        /// <param name="code">The code to analyze with <paramref name="analyzer"/>. Indicate error position with ↓ (alt + 25).</param>
        /// <param name="settings">The <see cref="Settings"/>.</param>
        public static void NoFix(
            DiagnosticAnalyzer analyzer,
            CodeFixProvider fix,
            ExpectedDiagnostic expectedDiagnostic,
            string code,
            Settings? settings = null)
        {
            if (analyzer is null)
            {
                throw new System.ArgumentNullException(nameof(analyzer));
            }

            if (fix is null)
            {
                throw new System.ArgumentNullException(nameof(fix));
            }

            if (expectedDiagnostic is null)
            {
                throw new System.ArgumentNullException(nameof(expectedDiagnostic));
            }

            if (code is null)
            {
                throw new System.ArgumentNullException(nameof(code));
            }

            NoFix(
                analyzer: analyzer,
                fix: fix,
                diagnosticsAndSources: DiagnosticsAndSources.Create(expectedDiagnostic, code),
                settings: settings);
        }

        /// <summary>
        /// Verifies that
        /// 1. <paramref name="code"/> produces the expected diagnostics
        /// 2. The code fix does not change the code.
        /// </summary>
        /// <param name="analyzer">The <see cref="DiagnosticAnalyzer"/> to check <paramref name="code"/> with.</param>
        /// <param name="fix">The <see cref="CodeFixProvider"/> to apply on the <see cref="Diagnostic"/> reported.</param>
        /// <param name="expectedDiagnostic">The <see cref="ExpectedDiagnostic"/> with information about the expected <see cref="Diagnostic"/>. If <paramref name="analyzer"/> supports more than one <see cref="DiagnosticDescriptor.Id"/> this must be provided.</param>
        /// <param name="code">The code to analyze with <paramref name="analyzer"/>. Indicate error position with ↓ (alt + 25).</param>
        /// <param name="settings">The <see cref="Settings"/>.</param>
        public static void NoFix(
            DiagnosticAnalyzer analyzer,
            CodeFixProvider fix,
            ExpectedDiagnostic expectedDiagnostic,
            IReadOnlyList<string> code,
            Settings? settings = null)
        {
            if (analyzer is null)
            {
                throw new System.ArgumentNullException(nameof(analyzer));
            }

            if (fix is null)
            {
                throw new System.ArgumentNullException(nameof(fix));
            }

            if (expectedDiagnostic is null)
            {
                throw new System.ArgumentNullException(nameof(expectedDiagnostic));
            }

            if (code is null)
            {
                throw new System.ArgumentNullException(nameof(code));
            }

            NoFix(
                analyzer: analyzer,
                fix: fix,
                diagnosticsAndSources: DiagnosticsAndSources.Create(expectedDiagnostic, code),
                settings: settings);
        }

        /// <summary>
        /// Verifies that
        /// 1. <paramref name="code"/> produces the expected diagnostics
        /// 2. The code fix does not change the code.
        /// </summary>
        /// <param name="analyzer">The <see cref="DiagnosticAnalyzer"/> to check <paramref name="code"/> with.</param>
        /// <param name="fix">The <see cref="CodeFixProvider"/> to apply on the <see cref="Diagnostic"/> reported.</param>
        /// <param name="expectedDiagnostics">The <see cref="ExpectedDiagnostic"/> with information about the expected <see cref="Diagnostic"/>. If <paramref name="analyzer"/> supports more than one <see cref="DiagnosticDescriptor.Id"/> this must be provided.</param>
        /// <param name="code">The code to analyze with <paramref name="analyzer"/>. Indicate error position with ↓ (alt + 25).</param>
        public static void NoFix(DiagnosticAnalyzer analyzer, CodeFixProvider fix, IReadOnlyList<ExpectedDiagnostic> expectedDiagnostics, params string[] code)
        {
            if (analyzer is null)
            {
                throw new System.ArgumentNullException(nameof(analyzer));
            }

            if (fix is null)
            {
                throw new System.ArgumentNullException(nameof(fix));
            }

            if (expectedDiagnostics is null)
            {
                throw new System.ArgumentNullException(nameof(expectedDiagnostics));
            }

            NoFix(
                analyzer: analyzer,
                fix: fix,
                diagnosticsAndSources: new DiagnosticsAndSources(expectedDiagnostics, code));
        }

        /// <summary>
        /// Verifies that
        /// 1. <paramref name="code"/> produces the expected diagnostics
        /// 2. The code fix does not change the code.
        /// </summary>
        /// <param name="analyzer">The <see cref="DiagnosticAnalyzer"/> to check <paramref name="code"/> with.</param>
        /// <param name="fix">The <see cref="CodeFixProvider"/> to apply on the <see cref="Diagnostic"/> reported.</param>
        /// <param name="code">The code to analyze with <paramref name="analyzer"/>. Indicate error position with ↓ (alt + 25).</param>
        /// <param name="settings">The <see cref="Settings"/>.</param>
        public static void NoFix(
            DiagnosticAnalyzer analyzer,
            CodeFixProvider fix,
            IReadOnlyList<string> code,
            Settings? settings = null)
        {
            if (analyzer is null)
            {
                throw new System.ArgumentNullException(nameof(analyzer));
            }

            if (fix is null)
            {
                throw new System.ArgumentNullException(nameof(fix));
            }

            if (code is null)
            {
                throw new System.ArgumentNullException(nameof(code));
            }

            NoFix(
                analyzer: analyzer,
                fix: fix,
                diagnosticsAndSources: DiagnosticsAndSources.CreateFromCodeWithErrorsIndicated(analyzer, code),
                settings: settings);
        }

        /// <summary>
        /// Verifies that
        /// 1. <paramref name="code"/> produces the expected diagnostics
        /// 2. The code fix does not change the code.
        /// </summary>
        /// <param name="fix">The <see cref="CodeFixProvider"/> to apply on the <see cref="Diagnostic"/> reported.</param>
        /// <param name="expectedDiagnostic">The <see cref="ExpectedDiagnostic"/> with information about the expected <see cref="Diagnostic"/>.</param>
        /// <param name="code">The code to analyze. Indicate error position with ↓ (alt + 25).</param>
        /// <param name="settings">The <see cref="Settings"/>.</param>
        public static void NoFix(
            CodeFixProvider fix,
            ExpectedDiagnostic expectedDiagnostic,
            IReadOnlyList<string> code,
            Settings? settings = null)
        {
            if (fix is null)
            {
                throw new System.ArgumentNullException(nameof(fix));
            }

            if (expectedDiagnostic is null)
            {
                throw new System.ArgumentNullException(nameof(expectedDiagnostic));
            }

            if (code is null)
            {
                throw new System.ArgumentNullException(nameof(code));
            }

            var analyzer = new PlaceholderAnalyzer(expectedDiagnostic.Id);
            NoFix(
                analyzer: analyzer,
                fix: fix,
                diagnosticsAndSources: DiagnosticsAndSources.CreateFromCodeWithErrorsIndicated(analyzer, code),
                settings: settings);
        }

        /// <summary>
        /// Verifies that
        /// 1. <paramref name="diagnosticsAndSources"/> produces the expected diagnostics
        /// 2. The code fix does not change the code.
        /// </summary>
        /// <param name="analyzer">The <see cref="DiagnosticAnalyzer"/> to check <paramref name="diagnosticsAndSources"/> with.</param>
        /// <param name="fix">The <see cref="CodeFixProvider"/> to apply on the <see cref="Diagnostic"/> reported.</param>
        /// <param name="diagnosticsAndSources">The code to analyze with <paramref name="analyzer"/>. Indicate error position with ↓ (alt + 25).</param>
        /// <param name="settings">The <see cref="Settings"/>.</param>
        public static void NoFix(
            DiagnosticAnalyzer analyzer,
            CodeFixProvider fix,
            DiagnosticsAndSources diagnosticsAndSources,
            Settings? settings = null)
        {
            if (analyzer is null)
            {
                throw new System.ArgumentNullException(nameof(analyzer));
            }

            if (fix is null)
            {
                throw new System.ArgumentNullException(nameof(fix));
            }

            if (diagnosticsAndSources is null)
            {
                throw new System.ArgumentNullException(nameof(diagnosticsAndSources));
            }

            settings ??= Settings.Default;
            VerifyAnalyzerSupportsDiagnostics(analyzer, diagnosticsAndSources.ExpectedDiagnostics);
            VerifyCodeFixSupportsAnalyzer(analyzer, fix);
            var sln = CodeFactory.CreateSolution(
                analyzer,
                diagnosticsAndSources,
                settings);
            var diagnostics = Analyze.GetDiagnostics(analyzer, sln);
            VerifyDiagnostics(diagnosticsAndSources, diagnostics, sln);
            if (settings.AllowCompilationDiagnostics == AllowCompilationDiagnostics.None)
            {
                NoCompilerDiagnostics(sln);
            }

            VerifyNoFix(sln, diagnostics, fix);
        }

        private static void VerifyNoFix(Solution sln, IReadOnlyList<ImmutableArray<Diagnostic>> diagnostics, CodeFixProvider fix)
        {
            var fixableDiagnostics = diagnostics.SelectMany(x => x)
                                         .Where(x => fix.FixableDiagnosticIds.Contains(x.Id))
                                         .ToArray();

            foreach (var fixableDiagnostic in fixableDiagnostics)
            {
                var actions = Fix.GetActions(sln, fix, fixableDiagnostic);
                if (actions.Any())
                {
                    var builder = StringBuilderPool.Borrow()
                                                   .AppendLine("Expected code to have no fixable diagnostics.")
                                                   .AppendLine("The following actions were registered:");

                    foreach (var action in actions)
                    {
                        builder.AppendLine($"  '{action.Title}'");
                    }

                    throw new AssertException(builder.Return());
                }
            }
        }
    }
}
