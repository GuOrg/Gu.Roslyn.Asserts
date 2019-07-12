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

    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(IndicateErrorPositionFix))]
    [Shared]
    public class IndicateErrorPositionFix : DocumentEditorCodeFixProvider
    {
        public override ImmutableArray<string> FixableDiagnosticIds { get; } = ImmutableArray.Create(
            GURA02IndicateErrorPosition.DiagnosticId);

        protected override DocumentEditorFixAllProvider FixAllProvider() => null;

        protected override async Task RegisterCodeFixesAsync(DocumentEditorCodeFixContext context)
        {
            var syntaxRoot = await context.Document.GetSyntaxRootAsync(context.CancellationToken)
                                          .ConfigureAwait(false);
            foreach (var diagnostic in context.Diagnostics)
            {
                if (diagnostic.AdditionalLocations.TrySingle(out var location) &&
                    syntaxRoot.FindNode(location.SourceSpan) is LiteralExpressionSyntax literal)
                {
                    context.RegisterCodeFix(
                        $"Add ↓ to the start of the string literal (move it manually after).",
                        (editor, _) => editor.ReplaceNode(literal, literal.WithToken(SyntaxFactory.Literal("↓" + literal.Token.ValueText))),
                        nameof(IndicateErrorPositionFix),
                        diagnostic);
                }
            }
        }
    }
}
