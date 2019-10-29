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

    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(MakePublicStaticFix))]
    [Shared]
    internal class MakePublicStaticFix : CodeFixProvider
    {
        public override ImmutableArray<string> FixableDiagnosticIds { get; } = ImmutableArray.Create(
            Descriptors.GURA07TestClassShouldBePublicStatic.Id);

        public override FixAllProvider GetFixAllProvider() => WellKnownFixAllProviders.BatchFixer;

        public override async Task RegisterCodeFixesAsync(CodeFixContext context)
        {
            var syntaxRoot = await context.Document.GetSyntaxRootAsync(context.CancellationToken)
                                          .ConfigureAwait(false);
            foreach (var diagnostic in context.Diagnostics)
            {
                if (syntaxRoot.TryFindNodeOrAncestor(diagnostic, out ClassDeclarationSyntax? classDeclaration))
                {
                    context.RegisterCodeFix(
                        CodeAction.Create(
                            "Make public static.",
                            _ => Task.FromResult(context.Document.WithSyntaxRoot(
                                syntaxRoot.ReplaceNode(
                                    classDeclaration,
                                    PublicStaticRewriter.Update(classDeclaration)))),
                            nameof(MakePublicStaticFix)),
                        diagnostic);
                }
            }
        }

        private class PublicStaticRewriter : CSharpSyntaxRewriter
        {
            private static readonly PublicStaticRewriter Default = new PublicStaticRewriter();

            public override SyntaxNode VisitFieldDeclaration(FieldDeclarationSyntax node)
            {
                if (IsPublicStatic(node.Modifiers))
                {
                    return node;
                }

                return node.WithModifiers(Static(node.Modifiers));
            }

            public override SyntaxNode VisitPropertyDeclaration(PropertyDeclarationSyntax node)
            {
                if (IsPublicStatic(node.Modifiers))
                {
                    return node;
                }

                return node.WithModifiers(Static(node.Modifiers));
            }

            public override SyntaxNode VisitMethodDeclaration(MethodDeclarationSyntax node)
            {
                if (IsPublicStatic(node.Modifiers))
                {
                    return node;
                }

                return node.WithModifiers(PublicStatic(node.Modifiers));
            }

            internal static ClassDeclarationSyntax Update(ClassDeclarationSyntax classDeclaration)
            {
                classDeclaration = classDeclaration.WithModifiers(PublicStatic(classDeclaration.Modifiers));
                return (ClassDeclarationSyntax)Default.Visit(classDeclaration);
            }

            private static SyntaxTokenList PublicStatic(SyntaxTokenList modifiers)
            {
                if (modifiers.TryFirst(out var first))
                {
                    switch (first.Kind())
                    {
                        case SyntaxKind.PrivateKeyword:
                        case SyntaxKind.ProtectedKeyword:
                        case SyntaxKind.InternalKeyword:
                            modifiers = modifiers.Replace(
                                first,
                                SyntaxFactory.Token(SyntaxKind.PublicKeyword).WithTriviaFrom(first));
                            break;
                        case SyntaxKind.PublicKeyword:
                            break;
                        default:
                            modifiers = modifiers.Insert(0, SyntaxFactory.Token(SyntaxKind.PublicKeyword));
                            break;
                    }
                }
                else
                {
                    modifiers = SyntaxTokenList.Create(SyntaxFactory.Token(SyntaxKind.PublicKeyword));
                }

                return Static(modifiers);
            }

            private static SyntaxTokenList Static(SyntaxTokenList modifiers)
            {
                if (!modifiers.Any(SyntaxKind.StaticKeyword))
                {
                    var first = modifiers.First();
                    switch (first.Kind())
                    {
                        case SyntaxKind.PrivateKeyword:
                        case SyntaxKind.ProtectedKeyword:
                        case SyntaxKind.InternalKeyword:
                        case SyntaxKind.PublicKeyword:
                            return modifiers.Insert(1, SyntaxFactory.Token(SyntaxKind.StaticKeyword));
                        default:
                            return modifiers.Insert(0, SyntaxFactory.Token(SyntaxKind.StaticKeyword));
                    }
                }

                return modifiers;
            }

            private static bool IsPublicStatic(SyntaxTokenList modifiers) => modifiers.Any(SyntaxKind.PublicKeyword) &&
                                                                             modifiers.Any(SyntaxKind.StaticKeyword);
        }
    }
}
