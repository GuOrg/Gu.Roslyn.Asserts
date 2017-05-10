namespace Gu.Roslyn.Asserts
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Gu.Roslyn.Asserts.Internals;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CodeActions;
    using Microsoft.CodeAnalysis.CodeFixes;
    using Microsoft.CodeAnalysis.Diagnostics;
    using Microsoft.CodeAnalysis.Formatting;
    using Microsoft.CodeAnalysis.Simplification;

    /// <summary>
    /// The AnalyzerAssert class contains a collection of static methods used for assertions on the behavior of analyzers and code fixes.
    /// </summary>
    public static partial class AnalyzerAssert
    {
        /// <summary>
        /// The metadata references used when creating the projects created in the tests.
        /// </summary>
        public static readonly List<MetadataReference> MetadataReference = new List<MetadataReference>();

        private static void AssertCodeFixCanFixDiagnosticsFromAnalyzer(DiagnosticAnalyzer analyzer, CodeFixProvider codeFix)
        {
            if (!analyzer.SupportedDiagnostics.Select(d => d.Id).Intersect(codeFix.FixableDiagnosticIds).Any())
            {
                var message = $"Analyzer {analyzer} does not produce diagnostics fixable by {codeFix}.{Environment.NewLine}" +
                              $"The analyzer produces the following diagnostics: {{{string.Join(", ", analyzer.SupportedDiagnostics.Select(d => d.Id))}}}{Environment.NewLine}" +
                              $"The code fix supports the following diagnostics: {{{string.Join(", ", codeFix.FixableDiagnosticIds)}}}";
                throw Fail.CreateException(message);
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

        private static async Task<Solution> ApplyFixAsync(Solution solution, CodeFixProvider codeFix, Diagnostic diagnostic, CancellationToken cancellationToken)
        {
            var actions = new List<CodeAction>();
            var document = solution.GetDocument(diagnostic.Location.SourceTree);
            actions.Clear();
            var context = new CodeFixContext(
                document,
                diagnostic,
                (a, d) => actions.Add(a),
                CancellationToken.None);
            await codeFix.RegisterCodeFixesAsync(context).ConfigureAwait(false);
            if (actions.Count == 0)
            {
                return solution;
            }

            if (actions.Count > 1)
            {
                throw Fail.CreateException("Expected only one action");
            }

            var operations = await actions[0].GetOperationsAsync(cancellationToken)
                                             .ConfigureAwait(false);
            return operations.OfType<ApplyChangesOperation>()
                             .Single()
                             .ChangedSolution;
        }
    }
}