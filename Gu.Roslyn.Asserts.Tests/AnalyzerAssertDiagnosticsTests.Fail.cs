// ReSharper disable RedundantNameQualifier
namespace Gu.Roslyn.Asserts.Tests
{
    using System;
    using NUnit.Framework;

    [TestFixture]
    public partial class AnalyzerAssertDiagnosticsTests
    {
        public class Fail
        {
            [Test]
            public void NoErrorIndicatedGeneric()
            {
                var code = @"
namespace RoslynSandbox
{
    class Foo
    {
    }
}";
                var exception = Assert.Throws<NUnit.Framework.AssertionException>(() => AnalyzerAssert.Diagnostics<FieldNameMustNotBeginWithUnderscore>(code));
                var expected = "Expected code to have at least one error position indicated with '↓'";
                Assert.AreEqual(expected, exception.Message);
            }

            [TestCase(typeof(FieldNameMustNotBeginWithUnderscore))]
            [TestCase(typeof(NoErrorAnalyzer))]
            public void NoErrorIndicatedType(Type type)
            {
                var code = @"
namespace RoslynSandbox
{
    class Foo
    {
    }
}";
                var exception = Assert.Throws<NUnit.Framework.AssertionException>(() => AnalyzerAssert.Diagnostics(type, code));
                var expected = "Expected code to have at least one error position indicated with '↓'";
                Assert.AreEqual(expected, exception.Message);
            }

            [Test]
            public void NoErrorIndicatedPassingAnalyzer()
            {
                var code = @"
namespace RoslynSandbox
{
    class Foo
    {
    }
}";
                var exception = Assert.Throws<NUnit.Framework.AssertionException>(() => AnalyzerAssert.Diagnostics<FieldNameMustNotBeginWithUnderscore>(code));
                var expected = "Expected code to have at least one error position indicated with '↓'";
                Assert.AreEqual(expected, exception.Message);
            }

            [Test]
            public void NoErrorInCode()
            {
                var code = @"
namespace RoslynSandbox
{
    ↓class Foo
    {
    }
}";
                var exception = Assert.Throws<NUnit.Framework.AssertionException>(() => AnalyzerAssert.Diagnostics<FieldNameMustNotBeginWithUnderscore>(code));
                var expected = "Expected and actual diagnostics do not match.\r\n" +
                               "Expected: SA1309 at line 3 and character 4 in file Foo.cs |    ↓class Foo\r\n" +
                               "Actual:   <no errors>\r\n";
                Assert.AreEqual(expected, exception.Message);
            }

            [Test]
            public void TwoClassesNoErrorIndicated()
            {
                var code1 = @"
namespace RoslynSandbox
{
    class Code1
    {
    }
}";
                var code2 = @"
namespace RoslynSandbox
{
    class Code2
    {
    }
}";
                var exception = Assert.Throws<NUnit.Framework.AssertionException>(() => AnalyzerAssert.Diagnostics<FieldNameMustNotBeginWithUnderscore>(code1, code2));
                var expected = "Expected code to have at least one error position indicated with '↓'";
                Assert.AreEqual(expected, exception.Message);
            }

            [Test]
            public void TwoClassesNoErrorInCode()
            {
                var code1 = @"
namespace RoslynSandbox
{
    ↓class Code1
    {
    }
}";
                var code2 = @"
namespace RoslynSandbox
{
    class Code2
    {
    }
}";
                var exception = Assert.Throws<NUnit.Framework.AssertionException>(() => AnalyzerAssert.Diagnostics<FieldNameMustNotBeginWithUnderscore>(code1, code2));
                var expected = "Expected and actual diagnostics do not match.\r\n" +
                               "Expected: SA1309 at line 3 and character 4 in file Code1.cs |    ↓class Code1\r\n" +
                               "Actual:   <no errors>\r\n";
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

                var exception = Assert.Throws<NUnit.Framework.AssertionException>(() => AnalyzerAssert.Diagnostics<FieldNameMustNotBeginWithUnderscore>(code));
                var expected = "Expected and actual diagnostics do not match.\r\n" +
                               "Expected: SA1309 at line 5 and character 16 in file Foo.cs |        private ↓readonly int _value1;\r\n" +
                               "Actual:   SA1309 at line 5 and character 29 in file Foo.cs |        private readonly int ↓_value1;\r\n";
                Assert.AreEqual(expected, exception.Message);
            }

            [Test]
            public void TwoErrorsOnlyOneIndicated()
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

                var exception = Assert.Throws<NUnit.Framework.AssertionException>(() => AnalyzerAssert.Diagnostics<FieldNameMustNotBeginWithUnderscore>(code));
                var expected = "Expected and actual diagnostics do not match.\r\n" +
                               "Actual:   SA1309 at line 6 and character 29 in file Foo.cs |        private readonly int ↓_value2;\r\n";
                Assert.AreEqual(expected, exception.Message);
            }

            [Test]
            public void TwoClassesIndicatedAndActualPositionDoNotMatch()
            {
                var code1 = @"
namespace RoslynSandbox
{
    class Foo1
    {
        private readonly int _value1;
    }
}";

                var code2 = @"
namespace RoslynSandbox
{
    class Foo2
    {
        private readonly int ↓value2;
    }
}";

                var exception = Assert.Throws<NUnit.Framework.AssertionException>(() => AnalyzerAssert.Diagnostics<FieldNameMustNotBeginWithUnderscore>(code1, code2));
                var expected = "Expected and actual diagnostics do not match.\r\n" +
                               "Expected: SA1309 at line 5 and character 29 in file Foo2.cs |        private readonly int ↓value2;\r\n" +
                               "Actual:   SA1309 at line 5 and character 29 in file Foo1.cs |        private readonly int ↓_value1;\r\n";
                Assert.AreEqual(expected, exception.Message);
            }
        }
    }
}