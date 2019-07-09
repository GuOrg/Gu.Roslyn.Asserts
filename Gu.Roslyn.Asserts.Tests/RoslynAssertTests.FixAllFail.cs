// ReSharper disable RedundantNameQualifier
namespace Gu.Roslyn.Asserts.Tests
{
    using Gu.Roslyn.Asserts.Tests.CodeFixes;
    using Microsoft.CodeAnalysis;
    using NUnit.Framework;

    [TestFixture]
    public partial class RoslynAssertTests
    {
        public static class FixAllFail
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
            public static void SingleDocumentTwoErrorsOnlyOneIndicated()
            {
                var code = @"
namespace RoslynSandbox
{
    class Foo
    {
        private readonly int ↓_value1;
        private readonly int _value2;
    }
}";

                var analyzer = new FieldNameMustNotBeginWithUnderscore();
                var fix = new DontUseUnderscoreCodeFixProvider();
                var expected = "Expected and actual diagnostics do not match.\r\n" +
                               "Actual:\r\n" +
                               "SA1309 Field '_value2' must not begin with an underscore\r\n" +
                               "  at line 6 and character 29 in file Foo.cs | private readonly int ↓_value2;\r\n";
                var exception = Assert.Throws<AssertException>(() => RoslynAssert.FixAll(analyzer, fix, code, string.Empty));
                Assert.AreEqual(expected, exception.Message);
            }

            [Test]
            public static void SingleDocumentExplicitTitle()
            {
                var code = @"
namespace RoslynSandbox
{
    class Foo
    {
        private readonly int ↓_value;
    }
}";

                var analyzer = new FieldNameMustNotBeginWithUnderscore();
                var fix = new DontUseUnderscoreCodeFixProvider();
                var expected = "Did not find a code fix with title WRONG.\r\n" +
                               "Found:\r\n" +
                               "Rename to: value\r\n";
                var exception = Assert.Throws<AssertException>(() => RoslynAssert.FixAll(analyzer, fix, code, string.Empty, "WRONG"));
                Assert.AreEqual(expected, exception.Message);
            }

            [Test]
            public static void SingleDocumentOneErrorWrongPosition()
            {
                var code = @"
namespace RoslynSandbox
{
    class Foo
    {
        private ↓readonly int _value1;
    }
}";
                var analyzer = new FieldNameMustNotBeginWithUnderscore();
                var fix = new DontUseUnderscoreCodeFixProvider();
                var exception = Assert.Throws<AssertException>(() => RoslynAssert.FixAll(analyzer, fix, code, null));
                var expected = "Expected and actual diagnostics do not match.\r\n" +
                               "Expected:\r\n" +
                               "SA1309 \r\n" +
                               "  at line 5 and character 16 in file Foo.cs | private ↓readonly int _value1;\r\n" +
                               "Actual:\r\n" +
                               "SA1309 Field '_value1' must not begin with an underscore\r\n" +
                               "  at line 5 and character 29 in file Foo.cs | private readonly int ↓_value1;\r\n";
                Assert.AreEqual(expected, exception.Message);
            }

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

                var analyzer = new NoErrorAnalyzer();
                var fix = new DontUseUnderscoreCodeFixProvider();
                var exception = Assert.Throws<AssertException>(() => RoslynAssert.FixAll(analyzer, fix, code, fixedCode));
                var expected = "Analyzer Gu.Roslyn.Asserts.Tests.NoErrorAnalyzer does not produce diagnostics fixable by Gu.Roslyn.Asserts.Tests.CodeFixes.DontUseUnderscoreCodeFixProvider.\r\n" +
                               "The analyzer produces the following diagnostics: {NoError}\r\n" +
                               "The code fix supports the following diagnostics: {SA1309, ID1, ID2}";
                Assert.AreEqual(expected, exception.Message);
            }

            [Test]
            public static void SingleDocumentOneErrorErrorInFix()
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
        private readonly int bar;
    }
}";
                var analyzer = new FieldNameMustNotBeginWithUnderscore();
                var fix = new DontUseUnderscoreCodeFixProvider();
                var exception = Assert.Throws<AssertException>(() => RoslynAssert.FixAll(analyzer, fix, code, fixedCode));
                var expected = "Applying fixes one by one failed.\r\n" +
                               "Mismatch on line 6 of file Foo.cs.\r\n" +
                               "Expected:         private readonly int bar;\r\n" +
                               "Actual:           private readonly int value;\r\n" +
                               "                                       ^\r\n" +
                               "Expected:\r\n\r\n" +
                               "namespace RoslynSandbox\r\n" +
                               "{\r\n" +
                               "    class Foo\r\n" +
                               "    {\r\n" +
                               "        private readonly int bar;\r\n" +
                               "    }\r\n" +
                               "}\r\n" +
                               "Actual:\r\n\r\n" +
                               "namespace RoslynSandbox\r\n" +
                               "{\r\n" +
                               "    class Foo\r\n" +
                               "    {\r\n" +
                               "        private readonly int value;\r\n" +
                               "    }\r\n" +
                               "}\r\n";
                CodeAssert.AreEqual(expected, exception.Message);
            }

            [Test]
            public static void TwoDocumentsOneErrorErrorInFix()
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

                var fixedCode = @"
namespace RoslynSandbox
{
    class Foo
    {
        private readonly int bar;
    }
}";
                var analyzer = new FieldNameMustNotBeginWithUnderscore();
                var fix = new DontUseUnderscoreCodeFixProvider();
                var exception = Assert.Throws<AssertException>(() => RoslynAssert.FixAll(analyzer, fix, new[] { barCode, code }, new[] { barCode, fixedCode }));
                var expected = "Applying fixes one by one failed.\r\n" +
                               "Mismatch on line 6 of file Foo.cs.\r\n" +
                               "Expected:         private readonly int bar;\r\n" +
                               "Actual:           private readonly int value;\r\n" +
                               "                                       ^\r\n" +
                               "Expected:\r\n\r\n" +
                               "namespace RoslynSandbox\r\n" +
                               "{\r\n" +
                               "    class Foo\r\n" +
                               "    {\r\n" +
                               "        private readonly int bar;\r\n" +
                               "    }\r\n" +
                               "}\r\n" +
                               "Actual:\r\n\r\n" +
                               "namespace RoslynSandbox\r\n" +
                               "{\r\n" +
                               "    class Foo\r\n" +
                               "    {\r\n" +
                               "        private readonly int value;\r\n" +
                               "    }\r\n" +
                               "}\r\n";
                CodeAssert.AreEqual(expected, exception.Message);
            }

            [Test]
            public static void IndicatedAndActualPositionDoNotMatch()
            {
                var code = @"
namespace RoslynSandbox
{
    class Foo
    {
        private ↓readonly int _value1;
    }
}";
                var analyzer = new FieldNameMustNotBeginWithUnderscore();
                var fix = new DontUseUnderscoreCodeFixProvider();
                var exception = Assert.Throws<AssertException>(() => RoslynAssert.FixAll(analyzer, fix, code, null));
                var expected = "Expected and actual diagnostics do not match.\r\n" +
                               "Expected:\r\n" +
                               "SA1309 \r\n" +
                               "  at line 5 and character 16 in file Foo.cs | private ↓readonly int _value1;\r\n" +
                               "Actual:\r\n" +
                               "SA1309 Field '_value1' must not begin with an underscore\r\n" +
                               "  at line 5 and character 29 in file Foo.cs | private readonly int ↓_value1;\r\n";
                Assert.AreEqual(expected, exception.Message);
            }

            [Test]
            public static void WhenFixIntroducesCompilerErrors()
            {
                RoslynAssert.MetadataReferences.Clear();
                var code = @"
namespace RoslynSandbox
{
    ↓class Foo
    {
    }
}";

                var fixedCode = @"
namespace RoslynSandbox
{
    class Foo
    {
        public event EventHandler SomeEvent;
    }
}";
                var analyzer = new ClassMustHaveEventAnalyzer();
                var fix = new InsertEventFixProvider();
                var exception = Assert.Throws<AssertException>(() => RoslynAssert.FixAll(analyzer, fix, code, fixedCode));
                var expected = "Gu.Roslyn.Asserts.Tests.CodeFixes.InsertEventFixProvider introduced syntax errors.\r\n" +
                               "CS0518 Predefined type 'System.Object' is not defined or imported\r\n" +
                               "  at line 3 and character 10 in file Foo.cs | class ↓Foo\r\n" +
                               "CS0518 Predefined type 'System.Object' is not defined or imported\r\n" +
                               "  at line 5 and character 21 in file Foo.cs | public event ↓EventHandler SomeEvent;\r\n" +
                               "CS0246 The type or namespace name 'EventHandler' could not be found (are you missing a using directive or an assembly reference?)\r\n" +
                               "  at line 5 and character 21 in file Foo.cs | public event ↓EventHandler SomeEvent;\r\n" +
                               "CS0518 Predefined type 'System.Void' is not defined or imported\r\n" +
                               "  at line 5 and character 34 in file Foo.cs | public event EventHandler ↓SomeEvent;\r\n" +
                               "CS0518 Predefined type 'System.Void' is not defined or imported\r\n" +
                               "  at line 5 and character 34 in file Foo.cs | public event EventHandler ↓SomeEvent;\r\n" +
                               "CS1729 'object' does not contain a constructor that takes 0 arguments\r\n" +
                               "  at line 3 and character 10 in file Foo.cs | class ↓Foo\r\n" +
                               "First source file with error is:\r\n\r\n" +
                               "namespace RoslynSandbox\r\n" +
                               "{\r\n" +
                               "    class Foo\r\n" +
                               "    {\r\n" +
                               "        public event EventHandler SomeEvent;\r\n" +
                               "    }\r\n" +
                               "}\r\n";
                CodeAssert.AreEqual(expected, exception.Message);
            }

            [Test]
            public static void DuplicateId()
            {
                var expectedDiagnostic = ExpectedDiagnostic.Create(DuplicateIdAnalyzer.Descriptor1);
                var expected = "Analyzer Gu.Roslyn.Asserts.Tests.DuplicateIdAnalyzer has more than one diagnostic with ID 0.";

                var exception = Assert.Throws<AssertException>(() => RoslynAssert.FixAll(new DuplicateIdAnalyzer(), new DuplicateIdFix(), expectedDiagnostic, string.Empty, string.Empty));
                Assert.AreEqual(expected, exception.Message);
            }
        }
    }
}
