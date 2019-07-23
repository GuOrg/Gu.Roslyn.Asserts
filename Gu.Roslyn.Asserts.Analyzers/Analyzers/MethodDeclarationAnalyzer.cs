namespace Gu.Roslyn.Asserts.Analyzers
{
    using System;
    using System.Collections.Generic;
    using System.Collections.Immutable;
    using Gu.Roslyn.AnalyzerExtensions;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using Microsoft.CodeAnalysis.Diagnostics;
    using Microsoft.CodeAnalysis.Text;

    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class MethodDeclarationAnalyzer : DiagnosticAnalyzer
    {
        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get; } = ImmutableArray.Create(
            Descriptors.GURA06TestShouldBeInCorrectClass,
            Descriptors.GURA09UseStandardNames);

        public override void Initialize(AnalysisContext context)
        {
            context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
            context.EnableConcurrentExecution();
            context.RegisterSyntaxNodeAction(c => Handle(c), SyntaxKind.MethodDeclaration);
        }

        private static void Handle(SyntaxNodeAnalysisContext context)
        {
            if (context.Node is MethodDeclarationSyntax methodDeclaration &&
                context.ContainingSymbol is IMethodSymbol method &&
                InvocationWalker.TryFindRoslynAssert(methodDeclaration, out var invocation))
            {
                if (invocation.TryGetMethodName(out var name) &&
                    name != method.ContainingType.Name)
                {
                    context.ReportDiagnostic(Diagnostic.Create(
                        Descriptors.GURA06TestShouldBeInCorrectClass,
                        methodDeclaration.Identifier.GetLocation(),
                        ImmutableDictionary<string, string>.Empty.Add(nameof(IdentifierNameSyntax), name),
                        name));
                }

                if (StringLiteralWalker.TryFindReplace(methodDeclaration, out var before, out var location, out var after))
                {
                    context.ReportDiagnostic(Diagnostic.Create(
                        Descriptors.GURA09UseStandardNames,
                        location,
                        ImmutableDictionary<string, string>.Empty.Add("before", before).Add("after", after),
                        after,
                        before));
                }
            }
        }

        private sealed class StringLiteralWalker : PooledWalker<StringLiteralWalker>
        {
            private readonly List<LiteralExpressionSyntax> literals = new List<LiteralExpressionSyntax>();

            private StringLiteralWalker()
            {
            }

            public override void VisitLiteralExpression(LiteralExpressionSyntax node)
            {
                if (node.IsKind(SyntaxKind.StringLiteralExpression))
                {
                    this.literals.Add(node);
                }

                base.VisitLiteralExpression(node);
            }

            internal static bool TryFindReplace(SyntaxNode node, out string before, out Location location, out string after)
            {
                using (var walker = BorrowAndVisit(node, () => new StringLiteralWalker()))
                {
                    foreach (var literal in walker.literals)
                    {
                        var index = literal.Token.Text.IndexOf("class Foo", StringComparison.Ordinal);
                        if (index > 0)
                        {
                            before = "Foo";
                            location = literal.SyntaxTree.GetLocation(new TextSpan(literal.SpanStart + index + 6, 3));
                            if (walker.AnyContains("class Bar") || walker.AnyContains("class C"))
                            {
                                if (!walker.AnyContains("C1"))
                                {
                                    after = "C1";
                                    return true;
                                }
                            }
                            else
                            {
                                after = "C";
                                return true;
                            }
                        }

                        index = literal.Token.Text.IndexOf("class Bar", StringComparison.Ordinal);
                        if (index > 0)
                        {
                            before = "Bar";
                            location = literal.SyntaxTree.GetLocation(new TextSpan(literal.SpanStart + index + 6, 3));
                            if (walker.AnyContains("class Foo") || walker.AnyContains("class C"))
                            {
                                if (!walker.AnyContains("C2"))
                                {
                                    after = "C2";
                                    return true;
                                }
                            }
                            else
                            {
                                after = "C";
                                return true;
                            }
                        }

                        index = literal.Token.Text.IndexOf("Bar(", StringComparison.Ordinal);
                        if (index > 0)
                        {
                            before = "Bar";
                            location = literal.SyntaxTree.GetLocation(new TextSpan(literal.SpanStart + index, 3));
                            after = "M";
                            return true;
                        }
                    }

                    before = null;
                    location = null;
                    after = null;
                    return false;
                }
            }

            protected override void Clear()
            {
                this.literals.Clear();
            }

            private bool AnyContains(string text)
            {
                foreach (var literal in this.literals)
                {
                    if (literal.Token.ValueText.Contains(text))
                    {
                        return true;
                    }
                }

                return false;
            }
        }
    }
}
