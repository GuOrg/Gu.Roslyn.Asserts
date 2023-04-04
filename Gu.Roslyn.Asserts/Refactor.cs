namespace Gu.Roslyn.Asserts;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeRefactorings;
using Microsoft.CodeAnalysis.Text;

/// <summary>
/// Helper methods for working with <see cref="CodeRefactoringProvider"/>.
/// </summary>
public static class Refactor
{
    /// <summary>
    /// Apply the single refactoring registered at <paramref name="position"/>.
    /// </summary>
    /// <param name="refactoring">The <see cref="CodeRefactoringProvider"/>.</param>
    /// <param name="testCode">The code to refactor.</param>
    /// <param name="position">The position to pass in to the RefactoringContext.</param>
    /// <param name="settings">The <see cref="Settings"/>.</param>
    /// <returns>The refactored document.</returns>
    public static Document Apply(CodeRefactoringProvider refactoring, string testCode, int position, Settings? settings = null)
    {
        if (refactoring is null)
        {
            throw new ArgumentNullException(nameof(refactoring));
        }

        if (testCode is null)
        {
            throw new ArgumentNullException(nameof(testCode));
        }

        var sln = CodeFactory.CreateSolutionWithOneProject(
            testCode,
            settings);
        var document = sln.Projects.Single().Documents.Single();
        var action = SingleAction(document, refactoring, position);
        var edit = action.GetOperationsAsync(CancellationToken.None).GetAwaiter().GetResult().OfType<ApplyChangesOperation>().First();
        return edit.ChangedSolution.Projects.Single().Documents.Single();
    }

    /// <summary>
    /// Apply the single refactoring registered at <paramref name="position"/>.
    /// </summary>
    /// <param name="refactoring">The <see cref="CodeRefactoringProvider"/>.</param>
    /// <param name="testCode">The code to refactor.</param>
    /// <param name="position">The position to pass in to the RefactoringContext.</param>
    /// <param name="title">The title of the refactoring to apply.</param>
    /// <param name="settings">The <see cref="Settings"/>.</param>
    /// <returns>The refactored document.</returns>
    public static Document Apply(CodeRefactoringProvider refactoring, string testCode, int position, string title, Settings? settings = null)
    {
        if (refactoring is null)
        {
            throw new ArgumentNullException(nameof(refactoring));
        }

        if (testCode is null)
        {
            throw new ArgumentNullException(nameof(testCode));
        }

        if (title is null)
        {
            throw new ArgumentNullException(nameof(title));
        }

        var actions = CodeActions(refactoring, testCode, position, settings)
                      .Where(x => x.Title == title)
                      .ToArray();
        switch (actions.Length)
        {
            case 0:
                throw new InvalidOperationException("The refactoring did not register any refactorings at the current position.");
            case 1:
                var edit = actions[0].GetOperationsAsync(CancellationToken.None).GetAwaiter().GetResult().OfType<ApplyChangesOperation>().First();
                return edit.ChangedSolution.Projects.Single().Documents.Single();
            default:
                throw new NotSupportedException("More than one action available. Currently not supporting invoking action by index. We should add support for it.");
        }
    }

    /// <summary>
    /// Apply the single refactoring registered at <paramref name="position"/>.
    /// </summary>
    /// <param name="refactoring">The <see cref="CodeRefactoringProvider"/>.</param>
    /// <param name="testCode">The code to refactor.</param>
    /// <param name="position">The position to pass in to the RefactoringContext.</param>
    /// <param name="index">The index of the refactoring to apply.</param>
    /// <param name="settings">The <see cref="Settings"/>.</param>
    /// <returns>The refactored document.</returns>
    public static Document Apply(CodeRefactoringProvider refactoring, string testCode, int position, int index, Settings? settings = null)
    {
        if (refactoring is null)
        {
            throw new ArgumentNullException(nameof(refactoring));
        }

        if (testCode is null)
        {
            throw new ArgumentNullException(nameof(testCode));
        }

        var actions = CodeActions(refactoring, testCode, position, settings);
        if (actions.Count == 0)
        {
            throw new InvalidOperationException("The refactoring did not register any refactorings at the current position.");
        }

        if (actions.Count < index)
        {
            throw new InvalidOperationException("The refactoring did not register a refactoring for the current index.");
        }

        var edit = actions[index].GetOperationsAsync(CancellationToken.None).GetAwaiter().GetResult().OfType<ApplyChangesOperation>().First();
        return edit.ChangedSolution.Projects.Single().Documents.Single();
    }

    /// <summary>
    /// Apply the single refactoring registered at <paramref name="span"/>.
    /// </summary>
    /// <param name="refactoring">The <see cref="CodeRefactoringProvider"/>.</param>
    /// <param name="testCode">The code to refactor.</param>
    /// <param name="span">The position to pass in to the RefactoringContext.</param>
    /// <param name="settings">The <see cref="Settings"/>.</param>
    /// <returns>The refactored document.</returns>
    public static Document Apply(CodeRefactoringProvider refactoring, string testCode, TextSpan span, Settings? settings = null)
    {
        if (refactoring is null)
        {
            throw new ArgumentNullException(nameof(refactoring));
        }

        if (testCode is null)
        {
            throw new ArgumentNullException(nameof(testCode));
        }

        var actions = CodeActions(refactoring, testCode, span, settings);
        switch (actions.Count)
        {
            case 0:
                throw new InvalidOperationException("The refactoring did not register any refactorings at the current position.");
            case 1:
                var edit = actions[0].GetOperationsAsync(CancellationToken.None).GetAwaiter().GetResult().OfType<ApplyChangesOperation>().First();
                return edit.ChangedSolution.Projects.Single().Documents.Single();
            default:
                throw new NotSupportedException("More than one action available. Currently not supporting invoking action by index. We should add support for it.");
        }
    }

    /// <summary>
    /// Apply the single refactoring registered at <paramref name="span"/>.
    /// </summary>
    /// <param name="refactoring">The <see cref="CodeRefactoringProvider"/>.</param>
    /// <param name="testCode">The code to refactor.</param>
    /// <param name="span">The position to pass in to the RefactoringContext.</param>
    /// <param name="title">The title of the refactoring to apply.</param>
    /// <param name="settings">The <see cref="Settings"/>.</param>
    /// <returns>The refactored document.</returns>
    public static Document Apply(CodeRefactoringProvider refactoring, string testCode, TextSpan span, string title, Settings? settings = null)
    {
        if (refactoring is null)
        {
            throw new ArgumentNullException(nameof(refactoring));
        }

        if (testCode is null)
        {
            throw new ArgumentNullException(nameof(testCode));
        }

        if (title is null)
        {
            throw new ArgumentNullException(nameof(title));
        }

        var actions = CodeActions(refactoring, testCode, span, settings)
                      .Where(x => x.Title == title)
                      .ToArray();
        switch (actions.Length)
        {
            case 0:
                throw new InvalidOperationException("The refactoring did not register any refactorings at the current position.");
            case 1:
                var edit = actions[0].GetOperationsAsync(CancellationToken.None).GetAwaiter().GetResult().OfType<ApplyChangesOperation>().First();
                return edit.ChangedSolution.Projects.Single().Documents.Single();
            default:
                throw new NotSupportedException("More than one action available. Currently not supporting invoking action by index. We should add support for it.");
        }
    }

    /// <summary>
    /// Apply the single refactoring registered at <paramref name="span"/>.
    /// </summary>
    /// <param name="refactoring">The <see cref="CodeRefactoringProvider"/>.</param>
    /// <param name="testCode">The code to refactor.</param>
    /// <param name="span">The position to pass in to the RefactoringContext.</param>
    /// <param name="index">The index of the refactoring to apply.</param>
    /// <param name="settings">The <see cref="Settings"/>.</param>
    /// <returns>The refactored document.</returns>
    public static Document Apply(CodeRefactoringProvider refactoring, string testCode, TextSpan span, int index, Settings? settings = null)
    {
        if (refactoring is null)
        {
            throw new ArgumentNullException(nameof(refactoring));
        }

        if (testCode is null)
        {
            throw new ArgumentNullException(nameof(testCode));
        }

        var actions = CodeActions(refactoring, testCode, span, settings);
        if (actions.Count == 0)
        {
            throw new InvalidOperationException("The refactoring did not register any refactorings at the current position.");
        }

        if (actions.Count < index)
        {
            throw new InvalidOperationException("The refactoring did not register a refactoring for the current index.");
        }

        var edit = actions[index].GetOperationsAsync(CancellationToken.None).GetAwaiter().GetResult().OfType<ApplyChangesOperation>().First();
        return edit.ChangedSolution.Projects.Single().Documents.Single();
    }

    /// <summary>
    /// Get the code actions registered at current position.
    /// </summary>
    /// <param name="refactoring">The <see cref="CodeRefactoringProvider"/>.</param>
    /// <param name="testCode">The code to refactor.</param>
    /// <param name="span">The position to pass in to the RefactoringContext.</param>
    /// <param name="settings">The <see cref="Settings"/>.</param>
    /// <returns>An <see cref="IReadOnlyList{CodeAction}"/> registered by <paramref name="refactoring"/>.</returns>
    public static IReadOnlyList<CodeAction> CodeActions(CodeRefactoringProvider refactoring, string testCode, TextSpan span, Settings? settings = null)
    {
        if (refactoring is null)
        {
            throw new ArgumentNullException(nameof(refactoring));
        }

        if (testCode is null)
        {
            throw new ArgumentNullException(nameof(testCode));
        }

        var sln = CodeFactory.CreateSolutionWithOneProject(
            testCode,
            settings);
        var document = sln.Projects.Single().Documents.Single();
        var actions = new List<CodeAction>();
        var context = new CodeRefactoringContext(document, span, a => actions.Add(a), CancellationToken.None);
        refactoring.ComputeRefactoringsAsync(context).GetAwaiter().GetResult();
        return actions;
    }

    /// <summary>
    /// Get the code actions registered at current position.
    /// </summary>
    /// <param name="refactoring">The <see cref="CodeRefactoringProvider"/>.</param>
    /// <param name="testCode">The code to refactor.</param>
    /// <param name="position">The position to pass in to the RefactoringContext.</param>
    /// <param name="settings">The <see cref="Settings"/>.</param>
    /// <returns>An <see cref="IReadOnlyList{CodeAction}"/> registered by <paramref name="refactoring"/>.</returns>
    public static IReadOnlyList<CodeAction> CodeActions(CodeRefactoringProvider refactoring, string testCode, int position, Settings? settings = null)
    {
        if (refactoring is null)
        {
            throw new ArgumentNullException(nameof(refactoring));
        }

        if (testCode is null)
        {
            throw new ArgumentNullException(nameof(testCode));
        }

        var sln = CodeFactory.CreateSolutionWithOneProject(
            testCode,
            settings);
        var document = sln.Projects.Single().Documents.Single();
        var context = new RefactoringContext(document, refactoring, position);
        var token = context.SyntaxRoot.FindToken(position);
        refactoring.ComputeRefactoringsAsync(context.CreateRefactoringContext(token.Span)).GetAwaiter().GetResult();

        var node = token.Parent;
        while (node != null &&
               node.SpanStart == position)
        {
            refactoring.ComputeRefactoringsAsync(context.CreateRefactoringContext(node.Span)).GetAwaiter().GetResult();
            node = node.Parent;
        }

        return context.Actions;
    }

    private static CodeAction SingleAction(Document document, CodeRefactoringProvider refactoring, int position)
    {
        var context = new RefactoringContext(document, refactoring, position);
        var token = context.SyntaxRoot.FindToken(position);
        refactoring.ComputeRefactoringsAsync(context.CreateRefactoringContext(token.Span)).GetAwaiter().GetResult();
        return context.Actions.Count switch
        {
            0 when token.Parent is { }
                => SingleAction(context, token.Parent),
            1 => context.Actions[0],
            _ => throw new NotSupportedException("More than one action available. Currently not supporting invoking action by index. We should add support for it."),
        };
    }

    private static CodeAction SingleAction(RefactoringContext context, SyntaxNode node)
    {
        if (node.SpanStart == context.Position)
        {
            context.Refactoring.ComputeRefactoringsAsync(context.CreateRefactoringContext(node.Span)).GetAwaiter().GetResult();
            return context.Actions.Count switch
            {
                0 when node.Parent is { }
                    => SingleAction(context, node.Parent),
                1 => context.Actions[0],
                _ => throw new NotSupportedException("More than one action available. Currently not supporting invoking action by index. We should add support for it."),
            };
        }

        throw new InvalidOperationException("The refactoring did not register any refactorings at the position.");
    }

    private class RefactoringContext
    {
        private readonly List<CodeAction> actions;

        internal RefactoringContext(Document document, CodeRefactoringProvider refactoring, int position)
        {
            this.Document = document;
            this.Refactoring = refactoring;
            this.Position = position;
            this.SyntaxRoot = document.GetSyntaxRootAsync(CancellationToken.None).GetAwaiter().GetResult() ?? throw new InvalidOperationException("document.GetSyntaxRootAsync() returned null.");
            this.actions = new List<CodeAction>();
        }

        internal Document Document { get; }

        internal CodeRefactoringProvider Refactoring { get; }

        internal int Position { get; }

        internal SyntaxNode SyntaxRoot { get; }

        internal IReadOnlyList<CodeAction> Actions => this.actions;

        internal CodeRefactoringContext CreateRefactoringContext(TextSpan span)
        {
            return new CodeRefactoringContext(this.Document, span, a => this.actions.Add(a), CancellationToken.None);
        }
    }
}
