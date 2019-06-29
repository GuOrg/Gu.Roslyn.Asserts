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
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using NUnit.Framework;

    [Explicit("Script")]
    public static class Dump
    {
        private static readonly Type[] NodeTypes = typeof(CompilationUnitSyntax)
                                                   .Assembly.GetTypes()
                                                   .Where(x => typeof(CSharpSyntaxNode).IsAssignableFrom(x) && x.IsPublic)
                                                   .ToArray();

        private static readonly Dictionary<Type, MethodInfo[]> Methods = typeof(SyntaxFactory)
                                                                                   .GetMethods(BindingFlags.Public | BindingFlags.Static)
                                                                                   .Where(x => NodeTypes.Contains(x.ReturnType))
                                                                                   .OrderBy(x => x.Name)
                                                                                   .GroupBy(x => x.ReturnType)
                                                                                   .ToDictionary(x => x.Key, x => x.ToArray());

        [TestCaseSource(nameof(NodeTypes))]
        public static void CodeGenNodes(Type type)
        {
            if (Methods.TryGetValue(type, out var candidates))
            {
                var method = candidates.MaxBy(x => x.GetParameters().Length);
                var parameters = method.GetParameters();
                var variable = type.Name.Substring(0, 1).ToLower() + type.Name.Substring(1);
                if (variable.EndsWith("Syntax"))
                {
                    variable = variable.Substring(0, variable.Length - 6);
                }

                var stringBuilder = new StringBuilder()
                    .AppendLine($"                case {type.Name} {variable}:")
                    .AppendLine($"                    return this.AppendLine(\"SyntaxFactory.{method.Name}(\")")
                    .AppendLine($"                               .PushIndent()");
                foreach (var parameter in parameters)
                {
                    var commaOrParen = ReferenceEquals(parameter, parameters.Last()) ? ")" : ",";
                    stringBuilder.AppendLine($"                                .WriteArgument(\"{parameter.Name}\", {variable}.{parameter.Name.Substring(0, 1).ToUpper() + parameter.Name.Substring(1)}, \"{commaOrParen}\")");
                }

                stringBuilder.AppendLine("                               .PopIndent();");
                Console.Write(stringBuilder.ToString());
            }
            else
            {
                Assert.Inconclusive();
            }
        }

        [Test]
        public static void MethodsReturningToken()
        {
            foreach (var method in typeof(SyntaxFactory).GetMethods(BindingFlags.Public | BindingFlags.Static)
                                                        .Where(x => x.ReturnType == typeof(SyntaxToken))
                                                        .OrderBy(x => x.Name))
            {
                Console.WriteLine(method);
            }
        }
    }
}
