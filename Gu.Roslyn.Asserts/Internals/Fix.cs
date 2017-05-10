namespace Gu.Roslyn.Asserts.Internals
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CodeActions;
    using Microsoft.CodeAnalysis.CodeFixes;
    using Microsoft.CodeAnalysis.Diagnostics;

    internal static class Fix
    {
        internal static async Task<Solution> ApplyAsync(Solution solution, CodeFixProvider codeFix, Diagnostic diagnostic, CancellationToken cancellationToken)
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

        internal static async Task<Solution> ApplyAllFixableOneByOneAsync(Solution solution, DiagnosticAnalyzer analyzer, CodeFixProvider codeFix, CancellationToken cancellationToken)
        {
            var fixable = await Analyze.GetFixableDiagnosticsAsync(solution, analyzer, codeFix).ConfigureAwait(false);
            var fixedSolution = solution;
            int count;
            do
            {
                count = fixable.Count;
                if (count == 0)
                {
                    return fixedSolution;
                }

                fixedSolution = await ApplyAsync(fixedSolution, codeFix, fixable[0], cancellationToken).ConfigureAwait(false);
                fixable = await Analyze.GetFixableDiagnosticsAsync(fixedSolution, analyzer, codeFix).ConfigureAwait(false);
            }
            while (fixable.Count < count);

            return fixedSolution;
        }
    }
}