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
            if (solution is null)
            {
                throw new ArgumentNullException(nameof(solution));
            }

            if (fix is null)
            {
                throw new ArgumentNullException(nameof(fix));
            }

            if (diagnostic is null)
            {
                throw new ArgumentNullException(nameof(diagnostic));
            }

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
        public static Solution Apply(Solution solution, CodeFixProvider fix, IReadOnlyList<ProjectDiagnostics> diagnostics, string? fixTitle = null)
        {
            if (solution is null)
            {
                throw new ArgumentNullException(nameof(solution));
            }

            if (fix is null)
            {
                throw new ArgumentNullException(nameof(fix));
            }

            if (diagnostics is null)
            {
                throw new ArgumentNullException(nameof(diagnostics));
            }

            var fixable = diagnostics.SelectMany(x => x.FixableBy(fix)).ToArray();
            if (fixable.TrySingle(out var single))
            {
                return Apply(solution, fix, single, fixTitle);
            }

            var trees = fixable.Select(x => x.Location.SourceTree).Distinct().ToArray();
            if (trees.Length == 1)
            {
                var document = solution.Projects.SelectMany(x => x.Documents)
                                       .Single(x => x.GetSyntaxTreeAsync().GetAwaiter().GetResult() == trees[0]);
                var provider = TestDiagnosticProvider.CreateAsync(solution, fix, fixTitle, fixable).GetAwaiter().GetResult();
                var context = new FixAllContext(document, fix, FixAllScope.Document, provider.EquivalenceKey, fixable.Select(x => x.Id), provider, CancellationToken.None);
                var action = WellKnownFixAllProviders.BatchFixer.GetFixAsync(context).GetAwaiter().GetResult();
                var operations = action?.GetOperationsAsync(CancellationToken.None).GetAwaiter().GetResult() ??
                                 throw new InvalidOperationException("action!.GetOperationsAsync() returned null");
                if (operations.TrySingleOfType<CodeActionOperation, ApplyChangesOperation>(out var operation))
                {
                    return operation!.ChangedSolution;
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
            if (solution is null)
            {
                throw new ArgumentNullException(nameof(solution));
            }

            if (fix is null)
            {
                throw new ArgumentNullException(nameof(fix));
            }

            if (diagnostic is null)
            {
                throw new ArgumentNullException(nameof(diagnostic));
            }

            var actions = await GetActionsAsync(solution, fix, diagnostic).ConfigureAwait(false);
            var action = FindAction(actions, fixTitle);
            var operations = await action.GetOperationsAsync(cancellationToken).ConfigureAwait(false);
            if (operations.TrySingleOfType<CodeActionOperation, ApplyChangesOperation>(out var operation))
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
            if (operations.TrySingleOfType<CodeActionOperation, ApplyChangesOperation>(out var operation))
            {
                return operation!;
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
        internal static bool TryFindOperation(Solution solution, CodeFixProvider fix, Diagnostic fixableDiagnostic, string fixTitle, [NotNullWhen(true)] out ApplyChangesOperation? operation)
        {
            var actions = GetActionsAsync(solution, fix, fixableDiagnostic).GetAwaiter().GetResult();
            if (FindAction(out var action))
            {
                var operations = action!.GetOperationsAsync(CancellationToken.None).GetAwaiter().GetResult();
                return operations.TrySingleOfType<CodeActionOperation, ApplyChangesOperation>(out operation);
            }

            operation = null;
            return false;

            bool FindAction(out CodeAction? result)
            {
                if (fixTitle is null)
                {
                    return actions.TrySingle(out result);
                }

                return actions.TrySingle(x => x!.Title == fixTitle, out result);
            }
        }

        /// <summary>
        /// Fix the solution by applying the code fix one fix at the time until it stops fixing the code.
        /// </summary>
        /// <returns>The fixed solution or the same instance if no fix.</returns>
        internal static async Task<Solution> ApplyAllFixableOneByOneAsync(Solution solution, DiagnosticAnalyzer analyzer, CodeFixProvider fix, string? fixTitle = null, CancellationToken cancellationToken = default)
        {
            var n = 0;
            while (await ApplyNext(solution, analyzer, fix, fixTitle, cancellationToken).ConfigureAwait(false) is { } fixedSolution)
            {
                if (n > 1_000)
                {
                    return solution;
                }

                n++;
                solution = fixedSolution;
            }

            return solution;

            static async Task<Solution?> ApplyNext(Solution solution, DiagnosticAnalyzer analyzer, CodeFixProvider fix, string? fixTitle, CancellationToken cancellationToken)
            {
                if (await FirstFixableDiagnosticAsync(solution, analyzer, fix).ConfigureAwait(false) is { } fixable &&
                    await ApplyAsync(solution, fix, fixable, fixTitle, cancellationToken).ConfigureAwait(false) is
                    { } temp &&
                    temp != solution)
                {
                    return temp;
                }

                return null;

                static async Task<Diagnostic?> FirstFixableDiagnosticAsync(Solution solution, DiagnosticAnalyzer analyzer, CodeFixProvider fix)
                {
                    foreach (var project in solution.Projects)
                    {
                        var projectDiagnostics = await Analyze.GetDiagnosticsAsync(analyzer, project).ConfigureAwait(false);
                        var diagnostic = projectDiagnostics.FixableBy(fix)
                                                           .OrderBy(x => x.Location, LocationComparer.BySourceSpan)
                                                           .FirstOrDefault();
                        if (diagnostic is not null)
                        {
                            return diagnostic;
                        }
                    }

                    return null;
                }
            }
        }

        /// <summary>
        /// Fix the solution by applying the code fix one fix at the time until it stops fixing the code.
        /// </summary>
        /// <returns>The fixed solution or the same instance if no fix.</returns>
        internal static async Task<Solution> ApplyAllFixableScopeByScopeAsync(Solution solution, DiagnosticAnalyzer analyzer, CodeFixProvider fix, FixAllScope scope, string? fixTitle = null, CancellationToken cancellationToken = default)
        {
            while (await ApplyNext(solution, analyzer, fix, scope, fixTitle, cancellationToken).ConfigureAwait(false) is { } fixedSolution)
            {
                solution = fixedSolution;
            }

            return solution;

            static async Task<Solution?> ApplyNext(Solution solution, DiagnosticAnalyzer analyzer, CodeFixProvider fix, FixAllScope scope, string? fixTitle, CancellationToken cancellationToken)
            {
                var fixable = await FixableDiagnosticsAsync(solution, analyzer, fix).ConfigureAwait(false);
                if (fixable.Count > 0)
                {
                    var diagnosticProvider = await TestDiagnosticProvider.CreateAsync(solution, fix, fixTitle, fixable).ConfigureAwait(false);
                    var fixedSolution = await ApplyAsync(fix, scope, diagnosticProvider, cancellationToken).ConfigureAwait(false);
                    if (fixedSolution != solution)
                    {
                        return fixedSolution;
                    }
                }

                return null;

                static async Task<IReadOnlyList<Diagnostic>> FixableDiagnosticsAsync(Solution solution, DiagnosticAnalyzer analyzer, CodeFixProvider fix)
                {
                    var fixableDiagnostics = new List<Diagnostic>();
                    foreach (var project in solution.Projects)
                    {
                        var projectDiagnostics = await Analyze.GetDiagnosticsAsync(analyzer, project).ConfigureAwait(false);
                        fixableDiagnostics.AddRange(projectDiagnostics.FixableBy(fix));
                    }

                    return fixableDiagnostics;
                }
            }
        }

        /// <summary>
        /// Fix the solution by applying the code fix.
        /// </summary>
        /// <returns>The fixed solution or the same instance if no fix.</returns>
        internal static async Task<Solution> ApplyAsync(CodeFixProvider fix, FixAllScope scope, TestDiagnosticProvider diagnosticProvider, CancellationToken cancellationToken)
        {
            var fixAllProvider = fix.GetFixAllProvider() ?? throw new InvalidOperationException($"No fix all provider for {fix}");
            var context = new FixAllContext(
                diagnosticProvider.Document,
                fix,
                scope,
                diagnosticProvider.EquivalenceKey,
                fix.FixableDiagnosticIds,
                diagnosticProvider,
                cancellationToken);
            var action = await fixAllProvider.GetFixAsync(context).ConfigureAwait(false) ?? throw new InvalidOperationException("fixAllProvider.GetFixAsync(context) returned null.");

            var operations = await action.GetOperationsAsync(cancellationToken)
                                         .ConfigureAwait(false);
            if (operations.TrySingleOfType<CodeActionOperation, ApplyChangesOperation>(out var operation))
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
            var document = solution.GetDocument(diagnostic.Location.SourceTree) ??
                           throw new InvalidOperationException("solution.GetDocument(diagnostic.Location.SourceTree) returned null.");
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
            var document = solution.GetDocument(diagnostic.Location.SourceTree) ??
                           throw new InvalidOperationException("solution.GetDocument(diagnostic.Location.SourceTree) returned null.");
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
                var diagnostic = diagnostics[0];
                var document = solution.GetDocument(diagnostic.Location.SourceTree) ??
                               throw new InvalidOperationException("solution.GetDocument(diagnostic.Location.SourceTree) returned null.");
                var context = new CodeFixContext(document, diagnostic, (a, d) => actions.Add(a), CancellationToken.None);
                await fix.RegisterCodeFixesAsync(context).ConfigureAwait(false);
                var action = FindAction(actions, fixTitle);
                return new TestDiagnosticProvider(diagnostics, document, action.EquivalenceKey!);
            }
        }
    }
}
