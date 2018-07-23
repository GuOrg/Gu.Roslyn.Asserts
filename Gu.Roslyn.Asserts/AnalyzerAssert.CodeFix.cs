namespace Gu.Roslyn.Asserts
{
    using System;
    using System.Collections.Generic;
    using System.Collections.Immutable;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CodeFixes;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.Diagnostics;
    using Microsoft.CodeAnalysis.Formatting;

    public static partial class AnalyzerAssert
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
            var fix = new TCodeFix();
            CodeFixSupportsAnalyzer(analyzer, fix);
            var diagnosticsAndSources = DiagnosticsAndSources.CreateFromCodeWithErrorsIndicated(analyzer, codeWithErrorsIndicated);
            var sln = CodeFactory.CreateSolution(diagnosticsAndSources, analyzer, SuppressedDiagnostics, MetadataReferences);
            var diagnostics = Analyze.GetDiagnostics(analyzer, sln);
            VerifyDiagnostics(diagnosticsAndSources, diagnostics);
            VerifyFix(sln, diagnostics, analyzer, fix, fixedCode, fixTitle, allowCompilationErrors);
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
            var fix = new TCodeFix();
            CodeFixSupportsAnalyzer(analyzer, fix);
            var diagnosticsAndSources = DiagnosticsAndSources.CreateFromCodeWithErrorsIndicated(analyzer, codeWithErrorsIndicated);
            var sln = CodeFactory.CreateSolution(diagnosticsAndSources, analyzer, SuppressedDiagnostics, MetadataReferences);
            var diagnostics = Analyze.GetDiagnostics(analyzer, sln);
            VerifyDiagnostics(diagnosticsAndSources, diagnostics);
            VerifyFix(sln, diagnostics, analyzer, fix, fixedCode, fixTitle, allowCompilationErrors);
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
            CodeFixSupportsAnalyzer(analyzer, fix);
            var diagnosticsAndSources = DiagnosticsAndSources.CreateFromCodeWithErrorsIndicated(analyzer, codeWithErrorsIndicated);
            var sln = CodeFactory.CreateSolution(diagnosticsAndSources, analyzer, SuppressedDiagnostics, MetadataReferences);
            var diagnostics = Analyze.GetDiagnostics(analyzer, sln);
            VerifyDiagnostics(diagnosticsAndSources, diagnostics);
            VerifyFix(sln, diagnostics, analyzer, fix, fixedCode, fixTitle, allowCompilationErrors);
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
        public static void CodeFix(DiagnosticAnalyzer analyzer, CodeFixProvider fix, IReadOnlyList<string> codeWithErrorsIndicated, string fixedCode, string fixTitle = null, AllowCompilationErrors allowCompilationErrors = AllowCompilationErrors.No)
        {
            CodeFixSupportsAnalyzer(analyzer, fix);
            var diagnosticsAndSources = DiagnosticsAndSources.CreateFromCodeWithErrorsIndicated(analyzer, codeWithErrorsIndicated);
            var sln = CodeFactory.CreateSolution(diagnosticsAndSources, analyzer, SuppressedDiagnostics, MetadataReferences);
            var diagnostics = Analyze.GetDiagnostics(analyzer, sln);
            VerifyDiagnostics(diagnosticsAndSources, diagnostics);
            VerifyFix(sln, diagnostics, analyzer, fix, fixedCode, fixTitle, allowCompilationErrors);
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
            AnalyzerSupportsDiagnostic(analyzer, expectedDiagnostic);
            CodeFixSupportsAnalyzer(analyzer, fix);
            var diagnostics = Analyze.GetDiagnostics(analyzer, solution);
            var diagnosticsAndSources = DiagnosticsAndSources.Create(expectedDiagnostic, solution.Projects.SelectMany(x => x.Documents).Select(x => CodeReader.GetCode(x, null)).ToArray());
            VerifyDiagnostics(diagnosticsAndSources, diagnostics);
            VerifyFix(solution, diagnostics, analyzer, fix, fixedCode, fixTitle, allowCompilationErrors);
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
            var analyzer = new TAnalyzer();
            AnalyzerSupportsDiagnostic(analyzer, expectedDiagnostic);
            var fix = new TCodeFix();
            CodeFixSupportsAnalyzer(analyzer, fix);
            var diagnosticsAndSources = DiagnosticsAndSources.Create(expectedDiagnostic, new[] { code });
            var sln = CodeFactory.CreateSolution(diagnosticsAndSources, analyzer, SuppressedDiagnostics, MetadataReferences);
            var diagnostics = Analyze.GetDiagnostics(analyzer, sln);
            VerifyDiagnostics(diagnosticsAndSources, diagnostics);
            VerifyFix(sln, diagnostics, analyzer, fix, fixedCode, fixTitle, allowCompilationErrors);
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
            var analyzer = new TAnalyzer();
            AnalyzerSupportsDiagnostic(analyzer, expectedDiagnostic);
            var fix = new TCodeFix();
            CodeFixSupportsAnalyzer(analyzer, fix);
            var diagnosticsAndSources = DiagnosticsAndSources.Create(expectedDiagnostic, code);
            var sln = CodeFactory.CreateSolution(diagnosticsAndSources, analyzer, SuppressedDiagnostics, MetadataReferences);
            var diagnostics = Analyze.GetDiagnostics(analyzer, sln);
            VerifyDiagnostics(diagnosticsAndSources, diagnostics);
            VerifyFix(sln, diagnostics, analyzer, fix, fixedCode, fixTitle, allowCompilationErrors);
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
            CodeFixSupportsAnalyzer(analyzer, fix);
            AnalyzerSupportsDiagnostic(analyzer, expectedDiagnostic);
            var diagnosticsAndSources = DiagnosticsAndSources.Create(expectedDiagnostic, new[] { code });
            var sln = CodeFactory.CreateSolution(diagnosticsAndSources, analyzer, SuppressedDiagnostics, MetadataReferences);
            var diagnostics = Analyze.GetDiagnostics(analyzer, sln);
            VerifyDiagnostics(diagnosticsAndSources, diagnostics);
            VerifyFix(sln, diagnostics, analyzer, fix, fixedCode, fixTitle, allowCompilationErrors);
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
            CodeFixSupportsAnalyzer(analyzer, fix);
            AnalyzerSupportsDiagnostic(analyzer, expectedDiagnostic);
            var diagnosticsAndSources = DiagnosticsAndSources.Create(expectedDiagnostic, code);
            var sln = CodeFactory.CreateSolution(diagnosticsAndSources, analyzer, SuppressedDiagnostics, MetadataReferences);
            var diagnostics = Analyze.GetDiagnostics(analyzer, sln);
            VerifyDiagnostics(diagnosticsAndSources, diagnostics);
            VerifyFix(sln, diagnostics, analyzer, fix, fixedCode, fixTitle, allowCompilationErrors);
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
            var analyzer = new PlaceholderAnalyzer(expectedDiagnostic.Id);
            var fix = new TCodeFix();
            CodeFixSupportsAnalyzer(analyzer, fix);
            var diagnosticsAndSources = DiagnosticsAndSources.Create(expectedDiagnostic, new[] { code });
            var sln = CodeFactory.CreateSolution(diagnosticsAndSources, analyzer, SuppressedDiagnostics, MetadataReferences);
            var diagnostics = Analyze.GetDiagnostics(analyzer, sln);
            VerifyDiagnostics(diagnosticsAndSources, diagnostics);
            VerifyFix(sln, diagnostics, analyzer, fix, fixedCode, fixTitle, allowCompilationErrors);
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
            var analyzer = new PlaceholderAnalyzer(expectedDiagnostic.Id);
            var fix = new TCodeFix();
            CodeFixSupportsAnalyzer(analyzer, fix);
            var diagnosticsAndSources = DiagnosticsAndSources.Create(expectedDiagnostic, code);
            var sln = CodeFactory.CreateSolution(diagnosticsAndSources, analyzer, SuppressedDiagnostics, MetadataReferences);
            var diagnostics = Analyze.GetDiagnostics(analyzer, sln);
            VerifyDiagnostics(diagnosticsAndSources, diagnostics);
            VerifyFix(sln, diagnostics, analyzer, fix, fixedCode, fixTitle, allowCompilationErrors);
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
            var analyzer = new PlaceholderAnalyzer(expectedDiagnostic.Id);
            CodeFixSupportsAnalyzer(analyzer, fix);
            var diagnosticsAndSources = DiagnosticsAndSources.Create(expectedDiagnostic, new[] { code });
            var sln = CodeFactory.CreateSolution(diagnosticsAndSources, analyzer, SuppressedDiagnostics, MetadataReferences);
            var diagnostics = Analyze.GetDiagnostics(analyzer, sln);
            VerifyDiagnostics(diagnosticsAndSources, diagnostics);
            VerifyFix(sln, diagnostics, analyzer, fix, fixedCode, fixTitle, allowCompilationErrors);
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
            var analyzer = new PlaceholderAnalyzer(expectedDiagnostic.Id);
            CodeFixSupportsAnalyzer(analyzer, fix);
            var diagnosticsAndSources = DiagnosticsAndSources.Create(expectedDiagnostic, code);
            var sln = CodeFactory.CreateSolution(diagnosticsAndSources, analyzer, SuppressedDiagnostics, MetadataReferences);
            var diagnostics = Analyze.GetDiagnostics(analyzer, sln);
            VerifyDiagnostics(diagnosticsAndSources, diagnostics);
            VerifyFix(sln, diagnostics, analyzer, fix, fixedCode, fixTitle, allowCompilationErrors);
        }

        /// <summary>
        /// Verifies that
        /// 1. <paramref name="diagnosticsAndSources"/> produces the expected diagnostics when analyzed.
        /// 2. The code fix fixes the code.
        /// </summary>
        /// <param name="analyzer">The analyzer to run on the code..</param>
        /// <param name="fix">The code fix to apply.</param>
        /// <param name="diagnosticsAndSources">The code and expected diagnostics.</param>
        /// <param name="fixedCode">The expected code produced by the code fix.</param>
        /// <param name="fixTitle">The title of the fix to apply if more than one.</param>
        /// <param name="compilationOptions">The <see cref="CSharpCompilationOptions"/> to use.</param>
        /// <param name="metadataReferences">The meta data metadataReferences to add to the compilation.</param>
        /// <param name="allowCompilationErrors">If compilation errors are accepted in the fixed code.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        public static async Task CodeFixAsync(DiagnosticAnalyzer analyzer, CodeFixProvider fix, DiagnosticsAndSources diagnosticsAndSources, string fixedCode, string fixTitle, CSharpCompilationOptions compilationOptions, IReadOnlyList<MetadataReference> metadataReferences, AllowCompilationErrors allowCompilationErrors)
        {
            CodeFixSupportsAnalyzer(analyzer, fix);
            var data = await DiagnosticsWithMetadataAsync(analyzer, diagnosticsAndSources, compilationOptions, metadataReferences).ConfigureAwait(false);
            var fixableDiagnostics = data.ActualDiagnostics.SelectMany(x => x)
                                         .Where(x => fix.FixableDiagnosticIds.Contains(x.Id))
                                         .ToArray();
            if (fixableDiagnostics.Length == 0)
            {
                var message = $"Code analyzed with {analyzer} did not generate any diagnostics fixable by {fix}.{Environment.NewLine}" +
                              $"The analyzed code contained the following diagnostics: {{{string.Join(", ", data.ExpectedDiagnostics.Select(d => d.Id))}}}{Environment.NewLine}" +
                              $"The code fix supports the following diagnostics: {{{string.Join(", ", fix.FixableDiagnosticIds)}}}";
                throw AssertException.Create(message);
            }

            if (fixableDiagnostics.Length > 1)
            {
                var message = $"Code analyzed with {analyzer} generated more than one diagnostic fixable by {fix}.{Environment.NewLine}" +
                              $"The analyzed code contained the following diagnostics: {{{string.Join(", ", data.ExpectedDiagnostics.Select(d => d.Id))}}}{Environment.NewLine}" +
                              $"The code fix supports the following diagnostics: {{{string.Join(", ", fix.FixableDiagnosticIds)}}}{Environment.NewLine}" +
                              $"Maybe you meant to call AnalyzerAssert.FixAll?";
                throw AssertException.Create(message);
            }

            var diagnostic = fixableDiagnostics.Single();
            var fixedSolution = await Fix.ApplyAsync(data.Solution, fix, diagnostic, fixTitle, CancellationToken.None).ConfigureAwait(false);
            if (ReferenceEquals(data.Solution, fixedSolution))
            {
                throw AssertException.Create($"{fix} did not change any document.");
            }

            var fixedSource = await CodeReader.GetStringFromDocumentAsync(
                                                  fixedSolution.GetDocument(data.Solution.GetDocument(diagnostic.Location.SourceTree).Id),
                                                  Formatter.Annotation,
                                                  CancellationToken.None)
                                              .ConfigureAwait(false);
            CodeAssert.AreEqual(fixedCode, fixedSource);

            if (allowCompilationErrors == AllowCompilationErrors.No)
            {
                await AssertNoCompilerErrorsAsync(fix, fixedSolution).ConfigureAwait(false);
            }
        }

        /// <summary>
        /// Verifies that
        /// 1. <paramref name="code"/> produces the expected diagnostics
        /// 2. The code fix fixes the code.
        /// </summary>
        /// <typeparam name="TCodeFix">The type of the code fix.</typeparam>
        /// <param name="id">The id of the expected diagnostic.</param>
        /// <param name="code">The code to analyze.</param>
        /// <param name="fixedCode">The expected code produced by the code fix.</param>
        /// <param name="fixTitle">The title of the fix to apply if more than one.</param>
        /// <param name="allowCompilationErrors">If compilation errors are accepted in the fixed code.</param>
        [Obsolete("Use overload with ExpectedDiagnostic")]
        public static void CodeFix<TCodeFix>(string id, string code, string fixedCode, string fixTitle = null, AllowCompilationErrors allowCompilationErrors = AllowCompilationErrors.No)
            where TCodeFix : CodeFixProvider, new()
        {
            var analyzer = new PlaceholderAnalyzer(id);
            CodeFixAsync(
                    analyzer,
                    new TCodeFix(),
                    DiagnosticsAndSources.CreateFromCodeWithErrorsIndicated(analyzer, new[] { code }),
                    fixedCode,
                    fixTitle,
                    CodeFactory.DefaultCompilationOptions(analyzer, SuppressedDiagnostics),
                    MetadataReferences,
                    allowCompilationErrors)
                .GetAwaiter()
                .GetResult();
        }

        /// <summary>
        /// Verifies that
        /// 1. <paramref name="code"/> produces the expected diagnostics
        /// 2. The code fix fixes the code.
        /// </summary>
        /// <typeparam name="TCodeFix">The type of the code fix.</typeparam>
        /// <param name="id">The id of the expected diagnostic.</param>
        /// <param name="code">The code to analyze.</param>
        /// <param name="fixedCode">The expected code produced by the code fix.</param>
        /// <param name="fixTitle">The title of the fix to apply if more than one.</param>
        /// <param name="allowCompilationErrors">If compilation errors are accepted in the fixed code.</param>
        [Obsolete("Use overload with ExpectedDiagnostic")]
        public static void CodeFix<TCodeFix>(string id, IReadOnlyList<string> code, string fixedCode, string fixTitle = null, AllowCompilationErrors allowCompilationErrors = AllowCompilationErrors.No)
            where TCodeFix : CodeFixProvider, new()
        {
            var analyzer = new PlaceholderAnalyzer(id);
            CodeFixAsync(
                analyzer,
                new TCodeFix(),
                DiagnosticsAndSources.CreateFromCodeWithErrorsIndicated(analyzer, code),
                fixedCode,
                fixTitle,
                CodeFactory.DefaultCompilationOptions(analyzer, SuppressedDiagnostics),
                MetadataReferences,
                allowCompilationErrors)
                .GetAwaiter()
                .GetResult();
        }

        private static void VerifyFix(Solution sln, IReadOnlyList<ImmutableArray<Diagnostic>> diagnostics, DiagnosticAnalyzer analyzer, CodeFixProvider fix, string fixedCode, string fixTitle = null, AllowCompilationErrors allowCompilationErrors = AllowCompilationErrors.No)
        {
            var fixableDiagnostics = diagnostics.SelectMany(x => x)
                                                .Where(x => fix.FixableDiagnosticIds.Contains(x.Id))
                                                .ToArray();
            if (fixableDiagnostics.Length == 0)
            {
                var message = $"Code analyzed with {analyzer} did not generate any diagnostics fixable by {fix}.{Environment.NewLine}" +
                              $"The analyzed code contained the following diagnostics: {{{string.Join(", ", diagnostics.SelectMany(x => x).Select(d => d.Id))}}}{Environment.NewLine}" +
                              $"The code fix supports the following diagnostics: {{{string.Join(", ", fix.FixableDiagnosticIds)}}}";
                throw AssertException.Create(message);
            }

            if (fixableDiagnostics.Length > 1)
            {
                var message = $"Code analyzed with {analyzer} generated more than one diagnostic fixable by {fix}.{Environment.NewLine}" +
                              $"The analyzed code contained the following diagnostics: {{{string.Join(", ", diagnostics.SelectMany(x => x).Select(d => d.Id))}}}{Environment.NewLine}" +
                              $"The code fix supports the following diagnostics: {{{string.Join(", ", fix.FixableDiagnosticIds)}}}{Environment.NewLine}" +
                              $"Maybe you meant to call AnalyzerAssert.FixAll?";
                throw AssertException.Create(message);
            }

            var diagnostic = fixableDiagnostics.Single();
            var fixedSolution = Fix.Apply(sln, fix, diagnostic, fixTitle);
            if (ReferenceEquals(sln, fixedSolution))
            {
                throw AssertException.Create($"{fix} did not change any document.");
            }

            var fixedSource = CodeReader.GetStringFromDocumentAsync(
                                                  fixedSolution.GetDocument(sln.GetDocument(diagnostic.Location.SourceTree).Id),
                                                  Formatter.Annotation,
                                                  CancellationToken.None)
                                              .GetAwaiter().GetResult();
            CodeAssert.AreEqual(fixedCode, fixedSource);

            if (allowCompilationErrors == AllowCompilationErrors.No)
            {
                AssertNoCompilerErrorsAsync(fix, fixedSolution).GetAwaiter().GetResult();
            }
        }
    }
}
