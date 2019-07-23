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

                using (var walker = StringLiteralWalker.BorrowAndVisit(methodDeclaration))
                {
                    while (walker.TryFindReplacement(out var before, out var location, out var after))
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

            private static readonly MethodName[] MethodNames =
            {
                new MethodName("Foo"),
                new MethodName("Bar"),
                new MethodName("Baz"),
                new MethodName("Meh"),
                new MethodName("Lol"),
            };

            private readonly List<LiteralExpressionSyntax> literals = new List<LiteralExpressionSyntax>();
            private readonly HashSet<TextSpan> locations = new HashSet<TextSpan>();

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

            internal static StringLiteralWalker BorrowAndVisit(SyntaxNode node) => BorrowAndVisit(node, () => new StringLiteralWalker());

            internal bool TryFindReplacement(out string before, out Location location, out string after)
            {
                foreach (var literal in this.literals)
                {
                    foreach (var className in ClassNames)
                    {
                        if (className.TryFind(literal, this.literals, out before, out location, out after) &&
                            this.locations.Add(location.SourceSpan))
                        {
                            return true;
                        }
                    }

                    foreach (var className in MethodNames)
                    {
                        if (className.TryFind(literal, out before, out location, out after) &&
                            this.locations.Add(location.SourceSpan))
                        {
                            return true;
                        }
                    }
                }

                before = null;
                location = null;
                after = null;
                return false;
            }

            protected override void Clear()
            {
                this.literals.Clear();
                this.locations.Clear();
            }

            private static bool TryIndexOf(LiteralExpressionSyntax literal, string text, out int index)
            {
                index = literal.Token.Text.IndexOf(text, StringComparison.Ordinal);
                return index >= 0;
            }

            private static bool TryIndexOf(LiteralExpressionSyntax literal, string text, int startIndex, out int index)
            {
                index = literal.Token.Text.IndexOf(text, startIndex, StringComparison.Ordinal);
                return index >= 0;
            }

            private static bool HasMemberNamed(LiteralExpressionSyntax literal, string name)
            {
                return PropertyName.TryFind(literal, name) ||
                       MethodName.TryFind(literal, name, out _);
            }

            [DebuggerDisplay("{this.pattern}")]
            private class ClassName
            {
                private readonly string pattern;
                private readonly string replacement;

                public ClassName(string name, string replacement)
                {
                    this.pattern = "class " + name;
                    this.replacement = replacement;
                }

                public bool TryFind(LiteralExpressionSyntax literal, List<LiteralExpressionSyntax> literals, out string before, out Location location, out string after)
                {
                    if (TryIndexOf(literal, this.pattern, out var index))
                    {
                        var start = index + 6;
                        before = literal.Token.Text.Substring(start, literal.Token.Text.IndexOfAny(new[] { ' ', '<', '\r', '\n', }, start) - start);
                        location = literal.SyntaxTree.GetLocation(new TextSpan(literal.SpanStart + start, before.Length));
                        after = GetReplacement();
                        if (after != null)
                        {
                            if (HasMemberNamed(literal, after))
                            {
                                after = null;
                            }
                        }

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
            }

            private class PropertyName
            {
                internal static bool TryFind(LiteralExpressionSyntax literal, string name)
                {
                    var index = -1;
                    while (TryIndexOf(literal, name, index + 1, out index))
                    {
                        var text = literal.Token.Text;
                        if (text.TryElementAt(index - 1, out var c) &&
                            c != ' ')
                        {
                            index += name.Length;
                            continue;
                        }

                        index += name.Length;
                        if (IsProperty())
                        {
                            return true;
                        }

                        bool IsProperty()
                        {
                            while (text.TryElementAt(index, out c))
                            {
                                switch (c)
                                {
                                    case ' ':
                                    case '\r':
                                    case '\n':
                                        index++;
                                        break;
                                    case '{':
                                        return true;
                                    case '=':
                                        return text.TryElementAt(index + 1, out var c1) &&
                                               c1 == '>';
                                    default:
                                        return false;
                                }
                            }

                            return false;
                        }
                    }

                    return false;
                }
            }

            private class MethodName
            {
                private readonly string name;

                public MethodName(string name)
                {
                    this.name = name;
                }

                public bool TryFind(LiteralExpressionSyntax literal, out string before, out Location location, out string after)
                {
                    if (TryFind(literal, this.name, out var index))
                    {
                        var start = index;
                        before = literal.Token.Text.Substring(start, literal.Token.Text.IndexOfAny(new[] { ' ', '<', '\r', '\n', '(' }, start) - start);
                        location = literal.SyntaxTree.GetLocation(new TextSpan(literal.SpanStart + start, before.Length));
                        after = "M";
                        if (after != null)
                        {
                            if (HasMemberNamed(literal, after))
                            {
                                after = null;
                            }
                        }

                        return true;
                    }

                    before = null;
                    location = null;
                    after = null;
                    return false;
                }

                internal static bool TryFind(LiteralExpressionSyntax literal, string name, out int index)
                {
                    index = -1;
                    while (TryIndexOf(literal, name, index + 1, out index))
                    {
                        var text = literal.Token.Text;
                        if (text.TryElementAt(index - 1, out var c) &&
                            c != ' ')
                        {
                            index += name.Length;
                            continue;
                        }

                        if (IsMethod(index + name.Length))
                        {
                            return true;
                        }

                        index += name.Length;
                        bool IsMethod(int position)
                        {
                            while (text.TryElementAt(position, out c))
                            {
                                switch (c)
                                {
                                    case ' ':
                                    case '\r':
                                    case '\n':
                                        position++;
                                        break;
                                    case '(':
                                        return true;
                                    default:
                                        return false;
                                }
                            }

                            return false;
                        }
                    }

                    return false;
                }
            }
        }
    }
}
