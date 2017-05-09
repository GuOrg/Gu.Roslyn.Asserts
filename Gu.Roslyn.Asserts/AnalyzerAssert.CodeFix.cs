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
        public static void CodeFix<TAnalyzer, TCodeFix>(string code, string fixedCode)
            where TAnalyzer : DiagnosticAnalyzer, new()
            where TCodeFix : CodeFixProvider, new()
        {
            CodeFix(new TAnalyzer(), new TCodeFix(), new[] { code }, new[] { fixedCode });
        }

        public static void CodeFix<TCodeFix>(string id, string code, string fixedCode)
            where TCodeFix : CodeFixProvider, new()
        {
            CodeFix(new PlaceholderAnalyzer(id), new TCodeFix(), new[] { code }, new[] { fixedCode });
        }

        public static void CodeFix<TAnalyzer, TCodeFix>(IEnumerable<string> code, IEnumerable<string> fixedCode)
            where TAnalyzer : DiagnosticAnalyzer, new()
            where TCodeFix : CodeFixProvider, new()
        {
            CodeFix(new TAnalyzer(), new TCodeFix(), code, fixedCode);
        }

        public static void CodeFix<TCodeFix>(string id,IEnumerable<string> code, IEnumerable<string> fixedCode)
            where TCodeFix : CodeFixProvider, new()
        {
            CodeFix(new PlaceholderAnalyzer(id), new TCodeFix(), code, fixedCode);
        }

        public static void CodeFix(DiagnosticAnalyzer analyzer, CodeFixProvider codeFix, IEnumerable<string> code, IEnumerable<string> fixedCode)
        {
            try
            {
                CodeFixAsync(analyzer, codeFix, code, fixedCode, References).Wait();
            }
            catch (AggregateException e)
            {
                Fail.WithMessage(e.InnerExceptions[0].Message);
            }
        }

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

                var fixedProject = await ApplyFixAsync(project, actions[0], CancellationToken.None)
                    .ConfigureAwait(false);
                if (ReferenceEquals(fixedProject, project))
                {
                    Fail.WithMessage("Fix did nothing.");
                }

                for (var i = 0; i < fixedProject.DocumentIds.Count; i++)
                {
                    var fixedSource = await GetStringFromDocumentAsync(fixedProject.GetDocument(project.DocumentIds[i]), CancellationToken.None);
                    CodeAssert.AreEqual(fixedCode.ElementAt(i), fixedSource);
                }
            }
        }
    }
}