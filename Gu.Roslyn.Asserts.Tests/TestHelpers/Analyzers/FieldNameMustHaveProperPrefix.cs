namespace Gu.Roslyn.Asserts.Tests;

using System;
using System.Collections.Immutable;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

[DiagnosticAnalyzer(LanguageNames.CSharp)]
internal sealed class FieldNameMustHaveProperPrefix : DiagnosticAnalyzer
{
    private static readonly DiagnosticDescriptor Descriptor = new(
        "SA1309",
        "Field names must begin with proper prefix",
        "Field '{0}' must begin with '{1}'",
        "Naming",
        DiagnosticSeverity.Warning,
        isEnabledByDefault: true);

    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get; } = ImmutableArray.Create(Descriptor);

    /// <inheritdoc/>
    public override void Initialize(AnalysisContext context)
    {
        context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
        context.EnableConcurrentExecution();
        context.RegisterSyntaxNodeAction(x => HandleFieldDeclaration(x), SyntaxKind.FieldDeclaration);
    }

    private static void HandleFieldDeclaration(SyntaxNodeAnalysisContext context)
    {
        var syntax = (FieldDeclarationSyntax)context.Node;
        var variables = syntax.Declaration?.Variables;
        if (variables is null)
        {
            return;
        }

        if (!context.Options.AnalyzerConfigOptionsProvider
            .GetOptions(syntax.SyntaxTree)
            .TryGetValue("dotnet_diagnostic.SA1309.field_name_prefix", out var prefix))
        {
            prefix = "_";
        }

        foreach (var variableDeclarator in variables.Value)
        {
            var identifier = variableDeclarator.Identifier;
            if (identifier.IsMissing)
            {
                continue;
            }

            if (identifier.ValueText.StartsWith(prefix, StringComparison.Ordinal))
            {
                continue;
            }

            var name = identifier.ValueText;
            context.ReportDiagnostic(Diagnostic.Create(Descriptor, identifier.GetLocation(), name, prefix));
        }
    }
}
