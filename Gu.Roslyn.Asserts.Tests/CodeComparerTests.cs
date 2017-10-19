namespace Gu.Roslyn.Asserts.Tests
{
    using NUnit.Framework;

    public class CodeComparerTests
    {
        [Test]
        public void WhenEqual()
        {
            var x = @"
namespace RoslynSandbox
{
    class Foo
    {
        private readonly int _value;
    }
}";

            var y = @"
namespace RoslynSandbox
{
    class Foo
    {
        private readonly int _value;
    }
}";
            Assert.AreEqual(true, CodeComparer.Equals(x, y));
        }

        [Test]
        public void WhenNotEqual()
        {
            var x = @"
namespace RoslynSandbox
{
    class Foo
    {
        private readonly int _value;
    }
}";

            var y = @"
namespace RoslynSandbox
{
    class Foo
    {
        private readonly int bar;
    }
}";

            Assert.AreEqual(false, CodeComparer.Equals(x, y));
        }
    }
}