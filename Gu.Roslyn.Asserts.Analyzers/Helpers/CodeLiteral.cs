namespace Gu.Roslyn.Asserts.Analyzers
{
    using System.Collections.Immutable;
    using System.Diagnostics.CodeAnalysis;
    using Gu.Roslyn.AnalyzerExtensions;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;

    internal struct CodeLiteral
    {
        internal readonly SyntaxNode Root;
        internal readonly ImmutableArray<SyntaxToken> Identifiers;

        private CodeLiteral(SyntaxNode root, ImmutableArray<SyntaxToken> identifiers)
        {
            this.Root = root;
            this.Identifiers = identifiers;
        }

        internal static bool TryCreate(LiteralExpressionSyntax stringLiteral, [NotNullWhen(true)] out CodeLiteral? code)
        {
            if (CSharpSyntaxTree.ParseText(stringLiteral.Token.ValueText.Trim('\"')).TryGetRoot(out var node))
            {
                using (var walker = IdentifierTokenWalker.Borrow(node))
                {
                    code = new CodeLiteral(node, walker.IdentifierTokens.ToImmutableArray());
                    return true;
                }
            }

            code = default;
            return false;
        }
    }
}
