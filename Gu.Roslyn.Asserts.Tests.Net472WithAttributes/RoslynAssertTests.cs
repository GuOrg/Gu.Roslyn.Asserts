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
            var before = @"
namespace RoslynSandbox
{
    class Foo
    {
        private readonly int ↓_value;
    }
}";

            var after = @"
namespace RoslynSandbox
{
    class Foo
    {
        private readonly int value;
    }
}";
            var analyzer = new FieldNameMustNotBeginWithUnderscore();
            var fix = new DontUseUnderscoreCodeFixProvider();
            RoslynAssert.CodeFix(analyzer, fix, before, after);
        }
    }
}
