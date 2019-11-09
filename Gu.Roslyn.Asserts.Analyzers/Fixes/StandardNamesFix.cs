namespace Gu.Roslyn.Asserts.Analyzers
{
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Collections.Immutable;
    using System.Composition;
    using System.Diagnostics;
    using System.Linq;
    using System.Text.RegularExpressions;
    using System.Threading.Tasks;
    using Gu.Roslyn.AnalyzerExtensions;
    using Gu.Roslyn.CodeFixExtensions;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CodeFixes;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;

    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(StandardNamesFix))]
    [Shared]
    internal class StandardNamesFix : DocumentEditorCodeFixProvider
    {
        public override ImmutableArray<string> FixableDiagnosticIds { get; } = ImmutableArray.Create(
            Descriptors.GURA09UseStandardNames.Id);

        protected override async Task RegisterCodeFixesAsync(DocumentEditorCodeFixContext context)
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
                                        $"Replace {before} with {newName}",
                                        (e, _) => e.ReplaceNode(
                                            containingMethod,
                                            x => ReplaceRewriter.Update(x, before, newName)),
                                        $"Replace {before} with {newName}",
                                        diagnostic);
                            }
                        }

                        string? NewName(int? n)
                        {
                            return identifier.Parent switch
                            {
                                ClassDeclarationSyntax parent => FindNewTypeName(parent, $"C{n}"),
                                StructDeclarationSyntax parent => FindNewTypeName(parent, $"S{n}"),
                                InterfaceDeclarationSyntax parent => FindNewTypeName(parent, $"I{n}"),
                                EnumDeclarationSyntax parent => FindNewTypeName(parent, $"E{n}"),
                                VariableDeclaratorSyntax { Parent: FieldDeclarationSyntax { Parent: TypeDeclarationSyntax parent } } => FindNewMemberName(parent, $"f{n}"),
                                EventDeclarationSyntax { Parent: TypeDeclarationSyntax parent } => FindNewMemberName(parent, $"E{n}"),
                                EventFieldDeclarationSyntax { Parent: TypeDeclarationSyntax parent } => FindNewMemberName(parent, $"E{n}"),
                                PropertyDeclarationSyntax { Parent: TypeDeclarationSyntax parent } => FindNewMemberName(parent, $"P{n}"),
                                MethodDeclarationSyntax { Parent: TypeDeclarationSyntax parent } => FindNewMemberName(parent, $"M{n}"),
                                _ => null,
                            };
                        }

                        static string? FindNewTypeName(BaseTypeDeclarationSyntax type, string name)
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

                        static string? FindNewMemberName(TypeDeclarationSyntax type, string name)
                        {
                            if (type is { } typeDeclaration)
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
            private readonly List<Replacement> replacements = new List<Replacement>();

            public override SyntaxNode VisitLiteralExpression(LiteralExpressionSyntax node)
            {
                if (node.IsKind(SyntaxKind.StringLiteralExpression) &&
                    this.replacements.Any(x => Regex.IsMatch(node.Token.ValueText, $"[^\\w](?<before>{x.Before})[^\\w]")))
                {
                    return node.Update(
                        SyntaxFactory.Literal(
                            ReplaceAll(node.Token.Text),
                            ReplaceAll(node.Token.ValueText)));

                    string ReplaceAll(string text)
                    {
                        foreach (Replacement replacement in this.replacements)
                        {
                            text = Regex.Replace(text, replacement.Before, x => replacement.Replace(x));
                        }

                        return text;
                    }
                }

                return base.VisitLiteralExpression(node);
            }

            internal static SyntaxNode Update(MethodDeclarationSyntax method, string before, string after)
            {
                if (!Cache.TryDequeue(out var rewriter))
                {
                    rewriter = new ReplaceRewriter();
                }

                rewriter.replacements.Add(new Replacement(before, after));
                if (char.IsUpper(before[0]) &&
                    char.IsUpper(after[0]))
                {
                    rewriter.replacements.Add(new Replacement(FirstCharLower(before), FirstCharLower(after)));

                    string FirstCharLower(string s) => s.Substring(0, 1).ToLower() + s.Substring(1);
                }

                if (char.IsLower(before[0]) &&
                    char.IsLower(after[0]))
                {
                    rewriter.replacements.Add(new Replacement(FirstCharUpper(before), FirstCharUpper(after)));

                    string FirstCharUpper(string s) => s.Substring(0, 1).ToUpper() + s.Substring(1);
                }

                var updated = rewriter.Visit(method);
                rewriter.replacements.Clear();
                Cache.Enqueue(rewriter);
                return updated;
            }

            [DebuggerDisplay("Before: {Before} After: {After}")]
            private struct Replacement
            {
                internal readonly string Before;
                internal readonly string After;

                internal Replacement(string before, string after)
                {
                    this.Before = before;
                    this.After = after;
                }

                internal string Replace(Match match)
                {
                    return match.Value.Replace(this.Before, this.After);
                }
            }
        }
    }
}
