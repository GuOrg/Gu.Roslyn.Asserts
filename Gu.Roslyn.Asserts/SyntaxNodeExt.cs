namespace Gu.Roslyn.Asserts
{
    using System;
    using System.Linq;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp.Syntax;

    /// <summary>
    /// Helpers for finding things in SyntaxTrees
    /// </summary>
    public static class SyntaxNodeExt
    {
        /// <summary>
        /// Find a <see cref="EqualsValueClauseSyntax"/> that matches <paramref name="code"/>.
        /// </summary>
        public static EqualsValueClauseSyntax FindEqualsValueClause(this SyntaxTree tree, string code)
        {
            return tree.FindBestMatch<EqualsValueClauseSyntax>(code);
        }

        /// <summary>
        /// Find a <see cref="AssignmentExpressionSyntax"/> that matches <paramref name="code"/>.
        /// </summary>
        public static AssignmentExpressionSyntax FindAssignmentExpression(this SyntaxTree tree, string code)
        {
            return tree.FindBestMatch<AssignmentExpressionSyntax>(code);
        }

        /// <summary>
        /// Find a <see cref="StatementSyntax"/> that matches <paramref name="code"/>.
        /// </summary>
        public static StatementSyntax FindStatement(this SyntaxTree tree, string code)
        {
            return tree.FindBestMatch<StatementSyntax>(code);
        }

        /// <summary>
        /// Find a <see cref="MethodDeclarationSyntax"/> that matches <paramref name="signature"/>.
        /// </summary>
        public static TypeDeclarationSyntax FindTypeDeclaration(this SyntaxTree tree, string signature)
        {
            return tree.FindBestMatch<TypeDeclarationSyntax>(signature);
        }

        /// <summary>
        /// Find a <see cref="MethodDeclarationSyntax"/> that matches <paramref name="signature"/>.
        /// </summary>
        public static ClassDeclarationSyntax FindClassDeclaration(this SyntaxTree tree, string signature)
        {
            return tree.FindBestMatch<ClassDeclarationSyntax>(signature);
        }

        /// <summary>
        /// Find a <see cref="MethodDeclarationSyntax"/> that matches <paramref name="signature"/>.
        /// </summary>
        public static FieldDeclarationSyntax FindFieldDeclaration(this SyntaxTree tree, string signature)
        {
            return tree.FindBestMatch<FieldDeclarationSyntax>(signature);
        }

        /// <summary>
        /// Find a <see cref="PropertyDeclarationSyntax"/> that matches <paramref name="signature"/>.
        /// </summary>
        public static PropertyDeclarationSyntax FindPropertyDeclaration(this SyntaxTree tree, string signature)
        {
            return tree.FindBestMatch<PropertyDeclarationSyntax>(signature);
        }

        /// <summary>
        /// Find a <see cref="MethodDeclarationSyntax"/> that matches <paramref name="signature"/>.
        /// </summary>
        public static MethodDeclarationSyntax FindMethodDeclaration(this SyntaxTree tree, string signature)
        {
            return tree.FindBestMatch<MethodDeclarationSyntax>(signature);
        }

        /// <summary>
        /// Find a <see cref="ConstructorDeclarationSyntax"/> that matches <paramref name="signature"/>.
        /// </summary>
        public static ConstructorDeclarationSyntax FindConstructorDeclarationSyntax(this SyntaxTree tree, string signature)
        {
            foreach (var ctor in tree.GetRoot().DescendantNodes().OfType<ConstructorDeclarationSyntax>())
            {
                if (ctor.ToFullString().Contains(signature))
                {
                    return ctor;
                }
            }

            throw new InvalidOperationException($"The tree does not contain an {typeof(ConstructorDeclarationSyntax).Name} matching {signature}");
        }

        /// <summary>
        /// Find a <typeparamref name="T"/> that matches <paramref name="code"/>.
        /// </summary>
        /// <typeparam name="T">The type of the node to find.</typeparam>
        public static T FindBestMatch<T>(this SyntaxTree tree, string code)
            where T : SyntaxNode
        {
            SyntaxNode parent = null;
            T best = null;
            foreach (var node in tree.GetRoot()
                                     .DescendantNodes()
                                     .OfType<T>())
            {
                var statementSyntax = node.FirstAncestorOrSelf<StatementSyntax>();
                if (statementSyntax?.ToFullString().Contains(code) == true)
                {
                    if (parent == null || statementSyntax.Span.Length < parent.Span.Length)
                    {
                        parent = statementSyntax;
                        best = node;
                    }
                }

                var member = node.FirstAncestorOrSelf<MemberDeclarationSyntax>();
                if (member?.ToFullString().Contains(code) == true)
                {
                    if (parent == null || member.Span.Length < parent.Span.Length)
                    {
                        parent = member;
                        best = node;
                    }
                }
            }

            if (best == null)
            {
                throw new InvalidOperationException($"The tree does not contain an {typeof(T).Name} matching the code.");
            }

            return best;
        }
    }
}