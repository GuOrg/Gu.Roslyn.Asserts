namespace Gu.Roslyn.Asserts
{
    using System;
    using System.Collections.Generic;
    using System.Collections.Immutable;
    using System.Linq;

    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CodeFixes;
    using Microsoft.CodeAnalysis.Diagnostics;

    public static partial class RoslynAssert
    {
        /// <summary>
        /// Verifies that
        /// 1. <paramref name="fix"/> supports fixing diagnostics reported by <paramref name="analyzer"/>.
        /// 2. <paramref name="before"/> produces diagnostics fixable by <paramref name="fix"/>.
        /// 3. Applying <paramref name="fix"/> results in <paramref name="after"/>.
        /// </summary>
        /// <param name="analyzer">The <see cref="DiagnosticAnalyzer"/> to check <paramref name="before"/> with.</param>
        /// <param name="fix">The <see cref="CodeFixProvider"/> to apply on the <see cref="Diagnostic"/> reported.</param>
        /// <param name="before">The code to analyze with <paramref name="analyzer"/>. Indicate error position with ↓ (alt + 25).</param>
        /// <param name="after">The expected code produced by applying <paramref name="fix"/>.</param>
        /// <param name="fixTitle">The expected title of the fix. Must be provided if more than one code action is registered.</param>
        /// <param name="settings">The <see cref="Settings"/>.</param>
        public static void CodeFix(
            DiagnosticAnalyzer analyzer,
            CodeFixProvider fix,
            string before,
            string after,
            string? fixTitle = null,
            Settings? settings = null)
        {
            if (analyzer is null)
            {
                throw new ArgumentNullException(nameof(analyzer));
            }

            if (fix is null)
            {
                throw new ArgumentNullException(nameof(fix));
            }

            if (before is null)
            {
                throw new ArgumentNullException(nameof(before));
            }

            if (after is null)
            {
                throw new ArgumentNullException(nameof(after));
            }

            CodeFix(
                analyzer: analyzer,
                fix: fix,
                diagnosticsAndSources: DiagnosticsAndSources.CreateFromCodeWithErrorsIndicated(analyzer, before),
                after: new[] { after },
                fixTitle: fixTitle,
                settings: settings);
        }

        /// <summary>
        /// Verifies that
        /// 1. <paramref name="fix"/> supports fixing diagnostics reported by <paramref name="analyzer"/>.
        /// 2. <paramref name="before"/> produces diagnostics fixable by <paramref name="fix"/>.
        /// 3. Applying <paramref name="fix"/> results in <paramref name="after"/>.
        /// </summary>
        /// <param name="analyzer">The <see cref="DiagnosticAnalyzer"/> to check <paramref name="before"/> with.</param>
        /// <param name="fix">The <see cref="CodeFixProvider"/> to apply on the <see cref="Diagnostic"/> reported.</param>
        /// <param name="before">The code to analyze with <paramref name="analyzer"/>. Indicate error position with ↓ (alt + 25).</param>
        /// <param name="after">The expected code produced by applying <paramref name="fix"/>.</param>
        /// <param name="fixTitle">The expected title of the fix. Must be provided if more than one code action is registered.</param>
        /// <param name="settings">The <see cref="Settings"/>.</param>
        public static void CodeFix(
            DiagnosticAnalyzer analyzer,
            CodeFixProvider fix,
            IReadOnlyList<string> before,
            string after,
            string? fixTitle = null,
            Settings? settings = null)
        {
            if (analyzer is null)
            {
                throw new ArgumentNullException(nameof(analyzer));
            }

            if (fix is null)
            {
                throw new ArgumentNullException(nameof(fix));
            }

            if (before is null)
            {
                throw new ArgumentNullException(nameof(before));
            }

            if (after is null)
            {
                throw new ArgumentNullException(nameof(after));
            }

            CodeFix(
                analyzer: analyzer,
                fix: fix,
                diagnosticsAndSources: DiagnosticsAndSources.CreateFromCodeWithErrorsIndicated(analyzer, before),
                after: MergeFixedCode(before, after),
                fixTitle: fixTitle,
                settings: settings);
        }

        /// <summary>
        /// Verifies that
        /// 1. <paramref name="fix"/> supports fixing diagnostics reported by <paramref name="analyzer"/>.
        /// 2. <paramref name="before"/> produces diagnostics fixable by <paramref name="fix"/>.
        /// 3. Applying <paramref name="fix"/> results in <paramref name="after"/>.
        /// </summary>
        /// <param name="analyzer">The <see cref="DiagnosticAnalyzer"/> to check <paramref name="before"/> with.</param>
        /// <param name="fix">The <see cref="CodeFixProvider"/> to apply on the <see cref="Diagnostic"/> reported.</param>
        /// <param name="before">The code to analyze with <paramref name="analyzer"/>. Indicate error position with ↓ (alt + 25).</param>
        /// <param name="after">The expected code produced by applying <paramref name="fix"/>.</param>
        /// <param name="fixTitle">The expected title of the fix. Must be provided if more than one code action is registered.</param>
        /// <param name="settings">The <see cref="Settings"/>.</param>
        public static void CodeFix(
            DiagnosticAnalyzer analyzer,
            CodeFixProvider fix,
            IReadOnlyList<string> before,
            IReadOnlyList<string> after,
            string? fixTitle = null,
            Settings? settings = null)
        {
            if (analyzer is null)
            {
                throw new ArgumentNullException(nameof(analyzer));
            }

            if (fix is null)
            {
                throw new ArgumentNullException(nameof(fix));
            }

            if (before is null)
            {
                throw new ArgumentNullException(nameof(before));
            }

            if (after is null)
            {
                throw new ArgumentNullException(nameof(after));
            }

            CodeFix(
                analyzer: analyzer,
                fix: fix,
                diagnosticsAndSources: DiagnosticsAndSources.CreateFromCodeWithErrorsIndicated(analyzer, before),
                after: after,
                fixTitle: fixTitle,
                settings: settings);
        }

        /// <summary>
        /// Verifies that
        /// 1. <paramref name="fix"/> supports fixing diagnostics reported by <paramref name="analyzer"/>.
        /// 2. <paramref name="before"/> produces diagnostics fixable by <paramref name="fix"/>.
        /// 3. Applying <paramref name="fix"/> results in <paramref name="after"/>.
        /// </summary>
        /// <param name="analyzer">The <see cref="DiagnosticAnalyzer"/> to check <paramref name="before"/> with.</param>
        /// <param name="fix">The <see cref="CodeFixProvider"/> to apply on the <see cref="Diagnostic"/> reported.</param>
        /// <param name="expectedDiagnostic">The <see cref="ExpectedDiagnostic"/> with information about the expected <see cref="Diagnostic"/>. If <paramref name="analyzer"/> supports more than one <see cref="DiagnosticDescriptor.Id"/> this must be provided.</param>
        /// <param name="before">The code to analyze with <paramref name="analyzer"/>. Indicate error position with ↓ (alt + 25).</param>
        /// <param name="after">The expected code produced by applying <paramref name="fix"/>.</param>
        /// <param name="fixTitle">The expected title of the fix. Must be provided if more than one code action is registered.</param>
        /// <param name="settings">The <see cref="Settings"/>.</param>
        public static void CodeFix(
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
                throw new ArgumentNullException(nameof(analyzer));
            }

            if (fix is null)
            {
                throw new ArgumentNullException(nameof(fix));
            }

            if (expectedDiagnostic is null)
            {
                throw new ArgumentNullException(nameof(expectedDiagnostic));
            }

            if (before is null)
            {
                throw new ArgumentNullException(nameof(before));
            }

            if (after is null)
            {
                throw new ArgumentNullException(nameof(after));
            }

            CodeFix(
                analyzer: analyzer,
                fix: fix,
                diagnosticsAndSources: DiagnosticsAndSources.Create(expectedDiagnostic, before),
                after: new[] { after },
                fixTitle: fixTitle,
                settings: settings);
        }

        /// <summary>
        /// Verifies that
        /// 1. <paramref name="fix"/> supports fixing diagnostics reported by <paramref name="analyzer"/>.
        /// 2. <paramref name="before"/> produces diagnostics fixable by <paramref name="fix"/>.
        /// 3. Applying <paramref name="fix"/> results in <paramref name="after"/>.
        /// </summary>
        /// <param name="analyzer">The <see cref="DiagnosticAnalyzer"/> to check <paramref name="before"/> with.</param>
        /// <param name="fix">The <see cref="CodeFixProvider"/> to apply on the <see cref="Diagnostic"/> reported.</param>
        /// <param name="expectedDiagnostic">The <see cref="ExpectedDiagnostic"/> with information about the expected <see cref="Diagnostic"/>. If <paramref name="analyzer"/> supports more than one <see cref="DiagnosticDescriptor.Id"/> this must be provided.</param>
        /// <param name="before">The code to analyze with <paramref name="analyzer"/>. Indicate error position with ↓ (alt + 25).</param>
        /// <param name="after">The expected code produced by applying <paramref name="fix"/>.</param>
        /// <param name="fixTitle">The expected title of the fix. Must be provided if more than one code action is registered.</param>
        /// <param name="settings">The <see cref="Settings"/>.</param>
        public static void CodeFix(
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
                throw new ArgumentNullException(nameof(analyzer));
            }

            if (fix is null)
            {
                throw new ArgumentNullException(nameof(fix));
            }

            if (expectedDiagnostic is null)
            {
                throw new ArgumentNullException(nameof(expectedDiagnostic));
            }

            if (before is null)
            {
                throw new ArgumentNullException(nameof(before));
            }

            if (after is null)
            {
                throw new ArgumentNullException(nameof(after));
            }

            CodeFix(
                analyzer: analyzer,
                fix: fix,
                diagnosticsAndSources: DiagnosticsAndSources.Create(expectedDiagnostic, before),
                after: MergeFixedCode(before, after),
                fixTitle: fixTitle,
                settings: settings);
        }

        /// <summary>
        /// Verifies that
        /// 1. <paramref name="fix"/> supports fixing diagnostics reported by <paramref name="analyzer"/>.
        /// 2. <paramref name="before"/> produces diagnostics fixable by <paramref name="fix"/>.
        /// 3. Applying <paramref name="fix"/> results in <paramref name="after"/>.
        /// </summary>
        /// <param name="analyzer">The <see cref="DiagnosticAnalyzer"/> to check <paramref name="before"/> with.</param>
        /// <param name="fix">The <see cref="CodeFixProvider"/> to apply on the <see cref="Diagnostic"/> reported.</param>
        /// <param name="expectedDiagnostic">The <see cref="ExpectedDiagnostic"/> with information about the expected <see cref="Diagnostic"/>. If <paramref name="analyzer"/> supports more than one <see cref="DiagnosticDescriptor.Id"/> this must be provided.</param>
        /// <param name="before">The code to analyze with <paramref name="analyzer"/>. Indicate error position with ↓ (alt + 25).</param>
        /// <param name="after">The expected code produced by applying <paramref name="fix"/>.</param>
        /// <param name="fixTitle">The expected title of the fix. Must be provided if more than one code action is registered.</param>
        /// <param name="settings">The <see cref="Settings"/>.</param>
        public static void CodeFix(
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
                throw new ArgumentNullException(nameof(analyzer));
            }

            if (fix is null)
            {
                throw new ArgumentNullException(nameof(fix));
            }

            if (expectedDiagnostic is null)
            {
                throw new ArgumentNullException(nameof(expectedDiagnostic));
            }

            if (before is null)
            {
                throw new ArgumentNullException(nameof(before));
            }

            if (after is null)
            {
                throw new ArgumentNullException(nameof(after));
            }

            CodeFix(
                analyzer: analyzer,
                fix: fix,
                diagnosticsAndSources: DiagnosticsAndSources.Create(expectedDiagnostic, before),
                after: after,
                fixTitle: fixTitle,
                settings: settings);
        }

        /// <summary>
        /// Verifies that
        /// 1. <paramref name="fix"/> supports fixing <paramref name="expectedDiagnostic"/>.
        /// 2. <paramref name="before"/> produces diagnostics fixable by <paramref name="fix"/>.
        /// 3. Applying <paramref name="fix"/> results in <paramref name="after"/>.
        /// </summary>
        /// <param name="fix">The <see cref="CodeFixProvider"/> to apply on the <see cref="Diagnostic"/> reported.</param>
        /// <param name="expectedDiagnostic">The <see cref="ExpectedDiagnostic"/> with information about the expected <see cref="Diagnostic"/>.</param>
        /// <param name="before">The code to analyze for <paramref name="expectedDiagnostic"/>. Indicate error position with ↓ (alt + 25).</param>
        /// <param name="after">The expected code produced by applying <paramref name="fix"/>.</param>
        /// <param name="fixTitle">The expected title of the fix. Must be provided if more than one code action is registered.</param>
        /// <param name="settings">The <see cref="Settings"/>.</param>
        public static void CodeFix(
            CodeFixProvider fix,
            ExpectedDiagnostic expectedDiagnostic,
            string before,
            string after,
            string? fixTitle = null,
            Settings? settings = null)
        {
            if (fix is null)
            {
                throw new ArgumentNullException(nameof(fix));
            }

            if (expectedDiagnostic is null)
            {
                throw new ArgumentNullException(nameof(expectedDiagnostic));
            }

            if (before is null)
            {
                throw new ArgumentNullException(nameof(before));
            }

            if (after is null)
            {
                throw new ArgumentNullException(nameof(after));
            }

            CodeFix(
                analyzer: new PlaceholderAnalyzer(expectedDiagnostic.Id),
                fix: fix,
                diagnosticsAndSources: DiagnosticsAndSources.Create(expectedDiagnostic, before),
                after: new[] { after },
                fixTitle: fixTitle,
                settings: settings);
        }

        /// <summary>
        /// Verifies that
        /// 1. <paramref name="fix"/> supports fixing <paramref name="expectedDiagnostic"/>.
        /// 2. <paramref name="before"/> produces diagnostics fixable by <paramref name="fix"/>.
        /// 3. Applying <paramref name="fix"/> results in <paramref name="after"/>.
        /// </summary>
        /// <param name="fix">The <see cref="CodeFixProvider"/> to apply on the <see cref="Diagnostic"/> reported.</param>
        /// <param name="expectedDiagnostic">The <see cref="ExpectedDiagnostic"/> with information about the expected <see cref="Diagnostic"/>.</param>
        /// <param name="before">The code to analyze for <paramref name="expectedDiagnostic"/>. Indicate error position with ↓ (alt + 25).</param>
        /// <param name="after">The expected code produced by applying <paramref name="fix"/>.</param>
        /// <param name="fixTitle">The expected title of the fix. Must be provided if more than one code action is registered.</param>
        /// <param name="settings">The <see cref="Settings"/>.</param>
        public static void CodeFix(
            CodeFixProvider fix,
            ExpectedDiagnostic expectedDiagnostic,
            IReadOnlyList<string> before,
            string after,
            string? fixTitle = null,
            Settings? settings = null)
        {
            if (fix is null)
            {
                throw new ArgumentNullException(nameof(fix));
            }

            if (expectedDiagnostic is null)
            {
                throw new ArgumentNullException(nameof(expectedDiagnostic));
            }

            if (before is null)
            {
                throw new ArgumentNullException(nameof(before));
            }

            if (after is null)
            {
                throw new ArgumentNullException(nameof(after));
            }

            CodeFix(
                analyzer: new PlaceholderAnalyzer(expectedDiagnostic.Id),
                fix: fix,
                diagnosticsAndSources: DiagnosticsAndSources.Create(expectedDiagnostic, before),
                after: MergeFixedCode(before, after),
                fixTitle: fixTitle,
                settings: settings);
        }

        /// <summary>
        /// Verifies that
        /// 1. <paramref name="fix"/> supports fixing <paramref name="expectedDiagnostic"/>.
        /// 2. <paramref name="before"/> produces diagnostics fixable by <paramref name="fix"/>.
        /// 3. Applying <paramref name="fix"/> results in <paramref name="after"/>.
        /// </summary>
        /// <param name="fix">The <see cref="CodeFixProvider"/> to apply on the <see cref="Diagnostic"/> reported.</param>
        /// <param name="expectedDiagnostic">The <see cref="ExpectedDiagnostic"/> with information about the expected <see cref="Diagnostic"/>.</param>
        /// <param name="before">The code to analyze for <paramref name="expectedDiagnostic"/>. Indicate error position with ↓ (alt + 25).</param>
        /// <param name="after">The expected code produced by applying <paramref name="fix"/>.</param>
        /// <param name="fixTitle">The expected title of the fix. Must be provided if more than one code action is registered.</param>
        /// <param name="settings">The <see cref="Settings"/>.</param>
        public static void CodeFix(
            CodeFixProvider fix,
            ExpectedDiagnostic expectedDiagnostic,
            IReadOnlyList<string> before,
            IReadOnlyList<string> after,
            string? fixTitle = null,
            Settings? settings = null)
        {
            if (fix is null)
            {
                throw new ArgumentNullException(nameof(fix));
            }

            if (expectedDiagnostic is null)
            {
                throw new ArgumentNullException(nameof(expectedDiagnostic));
            }

            if (before is null)
            {
                throw new ArgumentNullException(nameof(before));
            }

            if (after is null)
            {
                throw new ArgumentNullException(nameof(after));
            }

            CodeFix(
                analyzer: new PlaceholderAnalyzer(expectedDiagnostic.Id),
                fix: fix,
                diagnosticsAndSources: DiagnosticsAndSources.Create(expectedDiagnostic, before),
                after: after,
                fixTitle: fixTitle,
                settings: settings);
        }

        /// <summary>
        /// Verifies that
        /// 1. <paramref name="solution"/> produces the expected diagnostics
        /// 2. The code fix fixes the code.
        /// </summary>
        /// <param name="analyzer">The <see cref="DiagnosticAnalyzer"/> to check <paramref name="solution"/> with.</param>
        /// <param name="fix">The <see cref="CodeFixProvider"/> to apply on the <see cref="Diagnostic"/> reported.</param>
        /// <param name="expectedDiagnostic">The <see cref="ExpectedDiagnostic"/> with information about the expected <see cref="Diagnostic"/>. If <paramref name="analyzer"/> supports more than one <see cref="DiagnosticDescriptor.Id"/> this must be provided.</param>
        /// <param name="solution">The code to analyze with <paramref name="analyzer"/>. Indicate error position with ↓ (alt + 25).</param>
        /// <param name="after">The expected code produced by applying <paramref name="fix"/>.</param>
        /// <param name="fixTitle">The expected title of the fix. Must be provided if more than one code action is registered.</param>
        /// <param name="allowCompilationErrors">Specify if compilation errors are accepted in the fixed code. This can be for example syntax errors. Default value is <see cref="AllowCompilationErrors.No"/>.</param>
        public static void CodeFix(
            DiagnosticAnalyzer analyzer,
            CodeFixProvider fix,
            ExpectedDiagnostic expectedDiagnostic,
            Solution solution,
            string after,
            string? fixTitle = null,
            AllowCompilationErrors allowCompilationErrors = AllowCompilationErrors.No)
        {
            if (analyzer is null)
            {
                throw new ArgumentNullException(nameof(analyzer));
            }

            if (fix is null)
            {
                throw new ArgumentNullException(nameof(fix));
            }

            if (expectedDiagnostic is null)
            {
                throw new ArgumentNullException(nameof(expectedDiagnostic));
            }

            if (solution is null)
            {
                throw new ArgumentNullException(nameof(solution));
            }

            if (after is null)
            {
                throw new ArgumentNullException(nameof(after));
            }

            VerifyAnalyzerSupportsDiagnostic(analyzer, expectedDiagnostic);
            VerifyCodeFixSupportsAnalyzer(analyzer, fix);
            var diagnostics = Analyze.GetDiagnostics(analyzer, solution);
            var diagnosticsAndSources = DiagnosticsAndSources.Create(
                expectedDiagnostic,
                solution.Projects.SelectMany(x => x.Documents).Select(x => x.GetCode()).ToArray());
            VerifyDiagnostics(diagnosticsAndSources, diagnostics, solution);
            VerifyFix(solution, diagnostics, analyzer, fix, MergeFixedCode(diagnosticsAndSources.Code, after), fixTitle, allowCompilationErrors);
        }

        /// <summary>
        /// Verifies that
        /// 1. <paramref name="fix"/> supports fixing diagnostics reported by <paramref name="analyzer"/>.
        /// 2. <paramref name="diagnosticsAndSources"/> produces diagnostics fixable by <paramref name="fix"/>.
        /// 3. Applying <paramref name="fix"/> results in <paramref name="after"/>.
        /// </summary>
        /// <param name="analyzer">The <see cref="DiagnosticAnalyzer"/> to check <paramref name="diagnosticsAndSources"/> with.</param>
        /// <param name="fix">The <see cref="CodeFixProvider"/> to apply on the <see cref="Diagnostic"/> reported.</param>
        /// <param name="diagnosticsAndSources">The code to analyze with <paramref name="analyzer"/>. Indicate error position with ↓ (alt + 25).</param>
        /// <param name="after">The expected code produced by applying <paramref name="fix"/>.</param>
        /// <param name="fixTitle">The expected title of the fix. Must be provided if more than one code action is registered.</param>
        /// <param name="settings">The <see cref="Settings"/>.</param>
        public static void CodeFix(
            DiagnosticAnalyzer analyzer,
            CodeFixProvider fix,
            DiagnosticsAndSources diagnosticsAndSources,
            IReadOnlyList<string> after,
            string? fixTitle = null,
            Settings? settings = null)
        {
            if (analyzer is null)
            {
                throw new ArgumentNullException(nameof(analyzer));
            }

            if (fix is null)
            {
                throw new ArgumentNullException(nameof(fix));
            }

            if (diagnosticsAndSources is null)
            {
                throw new ArgumentNullException(nameof(diagnosticsAndSources));
            }

            if (after is null)
            {
                throw new ArgumentNullException(nameof(after));
            }

            settings ??= Settings.Default;
            VerifyCodeFixSupportsAnalyzer(analyzer, fix);
            VerifyAnalyzerSupportsDiagnostics(analyzer, diagnosticsAndSources.ExpectedDiagnostics);
            var sln = CodeFactory.CreateSolution(
                diagnosticsAndSources: diagnosticsAndSources,
                analyzer: analyzer,
                settings: settings);
            var diagnostics = Analyze.GetDiagnostics(analyzer, sln);
            VerifyDiagnostics(diagnosticsAndSources, diagnostics, sln);
            VerifyFix(sln, diagnostics, analyzer, fix, after, fixTitle, settings.AllowCompilationErrors);
        }

        private static void VerifyFix(Solution sln, IReadOnlyList<ImmutableArray<Diagnostic>> diagnostics, DiagnosticAnalyzer analyzer, CodeFixProvider fix, IReadOnlyList<string> after, string? fixTitle = null, AllowCompilationErrors allowCompilationErrors = AllowCompilationErrors.No)
        {
            var fixableDiagnostics = diagnostics.SelectMany(x => x)
                                                .Where(x => fix.FixableDiagnosticIds.Contains(x.Id))
                                                .ToArray();
            if (fixableDiagnostics.Length == 0)
            {
                var message = $"Code analyzed with {analyzer.GetType().Name} did not generate any diagnostics fixable by {fix.GetType().Name}.{Environment.NewLine}" +
                              $"The analyzed code contained the following diagnostics: {{{string.Join(", ", diagnostics.SelectMany(x => x).Select(d => d.Id))}}}.{Environment.NewLine}" +
                              $"{fix.GetType().Name}.{nameof(fix.FixableDiagnosticIds)}: {{{string.Join(", ", fix.FixableDiagnosticIds)}}}.";
                throw new AssertException(message);
            }

            var operation = Fix.FindSingleOperation(sln, fix, fixableDiagnostics, fixTitle);
            if (ReferenceEquals(operation.ChangedSolution, sln))
            {
                throw new AssertException($"{fix.GetType().Name} did not change any document.");
            }

            if (allowCompilationErrors == AllowCompilationErrors.No)
            {
                VerifyNoCompilerErrorsAsync(fix, operation.ChangedSolution).GetAwaiter().GetResult();
            }

            AreEqualAsync(after, operation.ChangedSolution, null).GetAwaiter().GetResult();
        }
    }
}
