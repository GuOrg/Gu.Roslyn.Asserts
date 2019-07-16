namespace Gu.Roslyn.Asserts.Analyzers
{
    using System;
    using System.Collections.Immutable;
    using System.Diagnostics;
    using System.Text.RegularExpressions;
    using System.Threading;
    using Gu.Roslyn.AnalyzerExtensions;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using Microsoft.CodeAnalysis.Diagnostics;
    using Microsoft.CodeAnalysis.Text;

    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class ArgumentAnalyzer : DiagnosticAnalyzer
    {
        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get; } = ImmutableArray.Create(
            Descriptors.NameOfLocalShouldMatchParameter,
            Descriptors.IndicateErrorPosition,
            Descriptors.NameToFirstClass);

        public override void Initialize(AnalysisContext context)
        {
            context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
            context.EnableConcurrentExecution();
            context.RegisterSyntaxNodeAction(c => Handle(c), SyntaxKind.Argument);
        }

        private static void Handle(SyntaxNodeAnalysisContext context)
        {
            if (context.Node is ArgumentSyntax argument &&
                argument.Parent is ArgumentListSyntax argumentList &&
                argumentList.Parent is InvocationExpressionSyntax invocation &&
                context.SemanticModel.TryGetSymbol(invocation, context.CancellationToken, out var method) &&
                method.ContainingType == KnownSymbol.RoslynAssert &&
                method.TryFindParameter(argument, out var parameter))
            {
                if (argument.Expression is IdentifierNameSyntax identifierName &&
                    context.SemanticModel.TryGetSymbol(identifierName, context.CancellationToken, out ILocalSymbol local) &&
                    parameter.Name != local.Name &&
                    !IsParams())
                {
                    context.ReportDiagnostic(
                        Diagnostic.Create(
                            Descriptors.NameOfLocalShouldMatchParameter,
                            identifierName.GetLocation(),
                            ImmutableDictionary<string, string>.Empty.Add(nameof(IdentifierNameSyntax), parameter.Name),
                            local.Name,
                            parameter.Name));
                }

                var args = StringArg.CreateMany(argument, parameter, context.SemanticModel, context.CancellationToken);
                if (StringArg.ShouldRename(argument, parameter, args, out var toRename, out var descriptor, out var newName))
                {
                    context.ReportDiagnostic(
                        Diagnostic.Create(
                            descriptor,
                            toRename.GetLocation(),
                            ImmutableDictionary<string, string>.Empty.Add(nameof(IdentifierNameSyntax), newName),
                            toRename.Identifier.ValueText,
                            newName));
                }

                if (StringArg.ShouldIndicatePosition(argument, parameter, args, out var location, out var additionalLocation, out var message))
                {
                    context.ReportDiagnostic(
                        Diagnostic.Create(
                            Descriptors.IndicateErrorPosition,
                            location,
                            messageArgs: message,
                            additionalLocations: additionalLocation == null ? Array.Empty<Location>() : new[] { additionalLocation }));
                }

                bool IsParams()
                {
                    if (parameter.IsParams)
                    {
                        return !invocation.TryFindArgument(parameter, out _);
                    }

                    return false;
                }
            }
        }

        [DebuggerDisplay("{expression}")]
        private struct StringArg
        {
            private readonly ExpressionSyntax expression;
#pragma warning disable RS1008 // Avoid storing per-compilation data into the fields of a diagnostic analyzer.
            private readonly ISymbol symbol;
#pragma warning restore RS1008 // Avoid storing per-compilation data into the fields of a diagnostic analyzer.
            private readonly ExpressionSyntax value;

            private StringArg(ExpressionSyntax identifierName, ISymbol symbol, ExpressionSyntax value)
            {
                this.expression = identifierName;
                this.symbol = symbol;
                this.value = value;
            }

            private bool? HasPosition
            {
                get
                {
                    switch (this.value)
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

            internal static ImmutableArray<StringArg> CreateMany(ArgumentSyntax argument, IParameterSymbol parameter, SemanticModel semanticModel, CancellationToken cancellationToken)
            {
                if (TryGetInitializer(out var initializer))
                {
                    var builder = ImmutableArray.CreateBuilder<StringArg>(initializer.Expressions.Count);
                    foreach (var expression in initializer.Expressions)
                    {
                        builder.Add(Create(expression, semanticModel, cancellationToken));
                    }

                    return builder.MoveToImmutable();
                }

                if (parameter.IsParams)
                {
                    if (argument.Parent is ArgumentListSyntax argumentList)
                    {
                        var builder = ImmutableArray.CreateBuilder<StringArg>(argumentList.Arguments.Count - parameter.Ordinal);
                        for (var i = parameter.Ordinal; i < argumentList.Arguments.Count; i++)
                        {
                            builder.Add(Create(argumentList.Arguments[i].Expression, semanticModel, cancellationToken));
                        }

                        return builder.MoveToImmutable();
                    }

                    return ImmutableArray<StringArg>.Empty;
                }

                return ImmutableArray.Create(Create(argument.Expression, semanticModel, cancellationToken));

                bool TryGetInitializer(out InitializerExpressionSyntax result)
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
                        default:
                            result = null;
                            return false;
                    }
                }
            }

            internal static bool ShouldIndicatePosition(ArgumentSyntax argument, IParameterSymbol parameter, ImmutableArray<StringArg> args, out Location location, out Location additionalLocation, out string message)
            {
                if (IsPositionArgument(argument, parameter, args, out message))
                {
                    if (args.TryFirst(x => x.HasPosition == true, out _) ||
                        args.TryFirst(x => x.symbol is ILocalSymbol && x.HasPosition == null, out _))
                    {
                        location = null;
                        additionalLocation = null;
                        message = null;
                        return false;
                    }

                    if (args.TrySingle(x => x.symbol != null && x.HasPosition == false, out var match))
                    {
                        location = match.expression.GetLocation();
                        additionalLocation = match.value.GetLocation();
                        return argument.Contains(match.expression);
                    }

                    if (args.TrySingle(x => x.symbol?.Name == parameter.Name && x.HasPosition == false, out match))
                    {
                        location = match.expression.GetLocation();
                        additionalLocation = match.value.GetLocation();
                        return argument.Contains(match.expression);
                    }

                    if (args.TryFirst(x => x.symbol != null && x.HasPosition == false, out match))
                    {
                        location = argument.GetLocation();
                        additionalLocation = null;
                        return true;
                    }
                }

                location = null;
                additionalLocation = null;
                message = null;
                return false;
            }

            internal static bool ShouldRename(ArgumentSyntax argument, IParameterSymbol parameter, ImmutableArray<StringArg> args, out IdentifierNameSyntax identifierName, out DiagnosticDescriptor descriptor, out string newName)
            {
                if (IsPositionArgument(argument, parameter, args, out _))
                {
                    if (args.TrySingle(x => x.symbol != null, out var match) &&
                        argument.Contains(match.expression))
                    {
                        identifierName = (IdentifierNameSyntax)match.expression;
                        descriptor = Descriptors.NameOfLocalShouldMatchParameter;
                        newName = parameter.Name;
                        return !IsMatch(identifierName, parameter.Name);
                    }

                    if (args.TrySingle(x => x.symbol != null && x.HasPosition == true, out match) &&
                        argument.Contains(match.expression))
                    {
                        identifierName = (IdentifierNameSyntax)match.expression;
                        descriptor = Descriptors.NameOfLocalShouldMatchParameter;
                        newName = parameter.Name;
                        return !IsMatch(identifierName, parameter.Name);
                    }
                }

                if (args.Length > 1 &&
                    args.TrySingle(x => x.expression == argument.Expression, out var stringArg) &&
                    TryGetNameFromCode(stringArg, out identifierName, out newName) &&
                    stringArg.symbol.Name != newName)
                {
                    descriptor = Descriptors.NameToFirstClass;
                    return true;
                }

                identifierName = null;
                newName = null;
                descriptor = null;
                return false;

                bool TryGetNameFromCode(StringArg arg, out IdentifierNameSyntax identifier, out string name)
                {
                    if (arg.expression is IdentifierNameSyntax candidate &&
                        arg.value is LiteralExpressionSyntax literal &&
                        Regex.Match(literal.Token.ValueText, @"^ *(↓?(public|internal|static|sealed|abstract) )*↓?(class|struct|enum|interface) ↓?(?<name>\w+)(<(?<type>↓?\w+)(, ?(?<type>↓?\w+))*>)?", RegexOptions.ExplicitCapture | RegexOptions.Multiline) is Match match &&
                        match.Success &&
                        !match.Groups["type"].Success &&
                        !IsMatch(candidate, match.Groups["name"].Value))
                    {
                        identifier = candidate;
                        name = match.Groups["name"].Value;
                        return true;
                    }

                    identifier = null;
                    name = null;
                    return false;
                }

                bool IsMatch(IdentifierNameSyntax identifier, string expected)
                {
                    if (identifier.Identifier.ValueText.Length != expected.Length)
                    {
                        return false;
                    }

                    if (char.ToLowerInvariant(identifier.Identifier.ValueText[0]) != char.ToLowerInvariant(expected[0]))
                    {
                        return false;
                    }

                    for (var i = 1; i < expected.Length; i++)
                    {
                        if (identifier.Identifier.ValueText[i] != expected[i])
                        {
                            return false;
                        }
                    }

                    return true;
                }
            }

            private static bool IsPositionArgument(ArgumentSyntax argument, IParameterSymbol parameter, ImmutableArray<StringArg> args, out string message)
            {
                if (args.TrySingle(x => x.expression == argument.Expression, out var match) &&
                   match.symbol?.Kind != SymbolKind.Local)
                {
                    message = null;
                    return false;
                }

                if (parameter.Name == "before" ||
                    (parameter.ContainingSymbol.Name == "Diagnostics" && parameter.Name == "code") ||
                    (parameter.ContainingSymbol.Name == "Refactoring" && parameter.Name == "code"))
                {
                    message = "Indicate expected error position with ↓ (alt + 25).";
                    return true;
                }

                if (parameter.Name == "before" &&
                    parameter.ContainingSymbol is IMethodSymbol method &&
                    method.Name == "Refactoring" &&
                    !method.Parameters.TryFirst(x => x.Type.MetadataName == typeof(TextSpan).Name, out _))
                {
                    message = "Indicate cursor position with ↓ (alt + 25).";
                    return true;
                }

                message = null;
                return false;
            }

            private static StringArg Create(ExpressionSyntax expression, SemanticModel semanticModel, CancellationToken cancellationToken)
            {
                if (expression is IdentifierNameSyntax candidate &&
                    semanticModel.TryGetSymbol(candidate, cancellationToken, out ISymbol candidateSymbol))
                {
                    _ = TryGetValue(out var literal);
                    return new StringArg(expression, candidateSymbol, literal);
                }

                return new StringArg(expression, null, null);

                bool TryGetValue(out ExpressionSyntax result)
                {
                    if (candidateSymbol.TrySingleDeclaration(cancellationToken, out LocalDeclarationStatementSyntax localDeclaration) &&
                        localDeclaration.Declaration is VariableDeclarationSyntax localVariableDeclaration &&
                        localVariableDeclaration.Variables.TrySingle(out var localVariable) &&
                        localVariable.Initializer is EqualsValueClauseSyntax localInitializer)
                    {
                        result = localInitializer.Value;
                        return true;
                    }

                    if (candidateSymbol.TrySingleDeclaration(cancellationToken, out FieldDeclarationSyntax fieldDeclaration) &&
                        fieldDeclaration.Declaration is VariableDeclarationSyntax fieldVariableDeclaration &&
                        fieldVariableDeclaration.Variables.TrySingle(out var fieldVariable) &&
                        fieldVariable.Initializer is EqualsValueClauseSyntax fieldInitializer)
                    {
                        result = fieldInitializer.Value;
                        return true;
                    }

                    result = null;
                    return false;
                }
            }
        }
    }
}
