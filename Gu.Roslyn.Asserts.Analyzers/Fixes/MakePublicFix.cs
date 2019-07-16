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
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;

    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(MakePublicFix))]
    [Shared]
    public class MakePublicFix : CodeFixProvider
    {
        public override ImmutableArray<string> FixableDiagnosticIds { get; } = ImmutableArray.Create(
            Descriptors.ShouldBePublic.Id);

        public override FixAllProvider GetFixAllProvider() => WellKnownFixAllProviders.BatchFixer;

        public override async Task RegisterCodeFixesAsync(CodeFixContext context)
        {
            var syntaxRoot = await context.Document.GetSyntaxRootAsync(context.CancellationToken)
                                          .ConfigureAwait(false);
            var semanticModel = await context.Document.GetSemanticModelAsync(context.CancellationToken).ConfigureAwait(false);
            foreach (var diagnostic in context.Diagnostics)
            {
                if (syntaxRoot.TryFindNodeOrAncestor(diagnostic, out ObjectCreationExpressionSyntax objectCreation) &&
                    semanticModel.TryGetType(objectCreation, context.CancellationToken, out var type) &&
                    type.TrySingleDeclaration(context.CancellationToken, out ClassDeclarationSyntax declaration))
                {
                    context.RegisterCodeFix(
                        CodeAction.Create(
                            "Make public.",
                            cancellationToken => Task.FromResult(
                                context.Document.Project.Solution
                                       .GetDocument(declaration.SyntaxTree)
                                       .WithSyntaxRoot(
                                declaration.SyntaxTree.GetRoot(cancellationToken).ReplaceNode(
                                    declaration,
                                    MakePublic(declaration)))),
                            nameof(MakePublicFix)),
                        diagnostic);

                    MemberDeclarationSyntax MakePublic(MemberDeclarationSyntax before)
                    {
                        switch (before)
                        {
                            case FieldDeclarationSyntax field:
                                return field.WithModifiers(Public(field.Modifiers));
                            case PropertyDeclarationSyntax property:
                                return property.WithModifiers(Public(property.Modifiers));
                            case ClassDeclarationSyntax @class:
                                return @class.WithModifiers(Public(@class.Modifiers));
                            default:
                                return before;
                        }

                        SyntaxTokenList Public(SyntaxTokenList modifiers)
                        {
                            if (modifiers.TryFirst(out var first))
                            {
                                switch (first.Kind())
                                {
                                    case SyntaxKind.PrivateKeyword:
                                    case SyntaxKind.ProtectedKeyword:
                                    case SyntaxKind.InternalKeyword:
                                        return modifiers.Replace(
                                            first,
                                            SyntaxFactory.Token(SyntaxKind.PublicKeyword).WithTriviaFrom(first));
                                    default:
                                        return modifiers.Insert(0, SyntaxFactory.Token(SyntaxKind.PublicKeyword));
                                }
                            }

                            return SyntaxTokenList.Create(SyntaxFactory.Token(SyntaxKind.PublicKeyword));
                        }
                    }
                }
            }
        }
    }
}
