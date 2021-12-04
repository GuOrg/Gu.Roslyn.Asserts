namespace Gu.Roslyn.Asserts
{
    using System.Collections.Generic;
    using System.Threading;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CodeFixes;
    using Microsoft.CodeAnalysis.Diagnostics;

    public static partial class RoslynAssert
    {
        /// <summary>
        /// Verifies that
        /// 1. <paramref name="before"/> produces the expected diagnostics
        /// 2. The code fix fixes the code.
        /// </summary>
        /// <param name="analyzer">The <see cref="DiagnosticAnalyzer"/> to check <paramref name="before"/> with.</param>
        /// <param name="fix">The <see cref="CodeFixProvider"/> to apply on the <see cref="Diagnostic"/> reported.</param>
        /// <param name="expectedDiagnostic">The <see cref="ExpectedDiagnostic"/> with information about the expected <see cref="Diagnostic"/>. If <paramref name="analyzer"/> supports more than one <see cref="DiagnosticDescriptor.Id"/> this must be provided.</param>
        /// <param name="before">The code to analyze with <paramref name="analyzer"/>. Indicate error position with ↓ (alt + 25).</param>
        /// <param name="after">The expected code produced by applying <paramref name="fix"/>.</param>
        /// <param name="fixTitle">The expected title of the fix. Must be provided if more than one code action is registered.</param>
        /// <param name="settings">The <see cref="Settings"/>.</param>
        public static void FixAll(
            DiagnosticAnalyzer analyzer,
            CodeFixProvider fix,
            ExpectedDiagnostic expectedDiagnostic,
            string before,
            string after,
            string? fixTitle = null,
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

            if (before is null)
            {
                throw new System.ArgumentNullException(nameof(before));
            }

            if (after is null)
            {
                throw new System.ArgumentNullException(nameof(after));
            }

            FixAll(
                analyzer: analyzer,
                fix: fix,
                diagnosticsAndSources: DiagnosticsAndSources.Create(expectedDiagnostic, before),
                after: MergeFixedCode(new[] { before }, after),
                fixTitle: fixTitle,
                settings: settings);
        }

        /// <summary>
        /// Verifies that
        /// 1. <paramref name="before"/> produces the expected diagnostics
        /// 2. The code fix fixes the code.
        /// </summary>
        /// <param name="analyzer">The <see cref="DiagnosticAnalyzer"/> to check <paramref name="before"/> with.</param>
        /// <param name="fix">The <see cref="CodeFixProvider"/> to apply on the <see cref="Diagnostic"/> reported.</param>
        /// <param name="expectedDiagnostic">The <see cref="ExpectedDiagnostic"/> with information about the expected <see cref="Diagnostic"/>. If <paramref name="analyzer"/> supports more than one <see cref="DiagnosticDescriptor.Id"/> this must be provided.</param>
        /// <param name="before">The code to analyze with <paramref name="analyzer"/>. Indicate error position with ↓ (alt + 25).</param>
        /// <param name="after">The expected code produced by applying <paramref name="fix"/>.</param>
        /// <param name="fixTitle">The expected title of the fix. Must be provided if more than one code action is registered.</param>
        /// <param name="settings">The <see cref="Settings"/>.</param>
        public static void FixAll(
            DiagnosticAnalyzer analyzer,
            CodeFixProvider fix,
            ExpectedDiagnostic expectedDiagnostic,
            IReadOnlyList<string> before,
            string after,
            string? fixTitle = null,
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

            if (before is null)
            {
                throw new System.ArgumentNullException(nameof(before));
            }

            if (after is null)
            {
                throw new System.ArgumentNullException(nameof(after));
            }

            FixAll(
                analyzer: analyzer,
                fix: fix,
                diagnosticsAndSources: DiagnosticsAndSources.Create(expectedDiagnostic, before),
                after: MergeFixedCode(before, after),
                fixTitle: fixTitle,
                settings: settings);
        }

        /// <summary>
        /// Verifies that
        /// 1. <paramref name="before"/> produces the expected diagnostics
        /// 2. The code fix fixes the code.
        /// </summary>
        /// <param name="analyzer">The <see cref="DiagnosticAnalyzer"/> to check <paramref name="before"/> with.</param>
        /// <param name="fix">The <see cref="CodeFixProvider"/> to apply on the <see cref="Diagnostic"/> reported.</param>
        /// <param name="expectedDiagnostic">The <see cref="ExpectedDiagnostic"/> with information about the expected <see cref="Diagnostic"/>. If <paramref name="analyzer"/> supports more than one <see cref="DiagnosticDescriptor.Id"/> this must be provided.</param>
        /// <param name="before">The code to analyze with <paramref name="analyzer"/>. Indicate error position with ↓ (alt + 25).</param>
        /// <param name="after">The expected code produced by applying <paramref name="fix"/>.</param>
        /// <param name="fixTitle">The expected title of the fix. Must be provided if more than one code action is registered.</param>
        /// <param name="settings">The <see cref="Settings"/>.</param>
        public static void FixAll(
            DiagnosticAnalyzer analyzer,
            CodeFixProvider fix,
            ExpectedDiagnostic expectedDiagnostic,
            IReadOnlyList<string> before,
            IReadOnlyList<string> after,
            string? fixTitle = null,
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

            if (before is null)
            {
                throw new System.ArgumentNullException(nameof(before));
            }

            if (after is null)
            {
                throw new System.ArgumentNullException(nameof(after));
            }

            FixAll(
                analyzer: analyzer,
                fix: fix,
                diagnosticsAndSources: DiagnosticsAndSources.Create(expectedDiagnostic, before),
                after: after,
                fixTitle: fixTitle,
                settings: settings);
        }

        /// <summary>
        /// Verifies that
        /// 1. <paramref name="before"/> produces the expected diagnostics
        /// 2. The code fix fixes the code.
        /// </summary>
        /// <param name="fix">The <see cref="CodeFixProvider"/> to apply on the <see cref="Diagnostic"/> reported.</param>
        /// <param name="expectedDiagnostic">The <see cref="ExpectedDiagnostic"/> with information about the expected <see cref="Diagnostic"/>.</param>
        /// <param name="before">The code to analyze for <paramref name="expectedDiagnostic"/>. Indicate error position with ↓ (alt + 25).</param>
        /// <param name="after">The expected code produced by applying <paramref name="fix"/>.</param>
        /// <param name="fixTitle">The expected title of the fix. Must be provided if more than one code action is registered.</param>
        /// <param name="settings">The <see cref="Settings"/>.</param>
        public static void FixAll(
            CodeFixProvider fix,
            ExpectedDiagnostic expectedDiagnostic,
            string before,
            string after,
            string? fixTitle = null,
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

            if (before is null)
            {
                throw new System.ArgumentNullException(nameof(before));
            }

            if (after is null)
            {
                throw new System.ArgumentNullException(nameof(after));
            }

            var analyzer = new PlaceholderAnalyzer(expectedDiagnostic.Id);
            FixAll(
                analyzer: analyzer,
                fix: fix,
                diagnosticsAndSources: DiagnosticsAndSources.Create(expectedDiagnostic, before),
                after: new[] { after },
                fixTitle: fixTitle,
                settings: settings);
        }

        /// <summary>
        /// Verifies that
        /// 1. <paramref name="before"/> produces the expected diagnostics
        /// 2. The code fix fixes the code.
        /// </summary>
        /// <param name="fix">The <see cref="CodeFixProvider"/> to apply on the <see cref="Diagnostic"/> reported.</param>
        /// <param name="expectedDiagnostic">The <see cref="ExpectedDiagnostic"/> with information about the expected <see cref="Diagnostic"/>.</param>
        /// <param name="before">The code to analyze for <paramref name="expectedDiagnostic"/>. Indicate error position with ↓ (alt + 25).</param>
        /// <param name="after">The expected code produced by applying <paramref name="fix"/>.</param>
        /// <param name="fixTitle">The expected title of the fix. Must be provided if more than one code action is registered.</param>
        /// <param name="settings">The <see cref="Settings"/>.</param>
        public static void FixAll(
            CodeFixProvider fix,
            ExpectedDiagnostic expectedDiagnostic,
            IReadOnlyList<string> before,
            string after,
            string? fixTitle = null,
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

            if (before is null)
            {
                throw new System.ArgumentNullException(nameof(before));
            }

            if (after is null)
            {
                throw new System.ArgumentNullException(nameof(after));
            }

            var analyzer = new PlaceholderAnalyzer(expectedDiagnostic.Id);
            FixAll(
                analyzer: analyzer,
                fix: fix,
                diagnosticsAndSources: DiagnosticsAndSources.Create(expectedDiagnostic, before),
                after: MergeFixedCode(before, after),
                fixTitle: fixTitle,
                settings: settings);
        }

        /// <summary>
        /// Verifies that
        /// 1. <paramref name="before"/> produces the expected diagnostics
        /// 2. The code fix fixes the code.
        /// </summary>
        /// <param name="fix">The <see cref="CodeFixProvider"/> to apply on the <see cref="Diagnostic"/> reported.</param>
        /// <param name="expectedDiagnostic">The <see cref="ExpectedDiagnostic"/> with information about the expected <see cref="Diagnostic"/>.</param>
        /// <param name="before">The code to analyze for <paramref name="expectedDiagnostic"/>. Indicate error position with ↓ (alt + 25).</param>
        /// <param name="after">The expected code produced by applying <paramref name="fix"/>.</param>
        /// <param name="fixTitle">The expected title of the fix. Must be provided if more than one code action is registered.</param>
        /// <param name="settings">The <see cref="Settings"/>.</param>
        public static void FixAll(
            CodeFixProvider fix,
            ExpectedDiagnostic expectedDiagnostic,
            IReadOnlyList<string> before,
            IReadOnlyList<string> after,
            string? fixTitle = null,
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

            if (before is null)
            {
                throw new System.ArgumentNullException(nameof(before));
            }

            if (after is null)
            {
                throw new System.ArgumentNullException(nameof(after));
            }

            var analyzer = new PlaceholderAnalyzer(expectedDiagnostic.Id);
            FixAll(
                analyzer: analyzer,
                fix: fix,
                diagnosticsAndSources: DiagnosticsAndSources.Create(expectedDiagnostic, before),
                after: after,
                fixTitle: fixTitle,
                settings: settings);
        }

        /// <summary>
        /// Verifies that
        /// 1. <paramref name="before"/> produces the expected diagnostics
        /// 2. The code fix fixes the code.
        /// </summary>
        /// <param name="analyzer">The <see cref="DiagnosticAnalyzer"/> to check <paramref name="before"/> with.</param>
        /// <param name="fix">The <see cref="CodeFixProvider"/> to apply on the <see cref="Diagnostic"/> reported.</param>
        /// <param name="before">The code to analyze with <paramref name="analyzer"/>. Indicate error position with ↓ (alt + 25).</param>
        /// <param name="after">The expected code produced by applying <paramref name="fix"/>.</param>
        /// <param name="fixTitle">The expected title of the fix. Must be provided if more than one code action is registered.</param>
        /// <param name="settings">The <see cref="Settings"/>.</param>
        public static void FixAll(
            DiagnosticAnalyzer analyzer,
            CodeFixProvider fix,
            string before,
            string after,
            string? fixTitle = null,
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

            if (before is null)
            {
                throw new System.ArgumentNullException(nameof(before));
            }

            if (after is null)
            {
                throw new System.ArgumentNullException(nameof(after));
            }

            FixAll(
                analyzer: analyzer,
                fix: fix,
                diagnosticsAndSources: DiagnosticsAndSources.CreateFromCodeWithErrorsIndicated(analyzer, before),
                after: new[] { after },
                fixTitle: fixTitle,
                settings: settings);
        }

        /// <summary>
        /// Verifies that
        /// 1. <paramref name="before"/> produces the expected diagnostics
        /// 2. The code fix fixes the code.
        /// </summary>
        /// <param name="analyzer">The <see cref="DiagnosticAnalyzer"/> to check <paramref name="before"/> with.</param>
        /// <param name="fix">The <see cref="CodeFixProvider"/> to apply on the <see cref="Diagnostic"/> reported.</param>
        /// <param name="before">The code to analyze with <paramref name="analyzer"/>. Indicate error position with ↓ (alt + 25).</param>
        /// <param name="after">The expected code produced by applying <paramref name="fix"/>.</param>
        /// <param name="fixTitle">The expected title of the fix. Must be provided if more than one code action is registered.</param>
        /// <param name="settings">The <see cref="Settings"/>.</param>
        public static void FixAll(
            DiagnosticAnalyzer analyzer,
            CodeFixProvider fix,
            IReadOnlyList<string> before,
            string after,
            string? fixTitle = null,
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

            if (before is null)
            {
                throw new System.ArgumentNullException(nameof(before));
            }

            if (after is null)
            {
                throw new System.ArgumentNullException(nameof(after));
            }

            FixAll(
                analyzer: analyzer,
                fix: fix,
                diagnosticsAndSources: DiagnosticsAndSources.CreateFromCodeWithErrorsIndicated(analyzer, before),
                after: MergeFixedCode(before, after),
                fixTitle: fixTitle,
                settings: settings);
        }

        /// <summary>
        /// Verifies that
        /// 1. <paramref name="before"/> produces the expected diagnostics
        /// 2. The code fix fixes the code.
        /// </summary>
        /// <param name="analyzer">The <see cref="DiagnosticAnalyzer"/> to check <paramref name="before"/> with.</param>
        /// <param name="fix">The <see cref="CodeFixProvider"/> to apply on the <see cref="Diagnostic"/> reported.</param>
        /// <param name="before">The code to analyze with <paramref name="analyzer"/>. Indicate error position with ↓ (alt + 25).</param>
        /// <param name="after">The expected code produced by applying <paramref name="fix"/>.</param>
        /// <param name="fixTitle">The expected title of the fix. Must be provided if more than one code action is registered.</param>
        /// <param name="settings">The <see cref="Settings"/>.</param>
        public static void FixAll(
            DiagnosticAnalyzer analyzer,
            CodeFixProvider fix,
            IReadOnlyList<string> before,
            IReadOnlyList<string> after,
            string? fixTitle = null,
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

            if (before is null)
            {
                throw new System.ArgumentNullException(nameof(before));
            }

            if (after is null)
            {
                throw new System.ArgumentNullException(nameof(after));
            }

            FixAll(
                analyzer: analyzer,
                fix: fix,
                diagnosticsAndSources: DiagnosticsAndSources.CreateFromCodeWithErrorsIndicated(analyzer, before),
                after: after,
                fixTitle: fixTitle,
                settings: settings);
        }

        /// <summary>
        /// Verifies that
        /// 1. <paramref name="diagnosticsAndSources"/> produces the expected diagnostics
        /// 2. The code fix fixes the code.
        /// </summary>
        /// <param name="analyzer">The <see cref="DiagnosticAnalyzer"/> to check <paramref name="diagnosticsAndSources"/> with.</param>
        /// <param name="fix">The <see cref="CodeFixProvider"/> to apply on the <see cref="Diagnostic"/> reported.</param>
        /// <param name="diagnosticsAndSources">The code and expected diagnostics.</param>
        /// <param name="after">The expected code produced by applying <paramref name="fix"/>.</param>
        /// <param name="fixTitle">The expected title of the fix. Must be provided if more than one code action is registered.</param>
        /// <param name="settings">The <see cref="Settings"/>.</param>
        public static void FixAll(
            DiagnosticAnalyzer analyzer,
            CodeFixProvider fix,
            DiagnosticsAndSources diagnosticsAndSources,
            IReadOnlyList<string> after,
            string? fixTitle,
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

            if (after is null)
            {
                throw new System.ArgumentNullException(nameof(after));
            }

            settings ??= Settings.Default;
            VerifyAnalyzerSupportsDiagnostics(analyzer, diagnosticsAndSources.ExpectedDiagnostics);
            VerifyCodeFixSupportsAnalyzer(analyzer, fix);
            var sln = CodeFactory.CreateSolution(
                diagnosticsAndSources: diagnosticsAndSources,
                analyzer: analyzer,
                settings: settings);
            var diagnostics = Analyze.GetDiagnostics(analyzer, sln);
            VerifyDiagnostics(diagnosticsAndSources, diagnostics);
            FixAllOneByOne(analyzer, fix, sln, after, fixTitle, settings.AllowCompilationDiagnostics);

            var fixAllProvider = fix.GetFixAllProvider();
            if (fixAllProvider != null)
            {
                foreach (var scope in fixAllProvider.GetSupportedFixAllScopes())
                {
                    FixAllByScope(analyzer, fix, sln, after, fixTitle, settings.AllowCompilationDiagnostics, scope);
                }
            }
        }

        /// <summary>
        /// Verifies that
        /// 1. <paramref name="before"/> produces the expected diagnostics
        /// 2. The code fix fixes the code.
        /// </summary>
        /// <param name="analyzer">The <see cref="DiagnosticAnalyzer"/> to check <paramref name="before"/> with.</param>
        /// <param name="fix">The <see cref="CodeFixProvider"/> to apply on the <see cref="Diagnostic"/> reported.</param>
        /// <param name="before">The code to analyze with <paramref name="analyzer"/>. Indicate error position with ↓ (alt + 25).</param>
        /// <param name="after">The expected code produced by applying <paramref name="fix"/>.</param>
        /// <param name="fixTitle">The expected title of the fix. Must be provided if more than one code action is registered.</param>
        /// <param name="settings">The <see cref="Settings"/>.</param>
        public static void FixAllInDocument(
            DiagnosticAnalyzer analyzer,
            CodeFixProvider fix,
            string before,
            string after,
            string? fixTitle = null,
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

            if (before is null)
            {
                throw new System.ArgumentNullException(nameof(before));
            }

            if (after is null)
            {
                throw new System.ArgumentNullException(nameof(after));
            }

            FixAllByScope(
                analyzer: analyzer,
                fix: fix,
                before: new[] { before },
                after: new[] { after },
                scope: FixAllScope.Document,
                fixTitle: fixTitle,
                settings: settings);
        }

        /// <summary>
        /// Verifies that
        /// 1. <paramref name="before"/> produces the expected diagnostics
        /// 2. The code fix fixes the code.
        /// </summary>
        /// <param name="analyzer">The <see cref="DiagnosticAnalyzer"/> to check <paramref name="before"/> with.</param>
        /// <param name="fix">The <see cref="CodeFixProvider"/> to apply on the <see cref="Diagnostic"/> reported.</param>
        /// <param name="expectedDiagnostic">The <see cref="ExpectedDiagnostic"/> with information about the expected <see cref="Diagnostic"/>. If <paramref name="analyzer"/> supports more than one <see cref="DiagnosticDescriptor.Id"/> this must be provided.</param>
        /// <param name="before">The code to analyze with <paramref name="analyzer"/>. Indicate error position with ↓ (alt + 25).</param>
        /// <param name="after">The expected code produced by applying <paramref name="fix"/>.</param>
        /// <param name="fixTitle">The expected title of the fix. Must be provided if more than one code action is registered.</param>
        /// <param name="settings">The <see cref="Settings"/>.</param>
        public static void FixAllInDocument(
            DiagnosticAnalyzer analyzer,
            CodeFixProvider fix,
            ExpectedDiagnostic expectedDiagnostic,
            string before,
            string after,
            string? fixTitle = null,
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

            if (before is null)
            {
                throw new System.ArgumentNullException(nameof(before));
            }

            if (after is null)
            {
                throw new System.ArgumentNullException(nameof(after));
            }

            settings ??= Settings.Default;
            var diagnosticsAndSources = DiagnosticsAndSources.Create(expectedDiagnostic, before);
            VerifyAnalyzerSupportsDiagnostics(analyzer, diagnosticsAndSources.ExpectedDiagnostics);
            VerifyCodeFixSupportsAnalyzer(analyzer, fix);
            var sln = CodeFactory.CreateSolution(
                analyzer,
                diagnosticsAndSources,
                settings: settings);
            var diagnostics = Analyze.GetDiagnostics(analyzer, sln);
            VerifyDiagnostics(diagnosticsAndSources, diagnostics);
            FixAllByScope(analyzer, fix, sln, new[] { after }, fixTitle, settings.AllowCompilationDiagnostics, FixAllScope.Document);
        }

        /// <summary>
        /// Verifies that
        /// 1. <paramref name="before"/> produces the expected diagnostics
        /// 2. The code fix fixes the code.
        /// </summary>
        /// <param name="analyzer">The <see cref="DiagnosticAnalyzer"/> to check <paramref name="before"/> with.</param>
        /// <param name="fix">The <see cref="CodeFixProvider"/> to apply on the <see cref="Diagnostic"/> reported.</param>
        /// <param name="before">The code to analyze with <paramref name="analyzer"/>. Indicate error position with ↓ (alt + 25).</param>
        /// <param name="after">The expected code produced by applying <paramref name="fix"/>.</param>
        /// <param name="fixTitle">The expected title of the fix. Must be provided if more than one code action is registered.</param>
        /// <param name="settings">The <see cref="Settings"/>.</param>
        public static void FixAllOneByOne(
            DiagnosticAnalyzer analyzer,
            CodeFixProvider fix,
            string before,
            string after,
            string? fixTitle = null,
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

            if (before is null)
            {
                throw new System.ArgumentNullException(nameof(before));
            }

            if (after is null)
            {
                throw new System.ArgumentNullException(nameof(after));
            }

            settings ??= Settings.Default;
            var diagnosticsAndSources = DiagnosticsAndSources.CreateFromCodeWithErrorsIndicated(analyzer, before);
            VerifyAnalyzerSupportsDiagnostics(analyzer, diagnosticsAndSources.ExpectedDiagnostics);
            VerifyCodeFixSupportsAnalyzer(analyzer, fix);
            var sln = CodeFactory.CreateSolution(
                analyzer,
                diagnosticsAndSources,
                settings: settings);
            var diagnostics = Analyze.GetDiagnostics(analyzer, sln);
            VerifyDiagnostics(diagnosticsAndSources, diagnostics);
            FixAllOneByOne(analyzer, fix, sln, new[] { after }, fixTitle, settings.AllowCompilationDiagnostics);
        }

        /// <summary>
        /// Verifies that
        /// 1. <paramref name="before"/> produces the expected diagnostics
        /// 2. The code fix fixes the code.
        /// </summary>
        /// <param name="analyzer">The <see cref="DiagnosticAnalyzer"/> to check <paramref name="before"/> with.</param>
        /// <param name="fix">The <see cref="CodeFixProvider"/> to apply on the <see cref="Diagnostic"/> reported.</param>
        /// <param name="expectedDiagnostic">The <see cref="ExpectedDiagnostic"/> with information about the expected <see cref="Diagnostic"/>. If <paramref name="analyzer"/> supports more than one <see cref="DiagnosticDescriptor.Id"/> this must be provided.</param>
        /// <param name="before">The code to analyze with <paramref name="analyzer"/>. Indicate error position with ↓ (alt + 25).</param>
        /// <param name="after">The expected code produced by applying <paramref name="fix"/>.</param>
        /// <param name="fixTitle">The expected title of the fix. Must be provided if more than one code action is registered.</param>
        /// <param name="settings">The <see cref="Settings"/>.</param>
        public static void FixAllOneByOne(
            DiagnosticAnalyzer analyzer,
            CodeFixProvider fix,
            ExpectedDiagnostic expectedDiagnostic,
            string before,
            string after,
            string? fixTitle = null,
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

            if (before is null)
            {
                throw new System.ArgumentNullException(nameof(before));
            }

            if (after is null)
            {
                throw new System.ArgumentNullException(nameof(after));
            }

            settings ??= Settings.Default;
            var diagnosticsAndSources = DiagnosticsAndSources.Create(expectedDiagnostic, before);
            VerifyAnalyzerSupportsDiagnostics(analyzer, diagnosticsAndSources.ExpectedDiagnostics);
            VerifyCodeFixSupportsAnalyzer(analyzer, fix);
            var sln = CodeFactory.CreateSolution(
                analyzer,
                diagnosticsAndSources,
                settings: settings);
            var diagnostics = Analyze.GetDiagnostics(analyzer, sln);
            VerifyDiagnostics(diagnosticsAndSources, diagnostics);
            FixAllOneByOne(analyzer, fix, sln, new[] { after }, fixTitle, settings.AllowCompilationDiagnostics);
        }

        /// <summary>
        /// Verifies that
        /// 1. <paramref name="before"/> produces the expected diagnostics
        /// 2. The code fix fixes the code.
        /// </summary>
        /// <param name="fix">The <see cref="CodeFixProvider"/> to apply on the <see cref="Diagnostic"/> reported.</param>
        /// <param name="expectedDiagnostic">The <see cref="ExpectedDiagnostic"/> with information about the expected <see cref="Diagnostic"/>.</param>
        /// <param name="before">The code to analyze for <paramref name="expectedDiagnostic"/>. Indicate error position with ↓ (alt + 25).</param>
        /// <param name="after">The expected code produced by applying <paramref name="fix"/>.</param>
        /// <param name="fixTitle">The expected title of the fix. Must be provided if more than one code action is registered.</param>
        /// <param name="settings">The <see cref="Settings"/>.</param>
        public static void FixAllOneByOne(
            CodeFixProvider fix,
            ExpectedDiagnostic expectedDiagnostic,
            string before,
            string after,
            string? fixTitle = null,
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

            if (before is null)
            {
                throw new System.ArgumentNullException(nameof(before));
            }

            if (after is null)
            {
                throw new System.ArgumentNullException(nameof(after));
            }

            settings ??= Settings.Default;
            var analyzer = new PlaceholderAnalyzer(expectedDiagnostic.Id);
            var diagnosticsAndSources = DiagnosticsAndSources.Create(expectedDiagnostic, before);
            VerifyAnalyzerSupportsDiagnostics(analyzer, diagnosticsAndSources.ExpectedDiagnostics);
            VerifyCodeFixSupportsAnalyzer(analyzer, fix);
            var sln = CodeFactory.CreateSolution(
                analyzer,
                diagnosticsAndSources,
                settings: settings);
            var diagnostics = Analyze.GetDiagnostics(analyzer, sln);
            VerifyDiagnostics(diagnosticsAndSources, diagnostics);
            FixAllOneByOne(analyzer, fix, sln, new[] { after }, fixTitle, settings.AllowCompilationDiagnostics);
        }

        /// <summary>
        /// Verifies that
        /// 1. <paramref name="before"/> produces the expected diagnostics
        /// 2. The code fix fixes the code.
        /// </summary>
        /// <param name="fix">The <see cref="CodeFixProvider"/> to apply on the <see cref="Diagnostic"/> reported.</param>
        /// <param name="expectedDiagnostic">The <see cref="ExpectedDiagnostic"/> with information about the expected <see cref="Diagnostic"/>.</param>
        /// <param name="before">The code to analyze for <paramref name="expectedDiagnostic"/>. Indicate error position with ↓ (alt + 25).</param>
        /// <param name="after">The expected code produced by applying <paramref name="fix"/>.</param>
        /// <param name="scope">The scope to apply fixes for.</param>
        /// <param name="fixTitle">The expected title of the fix. Must be provided if more than one code action is registered.</param>
        /// <param name="settings">The <see cref="Settings"/>.</param>
        public static void FixAllByScope(
            CodeFixProvider fix,
            ExpectedDiagnostic expectedDiagnostic,
            IReadOnlyList<string> before,
            IReadOnlyList<string> after,
            FixAllScope scope,
            string? fixTitle = null,
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

            if (before is null)
            {
                throw new System.ArgumentNullException(nameof(before));
            }

            if (after is null)
            {
                throw new System.ArgumentNullException(nameof(after));
            }

            settings ??= Settings.Default;
            var analyzer = new PlaceholderAnalyzer(expectedDiagnostic.Id);
            var diagnosticsAndSources = DiagnosticsAndSources.Create(expectedDiagnostic, before);
            VerifyAnalyzerSupportsDiagnostics(analyzer, diagnosticsAndSources.ExpectedDiagnostics);
            VerifyCodeFixSupportsAnalyzer(analyzer, fix);
            var sln = CodeFactory.CreateSolution(
                analyzer,
                diagnosticsAndSources,
                settings: settings);
            var diagnostics = Analyze.GetDiagnostics(analyzer, sln);
            VerifyDiagnostics(diagnosticsAndSources, diagnostics);
            FixAllByScope(analyzer, fix, sln, after, fixTitle, settings.AllowCompilationDiagnostics, scope);
        }

        /// <summary>
        /// Verifies that
        /// 1. <paramref name="before"/> produces the expected diagnostics
        /// 2. The code fix fixes the code.
        /// </summary>
        /// <param name="analyzer">The <see cref="DiagnosticAnalyzer"/> to check <paramref name="before"/> with.</param>
        /// <param name="fix">The <see cref="CodeFixProvider"/> to apply on the <see cref="Diagnostic"/> reported.</param>
        /// <param name="before">The code to analyze with <paramref name="analyzer"/>. Indicate error position with ↓ (alt + 25).</param>
        /// <param name="after">The expected code produced by applying <paramref name="fix"/>.</param>
        /// <param name="scope">The scope to apply fixes for.</param>
        /// <param name="fixTitle">The expected title of the fix. Must be provided if more than one code action is registered.</param>
        /// <param name="settings">The <see cref="Settings"/>.</param>
        public static void FixAllByScope(
            DiagnosticAnalyzer analyzer,
            CodeFixProvider fix,
            IReadOnlyList<string> before,
            IReadOnlyList<string> after,
            FixAllScope scope,
            string? fixTitle = null,
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

            if (before is null)
            {
                throw new System.ArgumentNullException(nameof(before));
            }

            if (after is null)
            {
                throw new System.ArgumentNullException(nameof(after));
            }

            settings ??= Settings.Default;
            var diagnosticsAndSources = DiagnosticsAndSources.CreateFromCodeWithErrorsIndicated(analyzer, before);
            VerifyAnalyzerSupportsDiagnostics(analyzer, diagnosticsAndSources.ExpectedDiagnostics);
            VerifyCodeFixSupportsAnalyzer(analyzer, fix);
            var sln = CodeFactory.CreateSolution(
                analyzer,
                diagnosticsAndSources,
                settings: settings);
            var diagnostics = Analyze.GetDiagnostics(analyzer, sln);
            VerifyDiagnostics(diagnosticsAndSources, diagnostics);
            FixAllByScope(analyzer, fix, sln, after, fixTitle, settings.AllowCompilationDiagnostics, scope);
        }

        /// <summary>
        /// Verifies that
        /// 1. <paramref name="before"/> produces the expected diagnostics
        /// 2. The code fix fixes the code.
        /// </summary>
        /// <param name="analyzer">The <see cref="DiagnosticAnalyzer"/> to check <paramref name="before"/> with.</param>
        /// <param name="fix">The <see cref="CodeFixProvider"/> to apply on the <see cref="Diagnostic"/> reported.</param>
        /// <param name="expectedDiagnostic">The <see cref="ExpectedDiagnostic"/> with information about the expected <see cref="Diagnostic"/>. If <paramref name="analyzer"/> supports more than one <see cref="DiagnosticDescriptor.Id"/> this must be provided.</param>
        /// <param name="before">The code to analyze with <paramref name="analyzer"/>. Indicate error position with ↓ (alt + 25).</param>
        /// <param name="after">The expected code produced by applying <paramref name="fix"/>.</param>
        /// <param name="scope">The scope to apply fixes for.</param>
        /// <param name="fixTitle">The expected title of the fix. Must be provided if more than one code action is registered.</param>
        /// <param name="settings">The <see cref="Settings"/>.</param>
        public static void FixAllByScope(
            DiagnosticAnalyzer analyzer,
            CodeFixProvider fix,
            ExpectedDiagnostic expectedDiagnostic,
            IReadOnlyList<string> before,
            IReadOnlyList<string> after,
            FixAllScope scope,
            string? fixTitle = null,
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

            if (before is null)
            {
                throw new System.ArgumentNullException(nameof(before));
            }

            if (after is null)
            {
                throw new System.ArgumentNullException(nameof(after));
            }

            settings ??= Settings.Default;
            var diagnosticsAndSources = DiagnosticsAndSources.Create(expectedDiagnostic, before);
            VerifyAnalyzerSupportsDiagnostics(analyzer, diagnosticsAndSources.ExpectedDiagnostics);
            VerifyCodeFixSupportsAnalyzer(analyzer, fix);
            var sln = CodeFactory.CreateSolution(
                analyzer,
                diagnosticsAndSources,
                settings: settings);
            var diagnostics = Analyze.GetDiagnostics(analyzer, sln);
            VerifyDiagnostics(diagnosticsAndSources, diagnostics);
            FixAllByScope(analyzer, fix, sln, after, fixTitle, settings.AllowCompilationDiagnostics, scope);
        }

        private static void FixAllOneByOne(DiagnosticAnalyzer analyzer, CodeFixProvider fix, Solution solution, IReadOnlyList<string> after, string? fixTitle, AllowCompilationDiagnostics allowCompilationDiagnostics)
        {
            var fixedSolution = Fix.ApplyAllFixableOneByOneAsync(solution, analyzer, fix, fixTitle, CancellationToken.None).GetAwaiter().GetResult();
            AreEqualAsync(after, fixedSolution, "Applying fixes one by one failed.").GetAwaiter().GetResult();
            if (allowCompilationDiagnostics == AllowCompilationDiagnostics.None)
            {
                VerifyNoCompilerErrorsAsync(fix, fixedSolution).ConfigureAwait(false).GetAwaiter().GetResult();
            }
        }

        private static void FixAllByScope(DiagnosticAnalyzer analyzer, CodeFixProvider fix, Solution sln, IReadOnlyList<string> after, string? fixTitle, AllowCompilationDiagnostics allowCompilationDiagnostics, FixAllScope scope)
        {
            VerifyCodeFixSupportsAnalyzer(analyzer, fix);
            var fixedSolution = Fix.ApplyAllFixableScopeByScopeAsync(sln, analyzer, fix, scope, fixTitle, CancellationToken.None).GetAwaiter().GetResult();
            AreEqualAsync(after, fixedSolution, $"Applying fixes for {scope} failed.").GetAwaiter().GetResult();
            if (allowCompilationDiagnostics == AllowCompilationDiagnostics.None)
            {
                VerifyNoCompilerErrorsAsync(fix, fixedSolution).GetAwaiter().GetResult();
            }
        }

        private static List<string> MergeFixedCode(IReadOnlyList<string> codes, string after)
        {
            var merged = new List<string>(codes.Count);
            var found = false;
            foreach (var code in codes)
            {
                if (code.IndexOf('↓') >= 0)
                {
                    if (found)
                    {
                        throw new AssertException("Expected only one with errors indicated.");
                    }

                    merged.Add(after);
                    found = true;
                }
                else
                {
                    merged.Add(code);
                }
            }

            if (found)
            {
                return merged;
            }

            merged.Clear();
            var @namespace = CodeReader.Namespace(after);
            var fileName = CodeReader.FileName(after);
            foreach (var code in codes)
            {
                if (CodeReader.FileName(code) == fileName &&
                    CodeReader.Namespace(code) == @namespace)
                {
                    if (found)
                    {
                        throw new AssertException("Expected unique class names.");
                    }

                    merged.Add(after);
                    found = true;
                }
                else
                {
                    merged.Add(code);
                }
            }

            if (!found)
            {
                throw new AssertException("Failed merging expected one class to have same namespace and class name as after.\r\n" +
                                             "Try specifying a list with all fixed code.");
            }

            return merged;
        }
    }
}
