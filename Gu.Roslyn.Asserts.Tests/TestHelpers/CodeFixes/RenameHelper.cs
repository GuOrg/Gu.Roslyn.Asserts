namespace Gu.Roslyn.Asserts.Tests.CodeFixes;

using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.Rename;

internal static class RenameHelper
{
    internal static async Task<Solution> RenameSymbolAsync(Document document, SyntaxNode root, SyntaxToken declarationToken, string newName, CancellationToken cancellationToken)
    {
        var annotatedRoot = root.ReplaceToken(declarationToken, declarationToken.WithAdditionalAnnotations(RenameAnnotation.Create()));
        var annotatedSolution = document.Project.Solution.WithDocumentSyntaxRoot(document.Id, annotatedRoot);
        var annotatedDocument = annotatedSolution.GetDocument(document.Id) ?? throw new InvalidOperationException($"Did not find a document matching {document.Name}");
        annotatedRoot = await annotatedDocument.GetSyntaxRootAsync(cancellationToken).ConfigureAwait(false);
        var annotatedToken = annotatedRoot.FindToken(declarationToken.SpanStart);

        var semanticModel = await annotatedDocument.GetSemanticModelAsync(cancellationToken).ConfigureAwait(false);
        var symbol = semanticModel.GetDeclaredSymbol(annotatedToken.Parent, cancellationToken);

        var newSolution = await Renamer.RenameSymbolAsync(annotatedSolution, symbol, newName, document.Project.Solution.Options, cancellationToken).ConfigureAwait(false);

        // TODO: return annotatedSolution instead of newSolution if newSolution contains any new errors (for any project)
        return newSolution;
    }
}
