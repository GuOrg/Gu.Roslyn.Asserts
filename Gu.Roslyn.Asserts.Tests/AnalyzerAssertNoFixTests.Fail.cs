// ReSharper disable RedundantNameQualifier
namespace Gu.Roslyn.Asserts.Tests
{
    using Gu.Roslyn.Asserts.Tests.CodeFixes;
    using NUnit.Framework;

    [TestFixture]
    public partial class AnalyzerAssertNoFixTests
    {
        public class Fail
        {
            [Test]
            public void FixDoesNotMatchAnalyzer()
            {
                var exception = Assert.Throws<NUnit.Framework.AssertionException>(() => AnalyzerAssert.NoFix<NoErrorAnalyzer, DontUseUnderscoreCodeFixProvider>((string)null, null));
                var expected = "Analyzer Gu.Roslyn.Asserts.Tests.NoErrorAnalyzer does not produce diagnostics fixable by Gu.Roslyn.Asserts.Tests.CodeFixes.DontUseUnderscoreCodeFixProvider.\r\n" +
                               "The analyzer produces the following diagnostics: {NoError}\r\n" +
                               "The code fix supports the following diagnostics: {SA1309}";
                Assert.AreEqual(expected, exception.Message);
            }

            [Test]
            public void SingleClassOneErrorCodeFixFixedTheCode()
            {
                var code = @"
namespace RoslynSandbox
{
    class Foo
    {
        private readonly int ↓_value;
    }
}";

                var exception = Assert.Throws<NUnit.Framework.AssertionException>(() => AnalyzerAssert.NoFix<FieldNameMustNotBeginWithUnderscore, DontUseUnderscoreCodeFixProvider>(code));
                var expected = "Expected the code fix to not change any document.\r\n" +
                               "Mismatch on line 6 of file Foo.cs\r\n" +
                               "Expected:         private readonly int _value;\r\n" +
                               "Actual:           private readonly int value;\r\n" +
                               "                                       ^\r\n";
                Assert.AreEqual(expected, exception.Message);
            }

            [Test]
            public void TwoClassesOneErrorCodeFixFixedTheCode()
            {
                var barCode = @"
namespace RoslynSandbox
{
    class Bar
    {
        private readonly int value;
    }
}";

                var code = @"
namespace RoslynSandbox
{
    class Foo
    {
        private readonly int ↓_value;
    }
}";

                var exception = Assert.Throws<NUnit.Framework.AssertionException>(() => AnalyzerAssert.NoFix<FieldNameMustNotBeginWithUnderscore, DontUseUnderscoreCodeFixProvider>(barCode, code));
                var expected = "Expected the code fix to not change any document.\r\n" +
                               "Mismatch on line 6 of file Foo.cs\r\n" +
                               "Expected:         private readonly int _value;\r\n" +
                               "Actual:           private readonly int value;\r\n" +
                               "                                       ^\r\n";
                Assert.AreEqual(expected, exception.Message);
            }
        }
    }
}