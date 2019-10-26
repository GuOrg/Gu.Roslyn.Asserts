namespace Gu.Roslyn.Asserts
{
    using System;
    using System.Collections.Generic;
    using System.Collections.Immutable;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Gu.Roslyn.Asserts.Internals;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CodeActions;
    using Microsoft.CodeAnalysis.CodeFixes;
    using Microsoft.CodeAnalysis.Diagnostics;

    /// <summary>
    /// Helper class for applying code fixes.
    /// </summary>
    public static class Fix
    {
        /// <summary>
        /// Fix the solution by applying the code fix.
        /// </summary>
        /// <param name="solution">The solution with the diagnostic.</param>
        /// <param name="fix">The code fix.</param>
        /// <param name="diagnostic">The diagnostic.</param>
        /// <param name="fixTitle">The expected title of the fix. Must be provided if more than one code action is registered. If only one pass null.</param>
        /// <returns>The fixed solution or the same instance if no fix.</returns>
        public static Solution Apply(Solution solution, CodeFixProvider fix, Diagnostic diagnostic, string? fixTitle = null)
        {
            return FindSingleOperation(solution, fix, new[] { diagnostic }, fixTitle).ChangedSolution;
        }

        /// <summary>
        /// Fix the solution by applying the code fix.
        /// </summary>
        /// <param name="solution">The solution with the diagnostic.</param>
        /// <param name="fix">The code fix.</param>
        /// <param name="diagnostics">The diagnostics.</param>
        /// <param name="fixTitle">The expected title of the fix. Must be provided if more than one code action is registered. If only one pass null.</param>
        /// <returns>The fixed solution or the same instance if no fix.</returns>
        public static Solution Apply(Solution solution, CodeFixProvider fix, IReadOnlyList<ImmutableArray<Diagnostic>> diagnostics, string? fixTitle = null)
        {
            var flatDiagnostics = diagnostics.SelectMany(x => x).ToArray();
            if (flatDiagnostics.Length == 1)
            {
                return Apply(solution, fix, flatDiagnostics[0], fixTitle);
            }

            var trees = flatDiagnostics.Select(x => x.Location.SourceTree).Distinct().ToArray();
            if (trees.Length == 1)
            {
                var document = solution.Projects.SelectMany(x => x.Documents)
                                       .Single(x => x.GetSyntaxTreeAsync().GetAwaiter().GetResult() == trees[0]);
                var provider = TestDiagnosticProvider.CreateAsync(solution, fix, fixTitle, flatDiagnostics).GetAwaiter().GetResult();
                var context = new FixAllContext(document, fix, FixAllScope.Document, provider.EquivalenceKey, flatDiagnostics.Select(x => x.Id), provider, CancellationToken.None);
                var action = WellKnownFixAllProviders.BatchFixer.GetFixAsync(context).GetAwaiter().GetResult();
                var operations = action.GetOperationsAsync(CancellationToken.None).GetAwaiter().GetResult();
                if (operations.TrySingleOfType(out ApplyChangesOperation operation))
                {
                    return operation.ChangedSolution;
                }
            }

            throw new InvalidOperationException($"Failed applying fix, bug in Gu.Roslyn.Asserts");
        }

        /// <summary>
        /// Fix the solution by applying the code fix.
        /// </summary>
        /// <param name="solution">The solution with the diagnostic.</param>
        /// <param name="fix">The code fix.</param>
        /// <param name="diagnostic">The diagnostic.</param>
        /// <param name="fixTitle">The expected title of the fix. Must be provided if more than one code action is registered. If only one pass null.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>The fixed solution or the same instance if no fix.</returns>
        public static async Task<Solution> ApplyAsync(Solution solution, CodeFixProvider fix, Diagnostic diagnostic, string? fixTitle = null, CancellationToken cancellationToken = default)
        {
            var actions = await GetActionsAsync(solution, fix, diagnostic).ConfigureAwait(false);
            var action = FindAction(actions, fixTitle);
            var operations = await action.GetOperationsAsync(cancellationToken)
                                         .ConfigureAwait(false);
            if (operations.TrySingleOfType(out ApplyChangesOperation operation))
            {
                return operation.ChangedSolution;
            }

            throw new InvalidOperationException($"Expected one operation, was {string.Join(", ", operations)}");
        }

        /// <summary>
        /// Find the single <see cref="ApplyChangesOperation"/> or throw.
        /// </summary>
        /// <param name="solution">The <see cref="Solution"/>.</param>
        /// <param name="fix">The <see cref="CodeFixProvider"/>.</param>
        /// <param name="fixableDiagnostics">The <see cref="IEnumerable{Diagnostic}"/>.</param>
        /// <param name="fixTitle">The title of the fix.</param>
        /// <returns>The <see cref="ApplyChangesOperation"/>.</returns>
        internal static ApplyChangesOperation FindSingleOperation(Solution solution, CodeFixProvider fix, IEnumerable<Diagnostic> fixableDiagnostics, string? fixTitle = null)
        {
            var actions = fixableDiagnostics.SelectMany(x => GetActionsAsync(solution, fix, x).GetAwaiter().GetResult())
                                            .ToImmutableArray();
            var action = FindAction(actions, fixTitle);
            var operations = action.GetOperationsAsync(CancellationToken.None).GetAwaiter().GetResult();
            if (operations.TrySingleOfType(out ApplyChangesOperation operation))
            {
                return operation;
            }

            throw new AssertException($"Expected one operation, was {string.Join(", ", operations)}");
        }

        /// <summary>
        /// Find the <see cref="ApplyChangesOperation"/> fixable by <paramref name="fix"/> registered.
        /// </summary>
        /// <param name="solution">The <see cref="Solution"/>.</param>
        /// <param name="fix">The <see cref="CodeFixProvider"/>.</param>
        /// <param name="fixableDiagnostics">The <see cref="IEnumerable{Diagnostic}"/>.</param>
        /// <param name="fixTitle">The title of the fix.</param>
        /// <param name="operations">A collection of <see cref="ApplyChangesOperation"/>.</param>
        /// <returns>True if any were found.</returns>
        internal static bool TryFindOperations(Solution solution, CodeFixProvider fix, IEnumerable<Diagnostic> fixableDiagnostics, string fixTitle, [NotNullWhen(true)] out ImmutableArray<ApplyChangesOperation>? operations)
        {
            List<ApplyChangesOperation>? temp = null;
            foreach (var fixableDiagnostic in fixableDiagnostics)
            {
                if (TryFindOperation(solution, fix, fixableDiagnostic, fixTitle, out var operation))
                {
                    if (temp is null)
                    {
                        temp = new List<ApplyChangesOperation>();
                    }

                    temp.Add(operation);
                }
            }

            if (temp is null)
            {
                operations = default;
                return false;
            }

            operations = temp.ToImmutableArray();
            return true;
        }

        /// <summary>
        /// Find the single <see cref="ApplyChangesOperation"/> fixable by <paramref name="fix"/> registered.
        /// </summary>
        /// <param name="solution">The <see cref="Solution"/>.</param>
        /// <param name="fix">The <see cref="CodeFixProvider"/>.</param>
        /// <param name="fixableDiagnostic">The <see cref="Diagnostic"/>.</param>
        /// <param name="fixTitle">The title of the fix.</param>
        /// <param name="operation">The <see cref="ApplyChangesOperation"/>.</param>
        /// <returns>True if exactly one was found.</returns>
        internal static bool TryFindOperation(Solution solution, CodeFixProvider fix, Diagnostic fixableDiagnostic, string fixTitle, out ApplyChangesOperation operation)
        {
            var actions = GetActionsAsync(solution, fix, fixableDiagnostic).GetAwaiter().GetResult();
            if (FindAction(out var action))
            {
                var operations = action.GetOperationsAsync(CancellationToken.None).GetAwaiter().GetResult();
                return operations.TrySingleOfType(out operation);
            }

            operation = null;
            return false;

            bool FindAction(out CodeAction result)
            {
                if (fixTitle is null)
                {
                    return actions.TrySingle(out result);
                }

                return actions.TrySingle(x => x.Title == fixTitle, out result);
            }
        }

        /// <summary>
        /// Fix the solution by applying the code fix one fix at the time until it stops fixing the code.
        /// </summary>
        /// <returns>The fixed solution or the same instance if no fix.</returns>
        internal static async Task<Solution> ApplyAllFixableOneByOneAsync(Solution solution, DiagnosticAnalyzer analyzer, CodeFixProvider fix, string? fixTitle = null, CancellationToken cancellationToken = default)
        {
            var fixable = await Analyze.GetFixableDiagnosticsAsync(solution, analyzer, fix).ConfigureAwait(false);
            fixable = fixable.OrderBy(x => x.Location, LocationComparer.BySourceSpan).ToArray();
            var fixedSolution = solution;
            int count;
            do
            {
                count = fixable.Count;
                if (count == 0)
                {
                    return fixedSolution;
                }

                fixedSolution = await ApplyAsync(fixedSolution, fix, fixable[0], fixTitle, cancellationToken).ConfigureAwait(false);
                fixable = await Analyze.GetFixableDiagnosticsAsync(fixedSolution, analyzer, fix).ConfigureAwait(false);
                fixable = fixable.OrderBy(x => x.Location, LocationComparer.BySourceSpan).ToArray();
            }
            while (fixable.Count < count);
            return fixedSolution;
        }

        /// <summary>
        /// Fix the solution by applying the code fix one fix at the time until it stops fixing the code.
        /// </summary>
        /// <returns>The fixed solution or the same instance if no fix.</returns>
        internal static async Task<Solution> ApplyAllFixableScopeByScopeAsync(Solution solution, DiagnosticAnalyzer analyzer, CodeFixProvider fix, FixAllScope scope, string? fixTitle = null, CancellationToken cancellationToken = default)
        {
            var fixable = await Analyze.GetFixableDiagnosticsAsync(solution, analyzer, fix).ConfigureAwait(false);
            var fixedSolution = solution;
            int count;
            do
            {
                count = fixable.Count;
                if (count == 0)
                {
                    return fixedSolution;
                }

                var diagnosticProvider = await TestDiagnosticProvider.CreateAsync(fixedSolution, fix, fixTitle, fixable).ConfigureAwait(false);
                fixedSolution = await ApplyAsync(fix, scope, diagnosticProvider, cancellationToken).ConfigureAwait(false);
                fixable = await Analyze.GetFixableDiagnosticsAsync(fixedSolution, analyzer, fix).ConfigureAwait(false);
            }
            while (fixable.Count < count);
            return fixedSolution;
        }

        /// <summary>
        /// Fix the solution by applying the code fix.
        /// </summary>
        /// <returns>The fixed solution or the same instance if no fix.</returns>
        internal static async Task<Solution> ApplyAsync(CodeFixProvider fix, FixAllScope scope, TestDiagnosticProvider diagnosticProvider, CancellationToken cancellationToken)
        {
            var context = new FixAllContext(
                diagnosticProvider.Document,
                fix,
                scope,
                diagnosticProvider.EquivalenceKey,
                fix.FixableDiagnosticIds,
                diagnosticProvider,
                cancellationToken);
            var action = await fix.GetFixAllProvider().GetFixAsync(context).ConfigureAwait(false);

            var operations = await action.GetOperationsAsync(cancellationToken)
                                         .ConfigureAwait(false);
            if (operations.TrySingleOfType(out ApplyChangesOperation operation))
            {
                return operation.ChangedSolution;
            }

            throw new InvalidOperationException($"Expected one operation, was {string.Join(", ", operations)}");
        }

        /// <summary>
        /// Get the code actions registered by <paramref name="fix"/> for <paramref name="solution"/>.
        /// </summary>
        /// <param name="solution">The solution with the diagnostic.</param>
        /// <param name="fix">The code fix.</param>
        /// <param name="diagnostic">The diagnostic.</param>
        /// <returns>The list of registered actions.</returns>
        internal static IReadOnlyList<CodeAction> GetActions(Solution solution, CodeFixProvider fix, Diagnostic diagnostic)
        {
            var document = solution.GetDocument(diagnostic.Location.SourceTree);
            var actions = new List<CodeAction>();
            var context = new CodeFixContext(
                document,
                diagnostic,
                (a, d) => actions.Add(a),
                CancellationToken.None);
            fix.RegisterCodeFixesAsync(context).GetAwaiter().GetResult();
            return actions;
        }

        /// <summary>
        /// Get the code actions registered by <paramref name="fix"/> for <paramref name="solution"/>.
        /// </summary>
        /// <param name="solution">The solution with the diagnostic.</param>
        /// <param name="fix">The code fix.</param>
        /// <param name="diagnostic">The diagnostic.</param>
        /// <returns>The list of registered actions.</returns>
        internal static async Task<IReadOnlyList<CodeAction>> GetActionsAsync(Solution solution, CodeFixProvider fix, Diagnostic diagnostic)
        {
            var document = solution.GetDocument(diagnostic.Location.SourceTree);
            var actions = new List<CodeAction>();
            var context = new CodeFixContext(
                document,
                diagnostic,
                (a, d) => actions.Add(a),
                CancellationToken.None);
            await fix.RegisterCodeFixesAsync(context).ConfigureAwait(false);
            return actions;
        }

        private static CodeAction FindAction(IReadOnlyList<CodeAction> actions, string? fixTitle)
        {
            if (fixTitle is null)
            {
                if (actions.TrySingle(out var action))
                {
                    return action;
                }

                if (actions.Count == 0)
                {
                    throw new AssertException("Expected one code fix, was 0.");
                }

                throw new AssertException(FoundManyMessage());
            }
            else
            {
                if (actions.TrySingle(x => x.Title == fixTitle, out var action))
                {
                    return action;
                }

                if (actions.All(x => x.Title != fixTitle))
                {
                    var errorBuilder = StringBuilderPool.Borrow()
                                                        .AppendLine($"Did not find a code fix with title {fixTitle}.").AppendLine("Found:");
                    foreach (var codeAction in actions)
                    {
                        errorBuilder.AppendLine(codeAction.Title);
                    }

                    throw new AssertException(StringBuilderPool.Return(errorBuilder));
                }

                if (actions.Any(x => x.Title == fixTitle))
                {
                    throw new AssertException(FoundManyMessage());
                }

                throw new AssertException("Expected one code fix, was 0.");
            }

            string FoundManyMessage()
            {
                if (actions.Select(x => x.Title).Distinct().Count() > 1)
                {
                    var errorBuilder = StringBuilderPool
                                       .Borrow()
                                       .AppendLine($"Expected only one code fix, found {actions.Count}:");
                    foreach (var a in actions.OrderBy(x => x.Title))
                    {
                        errorBuilder.AppendLine("  " + a.Title);
                    }

                    return errorBuilder.AppendLine("Use the overload that specifies title.")
                                       .AppendLine("Or maybe you meant to call RoslynAssert.FixAll?")
                                       .Return();
                }
                else
                {
                    var errorBuilder = StringBuilderPool
                                       .Borrow()
                                       .AppendLine($"Expected only one code fix, found {actions.Count}:");
                    foreach (var a in actions.OrderBy(x => x.Title))
                    {
                        errorBuilder.AppendLine("  " + a.Title);
                    }

                    return errorBuilder.AppendLine("Or maybe you meant to call RoslynAssert.FixAll?")
                                     .Return();
                }
            }
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
            internal Document Document { get; }

            /// <summary>
            /// Gets the equivalence key for the first registered action.
            /// </summary>
            internal string EquivalenceKey { get; }

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
            /// Create an instance of <see cref="TestDiagnosticProvider"/>.
            /// </summary>
            /// <returns>The <see cref="TestDiagnosticProvider"/>.</returns>
            internal static async Task<TestDiagnosticProvider> CreateAsync(Solution solution, CodeFixProvider fix, string? fixTitle, IReadOnlyList<Diagnostic> diagnostics)
            {
                var actions = new List<CodeAction>();
                var diagnostic = diagnostics.First();
                var context = new CodeFixContext(solution.GetDocument(diagnostic.Location.SourceTree), diagnostic, (a, d) => actions.Add(a), CancellationToken.None);
                await fix.RegisterCodeFixesAsync(context).ConfigureAwait(false);
                var action = FindAction(actions, fixTitle);
                return new TestDiagnosticProvider(diagnostics, solution.GetDocument(diagnostics.First().Location.SourceTree), action.EquivalenceKey);
            }
        }
    }
}
