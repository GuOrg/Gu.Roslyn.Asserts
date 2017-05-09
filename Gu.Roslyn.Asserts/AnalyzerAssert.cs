using System.Linq;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.Formatting;
using Microsoft.CodeAnalysis.Simplification;

namespace Gu.Roslyn.Asserts
{
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.CodeAnalysis;

    public static partial class AnalyzerAssert
    {
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