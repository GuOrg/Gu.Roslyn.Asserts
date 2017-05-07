namespace Gu.Roslyn.Asserts.Tests
{
    using NUnit.Framework;

    public partial class AnalyzerAssertTests
    {
        public class HappyPath
        {
            [Test]
            public void SingleClassNoError()
            {
                var code = @"
namespace Gu.Roslyn.Asserts.Tests
{
    class Foo
    {
    }
}";
                AnalyzerAssert.NoDiagnostics<NoErrorAnalyzer>(code);
            }

            [Test]
            public void TwoClassesNoError()
            {
                var code1 = @"
namespace Gu.Roslyn.Asserts.Tests
{
    class Code1
    {
    }
}";
                var code2 = @"
namespace Gu.Roslyn.Asserts.Tests
{
    class Code2
    {
    }
}";
                AnalyzerAssert.NoDiagnostics<NoErrorAnalyzer>(code1, code2);
            }

            [Test]
            public void SingleClassOneError()
            {
                var code = @"
namespace Gu.Roslyn.Asserts.Tests
{
    class Foo
    {
        public Foo()
        {
        }
    }
}";
                var exception = Assert.Throws<AssertException>(() => AnalyzerAssert.NoDiagnostics<ErrorOnCtorAnalyzer>(code));
                Assert.AreEqual("Foo.cs(6,9): warning ErrorOnCtor: Message format.", exception.Message);
                ////AnalyzerAssert.NoDiagnostics<ErrorOnCtorAnalyzer>(code);
            }
        }
    }
}