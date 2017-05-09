namespace Gu.Roslyn.Asserts
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CodeActions;
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
        public static void CodeFix<TAnalyzer, TCodeFix>(string codeWithErrorsIndicated, string fixedCode)
            where TAnalyzer : DiagnosticAnalyzer, new()
            where TCodeFix : CodeFixProvider, new()
        {
            CodeFix(new TAnalyzer(), new TCodeFix(), new[] { codeWithErrorsIndicated }, new[] { fixedCode });
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
        public static void CodeFix<TCodeFix>(string id, string codeWithErrorsIndicated, string fixedCode)
            where TCodeFix : CodeFixProvider, new()
        {
            CodeFix(new PlaceholderAnalyzer(id), new TCodeFix(), new[] { codeWithErrorsIndicated }, new[] { fixedCode });
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
        public static void CodeFix<TAnalyzer, TCodeFix>(IEnumerable<string> codeWithErrorsIndicated, IEnumerable<string> fixedCode)
            where TAnalyzer : DiagnosticAnalyzer, new()
            where TCodeFix : CodeFixProvider, new()
        {
            CodeFix(new TAnalyzer(), new TCodeFix(), codeWithErrorsIndicated, fixedCode);
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
        public static void CodeFix<TCodeFix>(string id, IEnumerable<string> codeWithErrorsIndicated, IEnumerable<string> fixedCode)
            where TCodeFix : CodeFixProvider, new()
        {
            CodeFix(new PlaceholderAnalyzer(id), new TCodeFix(), codeWithErrorsIndicated, fixedCode);
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
        public static void CodeFix(DiagnosticAnalyzer analyzer, CodeFixProvider codeFix, IEnumerable<string> codeWithErrorsIndicated, IEnumerable<string> fixedCode)
        {
            try
            {
                CodeFixAsync(analyzer, codeFix, codeWithErrorsIndicated, fixedCode, References).Wait();
            }
            catch (AggregateException e)
            {
                Fail.WithMessage(e.InnerExceptions[0].Message);
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
        /// <param name="references">The meta data references to add to the compilation.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        public static async Task CodeFixAsync(DiagnosticAnalyzer analyzer, CodeFixProvider codeFix, IEnumerable<string> codeWithErrorsIndicated, IEnumerable<string> fixedCode, IEnumerable<MetadataReference> references)
        {
            var data = await DiagnosticsWithMetaDataAsync(analyzer, codeWithErrorsIndicated, references).ConfigureAwait(false);
            var fixableDiagnostics = data.ActualDiagnostics.SelectMany(x => x)
                                         .Where(x => codeFix.FixableDiagnosticIds.Contains(x.Id))
                                         .ToArray();
            if (fixableDiagnostics.Length != 1)
            {
                Fail.WithMessage("Expected code to have exactly one fixable diagnostic.");
            }

            var diagnostic = fixableDiagnostics.Single();
            foreach (var project in data.Solution.Projects)
            {
                var document = project.GetDocument(diagnostic.Location.SourceTree);
                if (document == null)
                {
                    continue;
                }

                var actions = new List<CodeAction>();
                var context = new CodeFixContext(
                    document,
                    diagnostic,
                    (a, d) => actions.Add(a),
                    CancellationToken.None);
                await codeFix.RegisterCodeFixesAsync(context).ConfigureAwait(false);
                if (actions.Count == 0)
                {
                    continue;
                }

                if (actions.Count > 1)
                {
                    Fail.WithMessage("Expected only one action");
                }

                var fixedProject = await ApplyFixAsync(project, actions[0], CancellationToken.None).ConfigureAwait(false);
                if (ReferenceEquals(fixedProject, project))
                {
                    Fail.WithMessage("Fix did nothing.");
                }

                for (var i = 0; i < fixedProject.DocumentIds.Count; i++)
                {
                    var fixedSource = await GetStringFromDocumentAsync(fixedProject.GetDocument(project.DocumentIds[i]), CancellationToken.None).ConfigureAwait(false);
                    //// ReSharper disable once PossibleMultipleEnumeration
                    CodeAssert.AreEqual(fixedCode.ElementAt(i), fixedSource);
                }
            }
        }
    }
}