namespace Gu.Roslyn.Asserts.Analyzers
{
    using System;
    using System.Collections.Immutable;
    using System.Text.RegularExpressions;
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
            Descriptors.NameOfLocalShouldMatchParameter,
            Descriptors.IndicateErrorPosition,
            Descriptors.NameToFirstClass);

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
                method.ContainingType == KnownSymbol.RoslynAssert &&
                method.TryFindParameter(argument, out var parameter))
            {
                if (argument.Expression is IdentifierNameSyntax identifierName &&
                    context.SemanticModel.TryGetSymbol(identifierName, context.CancellationToken, out ILocalSymbol local) &&
                    parameter.Name != local.Name &&
                    !IsParams())
                {
                    context.ReportDiagnostic(
                        Diagnostic.Create(
                            Descriptors.NameOfLocalShouldMatchParameter,
                            identifierName.GetLocation(),
                            ImmutableDictionary<string, string>.Empty.Add(nameof(IdentifierNameSyntax), parameter.Name),
                            local.Name,
                            parameter.Name));
                }

                var args = ArgumentInfo.CreateMany(argument, parameter, context.SemanticModel, context.CancellationToken);
                if (ShouldRename(argument, parameter, args, out var toRename, out var descriptor, out var newName))
                {
                    context.ReportDiagnostic(
                        Diagnostic.Create(
                            descriptor,
                            toRename.GetLocation(),
                            ImmutableDictionary<string, string>.Empty.Add(nameof(IdentifierNameSyntax), newName),
                            toRename.Identifier.ValueText,
                            newName));
                }

                if (ShouldIndicatePosition(argument, parameter, args, out var location, out var additionalLocation, out var message))
                {
                    context.ReportDiagnostic(
                        Diagnostic.Create(
                            Descriptors.IndicateErrorPosition,
                            location,
                            messageArgs: message,
                            additionalLocations: additionalLocation == null ? Array.Empty<Location>() : new[] { additionalLocation }));
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

        private static bool ShouldIndicatePosition(ArgumentSyntax argument, IParameterSymbol parameter, ImmutableArray<ArgumentInfo> args, out Location location, out Location additionalLocation, out string message)
        {
            if (IsPositionArgument(argument, parameter, args, out message))
            {
                if (args.TryFirst(x => x.HasPosition == true, out _) ||
                    args.TryFirst(x => x.Symbol is ILocalSymbol && x.HasPosition == null, out _))
                {
                    location = null;
                    additionalLocation = null;
                    message = null;
                    return false;
                }

                if (args.TrySingle(x => x.Symbol != null && x.HasPosition == false, out var match))
                {
                    location = match.Expression.GetLocation();
                    additionalLocation = match.Value.GetLocation();
                    return argument.Contains(match.Expression);
                }

                if (args.TrySingle(x => x.Symbol?.Name == parameter.Name && x.HasPosition == false, out match))
                {
                    location = match.Expression.GetLocation();
                    additionalLocation = match.Value.GetLocation();
                    return argument.Contains(match.Expression);
                }

                if (args.TryFirst(x => x.Symbol != null && x.HasPosition == false, out match))
                {
                    location = argument.GetLocation();
                    additionalLocation = match.Value.GetLocation();
                    return true;
                }
            }

            location = null;
            additionalLocation = null;
            message = null;
            return false;
        }

        private static bool ShouldRename(ArgumentSyntax argument, IParameterSymbol parameter, ImmutableArray<ArgumentInfo> args, out IdentifierNameSyntax identifierName, out DiagnosticDescriptor descriptor, out string newName)
        {
            if (IsPositionArgument(argument, parameter, args, out _))
            {
                if (args.TrySingle(x => x.Symbol != null, out var match) &&
                    argument.Contains(match.Expression))
                {
                    identifierName = (IdentifierNameSyntax)match.Expression;
                    descriptor = Descriptors.NameOfLocalShouldMatchParameter;
                    newName = parameter.Name;
                    return !IsMatch(identifierName, parameter.Name);
                }

                if (args.TrySingle(x => x.Symbol != null && x.HasPosition == true, out match) &&
                    argument.Contains(match.Expression))
                {
                    identifierName = (IdentifierNameSyntax)match.Expression;
                    descriptor = Descriptors.NameOfLocalShouldMatchParameter;
                    newName = parameter.Name;
                    return !IsMatch(identifierName, parameter.Name);
                }
            }

            if (args.Length > 1 &&
                args.TrySingle(x => x.Expression == argument.Expression, out var argumentInfo) &&
                TryGetClassName(argumentInfo, out identifierName, out newName) &&
                argumentInfo.Symbol.Name != newName)
            {
                descriptor = Descriptors.NameToFirstClass;
                return true;
            }

            identifierName = null;
            newName = null;
            descriptor = null;
            return false;

            bool TryGetClassName(ArgumentInfo arg, out IdentifierNameSyntax identifier, out string name)
            {
                if (arg.Expression is IdentifierNameSyntax candidate &&
                    arg.Value is LiteralExpressionSyntax literal &&
                    Regex.Match(literal.Token.ValueText, @"^ *(↓?(public|internal|static|sealed|abstract) )*↓?(class|struct|enum|interface) ↓?(?<name>\w+)(<(?<type>↓?\w+)(, ?(?<type>↓?\w+))*>)?", RegexOptions.ExplicitCapture | RegexOptions.Multiline) is Match match &&
                    match.Success &&
                    !match.Groups["type"].Success &&
                    !IsMatch(candidate, match.Groups["name"].Value))
                {
                    identifier = candidate;
                    name = match.Groups["name"].Value;
                    return true;
                }

                identifier = null;
                name = null;
                return false;
            }

            bool IsMatch(IdentifierNameSyntax identifier, string expected)
            {
                if (identifier.Identifier.ValueText.Length != expected.Length)
                {
                    return false;
                }

                if (char.ToLowerInvariant(identifier.Identifier.ValueText[0]) != char.ToLowerInvariant(expected[0]))
                {
                    return false;
                }

                for (var i = 1; i < expected.Length; i++)
                {
                    if (identifier.Identifier.ValueText[i] != expected[i])
                    {
                        return false;
                    }
                }

                return true;
            }
        }

        private static bool IsPositionArgument(ArgumentSyntax argument, IParameterSymbol parameter, ImmutableArray<ArgumentInfo> args, out string message)
        {
            if (args.TrySingle(x => x.Expression == argument.Expression, out var match) &&
               match.Symbol?.Kind != SymbolKind.Local)
            {
                message = null;
                return false;
            }

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
    }
}
