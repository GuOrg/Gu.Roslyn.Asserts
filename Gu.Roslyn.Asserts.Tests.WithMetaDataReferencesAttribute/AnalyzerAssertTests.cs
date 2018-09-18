namespace Gu.Roslyn.Asserts.Tests.WithMetadataReferencesAttribute
{
    using Gu.Roslyn.Asserts.Tests.WithMetadataReferencesAttribute.AnalyzersAndFixes;
    using NUnit.Framework;

    public partial class AnalyzerAssertTests
    {
        [Test]
        public void ResetMetadataReferences()
        {
            CollectionAssert.IsNotEmpty(AnalyzerAssert.MetadataReferences);

            AnalyzerAssert.MetadataReferences.Clear();
            CollectionAssert.IsEmpty(AnalyzerAssert.MetadataReferences);

            AnalyzerAssert.ResetMetadataReferences();
            CollectionAssert.IsNotEmpty(AnalyzerAssert.MetadataReferences);
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
