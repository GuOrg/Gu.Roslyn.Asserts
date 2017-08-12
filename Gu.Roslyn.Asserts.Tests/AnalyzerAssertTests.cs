namespace Gu.Roslyn.Asserts.Tests
{
    using Microsoft.CodeAnalysis;
    using NUnit.Framework;

    public class AnalyzerAssertTests
    {
        [Test]
        public void ResetMetadataReferences()
        {
            AnalyzerAssert.MetadataReferences.Add(MetadataReference.CreateFromFile(typeof(int).Assembly.Location));
            AnalyzerAssert.ResetMetadataReferences();
            CollectionAssert.AreEqual(AnalyzerAssert.MetadataReferences, MetaDataReferencesAttribute.GetMetaDataReferences());
        }
    }
}