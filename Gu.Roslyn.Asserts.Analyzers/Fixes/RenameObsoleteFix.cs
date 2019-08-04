namespace Gu.Roslyn.Asserts.Analyzers
{
    using System.Collections.Immutable;
    using System.Composition;
    using System.Globalization;
    using System.Threading.Tasks;
    using Gu.Roslyn.CodeFixExtensions;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CodeFixes;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;

    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(RenameObsoleteFix))]
    [Shared]
    internal class RenameObsoleteFix : DocumentEditorCodeFixProvider
    {
        public override ImmutableArray<string> FixableDiagnosticIds { get; } = ImmutableArray.Create("CS0103", "CS0234", "CS0618", "CS1739");

        protected override async Task RegisterCodeFixesAsync(DocumentEditorCodeFixContext context)
        {
            var syntaxRoot = await context.Document.GetSyntaxRootAsync(context.CancellationToken)
                                          .ConfigureAwait(false);
            foreach (var diagnostic in context.Diagnostics)
            {
                if (TryFindUpdate(out var expression, out var newText))
                {
                    context.RegisterCodeFix(
                        $"Change to: {newText}.",
                        (editor, _) => editor.ReplaceNode(
                            expression,
                            x => SyntaxFactory.ParseExpression(newText).WithTriviaFrom(x)),
                        nameof(RenameObsoleteFix),
                        diagnostic);
                }

                bool TryFindUpdate(out ExpressionSyntax result, out string text)
                {
                    switch (diagnostic.Id)
                    {
                        case "CS0103" when syntaxRoot.TryFindNode(diagnostic, out result):
                            {
                                var message = diagnostic.GetMessage(CultureInfo.InvariantCulture);
                                switch (message)
                                {
                                    case "The name 'AnalyzerAssert' does not exist in the current context":
                                        text = "RoslynAssert";
                                        return true;
                                }

                                break;
                            }

                        case "CS0234" when syntaxRoot.TryFindNode(diagnostic, out MemberAccessExpressionSyntax qualifiedName):
                            {
                                var message = diagnostic.GetMessage(CultureInfo.InvariantCulture);
                                switch (message)
                                {
                                    case "The type or namespace name 'AnalyzerAssert' does not exist in the namespace 'Gu.Roslyn.Asserts' (are you missing an assembly reference?)":
                                        result = qualifiedName.Name;
                                        text = "RoslynAssert";
                                        return true;
                                }

                                break;
                            }

                        case "CS0618" when syntaxRoot.TryFindNode(diagnostic, out result):
                            {
                                var message = diagnostic.GetMessage(CultureInfo.InvariantCulture);
                                if (message.StartsWith("'RoslynAssert.MetadataReferences' is obsolete:"))
                                {
                                    text = "MetadataReferences.FromAttributes()";
                                    return true;
                                }

                                if (message.StartsWith("'RoslynAssert.SuppressedDiagnostics' is obsolete:"))
                                {
                                    text = "SuppressWarnings.FromAttributes()";
                                    return true;
                                }

                                break;
                            }

                        case "CS1739" when syntaxRoot.TryFindNode(diagnostic, out result):
                            {
                                var message = diagnostic.GetMessage(CultureInfo.InvariantCulture);
                                if (message.Contains("suppressedDiagnostics"))
                                {
                                    text = "suppressWarnings";
                                    return true;
                                }

                                switch (message)
                                {
                                    case "The best overload for 'Diagnostics' does not have a parameter named 'codeWithErrorsIndicated'":
                                    case "The best overload for 'NoFix' does not have a parameter named 'codeWithErrorsIndicated'":
                                        text = "code";
                                        return true;
                                    case "The best overload for 'CodeFix' does not have a parameter named 'code'":
                                    case "The best overload for 'FixAll' does not have a parameter named 'code'":
                                    case "The best overload for 'CodeFix' does not have a parameter named 'codeWithErrorsIndicated'":
                                    case "The best overload for 'FixAll' does not have a parameter named 'codeWithErrorsIndicated'":
                                        text = "before";
                                        return true;
                                    case "The best overload for 'CodeFix' does not have a parameter named 'fixedCode'":
                                    case "The best overload for 'FixAll' does not have a parameter named 'fixedCode'":
                                    case "The best overload for 'CodeFix' does not have a parameter named 'fixedcode'":
                                    case "The best overload for 'FixAll' does not have a parameter named 'fixedcode'":
                                        text = "after";
                                        return true;
                                    case "The best overload for 'NoFix' does not have a parameter named 'codeFix'":
                                        text = "fix";
                                        return true;
                                }

                                break;
                            }
                    }

                    text = null;
                    result = null;
                    return false;
                }
            }
        }
    }
}
