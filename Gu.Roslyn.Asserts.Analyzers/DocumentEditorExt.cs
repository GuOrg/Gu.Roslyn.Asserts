namespace Gu.Roslyn.Asserts.Analyzers
{
    using Gu.Roslyn.CodeFixExtensions;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using Microsoft.CodeAnalysis.Editing;

    internal static class DocumentEditorExt
    {
        internal static DocumentEditor AddPrivateStaticField(this DocumentEditor editor, TypeDeclarationSyntax typeDeclaration, ITypeSymbol type, string name)
        {
            return editor.AddField(
                typeDeclaration,
                (FieldDeclarationSyntax)editor.Generator.FieldDeclaration(
                    name,
                    (TypeSyntax)editor.Generator.TypeExpression(type),
                    Accessibility.Private,
                    DeclarationModifiers.Static |
                    DeclarationModifiers.ReadOnly,
                    editor.Generator.ObjectCreationExpression(type)));
        }
    }
}
