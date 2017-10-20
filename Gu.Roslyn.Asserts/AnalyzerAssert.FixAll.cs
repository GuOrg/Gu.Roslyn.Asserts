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
        public static void FixAll<TAnalyzer, TCodeFix>(string codeWithErrorsIndicated, string fixedCode, string fixTitle = null, AllowCompilationErrors allowCompilationErrors = AllowCompilationErrors.No)
            where TAnalyzer : DiagnosticAnalyzer, new()
            where TCodeFix : CodeFixProvider, new()
        {
            var analyzer = new TAnalyzer();
            FixAllAsync(
                    analyzer,
                    new TCodeFix(),
                    DiagnosticsAndSources.CreateFromCodeWithErrorsIndicated(analyzer, new[] { codeWithErrorsIndicated }),
                    new[] { fixedCode },
                    CodeFactory.DefaultCompilationOptions(analyzer, SuppressedDiagnostics),
                    MetadataReferences,
                    fixTitle,
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
        public static void FixAll<TAnalyzer, TCodeFix>(IReadOnlyList<string> codeWithErrorsIndicated, string fixedCode, string fixTitle = null, AllowCompilationErrors allowCompilationErrors = AllowCompilationErrors.No)
            where TAnalyzer : DiagnosticAnalyzer, new()
            where TCodeFix : CodeFixProvider, new()
        {
            var analyzer = new TAnalyzer();
            FixAllAsync(
                    analyzer,
                    new TCodeFix(),
                    DiagnosticsAndSources.CreateFromCodeWithErrorsIndicated(analyzer, codeWithErrorsIndicated),
                    MergeFixedCode(codeWithErrorsIndicated, fixedCode),
                    CodeFactory.DefaultCompilationOptions(analyzer, SuppressedDiagnostics),
                    MetadataReferences,
                    fixTitle,
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
        public static void FixAll<TAnalyzer, TCodeFix>(IReadOnlyList<string> codeWithErrorsIndicated, IReadOnlyList<string> fixedCode, string fixTitle = null, AllowCompilationErrors allowCompilationErrors = AllowCompilationErrors.No)
            where TAnalyzer : DiagnosticAnalyzer, new()
            where TCodeFix : CodeFixProvider, new()
        {
            var analyzer = new TAnalyzer();
            FixAllAsync(
                    analyzer,
                    new TCodeFix(),
                    DiagnosticsAndSources.CreateFromCodeWithErrorsIndicated(analyzer, codeWithErrorsIndicated),
                    fixedCode,
                    CodeFactory.DefaultCompilationOptions(analyzer, SuppressedDiagnostics),
                    MetadataReferences,
                    fixTitle,
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
        public static void FixAll<TCodeFix>(string id, string codeWithErrorsIndicated, string fixedCode, string fixTitle = null, AllowCompilationErrors allowCompilationErrors = AllowCompilationErrors.No)
            where TCodeFix : CodeFixProvider, new()
        {
            var analyzer = new PlaceholderAnalyzer(id);
            FixAllAsync(
                    analyzer,
                    new TCodeFix(),
                    DiagnosticsAndSources.CreateFromCodeWithErrorsIndicated(analyzer, new[] { codeWithErrorsIndicated }),
                    new[] { fixedCode },
                    CodeFactory.DefaultCompilationOptions(analyzer, SuppressedDiagnostics),
                    MetadataReferences,
                    fixTitle,
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
        public static void FixAll<TCodeFix>(string id, IReadOnlyList<string> codeWithErrorsIndicated, string fixedCode, string fixTitle = null, AllowCompilationErrors allowCompilationErrors = AllowCompilationErrors.No)
            where TCodeFix : CodeFixProvider, new()
        {
            var analyzer = new PlaceholderAnalyzer(id);
            FixAllAsync(
                    analyzer,
                    new TCodeFix(),
                    DiagnosticsAndSources.CreateFromCodeWithErrorsIndicated(analyzer, codeWithErrorsIndicated),
                    MergeFixedCode(codeWithErrorsIndicated, fixedCode),
                    CodeFactory.DefaultCompilationOptions(analyzer, SuppressedDiagnostics),
                    MetadataReferences,
                    fixTitle,
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
        public static void FixAll<TCodeFix>(string id, IReadOnlyList<string> codeWithErrorsIndicated, IReadOnlyList<string> fixedCode, string fixTitle = null, AllowCompilationErrors allowCompilationErrors = AllowCompilationErrors.No)
            where TCodeFix : CodeFixProvider, new()
        {
            var analyzer = new PlaceholderAnalyzer(id);
            FixAllAsync(
                    analyzer,
                    new TCodeFix(),
                    DiagnosticsAndSources.CreateFromCodeWithErrorsIndicated(analyzer, codeWithErrorsIndicated),
                    fixedCode,
                    CodeFactory.DefaultCompilationOptions(analyzer, SuppressedDiagnostics),
                    MetadataReferences,
                    fixTitle,
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
        /// <param name="metadataReference">The meta data references to use when compiling the code.</param>
        /// <param name="fixTitle">The title of the fix to apply if more than one.</param>
        /// <param name="allowCompilationErrors">If compilation errors are accepted in the fixed code.</param>
        public static void FixAll(DiagnosticAnalyzer analyzer, CodeFixProvider codeFix, IReadOnlyList<string> codeWithErrorsIndicated, IReadOnlyList<string> fixedCode, IReadOnlyList<MetadataReference> metadataReference, string fixTitle = null, AllowCompilationErrors allowCompilationErrors = AllowCompilationErrors.No)
        {
            FixAllAsync(
                    analyzer,
                    codeFix,
                    DiagnosticsAndSources.CreateFromCodeWithErrorsIndicated(analyzer, codeWithErrorsIndicated),
                    fixedCode,
                    CodeFactory.DefaultCompilationOptions(analyzer, SuppressedDiagnostics),
                    metadataReference,
                    fixTitle,
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
        /// <param name="metadataReference">The meta data metadataReference to add to the compilation.</param>
        /// <param name="fixTitle">The title of the fix to apply if more than one.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        /// <param name="allowCompilationErrors">If compilation errors are accepted in the fixed code.</param>
        public static Task FixAllAsync(DiagnosticAnalyzer analyzer, CodeFixProvider codeFix, IReadOnlyList<string> codeWithErrorsIndicated, IReadOnlyList<string> fixedCode, IReadOnlyList<MetadataReference> metadataReference, string fixTitle, AllowCompilationErrors allowCompilationErrors)
        {
            return FixAllAsync(
                analyzer,
                codeFix,
                DiagnosticsAndSources.CreateFromCodeWithErrorsIndicated(analyzer, codeWithErrorsIndicated),
                fixedCode,
                CodeFactory.DefaultCompilationOptions(analyzer, SuppressedDiagnostics),
                metadataReference,
                fixTitle,
                allowCompilationErrors);
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
        /// <param name="compilationOptions">The <see cref="CSharpCompilationOptions"/> to use.</param>
        /// <param name="metadataReference">The meta data metadataReference to add to the compilation.</param>
        /// <param name="fixTitle">The title of the fix to apply if more than one.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        /// <param name="allowCompilationErrors">If compilation errors are accepted in the fixed code.</param>
        public static async Task FixAllAsync(DiagnosticAnalyzer analyzer, CodeFixProvider codeFix, IReadOnlyList<string> codeWithErrorsIndicated, IReadOnlyList<string> fixedCode, CSharpCompilationOptions compilationOptions, IReadOnlyList<MetadataReference> metadataReference, string fixTitle, AllowCompilationErrors allowCompilationErrors)
        {
            var data = await CreateDiagnosticsMetadataAsync(
                analyzer,
                codeFix,
               DiagnosticsAndSources.CreateFromCodeWithErrorsIndicated(analyzer, codeWithErrorsIndicated),
                compilationOptions,
                metadataReference);
            await FixAllOneByOneAsync(analyzer, codeFix, fixedCode, fixTitle, allowCompilationErrors, data).ConfigureAwait(false);

            var fixAllProvider = codeFix.GetFixAllProvider();
            if (fixAllProvider != null)
            {
                foreach (var scope in fixAllProvider.GetSupportedFixAllScopes())
                {
                    await FixAllByScopeAsync(analyzer, codeFix, fixedCode, fixTitle, allowCompilationErrors, data, scope);
                }
            }
        }

        /// <summary>
        /// Verifies that
        /// 1. <paramref name="diagnosticsAndSources"/> produces the expected diagnostics
        /// 2. The code fix fixes the code.
        /// </summary>
        /// <param name="analyzer">The analyzer to run on the code..</param>
        /// <param name="codeFix">The code fix to apply.</param>
        /// <param name="diagnosticsAndSources">The code and expected diagnostics</param>
        /// <param name="fixedCode">The expected code produced by the code fix.</param>
        /// <param name="compilationOptions">The <see cref="CSharpCompilationOptions"/> to use.</param>
        /// <param name="metadataReference">The meta data metadataReference to add to the compilation.</param>
        /// <param name="fixTitle">The title of the fix to apply if more than one.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        /// <param name="allowCompilationErrors">If compilation errors are accepted in the fixed code.</param>
        public static async Task FixAllAsync(DiagnosticAnalyzer analyzer, CodeFixProvider codeFix, DiagnosticsAndSources diagnosticsAndSources, IReadOnlyList<string> fixedCode, CSharpCompilationOptions compilationOptions, IReadOnlyList<MetadataReference> metadataReference, string fixTitle, AllowCompilationErrors allowCompilationErrors)
        {
            var data = await CreateDiagnosticsMetadataAsync(
                analyzer,
                codeFix,
                diagnosticsAndSources,
                compilationOptions,
                metadataReference);
            await FixAllOneByOneAsync(analyzer, codeFix, fixedCode, fixTitle, allowCompilationErrors, data).ConfigureAwait(false);

            var fixAllProvider = codeFix.GetFixAllProvider();
            if (fixAllProvider != null)
            {
                foreach (var scope in fixAllProvider.GetSupportedFixAllScopes())
                {
                    await FixAllByScopeAsync(analyzer, codeFix, fixedCode, fixTitle, allowCompilationErrors, data, scope);
                }
            }
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
        public static void FixAllInDocument<TAnalyzer, TCodeFix>(string codeWithErrorsIndicated, string fixedCode, string fixTitle = null, AllowCompilationErrors allowCompilationErrors = AllowCompilationErrors.No)
            where TAnalyzer : DiagnosticAnalyzer, new()
            where TCodeFix : CodeFixProvider, new()
        {
            var analyzer = new TAnalyzer();
            FixAllByScopeAsync(
                    analyzer,
                    new TCodeFix(),
                    new[] { codeWithErrorsIndicated },
                    new[] { fixedCode },
                    MetadataReferences,
                    fixTitle,
                    allowCompilationErrors,
                    FixAllScope.Document)
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
        public static void FixAllInDocument<TCodeFix>(string id, string codeWithErrorsIndicated, string fixedCode, string fixTitle = null, AllowCompilationErrors allowCompilationErrors = AllowCompilationErrors.No)
            where TCodeFix : CodeFixProvider, new()
        {
            var analyzer = new PlaceholderAnalyzer(id);
            FixAllByScopeAsync(
                    analyzer,
                    new TCodeFix(),
                    new[] { codeWithErrorsIndicated },
                    new[] { fixedCode },
                    MetadataReferences,
                    fixTitle,
                    allowCompilationErrors,
                    FixAllScope.Document)
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
        public static void FixAllOneByOne<TAnalyzer, TCodeFix>(string codeWithErrorsIndicated, string fixedCode, string fixTitle = null, AllowCompilationErrors allowCompilationErrors = AllowCompilationErrors.No)
            where TAnalyzer : DiagnosticAnalyzer, new()
            where TCodeFix : CodeFixProvider, new()
        {
            var analyzer = new TAnalyzer();
            FixAllOneByOneAsync(
                    analyzer,
                    new TCodeFix(),
                    new[] { codeWithErrorsIndicated },
                    new[] { fixedCode },
                    MetadataReferences,
                    fixTitle,
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
        public static void FixAllOneByOne<TCodeFix>(string id, string codeWithErrorsIndicated, string fixedCode, string fixTitle = null, AllowCompilationErrors allowCompilationErrors = AllowCompilationErrors.No)
            where TCodeFix : CodeFixProvider, new()
        {
            var analyzer = new PlaceholderAnalyzer(id);
            FixAllOneByOneAsync(
                    analyzer,
                    new TCodeFix(),
                    new[] { codeWithErrorsIndicated },
                    new[] { fixedCode },
                    MetadataReferences,
                    fixTitle,
                    allowCompilationErrors)
                .GetAwaiter()
                .GetResult();
        }

        /// <summary>
        /// Verifies that
        /// 1. <paramref name="codeWithErrorsIndicated"/> produces the expected diagnostics
        /// 2. The code fix fixes the code by applying it as long as there are fixable errors.
        /// </summary>
        /// <param name="analyzer">The analyzer to run on the code..</param>
        /// <param name="codeFix">The code fix to apply.</param>
        /// <param name="codeWithErrorsIndicated">The code with error positions indicated.</param>
        /// <param name="fixedCode">The expected code produced by the code fix.</param>
        /// <param name="metadataReference">The meta data metadataReference to add to the compilation.</param>
        /// <param name="fixTitle">The title of the fix to apply if more than one.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        /// <param name="allowCompilationErrors">If compilation errors are accepted in the fixed code.</param>
        public static async Task FixAllOneByOneAsync(DiagnosticAnalyzer analyzer, CodeFixProvider codeFix, IReadOnlyList<string> codeWithErrorsIndicated, IReadOnlyList<string> fixedCode, IReadOnlyList<MetadataReference> metadataReference, string fixTitle, AllowCompilationErrors allowCompilationErrors)
        {
            var data = await CreateDiagnosticsMetadataAsync(
                analyzer,
                codeFix,
                DiagnosticsAndSources.CreateFromCodeWithErrorsIndicated(analyzer, codeWithErrorsIndicated), 
                CodeFactory.DefaultCompilationOptions(analyzer, SuppressedDiagnostics),
                metadataReference);
            await FixAllOneByOneAsync(analyzer, codeFix, fixedCode, fixTitle, allowCompilationErrors, data);
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
        /// <param name="metadataReference">The meta data metadataReference to add to the compilation.</param>
        /// <param name="fixTitle">The title of the fix to apply if more than one.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        /// <param name="allowCompilationErrors">If compilation errors are accepted in the fixed code.</param>
        /// <param name="scope">The scope to apply fixes for.</param>
        public static async Task FixAllByScopeAsync(DiagnosticAnalyzer analyzer, CodeFixProvider codeFix, IReadOnlyList<string> codeWithErrorsIndicated, IReadOnlyList<string> fixedCode, IReadOnlyList<MetadataReference> metadataReference, string fixTitle, AllowCompilationErrors allowCompilationErrors, FixAllScope scope)
        {
            var data = await CreateDiagnosticsMetadataAsync(
                analyzer,
                codeFix,
                DiagnosticsAndSources.CreateFromCodeWithErrorsIndicated(analyzer, codeWithErrorsIndicated),
                CodeFactory.DefaultCompilationOptions(analyzer, SuppressedDiagnostics),
                metadataReference);
            await FixAllByScopeAsync(analyzer, codeFix, fixedCode, fixTitle, allowCompilationErrors, data, scope);
        }

        private static async Task FixAllOneByOneAsync(DiagnosticAnalyzer analyzer, CodeFixProvider codeFix, IReadOnlyList<string> fixedCode, string fixTitle, AllowCompilationErrors allowCompilationErrors, DiagnosticsMetadata data)
        {
            var fixedSolution = await Fix.ApplyAllFixableOneByOneAsync(data.Solution, analyzer, codeFix, fixTitle, CancellationToken.None).ConfigureAwait(false);
            await AreEqualAsync(fixedCode, fixedSolution, "Applying fixes one by one failed.").ConfigureAwait(false);
            if (allowCompilationErrors == AllowCompilationErrors.No)
            {
                await AssertNoCompilerErrorsAsync(codeFix, fixedSolution).ConfigureAwait(false);
            }
        }

        private static async Task FixAllByScopeAsync(DiagnosticAnalyzer analyzer, CodeFixProvider codeFix, IReadOnlyList<string> fixedCode, string fixTitle, AllowCompilationErrors allowCompilationErrors, DiagnosticsMetadata data, FixAllScope scope)
        {
            var fixedSolution = await Fix.ApplyAllFixableScopeByScopeAsync(data.Solution, analyzer, codeFix, fixTitle, scope, CancellationToken.None).ConfigureAwait(false);
            await AreEqualAsync(fixedCode, fixedSolution, $"Applying fixes for {scope} failed.").ConfigureAwait(false);
            if (allowCompilationErrors == AllowCompilationErrors.No)
            {
                await AssertNoCompilerErrorsAsync(codeFix, fixedSolution).ConfigureAwait(false);
            }
        }

        private static async Task<DiagnosticsMetadata> CreateDiagnosticsMetadataAsync(DiagnosticAnalyzer analyzer, CodeFixProvider codeFix, DiagnosticsAndSources diagnosticsAndSources, CSharpCompilationOptions compilationOptions, IReadOnlyList<MetadataReference> metadataReference)
        {
            if (analyzer.SupportedDiagnostics.Length != 1)
            {
                var message =
                    $"The analyzer supports multiple diagnostics {{{string.Join(", ", analyzer.SupportedDiagnostics.Select(d => d.Id))}}}.{Environment.NewLine}" +
                    $"This method can only be used with analyzers that have exactly one SupportedDiagnostic";
                throw AssertException.Create(message);
            }

            AssertCodeFixCanFixDiagnosticsFromAnalyzer(analyzer, codeFix);
            var data = await DiagnosticsWithMetadataAsync(analyzer, diagnosticsAndSources, compilationOptions, metadataReference)
                .ConfigureAwait(false);

            var fixableDiagnostics = data.ActualDiagnostics.SelectMany(x => x)
                                         .Where(x => codeFix.FixableDiagnosticIds.Contains(x.Id))
                                         .ToArray();
            if (fixableDiagnostics.Length == 0)
            {
                var message =
                    $"Code analyzed with {analyzer} did not generate any diagnostics fixable by {codeFix}.{Environment.NewLine}" +
                    $"The analyzed code contained the following diagnostics: {{{string.Join(", ", data.ExpectedDiagnostics.Select(d => d.Id))}}}{Environment.NewLine}" +
                    $"The code fix supports the following diagnostics: {{{string.Join(", ", codeFix.FixableDiagnosticIds)}}}";
                throw AssertException.Create(message);
            }

            return data;
        }

        private static List<string> MergeFixedCode(IReadOnlyList<string> codeWithErrorsIndicated, string fixedCode)
        {
            var merged = new List<string>(codeWithErrorsIndicated.Count);
            var found = false;
            foreach (var code in codeWithErrorsIndicated)
            {
                if (code.IndexOf('↓') >= 0)
                {
                    if (found)
                    {
                        throw AssertException.Create("Expected only one with errors indicated.");
                    }

                    merged.Add(fixedCode);
                    found = true;
                }
                else
                {
                    merged.Add(code);
                }
            }

            if (!found)
            {
                throw AssertException.Create("Expected one with errors indicated.");
            }

            return merged;
        }
    }
}