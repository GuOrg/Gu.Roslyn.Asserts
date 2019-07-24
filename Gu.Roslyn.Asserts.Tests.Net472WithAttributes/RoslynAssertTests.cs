namespace Gu.Roslyn.Asserts.Tests.Net472WithAttributes
{
    using Gu.Roslyn.Asserts.Tests.Net472WithAttributes.AnalyzersAndFixes;
    using NUnit.Framework;

    public partial class RoslynAssertTests
    {
        [Test]
        public void CodeFixSingleClassOneErrorCorrectFix()
        {
            var before = @"
namespace N
{
    class C
    {
        private readonly int ↓_value;
    }
}";

            var after = @"
namespace N
{
    class C
    {
        private readonly int value;
    }
}";
            var analyzer = new FieldNameMustNotBeginWithUnderscore();
            var fix = new DoNotUseUnderscoreFix();
            RoslynAssert.CodeFix(analyzer, fix, before, after);
        }
    }
}
