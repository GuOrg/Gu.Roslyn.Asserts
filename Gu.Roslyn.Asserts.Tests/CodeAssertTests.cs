namespace Gu.Roslyn.Asserts.Tests
{
    using NUnit.Framework;

    public class CodeAssertTests
    {
        [Test]
        public void WhenEqual()
        {
            var expected = @"
namespace RoslynSandbox
{
    class Foo
    {
        private readonly int _value;
    }
}";

            var actual = @"
namespace RoslynSandbox
{
    class Foo
    {
        private readonly int _value;
    }
}";
            CodeAssert.AreEqual(expected, actual);
        }

        [Test]
        public void WhenNotEqual()
        {
            var expectedCode = @"
namespace RoslynSandbox
{
    class Foo
    {
        private readonly int _value;
    }
}";

            var actualCode = @"
namespace RoslynSandbox
{
    class Foo
    {
        private readonly int bar;
    }
}";
            var exception = Assert.Throws<NUnit.Framework.AssertionException>(() => CodeAssert.AreEqual(expectedCode, actualCode));
            var expected = "Mismatch on line 6 of file Foo.cs\r\n" +
                           "Expected:         private readonly int _value;\r\n" +
                           "Actual:           private readonly int bar;\r\n" +
                           "                                       ^\r\n";
            Assert.AreEqual(expected, exception.Message);
        }
    }
}
