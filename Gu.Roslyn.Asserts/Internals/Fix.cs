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

    /// <summary>
    /// Helper class for applying code fixes
    /// </summary>
    internal static class Fix
    {
        /// <summary>
        /// Fix the solution by applying the code fix.
        /// </summary>
        /// <param name="solution">The solution with the diagnostic.</param>
        /// <param name="codeFix">The code fix.</param>
        /// <param name="diagnostic">The diagnostic.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>The fixed solution or the same instance if no fix.</returns>
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
                throw AssertException.Create("Expected only one action");
            }

            var operations = await actions[0].GetOperationsAsync(cancellationToken)
                                             .ConfigureAwait(false);
            return operations.OfType<ApplyChangesOperation>()
                             .Single()
                             .ChangedSolution;
        }

        /// <summary>
        /// Fix the solution by applying the code fix.
        /// </summary>
        /// <returns>The fixed solution or the same instance if no fix.</returns>
        internal static async Task<Solution> ApplyAsync(CodeFixProvider codeFix, FixAllScope scope, TestDiagnosticProvider diagnosticProvider, CancellationToken cancellationToken)
        {
            var context = new FixAllContext(
                diagnosticProvider.Document,
                codeFix,
                scope,
                diagnosticProvider.EquivalenceKey,
                codeFix.FixableDiagnosticIds,
                diagnosticProvider,
                cancellationToken);
            var action = await codeFix.GetFixAllProvider().GetFixAsync(context).ConfigureAwait(false);

            var operations = await action.GetOperationsAsync(cancellationToken)
                                             .ConfigureAwait(false);
            return operations.OfType<ApplyChangesOperation>()
                             .Single()
                             .ChangedSolution;
        }

        /// <summary>
        /// Fix the solution by applying the code fix one fix at the time until it stops fixing the code.
        /// </summary>
        /// <returns>The fixed solution or the same instance if no fix.</returns>
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

        /// <summary>
        /// Fix the solution by applying the code fix one fix at the time until it stops fixing the code.
        /// </summary>
        /// <returns>The fixed solution or the same instance if no fix.</returns>
        internal static async Task<Solution> ApplyAllFixableScopeByScopeAsync(Solution solution, DiagnosticAnalyzer analyzer, CodeFixProvider codeFix, FixAllScope scope, CancellationToken cancellationToken)
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

                var diagnosticProvider = await TestDiagnosticProvider.CreateAsync(fixedSolution, codeFix, fixable).ConfigureAwait(false);
                fixedSolution = await ApplyAsync(codeFix, scope, diagnosticProvider, cancellationToken).ConfigureAwait(false);
                fixable = await Analyze.GetFixableDiagnosticsAsync(fixedSolution, analyzer, codeFix).ConfigureAwait(false);
            }
            while (fixable.Count < count);
            return fixedSolution;
        }

        /// <inheritdoc />
        internal sealed class TestDiagnosticProvider : FixAllContext.DiagnosticProvider
        {
            private readonly IReadOnlyList<Diagnostic> diagnostics;

            private TestDiagnosticProvider(IReadOnlyList<Diagnostic> diagnostics, Document document, string equivalenceKey)
            {
                this.diagnostics = diagnostics;
                this.Document = document;
                this.EquivalenceKey = equivalenceKey;
            }

            /// <summary>
            /// Gets the document from the first diagnostic.
            /// </summary>
            public Document Document { get; }

            /// <summary>
            /// Gets the equivalence key for the first registered action.
            /// </summary>
            public string EquivalenceKey { get; }

            /// <inheritdoc />
            public override Task<IEnumerable<Diagnostic>> GetAllDiagnosticsAsync(Project project, CancellationToken cancellationToken)
            {
                return Task.FromResult((IEnumerable<Diagnostic>)this.diagnostics);
            }

            /// <inheritdoc />
            public override Task<IEnumerable<Diagnostic>> GetDocumentDiagnosticsAsync(Document document, CancellationToken cancellationToken)
            {
                return Task.FromResult(this.diagnostics.Where(i => i.Location.GetLineSpan().Path == document.Name));
            }

            /// <inheritdoc />
            public override Task<IEnumerable<Diagnostic>> GetProjectDiagnosticsAsync(Project project, CancellationToken cancellationToken)
            {
                return Task.FromResult(this.diagnostics.Where(i => !i.Location.IsInSource));
            }

            /// <summary>
            /// Create an instance of <see cref="TestDiagnosticProvider"/>
            /// </summary>
            /// <returns>The <see cref="TestDiagnosticProvider"/></returns>
            internal static async Task<TestDiagnosticProvider> CreateAsync(Solution solution, CodeFixProvider codeFix, IReadOnlyList<Diagnostic> diagnostics)
            {
                var actions = new List<CodeAction>();
                var diagnostic = diagnostics.First();
                var context = new CodeFixContext(solution.GetDocument(diagnostic.Location.SourceTree), diagnostic, (a, d) => actions.Add(a), CancellationToken.None);
                await codeFix.RegisterCodeFixesAsync(context).ConfigureAwait(false);
                return new TestDiagnosticProvider(diagnostics, solution.GetDocument(diagnostics.First().Location.SourceTree), actions.First().EquivalenceKey);
            }
        }
    }
}