using System;

namespace Gu.Roslyn.Asserts.Tests
{
    using NUnit.Framework;

    public partial class AnalyzerAssertTests
    {
        public class Diagnostics
        {
            [Test]
            public void SingleClassNoErrorIndicatedGeneric()
            {
                var code = @"
namespace Gu.Roslyn.Asserts.Tests
{
    class Foo
    {
    }
}";
                var exception = Assert.Throws<NUnit.Framework.AssertionException>(() => AnalyzerAssert.Diagnostics<ErrorOnCtorAnalyzer>(code));
                Assert.AreEqual("Expected code to have at least one error position indicated with '↓'", exception.Message);
            }

            [Test]
            public void SingleClassNoErrorInCode()
            {
                var code = @"
namespace Gu.Roslyn.Asserts.Tests
{
    ↓class Foo
    {
    }
}";
                var exception = Assert.Throws<NUnit.Framework.AssertionException>(() => AnalyzerAssert.Diagnostics<ErrorOnCtorAnalyzer>(code));
                Assert.AreEqual("Expected count does not match actual.\r\nExpected: 1\r\nActual:   0", exception.Message);
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
                var exception = Assert.Throws<NUnit.Framework.AssertionException>(() => AnalyzerAssert.Diagnostics<ErrorOnCtorAnalyzer>(code));
                Assert.AreEqual("Expected code to have at least one error position indicated with '↓'", exception.Message);
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
                var exception = Assert.Throws<NUnit.Framework.AssertionException>(() => AnalyzerAssert.Diagnostics<ErrorOnCtorAnalyzer>(code));
                Assert.AreEqual("Expected code to have at least one error position indicated with '↓'", exception.Message);
            }

            [Test]
            public void TwoClassesNoErrorIndicated()
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
                var exception = Assert.Throws<NUnit.Framework.AssertionException>(() => AnalyzerAssert.Diagnostics<ErrorOnCtorAnalyzer>(code1, code2));
                Assert.AreEqual("Expected code to have at least one error position indicated with '↓'", exception.Message);
            }

            [Test]
            public void TwoClassesNoErrorInCode()
            {
                var code1 = @"
namespace Gu.Roslyn.Asserts.Tests
{
    ↓class Code1
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
                var exception = Assert.Throws<NUnit.Framework.AssertionException>(() => AnalyzerAssert.Diagnostics<ErrorOnCtorAnalyzer>(code1, code2));
                Assert.AreEqual("Expected count does not match actual.\r\nExpected: 1\r\nActual:   0", exception.Message);
            }

            [Test]
            public void SingleClassOneErrorGeneric()
            {
                var code = @"
namespace Gu.Roslyn.Asserts.Tests
{
    class Foo
    {
        ↓public Foo()
        {
        }
    }
}";
                AnalyzerAssert.Diagnostics<ErrorOnCtorAnalyzer>(code);
            }

            [TestCase(typeof(ErrorOnCtorAnalyzer))]
            [TestCase(typeof(ErrorOnCtorAnalyzerDisabled))]
            public void SingleClassOneErrorType(Type type)
            {
                var code = @"
namespace Gu.Roslyn.Asserts.Tests
{
    class Foo
    {
        ↓public Foo()
        {
        }
    }
}";
                AnalyzerAssert.Diagnostics(type, code);
            }

            [Test]
            public void SingleClassOneErrorPassingAnalyzer()
            {
                var code = @"
namespace Gu.Roslyn.Asserts.Tests
{
    class Foo
    {
        ↓public Foo()
        {
        }
    }
}";
                AnalyzerAssert.Diagnostics(new ErrorOnCtorAnalyzer(), code);
            }

            [Test]
            public void TwoClassesOneError()
            {
                var foo1 = @"
namespace Gu.Roslyn.Asserts.Tests
{
    class Foo1
    {
        ↓public Foo1()
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
                AnalyzerAssert.Diagnostics<ErrorOnCtorAnalyzer>(foo1, foo2);
            }

            [Test]
            public void TwoClassesTwoErrors()
            {
                var foo1 = @"
namespace Gu.Roslyn.Asserts.Tests
{
    class Foo1
    {
        ↓public Foo1()
        {
        }
    }
}";
                var foo2 = @"
namespace Gu.Roslyn.Asserts.Tests
{
    class Foo2
    {
        ↓public Foo2()
        {
        }
    }
}";
                AnalyzerAssert.Diagnostics<ErrorOnCtorAnalyzer>(foo1, foo2);
            }
        }
    }
}