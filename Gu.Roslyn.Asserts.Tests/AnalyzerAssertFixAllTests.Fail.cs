// ReSharper disable RedundantNameQualifier
namespace Gu.Roslyn.Asserts.Tests
{
    using Gu.Roslyn.Asserts.Tests.CodeFixes;
    using NUnit.Framework;

    [TestFixture]
    public partial class AnalyzerAssertFixAllTests
    {
        public class Fail
        {
            [Test]
            public void SingleClassTwoErrorsOnlyOneIndicated()
            {
                var code = @"
namespace RoslynSandbox
{
    class Foo
    {
        private readonly int ↓_value1;
        private readonly int _value2;
    }
}";

                var exception = Assert.Throws<NUnit.Framework.AssertionException>(() => AnalyzerAssert.FixAll<FieldNameMustNotBeginWithUnderscore, DontUseUnderscoreCodeFixProvider>(code, null));
                var expected = "Expected and actual diagnostics do not match.\r\n" +
                               "Actual:   SA1309 at line 6 and character 29 in file Foo.cs |        private readonly int ↓_value2;\r\n";
                Assert.AreEqual(expected, exception.Message);
            }

            [Test]
            public void SingleClassOneErrorWrongPosition()
            {
                var code = @"
namespace RoslynSandbox
{
    class Foo
    {
        private ↓readonly int _value1;
    }
}";

                var exception = Assert.Throws<NUnit.Framework.AssertionException>(() => AnalyzerAssert.FixAll<FieldNameMustNotBeginWithUnderscore, DontUseUnderscoreCodeFixProvider>(code, null));
                var expected = "Expected and actual diagnostics do not match.\r\n" +
                               "Expected: SA1309 at line 5 and character 16 in file Foo.cs |        private ↓readonly int _value1;\r\n" +
                               "Actual:   SA1309 at line 5 and character 29 in file Foo.cs |        private readonly int ↓_value1;\r\n";
                Assert.AreEqual(expected, exception.Message);
            }

            [Test]
            public void FixDoesNotMatchAnalyzer()
            {
                var exception = Assert.Throws<NUnit.Framework.AssertionException>(() => AnalyzerAssert.FixAll<NoErrorAnalyzer, DontUseUnderscoreCodeFixProvider>((string)null, (string)null));
                var expected = "Analyzer Gu.Roslyn.Asserts.Tests.NoErrorAnalyzer does not produce diagnostics fixable by Gu.Roslyn.Asserts.Tests.CodeFixes.DontUseUnderscoreCodeFixProvider.\r\n" +
                               "The analyzer produces the following diagnostics: {NoError}\r\n" +
                               "The code fix supports the following diagnostics: {SA1309}";
                Assert.AreEqual(expected, exception.Message);
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
                var exception = Assert.Throws<NUnit.Framework.AssertionException>(() => AnalyzerAssert.FixAll<FieldNameMustNotBeginWithUnderscore, DontUseUnderscoreCodeFixProvider>(code, fixedCode));
                var expected = "Mismatch on line 6 of file Foo.cs\r\n" +
                               "Expected:         private readonly int bar;\r\n" +
                               "Actual:           private readonly int value;\r\n" +
                               "                                       ^\r\n";
                Assert.AreEqual(expected, exception.Message);
            }

            [Test]
            public void TwoClassesOneErrorErrorInFix()
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

                var fixedCode = @"
namespace RoslynSandbox
{
    class Foo
    {
        private readonly int bar;
    }
}";
                var exception = Assert.Throws<NUnit.Framework.AssertionException>(() => AnalyzerAssert.FixAll<FieldNameMustNotBeginWithUnderscore, DontUseUnderscoreCodeFixProvider>(new[] { barCode, code }, new[] { barCode, fixedCode }));
                var expected = "Mismatch on line 6 of file Foo.cs\r\n" +
                               "Expected:         private readonly int bar;\r\n" +
                               "Actual:           private readonly int value;\r\n" +
                               "                                       ^\r\n";
                Assert.AreEqual(expected, exception.Message);
            }

            [Test]
            public void IndicatedAndActualPositionDoNotMatch()
            {
                var code = @"
namespace RoslynSandbox
{
    class Foo
    {
        private ↓readonly int _value1;
    }
}";

                var exception = Assert.Throws<NUnit.Framework.AssertionException>(() => AnalyzerAssert.FixAll<FieldNameMustNotBeginWithUnderscore, DontUseUnderscoreCodeFixProvider>(code, null));
                var expected = "Expected and actual diagnostics do not match.\r\n" +
                               "Expected: SA1309 at line 5 and character 16 in file Foo.cs |        private ↓readonly int _value1;\r\n" +
                               "Actual:   SA1309 at line 5 and character 29 in file Foo.cs |        private readonly int ↓_value1;\r\n";
                Assert.AreEqual(expected, exception.Message);
            }
        }
    }
}