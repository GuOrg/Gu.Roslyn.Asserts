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
        private static readonly Dictionary<Type, MethodInfo[]> TypeFactoryMethodMap = typeof(SyntaxFactory)
                                                                                   .GetMethods(BindingFlags.Public | BindingFlags.Static)
                                                                                   .Where(x => !x.IsAbstract && typeof(CSharpSyntaxNode).IsAssignableFrom(typeof(CSharpSyntaxNode)))
                                                                                   .OrderBy(x => x.Name)
                                                                                   .GroupBy(x => x.ReturnType)
                                                                                   .ToDictionary(x => x.Key, x => x.ToArray());

        private static readonly Type[] NodeTypes = TypeFactoryMethodMap.Keys.ToArray();

        [Test]
        public static void CodeGenNodes()
        {
            var stringBuilder = new StringBuilder();
            foreach (var kvp in TypeFactoryMethodMap)
            {
                var type = kvp.Key;
                var candidates = kvp.Value;
                var method = candidates.MaxBy(x => x.GetParameters().Length);
                var parameters = method.GetParameters();
                var variable = type.Name.Substring(0, 1).ToLower() + type.Name.Substring(1);
                if (variable.EndsWith("Syntax"))
                {
                    variable = variable.Substring(0, variable.Length - 6);
                }

                stringBuilder.AppendLine($"                case {type.Name} {variable}:")
                             .AppendLine($"                    return this.AppendLine(\"SyntaxFactory.{method.Name}(\")")
                             .AppendLine($"                               .PushIndent()");
                for (var i = 0; i < parameters.Length; i++)
                {
                    var parameter = parameters[i];
                    var property = parameter.Name.Substring(0, 1).ToUpper() + parameter.Name.Substring(1);
                    var closeArg = i == parameters.Length - 1 ? ", closeArgumentList: true" : string.Empty;
                    stringBuilder.AppendLine($"                                .WriteArgument(\"{parameter.Name}\", {variable}.{property}{closeArg})");
                }

                stringBuilder.AppendLine("                               .PopIndent();");
            }

            var code = stringBuilder.ToString();
            Console.Write(code);
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
