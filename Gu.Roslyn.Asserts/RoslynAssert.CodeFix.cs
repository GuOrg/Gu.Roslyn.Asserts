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
        /// 1. <paramref name="codeWithErrorsIndicated"/> produces the expected diagnostics
        /// 2. The code fix fixes the code.
        /// </summary>
        /// <typeparam name="TAnalyzer">The type of the analyzer.</typeparam>
        /// <typeparam name="TCodeFix">The type of the code fix.</typeparam>
        /// <param name="codeWithErrorsIndicated">The code with error positions indicated.</param>
        /// <param name="fixedCode">The expected code produced by the code fix.</param>
        /// <param name="fixTitle">The title of the fix to apply if more than one.</param>
        /// <param name="allowCompilationErrors">If compilation errors are accepted in the fixed code.</param>
        public static void CodeFix<TAnalyzer, TCodeFix>(string codeWithErrorsIndicated, string fixedCode, string fixTitle = null, AllowCompilationErrors allowCompilationErrors = AllowCompilationErrors.No)
            where TAnalyzer : DiagnosticAnalyzer, new()
            where TCodeFix : CodeFixProvider, new()
        {
            var analyzer = new TAnalyzer();
            CodeFix(
                analyzer: analyzer,
                fix: new TCodeFix(),
                diagnosticsAndSources: DiagnosticsAndSources.CreateFromCodeWithErrorsIndicated(analyzer, codeWithErrorsIndicated),
                fixedCode: new[] { fixedCode },
                suppressedDiagnostics: null,
                metadataReferences: null,
                compilationOptions: null,
                fixTitle: fixTitle,
                allowCompilationErrors: allowCompilationErrors);
        }

        /// <summary>
        /// Verifies that
        /// 1. <paramref name="codeWithErrorsIndicated"/> produces the expected diagnostics
        /// 2. The code fix fixes the code.
        /// </summary>
        /// <typeparam name="TAnalyzer">The type of the analyzer.</typeparam>
        /// <typeparam name="TCodeFix">The type of the code fix.</typeparam>
        /// <param name="codeWithErrorsIndicated">The code with error positions indicated.</param>
        /// <param name="fixedCode">The expected code produced by the code fix.</param>
        /// <param name="fixTitle">The title of the fix to apply if more than one.</param>
        /// <param name="allowCompilationErrors">If compilation errors are accepted in the fixed code.</param>
        public static void CodeFix<TAnalyzer, TCodeFix>(IReadOnlyList<string> codeWithErrorsIndicated, string fixedCode, string fixTitle = null, AllowCompilationErrors allowCompilationErrors = AllowCompilationErrors.No)
            where TAnalyzer : DiagnosticAnalyzer, new()
            where TCodeFix : CodeFixProvider, new()
        {
            var analyzer = new TAnalyzer();
            CodeFix(
                analyzer: analyzer,
                fix: new TCodeFix(),
                diagnosticsAndSources: DiagnosticsAndSources.CreateFromCodeWithErrorsIndicated(analyzer, codeWithErrorsIndicated),
                fixedCode: MergeFixedCode(codeWithErrorsIndicated, fixedCode),
                suppressedDiagnostics: null,
                metadataReferences: null,
                compilationOptions: null,
                fixTitle: fixTitle,
                allowCompilationErrors: allowCompilationErrors);
        }

        /// <summary>
        /// Verifies that
        /// 1. <paramref name="codeWithErrorsIndicated"/> produces the expected diagnostics
        /// 2. The code fix fixes the code.
        /// </summary>
        /// <param name="analyzer">The analyzer to run on the code..</param>
        /// <param name="fix">The code fix to apply.</param>
        /// <param name="codeWithErrorsIndicated">The code with error positions indicated.</param>
        /// <param name="fixedCode">The expected code produced by the code fix.</param>
        /// <param name="fixTitle">The title of the fix to apply if more than one.</param>
        /// <param name="allowCompilationErrors">If compilation errors are accepted in the fixed code.</param>
        public static void CodeFix(DiagnosticAnalyzer analyzer, CodeFixProvider fix, string codeWithErrorsIndicated, string fixedCode, string fixTitle = null, AllowCompilationErrors allowCompilationErrors = AllowCompilationErrors.No)
        {
            CodeFix(
                analyzer: analyzer,
                fix: fix,
                diagnosticsAndSources: DiagnosticsAndSources.CreateFromCodeWithErrorsIndicated(analyzer, codeWithErrorsIndicated),
                fixedCode: new[] { fixedCode },
                suppressedDiagnostics: null,
                metadataReferences: null,
                compilationOptions: null,
                fixTitle: fixTitle,
                allowCompilationErrors: allowCompilationErrors);
        }

        /// <summary>
        /// Verifies that
        /// 1. <paramref name="solution"/> produces the expected diagnostics
        /// 2. The code fix fixes the code.
        /// </summary>
        /// <param name="analyzer">The analyzer to run on the code..</param>
        /// <param name="fix">The code fix to apply.</param>
        /// <param name="expectedDiagnostic">The expected diagnostic.</param>
        /// <param name="solution">The code with error positions indicated.</param>
        /// <param name="fixedCode">The expected code produced by the code fix.</param>
        /// <param name="fixTitle">The title of the fix to apply if more than one.</param>
        /// <param name="allowCompilationErrors">If compilation errors are accepted in the fixed code.</param>
        public static void CodeFix(DiagnosticAnalyzer analyzer, CodeFixProvider fix, ExpectedDiagnostic expectedDiagnostic, Solution solution, string fixedCode, string fixTitle = null, AllowCompilationErrors allowCompilationErrors = AllowCompilationErrors.No)
        {
            VerifyAnalyzerSupportsDiagnostic(analyzer, expectedDiagnostic);
            VerifyCodeFixSupportsAnalyzer(analyzer, fix);
            var diagnostics = Analyze.GetDiagnostics(analyzer, solution);
            var diagnosticsAndSources = DiagnosticsAndSources.Create(expectedDiagnostic, solution.Projects.SelectMany(x => x.Documents).Select(x => x.GetCode(null)).ToArray());
            VerifyDiagnostics(diagnosticsAndSources, diagnostics);
            VerifyFix(solution, diagnostics, analyzer, fix, MergeFixedCode(diagnosticsAndSources.Code, fixedCode), fixTitle, allowCompilationErrors);
        }

        /// <summary>
        /// Verifies that
        /// 1. <paramref name="code"/> produces the expected diagnostics
        /// 2. The code fix fixes the code.
        /// </summary>
        /// <typeparam name="TAnalyzer">The type of the analyzer.</typeparam>
        /// <typeparam name="TCodeFix">The type of the code fix.</typeparam>
        /// <param name="expectedDiagnostic">The expected diagnostic.</param>
        /// <param name="code">The code with error positions indicated.</param>
        /// <param name="fixedCode">The expected code produced by the code fix.</param>
        /// <param name="fixTitle">The title of the fix to apply if more than one.</param>
        /// <param name="allowCompilationErrors">If compilation errors are accepted in the fixed code.</param>
        public static void CodeFix<TAnalyzer, TCodeFix>(ExpectedDiagnostic expectedDiagnostic, string code, string fixedCode, string fixTitle = null, AllowCompilationErrors allowCompilationErrors = AllowCompilationErrors.No)
            where TAnalyzer : DiagnosticAnalyzer, new()
            where TCodeFix : CodeFixProvider, new()
        {
            CodeFix(
                analyzer: new TAnalyzer(),
                fix: new TCodeFix(),
                diagnosticsAndSources: DiagnosticsAndSources.Create(expectedDiagnostic, code),
                fixedCode: new[] { fixedCode },
                suppressedDiagnostics: null,
                metadataReferences: null,
                compilationOptions: null,
                fixTitle: fixTitle,
                allowCompilationErrors: allowCompilationErrors);
        }

        /// <summary>
        /// Verifies that
        /// 1. <paramref name="code"/> produces the expected diagnostics
        /// 2. The code fix fixes the code.
        /// </summary>
        /// <typeparam name="TAnalyzer">The type of the analyzer.</typeparam>
        /// <typeparam name="TCodeFix">The type of the code fix.</typeparam>
        /// <param name="expectedDiagnostic">The expected diagnostic.</param>
        /// <param name="code">The code with error positions indicated.</param>
        /// <param name="fixedCode">The expected code produced by the code fix.</param>
        /// <param name="fixTitle">The title of the fix to apply if more than one.</param>
        /// <param name="allowCompilationErrors">If compilation errors are accepted in the fixed code.</param>
        public static void CodeFix<TAnalyzer, TCodeFix>(ExpectedDiagnostic expectedDiagnostic, IReadOnlyList<string> code, string fixedCode, string fixTitle = null, AllowCompilationErrors allowCompilationErrors = AllowCompilationErrors.No)
            where TAnalyzer : DiagnosticAnalyzer, new()
            where TCodeFix : CodeFixProvider, new()
        {
            CodeFix(
                analyzer: new TAnalyzer(),
                fix: new TCodeFix(),
                diagnosticsAndSources: DiagnosticsAndSources.Create(expectedDiagnostic, code),
                fixedCode: MergeFixedCode(code, fixedCode),
                suppressedDiagnostics: null,
                metadataReferences: null,
                compilationOptions: null,
                fixTitle: fixTitle,
                allowCompilationErrors: allowCompilationErrors);
        }

        /// <summary>
        /// Verifies that
        /// 1. <paramref name="codeWithErrorsIndicated"/> produces the expected diagnostics
        /// 2. The code fix fixes the code.
        /// </summary>
        /// <param name="analyzer">The analyzer to run on the code..</param>
        /// <param name="fix">The <see cref="CodeFixProvider"/> to apply.</param>
        /// <param name="codeWithErrorsIndicated">The code to analyze.</param>
        /// <param name="fixedCode">The expected code produced by the code fix.</param>
        /// <param name="compilationOptions">The compilation options.</param>
        /// <param name="metadataReferences">Collection of <see cref="MetadataReference"/> to use when compiling.</param>
        /// <param name="fixTitle">The title of the fix to apply if more than one.</param>
        /// <param name="allowCompilationErrors">If compilation errors are accepted in the fixed code.</param>
        public static void CodeFix(DiagnosticAnalyzer analyzer, CodeFixProvider fix, IReadOnlyList<string> codeWithErrorsIndicated, string fixedCode, CSharpCompilationOptions compilationOptions = null, IEnumerable<MetadataReference> metadataReferences = null, string fixTitle = null, AllowCompilationErrors allowCompilationErrors = AllowCompilationErrors.No)
        {
            CodeFix(
                analyzer: analyzer,
                fix: fix,
                diagnosticsAndSources: DiagnosticsAndSources.CreateFromCodeWithErrorsIndicated(analyzer, codeWithErrorsIndicated),
                fixedCode: MergeFixedCode(codeWithErrorsIndicated, fixedCode),
                suppressedDiagnostics: null,
                metadataReferences: metadataReferences,
                compilationOptions: compilationOptions,
                fixTitle: fixTitle,
                allowCompilationErrors: allowCompilationErrors);
        }

        /// <summary>
        /// Verifies that
        /// 1. <paramref name="codeWithErrorsIndicated"/> produces the expected diagnostics
        /// 2. The code fix fixes the code.
        /// </summary>
        /// <param name="analyzer">The analyzer to run on the code..</param>
        /// <param name="fix">The <see cref="CodeFixProvider"/> to apply.</param>
        /// <param name="codeWithErrorsIndicated">The code to analyze.</param>
        /// <param name="fixedCode">The expected code produced by the code fix.</param>
        /// <param name="compilationOptions">The compilation options.</param>
        /// <param name="metadataReferences">Collection of <see cref="MetadataReference"/> to use when compiling.</param>
        /// <param name="fixTitle">The title of the fix to apply if more than one.</param>
        /// <param name="allowCompilationErrors">If compilation errors are accepted in the fixed code.</param>
        public static void CodeFix(DiagnosticAnalyzer analyzer, CodeFixProvider fix, IReadOnlyList<string> codeWithErrorsIndicated, IReadOnlyList<string> fixedCode, CSharpCompilationOptions compilationOptions = null, IEnumerable<MetadataReference> metadataReferences = null, string fixTitle = null, AllowCompilationErrors allowCompilationErrors = AllowCompilationErrors.No)
        {
            CodeFix(
                analyzer: analyzer,
                fix: fix,
                diagnosticsAndSources: DiagnosticsAndSources.CreateFromCodeWithErrorsIndicated(analyzer, codeWithErrorsIndicated),
                fixedCode: fixedCode,
                suppressedDiagnostics: null,
                metadataReferences: metadataReferences,
                compilationOptions: compilationOptions,
                fixTitle: fixTitle,
                allowCompilationErrors: allowCompilationErrors);
        }

        /// <summary>
        /// Verifies that
        /// 1. <paramref name="code"/> produces the expected diagnostics
        /// 2. The code fix fixes the code.
        /// </summary>
        /// <param name="analyzer">The analyzer to run on the code..</param>
        /// <param name="fix">The code fix to apply.</param>
        /// <param name="expectedDiagnostic">The expected diagnostic.</param>
        /// <param name="code">The code with error positions indicated.</param>
        /// <param name="fixedCode">The expected code produced by the code fix.</param>
        /// <param name="fixTitle">The title of the fix to apply if more than one.</param>
        /// <param name="allowCompilationErrors">If compilation errors are accepted in the fixed code.</param>
        public static void CodeFix(DiagnosticAnalyzer analyzer, CodeFixProvider fix, ExpectedDiagnostic expectedDiagnostic, string code, string fixedCode, string fixTitle = null, AllowCompilationErrors allowCompilationErrors = AllowCompilationErrors.No)
        {
            CodeFix(
                analyzer: analyzer,
                fix: fix,
                diagnosticsAndSources: DiagnosticsAndSources.Create(expectedDiagnostic, code),
                fixedCode: new[] { fixedCode },
                suppressedDiagnostics: null,
                metadataReferences: null,
                compilationOptions: null,
                fixTitle: fixTitle,
                allowCompilationErrors: allowCompilationErrors);
        }

        /// <summary>
        /// Verifies that
        /// 1. <paramref name="code"/> produces the expected diagnostics
        /// 2. The code fix fixes the code.
        /// </summary>
        /// <param name="analyzer">The analyzer to run on the code..</param>
        /// <param name="fix">The code fix to apply.</param>
        /// <param name="expectedDiagnostic">The expected diagnostic.</param>
        /// <param name="code">The code with error positions indicated.</param>
        /// <param name="fixedCode">The expected code produced by the code fix.</param>
        /// <param name="fixTitle">The title of the fix to apply if more than one.</param>
        /// <param name="allowCompilationErrors">If compilation errors are accepted in the fixed code.</param>
        public static void CodeFix(DiagnosticAnalyzer analyzer, CodeFixProvider fix, ExpectedDiagnostic expectedDiagnostic, IReadOnlyList<string> code, string fixedCode, string fixTitle = null, AllowCompilationErrors allowCompilationErrors = AllowCompilationErrors.No)
        {
            CodeFix(
                analyzer: analyzer,
                fix: fix,
                diagnosticsAndSources: DiagnosticsAndSources.Create(expectedDiagnostic, code),
                fixedCode: MergeFixedCode(code, fixedCode),
                suppressedDiagnostics: null,
                metadataReferences: null,
                compilationOptions: null,
                fixTitle: fixTitle,
                allowCompilationErrors: allowCompilationErrors);
        }

        /// <summary>
        /// Verifies that
        /// 1. <paramref name="code"/> produces the expected diagnostics
        /// 2. The code fix fixes the code.
        /// </summary>
        /// <param name="analyzer">The analyzer to run on the code..</param>
        /// <param name="fix">The code fix to apply.</param>
        /// <param name="expectedDiagnostic">The expected diagnostic.</param>
        /// <param name="code">The code with error positions indicated.</param>
        /// <param name="fixedCode">The expected code produced by the code fix.</param>
        /// <param name="fixTitle">The title of the fix to apply if more than one.</param>
        /// <param name="allowCompilationErrors">If compilation errors are accepted in the fixed code.</param>
        public static void CodeFix(DiagnosticAnalyzer analyzer, CodeFixProvider fix, ExpectedDiagnostic expectedDiagnostic, IReadOnlyList<string> code, IReadOnlyList<string> fixedCode, string fixTitle = null, AllowCompilationErrors allowCompilationErrors = AllowCompilationErrors.No)
        {
            CodeFix(
                analyzer: analyzer,
                fix: fix,
                diagnosticsAndSources: DiagnosticsAndSources.Create(expectedDiagnostic, code),
                fixedCode: fixedCode,
                suppressedDiagnostics: null,
                metadataReferences: null,
                compilationOptions: null,
                fixTitle: fixTitle,
                allowCompilationErrors: allowCompilationErrors);
        }

        /// <summary>
        /// Verifies that
        /// 1. <paramref name="code"/> produces the expected diagnostics
        /// 2. The code fix fixes the code.
        /// </summary>
        /// <typeparam name="TCodeFix">The type of the code fix.</typeparam>
        /// <param name="expectedDiagnostic">The expected diagnostic.</param>
        /// <param name="code">The code to analyze.</param>
        /// <param name="fixedCode">The expected code produced by the code fix.</param>
        /// <param name="fixTitle">The title of the fix to apply if more than one.</param>
        /// <param name="allowCompilationErrors">If compilation errors are accepted in the fixed code.</param>
        public static void CodeFix<TCodeFix>(ExpectedDiagnostic expectedDiagnostic, string code, string fixedCode, string fixTitle = null, AllowCompilationErrors allowCompilationErrors = AllowCompilationErrors.No)
            where TCodeFix : CodeFixProvider, new()
        {
            CodeFix(
                analyzer: new PlaceholderAnalyzer(expectedDiagnostic.Id),
                fix: new TCodeFix(),
                diagnosticsAndSources: DiagnosticsAndSources.Create(expectedDiagnostic, code),
                fixedCode: new[] { fixedCode },
                suppressedDiagnostics: null,
                metadataReferences: null,
                compilationOptions: null,
                fixTitle: fixTitle,
                allowCompilationErrors: allowCompilationErrors);
        }

        /// <summary>
        /// Verifies that
        /// 1. <paramref name="code"/> produces the expected diagnostics
        /// 2. The code fix fixes the code.
        /// </summary>
        /// <typeparam name="TCodeFix">The type of the code fix.</typeparam>
        /// <param name="expectedDiagnostic">The expected diagnostic.</param>
        /// <param name="code">The code to analyze.</param>
        /// <param name="fixedCode">The expected code produced by the code fix.</param>
        /// <param name="fixTitle">The title of the fix to apply if more than one.</param>
        /// <param name="allowCompilationErrors">If compilation errors are accepted in the fixed code.</param>
        public static void CodeFix<TCodeFix>(ExpectedDiagnostic expectedDiagnostic, IReadOnlyList<string> code, string fixedCode, string fixTitle = null, AllowCompilationErrors allowCompilationErrors = AllowCompilationErrors.No)
            where TCodeFix : CodeFixProvider, new()
        {
            CodeFix(
                analyzer: new PlaceholderAnalyzer(expectedDiagnostic.Id),
                fix: new TCodeFix(),
                diagnosticsAndSources: DiagnosticsAndSources.Create(expectedDiagnostic, code),
                fixedCode: MergeFixedCode(code, fixedCode),
                suppressedDiagnostics: null,
                metadataReferences: null,
                compilationOptions: null,
                fixTitle: fixTitle,
                allowCompilationErrors: allowCompilationErrors);
        }

        /// <summary>
        /// Verifies that
        /// 1. <paramref name="code"/> produces the expected diagnostics
        /// 2. The code fix fixes the code.
        /// </summary>
        /// <param name="fix">The <see cref="CodeFixProvider"/> to apply.</param>
        /// <param name="expectedDiagnostic">The expected diagnostic.</param>
        /// <param name="code">The code to analyze.</param>
        /// <param name="fixedCode">The expected code produced by the code fix.</param>
        /// <param name="fixTitle">The title of the fix to apply if more than one.</param>
        /// <param name="allowCompilationErrors">If compilation errors are accepted in the fixed code.</param>
        public static void CodeFix(CodeFixProvider fix, ExpectedDiagnostic expectedDiagnostic, string code, string fixedCode, string fixTitle = null, AllowCompilationErrors allowCompilationErrors = AllowCompilationErrors.No)
        {
            CodeFix(
                analyzer: new PlaceholderAnalyzer(expectedDiagnostic.Id),
                fix: fix,
                diagnosticsAndSources: DiagnosticsAndSources.Create(expectedDiagnostic, code),
                fixedCode: new[] { fixedCode },
                suppressedDiagnostics: null,
                metadataReferences: null,
                compilationOptions: null,
                fixTitle: fixTitle,
                allowCompilationErrors: allowCompilationErrors);
        }

        /// <summary>
        /// Verifies that
        /// 1. <paramref name="code"/> produces the expected diagnostics
        /// 2. The code fix fixes the code.
        /// </summary>
        /// <param name="fix">The <see cref="CodeFixProvider"/> to apply.</param>
        /// <param name="expectedDiagnostic">The expected diagnostic.</param>
        /// <param name="code">The code to analyze.</param>
        /// <param name="fixedCode">The expected code produced by the code fix.</param>
        /// <param name="fixTitle">The title of the fix to apply if more than one.</param>
        /// <param name="allowCompilationErrors">If compilation errors are accepted in the fixed code.</param>
        public static void CodeFix(CodeFixProvider fix, ExpectedDiagnostic expectedDiagnostic, IReadOnlyList<string> code, string fixedCode, string fixTitle = null, AllowCompilationErrors allowCompilationErrors = AllowCompilationErrors.No)
        {
            CodeFix(
                analyzer: new PlaceholderAnalyzer(expectedDiagnostic.Id),
                fix: fix,
                diagnosticsAndSources: DiagnosticsAndSources.Create(expectedDiagnostic, code),
                fixedCode: MergeFixedCode(code, fixedCode),
                suppressedDiagnostics: null,
                metadataReferences: null,
                compilationOptions: null,
                fixTitle: fixTitle,
                allowCompilationErrors: allowCompilationErrors);
        }

        /// <summary>
        /// Verifies that
        /// 1. <paramref name="code"/> produces the expected diagnostics
        /// 2. The code fix fixes the code.
        /// </summary>
        /// <param name="fix">The <see cref="CodeFixProvider"/> to apply.</param>
        /// <param name="expectedDiagnostic">The expected diagnostic.</param>
        /// <param name="code">The code to analyze.</param>
        /// <param name="fixedCode">The expected code produced by the code fix.</param>
        /// <param name="fixTitle">The title of the fix to apply if more than one.</param>
        /// <param name="allowCompilationErrors">If compilation errors are accepted in the fixed code.</param>
        public static void CodeFix(CodeFixProvider fix, ExpectedDiagnostic expectedDiagnostic, IReadOnlyList<string> code, IReadOnlyList<string> fixedCode, string fixTitle = null, AllowCompilationErrors allowCompilationErrors = AllowCompilationErrors.No)
        {
            CodeFix(
                analyzer: new PlaceholderAnalyzer(expectedDiagnostic.Id),
                fix: fix,
                diagnosticsAndSources: DiagnosticsAndSources.Create(expectedDiagnostic, code),
                fixedCode: fixedCode,
                suppressedDiagnostics: null,
                metadataReferences: null,
                compilationOptions: null,
                fixTitle: fixTitle,
                allowCompilationErrors: allowCompilationErrors);
        }

        /// <summary>
        /// Verifies that
        /// 1. <paramref name="diagnosticsAndSources"/> produces the expected diagnostics
        /// 2. The code fix fixes the code.
        /// </summary>
        /// <param name="analyzer">The analyzer to run on the code..</param>
        /// <param name="fix">The <see cref="CodeFixProvider"/> to apply.</param>
        /// <param name="diagnosticsAndSources">The code to analyze.</param>
        /// <param name="fixedCode">The expected code produced by the code fix.</param>
        /// <param name="fixTitle">The title of the fix to apply if more than one.</param>
        /// <param name="allowCompilationErrors">If compilation errors are accepted in the fixed code.</param>
        public static void CodeFix(DiagnosticAnalyzer analyzer, CodeFixProvider fix, DiagnosticsAndSources diagnosticsAndSources, string fixedCode, string fixTitle = null, AllowCompilationErrors allowCompilationErrors = AllowCompilationErrors.No)
        {
            CodeFix(
                analyzer: analyzer,
                fix: fix,
                diagnosticsAndSources: diagnosticsAndSources,
                fixedCode: MergeFixedCode(diagnosticsAndSources.Code, fixedCode),
                suppressedDiagnostics: null,
                metadataReferences: null,
                compilationOptions: null,
                fixTitle: fixTitle,
                allowCompilationErrors: allowCompilationErrors);
        }

        /// <summary>
        /// Verifies that
        /// 1. <paramref name="diagnosticsAndSources"/> produces the expected diagnostics
        /// 2. The code fix fixes the code.
        /// </summary>
        /// <param name="analyzer">The analyzer to run on the code..</param>
        /// <param name="fix">The <see cref="CodeFixProvider"/> to apply.</param>
        /// <param name="diagnosticsAndSources">The code to analyze.</param>
        /// <param name="fixedCode">The expected code produced by the code fix.</param>
        /// <param name="suppressedDiagnostics">Ids of diagnostics to suppress.</param>
        /// <param name="metadataReferences">Collection of <see cref="MetadataReference"/> to use when compiling.</param>
        /// <param name="fixTitle">The title of the fix to apply if more than one.</param>
        /// <param name="allowCompilationErrors">If compilation errors are accepted in the fixed code.</param>
        public static void CodeFix(DiagnosticAnalyzer analyzer, CodeFixProvider fix, DiagnosticsAndSources diagnosticsAndSources, string fixedCode, IEnumerable<string> suppressedDiagnostics, IEnumerable<MetadataReference> metadataReferences, string fixTitle = null, AllowCompilationErrors allowCompilationErrors = AllowCompilationErrors.No)
        {
            CodeFix(
                analyzer: analyzer,
                fix: fix,
                diagnosticsAndSources: diagnosticsAndSources,
                fixedCode: MergeFixedCode(diagnosticsAndSources.Code, fixedCode),
                suppressedDiagnostics: suppressedDiagnostics,
                metadataReferences: metadataReferences,
                compilationOptions: null,
                fixTitle: fixTitle,
                allowCompilationErrors: allowCompilationErrors);
        }

        /// <summary>
        /// Verifies that
        /// 1. <paramref name="diagnosticsAndSources"/> produces the expected diagnostics
        /// 2. The code fix fixes the code.
        /// </summary>
        /// <param name="analyzer">The analyzer to run on the code..</param>
        /// <param name="fix">The <see cref="CodeFixProvider"/> to apply.</param>
        /// <param name="diagnosticsAndSources">The code to analyze.</param>
        /// <param name="fixedCode">The expected code produced by the code fix.</param>
        /// <param name="suppressedDiagnostics">Ids of diagnostics to suppress.</param>
        /// <param name="metadataReferences">Collection of <see cref="MetadataReference"/> to use when compiling.</param>
        /// <param name="fixTitle">The title of the fix to apply if more than one.</param>
        /// <param name="allowCompilationErrors">If compilation errors are accepted in the fixed code.</param>
        public static void CodeFix(DiagnosticAnalyzer analyzer, CodeFixProvider fix, DiagnosticsAndSources diagnosticsAndSources, IReadOnlyList<string> fixedCode, IEnumerable<string> suppressedDiagnostics = null, IEnumerable<MetadataReference> metadataReferences = null, CSharpCompilationOptions compilationOptions = null, string fixTitle = null, AllowCompilationErrors allowCompilationErrors = AllowCompilationErrors.No)
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
            VerifyFix(sln, diagnostics, analyzer, fix, fixedCode, fixTitle, allowCompilationErrors);
        }

        private static void VerifyFix(Solution sln, IReadOnlyList<ImmutableArray<Diagnostic>> diagnostics, DiagnosticAnalyzer analyzer, CodeFixProvider fix, IReadOnlyList<string> fixedCode, string fixTitle = null, AllowCompilationErrors allowCompilationErrors = AllowCompilationErrors.No)
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

            AreEqualAsync(fixedCode, fixedSolution, null).GetAwaiter().GetResult();
            if (allowCompilationErrors == AllowCompilationErrors.No)
            {
                VerifyNoCompilerErrorsAsync(fix, fixedSolution).GetAwaiter().GetResult();
            }
        }
    }
}
