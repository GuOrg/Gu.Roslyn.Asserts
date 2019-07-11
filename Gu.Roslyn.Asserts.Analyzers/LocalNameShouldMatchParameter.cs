namespace Gu.Roslyn.Asserts.Analyzers
{
    using System.Collections.Immutable;
    using Gu.Roslyn.AnalyzerExtensions;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using Microsoft.CodeAnalysis.Diagnostics;

    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class LocalNameShouldMatchParameter : DiagnosticAnalyzer
    {
        public const string DiagnosticId = "GURA01";

        public static readonly DiagnosticDescriptor Descriptor = new DiagnosticDescriptor(
            id: DiagnosticId,
            title: "Name of local should match parameter.",
            messageFormat: "Name of '{0}' should be '{1}'.",
            category: AnalyzerCategory.Ocd,
            defaultSeverity: DiagnosticSeverity.Hidden,
            isEnabledByDefault: true,
            description: "Name of local should match parameter for max consistency.");

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get; } =
            ImmutableArray.Create(Descriptor);

        public override void Initialize(AnalysisContext context)
        {
            context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
            context.EnableConcurrentExecution();
            context.RegisterSyntaxNodeAction(c => Handle(c), SyntaxKind.Argument);
        }

        private static void Handle(SyntaxNodeAnalysisContext context)
        {
            if (context.Node is ArgumentSyntax argument &&
                argument.Expression is IdentifierNameSyntax identifierName &&
                argument.Parent is ArgumentListSyntax argumentList &&
                argumentList.Parent is InvocationExpressionSyntax invocation &&
                context.SemanticModel.TryGetSymbol(invocation, context.CancellationToken, out var method) &&
                context.SemanticModel.TryGetSymbol(identifierName, context.CancellationToken, out ILocalSymbol local) &&
                method.ContainingType.Name == "RoslynAssert" &&
                method.TryFindParameter(argument, out var parameter) &&
                parameter.Name != local.Name &&
                !IsParams())
            {
                context.ReportDiagnostic(
                    Diagnostic.Create(
                        Descriptor,
                        identifierName.GetLocation(),
                        ImmutableDictionary<string, string>.Empty.Add(nameof(IdentifierNameSyntax), parameter.Name),
                        local.Name,
                        parameter.Name));
            }

            bool IsParams()
            {
                if (parameter.IsParams)
                {
                    return !invocation.TryFindArgument(parameter, out var arg);
                }

                return false;
            }
        }
    }
}
