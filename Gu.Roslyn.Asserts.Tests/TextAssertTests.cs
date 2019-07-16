// ReSharper disable RedundantNameQualifier
namespace Gu.Roslyn.Asserts.Tests
{
    using NUnit.Framework;

    public static class TextAssertTests
    {
        [Test]
        public static void WhenEqual()
        {
            var expected = @"
namespace N
{
    class C
    {
        private readonly int _value;
    }
}";

            var actual = @"
namespace N
{
    class C
    {
        private readonly int _value;
    }
}";
            TextAssert.AreEqual(expected, actual);
        }

        [Test]
        public static void WhenNotEqual()
        {
            var expectedCode = @"
namespace N
{
    class C
    {
        private readonly int _value;
    }
}";

            var actualCode = @"
namespace N
{
    class C
    {
        private readonly int bar;
    }
}";
            var exception = Assert.Throws<AssertException>(() => TextAssert.AreEqual(expectedCode, actualCode));
            var expected = "Mismatch on line 6\r\n" +
                           "Expected:         private readonly int _value;\r\n" +
                           "Actual:           private readonly int bar;\r\n" +
                           "                                       ^\r\n";
            Assert.AreEqual(expected, exception.Message);
        }
    }
}
