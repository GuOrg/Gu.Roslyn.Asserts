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
            return tree.Find<EqualsValueClauseSyntax>(code);
        }

        /// <summary>
        /// Find a <see cref="AssignmentExpressionSyntax"/> that matches <paramref name="code"/>.
        /// </summary>
        public static AssignmentExpressionSyntax FindAssignmentExpression(this SyntaxTree tree, string code)
        {
            return tree.Find<AssignmentExpressionSyntax>(code);
        }

        /// <summary>
        /// Find a <see cref="InvocationExpressionSyntax"/> that matches <paramref name="signature"/>.
        /// </summary>
        public static InvocationExpressionSyntax FindInvocation(this SyntaxTree tree, string signature)
        {
            return tree.Find<InvocationExpressionSyntax>(signature);
        }

        /// <summary>
        /// Find a <see cref="ParameterSyntax"/> that matches <paramref name="signature"/>.
        /// </summary>
        public static ParameterSyntax FindParameter(this SyntaxTree tree, string signature)
        {
            return tree.Find<ParameterSyntax>(signature);
        }

        /// <summary>
        /// Find a <see cref="ArgumentSyntax"/> that matches <paramref name="signature"/>.
        /// </summary>
        public static ArgumentSyntax FindArgument(this SyntaxTree tree, string signature)
        {
            return tree.Find<ArgumentSyntax>(signature);
        }

        /// <summary>
        /// Find a <see cref="LiteralExpressionSyntax"/> that matches <paramref name="signature"/>.
        /// </summary>
        public static LiteralExpressionSyntax FindLiteralExpression(this SyntaxTree tree, string signature)
        {
            return tree.Find<LiteralExpressionSyntax>(signature);
        }

        /// <summary>
        /// Find a <see cref="StatementSyntax"/> that matches <paramref name="code"/>.
        /// </summary>
        public static StatementSyntax FindStatement(this SyntaxTree tree, string code)
        {
            return tree.Find<StatementSyntax>(code);
        }

        /// <summary>
        /// Find a <see cref="MethodDeclarationSyntax"/> that matches <paramref name="signature"/>.
        /// </summary>
        public static TypeDeclarationSyntax FindTypeDeclaration(this SyntaxTree tree, string signature)
        {
            return tree.Find<TypeDeclarationSyntax>(signature);
        }

        /// <summary>
        /// Find a <see cref="MethodDeclarationSyntax"/> that matches <paramref name="signature"/>.
        /// </summary>
        public static ClassDeclarationSyntax FindClassDeclaration(this SyntaxTree tree, string signature)
        {
            return tree.Find<ClassDeclarationSyntax>(signature);
        }

        /// <summary>
        /// Find a <see cref="MethodDeclarationSyntax"/> that matches <paramref name="signature"/>.
        /// </summary>
        public static StructDeclarationSyntax FindStructDeclaration(this SyntaxTree tree, string signature)
        {
            return tree.Find<StructDeclarationSyntax>(signature);
        }

        /// <summary>
        /// Find a <see cref="MethodDeclarationSyntax"/> that matches <paramref name="signature"/>.
        /// </summary>
        public static FieldDeclarationSyntax FindFieldDeclaration(this SyntaxTree tree, string signature)
        {
            return tree.Find<FieldDeclarationSyntax>(signature);
        }

        /// <summary>
        /// Find a <see cref="ConstructorDeclarationSyntax"/> that matches <paramref name="signature"/>.
        /// </summary>
        public static ConstructorDeclarationSyntax FindConstructorDeclaration(this SyntaxTree tree, string signature)
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
        /// Find a <see cref="ConstructorDeclarationSyntax"/> that matches <paramref name="signature"/>.
        /// </summary>
        [Obsolete("Use FindConstructorDeclaration")]
        public static ConstructorDeclarationSyntax FindConstructorDeclarationSyntax(this SyntaxTree tree, string signature)
        {
            return FindConstructorDeclaration(tree, signature);
        }

        /// <summary>
        /// Find a <see cref="EventFieldDeclarationSyntax"/> that matches <paramref name="signature"/>.
        /// </summary>
        public static EventFieldDeclarationSyntax FindEventFieldDeclaration(this SyntaxTree tree, string signature)
        {
            return tree.Find<EventFieldDeclarationSyntax>(signature);
        }

        /// <summary>
        /// Find a <see cref="EventDeclarationSyntax"/> that matches <paramref name="signature"/>.
        /// </summary>
        public static EventDeclarationSyntax FindEventDeclaration(this SyntaxTree tree, string signature)
        {
            return tree.Find<EventDeclarationSyntax>(signature);
        }

        /// <summary>
        /// Find a <see cref="BasePropertyDeclarationSyntax"/> that matches <paramref name="signature"/>.
        /// </summary>
        public static BasePropertyDeclarationSyntax FindBasePropertyDeclaration(this SyntaxTree tree, string signature)
        {
            return tree.Find<BasePropertyDeclarationSyntax>(signature);
        }

        /// <summary>
        /// Find a <see cref="PropertyDeclarationSyntax"/> that matches <paramref name="signature"/>.
        /// </summary>
        public static PropertyDeclarationSyntax FindPropertyDeclaration(this SyntaxTree tree, string signature)
        {
            return tree.Find<PropertyDeclarationSyntax>(signature);
        }

        /// <summary>
        /// Find a <see cref="IndexerDeclarationSyntax"/> that matches <paramref name="signature"/>.
        /// </summary>
        public static IndexerDeclarationSyntax FindIndexerDeclaration(this SyntaxTree tree, string signature)
        {
            return tree.Find<IndexerDeclarationSyntax>(signature);
        }

        /// <summary>
        /// Find a <see cref="AccessorDeclarationSyntax"/> that matches <paramref name="signature"/>.
        /// </summary>
        public static AccessorDeclarationSyntax FindAccessorDeclaration(this SyntaxTree tree, string signature)
        {
            return tree.Find<AccessorDeclarationSyntax>(signature);
        }

        /// <summary>
        /// Find a <see cref="VariableDeclarationSyntax"/> that matches <paramref name="signature"/>.
        /// </summary>
        public static VariableDeclarationSyntax FindVariableDeclaration(this SyntaxTree tree, string signature)
        {
            return tree.Find<VariableDeclarationSyntax>(signature);
        }

        /// <summary>
        /// Find a <see cref="ExpressionSyntax"/> that matches <paramref name="signature"/>.
        /// </summary>
        public static ExpressionSyntax FindExpression(this SyntaxTree tree, string signature)
        {
            return tree.Find<ExpressionSyntax>(signature);
        }

        /// <summary>
        /// Find a <see cref="MemberAccessExpressionSyntax"/> that matches <paramref name="signature"/>.
        /// </summary>
        public static MemberAccessExpressionSyntax FindMemberAccessExpression(this SyntaxTree tree, string signature)
        {
            return tree.Find<MemberAccessExpressionSyntax>(signature);
        }

        /// <summary>
        /// Find a <see cref="BinaryExpressionSyntax"/> that matches <paramref name="signature"/>.
        /// </summary>
        public static BinaryExpressionSyntax FindBinaryExpression(this SyntaxTree tree, string signature)
        {
            return tree.Find<BinaryExpressionSyntax>(signature);
        }

        /// <summary>
        /// Find a <see cref="ObjectCreationExpressionSyntax"/> that matches <paramref name="signature"/>.
        /// </summary>
        public static ObjectCreationExpressionSyntax FindObjectCreationExpression(this SyntaxTree tree, string signature)
        {
            return tree.Find<ObjectCreationExpressionSyntax>(signature);
        }

        /// <summary>
        /// Find a <see cref="MethodDeclarationSyntax"/> that matches <paramref name="signature"/>.
        /// </summary>
        public static MethodDeclarationSyntax FindMethodDeclaration(this SyntaxTree tree, string signature)
        {
            return tree.Find<MethodDeclarationSyntax>(signature);
        }

        /// <summary>
        /// Find a <see cref="AttributeSyntax"/> that matches <paramref name="signature"/>.
        /// </summary>
        public static AttributeSyntax FindAttribute(this SyntaxTree tree, string signature)
        {
            return tree.Find<AttributeSyntax>(signature);
        }

        /// <summary>
        /// Find a <see cref="AttributeArgumentSyntax"/> that matches <paramref name="signature"/>.
        /// </summary>
        public static AttributeArgumentSyntax FindAttributeArgument(this SyntaxTree tree, string signature)
        {
            return tree.Find<AttributeArgumentSyntax>(signature);
        }

        [Obsolete("Use Find<T>")]
        /// <summary>
        /// Find a <typeparamref name="T"/> that matches <paramref name="code"/>.
        /// </summary>
        /// <typeparam name="T">The type of the node to find.</typeparam>
        public static T FindBestMatch<T>(this SyntaxTree tree, string code)
            where T : SyntaxNode
        {
            return Find<T>(tree, code);
        }

        /// <summary>
        /// Find a <typeparamref name="T"/> that matches <paramref name="code"/>.
        /// </summary>
        /// <typeparam name="T">The type of the node to find.</typeparam>
        public static T Find<T>(this SyntaxTree tree, string code)
            where T : SyntaxNode
        {
            T best = null;
            if (tree.TryGetRoot(out var root))
            {
                best = FindBestMatchRecursive<T>(root, code);
            }

            return best ?? throw new InvalidOperationException($"The tree does not contain an {typeof(T).Name} matching the code.");
        }

        /// <summary>
        /// Find a <typeparamref name="T"/> that matches <paramref name="code"/>.
        /// </summary>
        /// <typeparam name="T">The type of the node to find.</typeparam>
        public static T Find<T>(this SyntaxNode root, string code)
            where T : SyntaxNode
        {
            return FindBestMatchRecursive<T>(root, code) ?? throw new InvalidOperationException($"The tree does not contain an {typeof(T).Name} matching the code.");
        }

        private static T FindBestMatchRecursive<T>(SyntaxNode root, string code)
            where T : SyntaxNode
        {
            foreach (var node in root.DescendantNodes()
                                     .OfType<T>())
            {
                var nodeText = node.ToString();
                if (nodeText == code)
                {
                    return node;
                }

                if (code.Contains(nodeText))
                {
                    var parent = node.Parent;
                    while (parent != null)
                    {
                        if (parent.ToString() == code)
                        {
                            return node;
                        }

                        parent = parent.Parent;
                    }
                }

                if (nodeText.Contains(code) ||
                    node.FirstAncestorOrSelf<StatementSyntax>()?.ToFullString().Contains(code) == true)
                {
                    return FindBestMatchRecursive<T>(node, code) ?? node;
                }
            }

            return null;
        }
    }
}
