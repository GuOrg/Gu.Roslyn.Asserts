namespace Gu.Roslyn.Asserts.Analyzers
{
    using System;
    using System.Collections.Immutable;
    using System.Diagnostics;
    using System.Diagnostics.CodeAnalysis;
    using System.Threading;
    using Gu.Roslyn.AnalyzerExtensions;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;

    [DebuggerDisplay("{Expression}")]
    internal struct StringArgument : IEquatable<StringArgument>
    {
#pragma warning disable RS1008 // Avoid storing per-compilation data into the fields of a diagnostic analyzer.
        internal readonly ExpressionSyntax Expression;
        internal readonly ISymbol? Symbol;
        internal readonly SyntaxToken SymbolIdentifier;
        internal readonly ExpressionSyntax? Value;
        internal readonly LiteralExpressionSyntax? StringLiteral;
#pragma warning restore RS1008 // Avoid storing per-compilation data into the fields of a diagnostic analyzer.

        private StringArgument(ExpressionSyntax expression, ISymbol? symbol, SyntaxToken symbolIdentifier, ExpressionSyntax? value)
        {
            this.Expression = expression;
            this.Symbol = symbol;
            this.SymbolIdentifier = symbolIdentifier;
            this.Value = value;
            switch (value)
            {
                case LiteralExpressionSyntax literal
                    when literal.IsKind(SyntaxKind.StringLiteralExpression):
                    this.StringLiteral = literal;
                    break;
                case InvocationExpressionSyntax { Expression: MemberAccessExpressionSyntax { Expression: LiteralExpressionSyntax literal } }
                    when literal.IsKind(SyntaxKind.StringLiteralExpression):
                    this.StringLiteral = literal;
                    break;
                default:
                    this.StringLiteral = null;
                    break;
            }
        }

        internal bool? HasPosition
        {
            get
            {
                return this.Value switch
                {
                    LiteralExpressionSyntax literal when literal.IsKind(SyntaxKind.StringLiteralExpression) => literal.Token.ValueText.Contains("↓"),
                    InvocationExpressionSyntax { Expression: LiteralExpressionSyntax { Token: { ValueText: { } valueText } } } => valueText.Contains("↓"),
                    _ => (bool?)null,
                };
            }
        }

        public static bool operator ==(StringArgument left, StringArgument right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(StringArgument left, StringArgument right)
        {
            return !left.Equals(right);
        }

        public bool Equals(StringArgument other) => this.Expression.Equals(other.Expression);

        public override bool Equals(object? obj) => obj is StringArgument other && this.Equals(other);

        public override int GetHashCode() => this.Expression.GetHashCode();

        internal static bool TrySingle(InvocationExpressionSyntax invocation, IParameterSymbol parameter, SemanticModel semanticModel, CancellationToken cancellationToken, out StringArgument result)
        {
            if (invocation.TryFindArgument(parameter, out var argument))
            {
                switch (argument.Expression.Kind())
                {
                    case SyntaxKind.ImplicitArrayCreationExpression:
                    case SyntaxKind.ArrayCreationExpression:
                    case SyntaxKind.ObjectCreationExpression:
                        break;
                    default:
                        result = Create(argument.Expression, semanticModel, cancellationToken);
                        return true;
                }
            }

            result = default;
            return false;
        }

        internal static bool TryMany(InvocationExpressionSyntax invocation, IParameterSymbol parameter, SemanticModel semanticModel, CancellationToken cancellationToken, out ImmutableArray<StringArgument> results)
        {
            if (TryGetCollectionInitializer(out var initializer))
            {
                var builder = ImmutableArray.CreateBuilder<StringArgument>(initializer!.Expressions.Count);
                foreach (var expression in initializer.Expressions)
                {
                    builder.Add(Create(expression, semanticModel, cancellationToken));
                }

                results = builder.MoveToImmutable();
                return true;
            }

            if (parameter.IsParams &&
                invocation.ArgumentList is { Arguments: { } arguments })
            {
                var builder = ImmutableArray.CreateBuilder<StringArgument>(arguments.Count - parameter.Ordinal);
                for (var i = parameter.Ordinal; i < arguments.Count; i++)
                {
                    builder.Add(Create(arguments[i].Expression, semanticModel, cancellationToken));
                }

                results = builder.MoveToImmutable();
                return true;
            }

            results = default;
            return false;

            bool TryGetCollectionInitializer(out InitializerExpressionSyntax? result)
            {
                if (invocation.TryFindArgument(parameter, out var argument))
                {
                    switch (argument.Expression)
                    {
                        case ImplicitArrayCreationExpressionSyntax { Initializer: { } initializer }:
                            result = initializer;
                            return true;
                        case ArrayCreationExpressionSyntax { Initializer: { } initializer }:
                            result = initializer;
                            return true;
                        case ObjectCreationExpressionSyntax { Initializer: { } initializer }:
                            result = initializer;
                            return true;
                    }
                }

                result = null;
                return false;
            }
        }

        internal static StringArgument Create(ExpressionSyntax expression, SemanticModel semanticModel, CancellationToken cancellationToken)
        {
            if (expression is IdentifierNameSyntax candidate &&
                semanticModel.TryGetSymbol(candidate, cancellationToken, out var candidateSymbol))
            {
                _ = TryGetValue(out var symbolIdentifier, out var value);
                return new StringArgument(expression, candidateSymbol, symbolIdentifier, value);
            }

            if (expression.Kind() == SyntaxKind.StringLiteralExpression)
            {
                return new StringArgument(expression, null, default, expression);
            }

            return new StringArgument(expression, null, default, null);

            bool TryGetValue(out SyntaxToken identifier, out ExpressionSyntax? result)
            {
                if (candidateSymbol!.TrySingleDeclaration(cancellationToken, out LocalDeclarationStatementSyntax? localDeclaration) &&
                    localDeclaration.Declaration is { Variables: { Count: 1 } localVariables } &&
                    localVariables.TrySingle(out var localVariable) &&
                    localVariable.Initializer is { } localInitializer)
                {
                    identifier = localVariable.Identifier;
                    result = localInitializer.Value;
                    return true;
                }

                if (candidateSymbol!.TrySingleDeclaration(cancellationToken, out FieldDeclarationSyntax? fieldDeclaration) &&
                    fieldDeclaration.Declaration is { Variables: { Count: 1 } fieldVariables } &&
                    fieldVariables.TrySingle(out var fieldVariable) &&
                    fieldVariable.Initializer is { } fieldInitializer)
                {
                    identifier = fieldVariable.Identifier;
                    result = fieldInitializer.Value;
                    return true;
                }

                identifier = default;
                result = null;
                return false;
            }
        }

        internal bool TryGetNameFromCode([NotNullWhen(true)] out string? codeName)
        {
            codeName = null;
#pragma warning disable CS8762 // Parameter must have a non-null value when exiting in some condition.
            return this.StringLiteral is { Token: { ValueText: { } valueText } } &&
                   (TryGetName(valueText, "class ", out codeName) ||
                    TryGetName(valueText, "struct ", out codeName) ||
                    TryGetName(valueText, "interface ", out codeName) ||
                    TryGetName(valueText, "enum ", out codeName));
#pragma warning restore CS8762 // Parameter must have a non-null value when exiting in some condition.

            static bool TryGetName(string text, string prefix, out string? name)
            {
                var index = text.IndexOf(prefix, StringComparison.Ordinal);
                while (index > 0 &&
                       text.LastIndexOf('/', index) > text.LastIndexOf('\n', index))
                {
                    index = text.IndexOf(prefix, index + 1, StringComparison.Ordinal);
                }

                if (index >= 0 &&
                    text.LastIndexOf("partial", index, StringComparison.Ordinal) < 0)
                {
                    var start = index + prefix.Length;
                    var end = text.IndexOfAny(new[] { ':', '{', '\r', '\n' }, start);
                    if (end > start)
                    {
                        name = text.Substring(start, end - start).Replace("<", "Of")
                                                                 .Replace(">", string.Empty)
                                                                 .Replace(">", string.Empty)
                                                                 .Replace(",", string.Empty)
                                                                 .Replace(" ", string.Empty);
                        return SyntaxFacts.IsValidIdentifier(name);
                    }
                }

                name = null;
                return false;
            }
        }
    }
}
