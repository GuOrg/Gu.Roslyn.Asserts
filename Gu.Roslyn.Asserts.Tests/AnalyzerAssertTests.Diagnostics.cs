namespace Gu.Roslyn.Asserts.Tests
{
    using System;
    using NUnit.Framework;

    public partial class AnalyzerAssertTests
    {
        public class Diagnostics
        {
            [Test]
            public void SingleClassNoErrorIndicatedGeneric()
            {
                var code = @"
namespace RoslynSandbox
{
    class Foo
    {
    }
}";
                var exception = Assert.Throws<NUnit.Framework.AssertionException>(() => AnalyzerAssert.Diagnostics<FieldNameMustNotBeginWithUnderscore>(code));
                Assert.AreEqual("Expected code to have at least one error position indicated with '↓'", exception.Message);
            }

            [Test]
            public void SingleClassNoErrorInCode()
            {
                var code = @"
namespace RoslynSandbox
{
    ↓class Foo
    {
    }
}";
                var exception = Assert.Throws<NUnit.Framework.AssertionException>(() => AnalyzerAssert.Diagnostics<FieldNameMustNotBeginWithUnderscore>(code));
                Assert.AreEqual("Expected count does not match actual.\r\nExpected: 1\r\nActual:   0", exception.Message);
            }

            [Test]
            public void SingleClassNoErrorType()
            {
                var code = @"
namespace RoslynSandbox
{
    class Foo
    {
    }
}";
                var exception = Assert.Throws<NUnit.Framework.AssertionException>(() => AnalyzerAssert.Diagnostics<FieldNameMustNotBeginWithUnderscore>(code));
                Assert.AreEqual("Expected code to have at least one error position indicated with '↓'", exception.Message);
            }

            [Test]
            public void SingleClassNoErrorPassingAnalyzer()
            {
                var code = @"
namespace RoslynSandbox
{
    class Foo
    {
    }
}";
                var exception = Assert.Throws<NUnit.Framework.AssertionException>(() => AnalyzerAssert.Diagnostics<FieldNameMustNotBeginWithUnderscore>(code));
                Assert.AreEqual("Expected code to have at least one error position indicated with '↓'", exception.Message);
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
                Assert.AreEqual("Expected code to have at least one error position indicated with '↓'", exception.Message);
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
                Assert.AreEqual("Expected count does not match actual.\r\nExpected: 1\r\nActual:   0", exception.Message);
            }

            [Test]
            public void SingleClassOneErrorGeneric()
            {
                var code = @"
namespace RoslynSandbox
{
    class Foo
    {
        private readonly int ↓_value;
    }
}";
                AnalyzerAssert.Diagnostics<FieldNameMustNotBeginWithUnderscore>(code);
            }

            [TestCase(typeof(FieldNameMustNotBeginWithUnderscore))]
            [TestCase(typeof(FieldNameMustNotBeginWithUnderscoreDisabled))]
            public void SingleClassOneErrorType(Type type)
            {
                var code = @"
namespace RoslynSandbox
{
    class Foo
    {
        private readonly int ↓_value = 1;
    }
}";
                AnalyzerAssert.Diagnostics(type, code);
            }

            [Test]
            public void SingleClassOneErrorPassingAnalyzer()
            {
                var code = @"
namespace RoslynSandbox
{
    class Foo
    {
        private readonly int ↓_value = 1;
    }
}";
                AnalyzerAssert.Diagnostics(new FieldNameMustNotBeginWithUnderscore(), code);
            }

            [Test]
            public void TwoClassesOneError()
            {
                var foo1 = @"
namespace RoslynSandbox
{
    class Foo1
    {
        private readonly int ↓_value = 1;
    }
}";
                var foo2 = @"
namespace RoslynSandbox
{
    class Foo2
    {
    }
}";
                AnalyzerAssert.Diagnostics<FieldNameMustNotBeginWithUnderscore>(foo1, foo2);
            }

            [Test]
            public void TwoClassesTwoErrorsGeneric()
            {
                var foo1 = @"
namespace RoslynSandbox
{
    class Foo1
    {
        private readonly int ↓_value = 1;
    }
}";
                var foo2 = @"
namespace RoslynSandbox
{
    class Foo2
    {
        private readonly int ↓_value = 1;
    }
}";
                AnalyzerAssert.Diagnostics<FieldNameMustNotBeginWithUnderscore>(foo1, foo2);
            }

            [TestCase(typeof(FieldNameMustNotBeginWithUnderscore))]
            [TestCase(typeof(FieldNameMustNotBeginWithUnderscoreDisabled))]
            public void TwoClassesTwoErrorsType(Type type)
            {
                var foo1 = @"
namespace RoslynSandbox
{
    class Foo1
    {
        private readonly int ↓_value = 1;
    }
}";
                var foo2 = @"
namespace RoslynSandbox
{
    class Foo2
    {
        private readonly int ↓_value = 1;
    }
}";
                AnalyzerAssert.Diagnostics(type, foo1, foo2);
            }
        }
    }
}