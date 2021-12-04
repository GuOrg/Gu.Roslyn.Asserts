namespace Gu.Roslyn.Asserts.Tests.CodeFixes
{
    using System.Collections.Immutable;
    using System.Threading;
    using System.Threading.Tasks;

    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CodeActions;
    using Microsoft.CodeAnalysis.CodeFixes;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using Microsoft.CodeAnalysis.Editing;
    using Microsoft.CodeAnalysis.Simplification;

    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(InsertFullyQualifiedSimplifiedMethodFix))]
    internal class InsertFullyQualifiedSimplifiedMethodFix : CodeFixProvider
    {
        public override ImmutableArray<string> FixableDiagnosticIds { get; } = ImmutableArray.Create(ClassMustHaveMethodAnalyzer.DiagnosticId);

        public override FixAllProvider GetFixAllProvider() => WellKnownFixAllProviders.BatchFixer;

        public override async Task RegisterCodeFixesAsync(CodeFixContext context)
        {
            var document = context.Document;
            var root = await document.GetSyntaxRootAsync(context.CancellationToken).ConfigureAwait(false);

            foreach (var diagnostic in context.Diagnostics)
            {
                var classDeclaration = root.FindNode(diagnostic.Location.SourceSpan)
                                           .FirstAncestorOrSelf<ClassDeclarationSyntax>();
                context.RegisterCodeFix(
                    CodeAction.Create(
                        $"Add method to {classDeclaration}",
                        cancellationToken => ApplyFixAsync(document, classDeclaration, cancellationToken),
                        nameof(InsertEventFix)),
                    diagnostic);
            }
        }

        private static async Task<Document> ApplyFixAsync(Document document, ClassDeclarationSyntax classDeclaration, CancellationToken cancellationToken)
        {
            var editor = await DocumentEditor.CreateAsync(document, cancellationToken)
                                             .ConfigureAwait(false);
            editor.AddMember(
                classDeclaration,
                SyntaxFactory.MethodDeclaration(
                    attributeLists: default,
                    modifiers: SyntaxTokenList.Create(SyntaxFactory.Token(SyntaxKind.PublicKeyword)),
                    returnType: SyntaxFactory.ParseTypeName("System.EventHandler?").WithAdditionalAnnotations(Simplifier.Annotation),
                    explicitInterfaceSpecifier: null,
                    identifier: SyntaxFactory.Identifier("M"),
                    typeParameterList: default,
                    parameterList: SyntaxFactory.ParameterList(),
                    constraintClauses: default,
                    body: null,
                    expressionBody: SyntaxFactory.ArrowExpressionClause(SyntaxFactory.LiteralExpression(SyntaxKind.NullLiteralExpression)),
                    semicolonToken: SyntaxFactory.Token(SyntaxKind.SemicolonToken)));
            return editor.GetChangedDocument();
        }
    }
}
