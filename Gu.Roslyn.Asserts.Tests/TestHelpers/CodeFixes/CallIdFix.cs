namespace Gu.Roslyn.Asserts.Tests.CodeFixes
{
    using System.Collections.Immutable;
    using System.Text;
    using System.Threading.Tasks;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CodeActions;
    using Microsoft.CodeAnalysis.CodeFixes;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using Microsoft.CodeAnalysis.Text;

    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(CallIdFix))]
    internal class CallIdFix : CodeFixProvider
    {
        public override ImmutableArray<string> FixableDiagnosticIds { get; } = ImmutableArray.Create(
            CallIdAnalyzer.DiagnosticId);

        public override FixAllProvider GetFixAllProvider()
        {
            return CustomFixAllProviders.BatchFixer;
        }

        public override async Task RegisterCodeFixesAsync(CodeFixContext context)
        {
            var document = context.Document;
            var root = await document.GetSyntaxRootAsync(context.CancellationToken).ConfigureAwait(false);

            foreach (var diagnostic in context.Diagnostics)
            {
                var creation = (ObjectCreationExpressionSyntax)root.FindNode(diagnostic.Location.SourceSpan);
                context.RegisterCodeFix(
                    CodeAction.Create(
                        "Call ID()",
                        cancellationToken =>
                        {
                            var sln = context.Document.Project.Solution;
                            return Task.FromResult(
                                sln.WithDocumentSyntaxRoot(
                                       context.Document.Id,
                                       WithCallId(creation))
                                   .AddDocument(
                                       DocumentId.CreateNewId(context.Document.Project.Id),
                                       "Extensions.generated.cs",
                                       SourceText.From("namespace N\r\n{\r\n    public static class Extensions\r\n    {\r\n        public static T Id<T>(this T t) => t;\r\n    }\r\n}", Encoding.UTF8)));
                        },
                        nameof(CallIdFix)),
                    diagnostic);

                SyntaxNode WithCallId(ExpressionSyntax expression)
                {
                    return root.ReplaceNode(
                        expression,
                        SyntaxFactory.InvocationExpression(
                            SyntaxFactory.MemberAccessExpression(
                                SyntaxKind.SimpleMemberAccessExpression,
                                expression,
                                SyntaxFactory
                                    .IdentifierName("Id"))));
                }
            }
        }
    }
}
