// ReSharper disable UnusedVariable
// ReSharper disable RedundantNameQualifier
namespace Gu.Roslyn.Asserts.Tests
{
    using System;
    using System.Reflection;
    using NUnit.Framework;

    [Explicit("Sandbox")]
    public static class Sandbox
    {
        [Test]
        public static void FindReferences()
        {
            foreach (var assembly in typeof(Sandbox).Assembly.GetReferencedAssemblies())
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
