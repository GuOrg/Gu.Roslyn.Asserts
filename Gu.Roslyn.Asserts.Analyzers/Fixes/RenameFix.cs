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
    internal class RenameFix : CodeFixProvider
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
                if (diagnostic.Properties.TryGetValue(nameof(IdentifierNameSyntax), out var name))
                {
                    if (diagnostic.Id == Descriptors.GURA05NameFileToMatchClass.Id)
                    {
                        context.RegisterCodeFix(
                            CodeAction.Create(
                                $"Rename file to '{name}'",
                                _ => Task.FromResult(context.Document.Project.Solution.WithDocumentName(context.Document.Id, name + ".cs")),
                                nameof(RenameFix)),
                            diagnostic);
                    }
                    else if (diagnostic.Id == Descriptors.GURA04NameClassToMatchAsserts.Id &&
                            syntaxRoot.FindToken(diagnostic.Location.SourceSpan.Start) is { } identifier &&
                            semanticModel.TryGetSymbol(identifier.Parent, context.CancellationToken, out INamedTypeSymbol? namedType) &&
                            semanticModel.LookupSymbols(identifier.SpanStart, name: name).IsEmpty)
                    {
                        context.RegisterCodeFix(
                            CodeAction.Create(
                                $"Rename to '{name}'",
                                async cancellationToken =>
                                {
                                    var sln = context.Document.Project.Solution.WithDocumentName(context.Document.Id, name + ".cs");
                                    var options = await context.Document.GetOptionsAsync(cancellationToken)
                                                               .ConfigureAwait(false);
                                    return await Renamer.RenameSymbolAsync(
                                                            sln,
                                                            namedType,
                                                            name,
                                                            options,
                                                            cancellationToken)
                                                        .ConfigureAwait(false);
                                },
                                nameof(RenameFix)),
                            diagnostic);
                    }
                    else if (syntaxRoot.FindToken(diagnostic.Location.SourceSpan.Start) is { } token &&
                             semanticModel.TryGetSymbol(token.Parent, context.CancellationToken, out ISymbol? symbol) &&
                             semanticModel.LookupSymbols(token.SpanStart, name: name).IsEmpty)
                    {
                        context.RegisterCodeFix(
                            CodeAction.Create(
                                $"Rename to '{name}'",
                                async cancellationToken =>
                                {
                                    var options = await context.Document.GetOptionsAsync(cancellationToken)
                                                               .ConfigureAwait(false);
                                    return await Renamer.RenameSymbolAsync(
                                                            context.Document.Project.Solution,
                                                            symbol,
                                                            name,
                                                            options,
                                                            cancellationToken)
                                                        .ConfigureAwait(false);
                                },
                                nameof(RenameFix)),
                            diagnostic);
                    }
                }
            }
        }
    }
}
