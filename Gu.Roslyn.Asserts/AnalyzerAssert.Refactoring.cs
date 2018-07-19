namespace Gu.Roslyn.Asserts
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using Microsoft.CodeAnalysis.CodeActions;
    using Microsoft.CodeAnalysis.CodeRefactorings;
    using Microsoft.CodeAnalysis.Diagnostics;

    public static partial class AnalyzerAssert
    {
        public static void Refactoring(CodeRefactoringProvider refactoring, string testCode, string fixedCode)
        {
            var sources = DiagnosticsAndSources.Create(ExpectedDiagnostic.Create("Refactoring"), new[] { testCode });
            var sln = CodeFactory.CreateSolutionWithOneProject(
                sources.Code,
                CodeFactory.DefaultCompilationOptions(Array.Empty<DiagnosticAnalyzer>()),
                MetadataReferences);
            var actions = new List<CodeAction>();
            var document = sln.Projects.Single().Documents.Single();
            var root = document.GetSyntaxRootAsync(CancellationToken.None).GetAwaiter().GetResult();
            var position = root.SyntaxTree.GetText(CancellationToken.None)
                               .Lines.GetPosition(sources.ExpectedDiagnostics.Single().Span.Span.Start);
            var token = root.FindToken(position);
            var context = new CodeRefactoringContext(document, token.Span, a => actions.Add(a), CancellationToken.None);
            refactoring.ComputeRefactoringsAsync(context).GetAwaiter().GetResult();
            var edit = actions.Single().GetOperationsAsync(CancellationToken.None).Result.OfType<ApplyChangesOperation>().First();
            var refactored = edit.ChangedSolution.Projects.Single().Documents.Single();
            CodeAssert.AreEqual(refactored, fixedCode);
        }
    }
}
