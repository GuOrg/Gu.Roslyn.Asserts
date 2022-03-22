namespace Gu.Roslyn.Asserts.Tests.CodeFixes;

using System.Collections.Immutable;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

[ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(InitializeFieldWithTwoFix))]
internal class InitializeFieldWithTwoFix : CodeFixProvider
{
    public override ImmutableArray<string> FixableDiagnosticIds { get; } =
        ImmutableArray.Create(FieldNameMustNotBeginWithUnderscore.DiagnosticId);

    public override FixAllProvider GetFixAllProvider() => WellKnownFixAllProviders.BatchFixer;

    public override async Task RegisterCodeFixesAsync(CodeFixContext context)
    {
        var document = context.Document;
        var root = await document.GetSyntaxRootAsync(context.CancellationToken).ConfigureAwait(false);

        foreach (var diagnostic in context.Diagnostics)
        {
            var initializer = root.FindNode(diagnostic.Location.SourceSpan)
                            .FirstAncestorOrSelf<FieldDeclarationSyntax>()
                            .Declaration
                            .Variables
                            .Last()
                            .Initializer;
            context.RegisterCodeFix(
                CodeAction.Create(
                    $"Initialize with 2",
                    cancellationToken => Task.FromResult(
                        document.WithSyntaxRoot(
                            root.ReplaceNode(
                                initializer,
                                initializer.WithValue(SyntaxFactory.LiteralExpression(SyntaxKind.NumericLiteralExpression, SyntaxFactory.Literal(2)))))),
                    nameof(InitializeFieldWithTwoFix)),
                diagnostic);
        }
    }
}
