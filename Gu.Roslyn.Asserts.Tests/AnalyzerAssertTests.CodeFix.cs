namespace Gu.Roslyn.Asserts.Tests
{
    using Gu.Roslyn.Asserts.Tests.CodeFixes;
    using NUnit.Framework;

    public partial class AnalyzerAssertTests
    {
        public class CodeFix
        {
            [Test]
            public void SingleClassOneErrorCorrectFix()
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
                AnalyzerAssert.CodeFix<FieldNameMustNotBeginWithUnderscore, SA1309CodeFixProvider>(code, fixedCode);
            }

            [Test]
            public void SingleClassOneErrorErrorInFix()
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
        private readonly int bar;
    }
}";
                var exception = Assert.Throws<NUnit.Framework.AssertionException>(() => AnalyzerAssert.CodeFix<FieldNameMustNotBeginWithUnderscore, SA1309CodeFixProvider>(code, fixedCode));
                var expected = "Mismatch on line 6 of file Foo.cs\r\n" +
                               "Expected:         private readonly int bar;\r\n" +
                               "Actual:           private readonly int value;\r\n" +
                               "                                       ^\r\n";
                Assert.AreEqual(expected, exception.Message);
            }
        }
    }
}