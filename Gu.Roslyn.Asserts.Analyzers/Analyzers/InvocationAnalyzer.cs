// ReSharper disable PossiblyImpureMethodCallOnReadonlyVariable
namespace Gu.Roslyn.Asserts.Analyzers
{
    using System;
    using System.Collections.Immutable;
    using System.Diagnostics;
    using System.Diagnostics.CodeAnalysis;
    using System.Threading;
    using Gu.Roslyn.AnalyzerExtensions;
    using Gu.Roslyn.CodeFixExtensions;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using Microsoft.CodeAnalysis.Diagnostics;

    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    internal class InvocationAnalyzer : DiagnosticAnalyzer
    {
        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get; } = ImmutableArray.Create(
            Descriptors.GURA01NameShouldMatchParameter,
            Descriptors.GURA02IndicateErrorPosition,
            Descriptors.GURA03NameShouldMatchCode,
            Descriptors.GURA10UseLocal,
            Descriptors.GURA11ChainAssertReplace);

        public override void Initialize(AnalysisContext context)
        {
            context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
            context.EnableConcurrentExecution();
            context.RegisterSyntaxNodeAction(c => Handle(c), SyntaxKind.InvocationExpression);
        }

        private static void Handle(SyntaxNodeAnalysisContext context)
        {
            if (context.Node is InvocationExpressionSyntax invocation &&
                context.SemanticModel.TryGetSymbol(invocation, context.CancellationToken, out var method))
            {
                if (method.ContainingType == KnownSymbols.RoslynAssert)
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
                                if (ShouldIndicatePosition() &&
                                    stringArg.Value is { })
                                {
                                    context.ReportDiagnostic(
                                        Diagnostic.Create(
                                            Descriptors.GURA02IndicateErrorPosition,
                                            stringArg.Value.GetLocation()));
                                }

                                if (stringArg.Symbol is { } symbol)
                                {
                                    if (stringArg.TryGetNameFromCode(out var codeName) &&
                                        ShouldRename(stringArg.Symbol, codeName, out codeName))
                                    {
                                        context.ReportDiagnostic(
                                            Diagnostic.Create(
                                                Descriptors.GURA03NameShouldMatchCode,
                                                stringArg.SymbolIdentifier.GetLocation(),
                                                ImmutableDictionary<string, string>.Empty.Add(nameof(IdentifierNameSyntax), codeName),
                                                symbol.Name,
                                                codeName));
                                    }

                                    if (stringArg.Symbol.Kind == SymbolKind.Local &&
                                        stringArg.HasPosition == true &&
                                        args.TrySingle(x => x.HasPosition == true, out _) &&
                                        ShouldRename(symbol, parameter.Name, out var parameterName))
                                    {
                                        context.ReportDiagnostic(
                                            Diagnostic.Create(
                                                Descriptors.GURA01NameShouldMatchParameter,
                                                stringArg.Expression.GetLocation(),
                                                ImmutableDictionary<string, string>.Empty.Add(nameof(IdentifierNameSyntax), parameterName),
                                                symbol.Name,
                                                parameterName));
                                    }

                                    if (stringArg.Symbol.IsEitherKind(SymbolKind.Field, SymbolKind.Property) &&
                                        stringArg.Value is { })
                                    {
                                        context.ReportDiagnostic(
                                            Diagnostic.Create(
                                                Descriptors.GURA10UseLocal,
                                                stringArg.Expression.GetLocation(),
                                                additionalLocations: new[] { stringArg.Value.GetLocation() },
                                                messageArgs: stringArg.Symbol.Name));
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
                                            if (a.Symbol?.Kind == SymbolKind.Local &&
                                                (a.HasPosition != false ||
                                                 a.Symbol.Name == parameter.Name))
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
                                if (stringArg.Symbol is { } symbol)
                                {
                                    if (stringArg.TryGetNameFromCode(out var codeName) &&
                                        ShouldRename(stringArg.Symbol, codeName, out codeName))
                                    {
                                        context.ReportDiagnostic(
                                            Diagnostic.Create(
                                                Descriptors.GURA03NameShouldMatchCode,
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
                                                Descriptors.GURA01NameShouldMatchParameter,
                                                stringArg.Expression.GetLocation(),
                                                ImmutableDictionary<string, string>.Empty.Add(nameof(IdentifierNameSyntax), parameterName),
                                                symbol.Name,
                                                parameterName));
                                    }

                                    if (stringArg.Symbol.IsEitherKind(SymbolKind.Field, SymbolKind.Property) &&
                                        stringArg.Value is { })
                                    {
                                        context.ReportDiagnostic(
                                            Diagnostic.Create(
                                                Descriptors.GURA10UseLocal,
                                                stringArg.Expression.GetLocation(),
                                                additionalLocations: new[] { stringArg.Value.GetLocation() },
                                                messageArgs: stringArg.Symbol.Name));
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
                                    Descriptors.GURA01NameShouldMatchParameter,
                                    argument.Expression.GetLocation(),
                                    ImmutableDictionary<string, string>.Empty.Add(nameof(IdentifierNameSyntax), newName),
                                    symbol.Name,
                                    newName));
                        }
                    }
                }
                else if (method.Name == "AssertReplace" &&
                         ShouldChain(invocation, context.SemanticModel, context.CancellationToken, out var location, out var additionalLocation))
                {
                    context.ReportDiagnostic(
                        Diagnostic.Create(
                            Descriptors.GURA11ChainAssertReplace,
                            location,
                            additionalLocations: new[] { additionalLocation }));
                }
            }
        }

        private static bool ShouldRename(ISymbol symbol, string expectedName, [NotNullWhen(true)]out string? newName)
        {
            if (symbol.IsEitherKind(SymbolKind.Local, SymbolKind.Field, SymbolKind.Property) &&
                symbol.ContainingType.TypeKind != TypeKind.Enum &&
                symbol.Name.IndexOf(expectedName, StringComparison.OrdinalIgnoreCase) < 0)
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
                return symbol switch
                {
                    IFieldSymbol field => field.IsConst,
                    ILocalSymbol local => local.IsConst,
                    _ => false,
                };
            }
        }

        private static bool ShouldChain(InvocationExpressionSyntax invocation, SemanticModel semanticModel, CancellationToken cancellationToken, [NotNullWhen(true)]out Location? location, [NotNullWhen(true)]out Location? additionalLocation)
        {
            if (invocation.Expression is MemberAccessExpressionSyntax memberAccess &&
                memberAccess.Expression is IdentifierNameSyntax identifierName &&
                invocation.Parent is AssignmentExpressionSyntax assignment &&
                assignment.Left is IdentifierNameSyntax left &&
                left.Identifier.ValueText == identifierName.Identifier.ValueText &&
                StringArgument.Create(identifierName, semanticModel, cancellationToken) is StringArgument stringArgument &&
                stringArgument.Symbol?.Kind == SymbolKind.Local &&
                stringArgument.Value.IsKind(SyntaxKind.StringLiteralExpression))
            {
                location = memberAccess.Name.GetLocation();
                additionalLocation = stringArgument.Value!.GetLocation();
                return true;
            }

            location = null;
            additionalLocation = null;
            return false;
        }

        [DebuggerDisplay("{Expression}")]
        internal struct StringArgument : IEquatable<StringArgument>
        {
#pragma warning disable RS1008 // Avoid storing per-compilation data into the fields of a diagnostic analyzer.
            internal readonly ExpressionSyntax Expression;
            internal readonly ISymbol? Symbol;
            internal readonly SyntaxToken SymbolIdentifier;
            internal readonly ExpressionSyntax? Value;
#pragma warning restore RS1008 // Avoid storing per-compilation data into the fields of a diagnostic analyzer.

            private StringArgument(ExpressionSyntax expression, ISymbol? symbol, SyntaxToken symbolIdentifier, ExpressionSyntax? value)
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
                    if (candidateSymbol.TrySingleDeclaration(cancellationToken, out LocalDeclarationStatementSyntax? localDeclaration) &&
                        localDeclaration.Declaration is { Variables: { Count: 1 } localVariables } &&
                        localVariables.TrySingle(out var localVariable) &&
                        localVariable.Initializer is { } localInitializer)
                    {
                        identifier = localVariable.Identifier;
                        result = localInitializer.Value;
                        return true;
                    }

                    if (candidateSymbol.TrySingleDeclaration(cancellationToken, out FieldDeclarationSyntax? fieldDeclaration) &&
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

            internal bool TryGetNameFromCode([NotNullWhen(true)]out string? codeName)
            {
                codeName = null;
                return this.Value is LiteralExpressionSyntax { Token: { ValueText: { } valueText } } &&
                       (TryGetName(valueText, "class ", out codeName) ||
                        TryGetName(valueText, "struct ", out codeName) ||
                        TryGetName(valueText, "interface ", out codeName) ||
                        TryGetName(valueText, "enum ", out codeName));

                static bool TryGetName(string text, string prefix, out string? name)
                {
                    var index = text.IndexOf(prefix, StringComparison.Ordinal);
                    if (index >= 0 &&
                        text.LastIndexOf("partial", index, StringComparison.Ordinal) < 0)
                    {
                        var start = index + prefix.Length;
                        var end = text.IndexOfAny(new[] { ' ', '\r', '\n' }, start);
                        if (end > start)
                        {
                            name = text.Substring(start, end - start).Replace("<", "Of").Replace(">", string.Empty);
                            return true;
                        }
                    }

                    name = null;
                    return false;
                }
            }
        }
    }
}
