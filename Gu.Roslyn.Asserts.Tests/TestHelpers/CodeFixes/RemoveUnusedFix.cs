namespace Gu.Roslyn.Asserts.Tests.CodeFixes
{
    using System.Collections.Immutable;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CodeActions;
    using Microsoft.CodeAnalysis.CodeFixes;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using Microsoft.CodeAnalysis.Editing;

    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(RemoveUnusedFix))]
    internal class RemoveUnusedFix : CodeFixProvider
    {
        public override ImmutableArray<string> FixableDiagnosticIds { get; } = ImmutableArray.Create("CS0067");

        public override FixAllProvider GetFixAllProvider() => WellKnownFixAllProviders.BatchFixer;

        public override async Task RegisterCodeFixesAsync(CodeFixContext context)
        {
            var document = context.Document;
            var root = await document.GetSyntaxRootAsync(context.CancellationToken).ConfigureAwait(false);

            foreach (var diagnostic in context.Diagnostics)
            {
                var member = root.FindNode(diagnostic.Location.SourceSpan)
                               .FirstAncestorOrSelf<MemberDeclarationSyntax>();
                context.RegisterCodeFix(
                    CodeAction.Create(
                        $"Remove {member}",
                        cancellationToken => ApplyFixAsync(cancellationToken, document, member),
                        nameof(RemoveUnusedFix)),
                    diagnostic);
            }
        }

        private static async Task<Document> ApplyFixAsync(CancellationToken cancellationToken, Document document, MemberDeclarationSyntax member)
        {
            var editor = await DocumentEditor.CreateAsync(document, cancellationToken)
                                             .ConfigureAwait(false);
            editor.RemoveNode(member);
            return editor.GetChangedDocument();
        }
    }
}
