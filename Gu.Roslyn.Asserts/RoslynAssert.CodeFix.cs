namespace Gu.Roslyn.Asserts
{
    using System;
    using System.Collections.Generic;
    using System.Collections.Immutable;
    using System.Linq;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CodeFixes;
    using Microsoft.CodeAnalysis.CSharp;
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
        /// <param name="before">The code with error positions indicated.</param>
        /// <param name="after">The expected code produced by the code fix.</param>
        /// <param name="fixTitle">The title of the fix to apply if more than one.</param>
         /// <param name="allowCompilationErrors">Specify if compilation errors are accepted in the fixed code. This can be for example syntax errors. Default value is <see cref="AllowCompilationErrors.No"/>.</param>
        /// <param name="suppressedDiagnostics">A collection of <see cref="DiagnosticDescriptor.Id"/> to suppress when analyzing the code. Default is <see langword="null" /> meaning no warnings or errors are suppressed.</param>
        /// <param name="metadataReferences">Collection of <see cref="MetadataReference"/> to use when compiling.</param>
        /// <param name="compilationOptions">The <see cref="CSharpCompilationOptions"/>.</param>
        public static void CodeFix(
            DiagnosticAnalyzer analyzer,
            CodeFixProvider fix,
            string before,
            string after,
            string fixTitle = null,
            AllowCompilationErrors allowCompilationErrors = AllowCompilationErrors.No,
            IEnumerable<string> suppressedDiagnostics = null,
            IEnumerable<MetadataReference> metadataReferences = null,
            CSharpCompilationOptions compilationOptions = null)
        {
            CodeFix(
                analyzer: analyzer,
                fix: fix,
                diagnosticsAndSources: DiagnosticsAndSources.CreateFromCodeWithErrorsIndicated(analyzer, before),
                after: new[] { after },
                fixTitle: fixTitle,
                allowCompilationErrors: allowCompilationErrors,
                suppressedDiagnostics: suppressedDiagnostics,
                metadataReferences: metadataReferences,
                compilationOptions: compilationOptions);
        }

        /// <summary>
        /// Verifies that
        /// 1. <paramref name="fix"/> supports fixing diagnostics reported by <paramref name="analyzer"/>.
        /// 2. <paramref name="before"/> produces diagnostics fixable by <paramref name="fix"/>.
        /// 3. Applying <paramref name="fix"/> results in <paramref name="after"/>.
        /// </summary>
        /// <param name="analyzer">The <see cref="DiagnosticAnalyzer"/> to check <paramref name="before"/> with.</param>
        /// <param name="fix">The <see cref="CodeFixProvider"/> to apply on the <see cref="Diagnostic"/> reported.</param>
        /// <param name="before">The code to analyze.</param>
        /// <param name="after">The expected code produced by the code fix.</param>
        /// <param name="fixTitle">The title of the fix to apply if more than one.</param>
         /// <param name="allowCompilationErrors">Specify if compilation errors are accepted in the fixed code. This can be for example syntax errors. Default value is <see cref="AllowCompilationErrors.No"/>.</param>
        /// <param name="suppressedDiagnostics">A collection of <see cref="DiagnosticDescriptor.Id"/> to suppress when analyzing the code. Default is <see langword="null" /> meaning no warnings or errors are suppressed.</param>
        /// <param name="metadataReferences">Collection of <see cref="MetadataReference"/> to use when compiling.</param>
        /// <param name="compilationOptions">The <see cref="CSharpCompilationOptions"/>.</param>
        public static void CodeFix(
            DiagnosticAnalyzer analyzer,
            CodeFixProvider fix,
            IReadOnlyList<string> before,
            string after,
            string fixTitle = null,
            AllowCompilationErrors allowCompilationErrors = AllowCompilationErrors.No,
            IEnumerable<string> suppressedDiagnostics = null,
            IEnumerable<MetadataReference> metadataReferences = null,
            CSharpCompilationOptions compilationOptions = null)
        {
            CodeFix(
                analyzer: analyzer,
                fix: fix,
                diagnosticsAndSources: DiagnosticsAndSources.CreateFromCodeWithErrorsIndicated(analyzer, before),
                after: MergeFixedCode(before, after),
                fixTitle: fixTitle,
                allowCompilationErrors: allowCompilationErrors,
                suppressedDiagnostics: suppressedDiagnostics,
                metadataReferences: metadataReferences,
                compilationOptions: compilationOptions);
        }

        /// <summary>
        /// Verifies that
        /// 1. <paramref name="fix"/> supports fixing diagnostics reported by <paramref name="analyzer"/>.
        /// 2. <paramref name="before"/> produces diagnostics fixable by <paramref name="fix"/>.
        /// 3. Applying <paramref name="fix"/> results in <paramref name="after"/>.
        /// </summary>
        /// <param name="analyzer">The <see cref="DiagnosticAnalyzer"/> to check <paramref name="before"/> with.</param>
        /// <param name="fix">The <see cref="CodeFixProvider"/> to apply on the <see cref="Diagnostic"/> reported.</param>
        /// <param name="before">The code to analyze.</param>
        /// <param name="after">The expected code produced by the code fix.</param>
        /// <param name="fixTitle">The title of the fix to apply if more than one.</param>
        /// <param name="allowCompilationErrors">Specify if compilation errors are accepted in the fixed code. This can be for example syntax errors. Default value is <see cref="AllowCompilationErrors.No"/>.</param>
        /// <param name="suppressedDiagnostics">A collection of <see cref="DiagnosticDescriptor.Id"/> to suppress when analyzing the code. Default is <see langword="null" /> meaning no warnings or errors are suppressed.</param>
        /// <param name="metadataReferences">Collection of <see cref="MetadataReference"/> to use when compiling.</param>
        /// <param name="compilationOptions">The <see cref="CSharpCompilationOptions"/>.</param>
        public static void CodeFix(
            DiagnosticAnalyzer analyzer,
            CodeFixProvider fix,
            IReadOnlyList<string> before,
            IReadOnlyList<string> after,
            string fixTitle = null,
            AllowCompilationErrors allowCompilationErrors = AllowCompilationErrors.No,
            IEnumerable<string> suppressedDiagnostics = null,
            IEnumerable<MetadataReference> metadataReferences = null,
            CSharpCompilationOptions compilationOptions = null)
        {
            CodeFix(
                analyzer: analyzer,
                fix: fix,
                diagnosticsAndSources: DiagnosticsAndSources.CreateFromCodeWithErrorsIndicated(analyzer, before),
                after: after,
                fixTitle: fixTitle,
                allowCompilationErrors: allowCompilationErrors,
                suppressedDiagnostics: suppressedDiagnostics,
                metadataReferences: metadataReferences,
                compilationOptions: compilationOptions);
        }

        /// <summary>
        /// Verifies that
        /// 1. <paramref name="fix"/> supports fixing diagnostics reported by <paramref name="analyzer"/>.
        /// 2. <paramref name="before"/> produces diagnostics fixable by <paramref name="fix"/>.
        /// 3. Applying <paramref name="fix"/> results in <paramref name="after"/>.
        /// </summary>
        /// <param name="analyzer">The <see cref="DiagnosticAnalyzer"/> to check <paramref name="before"/> with.</param>
        /// <param name="fix">The <see cref="CodeFixProvider"/> to apply on the <see cref="Diagnostic"/> reported.</param>
        /// <param name="expectedDiagnostic">The expected diagnostic.</param>
        /// <param name="before">The code with error positions indicated.</param>
        /// <param name="after">The expected code produced by the code fix.</param>
        /// <param name="fixTitle">The title of the fix to apply if more than one.</param>
         /// <param name="allowCompilationErrors">Specify if compilation errors are accepted in the fixed code. This can be for example syntax errors. Default value is <see cref="AllowCompilationErrors.No"/>.</param>
        /// <param name="suppressedDiagnostics">A collection of <see cref="DiagnosticDescriptor.Id"/> to suppress when analyzing the code. Default is <see langword="null" /> meaning no warnings or errors are suppressed.</param>
        /// <param name="metadataReferences">Collection of <see cref="MetadataReference"/> to use when compiling.</param>
        /// <param name="compilationOptions">The <see cref="CSharpCompilationOptions"/>.</param>
        public static void CodeFix(
            DiagnosticAnalyzer analyzer,
            CodeFixProvider fix,
            ExpectedDiagnostic expectedDiagnostic,
            string before,
            string after,
            string fixTitle = null,
            AllowCompilationErrors allowCompilationErrors = AllowCompilationErrors.No,
            IEnumerable<string> suppressedDiagnostics = null,
            IEnumerable<MetadataReference> metadataReferences = null,
            CSharpCompilationOptions compilationOptions = null)
        {
            CodeFix(
                analyzer: analyzer,
                fix: fix,
                diagnosticsAndSources: DiagnosticsAndSources.Create(expectedDiagnostic, before),
                after: new[] { after },
                fixTitle: fixTitle,
                allowCompilationErrors: allowCompilationErrors,
                suppressedDiagnostics: suppressedDiagnostics,
                metadataReferences: metadataReferences,
                compilationOptions: compilationOptions);
        }

        /// <summary>
        /// Verifies that
        /// 1. <paramref name="fix"/> supports fixing diagnostics reported by <paramref name="analyzer"/>.
        /// 2. <paramref name="before"/> produces diagnostics fixable by <paramref name="fix"/>.
        /// 3. Applying <paramref name="fix"/> results in <paramref name="after"/>.
        /// </summary>
        /// <param name="analyzer">The <see cref="DiagnosticAnalyzer"/> to check <paramref name="before"/> with.</param>
        /// <param name="fix">The <see cref="CodeFixProvider"/> to apply on the <see cref="Diagnostic"/> reported.</param>
        /// <param name="expectedDiagnostic">The expected diagnostic.</param>
        /// <param name="before">The code with error positions indicated.</param>
        /// <param name="after">The expected code produced by the code fix.</param>
        /// <param name="fixTitle">The title of the fix to apply if more than one.</param>
         /// <param name="allowCompilationErrors">Specify if compilation errors are accepted in the fixed code. This can be for example syntax errors. Default value is <see cref="AllowCompilationErrors.No"/>.</param>
        /// <param name="suppressedDiagnostics">A collection of <see cref="DiagnosticDescriptor.Id"/> to suppress when analyzing the code. Default is <see langword="null" /> meaning no warnings or errors are suppressed.</param>
        /// <param name="metadataReferences">Collection of <see cref="MetadataReference"/> to use when compiling.</param>
        /// <param name="compilationOptions">The <see cref="CSharpCompilationOptions"/>.</param>
        public static void CodeFix(
            DiagnosticAnalyzer analyzer,
            CodeFixProvider fix,
            ExpectedDiagnostic expectedDiagnostic,
            IReadOnlyList<string> before,
            string after,
            string fixTitle = null,
            AllowCompilationErrors allowCompilationErrors = AllowCompilationErrors.No,
            IEnumerable<string> suppressedDiagnostics = null,
            IEnumerable<MetadataReference> metadataReferences = null,
            CSharpCompilationOptions compilationOptions = null)
        {
            CodeFix(
                analyzer: analyzer,
                fix: fix,
                diagnosticsAndSources: DiagnosticsAndSources.Create(expectedDiagnostic, before),
                after: MergeFixedCode(before, after),
                fixTitle: fixTitle,
                allowCompilationErrors: allowCompilationErrors,
                suppressedDiagnostics: suppressedDiagnostics,
                metadataReferences: metadataReferences,
                compilationOptions: compilationOptions);
        }

        /// <summary>
        /// Verifies that
        /// 1. <paramref name="fix"/> supports fixing diagnostics reported by <paramref name="analyzer"/>.
        /// 2. <paramref name="before"/> produces diagnostics fixable by <paramref name="fix"/>.
        /// 3. Applying <paramref name="fix"/> results in <paramref name="after"/>.
        /// </summary>
        /// <param name="analyzer">The <see cref="DiagnosticAnalyzer"/> to check <paramref name="before"/> with.</param>
        /// <param name="fix">The <see cref="CodeFixProvider"/> to apply on the <see cref="Diagnostic"/> reported.</param>
        /// <param name="expectedDiagnostic">The expected diagnostic.</param>
        /// <param name="before">The code with error positions indicated.</param>
        /// <param name="after">The expected code produced by the code fix.</param>
        /// <param name="fixTitle">The title of the fix to apply if more than one.</param>
         /// <param name="allowCompilationErrors">Specify if compilation errors are accepted in the fixed code. This can be for example syntax errors. Default value is <see cref="AllowCompilationErrors.No"/>.</param>
        /// <param name="suppressedDiagnostics">A collection of <see cref="DiagnosticDescriptor.Id"/> to suppress when analyzing the code. Default is <see langword="null" /> meaning no warnings or errors are suppressed.</param>
        /// <param name="metadataReferences">Collection of <see cref="MetadataReference"/> to use when compiling.</param>
        /// <param name="compilationOptions">The <see cref="CSharpCompilationOptions"/>.</param>
        public static void CodeFix(
            DiagnosticAnalyzer analyzer,
            CodeFixProvider fix,
            ExpectedDiagnostic expectedDiagnostic,
            IReadOnlyList<string> before,
            IReadOnlyList<string> after,
            string fixTitle = null,
            AllowCompilationErrors allowCompilationErrors = AllowCompilationErrors.No,
            IEnumerable<string> suppressedDiagnostics = null,
            IEnumerable<MetadataReference> metadataReferences = null,
            CSharpCompilationOptions compilationOptions = null)
        {
            CodeFix(
                analyzer: analyzer,
                fix: fix,
                diagnosticsAndSources: DiagnosticsAndSources.Create(expectedDiagnostic, before),
                after: after,
                fixTitle: fixTitle,
                allowCompilationErrors: allowCompilationErrors,
                suppressedDiagnostics: suppressedDiagnostics,
                metadataReferences: metadataReferences,
                compilationOptions: compilationOptions);
        }

        /// <summary>
        /// Verifies that
        /// 1. <paramref name="fix"/> supports fixing <paramref name="expectedDiagnostic"/>.
        /// 2. <paramref name="before"/> produces diagnostics fixable by <paramref name="fix"/>.
        /// 3. Applying <paramref name="fix"/> results in <paramref name="after"/>.
        /// </summary>
        /// <param name="fix">The <see cref="CodeFixProvider"/> to apply on the <see cref="Diagnostic"/> reported.</param>
        /// <param name="expectedDiagnostic">The expected diagnostic.</param>
        /// <param name="before">The code to analyze.</param>
        /// <param name="after">The expected code produced by the code fix.</param>
        /// <param name="fixTitle">The title of the fix to apply if more than one.</param>
         /// <param name="allowCompilationErrors">Specify if compilation errors are accepted in the fixed code. This can be for example syntax errors. Default value is <see cref="AllowCompilationErrors.No"/>.</param>
        /// <param name="suppressedDiagnostics">A collection of <see cref="DiagnosticDescriptor.Id"/> to suppress when analyzing the code. Default is <see langword="null" /> meaning no warnings or errors are suppressed.</param>
        /// <param name="metadataReferences">Collection of <see cref="MetadataReference"/> to use when compiling.</param>
        /// <param name="compilationOptions">The <see cref="CSharpCompilationOptions"/>.</param>
        public static void CodeFix(
            CodeFixProvider fix,
            ExpectedDiagnostic expectedDiagnostic,
            string before,
            string after,
            string fixTitle = null,
            AllowCompilationErrors allowCompilationErrors = AllowCompilationErrors.No,
            IEnumerable<string> suppressedDiagnostics = null,
            IEnumerable<MetadataReference> metadataReferences = null,
            CSharpCompilationOptions compilationOptions = null)
        {
            CodeFix(
                analyzer: new PlaceholderAnalyzer(expectedDiagnostic.Id),
                fix: fix,
                diagnosticsAndSources: DiagnosticsAndSources.Create(expectedDiagnostic, before),
                after: new[] { after },
                fixTitle: fixTitle,
                allowCompilationErrors: allowCompilationErrors,
                suppressedDiagnostics: suppressedDiagnostics,
                metadataReferences: metadataReferences,
                compilationOptions: compilationOptions);
        }

        /// <summary>
        /// Verifies that
        /// 1. <paramref name="fix"/> supports fixing <paramref name="expectedDiagnostic"/>.
        /// 2. <paramref name="before"/> produces diagnostics fixable by <paramref name="fix"/>.
        /// 3. Applying <paramref name="fix"/> results in <paramref name="after"/>.
        /// </summary>
        /// <param name="fix">The <see cref="CodeFixProvider"/> to apply on the <see cref="Diagnostic"/> reported.</param>
        /// <param name="expectedDiagnostic">The expected diagnostic.</param>
        /// <param name="before">The code to analyze.</param>
        /// <param name="after">The expected code produced by the code fix.</param>
        /// <param name="fixTitle">The title of the fix to apply if more than one.</param>
         /// <param name="allowCompilationErrors">Specify if compilation errors are accepted in the fixed code. This can be for example syntax errors. Default value is <see cref="AllowCompilationErrors.No"/>.</param>
        /// <param name="suppressedDiagnostics">A collection of <see cref="DiagnosticDescriptor.Id"/> to suppress when analyzing the code. Default is <see langword="null" /> meaning no warnings or errors are suppressed.</param>
        /// <param name="metadataReferences">Collection of <see cref="MetadataReference"/> to use when compiling.</param>
        /// <param name="compilationOptions">The <see cref="CSharpCompilationOptions"/>.</param>
        public static void CodeFix(
            CodeFixProvider fix,
            ExpectedDiagnostic expectedDiagnostic,
            IReadOnlyList<string> before,
            string after,
            string fixTitle = null,
            AllowCompilationErrors allowCompilationErrors = AllowCompilationErrors.No,
            IEnumerable<string> suppressedDiagnostics = null,
            IEnumerable<MetadataReference> metadataReferences = null,
            CSharpCompilationOptions compilationOptions = null)
        {
            CodeFix(
                analyzer: new PlaceholderAnalyzer(expectedDiagnostic.Id),
                fix: fix,
                diagnosticsAndSources: DiagnosticsAndSources.Create(expectedDiagnostic, before),
                after: MergeFixedCode(before, after),
                fixTitle: fixTitle,
                allowCompilationErrors: allowCompilationErrors,
                suppressedDiagnostics: suppressedDiagnostics,
                metadataReferences: metadataReferences,
                compilationOptions: compilationOptions);
        }

        /// <summary>
        /// Verifies that
        /// 1. <paramref name="fix"/> supports fixing <paramref name="expectedDiagnostic"/>.
        /// 2. <paramref name="before"/> produces diagnostics fixable by <paramref name="fix"/>.
        /// 3. Applying <paramref name="fix"/> results in <paramref name="after"/>.
        /// </summary>
        /// <param name="fix">The <see cref="CodeFixProvider"/> to apply on the <see cref="Diagnostic"/> reported.</param>
        /// <param name="expectedDiagnostic">The expected diagnostic.</param>
        /// <param name="before">The code to analyze.</param>
        /// <param name="after">The expected code produced by the code fix.</param>
        /// <param name="fixTitle">The title of the fix to apply if more than one.</param>
         /// <param name="allowCompilationErrors">Specify if compilation errors are accepted in the fixed code. This can be for example syntax errors. Default value is <see cref="AllowCompilationErrors.No"/>.</param>
        /// <param name="suppressedDiagnostics">A collection of <see cref="DiagnosticDescriptor.Id"/> to suppress when analyzing the code. Default is <see langword="null" /> meaning no warnings or errors are suppressed.</param>
        /// <param name="metadataReferences">Collection of <see cref="MetadataReference"/> to use when compiling.</param>
        /// <param name="compilationOptions">The <see cref="CSharpCompilationOptions"/>.</param>
        public static void CodeFix(
            CodeFixProvider fix,
            ExpectedDiagnostic expectedDiagnostic,
            IReadOnlyList<string> before,
            IReadOnlyList<string> after,
            string fixTitle = null,
            AllowCompilationErrors allowCompilationErrors = AllowCompilationErrors.No,
            IEnumerable<string> suppressedDiagnostics = null,
            IEnumerable<MetadataReference> metadataReferences = null,
            CSharpCompilationOptions compilationOptions = null)
        {
            CodeFix(
                analyzer: new PlaceholderAnalyzer(expectedDiagnostic.Id),
                fix: fix,
                diagnosticsAndSources: DiagnosticsAndSources.Create(expectedDiagnostic, before),
                after: after,
                fixTitle: fixTitle,
                allowCompilationErrors: allowCompilationErrors,
                suppressedDiagnostics: suppressedDiagnostics,
                metadataReferences: metadataReferences,
                compilationOptions: compilationOptions);
        }

        /// <summary>
        /// Verifies that
        /// 1. <paramref name="solution"/> produces the expected diagnostics
        /// 2. The code fix fixes the code.
        /// </summary>
        /// <param name="analyzer">The <see cref="DiagnosticAnalyzer"/> to check <paramref name="solution"/> with.</param>
        /// <param name="fix">The <see cref="CodeFixProvider"/> to apply on the <see cref="Diagnostic"/> reported.</param>
        /// <param name="expectedDiagnostic">The expected diagnostic.</param>
        /// <param name="solution">The code with error positions indicated.</param>
        /// <param name="after">The expected code produced by the code fix.</param>
        /// <param name="fixTitle">The title of the fix to apply if more than one.</param>
        /// <param name="allowCompilationErrors">Specify if compilation errors are accepted in the fixed code. This can be for example syntax errors. Default value is <see cref="AllowCompilationErrors.No"/>.</param>
        public static void CodeFix(
            DiagnosticAnalyzer analyzer,
            CodeFixProvider fix,
            ExpectedDiagnostic expectedDiagnostic,
            Solution solution,
            string after,
            string fixTitle = null,
            AllowCompilationErrors allowCompilationErrors = AllowCompilationErrors.No)
        {
            VerifyAnalyzerSupportsDiagnostic(analyzer, expectedDiagnostic);
            VerifyCodeFixSupportsAnalyzer(analyzer, fix);
            var diagnostics = Analyze.GetDiagnostics(analyzer, solution);
            var diagnosticsAndSources = DiagnosticsAndSources.Create(
                expectedDiagnostic,
                solution.Projects.SelectMany(x => x.Documents).Select(x => x.GetCode(null)).ToArray());
            VerifyDiagnostics(diagnosticsAndSources, diagnostics);
            VerifyFix(solution, diagnostics, analyzer, fix, MergeFixedCode(diagnosticsAndSources.Code, after), fixTitle, allowCompilationErrors);
        }

        private static void CodeFix(
            DiagnosticAnalyzer analyzer,
            CodeFixProvider fix,
            DiagnosticsAndSources diagnosticsAndSources,
            IReadOnlyList<string> after,
            string fixTitle = null,
            AllowCompilationErrors allowCompilationErrors = AllowCompilationErrors.No,
            IEnumerable<string> suppressedDiagnostics = null,
            IEnumerable<MetadataReference> metadataReferences = null,
            CSharpCompilationOptions compilationOptions = null)
        {
            VerifyCodeFixSupportsAnalyzer(analyzer, fix);
            VerifyAnalyzerSupportsDiagnostics(analyzer, diagnosticsAndSources.ExpectedDiagnostics);
            var sln = CodeFactory.CreateSolution(
                diagnosticsAndSources: diagnosticsAndSources,
                analyzer: analyzer,
                compilationOptions: compilationOptions,
                suppressedDiagnostics: suppressedDiagnostics ?? SuppressedDiagnostics,
                metadataReferences: metadataReferences ?? MetadataReferences);
            var diagnostics = Analyze.GetDiagnostics(analyzer, sln);
            VerifyDiagnostics(diagnosticsAndSources, diagnostics);
            VerifyFix(sln, diagnostics, analyzer, fix, after, fixTitle, allowCompilationErrors);
        }

        private static void VerifyFix(Solution sln, IReadOnlyList<ImmutableArray<Diagnostic>> diagnostics, DiagnosticAnalyzer analyzer, CodeFixProvider fix, IReadOnlyList<string> after, string fixTitle = null, AllowCompilationErrors allowCompilationErrors = AllowCompilationErrors.No)
        {
            var fixableDiagnostics = diagnostics.SelectMany(x => x)
                                                .Where(x => fix.FixableDiagnosticIds.Contains(x.Id))
                                                .ToArray();
            if (fixableDiagnostics.Length == 0)
            {
                var message = $"Code analyzed with {analyzer} did not generate any diagnostics fixable by {fix}.{Environment.NewLine}" +
                              $"The analyzed code contained the following diagnostics: {{{string.Join(", ", diagnostics.SelectMany(x => x).Select(d => d.Id))}}}{Environment.NewLine}" +
                              $"The code fix supports the following diagnostics: {{{string.Join(", ", fix.FixableDiagnosticIds)}}}";
                throw new AssertException(message);
            }

            if (fixableDiagnostics.Length > 1)
            {
                var message = $"Code analyzed with {analyzer} generated more than one diagnostic fixable by {fix}.{Environment.NewLine}" +
                              $"The analyzed code contained the following diagnostics: {{{string.Join(", ", diagnostics.SelectMany(x => x).Select(d => d.Id))}}}{Environment.NewLine}" +
                              $"The code fix supports the following diagnostics: {{{string.Join(", ", fix.FixableDiagnosticIds)}}}{Environment.NewLine}" +
                              $"Maybe you meant to call AnalyzerAssert.FixAll?";
                throw new AssertException(message);
            }

            var diagnostic = fixableDiagnostics.Single();
            var fixedSolution = Fix.Apply(sln, fix, diagnostic, fixTitle);
            if (ReferenceEquals(sln, fixedSolution))
            {
                throw new AssertException($"{fix} did not change any document.");
            }

            AreEqualAsync(after, fixedSolution, null).GetAwaiter().GetResult();
            if (allowCompilationErrors == AllowCompilationErrors.No)
            {
                VerifyNoCompilerErrorsAsync(fix, fixedSolution).GetAwaiter().GetResult();
            }
        }
    }
}
