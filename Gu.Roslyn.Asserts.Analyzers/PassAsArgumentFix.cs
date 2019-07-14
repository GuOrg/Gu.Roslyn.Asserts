namespace Gu.Roslyn.Asserts.Analyzers
{
    using System.Collections.Immutable;
    using System.Composition;
    using System.Threading;
    using System.Threading.Tasks;
    using Gu.Roslyn.AnalyzerExtensions;
    using Gu.Roslyn.CodeFixExtensions;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CodeFixes;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;

    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(PassAsArgumentFix))]
    [Shared]
    public class PassAsArgumentFix : DocumentEditorCodeFixProvider
    {
        public override ImmutableArray<string> FixableDiagnosticIds { get; } = ImmutableArray.Create("CS0618");

        protected override async Task RegisterCodeFixesAsync(DocumentEditorCodeFixContext context)
        {
            var syntaxRoot = await context.Document.GetSyntaxRootAsync(context.CancellationToken)
                                          .ConfigureAwait(false);
            var semanticModel = await context.Document.GetSemanticModelAsync(context.CancellationToken).ConfigureAwait(false);
            foreach (var diagnostic in context.Diagnostics)
            {
                if (syntaxRoot.TryFindNode(diagnostic, out InvocationExpressionSyntax invocation) &&
                    invocation.Expression is MemberAccessExpressionSyntax memberAccess &&
                    memberAccess.Name is GenericNameSyntax genericName &&
                    genericName.TypeArgumentList is TypeArgumentListSyntax typeArgumentList &&
                    semanticModel.TryGetSymbol(invocation, context.CancellationToken, out var method) &&
                    method.ContainingType.Name == "RoslynAssert" &&
                    invocation.TryFirstAncestor(out TypeDeclarationSyntax typeDeclaration) &&
                    semanticModel.TryGetSymbol(typeDeclaration, context.CancellationToken, out var containingType))
                {
                    if (typeArgumentList.Arguments.TrySingle(out var typeArg) &&
                        semanticModel.TryGetType(typeArg, context.CancellationToken, out var argType))
                    {
                        if (TryFindFieldOrProperty(containingType, argType, semanticModel, context.CancellationToken, out var fieldOrProperty))
                        {
                            context.RegisterCodeFix(
                                $"Use '{fieldOrProperty.Name}'.",
                                (editor, _) => editor.ReplaceNode(
                                    invocation,
                                    x => x.WithExpression(memberAccess.WithName(SyntaxFactory.IdentifierName(genericName.Identifier)))
                                          .WithArgumentList(PrependArgument(x.ArgumentList, fieldOrProperty))),
                                nameof(RenameFix),
                                diagnostic);
                        }
                        else if (TryFindAvailableFieldName(containingType, argType, out var name))
                        {
                            context.RegisterCodeFix(
                                $"Create and use field '{name}'.",
                                (editor, _) => editor.AddPrivateStaticField(typeDeclaration, argType, name)
                                                     .ReplaceNode(
                                                         invocation,
                                                         x => x.WithExpression(memberAccess.WithName(SyntaxFactory.IdentifierName(genericName.Identifier)))
                                                               .WithArgumentList(x.ArgumentList.WithArguments(x.ArgumentList.Arguments.Insert(0, SyntaxFactory.Argument(SyntaxFactory.IdentifierName(name)))))),
                                nameof(RenameFix),
                                diagnostic);
                        }
                    }
                    else if (typeArgumentList.Arguments.Count == 2 &&
                             semanticModel.TryGetType(typeArgumentList.Arguments[0], context.CancellationToken, out var arg0Type) &&
                             semanticModel.TryGetType(typeArgumentList.Arguments[1], context.CancellationToken, out var arg1Type))
                    {
                        if (TryFindFieldOrProperty(containingType, arg0Type, semanticModel, context.CancellationToken, out var fieldOrProperty0) &&
                            TryFindFieldOrProperty(containingType, arg1Type, semanticModel, context.CancellationToken, out var fieldOrProperty1))
                        {
                            context.RegisterCodeFix(
                                $"Use '{fieldOrProperty0.Name}' and '{fieldOrProperty1.Name}.",
                                (editor, _) => editor.ReplaceNode(
                                    invocation,
                                    x => x.WithExpression(memberAccess.WithName(SyntaxFactory.IdentifierName(genericName.Identifier)))
                                          .WithArgumentList(PrependArgument(PrependArgument(x.ArgumentList, fieldOrProperty1), fieldOrProperty0))),
                                nameof(RenameFix),
                                diagnostic);
                        }
                        else if (TryFindAvailableFieldName(containingType, arg0Type, out var name0) &&
                                 TryFindAvailableFieldName(containingType, arg1Type, out var name1))
                        {
                            context.RegisterCodeFix(
                                $"Create and use fields '{name0}' and '{name1}'.",
                                (editor, _) => editor.AddPrivateStaticField(typeDeclaration, arg0Type, name0)
                                                     .AddPrivateStaticField(typeDeclaration, arg1Type, name1)
                                                     .ReplaceNode(
                                                         invocation,
                                                         x => x.WithExpression(memberAccess.WithName(SyntaxFactory.IdentifierName(genericName.Identifier)))
                                                               .WithArgumentList(x.ArgumentList.WithArguments(x.ArgumentList.Arguments.Insert(0, SyntaxFactory.Argument(SyntaxFactory.IdentifierName(name1)))
                                                                                                                                     .Insert(0, SyntaxFactory.Argument(SyntaxFactory.IdentifierName(name0)))))),
                                nameof(RenameFix),
                                diagnostic);
                        }
                    }
                }
            }

            ArgumentListSyntax PrependArgument(ArgumentListSyntax argumentList, FieldOrProperty fieldOrProperty)
            {
                var expression = !fieldOrProperty.IsStatic && !semanticModel.UnderscoreFields()
                    ? (ExpressionSyntax)SyntaxFactory.MemberAccessExpression(
                        SyntaxKind.SimpleMemberAccessExpression,
                        SyntaxFactory.ThisExpression(),
                        SyntaxFactory.IdentifierName(fieldOrProperty.Name))
                    : SyntaxFactory.IdentifierName(fieldOrProperty.Name);
                return argumentList.WithArguments(argumentList.Arguments.Insert(0, SyntaxFactory.Argument(expression)));
            }

            bool TryFindAvailableFieldName(ITypeSymbol containingType, ITypeSymbol argType, out string result)
            {
                if (semanticModel.Compilation.GetTypeByMetadataName("Microsoft.CodeAnalysis.Diagnostics.DiagnosticAnalyzer") is ITypeSymbol analyzerType &&
                    argType.IsAssignableTo(analyzerType, semanticModel.Compilation))
                {
                    if (!containingType.TryFindFirstMember("Analyzer", out _))
                    {
                        result = "Analyzer";
                        return true;
                    }

                    if (!containingType.TryFindFirstMember(argType.Name, out _))
                    {
                        result = argType.Name;
                        return SyntaxFacts.IsValidIdentifier(result);
                    }
                }
                else if (semanticModel.Compilation.GetTypeByMetadataName("Microsoft.CodeAnalysis.CodeFixes.CodeFixProvider") is ITypeSymbol fixType &&
                    argType.IsAssignableTo(fixType, semanticModel.Compilation))
                {
                    if (!containingType.TryFindFirstMember("Fix", out _))
                    {
                        result = "Fix";
                        return true;
                    }

                    if (!containingType.TryFindFirstMember(argType.Name, out _))
                    {
                        result = argType.Name;
                        return SyntaxFacts.IsValidIdentifier(result);
                    }
                }

                result = null;
                return false;
            }
        }

        private static bool TryFindFieldOrProperty(ITypeSymbol containingType, ITypeSymbol argumentType, SemanticModel semanticModel, CancellationToken cancellationToken, out FieldOrProperty fieldOrProperty)
        {
            foreach (var member in containingType.GetMembers())
            {
                if (FieldOrProperty.TryCreate(member, out fieldOrProperty) &&
                    fieldOrProperty.Initializer(cancellationToken) is EqualsValueClauseSyntax initializer &&
                    semanticModel.TryGetType(initializer.Value, cancellationToken, out var candidate) &&
                    Equals(candidate, argumentType))
                {
                    return true;
                }
            }

            if (containingType.ContainingType is INamedTypeSymbol parent)
            {
                return TryFindFieldOrProperty(parent, argumentType, semanticModel, cancellationToken, out fieldOrProperty);
            }

            return false;
        }
    }
}
