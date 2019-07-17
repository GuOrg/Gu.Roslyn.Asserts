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
namespace N
{
    class C
    {
        private readonly int ↓_value;
    }
}";

                var fixedCode = @"
namespace N
{
    class C
    {
        private readonly int value;
    }
}";
                var expected = "NopAnalyzer does not produce diagnostics fixable by DoNotUseUnderscoreFix.\r\n" +
                               "NopAnalyzer.SupportedDiagnostics: 'IdWithNoFix'.\r\n" +
                               "DoNotUseUnderscoreFix.FixableDiagnosticIds: {SA1309, SA1309a, SA1309b}.";
                var analyzer = new NopAnalyzer(Descriptors.IdWithNoFix);
                var fix = new DoNotUseUnderscoreFix();
                var exception = Assert.Throws<AssertException>(() => RoslynAssert.NoFix(analyzer, fix, code, fixedCode));
                Assert.AreEqual(expected, exception.Message);
            }

            [Test]
            public static void SingleDocumentOneErrorEmptyFix()
            {
                var code = @"
namespace N
{
    class C
    {
        private readonly int ↓_value;
    }
}";
                var expected = "Expected code to have no fixable diagnostics.\r\n" +
                               "The following actions were registered:\r\n" +
                               "  'Rename to: value'\r\n";
                var analyzer = new FieldNameMustNotBeginWithUnderscore();
                var fix = new DoNotUseUnderscoreFix();
                var exception = Assert.Throws<AssertException>(() => RoslynAssert.NoFix(analyzer, fix, code));
                CodeAssert.AreEqual(expected, exception.Message);
            }

            [Test]
            public static void SingleDocumentOneErrorCodeFixFixedTheCode()
            {
                var code = @"
namespace N
{
    class C
    {
        private readonly int ↓_value;
    }
}";
                var analyzer = new FieldNameMustNotBeginWithUnderscore();
                var fix = new DoNotUseUnderscoreFix();
                var exception = Assert.Throws<AssertException>(() => RoslynAssert.NoFix(analyzer, fix, code));
                var expected = "Expected code to have no fixable diagnostics.\r\n" +
                               "The following actions were registered:\r\n" +
                               "  'Rename to: value'\r\n";
                CodeAssert.AreEqual(expected, exception.Message);
            }

            [Test]
            public static void TwoDocumentsOneErrorCodeFixFixedTheCode()
            {
                var barCode = @"
namespace N
{
    class Bar
    {
        private readonly int value;
    }
}";

                var code = @"
namespace N
{
    class C
    {
        private readonly int ↓_value;
    }
}";
                var analyzer = new FieldNameMustNotBeginWithUnderscore();
                var fix = new DoNotUseUnderscoreFix();
                var exception = Assert.Throws<AssertException>(() => RoslynAssert.NoFix(analyzer, fix, barCode, code));
                var expected = "Expected code to have no fixable diagnostics.\r\n" +
                               "The following actions were registered:\r\n" +
                               "  'Rename to: value'\r\n";
                CodeAssert.AreEqual(expected, exception.Message);
            }

            [Test]
            public static void DuplicateId()
            {
                var expected = "SyntaxNodeAnalyzer.SupportedDiagnostics has more than one descriptor with ID 'ID1'.";
                var analyzer = new SyntaxNodeAnalyzer(Descriptors.Id1, Descriptors.Id1Duplicate);
                var fix = new DuplicateIdFix();
                var expectedDiagnostic = ExpectedDiagnostic.Create(Descriptors.Id1);
                var exception = Assert.Throws<AssertException>(() => RoslynAssert.NoFix(analyzer, fix, expectedDiagnostic, string.Empty));
                Assert.AreEqual(expected, exception.Message);
            }

            [Test]
            public static void WhenNoAnalyzerDiagnosticsButCompilerError()
            {
                var code = @"
namespace N
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
