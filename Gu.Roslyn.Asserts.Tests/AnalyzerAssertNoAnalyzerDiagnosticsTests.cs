namespace Gu.Roslyn.Asserts.Tests
{
    using System;
    using NUnit.Framework;

    public class AnalyzerAssertNoAnalyzerDiagnosticsTests
    {
        public class Fail
        {
            [Test]
            public void SingleClassFieldNameMustNotBeginWithUnderscore()
            {
                var code = @"
namespace RoslynSandbox
{
    class Foo
    {
        private readonly int _value = 1;
    }
}";
                var expected = "Expected no diagnostics, found:\r\n" +
                               "SA1309 Field '_value' must not begin with an underscore\r\n" +
                               "  at line 5 and character 29 in file Foo.cs | private readonly int ↓_value = 1;\r\n";

                var exception = Assert.Throws<AssertException>(() => AnalyzerAssert.Valid<FieldNameMustNotBeginWithUnderscore>(code));
                Assert.AreEqual(expected, exception.Message);

                exception = Assert.Throws<AssertException>(() => AnalyzerAssert.Valid(typeof(FieldNameMustNotBeginWithUnderscore), code));
                Assert.AreEqual(expected, exception.Message);

                exception = Assert.Throws<AssertException>(() => AnalyzerAssert.Valid(new FieldNameMustNotBeginWithUnderscore(), code));
                Assert.AreEqual(expected, exception.Message);
            }

            [Test]
            public void WhenAnalyzerThrows()
            {
                var code = @"
namespace RoslynSandbox
{
    class Foo
    {
    }
}";
                _ = Assert.Throws<NullReferenceException>(() => AnalyzerAssert.NoAnalyzerDiagnostics<ThrowingAnalyzer>(code));
                _ = Assert.Throws<NullReferenceException>(() => AnalyzerAssert.NoAnalyzerDiagnostics(typeof(ThrowingAnalyzer), code));
                _ = Assert.Throws<NullReferenceException>(() => AnalyzerAssert.NoAnalyzerDiagnostics(new ThrowingAnalyzer(), code));
            }
        }
    }
}
