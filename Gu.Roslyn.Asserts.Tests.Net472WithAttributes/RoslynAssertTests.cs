namespace Gu.Roslyn.Asserts.Tests.Net472WithAttributes
{
    using Gu.Roslyn.Asserts.Tests.Net472WithAttributes.AnalyzersAndFixes;
    using NUnit.Framework;

    public partial class RoslynAssertTests
    {
        [Test]
        public void ResetMetadataReferences()
        {
            CollectionAssert.IsNotEmpty(RoslynAssert.MetadataReferences);

            RoslynAssert.MetadataReferences.Clear();
            CollectionAssert.IsEmpty(RoslynAssert.MetadataReferences);

            RoslynAssert.ResetMetadataReferences();
            CollectionAssert.IsNotEmpty(RoslynAssert.MetadataReferences);
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
            RoslynAssert.CodeFix<FieldNameMustNotBeginWithUnderscore, DontUseUnderscoreCodeFixProvider>(code, fixedCode);
        }
    }
}
