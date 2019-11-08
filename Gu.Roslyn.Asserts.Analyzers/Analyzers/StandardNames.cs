namespace Gu.Roslyn.Asserts.Analyzers
{
    using System;
    using System.Collections.Generic;
    using System.Text.RegularExpressions;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using Microsoft.CodeAnalysis.Text;

    internal static class StandardNames
    {
        private static readonly string[] Words =
        {
            "Foo",
            "IFoo",
            "Bar",
            "IBar",
            "Baz",
            "IBaz",
            "Meh",
            "IMeh",
            "Lol",
            "ILol",
            "SomeClass",
            "SomeInterface",
            "ISomeInterface",
            "SomeField",
            "SomeEvent",
            "SomeProperty",
            "SomeMethod",
        };

        internal static IEnumerable<WordAndLocation> FindReplacements(StringArgument argument)
        {
            if (argument.Value is LiteralExpressionSyntax { Token: { ValueText: { } valueText } } literal &&
                literal.IsKind(SyntaxKind.StringLiteralExpression))
            {
                CodeLiteral? code = null;
                foreach (var word in Words)
                {
                    if (Regex.IsMatch(valueText, $"[^\\w]{word}[^\\w]", RegexOptions.IgnoreCase))
                    {
                        if (code is null)
                        {
                            if (!CodeLiteral.TryCreate(literal, out code))
                            {
                                yield break;
                            }
                        }

#pragma warning disable CS8629 // Nullable value type may be null. ReSharper disable once PossibleInvalidOperationException
                        foreach (SyntaxToken identifier in code.Value.Identifiers)
#pragma warning restore CS8629 // Nullable value type may be null.
                        {
                            if (string.Equals(identifier.ValueText, word, StringComparison.OrdinalIgnoreCase))
                            {
                                switch (identifier.Parent)
                                {
                                    case ClassDeclarationSyntax _ when word != "C":
                                        yield return new WordAndLocation(word, Location());
                                        break;
                                    case StructDeclarationSyntax _:
                                        yield return new WordAndLocation(word, Location());
                                        break;
                                    case InterfaceDeclarationSyntax _:
                                        yield return new WordAndLocation(word, Location());
                                        break;
                                    case EnumDeclarationSyntax _ when word != "E":
                                        yield return new WordAndLocation(word, Location());
                                        break;
                                    case VariableDeclaratorSyntax { Parent: FieldDeclarationSyntax _ }:
                                        yield return new WordAndLocation(word, Location());
                                        break;
                                    case EventDeclarationSyntax _:
                                    case EventFieldDeclarationSyntax _:
                                        yield return new WordAndLocation(word, Location());
                                        break;
                                    case PropertyDeclarationSyntax _:
                                        yield return new WordAndLocation(word, Location());
                                        break;
                                    case MethodDeclarationSyntax _:
                                        yield return new WordAndLocation(word, Location());
                                        break;
                                    case VariableDeclaratorSyntax { Parent: LocalDeclarationStatementSyntax _ }:
                                    case ParameterSyntax _:
                                        yield return new WordAndLocation(word, Location());
                                        break;
                                }

                                Location Location()
                                {
                                    return literal.SyntaxTree.GetLocation(
                                        new TextSpan(
                                            literal.SpanStart + literal.Token.Text.IndexOf('\"') + 1 +
                                            identifier.SpanStart,
                                            identifier.Text.Length));
                                }
                            }
                        }
                    }
                }
            }
        }
    }
}
