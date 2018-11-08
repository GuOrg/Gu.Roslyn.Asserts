namespace Gu.Roslyn.Asserts.Tests.MetadataReferences
{
    using System;
    using NUnit.Framework;

    public class ReferenceAssemblyTests
    {
        [TestCase(typeof(int))]
        public void TryGet(Type type)
        {
            Assert.AreEqual(true, ReferenceAssembly.TryGet(type.Assembly, out var metadataReference));
            //Assert.AreEqual(true, ReferenceAssembly.TryGet(type.Assembly.Location, out metadataReference));
            //Assert.AreEqual(true, ReferenceAssembly.TryGet(Path.GetFileNameWithoutExtension(type.Assembly.Location), out metadataReference));
        }
    }
}
