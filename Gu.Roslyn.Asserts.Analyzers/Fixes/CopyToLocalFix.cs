namespace Gu.Roslyn.Asserts.Analyzers
{
    using System.Collections.Immutable;
    using System.Composition;
    using System.Threading.Tasks;
    using Gu.Roslyn.AnalyzerExtensions;
    using Gu.Roslyn.CodeFixExtensions;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CodeFixes;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;

    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(CopyToLocalFix))]
    [Shared]
    internal class CopyToLocalFix : DocumentEditorCodeFixProvider
    {
        public override ImmutableArray<string> FixableDiagnosticIds { get; } = ImmutableArray.Create(
            Descriptors.GURA10UseLocal.Id);

        protected override DocumentEditorFixAllProvider? FixAllProvider() => null;

        protected override async Task RegisterCodeFixesAsync(DocumentEditorCodeFixContext context)
        {
            var syntaxRoot = await context.Document.GetSyntaxRootAsync(context.CancellationToken)
                                          .ConfigureAwait(false);
            var semanticModel = await context.Document.GetSemanticModelAsync(context.CancellationToken)
                                             .ConfigureAwait(false);
            foreach (var diagnostic in context.Diagnostics)
            {
                if (syntaxRoot.TryFindNode(diagnostic, out ExpressionSyntax expression) &&
                    semanticModel.TryGetSymbol(expression, context.CancellationToken, out var symbol) &&
                    diagnostic.AdditionalLocations.TrySingle(out var valueLocation) &&
                    syntaxRoot.FindNode(valueLocation.SourceSpan) is ExpressionSyntax value &&
                    expression.TryFirstAncestor(out BlockSyntax block))
                {
                    context.RegisterCodeFix(
                        $"Copy to local.",
                        (editor, _) =>
                        {
                            var identifierName = SyntaxFactory.IdentifierName(symbol.Name.ToFirstCharLower());
                            editor.ReplaceNode(
                                expression,
                                identifierName);
                            editor.InsertBefore(
                                block.Statements[0],
                                editor.Generator.LocalDeclarationStatement(identifierName.Identifier.ValueText, value));
                        },
                        nameof(CopyToLocalFix),
                        diagnostic);
                }
            }
        }
    }
}
