namespace Gu.Roslyn.Asserts.Analyzers
{
    using System.Collections.Immutable;
    using System.Linq;
    using System.Threading;
    using Gu.Roslyn.AnalyzerExtensions;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using Microsoft.CodeAnalysis.Diagnostics;

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
                else if (StringArg.ShouldHavePosition(parameter))
                {
                    var args = StringArg.Create(argument, context.SemanticModel, context.CancellationToken);
                    if (args.TrySingle(x => x.HasPosition != false, out var stringArg) &&
                        stringArg.IdentifierName.Identifier.ValueText != parameter.Name)
                    {
                        context.ReportDiagnostic(
                            Diagnostic.Create(
                                GURA01NameOfLocalShouldMatchParameter.Descriptor,
                                stringArg.IdentifierName.GetLocation(),
                                ImmutableDictionary<string, string>.Empty.Add(nameof(IdentifierNameSyntax), parameter.Name),
                                stringArg.IdentifierName.Identifier.ValueText,
                                parameter.Name));
                    }

                    if (!args.TryFirst(x => x.HasPosition != false, out stringArg))
                    {
                        context.ReportDiagnostic(
                            Diagnostic.Create(
                                GURA02IndicateErrorPosition.Descriptor,
                                argument.GetLocation(),
                                additionalLocations: new[] { stringArg.Literal.GetLocation() }));
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

        private struct StringArg
        {
            public readonly IdentifierNameSyntax IdentifierName;

            public readonly LiteralExpressionSyntax Literal;

            private StringArg(IdentifierNameSyntax identifierName, LiteralExpressionSyntax literal)
            {
                this.IdentifierName = identifierName;
                this.Literal = literal;
            }

            internal bool? HasPosition => this.Literal?.Token.ValueText.Contains("↓");

            internal static bool ShouldHavePosition(IParameterSymbol parameter)
            {
                return parameter.Name == "before" ||
                       (parameter.ContainingSymbol.Name == "Diagnostics" && parameter.Name == "code");
            }

            internal static ImmutableArray<StringArg> Create(ArgumentSyntax argument, SemanticModel semanticModel, CancellationToken cancellationToken)
            {
                if (argument.Expression is IdentifierNameSyntax identifierName)
                {
                    return ImmutableArray.Create(new StringArg(identifierName, FindLiteral(identifierName)));
                }
                else if (TryGetInitializer(out var initializer) &&
                         !initializer.Expressions.TryFirst(x => !x.IsKind(SyntaxKind.IdentifierName), out _))
                {
                    return ImmutableArray.CreateRange(initializer.Expressions.Cast<IdentifierNameSyntax>().Select(x => new StringArg(x, FindLiteral(x))));
                }

                return ImmutableArray<StringArg>.Empty;

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

                LiteralExpressionSyntax FindLiteral(IdentifierNameSyntax local)
                {
                    if (semanticModel.TryGetSymbol(local, cancellationToken,
                            out ILocalSymbol candidateSymbol) &&
                        candidateSymbol.TrySingleDeclaration(cancellationToken,
                            out LocalDeclarationStatementSyntax localDeclaration) &&
                        localDeclaration.Declaration is VariableDeclarationSyntax variableDeclaration &&
                        variableDeclaration.Variables.TrySingle(out var variable) &&
                        variable.Initializer is EqualsValueClauseSyntax localInitializer &&
                        localInitializer.Value is LiteralExpressionSyntax literal &&
                        literal.IsKind(SyntaxKind.StringLiteralExpression))
                    {
                        return literal;
                    }

                    return null;
                }
            }
        }
    }
}
