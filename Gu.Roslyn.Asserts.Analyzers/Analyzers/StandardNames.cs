namespace Gu.Roslyn.Asserts.Analyzers;

using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

internal static class StandardNames
{
    internal const string PrefixPattern = @"(^|[^a-z,A-Z])";
    internal const string SuffixPattern = @"([^0-9,a-z,A-Z]|$)";
    private static readonly string[] Words =
    {
        "Foo",
        "Foo1",
        "Foo2",
        "Foo3",
        "IFoo",
        "Bar",
        "Bar1",
        "Bar2",
        "Bar3",
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
        "ViewModel",
        "Value1",
        "Value2",
    };

    internal static IEnumerable<WordAndLocation> FindReplacements(StringArgument argument)
    {
        if (argument.StringLiteral is { Token.ValueText: { } valueText } literal &&
            literal.IsKind(SyntaxKind.StringLiteralExpression))
        {
            CodeLiteral? code = null;
            foreach (var word in Words)
            {
                if (Regex.IsMatch(valueText, $"{PrefixPattern}{word}{SuffixPattern}", RegexOptions.IgnoreCase))
                {
                    if (code is null)
                    {
                        if (!CodeLiteral.TryCreate(literal, out code))
                        {
                            yield break;
                        }
                    }

                    foreach (var identifier in code.Value.Identifiers)
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
                                case IdentifierNameSyntax { Parent: IncompleteMemberSyntax _ }:
                                case ParameterSyntax _:
                                    yield return new WordAndLocation(word, Location());
                                    break;
                            }

                            Location Location()
                            {
                                return code!.Value.Location(identifier);
                            }
                        }
                    }
                }
            }
        }
    }
}
