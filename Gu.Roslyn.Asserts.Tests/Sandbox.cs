// ReSharper disable UnusedVariable
// ReSharper disable RedundantNameQualifier
namespace Gu.Roslyn.Asserts.Tests
{
    using System;
    using System.Reflection;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using NUnit.Framework;

    [Explicit("Sandbox")]
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

        [Test]
        public static void References()
        {
            var assembly = typeof(Sandbox).GetTypeInfo().Assembly;
            var referencedAssemblies = assembly.GetReferencedAssemblies();
        }
    }
}
