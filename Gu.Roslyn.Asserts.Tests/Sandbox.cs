// ReSharper disable UnusedVariable
// ReSharper disable RedundantNameQualifier
namespace Gu.Roslyn.Asserts.Tests
{
    using System;
    using System.Reflection;
    using NUnit.Framework;

    [Explicit("Sandbox")]
    public class Sandbox
    {
        [Test]
        public void FindReferences()
        {
            foreach (var assembly in typeof(Sandbox).Assembly.GetReferencedAssemblies())
            {
                Console.WriteLine(assembly);
            }
        }

        [Test]
        public void References()
        {
            var assembly = typeof(Sandbox).GetTypeInfo().Assembly;
            var referencedAssemblies = assembly.GetReferencedAssemblies();
        }
#pragma warning disable SA1313 // Parameter names must begin with lower-case letter
#pragma warning restore SA1313 // Parameter names must begin with lower-case letter
    }
}
