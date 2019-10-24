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

    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(ChainAssertReplaceFix))]
    [Shared]
    internal class ChainAssertReplaceFix : DocumentEditorCodeFixProvider
    {
        public override ImmutableArray<string> FixableDiagnosticIds { get; } = ImmutableArray.Create(
            Descriptors.GURA11ChainAssertReplace.Id);

        protected override DocumentEditorFixAllProvider FixAllProvider() => null;

        protected override async Task RegisterCodeFixesAsync(DocumentEditorCodeFixContext context)
        {
            var syntaxRoot = await context.Document.GetSyntaxRootAsync(context.CancellationToken)
                                          .ConfigureAwait(false);
            foreach (var diagnostic in context.Diagnostics)
            {
                if (syntaxRoot.TryFindNodeOrAncestor(diagnostic, out InvocationExpressionSyntax invocation) &&
                    invocation.Expression is MemberAccessExpressionSyntax memberAccess &&
                    invocation.Parent is AssignmentExpressionSyntax assignment &&
                    assignment.Parent is ExpressionStatementSyntax statement &&
                    diagnostic.AdditionalLocations.TrySingle(out var additionalLocation) &&
                    syntaxRoot.FindNode(additionalLocation.SourceSpan) is LiteralExpressionSyntax literal)
                {
                    context.RegisterCodeFix(
                        "Chain AssertReplace.",
                        (e, c) =>
                        {
                            _ = e.ReplaceNode(
                                literal,
                                x => invocation.WithExpression(
                                    SyntaxFactory.MemberAccessExpression(
                                        SyntaxKind.SimpleMemberAccessExpression,
                                        literal,
                                        memberAccess.Name)));
                            e.RemoveNode(statement);
                        },
                        "Chain AssertReplace.",
                        diagnostic);
                }
            }
        }
    }
}
