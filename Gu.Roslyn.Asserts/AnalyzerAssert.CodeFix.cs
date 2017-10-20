namespace Gu.Roslyn.Asserts
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Gu.Roslyn.Asserts.Internals;
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
            CodeFixAsync(
                    analyzer,
                    new TCodeFix(),
                    new[] { codeWithErrorsIndicated },
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
        /// 1. <paramref name="codeWithErrorsIndicated"/> produces the expected diagnostics
        /// 2. The code fix fixes the code.
        /// </summary>
        /// <typeparam name="TCodeFix">The type of the code fix.</typeparam>
        /// <param name="id">The id of the expected diagnostic.</param>
        /// <param name="codeWithErrorsIndicated">The code with error positions indicated.</param>
        /// <param name="fixedCode">The expected code produced by the code fix.</param>
        /// <param name="fixTitle">The title of the fix to apply if more than one.</param>
        /// <param name="allowCompilationErrors">If compilation errors are accepted in the fixed code.</param>
        public static void CodeFix<TCodeFix>(string id, string codeWithErrorsIndicated, string fixedCode, string fixTitle = null, AllowCompilationErrors allowCompilationErrors = AllowCompilationErrors.No)
            where TCodeFix : CodeFixProvider, new()
        {
            var analyzer = new PlaceholderAnalyzer(id);
            CodeFixAsync(
                analyzer,
                new TCodeFix(),
                new[] { codeWithErrorsIndicated },
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
            CodeFixAsync(
                analyzer,
                new TCodeFix(),
                codeWithErrorsIndicated,
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
        /// 1. <paramref name="codeWithErrorsIndicated"/> produces the expected diagnostics
        /// 2. The code fix fixes the code.
        /// </summary>
        /// <typeparam name="TCodeFix">The type of the code fix.</typeparam>
        /// <param name="id">The id of the expected diagnostic.</param>
        /// <param name="codeWithErrorsIndicated">The code with error positions indicated.</param>
        /// <param name="fixedCode">The expected code produced by the code fix.</param>
        /// <param name="fixTitle">The title of the fix to apply if more than one.</param>
        /// <param name="allowCompilationErrors">If compilation errors are accepted in the fixed code.</param>
        public static void CodeFix<TCodeFix>(string id, IReadOnlyList<string> codeWithErrorsIndicated, string fixedCode, string fixTitle = null, AllowCompilationErrors allowCompilationErrors = AllowCompilationErrors.No)
            where TCodeFix : CodeFixProvider, new()
        {
            var analyzer = new PlaceholderAnalyzer(id);
            CodeFixAsync(
                analyzer,
                new TCodeFix(),
                codeWithErrorsIndicated,
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
        /// 1. <paramref name="codeWithErrorsIndicated"/> produces the expected diagnostics
        /// 2. The code fix fixes the code.
        /// </summary>
        /// <param name="analyzer">The analyzer to run on the code..</param>
        /// <param name="codeFix">The code fix to apply.</param>
        /// <param name="codeWithErrorsIndicated">The code with error positions indicated.</param>
        /// <param name="fixedCode">The expected code produced by the code fix.</param>
        /// <param name="fixTitle">The title of the fix to apply if more than one.</param>
        /// <param name="allowCompilationErrors">If compilation errors are accepted in the fixed code.</param>
        public static void CodeFix(DiagnosticAnalyzer analyzer, CodeFixProvider codeFix, IReadOnlyList<string> codeWithErrorsIndicated, string fixedCode, string fixTitle = null, AllowCompilationErrors allowCompilationErrors = AllowCompilationErrors.No)
        {
            CodeFixAsync(
                    analyzer,
                    codeFix,
                    codeWithErrorsIndicated,
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
        /// 1. <paramref name="codeWithErrorsIndicated"/> produces the expected diagnostics
        /// 2. The code fix fixes the code.
        /// </summary>
        /// <param name="analyzer">The analyzer to run on the code..</param>
        /// <param name="codeFix">The code fix to apply.</param>
        /// <param name="codeWithErrorsIndicated">The code with error positions indicated.</param>
        /// <param name="fixedCode">The expected code produced by the code fix.</param>
        /// <param name="fixTitle">The title of the fix to apply if more than one.</param>
        /// <param name="compilationOptions">The <see cref="CSharpCompilationOptions"/> to use.</param>
        /// <param name="metadataReferences">The meta data metadataReferences to add to the compilation.</param>
        /// <param name="allowCompilationErrors">If compilation errors are accepted in the fixed code.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        public static async Task CodeFixAsync(DiagnosticAnalyzer analyzer, CodeFixProvider codeFix, IReadOnlyList<string> codeWithErrorsIndicated, string fixedCode, string fixTitle, CSharpCompilationOptions compilationOptions, IReadOnlyList<MetadataReference> metadataReferences, AllowCompilationErrors allowCompilationErrors)
        {
            if (analyzer.SupportedDiagnostics.Length != 1)
            {
                var message = $"The analyzer supports multiple diagnostics {{{string.Join(", ", analyzer.SupportedDiagnostics.Select(d => d.Id))}}}.{Environment.NewLine}" +
                              $"This method can only be used with analyzers that have exactly one SupportedDiagnostic";
                throw AssertException.Create(message);
            }

            AssertCodeFixCanFixDiagnosticsFromAnalyzer(analyzer, codeFix);
            var diagnosticsAndSources = DiagnosticsAndSources.CreateFromCodeWithErrorsIndicated(analyzer, codeWithErrorsIndicated);
            var data = await DiagnosticsWithMetadataAsync(analyzer, diagnosticsAndSources, compilationOptions, metadataReferences).ConfigureAwait(false);
            var fixableDiagnostics = data.ActualDiagnostics.SelectMany(x => x)
                                         .Where(x => codeFix.FixableDiagnosticIds.Contains(x.Id))
                                         .ToArray();
            if (fixableDiagnostics.Length == 0)
            {
                var message = $"Code analyzed with {analyzer} did not generate any diagnostics fixable by {codeFix}.{Environment.NewLine}" +
                              $"The analyzed code contained the following diagnostics: {{{string.Join(", ", data.ExpectedDiagnostics.Select(d => d.Id))}}}{Environment.NewLine}" +
                              $"The code fix supports the following diagnostics: {{{string.Join(", ", codeFix.FixableDiagnosticIds)}}}";
                throw AssertException.Create(message);
            }

            if (fixableDiagnostics.Length > 1)
            {
                var message = $"Code analyzed with {analyzer} generated more than one diagnostic fixable by {codeFix}.{Environment.NewLine}" +
                              $"The analyzed code contained the following diagnostics: {{{string.Join(", ", data.ExpectedDiagnostics.Select(d => d.Id))}}}{Environment.NewLine}" +
                              $"The code fix supports the following diagnostics: {{{string.Join(", ", codeFix.FixableDiagnosticIds)}}}{Environment.NewLine}" +
                              $"Maybe you meant to call AnalyzerAssert.FixAll?";
                throw AssertException.Create(message);
            }

            var diagnostic = fixableDiagnostics.Single();
            var fixedSolution = await Fix.ApplyAsync(data.Solution, codeFix, diagnostic, fixTitle, CancellationToken.None).ConfigureAwait(false);
            if (ReferenceEquals(data.Solution, fixedSolution))
            {
                throw AssertException.Create($"{codeFix} did not change any document.");
            }

            var fixedSource = await CodeReader.GetStringFromDocumentAsync(
                                                  fixedSolution.GetDocument(data.Solution.GetDocument(diagnostic.Location.SourceTree).Id),
                                                  Formatter.Annotation,
                                                  CancellationToken.None)
                                              .ConfigureAwait(false);
            CodeAssert.AreEqual(fixedCode, fixedSource);

            if (allowCompilationErrors == AllowCompilationErrors.No)
            {
                await AssertNoCompilerErrorsAsync(codeFix, fixedSolution).ConfigureAwait(false);
            }
        }
    }
}