// ReSharper disable UnusedVariable
// ReSharper disable RedundantNameQualifier
namespace Gu.Roslyn.Asserts.Tests
{
    using System;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using NUnit.Framework;

    [Script]
    public static class Sandbox
    {
        [TestCase(typeof(SyntaxTriviaList))]
        [TestCase(typeof(CSharpSyntaxNode))]
        [TestCase(typeof(CompilationUnitSyntax))]
        public static void FindReferences(Type type)
        {
            foreach (var assembly in type.Assembly.GetReferencedAssemblies())
            {
                Console.WriteLine(assembly);
            }
        }
    }
}
