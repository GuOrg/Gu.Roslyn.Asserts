namespace Gu.Roslyn.Asserts.Analyzers
{
    using System.Collections.Immutable;
    using Gu.Roslyn.AnalyzerExtensions;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using Microsoft.CodeAnalysis.Diagnostics;

    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class ArgumentAnalyzer : DiagnosticAnalyzer
    {
        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get; } =
            ImmutableArray.Create(GURA01NameOfLocalShouldMatchParameter.Descriptor);

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
                else if (ShouldHavePosition() &&
                         argument.Expression is ImplicitArrayCreationExpressionSyntax arrayCreation &&
                         arrayCreation.Initializer is InitializerExpressionSyntax arrayInitializer &&
                         arrayInitializer.Expressions.TrySingleOfType(x => HasPosition(x), out IdentifierNameSyntax identifierNameSyntax) &&
                         identifierNameSyntax.Identifier.ValueText != parameter.Name)
                {
                    context.ReportDiagnostic(
                        Diagnostic.Create(
                            GURA01NameOfLocalShouldMatchParameter.Descriptor,
                            identifierNameSyntax.GetLocation(),
                            ImmutableDictionary<string, string>.Empty.Add(nameof(IdentifierNameSyntax), parameter.Name),
                            identifierNameSyntax.Identifier.ValueText,
                            parameter.Name));
                }

                bool IsParams()
                {
                    if (parameter.IsParams)
                    {
                        return !invocation.TryFindArgument(parameter, out _);
                    }

                    return false;
                }

                bool ShouldHavePosition()
                {
                    return parameter.Name == "before" ||
                           (method.Name == "Diagnostics" && parameter.Name == "code");
                }

                bool HasPosition(ExpressionSyntax expression)
                {
                    return expression is IdentifierNameSyntax candidate &&
                           context.SemanticModel.TryGetSymbol(candidate, context.CancellationToken, out ILocalSymbol candidateSymbol) &&
                           candidateSymbol.TrySingleDeclaration(context.CancellationToken, out LocalDeclarationStatementSyntax localDeclaration) &&
                           localDeclaration.Declaration is VariableDeclarationSyntax variableDeclaration &&
                           variableDeclaration.Variables.TrySingle(out var variable) &&
                           variable.Initializer is EqualsValueClauseSyntax localInitializer &&
                           localInitializer.Value is LiteralExpressionSyntax literal &&
                           literal.Token.ValueText.Contains("↓");
                }
            }
        }
    }
}
