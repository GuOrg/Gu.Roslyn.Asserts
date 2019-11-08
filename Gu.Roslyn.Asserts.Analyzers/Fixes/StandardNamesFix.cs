namespace Gu.Roslyn.Asserts.Analyzers
{
    using System.Collections.Concurrent;
    using System.Collections.Immutable;
    using System.Composition;
    using System.Linq;
    using System.Text.RegularExpressions;
    using System.Threading.Tasks;
    using Gu.Roslyn.AnalyzerExtensions;
    using Gu.Roslyn.CodeFixExtensions;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CodeActions;
    using Microsoft.CodeAnalysis.CodeFixes;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;

    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(StandardNamesFix))]
    [Shared]
    internal class StandardNamesFix : CodeFixProvider
    {
        public override ImmutableArray<string> FixableDiagnosticIds { get; } = ImmutableArray.Create(
            Descriptors.GURA09UseStandardNames.Id);

        public override FixAllProvider GetFixAllProvider() => WellKnownFixAllProviders.BatchFixer;

        public override async Task RegisterCodeFixesAsync(CodeFixContext context)
        {
            var syntaxRoot = await context.Document.GetSyntaxRootAsync(context.CancellationToken)
                                          .ConfigureAwait(false);
            foreach (var diagnostic in context.Diagnostics)
            {
                if (syntaxRoot.TryFindNodeOrAncestor(diagnostic, out LiteralExpressionSyntax? stringLiteral) &&
                    stringLiteral.TryFirstAncestor(out MethodDeclarationSyntax? containingMethod) &&
                    diagnostic.Properties.TryGetValue(nameof(WordAndLocation.Word), out var before))
                {
                    if (CodeLiteral.TryCreate(stringLiteral, out var codeLiteral) &&
                        codeLiteral.Value.Identifiers.TrySingle(x => x.Span.Contains(diagnostic.Location.SourceSpan.Start - stringLiteral.SpanStart), out var identifier))
                    {
                        for (int i = 0; i < 4; i++)
                        {
                            if (NewName(i == 0 ? (int?)null : i) is { } newName)
                            {
                                context.RegisterCodeFix(
                                    CodeAction.Create(
                                        $"Replace {before} with {newName}",
                                        _ => Task.FromResult(
                                            context.Document.WithSyntaxRoot(
                                                syntaxRoot.ReplaceNode(
                                                    containingMethod,
                                                    ReplaceRewriter.Update(containingMethod, before, newName)))),
                                        $"Replace {before} with {newName}"),
                                    diagnostic);
                            }
                        }

                        string? NewName(int? n)
                        {
                            switch (identifier.Parent)
                            {
                                case ClassDeclarationSyntax parent:
                                    return FindNewTypeName(parent, $"C{n}");
                                case StructDeclarationSyntax parent:
                                    return FindNewTypeName(parent, $"S{n}");
                                case InterfaceDeclarationSyntax parent:
                                    return FindNewTypeName(parent, $"I{n}");
                                case EnumDeclarationSyntax parent:
                                    return FindNewTypeName(parent, $"E{n}");
                                case VariableDeclaratorSyntax { Parent: FieldDeclarationSyntax { Parent: TypeDeclarationSyntax parent } }:
                                    return FindNewMemberName(parent, $"f{n}");
                                case EventDeclarationSyntax { Parent: TypeDeclarationSyntax parent }:
                                    return FindNewMemberName(parent, $"E{n}");
                                case EventFieldDeclarationSyntax { Parent: TypeDeclarationSyntax parent }:
                                    return FindNewMemberName(parent, $"E{n}");
                                case PropertyDeclarationSyntax { Parent: TypeDeclarationSyntax parent }:
                                    return FindNewMemberName(parent, $"P{n}");
                                case MethodDeclarationSyntax { Parent: TypeDeclarationSyntax parent }:
                                    return FindNewMemberName(parent, $"M{n}");
                                default:
                                    return null;
                            }
                        }

                        string? FindNewTypeName(BaseTypeDeclarationSyntax type, string name)
                        {
                            if (type is TypeDeclarationSyntax typeDeclaration)
                            {
                                foreach (var item in typeDeclaration.Members)
                                {
                                    if (item.TrySingleIdentifier(out var memberIdentifier) &&
                                        memberIdentifier.ValueText != name)
                                    {
                                        continue;
                                    }
                                    else
                                    {
                                        return null;
                                    }
                                }
                            }

                            foreach (var sibling in type.Parent.ChildNodes())
                            {
                                switch (sibling)
                                {
                                    case BaseTypeDeclarationSyntax { Identifier: { } identifier }:
                                        if (identifier.ValueText == name)
                                        {
                                            return null;
                                        }

                                        break;
                                    default:
                                        return null;
                                }
                            }

                            return name;
                        }

                        string? FindNewMemberName(TypeDeclarationSyntax type, string name)
                        {
                            if (type is TypeDeclarationSyntax typeDeclaration)
                            {
                                foreach (var item in typeDeclaration.Members)
                                {
                                    if (item.TrySingleIdentifier(out var memberIdentifier) &&
                                        memberIdentifier.ValueText != name)
                                    {
                                        continue;
                                    }
                                    else
                                    {
                                        return null;
                                    }
                                }
                            }

                            return name;
                        }
                    }
                }
            }
        }

        private class ReplaceRewriter : CSharpSyntaxRewriter
        {
            private static readonly ConcurrentQueue<ReplaceRewriter> Cache = new ConcurrentQueue<ReplaceRewriter>();

            private string? before;
            private string? after;

            public override SyntaxNode VisitLiteralExpression(LiteralExpressionSyntax node)
            {
                var pattern = $"[^\\w](?<before>{this.before})[^\\w]";
                if (node.IsKind(SyntaxKind.StringLiteralExpression) &&
                    Regex.IsMatch(node.Token.ValueText, pattern))
                {
                    return node.Update(
                        SyntaxFactory.Literal(
                           Regex.Replace(node.Token.Text, this.before, UpdateMatch),
                           Regex.Replace(node.Token.ValueText, this.before, UpdateMatch)));
                }

                return base.VisitLiteralExpression(node);

                string UpdateMatch(Match match)
                {
                    return match.Value.Replace(this.before!, this.after);
                }
            }

            internal static SyntaxNode Update(MethodDeclarationSyntax method, string before, string after)
            {
                if (!Cache.TryDequeue(out var rewriter))
                {
                    rewriter = new ReplaceRewriter();
                }

                rewriter.before = before;
                rewriter.after = after;
                var updated = rewriter.Visit(method);
                Cache.Enqueue(rewriter);
                return updated;
            }
        }
    }
}
