namespace Gu.Roslyn.Asserts.Analyzers
{
    using System.Collections.Generic;
    using System.Collections.Immutable;
    using System.Composition;
    using System.Threading;
    using System.Threading.Tasks;
    using Gu.Roslyn.AnalyzerExtensions;
    using Gu.Roslyn.CodeFixExtensions;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CodeActions;
    using Microsoft.CodeAnalysis.CodeFixes;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using Microsoft.CodeAnalysis.Editing;

    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(MoveFix))]
    [Shared]
    internal class MoveFix : CodeFixProvider
    {
        public override ImmutableArray<string> FixableDiagnosticIds { get; } = ImmutableArray.Create(
            Descriptors.GURA06TestShouldBeInCorrectClass.Id);

        public override FixAllProvider GetFixAllProvider() => WellKnownFixAllProviders.BatchFixer;

        public override async Task RegisterCodeFixesAsync(CodeFixContext context)
        {
            var syntaxRoot = await context.Document.GetSyntaxRootAsync(context.CancellationToken)
                                          .ConfigureAwait(false);
            var semanticModel = await context.Document.GetSemanticModelAsync(context.CancellationToken)
                                             .ConfigureAwait(false);
            foreach (var diagnostic in context.Diagnostics)
            {
                if (syntaxRoot.TryFindNodeOrAncestor(diagnostic, out MethodDeclarationSyntax? methodDeclaration) &&
                    diagnostic.Properties.TryGetValue(nameof(IdentifierNameSyntax), out var name))
                {
                    if (semanticModel.LookupSymbols(methodDeclaration.SpanStart, name: name).TrySingle(out var symbol))
                    {
                        if (symbol is INamedTypeSymbol namedType &&
                            namedType.TrySingleDeclaration(context.CancellationToken, out ClassDeclarationSyntax? classDeclarationSyntax))
                        {
                            context.RegisterCodeFix(
                                CodeAction.Create(
                                    $"Move method to {name}",
                                    cancellationToken => MoveMethod(cancellationToken),
                                    nameof(MoveFix)),
                                diagnostic);

                            async Task<Solution> MoveMethod(CancellationToken cancellationToken)
                            {
                                var editor = await DocumentEditor.CreateAsync(context.Document.Project.GetDocument(classDeclarationSyntax.SyntaxTree), cancellationToken)
                                                                 .ConfigureAwait(false);
                                _ = editor.AddMethod(classDeclarationSyntax, methodDeclaration);
                                return context.Document.Project.Solution.WithDocumentSyntaxRoot(
                                    context.Document.Id,
                                    syntaxRoot.RemoveNode(methodDeclaration, SyntaxRemoveOptions.AddElasticMarker))
                                              .WithDocumentSyntaxRoot(
                                                  editor.OriginalDocument.Id,
                                                  editor.GetChangedRoot());
                            }
                        }
                    }
                    else
                    {
                        context.RegisterCodeFix(
                            CodeAction.Create(
                                $"Create class {name} and move method.",
                                cancellationToken => CreateClassAndMoveMethod(cancellationToken),
                                nameof(MoveFix)),
                            diagnostic);

                        async Task<Solution> CreateClassAndMoveMethod(CancellationToken cancellationToken)
                        {
                            var root = await methodDeclaration.SyntaxTree.GetRootAsync(cancellationToken)
                                                              .ConfigureAwait(false);
                            var classDeclaration = (ClassDeclarationSyntax)methodDeclaration.Parent;
                            root = root.ReplaceNode(
                                classDeclaration,
                                classDeclaration.RemoveNodes(MembersToRemove(), SyntaxRemoveOptions.AddElasticMarker)
                                                .WithIdentifier(SyntaxFactory.Identifier(name)));
                            return context.Document.Project.Solution.WithDocumentSyntaxRoot(
                                context.Document.Id,
                                syntaxRoot.RemoveNode(methodDeclaration, SyntaxRemoveOptions.AddElasticMarker))
                                          .AddDocument(DocumentId.CreateNewId(context.Document.Project.Id), name, root, context.Document.Folders);

                            IEnumerable<MemberDeclarationSyntax> MembersToRemove()
                            {
                                foreach (var member in classDeclaration.Members)
                                {
                                    if (member.IsKind(SyntaxKind.MethodDeclaration) &&
                                        member != methodDeclaration)
                                    {
                                        yield return member;
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }
    }
}
