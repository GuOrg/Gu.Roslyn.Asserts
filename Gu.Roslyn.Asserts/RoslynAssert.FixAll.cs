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
        /// <param name="analyzer">The analyzer to run on the code.</param>
        /// <param name="fix">The code fix to apply.</param>
        /// <param name="expectedDiagnostic">The expected diagnostic.</param>
        /// <param name="before">The code with error positions indicated.</param>
        /// <param name="after">The expected code produced by the code fix.</param>
        /// <param name="fixTitle">The title of the fix to apply if more than one.</param>
        /// <param name="allowCompilationErrors">If compilation errors are accepted in the fixed code.</param>
        /// <param name="suppressedDiagnostics">Ids of diagnostics to suppress.</param>
        /// <param name="metadataReferences">Collection of <see cref="MetadataReference"/> to use when compiling.</param>
        /// <param name="compilationOptions">The <see cref="CSharpCompilationOptions"/>.</param>
        public static void FixAll(
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
            FixAll(
                analyzer: analyzer,
                fix: fix,
                diagnosticsAndSources: DiagnosticsAndSources.Create(expectedDiagnostic, before),
                after: MergeFixedCode(new[] { before }, after),
                fixTitle: fixTitle,
                allowCompilationErrors: allowCompilationErrors,
                suppressedDiagnostics: suppressedDiagnostics,
                metadataReferences: metadataReferences,
                compilationOptions: compilationOptions);
        }

        /// <summary>
        /// Verifies that
        /// 1. <paramref name="before"/> produces the expected diagnostics
        /// 2. The code fix fixes the code.
        /// </summary>
        /// <param name="analyzer">The analyzer to run on the code.</param>
        /// <param name="fix">The code fix to apply.</param>
        /// <param name="expectedDiagnostic">The expected diagnostic.</param>
        /// <param name="before">The code with error positions indicated.</param>
        /// <param name="after">The expected code produced by the code fix.</param>
        /// <param name="fixTitle">The title of the fix to apply if more than one.</param>
        /// <param name="allowCompilationErrors">If compilation errors are accepted in the fixed code.</param>
        /// <param name="suppressedDiagnostics">Ids of diagnostics to suppress.</param>
        /// <param name="metadataReferences">Collection of <see cref="MetadataReference"/> to use when compiling.</param>
        /// <param name="compilationOptions">The <see cref="CSharpCompilationOptions"/>.</param>
        public static void FixAll(
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
            FixAll(
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
        /// 1. <paramref name="before"/> produces the expected diagnostics
        /// 2. The code fix fixes the code.
        /// </summary>
        /// <param name="analyzer">The analyzer to run on the code.</param>
        /// <param name="fix">The code fix to apply.</param>
        /// <param name="expectedDiagnostic">The expected diagnostic.</param>
        /// <param name="before">The code with error positions indicated.</param>
        /// <param name="after">The expected code produced by the code fix.</param>
        /// <param name="fixTitle">The title of the fix to apply if more than one.</param>
        /// <param name="allowCompilationErrors">If compilation errors are accepted in the fixed code.</param>
        /// <param name="suppressedDiagnostics">Ids of diagnostics to suppress.</param>
        /// <param name="metadataReferences">Collection of <see cref="MetadataReference"/> to use when compiling.</param>
        /// <param name="compilationOptions">The <see cref="CSharpCompilationOptions"/>.</param>
        public static void FixAll(
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
            FixAll(
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
        /// 1. <paramref name="before"/> produces the expected diagnostics
        /// 2. The code fix fixes the code.
        /// </summary>
        /// <param name="fix">The <see cref="CodeFixProvider"/> to apply.</param>
        /// <param name="expectedDiagnostic">The expected diagnostic.</param>
        /// <param name="before">The code to analyze.</param>
        /// <param name="after">The expected code produced by the code fix.</param>
        /// <param name="fixTitle">The title of the fix to apply if more than one.</param>
        /// <param name="allowCompilationErrors">If compilation errors are accepted in the fixed code.</param>
        /// <param name="suppressedDiagnostics">Ids of diagnostics to suppress.</param>
        /// <param name="metadataReferences">Collection of <see cref="MetadataReference"/> to use when compiling.</param>
        /// <param name="compilationOptions">The <see cref="CSharpCompilationOptions"/>.</param>
        public static void FixAll(
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
            var analyzer = new PlaceholderAnalyzer(expectedDiagnostic.Id);
            FixAll(
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
        /// 1. <paramref name="code"/> produces the expected diagnostics
        /// 2. The code fix fixes the code.
        /// </summary>
        /// <param name="fix">The <see cref="CodeFixProvider"/> to apply.</param>
        /// <param name="expectedDiagnostic">The expected diagnostic.</param>
        /// <param name="code">The code to analyze.</param>
        /// <param name="after">The expected code produced by the code fix.</param>
        /// <param name="fixTitle">The title of the fix to apply if more than one.</param>
        /// <param name="allowCompilationErrors">If compilation errors are accepted in the fixed code.</param>
        /// <param name="suppressedDiagnostics">Ids of diagnostics to suppress.</param>
        /// <param name="metadataReferences">Collection of <see cref="MetadataReference"/> to use when compiling.</param>
        /// <param name="compilationOptions">The <see cref="CSharpCompilationOptions"/>.</param>
        public static void FixAll(
            CodeFixProvider fix,
            ExpectedDiagnostic expectedDiagnostic,
            IReadOnlyList<string> code,
            string after,
            string fixTitle = null,
            AllowCompilationErrors allowCompilationErrors = AllowCompilationErrors.No,
            IEnumerable<string> suppressedDiagnostics = null,
            IEnumerable<MetadataReference> metadataReferences = null,
            CSharpCompilationOptions compilationOptions = null)
        {
            var analyzer = new PlaceholderAnalyzer(expectedDiagnostic.Id);
            FixAll(
                analyzer: analyzer,
                fix: fix,
                diagnosticsAndSources: DiagnosticsAndSources.Create(expectedDiagnostic, code),
                after: MergeFixedCode(code, after),
                fixTitle: fixTitle,
                allowCompilationErrors: allowCompilationErrors,
                suppressedDiagnostics: suppressedDiagnostics,
                metadataReferences: metadataReferences,
                compilationOptions: compilationOptions);
        }

        /// <summary>
        /// Verifies that
        /// 1. <paramref name="code"/> produces the expected diagnostics
        /// 2. The code fix fixes the code.
        /// </summary>
        /// <param name="fix">The <see cref="CodeFixProvider"/> to apply.</param>
        /// <param name="expectedDiagnostic">The expected diagnostic.</param>
        /// <param name="code">The code to analyze.</param>
        /// <param name="after">The expected code produced by the code fix.</param>
        /// <param name="fixTitle">The title of the fix to apply if more than one.</param>
        /// <param name="allowCompilationErrors">If compilation errors are accepted in the fixed code.</param>
        /// <param name="suppressedDiagnostics">Ids of diagnostics to suppress.</param>
        /// <param name="metadataReferences">Collection of <see cref="MetadataReference"/> to use when compiling.</param>
        /// <param name="compilationOptions">The <see cref="CSharpCompilationOptions"/>.</param>
        public static void FixAll(
            CodeFixProvider fix,
            ExpectedDiagnostic expectedDiagnostic,
            IReadOnlyList<string> code,
            IReadOnlyList<string> after,
            string fixTitle = null,
            AllowCompilationErrors allowCompilationErrors = AllowCompilationErrors.No,
            IEnumerable<string> suppressedDiagnostics = null,
            IEnumerable<MetadataReference> metadataReferences = null,
            CSharpCompilationOptions compilationOptions = null)
        {
            var analyzer = new PlaceholderAnalyzer(expectedDiagnostic.Id);
            FixAll(
                analyzer: analyzer,
                fix: fix,
                diagnosticsAndSources: DiagnosticsAndSources.Create(expectedDiagnostic, code),
                after: after,
                fixTitle: fixTitle,
                allowCompilationErrors: allowCompilationErrors,
                suppressedDiagnostics: suppressedDiagnostics,
                metadataReferences: metadataReferences,
                compilationOptions: compilationOptions);
        }

        /// <summary>
        /// Verifies that
        /// 1. <paramref name="before"/> produces the expected diagnostics
        /// 2. The code fix fixes the code.
        /// </summary>
        /// <param name="analyzer">The analyzer to run on the code.</param>
        /// <param name="fix">The code fix to apply.</param>
        /// <param name="before">The code with error positions indicated.</param>
        /// <param name="after">The expected code produced by the code fix.</param>
        /// <param name="fixTitle">The title of the fix to apply if more than one.</param>
        /// <param name="allowCompilationErrors">If compilation errors are accepted in the fixed code.</param>
        /// <param name="suppressedDiagnostics">Ids of diagnostics to suppress.</param>
        /// <param name="metadataReferences">Collection of <see cref="MetadataReference"/> to use when compiling.</param>
        /// <param name="compilationOptions">The <see cref="CSharpCompilationOptions"/>.</param>
        public static void FixAll(
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
            FixAll(
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
        /// 1. <paramref name="before"/> produces the expected diagnostics
        /// 2. The code fix fixes the code.
        /// </summary>
        /// <param name="analyzer">The analyzer to run on the code.</param>
        /// <param name="fix">The code fix to apply.</param>
        /// <param name="before">The code with error positions indicated.</param>
        /// <param name="after">The expected code produced by the code fix.</param>
        /// <param name="fixTitle">The title of the fix to apply if more than one.</param>
        /// <param name="allowCompilationErrors">If compilation errors are accepted in the fixed code.</param>
        /// <param name="suppressedDiagnostics">Ids of diagnostics to suppress.</param>
        /// <param name="metadataReferences">Collection of <see cref="MetadataReference"/> to use when compiling.</param>
        /// <param name="compilationOptions">The <see cref="CSharpCompilationOptions"/>.</param>
        public static void FixAll(
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
            FixAll(
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
        /// 1. <paramref name="before"/> produces the expected diagnostics
        /// 2. The code fix fixes the code.
        /// </summary>
        /// <param name="analyzer">The analyzer to run on the code.</param>
        /// <param name="fix">The code fix to apply.</param>
        /// <param name="before">The code with error positions indicated.</param>
        /// <param name="after">The expected code produced by the code fix.</param>
        /// <param name="fixTitle">The title of the fix to apply if more than one.</param>
        /// <param name="allowCompilationErrors">If compilation errors are accepted in the fixed code.</param>
        /// <param name="suppressedDiagnostics">Ids of diagnostics to suppress.</param>
        /// <param name="metadataReferences">Collection of <see cref="MetadataReference"/> to use when compiling.</param>
        /// <param name="compilationOptions">The <see cref="CSharpCompilationOptions"/>.</param>
        public static void FixAll(
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
            FixAll(
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
        /// 1. <paramref name="diagnosticsAndSources"/> produces the expected diagnostics
        /// 2. The code fix fixes the code.
        /// </summary>
        /// <param name="analyzer">The analyzer to run on the code.</param>
        /// <param name="fix">The code fix to apply.</param>
        /// <param name="diagnosticsAndSources">The code and expected diagnostics.</param>
        /// <param name="after">The expected code produced by the code fix.</param>
        /// <param name="fixTitle">The title of the fix to apply if more than one.</param>
        /// <param name="allowCompilationErrors">If compilation errors are accepted in the fixed code.</param>
        /// <param name="suppressedDiagnostics">Ids of diagnostics to suppress.</param>
        /// <param name="metadataReferences">Collection of <see cref="MetadataReference"/> to use when compiling.</param>
        /// <param name="compilationOptions">The <see cref="CSharpCompilationOptions"/>.</param>
        public static void FixAll(
            DiagnosticAnalyzer analyzer,
            CodeFixProvider fix,
            DiagnosticsAndSources diagnosticsAndSources,
            IReadOnlyList<string> after,
            string fixTitle,
            AllowCompilationErrors allowCompilationErrors = AllowCompilationErrors.No,
            IEnumerable<string> suppressedDiagnostics = null,
            IEnumerable<MetadataReference> metadataReferences = null,
            CSharpCompilationOptions compilationOptions = null)
        {
            VerifyAnalyzerSupportsDiagnostics(analyzer, diagnosticsAndSources.ExpectedDiagnostics);
            VerifyCodeFixSupportsAnalyzer(analyzer, fix);
            var sln = CodeFactory.CreateSolution(
                diagnosticsAndSources: diagnosticsAndSources,
                analyzer: analyzer,
                compilationOptions: compilationOptions,
                suppressedDiagnostics: suppressedDiagnostics ?? SuppressedDiagnostics,
                metadataReferences: metadataReferences ?? MetadataReferences);
            var diagnostics = Analyze.GetDiagnostics(analyzer, sln);
            VerifyDiagnostics(diagnosticsAndSources, diagnostics);
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
        /// <param name="analyzer">The analyzer to run on the code.</param>
        /// <param name="codeFix">The code fix to apply.</param>
        /// <param name="before">The code with error positions indicated.</param>
        /// <param name="after">The expected code produced by the code fix.</param>
        /// <param name="fixTitle">The title of the fix to apply if more than one.</param>
        /// <param name="allowCompilationErrors">If compilation errors are accepted in the fixed code.</param>
        public static void FixAllInDocument(
            DiagnosticAnalyzer analyzer,
            CodeFixProvider codeFix,
            string before,
            string after,
            string fixTitle = null,
            AllowCompilationErrors allowCompilationErrors = AllowCompilationErrors.No)
        {
            FixAllByScope(
                analyzer,
                codeFix,
                new[] { before },
                new[] { after },
                fixTitle,
                SuppressedDiagnostics,
                MetadataReferences,
                allowCompilationErrors,
                FixAllScope.Document);
        }

        /// <summary>
        /// Verifies that
        /// 1. <paramref name="before"/> produces the expected diagnostics
        /// 2. The code fix fixes the code.
        /// </summary>
        /// <param name="analyzer">The analyzer to run on the code.</param>
        /// <param name="codeFix">The code fix to apply.</param>
        /// <param name="expectedDiagnostic">The expected diagnostic.</param>
        /// <param name="before">The code to analyze.</param>
        /// <param name="after">The expected code produced by the code fix.</param>
        /// <param name="fixTitle">The title of the fix to apply if more than one.</param>
        /// <param name="allowCompilationErrors">If compilation errors are accepted in the fixed code.</param>
        public static void FixAllInDocument(
            DiagnosticAnalyzer analyzer,
            CodeFixProvider codeFix,
            ExpectedDiagnostic expectedDiagnostic,
            string before,
            string after,
            string fixTitle = null,
            AllowCompilationErrors allowCompilationErrors = AllowCompilationErrors.No)
        {
            var diagnosticsAndSources = DiagnosticsAndSources.Create(expectedDiagnostic, before);
            VerifyAnalyzerSupportsDiagnostics(analyzer, diagnosticsAndSources.ExpectedDiagnostics);
            VerifyCodeFixSupportsAnalyzer(analyzer, codeFix);
            var sln = CodeFactory.CreateSolution(diagnosticsAndSources, analyzer, null, SuppressedDiagnostics,
                MetadataReferences);
            var diagnostics = Analyze.GetDiagnostics(sln, analyzer);
            VerifyDiagnostics(diagnosticsAndSources, diagnostics);
            FixAllByScope(analyzer, codeFix, sln, new[] { after }, fixTitle, allowCompilationErrors,
                FixAllScope.Document);
        }

        /// <summary>
        /// Verifies that
        /// 1. <paramref name="before"/> produces the expected diagnostics
        /// 2. The code fix fixes the code.
        /// </summary>
        /// <param name="analyzer">The analyzer to run on the code.</param>
        /// <param name="codeFix">The code fix to apply.</param>
        /// <param name="before">The code with error positions indicated.</param>
        /// <param name="after">The expected code produced by the code fix.</param>
        /// <param name="fixTitle">The title of the fix to apply if more than one.</param>
        /// <param name="allowCompilationErrors">If compilation errors are accepted in the fixed code.</param>
        public static void FixAllOneByOne(DiagnosticAnalyzer analyzer, CodeFixProvider codeFix, string before, string after, string fixTitle = null, AllowCompilationErrors allowCompilationErrors = AllowCompilationErrors.No)
        {
            var diagnosticsAndSources = DiagnosticsAndSources.CreateFromCodeWithErrorsIndicated(analyzer, before);
            VerifyAnalyzerSupportsDiagnostics(analyzer, diagnosticsAndSources.ExpectedDiagnostics);
            VerifyCodeFixSupportsAnalyzer(analyzer, codeFix);
            var sln = CodeFactory.CreateSolution(diagnosticsAndSources, analyzer, null, SuppressedDiagnostics, MetadataReferences);
            var diagnostics = Analyze.GetDiagnostics(analyzer, sln);
            VerifyDiagnostics(diagnosticsAndSources, diagnostics);
            FixAllOneByOne(analyzer, codeFix, sln, new[] { after }, fixTitle, allowCompilationErrors);
        }

        /// <summary>
        /// Verifies that
        /// 1. <paramref name="code"/> produces the expected diagnostics
        /// 2. The code fix fixes the code.
        /// </summary>
        /// <param name="analyzer">The analyzer to run on the code.</param>
        /// <param name="codeFix">The code fix to apply.</param>
        /// <param name="expectedDiagnostic">The expected diagnostic.</param>
        /// <param name="code">The code to analyze.</param>
        /// <param name="after">The expected code produced by the code fix.</param>
        /// <param name="fixTitle">The title of the fix to apply if more than one.</param>
        /// <param name="allowCompilationErrors">If compilation errors are accepted in the fixed code.</param>
        public static void FixAllOneByOne(DiagnosticAnalyzer analyzer, CodeFixProvider codeFix, ExpectedDiagnostic expectedDiagnostic, string code, string after, string fixTitle = null, AllowCompilationErrors allowCompilationErrors = AllowCompilationErrors.No)
        {
            var diagnosticsAndSources = DiagnosticsAndSources.Create(expectedDiagnostic, code);
            VerifyAnalyzerSupportsDiagnostics(analyzer, diagnosticsAndSources.ExpectedDiagnostics);
            VerifyCodeFixSupportsAnalyzer(analyzer, codeFix);
            var sln = CodeFactory.CreateSolution(diagnosticsAndSources, analyzer, null, SuppressedDiagnostics, MetadataReferences);
            var diagnostics = Analyze.GetDiagnostics(analyzer, sln);
            VerifyDiagnostics(diagnosticsAndSources, diagnostics);
            FixAllOneByOne(analyzer, codeFix, sln, new[] { after }, fixTitle, allowCompilationErrors);
        }

        /// <summary>
        /// Verifies that
        /// 1. <paramref name="code"/> produces the expected diagnostics
        /// 2. The code fix fixes the code.
        /// </summary>
        /// <param name="codeFix">The code fix to apply.</param>
        /// <param name="expectedDiagnostic">The expected diagnostic.</param>
        /// <param name="code">The code to analyze.</param>
        /// <param name="after">The expected code produced by the code fix.</param>
        /// <param name="fixTitle">The title of the fix to apply if more than one.</param>
        /// <param name="allowCompilationErrors">If compilation errors are accepted in the fixed code.</param>
        public static void FixAllOneByOne(CodeFixProvider codeFix, ExpectedDiagnostic expectedDiagnostic, string code, string after, string fixTitle = null, AllowCompilationErrors allowCompilationErrors = AllowCompilationErrors.No)
        {
            var analyzer = new PlaceholderAnalyzer(expectedDiagnostic.Id);
            var diagnosticsAndSources = DiagnosticsAndSources.Create(expectedDiagnostic, code);
            VerifyAnalyzerSupportsDiagnostics(analyzer, diagnosticsAndSources.ExpectedDiagnostics);
            VerifyCodeFixSupportsAnalyzer(analyzer, codeFix);
            var sln = CodeFactory.CreateSolution(diagnosticsAndSources, analyzer, null, SuppressedDiagnostics, MetadataReferences);
            var diagnostics = Analyze.GetDiagnostics(analyzer, sln);
            VerifyDiagnostics(diagnosticsAndSources, diagnostics);
            FixAllOneByOne(analyzer, codeFix, sln, new[] { after }, fixTitle, allowCompilationErrors);
        }

        /// <summary>
        /// Verifies that
        /// 1. <paramref name="code"/> produces the expected diagnostics
        /// 2. The code fix fixes the code.
        /// </summary>
        /// <param name="codeFix">The code fix to apply.</param>
        /// <param name="expectedDiagnostic">The expected diagnostic.</param>
        /// <param name="code">The code with error positions indicated.</param>
        /// <param name="after">The expected code produced by the code fix.</param>
        /// <param name="fixTitle">The title of the fix to apply if more than one.</param>
        /// <param name="suppressedDiagnostics">The diagnostics to suppress when compiling.</param>
        /// <param name="metadataReferences">The meta data metadataReferences to add to the compilation.</param>
        /// <param name="allowCompilationErrors">If compilation errors are accepted in the fixed code.</param>
        /// <param name="scope">The scope to apply fixes for.</param>
        public static void FixAllByScope(CodeFixProvider codeFix, ExpectedDiagnostic expectedDiagnostic, IReadOnlyList<string> code, IReadOnlyList<string> after, string fixTitle, IEnumerable<string> suppressedDiagnostics, IEnumerable<MetadataReference> metadataReferences, AllowCompilationErrors allowCompilationErrors, FixAllScope scope)
        {
            var analyzer = new PlaceholderAnalyzer(expectedDiagnostic.Id);
            var diagnosticsAndSources = DiagnosticsAndSources.Create(expectedDiagnostic, code);
            VerifyAnalyzerSupportsDiagnostics(analyzer, diagnosticsAndSources.ExpectedDiagnostics);
            VerifyCodeFixSupportsAnalyzer(analyzer, codeFix);
            var sln = CodeFactory.CreateSolution(diagnosticsAndSources, analyzer, null, suppressedDiagnostics, metadataReferences);
            var diagnostics = Analyze.GetDiagnostics(sln, analyzer);
            VerifyDiagnostics(diagnosticsAndSources, diagnostics);
            FixAllByScope(analyzer, codeFix, sln, after, fixTitle, allowCompilationErrors, scope);
        }

        /// <summary>
        /// Verifies that
        /// 1. <paramref name="code"/> produces the expected diagnostics
        /// 2. The code fix fixes the code.
        /// </summary>
        /// <param name="analyzer">The analyzer to run on the code.</param>
        /// <param name="codeFix">The code fix to apply.</param>
        /// <param name="code">The code with error positions indicated.</param>
        /// <param name="after">The expected code produced by the code fix.</param>
        /// <param name="fixTitle">The title of the fix to apply if more than one.</param>
        /// <param name="suppressedDiagnostics">The diagnostics to suppress when compiling.</param>
        /// <param name="metadataReferences">The meta data metadataReferences to add to the compilation.</param>
        /// <param name="allowCompilationErrors">If compilation errors are accepted in the fixed code.</param>
        /// <param name="scope">The scope to apply fixes for.</param>
        public static void FixAllByScope(DiagnosticAnalyzer analyzer, CodeFixProvider codeFix, IReadOnlyList<string> code, IReadOnlyList<string> after, string fixTitle, IEnumerable<string> suppressedDiagnostics, IEnumerable<MetadataReference> metadataReferences, AllowCompilationErrors allowCompilationErrors, FixAllScope scope)
        {
            var diagnosticsAndSources = DiagnosticsAndSources.CreateFromCodeWithErrorsIndicated(analyzer, code);
            VerifyAnalyzerSupportsDiagnostics(analyzer, diagnosticsAndSources.ExpectedDiagnostics);
            VerifyCodeFixSupportsAnalyzer(analyzer, codeFix);
            var sln = CodeFactory.CreateSolution(diagnosticsAndSources, analyzer, null, suppressedDiagnostics, metadataReferences);
            var diagnostics = Analyze.GetDiagnostics(sln, analyzer);
            VerifyDiagnostics(diagnosticsAndSources, diagnostics);
            FixAllByScope(analyzer, codeFix, sln, after, fixTitle, allowCompilationErrors, scope);
        }

        /// <summary>
        /// Verifies that
        /// 1. <paramref name="code"/> produces the expected diagnostics
        /// 2. The code fix fixes the code.
        /// </summary>
        /// <param name="analyzer">The analyzer to run on the code.</param>
        /// <param name="codeFix">The code fix to apply.</param>
        /// <param name="expectedDiagnostic">The expected diagnostic.</param>
        /// <param name="code">The code with error positions indicated.</param>
        /// <param name="after">The expected code produced by the code fix.</param>
        /// <param name="fixTitle">The title of the fix to apply if more than one.</param>
        /// <param name="suppressedDiagnostics">The diagnostics to suppress when compiling.</param>
        /// <param name="metadataReferences">The meta data metadataReferences to add to the compilation.</param>
        /// <param name="allowCompilationErrors">If compilation errors are accepted in the fixed code.</param>
        /// <param name="scope">The scope to apply fixes for.</param>
        public static void FixAllByScope(DiagnosticAnalyzer analyzer, CodeFixProvider codeFix, ExpectedDiagnostic expectedDiagnostic, IReadOnlyList<string> code, IReadOnlyList<string> after, string fixTitle, IEnumerable<string> suppressedDiagnostics, IEnumerable<MetadataReference> metadataReferences, AllowCompilationErrors allowCompilationErrors, FixAllScope scope)
        {
            var diagnosticsAndSources = DiagnosticsAndSources.Create(expectedDiagnostic, code);
            VerifyAnalyzerSupportsDiagnostics(analyzer, diagnosticsAndSources.ExpectedDiagnostics);
            VerifyCodeFixSupportsAnalyzer(analyzer, codeFix);
            var sln = CodeFactory.CreateSolution(diagnosticsAndSources, analyzer, null, suppressedDiagnostics, metadataReferences);
            var diagnostics = Analyze.GetDiagnostics(sln, analyzer);
            VerifyDiagnostics(diagnosticsAndSources, diagnostics);
            FixAllByScope(analyzer, codeFix, sln, after, fixTitle, allowCompilationErrors, scope);
        }

        private static void FixAllOneByOne(DiagnosticAnalyzer analyzer, CodeFixProvider codeFix, Solution solution, IReadOnlyList<string> after, string fixTitle, AllowCompilationErrors allowCompilationErrors)
        {
            var fixedSolution = Fix.ApplyAllFixableOneByOneAsync(solution, analyzer, codeFix, fixTitle, CancellationToken.None).GetAwaiter().GetResult();
            AreEqualAsync(after, fixedSolution, "Applying fixes one by one failed.").GetAwaiter().GetResult();
            if (allowCompilationErrors == AllowCompilationErrors.No)
            {
                VerifyNoCompilerErrorsAsync(codeFix, fixedSolution).ConfigureAwait(false).GetAwaiter().GetResult();
            }
        }

        private static void FixAllByScope(DiagnosticAnalyzer analyzer, CodeFixProvider codeFix, Solution sln, IReadOnlyList<string> after, string fixTitle, AllowCompilationErrors allowCompilationErrors, FixAllScope scope)
        {
            VerifyCodeFixSupportsAnalyzer(analyzer, codeFix);
            var fixedSolution = Fix.ApplyAllFixableScopeByScopeAsync(sln, analyzer, codeFix, scope, fixTitle, CancellationToken.None).GetAwaiter().GetResult();
            AreEqualAsync(after, fixedSolution, $"Applying fixes for {scope} failed.").GetAwaiter().GetResult();
            if (allowCompilationErrors == AllowCompilationErrors.No)
            {
                VerifyNoCompilerErrorsAsync(codeFix, fixedSolution).GetAwaiter().GetResult();
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
