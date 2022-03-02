#pragma warning disable CA1305 // Specify IFormatProvider
namespace Gu.Roslyn.Asserts.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using System.Text;

    using Gu.Roslyn.Asserts.Internals;

    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;

    using NUnit.Framework;

    [Script]
    public static class Dump
    {
        private static readonly Dictionary<Type, MethodInfo[]> TypeFactoryMethodMap =
            typeof(SyntaxFactory)
                .GetMethods(BindingFlags.Public | BindingFlags.Static)
                .Where(x => !x.Name.StartsWith("Parse", StringComparison.Ordinal) &&
                            typeof(CSharpSyntaxNode).IsAssignableFrom(x.ReturnType) &&
                            x.ReturnType.Name.StartsWith(x.Name, StringComparison.Ordinal))
                .OrderBy(x => x.Name)
                .ThenBy(x => x.GetParameters().Length)
                .GroupBy(x => x.ReturnType)
                .ToDictionary(x => x.Key, x => x.ToArray());

        [Test]
        public static void CodeGenNodes()
        {
            var stringBuilder = new StringBuilder();
            foreach (var kvp in TypeFactoryMethodMap)
            {
                var type = kvp.Key;
                var variable = char.ToLowerInvariant(type.Name[0]) + type.Name[1..];
                if (variable.EndsWith("Syntax", StringComparison.Ordinal))
                {
                    variable = variable[0..^6];
                }

                stringBuilder.AppendLine($"                case {type.Name} {variable}:");
                var candidates = kvp.Value;
                if (TryFindMethod(candidates, out var method))
                {
                    var parameters = method.GetParameters();
                    stringBuilder.AppendLine($"                    return this.AppendLine(\"SyntaxFactory.{method.Name}(\")")
                                 .AppendLine($"                               .PushIndent()");
                    for (var j = 0; j < parameters.Length; j++)
                    {
                        var parameter = parameters[j];
                        var property = Property(parameter);
                        var closeArg = j == parameters.Length - 1 ? ", closeArgumentList: true" : string.Empty;
                        stringBuilder.AppendLine($"                               .WriteArgument(\"{parameter.Name}\", {variable}.{property}{closeArg})");
                    }

                    stringBuilder.AppendLine("                               .PopIndent();");
                }
                else
                {
                    stringBuilder.AppendLine("                    throw new NotSupportedException();");
                }
            }

            var code = stringBuilder.ToString();
            Console.Write(code);

            static bool TryFindMethod(IEnumerable<MethodInfo> candidates, out MethodInfo result)
            {
                candidates = candidates.Where(x => x.GetParameters()
                                       .All(p => Property(p) != null));
                if (candidates.Any())
                {
                    result = Enumerable.MaxBy(candidates, x => x.GetParameters().Length)!;
                    return true;
                }

                result = default!;
                return false;
            }

            static string? Property(ParameterInfo parameter)
            {
                return parameter.Name switch
                {
                    "quoteKind" or "kind" => "Kind()",
                    _ => ((MethodInfo)parameter.Member).ReturnType.GetProperty(parameter.Name![..1].ToUpperInvariant() + parameter.Name[1..], BindingFlags.Public | BindingFlags.Instance)?.Name,
                };
            }
        }

        [Test]
        public static void CodeGenTokens()
        {
            var stringBuilder = new StringBuilder();
            foreach (var method in typeof(SyntaxFactory).GetMethods(BindingFlags.Public | BindingFlags.Static)
                                                        .Where(x => x.ReturnType == typeof(SyntaxToken))
                                                        .Distinct(MethodAndParameterNamesComparer.Default)
                                                        .OrderBy(x => x.Name)
                                                        .ThenBy(x => x.GetParameters().Length))
            {
                stringBuilder.AppendLine($"                case SyntaxKind.{method.Name}{When(method)}:");
                var parameters = method.GetParameters();
                if (parameters.Length == 1)
                {
                    var parameter = parameters[0];
                    var property = parameter.Name![..1].ToUpperInvariant() + parameter.Name[1..];
                    stringBuilder.AppendLine($"                    return this.Append($\"SyntaxFactory.{method.Name}({{token.{property}}})\");");
                }
                else
                {
                    stringBuilder.AppendLine($"                    return this.AppendLine(\"SyntaxFactory.{method.Name}(\")")
                                 .AppendLine($"                               .PushIndent()");
                    for (var i = 0; i < parameters.Length; i++)
                    {
                        var parameter = parameters[i];
                        var property = Property(parameter);
                        var closeArg = i == parameters.Length - 1 ? ", closeArgumentList: true" : string.Empty;
                        stringBuilder.AppendLine($"                               .WriteArgument(\"{parameter.Name}\", token.{property}{closeArg})");
                    }

                    stringBuilder.AppendLine("                               .PopIndent();");
                }
            }

            Console.WriteLine(stringBuilder.ToString());

            static string When(MethodInfo method)
            {
                if (method.GetParameters().Any(x => x.Name == "valueText"))
                {
                    return " when token.Text != token.ValueText";
                }

                if (method.GetParameters().All(x => x.Name != "leading"))
                {
                    return " when !token.HasLeadingTrivia && !token.HasTrailingTrivia";
                }

                return string.Empty;
            }

            static string Property(ParameterInfo parameter)
            {
                return parameter.Name switch
                {
                    "leading" => "LeadingTrivia",
                    "trailing" => "TrailingTrivia",
                    "contextualKind" or "kind" => "Kind()",
                    "text" => "Text",
                    "value" or "valueText" => "ValueText",
                    _ => parameter.Name![..1].ToUpperInvariant() + parameter.Name[1..],
                };
            }
        }

        [Test]
        public static void CodeGenTrivia()
        {
            var stringBuilder = new StringBuilder();
            foreach (var method in typeof(SyntaxFactory).GetMethods(BindingFlags.Public | BindingFlags.Static)
                                                        .Where(x => !x.IsSpecialName && x.ReturnType == typeof(SyntaxTrivia))
                                                        .OrderBy(x => x.Name))
            {
                if (method.GetParameters().TrySingle(out var parameter) &&
                    parameter.ParameterType == typeof(string))
                {
                    stringBuilder.AppendLine($"                case SyntaxKind.{method.Name}Trivia:")
                                 .AppendLine($"                    this.writer.Append($\"SyntaxFactory.{method.Name}(\").AppendQuotedEscaped(trivia.ToString()).Append(\")\");")
                                 .AppendLine($"                    return this;");
                }
                else
                {
                    stringBuilder.AppendLine(method.ToString());
                }
            }

            Console.WriteLine(stringBuilder.ToString());
        }

        [Test]
        public static void MethodsReturningToken()
        {
            foreach (var method in typeof(SyntaxFactory).GetMethods(BindingFlags.Public | BindingFlags.Static)
                                                        .Where(x => x.ReturnType == typeof(SyntaxToken))
                                                        .OrderBy(x => x.Name))
            {
                var parameters = method.GetParameters();
                var stringBuilder = new StringBuilder()
                             .AppendLine($"                    return this.AppendLine(\"SyntaxFactory.{method.Name}(\")")
                             .AppendLine($"                               .PushIndent()");
                for (var i = 0; i < parameters.Length; i++)
                {
                    var parameter = parameters[i];
                    var property = parameter.Name![..1].ToUpperInvariant() + parameter.Name[1..];
                    var closeArg = i == parameters.Length - 1 ? ", closeArgumentList: true" : string.Empty;
                    stringBuilder.AppendLine($"                               .WriteArgument(\"{parameter.Name}\", token.{property}{closeArg})");
                }

                stringBuilder.AppendLine("                               .PopIndent();");
                Console.WriteLine(stringBuilder.ToString());
            }
        }

        [Test]
        public static void DumpFactoryMethods()
        {
            foreach (var kvp in TypeFactoryMethodMap)
            {
                Console.WriteLine(kvp.Key.Name);
                foreach (var method in kvp.Value)
                {
                    Console.Write(" ");
                    Console.WriteLine($"SyntaxFactory.{method.Name}({string.Join(", ", method.GetParameters().Select(x => x.Name))})");
                }

                Console.WriteLine();
            }
        }

        [Test]
        public static void DumpTokenKinds()
        {
            foreach (var name in Enum.GetNames(typeof(SyntaxKind))
                                     .Where(x => x.EndsWith("Trivia", StringComparison.Ordinal))
                                     .OrderBy(x => x))
            {
                Console.WriteLine($"case SyntaxKind.{name}:");
            }
        }

        private sealed class MethodAndParameterNamesComparer : IEqualityComparer<MethodInfo>
        {
            internal static readonly IEqualityComparer<MethodInfo> Default = new MethodAndParameterNamesComparer();

            public bool Equals(MethodInfo? x, MethodInfo? y)
            {
                if (x is null && y is null)
                {
                    return true;
                }

                if (x is null || y is null)
                {
                    return false;
                }

                if (x.Name != y.Name)
                {
                    return false;
                }

                var xp = x.GetParameters();
                var yp = y.GetParameters();
                if (xp.Length != yp.Length)
                {
                    return false;
                }

                for (var i = 0; i < xp.Length; i++)
                {
                    if (xp[i].Name != yp[i].Name)
                    {
                        return false;
                    }
                }

                return true;
            }

            public int GetHashCode(MethodInfo obj)
            {
                return obj.Name.GetHashCode();
            }
        }
    }
}
