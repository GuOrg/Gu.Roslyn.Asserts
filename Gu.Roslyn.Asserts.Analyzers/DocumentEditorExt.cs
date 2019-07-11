namespace Gu.Roslyn.Asserts.Analyzers
{
    using System.Collections.Immutable;
    using System.Composition;
    using System.Threading;
    using System.Threading.Tasks;
    using Gu.Roslyn.AnalyzerExtensions;
    using Gu.Roslyn.CodeFixExtensions;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CodeFixes;
    using Microsoft.CodeAnalysis.CSharp;
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
