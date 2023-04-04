namespace Gu.Roslyn.Asserts.Analyzers;

using System.Collections.Immutable;
using System.Composition;
using System.Threading.Tasks;
using Gu.Roslyn.CodeFixExtensions;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

[ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(IndicateErrorPositionFix))]
[Shared]
internal class IndicateErrorPositionFix : DocumentEditorCodeFixProvider
{
    public override ImmutableArray<string> FixableDiagnosticIds { get; } = ImmutableArray.Create(
        Descriptors.GURA02IndicateErrorPosition.Id);

    protected override DocumentEditorFixAllProvider? FixAllProvider() => null;

    protected override async Task RegisterCodeFixesAsync(DocumentEditorCodeFixContext context)
    {
        var syntaxRoot = await context.Document.GetSyntaxRootAsync(context.CancellationToken)
                                      .ConfigureAwait(false);
        foreach (var diagnostic in context.Diagnostics)
        {
            if (syntaxRoot.TryFindNode(diagnostic, out LiteralExpressionSyntax? literal))
            {
                context.RegisterCodeFix(
                    "Add ↓ to the start of the string literal (move it manually after)",
                    (editor, _) => editor.ReplaceNode(
                        literal,
                        literal.WithToken(SyntaxFactory.Literal(InsertPosition(literal.Token.Text), $"↓{literal.Token.ValueText}"))),
                    nameof(IndicateErrorPositionFix),
                    diagnostic);

                static string InsertPosition(string text)
                {
                    var i = text.IndexOf('"') + 1;
                    return $"{text.Substring(0, i)}↓{text.Substring(i)}";
                }
            }
        }
    }
}
