// ReSharper disable UnusedVariable
namespace Gu.Roslyn.Asserts.Tests
{
    using System;
    using System.Reflection;
    using NUnit.Framework;

    public class Sandbox
    {
        [Test]
        public void ThrowAssertionExceptionUsingActivator()
        {
            var assemblyQualifiedName = typeof(AssertionException).AssemblyQualifiedName;
            Console.WriteLine(assemblyQualifiedName);
            var type = Type.GetType(
                "NUnit.Framework.AssertionException, nunit.framework, Version=3.6.1.0, Culture=neutral, PublicKeyToken=2638cd05610744eb",
                throwOnError: false);
            Assert.Throws<NUnit.Framework.AssertionException>(() => throw (Exception)Activator.CreateInstance(type, "Error"));
        }

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
    }
}
