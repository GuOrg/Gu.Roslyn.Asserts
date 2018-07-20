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

    public static partial class AnalyzerAssert
    {
        public static void Refactoring(CodeRefactoringProvider refactoring, string codeWithPositionIndicated, string fixedCode)
        {
            var position = codeWithPositionIndicated.IndexOf("↓");
            var testCode = codeWithPositionIndicated.AssertReplace("↓", string.Empty);
            var sln = CodeFactory.CreateSolutionWithOneProject(
                testCode,
                CodeFactory.DefaultCompilationOptions(Array.Empty<DiagnosticAnalyzer>()),
                MetadataReferences);
            var document = sln.Projects.Single().Documents.Single();
            var action = SingleAction(document, refactoring, position);
            var edit = action.GetOperationsAsync(CancellationToken.None).Result.OfType<ApplyChangesOperation>().First();
            var refactored = edit.ChangedSolution.Projects.Single().Documents.Single();
            CodeAssert.AreEqual(refactored, fixedCode);
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
