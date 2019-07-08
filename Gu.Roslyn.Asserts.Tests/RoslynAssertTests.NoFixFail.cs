// ReSharper disable RedundantNameQualifier
namespace Gu.Roslyn.Asserts.Tests
{
    using Gu.Roslyn.Asserts.Tests.CodeFixes;
    using NUnit.Framework;

    [TestFixture]
    public static partial class RoslynAssertTests
    {
        public static class NoFixFail
        {
            [Test]
            public static void FixDoesNotMatchAnalyzer()
            {
                var code = @"
namespace RoslynSandbox
{
    class Foo
    {
        private readonly int ↓_value;
    }
}";

                var fixedCode = @"
namespace RoslynSandbox
{
    class Foo
    {
        private readonly int value;
    }
}";
                var expected = "Analyzer Gu.Roslyn.Asserts.Tests.NoErrorAnalyzer does not produce diagnostics fixable by Gu.Roslyn.Asserts.Tests.CodeFixes.DontUseUnderscoreCodeFixProvider.\r\n" +
                               "The analyzer produces the following diagnostics: {NoError}\r\n" +
                               "The code fix supports the following diagnostics: {SA1309, ID1, ID2}";
                var exception = Assert.Throws<AssertException>(() => RoslynAssert.NoFix<NoErrorAnalyzer, DontUseUnderscoreCodeFixProvider>(code, fixedCode));
                Assert.AreEqual(expected, exception.Message);
            }

            [Test]
            public static void SingleDocumentOneErrorEmptyFix()
            {
                var code = @"
namespace RoslynSandbox
{
    class Foo
    {
        private readonly int ↓_value;
    }
}";
                var expected = "Expected code to have no fixable diagnostics.\r\n" +
                               "The following actions were registered:\r\n" +
                               "Rename to: value\r\n";
                var exception = Assert.Throws<AssertException>(() => RoslynAssert.NoFix<FieldNameMustNotBeginWithUnderscore, EmptyCodeFixProvider>(code));

                CodeAssert.AreEqual(expected, exception.Message);
            }

            [Test]
            public static void SingleDocumentOneErrorCodeFixFixedTheCode()
            {
                var code = @"
namespace RoslynSandbox
{
    class Foo
    {
        private readonly int ↓_value;
    }
}";

                var exception = Assert.Throws<AssertException>(() => RoslynAssert.NoFix<FieldNameMustNotBeginWithUnderscore, DontUseUnderscoreCodeFixProvider>(code));
                var expected = "Expected code to have no fixable diagnostics.\r\n" +
                               "The following actions were registered:\r\n" +
                               "Rename to: value\r\n";
                CodeAssert.AreEqual(expected, exception.Message);
            }

            [Test]
            public static void TwoDocumentsOneErrorCodeFixFixedTheCode()
            {
                var barCode = @"
namespace RoslynSandbox
{
    class Bar
    {
        private readonly int value;
    }
}";

                var code = @"
namespace RoslynSandbox
{
    class Foo
    {
        private readonly int ↓_value;
    }
}";

                var exception = Assert.Throws<AssertException>(() => RoslynAssert.NoFix<FieldNameMustNotBeginWithUnderscore, DontUseUnderscoreCodeFixProvider>(barCode, code));
                var expected = "Expected code to have no fixable diagnostics.\r\n" +
                               "The following actions were registered:\r\n" +
                               "Rename to: value\r\n";
                CodeAssert.AreEqual(expected, exception.Message);
            }

            [Test]
            public static void DuplicateId()
            {
                var expectedDiagnostic = ExpectedDiagnostic.Create(DuplicateIdAnalyzer.Descriptor1);
                var expected = "Analyzer Gu.Roslyn.Asserts.Tests.DuplicateIdAnalyzer has more than one diagnostic with ID 0.";

                var exception = Assert.Throws<AssertException>(() => RoslynAssert.NoFix<DuplicateIdAnalyzer, DuplicateIdFix>(expectedDiagnostic, string.Empty));
                Assert.AreEqual(expected, exception.Message);
                exception = Assert.Throws<AssertException>(() => RoslynAssert.NoFix(new DuplicateIdAnalyzer(), new DuplicateIdFix(), expectedDiagnostic, string.Empty));
                Assert.AreEqual(expected, exception.Message);
            }
        }
    }
}
