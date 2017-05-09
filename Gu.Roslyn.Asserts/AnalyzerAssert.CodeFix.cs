using Microsoft.CodeAnalysis.CSharp;

namespace Gu.Roslyn.Asserts
{
    using System;
    using System.Collections.Generic;
    using System.Collections.Immutable;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CodeActions;
    using Microsoft.CodeAnalysis.CodeFixes;
    using Microsoft.CodeAnalysis.Diagnostics;
    using Microsoft.CodeAnalysis.Formatting;
    using Microsoft.CodeAnalysis.Simplification;

    public static partial class AnalyzerAssert
    {
        public static void CodeFix<TAnalyzer, TCodeFix>(string code, string fixedCode)
            where TAnalyzer : DiagnosticAnalyzer, new()
            where TCodeFix : CodeFixProvider, new()
        {
            CodeFix(new TAnalyzer(), new TCodeFix(), new[] { code }, new[] { fixedCode });
        }

        public static void CodeFix(DiagnosticAnalyzer analyzer, CodeFixProvider codeFix, IEnumerable<string> code, IEnumerable<string> fixedCode)
        {
            try
            {
                CodeFixAsync(analyzer, codeFix, code, fixedCode).Wait();
            }
            catch (AggregateException e)
            {
                Fail.WithMessage(e.InnerExceptions[0].Message);
            }
        }

        public static async Task CodeFixAsync(DiagnosticAnalyzer analyzer, CodeFixProvider codeFix, IEnumerable<string> codeWithErrorsIndicated, IEnumerable<string> fixedCode)
        {
            var data = await DiagnosticsWithMetaDataAsync(analyzer, codeWithErrorsIndicated).ConfigureAwait(false);
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

        private static async Task<string> GetStringFromDocumentAsync(Document document, CancellationToken cancellationToken)
        {
            var simplifiedDoc = await Simplifier.ReduceAsync(document, Simplifier.Annotation, cancellationToken: cancellationToken).ConfigureAwait(false);
            var formatted = await Formatter.FormatAsync(simplifiedDoc, Formatter.Annotation, cancellationToken: cancellationToken).ConfigureAwait(false);
            var sourceText = await formatted.GetTextAsync(cancellationToken).ConfigureAwait(false);
            return sourceText.ToString();
        }

        private static async Task<Project> ApplyFixAsync(Project project, CodeAction codeAction, CancellationToken cancellationToken)
        {
            var operations = await codeAction.GetOperationsAsync(cancellationToken)
                                             .ConfigureAwait(false);
            var solution = operations.OfType<ApplyChangesOperation>()
                                     .Single()
                                     .ChangedSolution;
            return solution.GetProject(project.Id);
        }
    }
}