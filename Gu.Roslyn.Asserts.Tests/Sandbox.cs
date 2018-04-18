// ReSharper disable UnusedVariable
// ReSharper disable RedundantNameQualifier
namespace Gu.Roslyn.Asserts.Tests
{
    using System;
    using System.Linq;
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

        [Test]
        public void Test()
        {
            Touch(System.Data.AcceptRejectRule.None);
            Touch(System.Drawing.Brushes.AliceBlue);
            Touch(System.Runtime.Serialization.EmitTypeInformation.Always);
            Touch(System.Numerics.BigInteger.MinusOne);
            foreach (var assembly in typeof(Sandbox).Assembly.GetReferencedAssemblies().Select(Assembly.Load))
            {
                Console.WriteLine($"{assembly.GetName().Name} {assembly.ExportedTypes.First().FullName}");
            }
        }

        // ReSharper disable once UnusedParameter.Local
        private static void Touch(object _)
        {
        }
    }
}
