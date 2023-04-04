namespace Gu.Roslyn.Asserts.Analyzers;

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;
using Gu.Roslyn.AnalyzerExtensions;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

[DiagnosticAnalyzer(LanguageNames.CSharp)]
internal class ClassDeclarationAnalyzer : DiagnosticAnalyzer
{
    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get; } = ImmutableArray.Create(
        Descriptors.GURA04NameClassToMatchAsserts,
        Descriptors.GURA05NameFileToMatchClass,
        Descriptors.GURA07TestClassShouldBePublicStatic);

    public override void Initialize(AnalysisContext context)
    {
        context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
        context.EnableConcurrentExecution();
        context.RegisterSyntaxNodeAction(c => Handle(c), SyntaxKind.ClassDeclaration);
    }

    private static void Handle(SyntaxNodeAnalysisContext context)
    {
        if (context.Node is ClassDeclarationSyntax classDeclaration &&
            context.ContainingSymbol is INamedTypeSymbol type)
        {
            if (InvocationWalker.TryFindName(classDeclaration, out var name) &&
                type.Name != name &&
                type.ContainingType?.Name != name)
            {
                context.ReportDiagnostic(
                    Diagnostic.Create(
                        Descriptors.GURA04NameClassToMatchAsserts,
                        classDeclaration.Identifier.GetLocation(),
                        ImmutableDictionary<string, string?>.Empty.Add(nameof(IdentifierNameSyntax), name),
                        context.ContainingSymbol.ToMinimalDisplayString(context.SemanticModel, classDeclaration.SpanStart),
                        name));
            }

            if (ShouldRenameFile(classDeclaration.SyntaxTree, type, out name))
            {
                context.ReportDiagnostic(
                    Diagnostic.Create(
                        Descriptors.GURA05NameFileToMatchClass,
                        classDeclaration.Identifier.GetLocation(),
                        ImmutableDictionary<string, string?>.Empty.Add(nameof(IdentifierNameSyntax), name),
                        context.ContainingSymbol.ToMinimalDisplayString(context.SemanticModel, classDeclaration.SpanStart),
                        name));
            }

            if ((!type.IsStatic || type.DeclaredAccessibility != Accessibility.Public) &&
                UsingDirectiveWalker.IsUsingNUnit(context.SemanticModel.SyntaxTree) &&
                type.BaseType == KnownSymbols.Object &&
                type.Interfaces.IsDefaultOrEmpty &&
                type.TryFindFirstMethod(x => x.TryGetAttribute(KnownSymbols.NUnitTestAttribute, out _) || x.TryGetAttribute(KnownSymbols.NUnitTestCaseAttribute, out _), out _))
            {
                context.ReportDiagnostic(
                    Diagnostic.Create(
                        Descriptors.GURA07TestClassShouldBePublicStatic,
                        classDeclaration.Identifier.GetLocation(),
                        type.ToMinimalDisplayString(context.SemanticModel, classDeclaration.SpanStart)));
            }
        }
    }

    private static bool ShouldRenameFile(SyntaxTree tree, INamedTypeSymbol type, [NotNullWhen(true)] out string? result)
    {
        if (type is null ||
            type.IsGenericType)
        {
            result = null;
            return false;
        }

#pragma warning disable IDE0059 // Unnecessary assignment of a value
        var offset = -1;
#pragma warning restore IDE0059 // Unnecessary assignment of a value
        if (!EndsWith(tree.FilePath.Length - 1, "cs", out offset))
        {
            result = null;
            return false;
        }

        if (!EndsWith(offset, type.MetadataName, out offset))
        {
            return TryGetName(out result);
        }

        if (type.ContainingType is { IsGenericType: false, ContainingType: null } containingType)
        {
            if (!EndsWith(offset, containingType.Name, out _))
            {
                return TryGetName(out result);
            }
        }

        result = null;
        return false;

        bool EndsWith(int oldOffset, string text, out int newOffset)
        {
            var index = tree.FilePath.LastIndexOf(text, oldOffset, StringComparison.Ordinal);
            if (index != offset - text.Length)
            {
                newOffset = 0;
                return false;
            }

            newOffset = oldOffset - index - 1;
            return true;
        }

        bool TryGetName(out string? name)
        {
            if (type.ContainingType is { IsGenericType: false, ContainingType: null } containingType)
            {
                name = $"{containingType.Name}.{type.Name}";
                return true;
            }

            name = type.Name;
            return true;
        }
    }

    private sealed class UsingDirectiveWalker : PooledWalker<UsingDirectiveWalker>
    {
        private readonly List<UsingDirectiveSyntax> usingDirectives = new();

        public override void VisitUsingDirective(UsingDirectiveSyntax node)
        {
            this.usingDirectives.Add(node);
            base.VisitUsingDirective(node);
        }

        public override void VisitClassDeclaration(ClassDeclarationSyntax node)
        {
            // Stop walking here
        }

        public override void VisitStructDeclaration(StructDeclarationSyntax node)
        {
            // Stop walking here
        }

        internal static bool IsUsingNUnit(SyntaxTree tree)
        {
            if (tree.TryGetRoot(out var root))
            {
                using var walker = BorrowAndVisit(root, () => new UsingDirectiveWalker());
                foreach (var directive in walker.usingDirectives)
                {
                    if (directive.Name is QualifiedNameSyntax { Left: IdentifierNameSyntax { Identifier.ValueText: "NUnit" }, Right: IdentifierNameSyntax { Identifier.ValueText: "Framework" } })
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        protected override void Clear()
        {
            this.usingDirectives.Clear();
        }
    }
}
