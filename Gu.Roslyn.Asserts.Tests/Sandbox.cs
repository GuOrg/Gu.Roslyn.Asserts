namespace Gu.Roslyn.Asserts.Tests
{
    using System;
    using NUnit.Framework;

    public class Sandbox
    {
        [Test]
        public void Meh()
        {
            var assemblyQualifiedName = typeof(AssertionException).AssemblyQualifiedName;
            Console.WriteLine(assemblyQualifiedName);
            var type = Type.GetType(
                assemblyQualifiedName,
                false);
            throw (Exception)Activator.CreateInstance(type, "Error");
        }
    }
}
