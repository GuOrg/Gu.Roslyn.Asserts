namespace Gu.Roslyn.Asserts.Analyzers
{
    using System.Collections.Generic;
    using Gu.Roslyn.AnalyzerExtensions;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp.Syntax;

    internal sealed class InvocationWalker : PooledWalker<InvocationWalker>
    {
        private readonly List<InvocationExpressionSyntax> invocations = new List<InvocationExpressionSyntax>();

        private InvocationWalker()
        {
        }

        public override void VisitInvocationExpression(InvocationExpressionSyntax node)
        {
            if (node.Expression is MemberAccessExpressionSyntax memberAccess &&
                memberAccess.Expression is IdentifierNameSyntax identifierName &&
                identifierName.Identifier.ValueText == "RoslynAssert")
            {
                this.invocations.Add(node);
            }

            base.VisitInvocationExpression(node);
        }

        internal static bool TryFindName(SyntaxNode node, out string name)
        {
            name = null;
            using (var walker = BorrowAndVisit(node, () => new InvocationWalker()))
            {
                foreach (var invocation in walker.invocations)
                {
                    if (invocation.TryGetMethodName(out var candidate))
                    {
                        if (name == null)
                        {
                            name = candidate;
                        }
                        else if (name != candidate)
                        {
                            return false;
                        }
                    }
                }
            }

            return name != null;
        }

        internal static bool TryFindRoslynAssert(SyntaxNode node, out InvocationExpressionSyntax invocation)
        {
            using (var walker = BorrowAndVisit(node, () => new InvocationWalker()))
            {
                return walker.invocations.TrySingle(out invocation);
            }
        }

        /// <inheritdoc />
        protected override void Clear()
        {
            this.invocations.Clear();
        }
    }
}