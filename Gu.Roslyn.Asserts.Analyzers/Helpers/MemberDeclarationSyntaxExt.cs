namespace Gu.Roslyn.Asserts.Analyzers;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

internal static class MemberDeclarationSyntaxExt
{
    internal static bool TrySingleIdentifier(this MemberDeclarationSyntax member, out SyntaxToken identifier)
    {
        switch (member)
        {
            case BaseFieldDeclarationSyntax { Declaration.Variables: { Count: 1 } variables }:
                identifier = variables[0].Identifier;
                return true;
            case EventDeclarationSyntax { Identifier: { } temp }:
                identifier = temp;
                return true;
            case PropertyDeclarationSyntax { Identifier: { } temp }:
                identifier = temp;
                return true;
            case MethodDeclarationSyntax { Identifier: { } temp }:
                identifier = temp;
                return true;
            case BaseTypeDeclarationSyntax { Identifier: { } temp }:
                identifier = temp;
                return true;
            default:
                identifier = default;
                return true;
        }
    }
}
