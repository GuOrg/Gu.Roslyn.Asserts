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
            GURA04NameClassToMatchAsserts.Descriptor,
            GURA05NameFileToMatchClass.Descriptor);

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

                if (ShouldRenameFile(classDeclaration.SyntaxTree, context.ContainingSymbol as INamedTypeSymbol, out name))
                {
                    context.ReportDiagnostic(
                        Diagnostic.Create(
                            GURA05NameFileToMatchClass.Descriptor,
                            classDeclaration.Identifier.GetLocation(),
                            ImmutableDictionary<string, string>.Empty.Add(nameof(IdentifierNameSyntax), name),
                            context.ContainingSymbol.ToMinimalDisplayString(context.SemanticModel, classDeclaration.SpanStart),
                            name));
                }
            }

            bool ShouldRenameFile(SyntaxTree tree, INamedTypeSymbol type, out string result)
            {
                if (type == null ||
                    type.IsGenericType)
                {
                    result = null;
                    return false;
                }

                if (!EndsWith(tree.FilePath.Length - 1, "cs", out var offset))
                {
                    result = null;
                    return false;
                }

                if (!EndsWith(offset, type.MetadataName, out offset))
                {
                    return TryGetName(out result);
                }

                if (type.ContainingType is INamedTypeSymbol parent)
                {
                    if (parent.ContainingType != null ||
                        parent.IsGenericType)
                    {
                        result = null;
                        return false;
                    }

                    if (!EndsWith(offset, parent.Name, out _))
                    {
                        return TryGetName(out result);
                    }
                }

                result = null;
                return false;

                bool EndsWith(int oldOffset, string text, out int newOffset)
                {
                    var index = tree.FilePath.LastIndexOf(text, oldOffset);
                    if (index != offset - text.Length)
                    {
                        newOffset = 0;
                        return false;
                    }

                    newOffset = oldOffset - index - 1;
                    return true;
                }

                bool TryGetName(out string name)
                {
                    if (type.ContainingType is INamedTypeSymbol containing)
                    {
                        if (containing.IsGenericType ||
                            containing.ContainingType != null)
                        {
                            name = null;
                            return false;
                        }

                        name = $"{containing.Name}.{type.Name}";
                        return true;
                    }

                    name = type.Name;
                    return true;
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
