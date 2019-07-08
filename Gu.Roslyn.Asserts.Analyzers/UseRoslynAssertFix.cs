namespace Gu.Roslyn.Asserts.Analyzers
{
    using System.Collections.Immutable;
    using System.Composition;
    using System.Threading.Tasks;
    using Gu.Roslyn.CodeFixExtensions;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CodeFixes;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;

    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(UseRoslynAssertFix))]
    [Shared]
    public class UseRoslynAssertFix : DocumentEditorCodeFixProvider
    {
        public override ImmutableArray<string> FixableDiagnosticIds { get; } = ImmutableArray.Create("CS0103", "CS0234");

        protected override async Task RegisterCodeFixesAsync(DocumentEditorCodeFixContext context)
        {
            var syntaxRoot = await context.Document.GetSyntaxRootAsync(context.CancellationToken)
                                          .ConfigureAwait(false);
            foreach (var diagnostic in context.Diagnostics)
            {
                if (TryFindIdentifier(out IdentifierNameSyntax identifierName))
                {
                    context.RegisterCodeFix(
                        "Change to: RoslynAssert.",
                        (editor, _) => editor.ReplaceNode(
                            identifierName,
                            x => x.WithIdentifier(SyntaxFactory.Identifier("RoslynAssert").WithTriviaFrom(x.Identifier))),
                        nameof(UseRoslynAssertFix),
                        diagnostic);
                }

                bool TryFindIdentifier(out IdentifierNameSyntax result)
                {
                    const string AnalyzerAssert = "AnalyzerAssert";
                    if (diagnostic.Id == "CS0103" &&
                        syntaxRoot.TryFindNode(diagnostic, out result))
                    {
                        return result.Identifier.ValueText == AnalyzerAssert;
                    }

                    if (diagnostic.Id == "CS0234" &&
                        syntaxRoot.TryFindNode(diagnostic, out MemberAccessExpressionSyntax memberAccess))
                    {
                        while (memberAccess.Parent is MemberAccessExpressionSyntax parent)
                        {
                            memberAccess = parent;
                        }

                        result = (memberAccess.Expression as MemberAccessExpressionSyntax)?.Name as IdentifierNameSyntax;
                        return result?.Identifier.ValueText == AnalyzerAssert;
                    }

                    result = null;
                    return false;
                }
            }
        }
    }
}
