namespace Gu.Roslyn.Asserts.Tests
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using Microsoft.CodeAnalysis;
    using NUnit.Framework;

    public class MetadataReferencesTests
    {
        [Test]
        public void TransitiveMscorlib()
        {
            var expected = new[] { "mscorlib.dll" };
            var type = typeof(object);
            CollectionAssert.AreEqual(expected, MetadataReferences.Transitive(type).Select(x => Path.GetFileName(x.Display)));
            CollectionAssert.AreEqual(expected, MetadataReferences.Transitive(type.Assembly).Select(x => Path.GetFileName(x.Display)));
        }

        [Test]
        public void TransitiveSystemCore()
        {
            var expected = new[]
                           {
                               "System.Core.dll",
                               "mscorlib.dll",
                               "System.dll",
                               "System.Configuration.dll",
                               "System.Xml.dll",
                               "System.Data.SqlXml.dll",
                               "System.Security.dll",
                               "System.Core.dll",
                               "System.Numerics.dll",
                           };
            var type = typeof(Enumerable);
            Dump(MetadataReferences.Transitive(type));
            CollectionAssert.AreEqual(expected, MetadataReferences.Transitive(type).Select(x => Path.GetFileName(x.Display)));
            CollectionAssert.AreEqual(expected, MetadataReferences.Transitive(type.Assembly).Select(x => Path.GetFileName(x.Display)));
        }

        // ReSharper disable once UnusedMember.Local
        private static void Dump(IEnumerable<MetadataReference> references)
        {
            foreach (var metadataReference in references)
            {
                Console.WriteLine($"\"{Path.GetFileName(metadataReference.Display)}\",");
            }
        }
    }
}
