namespace Gu.Roslyn.Asserts.Analyzers
{
    using System;
    using System.Collections.Immutable;
    using System.Diagnostics;
    using System.Text.RegularExpressions;
    using System.Threading;
    using Gu.Roslyn.AnalyzerExtensions;
    using Gu.Roslyn.CodeFixExtensions;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using Microsoft.CodeAnalysis.Diagnostics;

    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class InvocationAnalyzer : DiagnosticAnalyzer
    {
        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get; } = ImmutableArray.Create(
            Descriptors.NameShouldMatchParameter,
            Descriptors.IndicateErrorPosition,
            Descriptors.NameShouldMatchCode);

        public override void Initialize(AnalysisContext context)
        {
            context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
            context.EnableConcurrentExecution();
            context.RegisterSyntaxNodeAction(c => Handle(c), SyntaxKind.InvocationExpression);
        }

        private static void Handle(SyntaxNodeAnalysisContext context)
        {
            if (context.Node is InvocationExpressionSyntax invocation &&
                context.SemanticModel.TryGetSymbol(invocation, context.CancellationToken, out var method) &&
                method.ContainingType == KnownSymbol.RoslynAssert)
            {
                foreach (var parameter in method.Parameters)
                {
                    if (parameter.Name == "before" ||
                        (parameter.Name == "code" && method.Name == "Diagnostics"))
                    {
                        if (StringArgument.TrySingle(invocation, parameter, context.SemanticModel, context.CancellationToken, out var single))
                        {
                            Handle(single, ImmutableArray<StringArgument>.Empty);
                        }
                        else if (StringArgument.TryMany(invocation, parameter, context.SemanticModel, context.CancellationToken, out var stringArgs))
                        {
                            foreach (var arg in stringArgs)
                            {
                                Handle(arg, stringArgs);
                            }
                        }

                        void Handle(StringArgument stringArg, ImmutableArray<StringArgument> args)
                        {
                            if (ShouldIndicatePosition())
                            {
                                context.ReportDiagnostic(
                                    Diagnostic.Create(
                                        Descriptors.IndicateErrorPosition,
                                        stringArg.Value.GetLocation()));
                            }

                            if (stringArg.Symbol is ISymbol symbol)
                            {
                                if (stringArg.TryGetNameFromCode(out var codeName) &&
                                    ShouldRename(stringArg.Symbol, codeName, out codeName))
                                {
                                    context.ReportDiagnostic(
                                        Diagnostic.Create(
                                            Descriptors.NameShouldMatchCode,
                                            stringArg.SymbolIdentifier.GetLocation(),
                                            ImmutableDictionary<string, string>.Empty.Add(nameof(IdentifierNameSyntax), codeName),
                                            symbol.Name,
                                            codeName));
                                }

                                if (stringArg.Symbol.Kind == SymbolKind.Local &&
                                    stringArg.HasPosition == true &&
                                    ShouldRename(symbol, parameter.Name, out var parameterName))
                                {
                                    context.ReportDiagnostic(
                                        Diagnostic.Create(
                                            Descriptors.NameShouldMatchParameter,
                                            stringArg.Expression.GetLocation(),
                                            ImmutableDictionary<string, string>.Empty.Add(nameof(IdentifierNameSyntax), parameterName),
                                            symbol.Name,
                                            parameterName));
                                }
                            }

                            bool ShouldIndicatePosition()
                            {
                                if (stringArg.HasPosition == false &&
                                    stringArg.Symbol?.IsEitherKind(SymbolKind.Field, SymbolKind.Property) != true)
                                {
                                    if (stringArg.Symbol?.Name == parameter.Name)
                                    {
                                        return true;
                                    }

                                    foreach (var a in args)
                                    {
                                        if (a.HasPosition == true ||
                                            a.Symbol?.Name == parameter.Name)
                                        {
                                            return false;
                                        }
                                    }

                                    return true;
                                }

                                return false;
                            }
                        }
                    }
                    else if (parameter.Name == "after" || parameter.Name == "code")
                    {
                        if (StringArgument.TrySingle(invocation, parameter, context.SemanticModel, context.CancellationToken, out var single))
                        {
                            Handle(single, ImmutableArray<StringArgument>.Empty);
                        }
                        else if (StringArgument.TryMany(invocation, parameter, context.SemanticModel, context.CancellationToken, out var stringArgs))
                        {
                            foreach (var arg in stringArgs)
                            {
                                Handle(arg, stringArgs);
                            }
                        }

                        void Handle(StringArgument stringArg, ImmutableArray<StringArgument> args)
                        {
                            if (stringArg.Symbol is ISymbol symbol)
                            {
                                if (stringArg.TryGetNameFromCode(out var codeName) &&
                                    ShouldRename(stringArg.Symbol, codeName, out codeName))
                                {
                                    context.ReportDiagnostic(
                                        Diagnostic.Create(
                                            Descriptors.NameShouldMatchCode,
                                            stringArg.SymbolIdentifier.GetLocation(),
                                            ImmutableDictionary<string, string>.Empty.Add(nameof(IdentifierNameSyntax), codeName),
                                            symbol.Name,
                                            codeName));
                                }

                                if (stringArg.Symbol.Kind == SymbolKind.Local &&
                                    !args.TryFirst(x => x.Symbol is ILocalSymbol && !Equals(x.Symbol, stringArg.Symbol), out _) &&
                                    ShouldRename(symbol, parameter.Name, out var parameterName))
                                {
                                    context.ReportDiagnostic(
                                        Diagnostic.Create(
                                            Descriptors.NameShouldMatchParameter,
                                            stringArg.Expression.GetLocation(),
                                            ImmutableDictionary<string, string>.Empty.Add(nameof(IdentifierNameSyntax), parameterName),
                                            symbol.Name,
                                            parameterName));
                                }
                            }
                        }
                    }
                    else if (invocation.TryFindArgument(parameter, out var argument) &&
                             context.SemanticModel.TryGetSymbol(argument.Expression, context.CancellationToken, out var symbol) &&
                             ShouldRename(symbol, parameter.Name, out var newName) &&
                             context.SemanticModel.TryGetType(argument.Expression, context.CancellationToken, out var type) &&
                             ShouldRename(symbol, type.Name, out _))
                    {
                        context.ReportDiagnostic(
                            Diagnostic.Create(
                                Descriptors.NameShouldMatchParameter,
                                argument.Expression.GetLocation(),
                                ImmutableDictionary<string, string>.Empty.Add(nameof(IdentifierNameSyntax), newName),
                                symbol.Name,
                                newName));
                    }
                }
            }
        }

        private static bool ShouldRename(ISymbol symbol, string expectedName, out string newName)
        {
            if (!string.Equals(symbol.Name, expectedName, StringComparison.OrdinalIgnoreCase))
            {
                if (symbol.IsStatic || IsConst())
                {
                    newName = expectedName.ToFirstCharUpper();
                }
                else if (char.IsUpper(expectedName[0]))
                {
                    newName = expectedName.ToFirstCharLower();
                }
                else
                {
                    newName = expectedName;
                }

                return true;
            }

            newName = null;
            return false;

            bool IsConst()
            {
                switch (symbol)
                {
                    case IFieldSymbol field:
                        return field.IsConst;
                    case ILocalSymbol local:
                        return local.IsConst;
                    default:
                        return false;
                }
            }
        }

        [DebuggerDisplay("{Expression}")]
        internal struct StringArgument : IEquatable<StringArgument>
        {
#pragma warning disable RS1008 // Avoid storing per-compilation data into the fields of a diagnostic analyzer.
            internal readonly ExpressionSyntax Expression;
            internal readonly ISymbol Symbol;
            internal readonly SyntaxToken SymbolIdentifier;
            internal readonly ExpressionSyntax Value;
#pragma warning restore RS1008 // Avoid storing per-compilation data into the fields of a diagnostic analyzer.

            private StringArgument(ExpressionSyntax expression, ISymbol symbol, SyntaxToken symbolIdentifier, ExpressionSyntax value)
            {
                this.Expression = expression;
                this.Symbol = symbol;
                this.SymbolIdentifier = symbolIdentifier;
                this.Value = value;
            }

            internal bool? HasPosition
            {
                get
                {
                    switch (this.Value)
                    {
                        case LiteralExpressionSyntax literal when literal.IsKind(SyntaxKind.StringLiteralExpression):
                            return literal.Token.ValueText.Contains("↓");
                        case InvocationExpressionSyntax invocation when invocation.Expression is MemberAccessExpressionSyntax memberAccess &&
                                                                        memberAccess.Expression is LiteralExpressionSyntax literal &&
                                                                        literal.Token.ValueText.Contains("↓"):
                            return true;
                        default:
                            return null;
                    }
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

            public override bool Equals(object obj) => obj is StringArgument other && this.Equals(other);

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
                    var builder = ImmutableArray.CreateBuilder<StringArgument>(initializer.Expressions.Count);
                    foreach (var expression in initializer.Expressions)
                    {
                        builder.Add(Create(expression, semanticModel, cancellationToken));
                    }

                    results = builder.MoveToImmutable();
                    return true;
                }

                if (parameter.IsParams &&
                    invocation.ArgumentList is ArgumentListSyntax argumentList)
                {
                    var builder = ImmutableArray.CreateBuilder<StringArgument>(argumentList.Arguments.Count - parameter.Ordinal);
                    for (var i = parameter.Ordinal; i < argumentList.Arguments.Count; i++)
                    {
                        builder.Add(Create(argumentList.Arguments[i].Expression, semanticModel, cancellationToken));
                    }

                    results = builder.MoveToImmutable();
                    return true;
                }

                results = default;
                return false;

                bool TryGetCollectionInitializer(out InitializerExpressionSyntax result)
                {
                    if (invocation.TryFindArgument(parameter, out var argument) ||
                        invocation.ArgumentList.Arguments.TryElementAt(parameter.Ordinal, out argument))
                    {
                        switch (argument.Expression)
                        {
                            case ImplicitArrayCreationExpressionSyntax arrayCreation:
                                result = arrayCreation.Initializer;
                                return result != null;
                            case ArrayCreationExpressionSyntax arrayCreation:
                                result = arrayCreation.Initializer;
                                return result != null;
                            case ObjectCreationExpressionSyntax objectCreation:
                                result = objectCreation.Initializer;
                                return result != null;
                        }
                    }

                    result = null;
                    return false;
                }
            }

            internal bool TryGetNameFromCode(out string codeName)
            {
                if (this.Value is LiteralExpressionSyntax literal &&
                    Regex.Match(literal.Token.ValueText, @"^ *(↓?(public|internal|static|sealed|abstract) )*↓?(class|struct|enum|interface) ↓?(?<name>\w+)(<(?<type>↓?\w+)(, ?(?<type>↓?\w+))*>)?", RegexOptions.ExplicitCapture | RegexOptions.Multiline) is Match match &&
                    match.Success &&
                    !match.Groups["type"].Success)
                {
                    codeName = match.Groups["name"].Value;
                    return true;
                }

                codeName = null;
                return false;
            }

            private static StringArgument Create(ExpressionSyntax expression, SemanticModel semanticModel, CancellationToken cancellationToken)
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

                bool TryGetValue(out SyntaxToken identifier, out ExpressionSyntax result)
                {
                    if (candidateSymbol.TrySingleDeclaration(cancellationToken, out LocalDeclarationStatementSyntax localDeclaration) &&
                        localDeclaration.Declaration is VariableDeclarationSyntax localVariableDeclaration &&
                        localVariableDeclaration.Variables.TrySingle(out var localVariable) &&
                        localVariable.Initializer is EqualsValueClauseSyntax localInitializer)
                    {
                        identifier = localVariable.Identifier;
                        result = localInitializer.Value;
                        return true;
                    }

                    if (candidateSymbol.TrySingleDeclaration(cancellationToken, out FieldDeclarationSyntax fieldDeclaration) &&
                        fieldDeclaration.Declaration is VariableDeclarationSyntax fieldVariableDeclaration &&
                        fieldVariableDeclaration.Variables.TrySingle(out var fieldVariable) &&
                        fieldVariable.Initializer is EqualsValueClauseSyntax fieldInitializer)
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
        }
    }
}
