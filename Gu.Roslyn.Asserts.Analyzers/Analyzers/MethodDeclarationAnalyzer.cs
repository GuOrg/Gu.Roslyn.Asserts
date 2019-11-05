namespace Gu.Roslyn.Asserts.Analyzers
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Collections.Immutable;
    using System.Diagnostics.CodeAnalysis;
    using Gu.Roslyn.AnalyzerExtensions;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using Microsoft.CodeAnalysis.Diagnostics;
    using Microsoft.CodeAnalysis.Text;

    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    internal class MethodDeclarationAnalyzer : DiagnosticAnalyzer
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
                    name != method.ContainingType.Name &&
                    name != method.ContainingType.ContainingType?.Name)
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
            private static readonly string[] Words =
            {
                "Foo",
                "IFoo",
                "Bar",
                "IBar",
                "Baz",
                "IBaz",
                "Meh",
                "IMeh",
                "Lol",
                "ILol",
                "SomeClass",
                "SomeInterface",
                "ISomeInterface",
                "SomeField",
                "SomeEvent",
                "SomeProperty",
                "SomeMethod",
            };

            private static readonly string[] PropertyWords =
            {
                "A",
                "B",
                "C",
                "D",
                "E",
                "F",
                "G",
            };

            private readonly List<LiteralExpressionSyntax> literals = new List<LiteralExpressionSyntax>();
            private readonly ConcurrentDictionary<LiteralExpressionSyntax, CompilationUnitSyntax?> roots = new ConcurrentDictionary<LiteralExpressionSyntax, CompilationUnitSyntax?>();
            private readonly HashSet<SyntaxToken> locations = new HashSet<SyntaxToken>();

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

            internal bool TryFindReplacement([NotNullWhen(true)]out string? before, [NotNullWhen(true)]out Location? location, [NotNullWhen(true)]out string? after)
            {
                foreach (var literal in this.literals)
                {
                    foreach (var word in Words)
                    {
                        var index = -1;
                        while (this.TryFindIdentifier(literal, word, index + 1, StringComparison.OrdinalIgnoreCase, out index, out var token))
                        {
                            if (token.IsKind(SyntaxKind.IdentifierToken) &&
                                this.locations.Add(token) &&
                                ShouldWarn(token))
                            {
                                before = token.ValueText;
                                location = literal.SyntaxTree.GetLocation(new TextSpan(literal.SpanStart + index, token.ValueText.Length));
                                after = this.Replace(token);
                                return true;
                            }
                        }
                    }

                    foreach (var word in PropertyWords)
                    {
                        var index = -1;
                        while (this.TryFindIdentifier(literal, word, index + 1, StringComparison.Ordinal, out index, out var token))
                        {
                            if (token.Parent.IsKind(SyntaxKind.PropertyDeclaration) &&
                                token.ValueText == word &&
                                token.IsKind(SyntaxKind.IdentifierToken) &&
                                this.locations.Add(token) &&
                                token.Parent.IsKind(SyntaxKind.PropertyDeclaration))
                            {
                                before = token.ValueText;
                                location = literal.SyntaxTree.GetLocation(new TextSpan(literal.SpanStart + index, token.ValueText.Length));
                                after = this.Replace(token);
                                return true;
                            }
                        }
                    }
                }

                before = null;
                location = null;
                after = null;
                return false;

                static bool ShouldWarn(SyntaxToken candidate)
                {
                    switch (candidate.Parent.Kind())
                    {
                        case SyntaxKind.StringLiteralExpression:
                        case SyntaxKind.IdentifierName:
                            return false;
                        default:
                            return true;
                    }
                }
            }

            protected override void Clear()
            {
                this.literals.Clear();
                this.roots.Clear();
                this.locations.Clear();
            }

            private string? Replace(SyntaxToken token)
            {
                return token.Parent switch
                {
                    EnumDeclarationSyntax declaration => token.ValueText switch
                    {
                        "Foo" => this.ReplaceTypeName(new Names("E", "E1"), declaration),
                        "Bar" => this.ReplaceTypeName(new Names("E", "E2"), declaration),
                        "Baz" => this.ReplaceTypeName(new Names("E", "E3"), declaration),
                        _ => this.ReplaceTypeName(new Names("E", null), declaration),
                    },

                    ClassDeclarationSyntax declaration => token.ValueText switch
                    {
                        "Foo" => this.ReplaceTypeName(new Names("C", "C1"), declaration),
                        "Bar" => this.ReplaceTypeName(new Names("C", "C2"), declaration),
                        "Baz" => this.ReplaceTypeName(new Names("C", "C3"), declaration),
                        _ => this.ReplaceTypeName(new Names("C", null), declaration),
                    },

                    StructDeclarationSyntax declaration => token.ValueText switch
                    {
                        "Foo" => this.ReplaceTypeName(new Names("S", "S1"), declaration),
                        "Bar" => this.ReplaceTypeName(new Names("S", "S2"), declaration),
                        "Baz" => this.ReplaceTypeName(new Names("S", "S3"), declaration),
                        _ => this.ReplaceTypeName(new Names("S", null), declaration),
                    },

                    InterfaceDeclarationSyntax declaration => token.ValueText switch
                    {
                        "IFoo" => this.ReplaceTypeName(new Names("I", "I1"), declaration),
                        "IBar" => this.ReplaceTypeName(new Names("I", "I2"), declaration),
                        "IBaz" => this.ReplaceTypeName(new Names("I", "I3"), declaration),
                        _ => this.ReplaceTypeName(new Names("I", null), declaration),
                    },

                    FieldDeclarationSyntax declaration => this.ReplaceMemberName("F", declaration),
                    EventDeclarationSyntax declaration => this.ReplaceMemberName("E", declaration),
                    EventFieldDeclarationSyntax declaration => this.ReplaceMemberName("E", declaration),
                    PropertyDeclarationSyntax declaration => this.ReplaceMemberName("P", declaration),
                    MethodDeclarationSyntax declaration => this.ReplaceMemberName("M", declaration),

                    _ => null,
                };
            }

            private string? ReplaceTypeName(Names candidateNames, BaseTypeDeclarationSyntax declaration)
            {
                if (this.literals.TrySingle(x => this.TryGetRoot(x, out _), out var single))
                {
                    var index = -1;
                    while (this.TryFindIdentifier(single, candidateNames.WhenSingle, index + 1, StringComparison.Ordinal, out index, out var candidateToken))
                    {
                        switch (candidateToken.Parent)
                        {
                            case BaseTypeDeclarationSyntax member when declaration.Contains(member):
                                return null;
                            case MemberDeclarationSyntax member when declaration.Contains(member):
                                return null;
                        }
                    }

                    return candidateNames.WhenSingle;
                }

                if (candidateNames.Else is { } name)
                {
                    foreach (var candidateLiteral in this.literals)
                    {
                        var index = -1;
                        while (this.TryFindIdentifier(candidateLiteral, name, index + 1, StringComparison.Ordinal, out index, out var candidateToken))
                        {
                            switch (candidateToken.Parent)
                            {
                                case BaseTypeDeclarationSyntax member when member != declaration:
                                    return null;
                                case MemberDeclarationSyntax member when declaration.Contains(member):
                                    return null;
                            }
                        }
                    }
                }

                return candidateNames.Else;
            }

            private string? ReplaceMemberName(string name, MemberDeclarationSyntax declaration)
            {
                switch (declaration.Parent)
                {
                    case TypeDeclarationSyntax typeDeclaration:
                        {
                            if (OnlyOverloads() ||
                                OnlyOneOfKind())
                            {
                                return name;
                            }

                            var i = 1;
                            while (typeDeclaration.Members.TryFirst(x => IsCollision(x), out _))
                            {
                                i++;
                                if (i > 100)
                                {
                                    return null;
                                }
                            }

                            return $"{name}{i}";

                            bool OnlyOverloads()
                            {
                                if (declaration is MethodDeclarationSyntax methodDeclaration)
                                {
                                    foreach (var member in typeDeclaration.Members)
                                    {
                                        if (member is MethodDeclarationSyntax { Identifier: { ValueText: { } valueText } } &&
                                            valueText != methodDeclaration.Identifier.ValueText)
                                        {
                                            return false;
                                        }
                                    }

                                    return true;
                                }

                                return false;
                            }

                            bool OnlyOneOfKind()
                            {
                                foreach (var member in typeDeclaration.Members)
                                {
                                    if (member.Kind() == declaration.Kind() &&
                                        member != declaration)
                                    {
                                        return false;
                                    }
                                }

                                return true;
                            }

                            bool IsCollision(MemberDeclarationSyntax candidate)
                            {
                                switch (candidate)
                                {
                                    case MethodDeclarationSyntax candidateDeclaration:
                                        if (declaration is MethodDeclarationSyntax method)
                                        {
                                            if (candidateDeclaration.Identifier.ValueText.IsParts(name, i.ToString()))
                                            {
                                                if (method.ParameterList.Parameters.Count ==
                                                    candidateDeclaration.ParameterList.Parameters.Count)
                                                {
                                                    for (var j = 0; j < method.ParameterList.Parameters.Count; j++)
                                                    {
                                                        if (!method.ParameterList.Parameters[j].Type.IsEquivalentTo(candidateDeclaration.ParameterList.Parameters[j].Type))
                                                        {
                                                            return false;
                                                        }
                                                    }

                                                    return true;
                                                }
                                            }

                                            return false;
                                        }

                                        return candidateDeclaration.Identifier.ValueText.IsParts(name, i.ToString());
                                    case TypeDeclarationSyntax candidateDeclaration:
                                        return candidateDeclaration.Identifier.ValueText.IsParts(name, i.ToString());
                                    case EnumDeclarationSyntax candidateDeclaration:
                                        return candidateDeclaration.Identifier.ValueText.IsParts(name, i.ToString());
                                    case PropertyDeclarationSyntax candidateDeclaration:
                                        return candidateDeclaration.Identifier.ValueText.IsParts(name, i.ToString());
                                    case EventDeclarationSyntax candidateDeclaration:
                                        return candidateDeclaration.Identifier.ValueText.IsParts(name, i.ToString());
                                    case BaseFieldDeclarationSyntax candidateDeclaration:
                                        return candidateDeclaration.Declaration.Variables.TrySingle(x => x.Identifier.ValueText.IsParts(name, i.ToString()), out _);
                                    case ConstructorDeclarationSyntax _:
                                        return false;
                                    default:
                                        return true;
                                }
                            }
                        }

                    case EnumDeclarationSyntax enumDeclaration:
                        {
                            if (enumDeclaration.Members.TrySingle(out _))
                            {
                                return name;
                            }

                            var i = 1;
                            while (enumDeclaration.Members.TryFirst(x => x.Identifier.ValueText.IsParts(name, i.ToString()), out _))
                            {
                                i++;
                            }

                            return $"{name}{i}";
                        }

                    case BaseTypeDeclarationSyntax _:
                        return name;
                    default:
                        return null;
                }
            }

            private bool TryGetRoot(LiteralExpressionSyntax literal, [NotNullWhen(true)]out CompilationUnitSyntax? root)
            {
                root = this.roots.GetOrAdd(
                    literal,
                    x =>
                    {
                        if (CSharpSyntaxTree.ParseText(literal.Token.ValueText).TryGetRoot(out var node))
                        {
                            return node as CompilationUnitSyntax;
                        }

                        return null;
                    });

                return root != null;
            }

            private bool TryFindIdentifier(LiteralExpressionSyntax literal, string word, int startIndex, StringComparison stringComparison, out int index, out SyntaxToken token)
            {
                index = literal.Token.Text.IndexOf(word, startIndex, stringComparison);
                if (index >= 0 &&
                    this.TryGetRoot(literal, out var root))
                {
                    var offset = literal.Token.Text.IndexOf("\"", StringComparison.Ordinal);
                    if (offset < 0)
                    {
                        token = default;
                        return false;
                    }

                    int position = index - offset - 1;
                    if (root.FullSpan.Contains(position))
                    {
                        token = root.FindToken(position);
                        return token.IsKind(SyntaxKind.IdentifierToken);
                    }
                }

                token = default;
                return false;
            }

            private struct Names
            {
                internal readonly string WhenSingle;
                internal readonly string? Else;

                internal Names(string whenSingle, string? @else)
                {
                    this.WhenSingle = whenSingle;
                    this.Else = @else;
                }
            }
        }
    }
}
