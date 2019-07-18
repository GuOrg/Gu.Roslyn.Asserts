namespace Gu.Roslyn.Asserts.Tests.MetadataReferences
{
    using Microsoft.CodeAnalysis;
    using NUnit.Framework;

    public class MetaDataReferencesCollectionTests
    {
        [Test]
        public void AddWithAliasFirst()
        {
            var metaDataReferences = new MetadataReferencesCollection_();
            var withAliases = MetadataReference.CreateFromFile(typeof(object).Assembly.Location).WithAliases(new[] { "global", "mscorlib" });
            Assert.AreEqual(true, metaDataReferences.Add(withAliases));
            Assert.AreEqual(false, metaDataReferences.Add(MetadataReference.CreateFromFile(typeof(object).Assembly.Location)));
            CollectionAssert.AreEqual(new[] { withAliases }, metaDataReferences);
        }

        [Test]
        public void AddWithAliasLast()
        {
            var metaDataReferences = new MetadataReferencesCollection_();
            var withAliases = MetadataReference.CreateFromFile(typeof(object).Assembly.Location).WithAliases(new[] { "global", "mscorlib" });
            Assert.AreEqual(true, metaDataReferences.Add(MetadataReference.CreateFromFile(typeof(object).Assembly.Location)));

            Assert.AreEqual(true, metaDataReferences.Add(withAliases));
            CollectionAssert.AreEqual(new[] { withAliases }, metaDataReferences);
        }
    }
}
