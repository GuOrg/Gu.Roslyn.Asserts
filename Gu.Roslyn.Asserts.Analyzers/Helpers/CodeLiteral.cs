﻿namespace Gu.Roslyn.Asserts.Analyzers;

using System;
using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;
using Gu.Roslyn.AnalyzerExtensions;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;

internal readonly struct CodeLiteral
{
    internal readonly ImmutableArray<SyntaxToken> Identifiers;
    private readonly LiteralExpressionSyntax stringLiteral;

    private CodeLiteral(ImmutableArray<SyntaxToken> identifiers, LiteralExpressionSyntax stringLiteral)
    {
        this.Identifiers = identifiers;
        this.stringLiteral = stringLiteral;
    }

    internal static bool TryCreate(LiteralExpressionSyntax stringLiteral, [NotNullWhen(true)] out CodeLiteral? code)
    {
        if (CSharpSyntaxTree.ParseText(stringLiteral.Token.ValueText.Trim('\"').Replace("↓", string.Empty)).TryGetRoot(out var node))
        {
            using var walker = IdentifierTokenWalker.Borrow(node);
            code = new CodeLiteral(walker.IdentifierTokens.ToImmutableArray(), stringLiteral);
            return true;
        }

        code = default;
        return false;
    }

    internal Location Location(SyntaxToken identifier)
    {
        return this.stringLiteral.SyntaxTree.GetLocation(this.Span(identifier));
    }

    internal bool TryFind(Location location, out SyntaxToken identifier)
    {
        foreach (var candidate in this.Identifiers)
        {
            if (location.SourceSpan == this.Span(candidate))
            {
                identifier = candidate;
                return true;
            }
        }

        identifier = default;
        return false;
    }

    private TextSpan Span(SyntaxToken identifier)
    {
        var text = this.stringLiteral.Token.Text;
        return new TextSpan(
            this.stringLiteral.SpanStart +
            text.IndexOf(identifier.ValueText, identifier.SpanStart, StringComparison.Ordinal),
            identifier.Text.Length);
    }
}
