namespace Gu.Roslyn.Asserts
{
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
        /// 2. The code fix does not change the code.
        /// </summary>
        /// <typeparam name="TAnalyzer">The type of the analyzer.</typeparam>
        /// <typeparam name="TCodeFix">The type of the code fix.</typeparam>
        /// <param name="codeWithErrorsIndicated">The code with error positions indicated.</param>
        public static void NoFix<TAnalyzer, TCodeFix>(params string[] codeWithErrorsIndicated)
            where TAnalyzer : DiagnosticAnalyzer, new()
            where TCodeFix : CodeFixProvider, new()
        {
            NoFix(new TAnalyzer(), new TCodeFix(), codeWithErrorsIndicated);
        }

        /// <summary>
        /// Verifies that
        /// 1. <paramref name="codeWithErrorsIndicated"/> produces the expected diagnostics
        /// 2. The code fix does not change the code.
        /// </summary>
        /// <typeparam name="TAnalyzer">The type of the analyzer.</typeparam>
        /// <typeparam name="TCodeFix">The type of the code fix.</typeparam>
        /// <param name="codeWithErrorsIndicated">The code with error positions indicated.</param>
        public static void NoFix<TAnalyzer, TCodeFix>(IReadOnlyList<string> codeWithErrorsIndicated)
            where TAnalyzer : DiagnosticAnalyzer, new()
            where TCodeFix : CodeFixProvider, new()
        {
            NoFix(new TAnalyzer(), new TCodeFix(), codeWithErrorsIndicated);
        }

        /// <summary>
        /// Verifies that
        /// 1. <paramref name="codeWithErrorsIndicated"/> produces the expected diagnostics
        /// 2. The code fix does not change the code.
        /// </summary>
        /// <param name="analyzer">The type of the analyzer.</param>
        /// <param name="codeFix">The type of the code fix.</param>
        /// <param name="codeWithErrorsIndicated">The code with error positions indicated.</param>
        public static void NoFix(DiagnosticAnalyzer analyzer, CodeFixProvider codeFix, IReadOnlyList<string> codeWithErrorsIndicated)
        {
            NoFixAsync(analyzer, codeFix, codeWithErrorsIndicated, MetadataReference).GetAwaiter().GetResult();
        }

        /// <summary>
        /// Verifies that
        /// 1. <paramref name="codeWithErrorsIndicated"/> produces the expected diagnostics
        /// 2. The code fix does not change the code.
        /// </summary>
        /// <param name="analyzer">The type of the analyzer.</param>
        /// <param name="codeFix">The type of the code fix.</param>
        /// <param name="codeWithErrorsIndicated">The code with error positions indicated.</param>
        /// <param name="metadataReferences">The meta data references to use when compiling.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        public static async Task NoFixAsync(DiagnosticAnalyzer analyzer, CodeFixProvider codeFix, IReadOnlyList<string> codeWithErrorsIndicated, IReadOnlyList<MetadataReference> metadataReferences)
        {
            AssertCodeFixCanFixDiagnosticsFromAnalyzer(analyzer, codeFix);
            var data = await DiagnosticsWithMetaDataAsync(analyzer, codeWithErrorsIndicated, metadataReferences).ConfigureAwait(false);
            var fixableDiagnostics = data.ActualDiagnostics.SelectMany(x => x)
                                         .Where(x => codeFix.FixableDiagnosticIds.Contains(x.Id))
                                         .ToArray();
            if (fixableDiagnostics.Length != 1)
            {
                throw AssertException.Create("Expected code to have exactly one fixable diagnostic.");
            }

            var fixedSolution = await Fix.ApplyAsync(data.Solution, codeFix, fixableDiagnostics.Single(), CancellationToken.None)
                                         .ConfigureAwait(false);
            await AreEqualAsync(data.Sources, fixedSolution, "Expected the code fix to not change any document.").ConfigureAwait(false);
        }
    }
}
