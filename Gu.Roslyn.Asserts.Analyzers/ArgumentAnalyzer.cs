namespace Gu.Roslyn.Asserts.Analyzers
{
    using System;
    using System.Collections.Immutable;
    using System.Diagnostics;
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
            GURA01NameOfLocalShouldMatchParameter.Descriptor,
            GURA02IndicateErrorPosition.Descriptor);

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
                method.ContainingType.Name == "RoslynAssert" &&
                method.TryFindParameter(argument, out var parameter))
            {
                if (argument.Expression is IdentifierNameSyntax identifierName &&
                    context.SemanticModel.TryGetSymbol(identifierName, context.CancellationToken, out ILocalSymbol local) &&
                    parameter.Name != local.Name &&
                    !IsParams())
                {
                    context.ReportDiagnostic(
                        Diagnostic.Create(
                            GURA01NameOfLocalShouldMatchParameter.Descriptor,
                            identifierName.GetLocation(),
                            ImmutableDictionary<string, string>.Empty.Add(nameof(IdentifierNameSyntax), parameter.Name),
                            local.Name,
                            parameter.Name));
                }

                if (StringArg.ShouldHavePosition(parameter, out var message))
                {
                    var args = StringArg.CreateMany(argument, parameter, context.SemanticModel, context.CancellationToken);
                    if (StringArg.ShouldRename(argument, parameter, args, out var toRename))
                    {
                        context.ReportDiagnostic(
                            Diagnostic.Create(
                                GURA01NameOfLocalShouldMatchParameter.Descriptor,
                                toRename.GetLocation(),
                                ImmutableDictionary<string, string>.Empty.Add(nameof(IdentifierNameSyntax), parameter.Name),
                                toRename.Identifier.ValueText,
                                parameter.Name));
                    }

                    if (StringArg.ShouldIndicatePosition(argument, parameter, args, out var location, out var additionalLocation))
                    {
                        context.ReportDiagnostic(
                            Diagnostic.Create(
                                GURA02IndicateErrorPosition.Descriptor,
                                location,
                                messageArgs: message,
                                additionalLocations: additionalLocation == null ? Array.Empty<Location>() : new[] { additionalLocation }));
                    }
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
            private readonly ILocalSymbol local;
#pragma warning restore RS1008 // Avoid storing per-compilation data into the fields of a diagnostic analyzer.
            private readonly ExpressionSyntax value;

            private StringArg(ExpressionSyntax identifierName, ILocalSymbol local, ExpressionSyntax value)
            {
                this.expression = identifierName;
                this.local = local;
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

            internal static bool ShouldHavePosition(IParameterSymbol parameter, out string message)
            {
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

            internal static bool ShouldIndicatePosition(ArgumentSyntax argument, IParameterSymbol parameter, ImmutableArray<StringArg> args, out Location location, out Location additionalLocation)
            {
                if (args.TryFirst(x => x.HasPosition == true, out _) ||
                    args.TryFirst(x => x.local != null && x.HasPosition == null, out _))
                {
                    location = null;
                    additionalLocation = null;
                    return false;
                }

                if (args.TrySingle(x => x.local != null && x.HasPosition == false, out var match))
                {
                    location = match.expression.GetLocation();
                    additionalLocation = match.value.GetLocation();
                    return argument.Contains(match.expression);
                }

                if (args.TrySingle(x => x.local?.Name == parameter.Name && x.HasPosition == false, out match))
                {
                    location = match.expression.GetLocation();
                    additionalLocation = match.value.GetLocation();
                    return argument.Contains(match.expression);
                }

                if (args.TryFirst(x => x.local != null && x.HasPosition == false, out match))
                {
                    location = argument.GetLocation();
                    additionalLocation = null;
                    return true;
                }

                location = null;
                additionalLocation = null;
                return false;
            }

            internal static bool ShouldRename(ArgumentSyntax argument, IParameterSymbol parameter, ImmutableArray<StringArg> args, out IdentifierNameSyntax identifierName)
            {
                if (args.TrySingle(x => x.local != null, out var match) &&
                    argument.Contains(match.expression))
                {
                    identifierName = (IdentifierNameSyntax)match.expression;
                    return identifierName.Identifier.ValueText != parameter.Name;
                }

                if (args.TrySingle(x => x.local != null && x.HasPosition == true, out match) &&
                    argument.Contains(match.expression))
                {
                    identifierName = (IdentifierNameSyntax)match.expression;
                    return identifierName.Identifier.ValueText != parameter.Name;
                }

                identifierName = null;
                return false;
            }

            private static StringArg Create(ExpressionSyntax expression, SemanticModel semanticModel, CancellationToken cancellationToken)
            {
                if (expression is IdentifierNameSyntax candidate &&
                    semanticModel.TryGetSymbol(candidate, cancellationToken, out ILocalSymbol candidateSymbol))
                {
                    _ = TryGetValue(out var literal);
                    return new StringArg(expression, candidateSymbol, literal);
                }

                return new StringArg(expression, null, null);

                bool TryGetValue(out ExpressionSyntax result)
                {
                    if (candidateSymbol.TrySingleDeclaration(cancellationToken, out LocalDeclarationStatementSyntax localDeclaration) &&
                        localDeclaration.Declaration is VariableDeclarationSyntax variableDeclaration &&
                        variableDeclaration.Variables.TrySingle(out var variable) &&
                        variable.Initializer is EqualsValueClauseSyntax initializer)
                    {
                        result = initializer.Value;
                        return true;
                    }

                    result = null;
                    return false;
                }
            }
        }
    }
}
