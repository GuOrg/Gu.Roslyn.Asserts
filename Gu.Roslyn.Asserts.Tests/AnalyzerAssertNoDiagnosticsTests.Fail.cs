namespace Gu.Roslyn.Asserts.Tests
{
    using System;
    using Gu.Roslyn.Asserts.Tests.CodeFixes;
    using NUnit.Framework;

    [TestFixture]
    public partial class AnalyzerAssertNoDiagnosticsTests
    {
        public class Fail
        {
            private static readonly bool Throw = false; // for testing what the output is in the runner.

            [Test]
            public void SingleClassOneErrorGeneric()
            {
                var code = @"
namespace RoslynSandbox
{
    class Foo
    {
        private readonly int _value = 1;
    }
}";
                var exception = Assert.Throws<NUnit.Framework.AssertionException>(() => AnalyzerAssert.NoDiagnostics<FieldNameMustNotBeginWithUnderscore>(code));
                Assert.AreEqual("Foo.cs(6,30): warning SA1309: Field '_value' must not begin with an underscore", exception.Message);
                if (Throw)
                {
                    AnalyzerAssert.NoDiagnostics<FieldNameMustNotBeginWithUnderscore>(code);
                }
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
        private readonly int _value = 1;
    }
}";
                var exception = Assert.Throws<NUnit.Framework.AssertionException>(() => AnalyzerAssert.NoDiagnostics(type, code));
                Assert.AreEqual("Foo.cs(6,30): warning SA1309: Field '_value' must not begin with an underscore", exception.Message);
                if (Throw)
                {
                    AnalyzerAssert.NoDiagnostics(type, code);
                }
            }

            [Test]
            public void SingleClassOneErrorPassingAnalyzer()
            {
                var code = @"
namespace RoslynSandbox
{
    class Foo
    {
        private readonly int _value = 1;
    }
}";
                var exception = Assert.Throws<NUnit.Framework.AssertionException>(() => AnalyzerAssert.NoDiagnostics(new FieldNameMustNotBeginWithUnderscore(), code));
                Assert.AreEqual("Foo.cs(6,30): warning SA1309: Field '_value' must not begin with an underscore", exception.Message);
                if (Throw)
                {
                    AnalyzerAssert.NoDiagnostics(new FieldNameMustNotBeginWithUnderscore(), code);
                }
            }

            [Test]
            public void TwoClassesOneError()
            {
                var foo1 = @"
namespace RoslynSandbox
{
    class Foo1
    {
        private readonly int _value = 1;
    }
}";
                var foo2 = @"
namespace RoslynSandbox
{
    class Foo2
    {
    }
}";
                var exception = Assert.Throws<NUnit.Framework.AssertionException>(() => AnalyzerAssert.NoDiagnostics<FieldNameMustNotBeginWithUnderscore>(foo1, foo2));
                Assert.AreEqual("Foo1.cs(6,30): warning SA1309: Field '_value' must not begin with an underscore", exception.Message);
                if (Throw)
                {
                    AnalyzerAssert.NoDiagnostics<FieldNameMustNotBeginWithUnderscore>(foo1, foo2);
                }
            }

            [Test]
            public void TwoClassesTwoErrors()
            {
                var foo1 = @"
namespace RoslynSandbox
{
    class Foo1
    {
        private readonly int _value1 = 1;
    }
}";
                var foo2 = @"
namespace RoslynSandbox
{
    class Foo2
    {
        private readonly int _value2 = 2;
    }
}";
                var exception = Assert.Throws<NUnit.Framework.AssertionException>(() => AnalyzerAssert.NoDiagnostics<FieldNameMustNotBeginWithUnderscore>(foo1, foo2));
                StringAssert.Contains("Foo1.cs(6,30): warning SA1309: Field '_value1' must not begin with an underscore", exception.Message);
                StringAssert.Contains("Foo2.cs(6,30): warning SA1309: Field '_value2' must not begin with an underscore", exception.Message);
                if (Throw)
                {
                    AnalyzerAssert.NoDiagnostics<FieldNameMustNotBeginWithUnderscore>(foo1, foo2);
                }
            }
        }
    }
}