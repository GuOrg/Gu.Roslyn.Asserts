namespace Gu.Roslyn.Asserts.Analyzers
{
    using System.Collections.Immutable;
    using System.Composition;
    using System.Threading.Tasks;
    using Gu.Roslyn.AnalyzerExtensions;
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
            Descriptors.GURA01NameShouldMatchParameter.Id,
            Descriptors.GURA03NameShouldMatchCode.Id,
            Descriptors.GURA04NameClassToMatchAsserts.Id,
            Descriptors.GURA05NameFileToMatchClass.Id);

        public override FixAllProvider GetFixAllProvider() => WellKnownFixAllProviders.BatchFixer;

        public override async Task RegisterCodeFixesAsync(CodeFixContext context)
        {
            var syntaxRoot = await context.Document.GetSyntaxRootAsync(context.CancellationToken)
                                          .ConfigureAwait(false);
            var semanticModel = await context.Document.GetSemanticModelAsync(context.CancellationToken)
                                             .ConfigureAwait(false);
            foreach (var diagnostic in context.Diagnostics)
            {
                if (diagnostic.Id == Descriptors.GURA05NameFileToMatchClass.Id &&
                    diagnostic.Properties.TryGetValue(nameof(IdentifierNameSyntax), out var name))
                {
                    context.RegisterCodeFix(
                        CodeAction.Create(
                            $"Rename file to '{name}'.",
                            _ => Task.FromResult(context.Document.WithName(name)),
                            nameof(RenameFix)),
                        diagnostic);
                }
                else if (syntaxRoot.FindToken(diagnostic.Location.SourceSpan.Start) is SyntaxToken token &&
                         semanticModel.TryGetSymbol(token.Parent, context.CancellationToken, out ISymbol symbol) &&
                         diagnostic.Properties.TryGetValue(nameof(IdentifierNameSyntax), out name) &&
                         semanticModel.LookupSymbols(token.SpanStart, name: name).IsEmpty)
                {
                    context.RegisterCodeFix(
                        CodeAction.Create(
                            $"Rename to '{name}'.",
                            cancellationToken => Renamer.RenameSymbolAsync(
                                context.Document.Project.Solution,
                                symbol,
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
