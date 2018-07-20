namespace Gu.Roslyn.Asserts
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CodeActions;
    using Microsoft.CodeAnalysis.CodeRefactorings;
    using Microsoft.CodeAnalysis.Diagnostics;
    using Microsoft.CodeAnalysis.Text;

    public static class Refactor
    {
        /// <summary>
        /// Apply the single refactoring registered at <paramref name="position"/>
        /// </summary>
        /// <param name="refactoring">The <see cref="CodeRefactoringProvider"/></param>
        /// <param name="testCode">The code to refactor.</param>
        /// <param name="position">The position to pass in to the RefactoringContext.</param>
        /// <param name="metadataReferences">The <see cref="MetadataReference"/> to use when compiling.</param>
        /// <returns>The refactored document.</returns>
        public static Document Apply(CodeRefactoringProvider refactoring, string testCode, int position, IReadOnlyList<MetadataReference> metadataReferences)
        {
            var sln = CodeFactory.CreateSolutionWithOneProject(
                testCode,
                CodeFactory.DefaultCompilationOptions(Array.Empty<DiagnosticAnalyzer>()),
                metadataReferences);
            var document = sln.Projects.Single().Documents.Single();
            var action = SingleAction(document, refactoring, position);
            var edit = action.GetOperationsAsync(CancellationToken.None).Result.OfType<ApplyChangesOperation>().First();
            return edit.ChangedSolution.Projects.Single().Documents.Single();
        }

        /// <summary>
        /// Apply the single refactoring registered at <paramref name="span"/>
        /// </summary>
        /// <param name="refactoring">The <see cref="CodeRefactoringProvider"/></param>
        /// <param name="testCode">The code to refactor.</param>
        /// <param name="span">The position to pass in to the RefactoringContext.</param>
        /// <param name="metadataReferences">The <see cref="MetadataReference"/> to use when compiling.</param>
        /// <returns>The refactored document.</returns>
        public static Document Apply(CodeRefactoringProvider refactoring, string testCode, TextSpan span, IReadOnlyList<MetadataReference> metadataReferences)
        {
            var actions = CodeActions(refactoring, testCode, span, metadataReferences);
            switch (actions.Count)
            {
                case 0:
                    throw new InvalidOperationException("The refactoring did not register any refactorings at the current position.");
                case 1:
                    var edit = actions[0].GetOperationsAsync(CancellationToken.None).Result.OfType<ApplyChangesOperation>().First();
                    return edit.ChangedSolution.Projects.Single().Documents.Single();
                default:
                    throw new NotSupportedException("More than one action available. Currently not supporting invoking action by index. We should add support for it.");
            }
        }

        /// <summary>
        /// Apply the single refactoring registered at <paramref name="span"/>
        /// </summary>
        /// <param name="refactoring">The <see cref="CodeRefactoringProvider"/></param>
        /// <param name="testCode">The code to refactor.</param>
        /// <param name="span">The position to pass in to the RefactoringContext.</param>
        /// <param name="index">The index of the refactoring to apply.</param>
        /// <param name="metadataReferences">The <see cref="MetadataReference"/> to use when compiling.</param>
        /// <returns>The refactored document.</returns>
        public static Document Apply(CodeRefactoringProvider refactoring, string testCode, TextSpan span, int index, IReadOnlyList<MetadataReference> metadataReferences)
        {
            var actions = CodeActions(refactoring, testCode, span, metadataReferences);
            if (actions.Count == 0)
            {
                throw new InvalidOperationException("The refactoring did not register any refactorings at the current position.");
            }

            if (actions.Count < index)
            {
                throw new InvalidOperationException("The refactoring did not register a refactoring for the current index.");
            }

            var edit = actions[index].GetOperationsAsync(CancellationToken.None).Result.OfType<ApplyChangesOperation>().First();
            return edit.ChangedSolution.Projects.Single().Documents.Single();
        }

        private static List<CodeAction> CodeActions(CodeRefactoringProvider refactoring, string testCode, TextSpan span, IReadOnlyList<MetadataReference> metadataReferences)
        {
            var sln = CodeFactory.CreateSolutionWithOneProject(
                testCode,
                CodeFactory.DefaultCompilationOptions(Array.Empty<DiagnosticAnalyzer>()),
                metadataReferences);
            var document = sln.Projects.Single().Documents.Single();
            var actions = new List<CodeAction>();
            var context = new CodeRefactoringContext(document, span, a => actions.Add(a), CancellationToken.None);
            refactoring.ComputeRefactoringsAsync(context).GetAwaiter().GetResult();
            return actions;
        }

        private static CodeAction SingleAction(Document document, CodeRefactoringProvider refactoring, int position)
        {
            var context = new RefactoringContext(document, refactoring, position);
            var token = context.SyntaxRoot.FindToken(position);
            refactoring.ComputeRefactoringsAsync(context.CreateRefactoringContext(token.Span)).GetAwaiter().GetResult();
            switch (context.Actions.Count)
            {
                case 0:
                    return SingleAction(context, token.Parent);
                case 1:
                    return context.Actions[0];
                default:
                    throw new NotSupportedException("More than one action available. Currently not supporting invoking action by index. We should add support for it.");
            }
        }

        private static CodeAction SingleAction(RefactoringContext context, SyntaxNode node)
        {
            if (node != null &&
                node.SpanStart == context.Position)
            {
                context.Refactoring.ComputeRefactoringsAsync(context.CreateRefactoringContext(node.Span)).GetAwaiter().GetResult();
                switch (context.Actions.Count)
                {
                    case 0:
                        return SingleAction(context, node.Parent);
                    case 1:
                        return context.Actions[0];
                    default:
                        throw new NotSupportedException("More than one action available. Currently not supporting invoking action by index. We should add support for it.");
                }
            }

            throw new InvalidOperationException("The refactoring did not register any refactorings at the position.");
        }

        private class RefactoringContext
        {
            private readonly List<CodeAction> actions;

            public RefactoringContext(Document document, CodeRefactoringProvider refactoring, int position)
            {
                this.Document = document;
                this.Refactoring = refactoring;
                this.Position = position;
                this.SyntaxRoot = document.GetSyntaxRootAsync(CancellationToken.None).GetAwaiter().GetResult();
                this.actions = new List<CodeAction>();
            }

            public Document Document { get; }

            public CodeRefactoringProvider Refactoring { get; }

            public int Position { get; }

            public SyntaxNode SyntaxRoot { get; }

            public IReadOnlyList<CodeAction> Actions => this.actions;

            public CodeRefactoringContext CreateRefactoringContext(TextSpan span)
            {
                return new CodeRefactoringContext(this.Document, span, a => this.actions.Add(a), CancellationToken.None);
            }
        }
    }
}
