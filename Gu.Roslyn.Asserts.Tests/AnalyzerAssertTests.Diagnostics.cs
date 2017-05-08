namespace Gu.Roslyn.Asserts.Tests
{
    using NUnit.Framework;

    public partial class AnalyzerAssertTests
    {
        public class Diagnostics
        {
            [Test]
            public void SingleClassNoErrorGeneric()
            {
                var code = @"
namespace Gu.Roslyn.Asserts.Tests
{
    class Foo
    {
    }
}";
                Assert.Fail("Implement");
                //AnalyzerAssert.Diagnostic<ErrorOnCtorAnalyzer>(code);
            }

            [Test]
            public void SingleClassNoErrorType()
            {
                var code = @"
namespace Gu.Roslyn.Asserts.Tests
{
    class Foo
    {
    }
}";
                Assert.Fail("Implement");
            }

            [Test]
            public void SingleClassNoErrorPassingAnalyzer()
            {
                var code = @"
namespace Gu.Roslyn.Asserts.Tests
{
    class Foo
    {
    }
}";
                Assert.Fail("Implement");
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
                Assert.Fail("Implement");
            }

            [Test]
            public void SingleClassOneErrorGeneric()
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
                Assert.Fail("Implement");
            }

            [Test]
            public void SingleClassOneErrorType()
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
                Assert.Fail("Implement");
            }

            [Test]
            public void SingleClassOneErrorPassingAnalyzer()
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
                Assert.Fail("Implement");
            }

            [Test]
            public void TwoClassesOneError()
            {
                var foo1 = @"
namespace Gu.Roslyn.Asserts.Tests
{
    class Foo1
    {
        public Foo1()
        {
        }
    }
}";
                var foo2 = @"
namespace Gu.Roslyn.Asserts.Tests
{
    class Foo2
    {
    }
}";
                Assert.Fail("Implement");
            }

            [Test]
            public void TwoClassesTwoErrors()
            {
                var foo1 = @"
namespace Gu.Roslyn.Asserts.Tests
{
    class Foo1
    {
        public Foo1()
        {
        }
    }
}";
                var foo2 = @"
namespace Gu.Roslyn.Asserts.Tests
{
    class Foo2
    {
        public Foo2()
        {
        }
    }
}";
                Assert.Fail("Implement");
            }
        }
    }
}