// ReSharper disable RedundantNameQualifier
namespace Gu.Roslyn.Asserts.Tests
{
    using Gu.Roslyn.Asserts.Tests.CodeFixes;
    using Microsoft.CodeAnalysis;
    using NUnit.Framework;

    [TestFixture]
    public static partial class RoslynAssertTests
    {
        public static class NoFixFail
        {
            [OneTimeSetUp]
            public static void OneTimeSetUp()
            {
                RoslynAssert.MetadataReferences.Add(MetadataReference.CreateFromFile(typeof(int).Assembly.Location));
            }

            [OneTimeTearDown]
            public static void OneTimeTearDown()
            {
                RoslynAssert.ResetAll();
            }

            [Test]
            public static void FixDoesNotMatchAnalyzer()
            {
                var code = @"
namespace RoslynSandbox
{
    class C
    {
        private readonly int ↓_value;
    }
}";

                var fixedCode = @"
namespace RoslynSandbox
{
    class C
    {
        private readonly int value;
    }
}";
                var expected = "Analyzer Gu.Roslyn.Asserts.Tests.NoErrorAnalyzer does not produce diagnostics fixable by Gu.Roslyn.Asserts.Tests.CodeFixes.DontUseUnderscoreCodeFixProvider.\r\n" +
                               "The analyzer produces the following diagnostics: {NoError}\r\n" +
                               "The code fix supports the following diagnostics: {SA1309, ID1, ID2}";
                var analyzer = new NoErrorAnalyzer();
                var fix = new DontUseUnderscoreCodeFixProvider();
                var exception = Assert.Throws<AssertException>(() => RoslynAssert.NoFix(analyzer, fix, code, fixedCode));
                Assert.AreEqual(expected, exception.Message);
            }

            [Test]
            public static void SingleDocumentOneErrorEmptyFix()
            {
                var code = @"
namespace RoslynSandbox
{
    class C
    {
        private readonly int ↓_value;
    }
}";
                var expected = "Expected code to have no fixable diagnostics.\r\n" +
                               "The following actions were registered:\r\n" +
                               "Rename to: value\r\n";
                var analyzer = new FieldNameMustNotBeginWithUnderscore();
                var fix = new DontUseUnderscoreCodeFixProvider();
                var exception = Assert.Throws<AssertException>(() => RoslynAssert.NoFix(analyzer, fix, code));
                CodeAssert.AreEqual(expected, exception.Message);
            }

            [Test]
            public static void SingleDocumentOneErrorCodeFixFixedTheCode()
            {
                var code = @"
namespace RoslynSandbox
{
    class C
    {
        private readonly int ↓_value;
    }
}";
                var analyzer = new FieldNameMustNotBeginWithUnderscore();
                var fix = new DontUseUnderscoreCodeFixProvider();
                var exception = Assert.Throws<AssertException>(() => RoslynAssert.NoFix(analyzer, fix, code));
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
    class C
    {
        private readonly int ↓_value;
    }
}";
                var analyzer = new FieldNameMustNotBeginWithUnderscore();
                var fix = new DontUseUnderscoreCodeFixProvider();
                var exception = Assert.Throws<AssertException>(() => RoslynAssert.NoFix(analyzer, fix, barCode, code));
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
                var analyzer = new DuplicateIdAnalyzer();
                var fix = new DuplicateIdFix();
                var exception = Assert.Throws<AssertException>(() => RoslynAssert.NoFix(analyzer, fix, expectedDiagnostic, string.Empty));
                Assert.AreEqual(expected, exception.Message);
            }

            [Test]
            public static void WhenNoAnalyzerDiagnosticsButCompilerError()
            {
                var code = @"
namespace RoslynSandbox
{
    class C
    {
        private readonly int ↓value = SYNTAX_ERROR;
    }
}";

                var analyzer = new FieldNameMustNotBeginWithUnderscore();
                var fix = new NoCodeFixProvider();
                var expectedDiagnostic = ExpectedDiagnostic.Create(FieldNameMustNotBeginWithUnderscore.DiagnosticId);
                var expected = "Expected and actual diagnostics do not match.\r\n" +
                               "Expected:\r\n" +
                               "SA1309 \r\n" +
                               "  at line 5 and character 29 in file C.cs | private readonly int ↓value = SYNTAX_ERROR;\r\n" +
                               "Actual:\r\n" +
                               "CS0103 The name 'SYNTAX_ERROR' does not exist in the current context\r\n" +
                               "  at line 5 and character 37 in file C.cs | private readonly int value = ↓SYNTAX_ERROR;\r\n";

                var exception = Assert.Throws<AssertException>(() => RoslynAssert.NoFix(analyzer, fix, code));
                Assert.AreEqual(expected, exception.Message);

                exception = Assert.Throws<AssertException>(() => RoslynAssert.NoFix(analyzer, fix, code, string.Empty));
                Assert.AreEqual(expected, exception.Message);

                exception = Assert.Throws<AssertException>(() => RoslynAssert.NoFix(analyzer, fix, expectedDiagnostic, code));
                Assert.AreEqual(expected, exception.Message);

                exception = Assert.Throws<AssertException>(() => RoslynAssert.NoFix(analyzer, fix, expectedDiagnostic, code, string.Empty));
                Assert.AreEqual(expected, exception.Message);
            }
        }
    }
}
