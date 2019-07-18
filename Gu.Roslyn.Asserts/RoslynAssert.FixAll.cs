namespace Gu.Roslyn.Asserts
{
    using System.Collections.Generic;
    using System.Threading;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CodeFixes;
    using Microsoft.CodeAnalysis.CSharp;
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
         /// <param name="allowCompilationErrors">Specify if compilation errors are accepted in the fixed code. This can be for example syntax errors. Default value is <see cref="AllowCompilationErrors.No"/>.</param>
        /// <param name="suppressWarnings">A collection of <see cref="DiagnosticDescriptor.Id"/> to suppress when analyzing the code. Default is <see langword="null" /> meaning <see cref="SuppressedDiagnostics"/> are used.</param>
        /// <param name="metadataReferences">A collection of <see cref="MetadataReference"/> to use when compiling. Default is <see langword="null" /> meaning <see cref="MetadataReferences"/> are used.</param>
        /// <param name="compilationOptions">The <see cref="CSharpCompilationOptions"/>.</param>
        public static void FixAll(
            DiagnosticAnalyzer analyzer,
            CodeFixProvider fix,
            ExpectedDiagnostic expectedDiagnostic,
            string before,
            string after,
            string fixTitle = null,
            AllowCompilationErrors allowCompilationErrors = AllowCompilationErrors.No,
            IEnumerable<string> suppressWarnings = null,
            IEnumerable<MetadataReference> metadataReferences = null,
            CSharpCompilationOptions compilationOptions = null)
        {
            FixAll(
                analyzer: analyzer,
                fix: fix,
                diagnosticsAndSources: DiagnosticsAndSources.Create(expectedDiagnostic, before),
                after: MergeFixedCode(new[] { before }, after),
                fixTitle: fixTitle,
                allowCompilationErrors: allowCompilationErrors,
                suppressWarnings: suppressWarnings,
                metadataReferences: metadataReferences,
                compilationOptions: compilationOptions);
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
         /// <param name="allowCompilationErrors">Specify if compilation errors are accepted in the fixed code. This can be for example syntax errors. Default value is <see cref="AllowCompilationErrors.No"/>.</param>
        /// <param name="suppressWarnings">A collection of <see cref="DiagnosticDescriptor.Id"/> to suppress when analyzing the code. Default is <see langword="null" /> meaning <see cref="SuppressedDiagnostics"/> are used.</param>
        /// <param name="metadataReferences">A collection of <see cref="MetadataReference"/> to use when compiling. Default is <see langword="null" /> meaning <see cref="MetadataReferences"/> are used.</param>
        /// <param name="compilationOptions">The <see cref="CSharpCompilationOptions"/>.</param>
        public static void FixAll(
            DiagnosticAnalyzer analyzer,
            CodeFixProvider fix,
            ExpectedDiagnostic expectedDiagnostic,
            IReadOnlyList<string> before,
            string after,
            string fixTitle = null,
            AllowCompilationErrors allowCompilationErrors = AllowCompilationErrors.No,
            IEnumerable<string> suppressWarnings = null,
            IEnumerable<MetadataReference> metadataReferences = null,
            CSharpCompilationOptions compilationOptions = null)
        {
            FixAll(
                analyzer: analyzer,
                fix: fix,
                diagnosticsAndSources: DiagnosticsAndSources.Create(expectedDiagnostic, before),
                after: MergeFixedCode(before, after),
                fixTitle: fixTitle,
                allowCompilationErrors: allowCompilationErrors,
                suppressWarnings: suppressWarnings,
                metadataReferences: metadataReferences,
                compilationOptions: compilationOptions);
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
         /// <param name="allowCompilationErrors">Specify if compilation errors are accepted in the fixed code. This can be for example syntax errors. Default value is <see cref="AllowCompilationErrors.No"/>.</param>
        /// <param name="suppressWarnings">A collection of <see cref="DiagnosticDescriptor.Id"/> to suppress when analyzing the code. Default is <see langword="null" /> meaning <see cref="SuppressedDiagnostics"/> are used.</param>
        /// <param name="metadataReferences">A collection of <see cref="MetadataReference"/> to use when compiling. Default is <see langword="null" /> meaning <see cref="MetadataReferences"/> are used.</param>
        /// <param name="compilationOptions">The <see cref="CSharpCompilationOptions"/>.</param>
        public static void FixAll(
            DiagnosticAnalyzer analyzer,
            CodeFixProvider fix,
            ExpectedDiagnostic expectedDiagnostic,
            IReadOnlyList<string> before,
            IReadOnlyList<string> after,
            string fixTitle = null,
            AllowCompilationErrors allowCompilationErrors = AllowCompilationErrors.No,
            IEnumerable<string> suppressWarnings = null,
            IEnumerable<MetadataReference> metadataReferences = null,
            CSharpCompilationOptions compilationOptions = null)
        {
            FixAll(
                analyzer: analyzer,
                fix: fix,
                diagnosticsAndSources: DiagnosticsAndSources.Create(expectedDiagnostic, before),
                after: after,
                fixTitle: fixTitle,
                allowCompilationErrors: allowCompilationErrors,
                suppressWarnings: suppressWarnings,
                metadataReferences: metadataReferences,
                compilationOptions: compilationOptions);
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
         /// <param name="allowCompilationErrors">Specify if compilation errors are accepted in the fixed code. This can be for example syntax errors. Default value is <see cref="AllowCompilationErrors.No"/>.</param>
        /// <param name="suppressWarnings">A collection of <see cref="DiagnosticDescriptor.Id"/> to suppress when analyzing the code. Default is <see langword="null" /> meaning <see cref="SuppressedDiagnostics"/> are used.</param>
        /// <param name="metadataReferences">A collection of <see cref="MetadataReference"/> to use when compiling. Default is <see langword="null" /> meaning <see cref="MetadataReferences"/> are used.</param>
        /// <param name="compilationOptions">The <see cref="CSharpCompilationOptions"/>.</param>
        public static void FixAll(
            CodeFixProvider fix,
            ExpectedDiagnostic expectedDiagnostic,
            string before,
            string after,
            string fixTitle = null,
            AllowCompilationErrors allowCompilationErrors = AllowCompilationErrors.No,
            IEnumerable<string> suppressWarnings = null,
            IEnumerable<MetadataReference> metadataReferences = null,
            CSharpCompilationOptions compilationOptions = null)
        {
            var analyzer = new PlaceholderAnalyzer(expectedDiagnostic.Id);
            FixAll(
                analyzer: analyzer,
                fix: fix,
                diagnosticsAndSources: DiagnosticsAndSources.Create(expectedDiagnostic, before),
                after: new[] { after },
                fixTitle: fixTitle,
                allowCompilationErrors: allowCompilationErrors,
                suppressWarnings: suppressWarnings,
                metadataReferences: metadataReferences,
                compilationOptions: compilationOptions);
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
        /// <param name="allowCompilationErrors">Specify if compilation errors are accepted in the fixed code. This can be for example syntax errors. Default value is <see cref="AllowCompilationErrors.No"/>.</param>
        /// <param name="suppressWarnings">A collection of <see cref="DiagnosticDescriptor.Id"/> to suppress when analyzing the code. Default is <see langword="null" /> meaning <see cref="SuppressedDiagnostics"/> are used.</param>
        /// <param name="metadataReferences">A collection of <see cref="MetadataReference"/> to use when compiling. Default is <see langword="null" /> meaning <see cref="MetadataReferences"/> are used.</param>
        /// <param name="compilationOptions">The <see cref="CSharpCompilationOptions"/>.</param>
        public static void FixAll(
            CodeFixProvider fix,
            ExpectedDiagnostic expectedDiagnostic,
            IReadOnlyList<string> before,
            string after,
            string fixTitle = null,
            AllowCompilationErrors allowCompilationErrors = AllowCompilationErrors.No,
            IEnumerable<string> suppressWarnings = null,
            IEnumerable<MetadataReference> metadataReferences = null,
            CSharpCompilationOptions compilationOptions = null)
        {
            var analyzer = new PlaceholderAnalyzer(expectedDiagnostic.Id);
            FixAll(
                analyzer: analyzer,
                fix: fix,
                diagnosticsAndSources: DiagnosticsAndSources.Create(expectedDiagnostic, before),
                after: MergeFixedCode(before, after),
                fixTitle: fixTitle,
                allowCompilationErrors: allowCompilationErrors,
                suppressWarnings: suppressWarnings,
                metadataReferences: metadataReferences,
                compilationOptions: compilationOptions);
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
        /// <param name="allowCompilationErrors">Specify if compilation errors are accepted in the fixed code. This can be for example syntax errors. Default value is <see cref="AllowCompilationErrors.No"/>.</param>
        /// <param name="suppressWarnings">A collection of <see cref="DiagnosticDescriptor.Id"/> to suppress when analyzing the code. Default is <see langword="null" /> meaning <see cref="SuppressedDiagnostics"/> are used.</param>
        /// <param name="metadataReferences">A collection of <see cref="MetadataReference"/> to use when compiling. Default is <see langword="null" /> meaning <see cref="MetadataReferences"/> are used.</param>
        /// <param name="compilationOptions">The <see cref="CSharpCompilationOptions"/>.</param>
        public static void FixAll(
            CodeFixProvider fix,
            ExpectedDiagnostic expectedDiagnostic,
            IReadOnlyList<string> before,
            IReadOnlyList<string> after,
            string fixTitle = null,
            AllowCompilationErrors allowCompilationErrors = AllowCompilationErrors.No,
            IEnumerable<string> suppressWarnings = null,
            IEnumerable<MetadataReference> metadataReferences = null,
            CSharpCompilationOptions compilationOptions = null)
        {
            var analyzer = new PlaceholderAnalyzer(expectedDiagnostic.Id);
            FixAll(
                analyzer: analyzer,
                fix: fix,
                diagnosticsAndSources: DiagnosticsAndSources.Create(expectedDiagnostic, before),
                after: after,
                fixTitle: fixTitle,
                allowCompilationErrors: allowCompilationErrors,
                suppressWarnings: suppressWarnings,
                metadataReferences: metadataReferences,
                compilationOptions: compilationOptions);
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
         /// <param name="allowCompilationErrors">Specify if compilation errors are accepted in the fixed code. This can be for example syntax errors. Default value is <see cref="AllowCompilationErrors.No"/>.</param>
        /// <param name="suppressWarnings">A collection of <see cref="DiagnosticDescriptor.Id"/> to suppress when analyzing the code. Default is <see langword="null" /> meaning <see cref="SuppressedDiagnostics"/> are used.</param>
        /// <param name="metadataReferences">A collection of <see cref="MetadataReference"/> to use when compiling. Default is <see langword="null" /> meaning <see cref="MetadataReferences"/> are used.</param>
        /// <param name="compilationOptions">The <see cref="CSharpCompilationOptions"/>.</param>
        public static void FixAll(
            DiagnosticAnalyzer analyzer,
            CodeFixProvider fix,
            string before,
            string after,
            string fixTitle = null,
            AllowCompilationErrors allowCompilationErrors = AllowCompilationErrors.No,
            IEnumerable<string> suppressWarnings = null,
            IEnumerable<MetadataReference> metadataReferences = null,
            CSharpCompilationOptions compilationOptions = null)
        {
            FixAll(
                analyzer: analyzer,
                fix: fix,
                diagnosticsAndSources: DiagnosticsAndSources.CreateFromCodeWithErrorsIndicated(analyzer, before),
                after: new[] { after },
                fixTitle: fixTitle,
                allowCompilationErrors: allowCompilationErrors,
                suppressWarnings: suppressWarnings,
                metadataReferences: metadataReferences,
                compilationOptions: compilationOptions);
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
         /// <param name="allowCompilationErrors">Specify if compilation errors are accepted in the fixed code. This can be for example syntax errors. Default value is <see cref="AllowCompilationErrors.No"/>.</param>
        /// <param name="suppressWarnings">A collection of <see cref="DiagnosticDescriptor.Id"/> to suppress when analyzing the code. Default is <see langword="null" /> meaning <see cref="SuppressedDiagnostics"/> are used.</param>
        /// <param name="metadataReferences">A collection of <see cref="MetadataReference"/> to use when compiling. Default is <see langword="null" /> meaning <see cref="MetadataReferences"/> are used.</param>
        /// <param name="compilationOptions">The <see cref="CSharpCompilationOptions"/>.</param>
        public static void FixAll(
            DiagnosticAnalyzer analyzer,
            CodeFixProvider fix,
            IReadOnlyList<string> before,
            string after,
            string fixTitle = null,
            AllowCompilationErrors allowCompilationErrors = AllowCompilationErrors.No,
            IEnumerable<string> suppressWarnings = null,
            IEnumerable<MetadataReference> metadataReferences = null,
            CSharpCompilationOptions compilationOptions = null)
        {
            FixAll(
                analyzer: analyzer,
                fix: fix,
                diagnosticsAndSources: DiagnosticsAndSources.CreateFromCodeWithErrorsIndicated(analyzer, before),
                after: MergeFixedCode(before, after),
                fixTitle: fixTitle,
                allowCompilationErrors: allowCompilationErrors,
                suppressWarnings: suppressWarnings,
                metadataReferences: metadataReferences,
                compilationOptions: compilationOptions);
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
         /// <param name="allowCompilationErrors">Specify if compilation errors are accepted in the fixed code. This can be for example syntax errors. Default value is <see cref="AllowCompilationErrors.No"/>.</param>
        /// <param name="suppressWarnings">A collection of <see cref="DiagnosticDescriptor.Id"/> to suppress when analyzing the code. Default is <see langword="null" /> meaning <see cref="SuppressedDiagnostics"/> are used.</param>
        /// <param name="metadataReferences">A collection of <see cref="MetadataReference"/> to use when compiling. Default is <see langword="null" /> meaning <see cref="MetadataReferences"/> are used.</param>
        /// <param name="compilationOptions">The <see cref="CSharpCompilationOptions"/>.</param>
        public static void FixAll(
            DiagnosticAnalyzer analyzer,
            CodeFixProvider fix,
            IReadOnlyList<string> before,
            IReadOnlyList<string> after,
            string fixTitle = null,
            AllowCompilationErrors allowCompilationErrors = AllowCompilationErrors.No,
            IEnumerable<string> suppressWarnings = null,
            IEnumerable<MetadataReference> metadataReferences = null,
            CSharpCompilationOptions compilationOptions = null)
        {
            FixAll(
                analyzer: analyzer,
                fix: fix,
                diagnosticsAndSources: DiagnosticsAndSources.CreateFromCodeWithErrorsIndicated(analyzer, before),
                after: after,
                fixTitle: fixTitle,
                allowCompilationErrors: allowCompilationErrors,
                suppressWarnings: suppressWarnings,
                metadataReferences: metadataReferences,
                compilationOptions: compilationOptions);
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
         /// <param name="allowCompilationErrors">Specify if compilation errors are accepted in the fixed code. This can be for example syntax errors. Default value is <see cref="AllowCompilationErrors.No"/>.</param>
        /// <param name="suppressWarnings">A collection of <see cref="DiagnosticDescriptor.Id"/> to suppress when analyzing the code. Default is <see langword="null" /> meaning <see cref="SuppressedDiagnostics"/> are used.</param>
        /// <param name="metadataReferences">A collection of <see cref="MetadataReference"/> to use when compiling. Default is <see langword="null" /> meaning <see cref="MetadataReferences"/> are used.</param>
        /// <param name="compilationOptions">The <see cref="CSharpCompilationOptions"/>.</param>
        public static void FixAll(
            DiagnosticAnalyzer analyzer,
            CodeFixProvider fix,
            DiagnosticsAndSources diagnosticsAndSources,
            IReadOnlyList<string> after,
            string fixTitle,
            AllowCompilationErrors allowCompilationErrors = AllowCompilationErrors.No,
            IEnumerable<string> suppressWarnings = null,
            IEnumerable<MetadataReference> metadataReferences = null,
            CSharpCompilationOptions compilationOptions = null)
        {
            VerifyAnalyzerSupportsDiagnostics(analyzer, diagnosticsAndSources.ExpectedDiagnostics);
            VerifyCodeFixSupportsAnalyzer(analyzer, fix);
            var sln = CodeFactory.CreateSolution(
                diagnosticsAndSources: diagnosticsAndSources,
                analyzer: analyzer,
                compilationOptions: compilationOptions,
                suppressWarnings: suppressWarnings ?? SuppressedDiagnostics,
                metadataReferences: metadataReferences ?? MetadataReferences);
            var diagnostics = Analyze.GetDiagnostics(analyzer, sln);
            VerifyDiagnostics(diagnosticsAndSources, diagnostics, sln);
            FixAllOneByOne(analyzer, fix, sln, after, fixTitle, allowCompilationErrors);

            var fixAllProvider = fix.GetFixAllProvider();
            if (fixAllProvider != null)
            {
                foreach (var scope in fixAllProvider.GetSupportedFixAllScopes())
                {
                    FixAllByScope(analyzer, fix, sln, after, fixTitle, allowCompilationErrors, scope);
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
         /// <param name="allowCompilationErrors">Specify if compilation errors are accepted in the fixed code. This can be for example syntax errors. Default value is <see cref="AllowCompilationErrors.No"/>.</param>
        /// <param name="suppressWarnings">A collection of <see cref="DiagnosticDescriptor.Id"/> to suppress when analyzing the code. Default is <see langword="null" /> meaning <see cref="SuppressedDiagnostics"/> are used.</param>
        /// <param name="metadataReferences">A collection of <see cref="MetadataReference"/> to use when compiling. Default is <see langword="null" /> meaning <see cref="MetadataReferences"/> are used.</param>
        /// <param name="compilationOptions">The <see cref="CSharpCompilationOptions"/>.</param>
        public static void FixAllInDocument(
            DiagnosticAnalyzer analyzer,
            CodeFixProvider fix,
            string before,
            string after,
            string fixTitle = null,
            AllowCompilationErrors allowCompilationErrors = AllowCompilationErrors.No,
            IEnumerable<string> suppressWarnings = null,
            IEnumerable<MetadataReference> metadataReferences = null,
            CSharpCompilationOptions compilationOptions = null)
        {
            FixAllByScope(
                analyzer: analyzer,
                fix: fix,
                before: new[] { before },
                after: new[] { after },
                scope: FixAllScope.Document,
                fixTitle: fixTitle,
                allowCompilationErrors: allowCompilationErrors,
                suppressWarnings: suppressWarnings,
                metadataReferences: metadataReferences,
                compilationOptions: compilationOptions);
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
         /// <param name="allowCompilationErrors">Specify if compilation errors are accepted in the fixed code. This can be for example syntax errors. Default value is <see cref="AllowCompilationErrors.No"/>.</param>
        /// <param name="suppressWarnings">A collection of <see cref="DiagnosticDescriptor.Id"/> to suppress when analyzing the code. Default is <see langword="null" /> meaning <see cref="SuppressedDiagnostics"/> are used.</param>
        /// <param name="metadataReferences">A collection of <see cref="MetadataReference"/> to use when compiling. Default is <see langword="null" /> meaning <see cref="MetadataReferences"/> are used.</param>
        /// <param name="compilationOptions">The <see cref="CSharpCompilationOptions"/>.</param>
        public static void FixAllInDocument(
            DiagnosticAnalyzer analyzer,
            CodeFixProvider fix,
            ExpectedDiagnostic expectedDiagnostic,
            string before,
            string after,
            string fixTitle = null,
            AllowCompilationErrors allowCompilationErrors = AllowCompilationErrors.No,
            IEnumerable<string> suppressWarnings = null,
            IEnumerable<MetadataReference> metadataReferences = null,
            CSharpCompilationOptions compilationOptions = null)
        {
            var diagnosticsAndSources = DiagnosticsAndSources.Create(expectedDiagnostic, before);
            VerifyAnalyzerSupportsDiagnostics(analyzer, diagnosticsAndSources.ExpectedDiagnostics);
            VerifyCodeFixSupportsAnalyzer(analyzer, fix);
            var sln = CodeFactory.CreateSolution(
                diagnosticsAndSources,
                analyzer,
                compilationOptions,
                suppressWarnings ?? SuppressedDiagnostics,
                metadataReferences ?? MetadataReferences);
            var diagnostics = Analyze.GetDiagnostics(sln, analyzer);
            VerifyDiagnostics(diagnosticsAndSources, diagnostics, sln);
            FixAllByScope(analyzer, fix, sln, new[] { after }, fixTitle, allowCompilationErrors, FixAllScope.Document);
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
         /// <param name="allowCompilationErrors">Specify if compilation errors are accepted in the fixed code. This can be for example syntax errors. Default value is <see cref="AllowCompilationErrors.No"/>.</param>
        /// <param name="suppressWarnings">A collection of <see cref="DiagnosticDescriptor.Id"/> to suppress when analyzing the code. Default is <see langword="null" /> meaning <see cref="SuppressedDiagnostics"/> are used.</param>
        /// <param name="metadataReferences">A collection of <see cref="MetadataReference"/> to use when compiling. Default is <see langword="null" /> meaning <see cref="MetadataReferences"/> are used.</param>
        /// <param name="compilationOptions">The <see cref="CSharpCompilationOptions"/>.</param>
        public static void FixAllOneByOne(
            DiagnosticAnalyzer analyzer,
            CodeFixProvider fix,
            string before,
            string after,
            string fixTitle = null,
            AllowCompilationErrors allowCompilationErrors = AllowCompilationErrors.No,
            IEnumerable<string> suppressWarnings = null,
            IEnumerable<MetadataReference> metadataReferences = null,
            CSharpCompilationOptions compilationOptions = null)
        {
            var diagnosticsAndSources = DiagnosticsAndSources.CreateFromCodeWithErrorsIndicated(analyzer, before);
            VerifyAnalyzerSupportsDiagnostics(analyzer, diagnosticsAndSources.ExpectedDiagnostics);
            VerifyCodeFixSupportsAnalyzer(analyzer, fix);
            var sln = CodeFactory.CreateSolution(
                diagnosticsAndSources,
                analyzer,
                compilationOptions,
                suppressWarnings ?? SuppressedDiagnostics,
                metadataReferences ?? MetadataReferences);
            var diagnostics = Analyze.GetDiagnostics(analyzer, sln);
            VerifyDiagnostics(diagnosticsAndSources, diagnostics, sln);
            FixAllOneByOne(analyzer, fix, sln, new[] { after }, fixTitle, allowCompilationErrors);
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
         /// <param name="allowCompilationErrors">Specify if compilation errors are accepted in the fixed code. This can be for example syntax errors. Default value is <see cref="AllowCompilationErrors.No"/>.</param>
        /// <param name="suppressWarnings">A collection of <see cref="DiagnosticDescriptor.Id"/> to suppress when analyzing the code. Default is <see langword="null" /> meaning <see cref="SuppressedDiagnostics"/> are used.</param>
        /// <param name="metadataReferences">A collection of <see cref="MetadataReference"/> to use when compiling. Default is <see langword="null" /> meaning <see cref="MetadataReferences"/> are used.</param>
        /// <param name="compilationOptions">The <see cref="CSharpCompilationOptions"/>.</param>
        public static void FixAllOneByOne(
            DiagnosticAnalyzer analyzer,
            CodeFixProvider fix,
            ExpectedDiagnostic expectedDiagnostic,
            string before,
            string after,
            string fixTitle = null,
            AllowCompilationErrors allowCompilationErrors = AllowCompilationErrors.No,
            IEnumerable<string> suppressWarnings = null,
            IEnumerable<MetadataReference> metadataReferences = null,
            CSharpCompilationOptions compilationOptions = null)
        {
            var diagnosticsAndSources = DiagnosticsAndSources.Create(expectedDiagnostic, before);
            VerifyAnalyzerSupportsDiagnostics(analyzer, diagnosticsAndSources.ExpectedDiagnostics);
            VerifyCodeFixSupportsAnalyzer(analyzer, fix);
            var sln = CodeFactory.CreateSolution(
                diagnosticsAndSources,
                analyzer,
                compilationOptions,
                suppressWarnings ?? SuppressedDiagnostics,
                metadataReferences ?? MetadataReferences);
            var diagnostics = Analyze.GetDiagnostics(analyzer, sln);
            VerifyDiagnostics(diagnosticsAndSources, diagnostics, sln);
            FixAllOneByOne(analyzer, fix, sln, new[] { after }, fixTitle, allowCompilationErrors);
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
         /// <param name="allowCompilationErrors">Specify if compilation errors are accepted in the fixed code. This can be for example syntax errors. Default value is <see cref="AllowCompilationErrors.No"/>.</param>
        /// <param name="suppressWarnings">A collection of <see cref="DiagnosticDescriptor.Id"/> to suppress when analyzing the code. Default is <see langword="null" /> meaning <see cref="SuppressedDiagnostics"/> are used.</param>
        /// <param name="metadataReferences">A collection of <see cref="MetadataReference"/> to use when compiling. Default is <see langword="null" /> meaning <see cref="MetadataReferences"/> are used.</param>
        /// <param name="compilationOptions">The <see cref="CSharpCompilationOptions"/>.</param>
        public static void FixAllOneByOne(
            CodeFixProvider fix,
            ExpectedDiagnostic expectedDiagnostic,
            string before,
            string after,
            string fixTitle = null,
            AllowCompilationErrors allowCompilationErrors = AllowCompilationErrors.No,
            IEnumerable<string> suppressWarnings = null,
            IEnumerable<MetadataReference> metadataReferences = null,
            CSharpCompilationOptions compilationOptions = null)
        {
            var analyzer = new PlaceholderAnalyzer(expectedDiagnostic.Id);
            var diagnosticsAndSources = DiagnosticsAndSources.Create(expectedDiagnostic, before);
            VerifyAnalyzerSupportsDiagnostics(analyzer, diagnosticsAndSources.ExpectedDiagnostics);
            VerifyCodeFixSupportsAnalyzer(analyzer, fix);
            var sln = CodeFactory.CreateSolution(
                diagnosticsAndSources,
                analyzer,
                compilationOptions,
                suppressWarnings ?? SuppressedDiagnostics,
                metadataReferences ?? MetadataReferences);
            var diagnostics = Analyze.GetDiagnostics(analyzer, sln);
            VerifyDiagnostics(diagnosticsAndSources, diagnostics, sln);
            FixAllOneByOne(analyzer, fix, sln, new[] { after }, fixTitle, allowCompilationErrors);
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
         /// <param name="allowCompilationErrors">Specify if compilation errors are accepted in the fixed code. This can be for example syntax errors. Default value is <see cref="AllowCompilationErrors.No"/>.</param>
        /// <param name="suppressWarnings">A collection of <see cref="DiagnosticDescriptor.Id"/> to suppress when analyzing the code. Default is <see langword="null" /> meaning <see cref="SuppressedDiagnostics"/> are used.</param>
        /// <param name="metadataReferences">A collection of <see cref="MetadataReference"/> to use when compiling. Default is <see langword="null" /> meaning <see cref="MetadataReferences"/> are used.</param>
        /// <param name="compilationOptions">The <see cref="CSharpCompilationOptions"/>.</param>
        public static void FixAllByScope(
            CodeFixProvider fix,
            ExpectedDiagnostic expectedDiagnostic,
            IReadOnlyList<string> before,
            IReadOnlyList<string> after,
            FixAllScope scope,
            string fixTitle = null,
            AllowCompilationErrors allowCompilationErrors = AllowCompilationErrors.No,
            IEnumerable<string> suppressWarnings = null,
            IEnumerable<MetadataReference> metadataReferences = null,
            CSharpCompilationOptions compilationOptions = null)
        {
            var analyzer = new PlaceholderAnalyzer(expectedDiagnostic.Id);
            var diagnosticsAndSources = DiagnosticsAndSources.Create(expectedDiagnostic, before);
            VerifyAnalyzerSupportsDiagnostics(analyzer, diagnosticsAndSources.ExpectedDiagnostics);
            VerifyCodeFixSupportsAnalyzer(analyzer, fix);
            var sln = CodeFactory.CreateSolution(
                diagnosticsAndSources,
                analyzer,
                compilationOptions,
                suppressWarnings ?? SuppressedDiagnostics,
                metadataReferences ?? MetadataReferences);
            var diagnostics = Analyze.GetDiagnostics(sln, analyzer);
            VerifyDiagnostics(diagnosticsAndSources, diagnostics, sln);
            FixAllByScope(analyzer, fix, sln, after, fixTitle, allowCompilationErrors, scope);
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
         /// <param name="allowCompilationErrors">Specify if compilation errors are accepted in the fixed code. This can be for example syntax errors. Default value is <see cref="AllowCompilationErrors.No"/>.</param>
        /// <param name="suppressWarnings">A collection of <see cref="DiagnosticDescriptor.Id"/> to suppress when analyzing the code. Default is <see langword="null" /> meaning <see cref="SuppressedDiagnostics"/> are used.</param>
        /// <param name="metadataReferences">A collection of <see cref="MetadataReference"/> to use when compiling. Default is <see langword="null" /> meaning <see cref="MetadataReferences"/> are used.</param>
        /// <param name="compilationOptions">The <see cref="CSharpCompilationOptions"/>.</param>
        public static void FixAllByScope(
            DiagnosticAnalyzer analyzer,
            CodeFixProvider fix,
            IReadOnlyList<string> before,
            IReadOnlyList<string> after,
            FixAllScope scope,
            string fixTitle = null,
            AllowCompilationErrors allowCompilationErrors = AllowCompilationErrors.No,
            IEnumerable<string> suppressWarnings = null,
            IEnumerable<MetadataReference> metadataReferences = null,
            CSharpCompilationOptions compilationOptions = null)
        {
            var diagnosticsAndSources = DiagnosticsAndSources.CreateFromCodeWithErrorsIndicated(analyzer, before);
            VerifyAnalyzerSupportsDiagnostics(analyzer, diagnosticsAndSources.ExpectedDiagnostics);
            VerifyCodeFixSupportsAnalyzer(analyzer, fix);
            var sln = CodeFactory.CreateSolution(
                diagnosticsAndSources,
                analyzer,
                compilationOptions,
                suppressWarnings ?? SuppressedDiagnostics,
                metadataReferences ?? MetadataReferences);
            var diagnostics = Analyze.GetDiagnostics(sln, analyzer);
            VerifyDiagnostics(diagnosticsAndSources, diagnostics, sln);
            FixAllByScope(analyzer, fix, sln, after, fixTitle, allowCompilationErrors, scope);
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
         /// <param name="allowCompilationErrors">Specify if compilation errors are accepted in the fixed code. This can be for example syntax errors. Default value is <see cref="AllowCompilationErrors.No"/>.</param>
        /// <param name="suppressWarnings">A collection of <see cref="DiagnosticDescriptor.Id"/> to suppress when analyzing the code. Default is <see langword="null" /> meaning <see cref="SuppressedDiagnostics"/> are used.</param>
        /// <param name="metadataReferences">A collection of <see cref="MetadataReference"/> to use when compiling. Default is <see langword="null" /> meaning <see cref="MetadataReferences"/> are used.</param>
        /// <param name="compilationOptions">The <see cref="CSharpCompilationOptions"/>.</param>
        public static void FixAllByScope(
            DiagnosticAnalyzer analyzer,
            CodeFixProvider fix,
            ExpectedDiagnostic expectedDiagnostic,
            IReadOnlyList<string> before,
            IReadOnlyList<string> after,
            FixAllScope scope,
            string fixTitle = null,
            AllowCompilationErrors allowCompilationErrors = AllowCompilationErrors.No,
            IEnumerable<string> suppressWarnings = null,
            IEnumerable<MetadataReference> metadataReferences = null,
            CSharpCompilationOptions compilationOptions = null)
        {
            var diagnosticsAndSources = DiagnosticsAndSources.Create(expectedDiagnostic, before);
            VerifyAnalyzerSupportsDiagnostics(analyzer, diagnosticsAndSources.ExpectedDiagnostics);
            VerifyCodeFixSupportsAnalyzer(analyzer, fix);
            var sln = CodeFactory.CreateSolution(
                diagnosticsAndSources,
                analyzer,
                compilationOptions,
                suppressWarnings ?? SuppressedDiagnostics,
                metadataReferences ?? MetadataReferences);
            var diagnostics = Analyze.GetDiagnostics(sln, analyzer);
            VerifyDiagnostics(diagnosticsAndSources, diagnostics, sln);
            FixAllByScope(analyzer, fix, sln, after, fixTitle, allowCompilationErrors, scope);
        }

        private static void FixAllOneByOne(DiagnosticAnalyzer analyzer, CodeFixProvider fix, Solution solution, IReadOnlyList<string> after, string fixTitle, AllowCompilationErrors allowCompilationErrors)
        {
            var fixedSolution = Fix.ApplyAllFixableOneByOneAsync(solution, analyzer, fix, fixTitle, CancellationToken.None).GetAwaiter().GetResult();
            AreEqualAsync(after, fixedSolution, "Applying fixes one by one failed.").GetAwaiter().GetResult();
            if (allowCompilationErrors == AllowCompilationErrors.No)
            {
                VerifyNoCompilerErrorsAsync(fix, fixedSolution).ConfigureAwait(false).GetAwaiter().GetResult();
            }
        }

        private static void FixAllByScope(DiagnosticAnalyzer analyzer, CodeFixProvider fix, Solution sln, IReadOnlyList<string> after, string fixTitle, AllowCompilationErrors allowCompilationErrors, FixAllScope scope)
        {
            VerifyCodeFixSupportsAnalyzer(analyzer, fix);
            var fixedSolution = Fix.ApplyAllFixableScopeByScopeAsync(sln, analyzer, fix, scope, fixTitle, CancellationToken.None).GetAwaiter().GetResult();
            AreEqualAsync(after, fixedSolution, $"Applying fixes for {scope} failed.").GetAwaiter().GetResult();
            if (allowCompilationErrors == AllowCompilationErrors.No)
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
