namespace Gu.Roslyn.Asserts.Tests
{
    using NUnit.Framework;

    public partial class AnalyzerAssertTests
    {
        public class HappyPath
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
                AnalyzerAssert.NoDiagnostics<NoErrorAnalyzer>(code);
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
                AnalyzerAssert.NoDiagnostics(typeof(NoErrorAnalyzer),code);
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
                AnalyzerAssert.NoDiagnostics(new NoErrorAnalyzer(), code);
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
                var exception = Assert.Throws<NUnit.Framework.AssertionException>(() => AnalyzerAssert.NoDiagnostics<ErrorOnCtorAnalyzer>(code));
                Assert.AreEqual("Foo.cs(6,9): warning ErrorOnCtor: Message format.", exception.Message);
                ////AnalyzerAssert.NoDiagnostics<ErrorOnCtorAnalyzer>(code);
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
                var exception = Assert.Throws<NUnit.Framework.AssertionException>(() => AnalyzerAssert.NoDiagnostics(typeof(ErrorOnCtorAnalyzer),code));
                Assert.AreEqual("Foo.cs(6,9): warning ErrorOnCtor: Message format.", exception.Message);
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
                var exception = Assert.Throws<NUnit.Framework.AssertionException>(() => AnalyzerAssert.NoDiagnostics(new ErrorOnCtorAnalyzer(), code));
                Assert.AreEqual("Foo.cs(6,9): warning ErrorOnCtor: Message format.", exception.Message);
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
                var exception = Assert.Throws<NUnit.Framework.AssertionException>(() => AnalyzerAssert.NoDiagnostics<ErrorOnCtorAnalyzer>(foo1, foo2));
                Assert.AreEqual("Foo1.cs(6,9): warning ErrorOnCtor: Message format.", exception.Message);
                ////AnalyzerAssert.NoDiagnostics<ErrorOnCtorAnalyzer>(code);
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
                var exception = Assert.Throws<NUnit.Framework.AssertionException>(() => AnalyzerAssert.NoDiagnostics<ErrorOnCtorAnalyzer>(foo1, foo2));
                StringAssert.Contains("Foo1.cs(6,9): warning ErrorOnCtor: Message format.", exception.Message);
                StringAssert.Contains("Foo2.cs(6,9): warning ErrorOnCtor: Message format.", exception.Message);
                ////AnalyzerAssert.NoDiagnostics<ErrorOnCtorAnalyzer>(code);
            }
        }
    }
}