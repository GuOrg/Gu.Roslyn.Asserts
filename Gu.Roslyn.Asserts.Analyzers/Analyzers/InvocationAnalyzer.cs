// ReSharper disable PossiblyImpureMethodCallOnReadonlyVariable
namespace Gu.Roslyn.Asserts.Analyzers
{
    using System;
    using System.Collections.Immutable;
    using System.Diagnostics.CodeAnalysis;
    using System.Threading;
    using Gu.Roslyn.AnalyzerExtensions;
    using Gu.Roslyn.CodeFixExtensions;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using Microsoft.CodeAnalysis.Diagnostics;

    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    internal class InvocationAnalyzer : DiagnosticAnalyzer
    {
        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get; } = ImmutableArray.Create(
            Descriptors.GURA01NameShouldMatchParameter,
            Descriptors.GURA02IndicateErrorPosition,
            Descriptors.GURA03NameShouldMatchCode,
            Descriptors.GURA09UseStandardNames,
            Descriptors.GURA10UseLocal,
            Descriptors.GURA11ChainAssertReplace);

        public override void Initialize(AnalysisContext context)
        {
            context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
            context.EnableConcurrentExecution();
            context.RegisterSyntaxNodeAction(c => Handle(c), SyntaxKind.InvocationExpression);
        }

        private static void Handle(SyntaxNodeAnalysisContext context)
        {
            if (context.Node is InvocationExpressionSyntax invocation &&
                context.SemanticModel.TryGetSymbol(invocation, context.CancellationToken, out var method))
            {
                if (method.ContainingType == KnownSymbols.RoslynAssert)
                {
                    foreach (var parameter in method.Parameters)
                    {
                        if (IsCode(parameter))
                        {
                            if (StringArgument.TrySingle(invocation, parameter, context.SemanticModel, context.CancellationToken, out var single))
                            {
                                Handle(single, ImmutableArray<StringArgument>.Empty);
                            }
                            else if (StringArgument.TryMany(invocation, parameter, context.SemanticModel, context.CancellationToken, out var stringArgs))
                            {
                                foreach (var arg in stringArgs)
                                {
                                    Handle(arg, stringArgs);
                                }
                            }

                            void Handle(StringArgument stringArg, ImmutableArray<StringArgument> args)
                            {
                                if (ShouldIndicatePosition() &&
                                    stringArg.Value is { })
                                {
                                    context.ReportDiagnostic(
                                        Diagnostic.Create(
                                            Descriptors.GURA02IndicateErrorPosition,
                                            stringArg.Value.GetLocation()));
                                }

                                if (stringArg.Symbol is { } symbol)
                                {
                                    if (stringArg.TryGetNameFromCode(out var codeName) &&
                                        ShouldRename(stringArg.Symbol, codeName, out codeName))
                                    {
                                        context.ReportDiagnostic(
                                            Diagnostic.Create(
                                                Descriptors.GURA03NameShouldMatchCode,
                                                stringArg.SymbolIdentifier.GetLocation(),
                                                ImmutableDictionary<string, string>.Empty.Add(nameof(IdentifierNameSyntax), codeName),
                                                symbol.Name,
                                                codeName));
                                    }

                                    if (ShouldRenameToMatchParameter() &&
                                        ShouldRename(symbol, parameter.Name, out var parameterName))
                                    {
                                        context.ReportDiagnostic(
                                            Diagnostic.Create(
                                                Descriptors.GURA01NameShouldMatchParameter,
                                                stringArg.Expression.GetLocation(),
                                                ImmutableDictionary<string, string>.Empty.Add(nameof(IdentifierNameSyntax), parameterName),
                                                symbol.Name,
                                                parameterName));
                                    }

                                    if (stringArg.Symbol.IsEitherKind(SymbolKind.Field, SymbolKind.Property) &&
                                        stringArg.Value is { })
                                    {
                                        context.ReportDiagnostic(
                                            Diagnostic.Create(
                                                Descriptors.GURA10UseLocal,
                                                stringArg.Expression.GetLocation(),
                                                additionalLocations: new[] { stringArg.Value.GetLocation() },
                                                messageArgs: stringArg.Symbol.Name));
                                    }
                                }

                                bool IsPositionParameter()
                                {
                                    return parameter switch
                                    {
                                        { Name: "before" } => true,
                                        { Name: "code", ContainingSymbol: IMethodSymbol { Name: "Diagnostics" } } => true,
                                        _ => false,

                                    };
                                }

                                bool ShouldIndicatePosition()
                                {
                                    if (IsPositionParameter() &&
                                        stringArg.HasPosition == false &&
                                        stringArg.Symbol?.IsEitherKind(SymbolKind.Field, SymbolKind.Property) != true)
                                    {
                                        if (stringArg.Symbol?.Name == parameter.Name)
                                        {
                                            return true;
                                        }

                                        foreach (var a in args)
                                        {
                                            if (a.Symbol?.Kind == SymbolKind.Local &&
                                                (a.HasPosition != false ||
                                                 a.Symbol.Name == parameter.Name))
                                            {
                                                return false;
                                            }
                                        }

                                        return true;
                                    }

                                    return false;
                                }

                                bool ShouldRenameToMatchParameter()
                                {
                                    if (stringArg.Symbol?.Kind != SymbolKind.Local)
                                    {
                                        return false;
                                    }

                                    if (IsPositionParameter())
                                    {
                                        return stringArg.HasPosition == true &&
                                               args.TrySingle(x => x.HasPosition == true, out _);
                                    }

                                    if (stringArg.TryGetNameFromCode(out var codeName))
                                    {
                                        return !string.Equals(stringArg.Symbol.Name, codeName, StringComparison.OrdinalIgnoreCase);
                                    }

                                    return true;
                                }
                            }
                        }
                        else if (invocation.TryFindArgument(parameter, out var argument) &&
                                 context.SemanticModel.TryGetSymbol(argument.Expression, context.CancellationToken, out var symbol) &&
                                 ShouldRename(symbol, parameter.Name, out var newName) &&
                                 context.SemanticModel.TryGetType(argument.Expression, context.CancellationToken, out var type) &&
                                 ShouldRename(symbol, type.Name, out _))
                        {
                            context.ReportDiagnostic(
                                Diagnostic.Create(
                                    Descriptors.GURA01NameShouldMatchParameter,
                                    argument.Expression.GetLocation(),
                                    ImmutableDictionary<string, string>.Empty.Add(nameof(IdentifierNameSyntax), newName),
                                    symbol.Name,
                                    newName));
                        }
                    }
                }
                else if (method.Name == "AssertReplace" &&
                         ShouldChain(invocation, context.SemanticModel, context.CancellationToken, out var location, out var additionalLocation))
                {
                    context.ReportDiagnostic(
                        Diagnostic.Create(
                            Descriptors.GURA11ChainAssertReplace,
                            location,
                            additionalLocations: new[] { additionalLocation }));
                }
            }
        }

        private static bool IsCode(IParameterSymbol parameter)
        {
            return parameter switch
            {
                { Name: "code", ContainingSymbol: { Name: "Valid" } } => true,
                { Name: "code", ContainingSymbol: { Name: "Diagnostics" } } => true,
                { Name: "code", ContainingSymbol: { Name: "NoDiagnostics" } } => true,
                { Name: "before", ContainingSymbol: { Name: "CodeFix" } } => true,
                { Name: "after", ContainingSymbol: { Name: "CodeFix" } } => true,
                { Name: "before", ContainingSymbol: { Name: "Refactoring" } } => true,
                { Name: "after", ContainingSymbol: { Name: "Refactoring" } } => true,
                _ => false,
            };
        }

        private static bool ShouldRename(ISymbol symbol, string expectedName, [NotNullWhen(true)] out string? newName)
        {
            if (symbol.IsEitherKind(SymbolKind.Local, SymbolKind.Field, SymbolKind.Property) &&
                symbol.ContainingType.TypeKind != TypeKind.Enum &&
                symbol.Name.IndexOf(expectedName, StringComparison.OrdinalIgnoreCase) < 0)
            {
                if (symbol.IsStatic || IsConst())
                {
                    newName = expectedName.ToFirstCharUpper();
                }
                else if (char.IsUpper(expectedName[0]))
                {
                    newName = expectedName.ToFirstCharLower();
                }
                else
                {
                    newName = expectedName;
                }

                return true;
            }

            newName = null;
            return false;

            bool IsConst()
            {
                return symbol switch
                {
                    IFieldSymbol field => field.IsConst,
                    ILocalSymbol local => local.IsConst,
                    _ => false,
                };
            }
        }

        private static bool ShouldChain(InvocationExpressionSyntax invocation, SemanticModel semanticModel, CancellationToken cancellationToken, [NotNullWhen(true)] out Location? location, [NotNullWhen(true)] out Location? additionalLocation)
        {
            if (invocation.Expression is MemberAccessExpressionSyntax memberAccess &&
                memberAccess.Expression is IdentifierNameSyntax identifierName &&
                invocation.Parent is AssignmentExpressionSyntax assignment &&
                assignment.Left is IdentifierNameSyntax left &&
                left.Identifier.ValueText == identifierName.Identifier.ValueText &&
                StringArgument.Create(identifierName, semanticModel, cancellationToken) is StringArgument stringArgument &&
                stringArgument.Symbol?.Kind == SymbolKind.Local &&
                stringArgument.Value.IsKind(SyntaxKind.StringLiteralExpression))
            {
                location = memberAccess.Name.GetLocation();
                additionalLocation = stringArgument.Value!.GetLocation();
                return true;
            }

            location = null;
            additionalLocation = null;
            return false;
        }
    }
}
