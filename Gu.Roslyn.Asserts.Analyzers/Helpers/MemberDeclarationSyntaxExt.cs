namespace Gu.Roslyn.Asserts.Analyzers
{
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp.Syntax;

    internal static class MemberDeclarationSyntaxExt
    {
        internal static bool TrySingleIdentifier(this MemberDeclarationSyntax member, out SyntaxToken identifer)
        {
            switch (member)
            {
                case BaseFieldDeclarationSyntax { Declaration: { Variables: { Count: 1 } variables } }:
                    identifer = variables[0].Identifier;
                    return true;
                case EventDeclarationSyntax { Identifier: { } temp }:
                    identifer = temp;
                    return true;
                case PropertyDeclarationSyntax { Identifier: { } temp }:
                    identifer = temp;
                    return true;
                case MethodDeclarationSyntax { Identifier: { } temp }:
                    identifer = temp;
                    return true;
                case BaseTypeDeclarationSyntax { Identifier: { } temp }:
                    identifer = temp;
                    return true;
                default:
                    identifer = default;
                    return true;
            }
        }
    }
}
