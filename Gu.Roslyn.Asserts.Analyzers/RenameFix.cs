namespace Gu.Roslyn.Asserts.Analyzers
{
    using System.Collections.Immutable;
    using System.Composition;
    using System.Threading.Tasks;
    using Gu.Roslyn.AnalyzerExtensions;
    using Gu.Roslyn.CodeFixExtensions;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CodeActions;
    using Microsoft.CodeAnalysis.CodeFixes;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using Microsoft.CodeAnalysis.Rename;

    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(RenameFix))]
    [Shared]
    public class RenameFix : CodeFixProvider
    {
        public override ImmutableArray<string> FixableDiagnosticIds { get; } = ImmutableArray.Create(
            GURA01NameOfLocalShouldMatchParameter.DiagnosticId,
            GURA03NameFieldToFirstClass.DiagnosticId,
            GURA04NameClassToMatchAsserts.DiagnosticId);

        public override FixAllProvider GetFixAllProvider() => WellKnownFixAllProviders.BatchFixer;

        public override async Task RegisterCodeFixesAsync(CodeFixContext context)
        {
            var syntaxRoot = await context.Document.GetSyntaxRootAsync(context.CancellationToken)
                                          .ConfigureAwait(false);
            var semanticModel = await context.Document.GetSemanticModelAsync(context.CancellationToken)
                                             .ConfigureAwait(false);
            foreach (var diagnostic in context.Diagnostics)
            {
                if (syntaxRoot.TryFindNodeOrAncestor(diagnostic, out SyntaxNode node) &&
                    diagnostic.Properties.TryGetValue(nameof(IdentifierNameSyntax), out var name) &&
                    semanticModel.TryGetSymbol(node, context.CancellationToken, out ISymbol local) &&
                    semanticModel.LookupSymbols(node.SpanStart, name: name).IsEmpty)
                {
                    context.RegisterCodeFix(
                        CodeAction.Create(
                            $"Rename to '{name}'.",
                            cancellationToken => Renamer.RenameSymbolAsync(
                                context.Document.Project.Solution,
                                local,
                                name,
                                null,
                                cancellationToken),
                            nameof(RenameFix)),
                        diagnostic);
                }
            }
        }
    }
}
