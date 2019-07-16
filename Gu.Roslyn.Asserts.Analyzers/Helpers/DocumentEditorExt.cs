namespace Gu.Roslyn.Asserts.Analyzers
{
    using Gu.Roslyn.AnalyzerExtensions.StyleCopComparers;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using Microsoft.CodeAnalysis.Editing;

    internal static class DocumentEditorExt
    {
        internal static DocumentEditor AddPrivateStaticField(this DocumentEditor editor,
            TypeDeclarationSyntax typeDeclaration,
            ITypeSymbol type,
            string name)
        {
            editor.ReplaceNode(
                typeDeclaration,
                (node, generator) => AddSorted(
                    generator,
                    (TypeDeclarationSyntax)node,
                    (FieldDeclarationSyntax)generator.FieldDeclaration(
                        name,
                        editor.Generator.TypeExpression(type),
                        Accessibility.Private,
                        DeclarationModifiers.Static | DeclarationModifiers.ReadOnly,
                        editor.Generator.ObjectCreationExpression(type))));
            return editor;
        }

        private static TypeDeclarationSyntax AddSorted(
            SyntaxGenerator generator,
            TypeDeclarationSyntax containingType,
            FieldDeclarationSyntax field)
        {
            foreach (var member in containingType.Members)
            {
                if (member.IsEquivalentTo(field))
                {
                    return containingType;
                }

                if (MemberDeclarationComparer.Compare(field, member) < 0)
                {
                    return (TypeDeclarationSyntax)generator.InsertNodesBefore(containingType, member, new[] { field });
                }
            }

            return (TypeDeclarationSyntax)generator.AddMembers(containingType, field);
        }
    }
}
