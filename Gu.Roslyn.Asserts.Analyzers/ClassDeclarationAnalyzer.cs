namespace Gu.Roslyn.Asserts.Analyzers
{
    using System.Collections.Generic;
    using System.Collections.Immutable;
    using System.Threading;
    using Gu.Roslyn.AnalyzerExtensions;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using Microsoft.CodeAnalysis.Diagnostics;

    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class ClassDeclarationAnalyzer : DiagnosticAnalyzer
    {
        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get; } = ImmutableArray.Create(
            GURA04NameClassToMatchAsserts.Descriptor);

        public override void Initialize(AnalysisContext context)
        {
            context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
            context.EnableConcurrentExecution();
            context.RegisterSyntaxNodeAction(c => Handle(c), SyntaxKind.ClassDeclaration);
        }

        private static void Handle(SyntaxNodeAnalysisContext context)
        {
            if (context.Node is ClassDeclarationSyntax classDeclaration)
            {
                if (InvocationWalker.TryFindName(classDeclaration, context.SemanticModel, context.CancellationToken, out var name) &&
                    context.ContainingSymbol.Name != name)
                {
                    context.ReportDiagnostic(
                        Diagnostic.Create(
                            GURA04NameClassToMatchAsserts.Descriptor,
                            classDeclaration.Identifier.GetLocation(),
                            ImmutableDictionary<string, string>.Empty.Add(nameof(IdentifierNameSyntax), name),
                            context.ContainingSymbol.ToMinimalDisplayString(context.SemanticModel, classDeclaration.SpanStart),
                            name));
                }
            }
        }

        private sealed class InvocationWalker : PooledWalker<InvocationWalker>
        {
            private readonly List<InvocationExpressionSyntax> invocations = new List<InvocationExpressionSyntax>();

            private InvocationWalker()
            {
            }

            public override void VisitInvocationExpression(InvocationExpressionSyntax node)
            {
                if (node.Expression is MemberAccessExpressionSyntax memberAccess &&
                    memberAccess.Expression is IdentifierNameSyntax identifierName &&
                    identifierName.Identifier.ValueText == "RoslynAssert")
                {
                    this.invocations.Add(node);
                }

                base.VisitInvocationExpression(node);
            }

            internal static bool TryFindName(ClassDeclarationSyntax node, SemanticModel semanticModel, CancellationToken cancellationToken, out string name)
            {
                name = null;
                using (var walker = BorrowAndVisit(node, () => new InvocationWalker()))
                {
                    foreach (var invocation in walker.invocations)
                    {
                        if (invocation.TryGetMethodName(out var candidate))
                        {
                            if (name == null)
                            {
                                name = candidate;
                            }
                            else if (name != candidate)
                            {
                                return false;
                            }
                        }
                    }
                }

                return name != null;
            }

            /// <inheritdoc />
            protected override void Clear()
            {
                this.invocations.Clear();
            }
        }
    }
}
