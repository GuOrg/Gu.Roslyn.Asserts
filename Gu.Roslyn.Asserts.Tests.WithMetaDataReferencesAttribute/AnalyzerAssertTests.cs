namespace Gu.Roslyn.Asserts.Tests.WithMetaDataReferencesAttribute
{
    using Gu.Roslyn.Asserts.Tests.WithMetaDataReferencesAttribute.AnalyzersAndFixes;
    using NUnit.Framework;

    public class AnalyzerAssertTests
    {
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
