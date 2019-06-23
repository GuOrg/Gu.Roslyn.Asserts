// ReSharper disable RedundantNameQualifier
namespace Gu.Roslyn.Asserts.Tests
{
    using Gu.Roslyn.Asserts.Tests.CodeFixes;
    using Microsoft.CodeAnalysis;
    using NUnit.Framework;

    [TestFixture]
    public partial class AnalyzerAssertFixAllTests
    {
        public class Fail
        {
            [Test]
            public void SingleDocumentTwoErrorsOnlyOneIndicated()
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

                var exception = Assert.Throws<AssertException>(() => RoslynAssert.FixAll<FieldNameMustNotBeginWithUnderscore, DontUseUnderscoreCodeFixProvider>(code, null));
                var expected = "Expected and actual diagnostics do not match.\r\n" +
                               "Actual:\r\n" +
                               "SA1309 Field '_value2' must not begin with an underscore\r\n" +
                               "  at line 6 and character 29 in file Foo.cs | private readonly int ↓_value2;\r\n";
                Assert.AreEqual(expected, exception.Message);
            }

            [Test]
            public void SingleDocumentExplicitTitle()
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
                RoslynAssert.MetadataReferences.Add(MetadataReference.CreateFromFile(typeof(int).Assembly.Location));
                var exception = Assert.Throws<AssertException>(() => RoslynAssert.FixAll<FieldNameMustNotBeginWithUnderscore, DontUseUnderscoreCodeFixProvider>(code, fixedCode, "WRONG"));
                var expected = "Did not find a code fix with title WRONG.\r\n" +
                               "Found:\r\n" +
                               "Rename to: value\r\n";
                Assert.AreEqual(expected, exception.Message);
            }

            [Test]
            public void SingleDocumentOneErrorWrongPosition()
            {
                var code = @"
namespace RoslynSandbox
{
    class Foo
    {
        private ↓readonly int _value1;
    }
}";

                var exception = Assert.Throws<AssertException>(() => RoslynAssert.FixAll<FieldNameMustNotBeginWithUnderscore, DontUseUnderscoreCodeFixProvider>(code, null));
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
            public void FixDoesNotMatchAnalyzer()
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

                var exception = Assert.Throws<AssertException>(() => RoslynAssert.FixAll<NoErrorAnalyzer, DontUseUnderscoreCodeFixProvider>(code, fixedCode));
                var expected = "Analyzer Gu.Roslyn.Asserts.Tests.NoErrorAnalyzer does not produce diagnostics fixable by Gu.Roslyn.Asserts.Tests.CodeFixes.DontUseUnderscoreCodeFixProvider.\r\n" +
                               "The analyzer produces the following diagnostics: {NoError}\r\n" +
                               "The code fix supports the following diagnostics: {SA1309, ID1, ID2}";
                Assert.AreEqual(expected, exception.Message);
            }

            [Test]
            public void SingleDocumentOneErrorErrorInFix()
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
                var exception = Assert.Throws<AssertException>(() => RoslynAssert.FixAll<FieldNameMustNotBeginWithUnderscore, DontUseUnderscoreCodeFixProvider>(code, fixedCode));
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
            public void TwoDocumentsOneErrorErrorInFix()
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
                var exception = Assert.Throws<AssertException>(() => RoslynAssert.FixAll<FieldNameMustNotBeginWithUnderscore, DontUseUnderscoreCodeFixProvider>(new[] { barCode, code }, new[] { barCode, fixedCode }));
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
            public void IndicatedAndActualPositionDoNotMatch()
            {
                var code = @"
namespace RoslynSandbox
{
    class Foo
    {
        private ↓readonly int _value1;
    }
}";

                var exception = Assert.Throws<AssertException>(() => RoslynAssert.FixAll<FieldNameMustNotBeginWithUnderscore, DontUseUnderscoreCodeFixProvider>(code, null));
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
            public void WhenFixIntroducesCompilerErrors()
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
                var exception = Assert.Throws<AssertException>(() => RoslynAssert.FixAll<ClassMustHaveEventAnalyzer, InsertEventFixProvider>(code, fixedCode));
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
        }
    }
}
