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
        public static void FixAll<TAnalyzer, TCodeFix>(string codeWithErrorsIndicated, string fixedCode)
            where TAnalyzer : DiagnosticAnalyzer, new()
            where TCodeFix : CodeFixProvider, new()
        {
            FixAll(new TAnalyzer(), new TCodeFix(), new[] { codeWithErrorsIndicated }, new[] { fixedCode }, MetadataReference);
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
        public static void FixAll<TCodeFix>(string id, string codeWithErrorsIndicated, string fixedCode)
            where TCodeFix : CodeFixProvider, new()
        {
            FixAll(new PlaceholderAnalyzer(id), new TCodeFix(), new[] { codeWithErrorsIndicated }, new[] { fixedCode }, MetadataReference);
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
        public static void FixAll<TAnalyzer, TCodeFix>(IEnumerable<string> codeWithErrorsIndicated, IEnumerable<string> fixedCode)
            where TAnalyzer : DiagnosticAnalyzer, new()
            where TCodeFix : CodeFixProvider, new()
        {
            FixAll(new TAnalyzer(), new TCodeFix(), codeWithErrorsIndicated, fixedCode, MetadataReference);
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
        public static void FixAll<TCodeFix>(string id, IEnumerable<string> codeWithErrorsIndicated, IEnumerable<string> fixedCode)
            where TCodeFix : CodeFixProvider, new()
        {
            FixAll(new PlaceholderAnalyzer(id), new TCodeFix(), codeWithErrorsIndicated, fixedCode, MetadataReference);
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
        public static void FixAll(DiagnosticAnalyzer analyzer, CodeFixProvider codeFix, IEnumerable<string> codeWithErrorsIndicated, IEnumerable<string> fixedCode, IEnumerable<MetadataReference> metadataReference)
        {
            try
            {
                FixAllAsync(analyzer, codeFix, codeWithErrorsIndicated, fixedCode, metadataReference).Wait();
            }
            catch (AggregateException e)
            {
                throw Fail.CreateException(e.InnerExceptions[0].Message);
            }
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
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        public static async Task FixAllAsync(DiagnosticAnalyzer analyzer, CodeFixProvider codeFix, IEnumerable<string> codeWithErrorsIndicated, IEnumerable<string> fixedCode, IEnumerable<MetadataReference> metadataReference)
        {
            if (analyzer.SupportedDiagnostics.Length != 1)
            {
                var message = $"The analyzer supports multiple diagnostics {{{string.Join(", ", analyzer.SupportedDiagnostics.Select(d => d.Id))}}}.{Environment.NewLine}" +
                              $"This method can only be used with analyzers that have exactly one SupportedDiagnostic";
                throw Fail.CreateException(message);
            }

            AssertCodeFixCanFixDiagnosticsFromAnalyzer(analyzer, codeFix);
            var data = await DiagnosticsWithMetaDataAsync(analyzer, codeWithErrorsIndicated, metadataReference).ConfigureAwait(false);

            var fixableDiagnostics = data.ActualDiagnostics.SelectMany(x => x)
                                         .Where(x => codeFix.FixableDiagnosticIds.Contains(x.Id))
                                         .ToArray();
            if (fixableDiagnostics.Length == 0)
            {
                var message = $"Code analyzed with {analyzer} did not generate any diagnostics fixable by {codeFix}.{Environment.NewLine}" +
                              $"The analyzed code contained the following diagnostics: {{{string.Join(", ", data.ExpectedDiagnostics.Select(d => d.Analyzer.SupportedDiagnostics[0].Id))}}}{Environment.NewLine}" +
                              $"The code fix supports the following diagnostics: {{{string.Join(", ", codeFix.FixableDiagnosticIds)}}}";
                throw Fail.CreateException(message);
            }

            var solution = await Fix.ApplyAllFixableOneByOneAsync(data.Solution, analyzer, codeFix, CancellationToken.None).ConfigureAwait(false);
            await AreEqualAsync(fixedCode, solution).ConfigureAwait(false);

            //throw new NotImplementedException("Test fixing all supported scopes if FixAll != null");
        }
    }
}