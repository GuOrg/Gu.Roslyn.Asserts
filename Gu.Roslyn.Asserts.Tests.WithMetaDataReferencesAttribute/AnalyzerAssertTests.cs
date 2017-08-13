namespace Gu.Roslyn.Asserts.Tests.WithMetadataReferencesAttribute
{
    using System.Collections.Immutable;
    using Gu.Roslyn.Asserts.Tests.WithMetadataReferencesAttribute.AnalyzersAndFixes;
    using Microsoft.CodeAnalysis;
    using NUnit.Framework;

    public class AnalyzerAssertTests
    {
        [Test]
        public void ResetMetadataReferences()
        {
            var expected = new[]
                           {
                               MetadataReference.CreateFromFile(typeof(object).Assembly.Location)
                                                .WithAliases(ImmutableArray.Create("global", "system"))
                           };
            CollectionAssert.AreEqual(expected, AnalyzerAssert.MetadataReferences);
            CollectionAssert.AreEqual(expected, MetadataReferencesAttribute.GetMetadataReferences());

            AnalyzerAssert.MetadataReferences.Clear();
            AnalyzerAssert.ResetMetadataReferences();
            CollectionAssert.AreEqual(expected, MetadataReferencesAttribute.GetMetadataReferences());
        }

        [Test]
        public void CodeFixSingleClassOneErrorCorrectFix()
        {
            var code = @"
namespace RoslynSandbox
{
    class Foo
    {
        private readonly int ↓_value;
    }
}";

            var fixedCode = @"
namespace RoslynSandbox
{
    class Foo
    {
        private readonly int value;
    }
}";
            AnalyzerAssert.CodeFix<FieldNameMustNotBeginWithUnderscore, DontUseUnderscoreCodeFixProvider>(code, fixedCode);
        }
    }
}
