namespace Gu.Roslyn.Asserts.Tests.CodeFixes;

using System.Collections.Immutable;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Editing;

[ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(InsertMethodFix))]
internal class InsertMethodFix : CodeFixProvider
{
    public override ImmutableArray<string> FixableDiagnosticIds { get; } = ImmutableArray.Create(ClassMustHaveEventAnalyzer.DiagnosticId);

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
        editor.AddMember(classDeclaration, editor.Generator.MethodDeclaration("M", accessibility: Accessibility.Public));
        return editor.GetChangedDocument();
    }
}
