namespace Gu.Roslyn.Asserts.Analyzers
{
    using System;
    using System.Collections.Generic;
    using System.Collections.Immutable;
    using System.Diagnostics;
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

                if (StringLiteralWalker.TryFindReplacement(methodDeclaration, out var before, out var location, out var after))
                {
                    if (after != null)
                    {
                        context.ReportDiagnostic(Diagnostic.Create(
                            Descriptors.GURA09UseStandardNames,
                            location,
                            ImmutableDictionary<string, string>.Empty.Add("before", before)
                                                                     .Add("after", after),
                            $"Use standard name {after} instead of {before}."));
                    }
                    else
                    {
                        context.ReportDiagnostic(Diagnostic.Create(
                            Descriptors.GURA09UseStandardNames,
                            location,
                            ImmutableDictionary<string, string>.Empty.Add("before", before),
                            $"Use standard name instead of {before}."));
                    }
                }
            }
        }

        private sealed class StringLiteralWalker : PooledWalker<StringLiteralWalker>
        {
            private static readonly ClassName[] ClassNames =
            {
                new ClassName("Foo", "C1"),
                new ClassName("Bar", "C2"),
                new ClassName("Baz", "C3"),
                new ClassName("Meh", "C4"),
            };

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

            internal static bool TryFindReplacement(SyntaxNode node, out string before, out Location location, out string after)
            {
                using (var walker = BorrowAndVisit(node, () => new StringLiteralWalker()))
                {
                    foreach (var literal in walker.literals)
                    {
                        foreach (var className in ClassNames)
                        {
                            if (className.TryFind(literal, walker.literals, out before, out location, out after))
                            {
                                return true;
                            }
                        }

                        var index = literal.Token.Text.IndexOf("Bar(", StringComparison.Ordinal);
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

            [DebuggerDisplay("{this.name}")]
            private class ClassName
            {
                private readonly string name;
                private readonly string pattern;
                private readonly string replacement;

                public ClassName(string name, string replacement)
                {
                    this.name = name;
                    this.pattern = "class " + name;
                    this.replacement = replacement;
                }

                public bool TryFind(LiteralExpressionSyntax literal, List<LiteralExpressionSyntax> literals, out string before, out Location location, out string after)
                {
                    if (TryIndexOf(literal, this.pattern, out var index))
                    {
                        before = this.name;
                        location = literal.SyntaxTree.GetLocation(new TextSpan(literal.SpanStart + index + 6, 3));
                        after = GetReplacement();
                        return true;

                        string GetReplacement()
                        {
                            foreach (var className in ClassNames)
                            {
                                if (className != this &&
                                    (AnyContains(className.pattern) ||
                                     AnyContains(className.replacement)))
                                {
                                    if (!AnyContains(this.replacement))
                                    {
                                        return this.replacement;
                                    }

                                    return null;
                                }
                            }

                            return "C";
                        }

                        bool AnyContains(string text)
                        {
                            foreach (var candidate in literals)
                            {
                                if (candidate.Token.ValueText.Contains(text))
                                {
                                    return true;
                                }
                            }

                            return false;
                        }
                    }

                    before = null;
                    location = null;
                    after = null;
                    return false;
                }

                private static bool TryIndexOf(LiteralExpressionSyntax literal, string text, out int index)
                {
                    index = literal.Token.Text.IndexOf(text, StringComparison.Ordinal);
                    return index >= 0;
                }
            }
        }
    }
}
