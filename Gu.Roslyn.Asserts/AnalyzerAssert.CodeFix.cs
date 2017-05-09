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

    public static partial class AnalyzerAssert
    {
        public static async Task CodeFixAsync(DiagnosticAnalyzer analyzer, CodeFixProvider codeFix, IEnumerable<string> code, IEnumerable<string> fixedCode)
        {
            if (!codeFix.FixableDiagnosticIds.Contains(analyzer.SupportedDiagnostics[0].Id))
            {
                Fail.WithMessage($"The code fix {codeFix} does not support fixing {analyzer.SupportedDiagnostics[0].Id}");
            }

            (IReadOnlyList<ExpectedDiagnostic> expecteds, IEnumerable<string> sources) = ExpectedDiagnostic.FromCode(analyzer, code);
            if (expecteds.Count != 1)
            {
                Fail.WithMessage("Expected code to have exactly one error with position indicated by '↓'");
            }

            var sln = CodeFactory.CreateSolution(sources, new[] { analyzer }, References);
            var results = new List<ImmutableArray<Diagnostic>>();
            foreach (var project in sln.Projects)
            {
                var compilation = await project.GetCompilationAsync(CancellationToken.None)
                                               .ConfigureAwait(false);

                var withAnalyzers = compilation.WithAnalyzers(
                    ImmutableArray.Create(analyzer),
                    project.AnalyzerOptions,
                    CancellationToken.None);
                results.Add(await withAnalyzers.GetAnalyzerDiagnosticsAsync(CancellationToken.None)
                                               .ConfigureAwait(false));
            }

            var actuals = results.SelectMany(x => x)
                                 .OrderBy(d => d.Id)
                                 .ThenBy(d => d.Location.GetMappedLineSpan(), FileLinePositionSpanComparer.Default)
                                 .ToArray();
            if (expecteds.Count != actuals.Length)
            {
                Fail.WithMessage($"Expected count does not match actual.{Environment.NewLine}" +
                                 $"Expected: {expecteds.Count}{Environment.NewLine}" +
                                 $"Actual:   {actuals.Length}");
            }

            expecteds = expecteds.OrderBy(d => d.Analyzer.SupportedDiagnostics[0].Id)
                                 .ThenBy(d => d.Span, FileLinePositionSpanComparer.Default)
                                 .ToArray();
            for (var i = 0; i < expecteds.Count; i++)
            {
                var expected = expecteds[i];
                var actual = actuals[i];
                if (expected.Analyzer.SupportedDiagnostics[0].Id != actual.Id)
                {
                    Fail.WithMessage($"Expected id does not match actual.{Environment.NewLine}" +
                                     $"Expected: {expected.Analyzer.SupportedDiagnostics[0].Id}{Environment.NewLine}" +
                                     $"Actual:   {actual.Id}");
                }

                var actualSpan = actual.Location.GetMappedLineSpan();
                if (expected.Span.Path != actualSpan.Path)
                {
                    Fail.WithMessage($"Expected id does not match actual.{Environment.NewLine}" +
                                     $"Expected: {expected.Span.Path}" +
                                     $"Actual:   {actual.Location.SourceTree.FilePath}");
                }

                if (expected.Span.StartLinePosition != actualSpan.StartLinePosition)
                {
                    Fail.WithMessage($"Expected id does not match actual.{Environment.NewLine}" +
                                     $"Expected: {expected.Span.Path}" +
                                     $"Actual:   {actual.Location.SourceTree.FilePath}");
                }
            }

            foreach (var project in sln.Projects)
            {
                var actions = new List<CodeAction>();
                var diagnostic = actuals[0];
                var context = new CodeFixContext(project.GetDocument(diagnostic.Location.SourceTree), diagnostic,
                    (a, d) => actions.Add(a), CancellationToken.None);
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
            }
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