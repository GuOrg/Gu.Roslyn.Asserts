// ReSharper disable RedundantNameQualifier
namespace Gu.Roslyn.Asserts.Tests.RoslynAssertTests
{
    using System.Linq;
    using Gu.Roslyn.Asserts.Tests.CodeFixes;
    using Microsoft.CodeAnalysis;
    using NUnit.Framework;

    [TestFixture]
    public partial class FixAll
    {
        public static class Fail
        {
            [Test]
            public static void SingleDocumentTwoErrorsOnlyOneIndicated()
            {
                var before = @"
namespace N
{
    class C
    {
        private readonly int ↓_value1;
        private readonly int _value2;
    }
}";

                var analyzer = new FieldNameMustNotBeginWithUnderscore();
                var fix = new DoNotUseUnderscoreFix();
                var expected = @"Expected and actual diagnostics do not match.
Expected:
  SA1309 
    at line 5 and character 29 in file C.cs | private readonly int ↓_value1;
Actual:
  SA1309 Field '_value1' must not begin with an underscore
    at line 5 and character 29 in file C.cs | private readonly int ↓_value1;
  SA1309 Field '_value2' must not begin with an underscore
    at line 6 and character 29 in file C.cs | private readonly int ↓_value2;
";
                var exception = Assert.Throws<AssertException>(() => RoslynAssert.FixAll(analyzer, fix, before, string.Empty));
                CodeAssert.AreEqual(expected, exception.Message);
            }

            [Test]
            public static void SingleDocumentExplicitTitle()
            {
                var before = @"
namespace N
{
    class C
    {
        private readonly int ↓_value;
    }
}";

                var analyzer = new FieldNameMustNotBeginWithUnderscore();
                var fix = new DoNotUseUnderscoreFix();
                var expected = "Did not find a code fix with title WRONG.\r\n" +
                               "Found:\r\n" +
                               "Rename to: 'value'\r\n";
                var exception = Assert.Throws<AssertException>(() => RoslynAssert.FixAll(analyzer, fix, before, string.Empty, "WRONG"));
                Assert.AreEqual(expected, exception.Message);
            }

            [Test]
            public static void SingleDocumentOneErrorWrongPosition()
            {
                var before = @"
namespace N
{
    class C
    {
        private ↓readonly int _value1;
    }
}";
                var analyzer = new FieldNameMustNotBeginWithUnderscore();
                var fix = new DoNotUseUnderscoreFix();
                var exception = Assert.Throws<AssertException>(() => RoslynAssert.FixAll(analyzer, fix, before, null));
                var expected = "Expected and actual diagnostics do not match.\r\n" +
                               "Expected:\r\n" +
                               "  SA1309 \r\n" +
                               "    at line 5 and character 16 in file C.cs | private ↓readonly int _value1;\r\n" +
                               "Actual:\r\n" +
                               "  SA1309 Field '_value1' must not begin with an underscore\r\n" +
                               "    at line 5 and character 29 in file C.cs | private readonly int ↓_value1;\r\n";
                Assert.AreEqual(expected, exception.Message);
            }

            [Test]
            public static void FixDoesNotMatchAnalyzer()
            {
                var before = @"
namespace N
{
    class C
    {
        private readonly int ↓_value;
    }
}";

                var after = @"
namespace N
{
    class C
    {
        private readonly int value;
    }
}";

                var analyzer = new NopAnalyzer(Descriptors.IdWithNoFix);
                var fix = new DoNotUseUnderscoreFix();
                var exception = Assert.Throws<AssertException>(() => RoslynAssert.FixAll(analyzer, fix, before, after));
                var expected = "NopAnalyzer does not produce diagnostics fixable by DoNotUseUnderscoreFix.\r\n" +
                               "NopAnalyzer.SupportedDiagnostics: 'IdWithNoFix'.\r\n" +
                               "DoNotUseUnderscoreFix.FixableDiagnosticIds: {SA1309, SA1309a, SA1309b}.";
                Assert.AreEqual(expected, exception.Message);
            }

            [Test]
            public static void SingleDocumentOneErrorErrorInFix()
            {
                var before = @"
namespace N
{
    class C
    {
        private readonly int ↓_value;
    }
}";

                var after = @"
namespace N
{
    class C
    {
        private readonly int bar;
    }
}";
                var analyzer = new FieldNameMustNotBeginWithUnderscore();
                var fix = new DoNotUseUnderscoreFix();
                var exception = Assert.Throws<AssertException>(() => RoslynAssert.FixAll(analyzer, fix, before, after));
                var expected = "Applying fixes one by one failed.\r\n" +
                               "Mismatch on line 6 of file C.cs.\r\n" +
                               "Expected:         private readonly int bar;\r\n" +
                               "Actual:           private readonly int value;\r\n" +
                               "                                       ^\r\n" +
                               "Expected:\r\n\r\n" +
                               "namespace N\r\n" +
                               "{\r\n" +
                               "    class C\r\n" +
                               "    {\r\n" +
                               "        private readonly int bar;\r\n" +
                               "    }\r\n" +
                               "}\r\n" +
                               "Actual:\r\n\r\n" +
                               "namespace N\r\n" +
                               "{\r\n" +
                               "    class C\r\n" +
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
namespace N
{
    class Bar
    {
        private readonly int value;
    }
}";

                var before = @"
namespace N
{
    class C
    {
        private readonly int ↓_value;
    }
}";

                var after = @"
namespace N
{
    class C
    {
        private readonly int bar;
    }
}";
                var analyzer = new FieldNameMustNotBeginWithUnderscore();
                var fix = new DoNotUseUnderscoreFix();
                var exception = Assert.Throws<AssertException>(() => RoslynAssert.FixAll(analyzer, fix, new[] { barCode, before }, new[] { barCode, after }));
                var expected = "Applying fixes one by one failed.\r\n" +
                               "Mismatch on line 6 of file C.cs.\r\n" +
                               "Expected:         private readonly int bar;\r\n" +
                               "Actual:           private readonly int value;\r\n" +
                               "                                       ^\r\n" +
                               "Expected:\r\n\r\n" +
                               "namespace N\r\n" +
                               "{\r\n" +
                               "    class C\r\n" +
                               "    {\r\n" +
                               "        private readonly int bar;\r\n" +
                               "    }\r\n" +
                               "}\r\n" +
                               "Actual:\r\n\r\n" +
                               "namespace N\r\n" +
                               "{\r\n" +
                               "    class C\r\n" +
                               "    {\r\n" +
                               "        private readonly int value;\r\n" +
                               "    }\r\n" +
                               "}\r\n";
                CodeAssert.AreEqual(expected, exception.Message);
            }

            [Test]
            public static void IndicatedAndActualPositionDoNotMatch()
            {
                var before = @"
namespace N
{
    class C
    {
        private ↓readonly int _value1;
    }
}";
                var analyzer = new FieldNameMustNotBeginWithUnderscore();
                var fix = new DoNotUseUnderscoreFix();
                var exception = Assert.Throws<AssertException>(() => RoslynAssert.FixAll(analyzer, fix, before, null));
                var expected = "Expected and actual diagnostics do not match.\r\n" +
                               "Expected:\r\n" +
                               "  SA1309 \r\n" +
                               "    at line 5 and character 16 in file C.cs | private ↓readonly int _value1;\r\n" +
                               "Actual:\r\n" +
                               "  SA1309 Field '_value1' must not begin with an underscore\r\n" +
                               "    at line 5 and character 29 in file C.cs | private readonly int ↓_value1;\r\n";
                Assert.AreEqual(expected, exception.Message);
            }

            [Test]
            public static void WhenFixIntroducesCompilerErrors()
            {
                var before = @"
namespace N
{
    ↓class C
    {
    }
}";

                var after = @"
namespace N
{
    class C
    {
        public event EventHandler E;
    }
}";
                var analyzer = new ClassMustHaveEventAnalyzer();
                var fix = new InsertEventFixProvider();
                var exception = Assert.Throws<AssertException>(() => RoslynAssert.FixAll(analyzer, fix, before, after, metadataReferences: Enumerable.Empty<MetadataReference>()));
                var expected = "Gu.Roslyn.Asserts.Tests.CodeFixes.InsertEventFixProvider introduced syntax errors.\r\n" +
                               "CS0518 Predefined type 'System.Object' is not defined or imported\r\n" +
                               "  at line 3 and character 10 in file C.cs | class ↓C\r\n" +
                               "CS0518 Predefined type 'System.Object' is not defined or imported\r\n" +
                               "  at line 5 and character 21 in file C.cs | public event ↓EventHandler E;\r\n" +
                               "CS0246 The type or namespace name 'EventHandler' could not be found (are you missing a using directive or an assembly reference?)\r\n" +
                               "  at line 5 and character 21 in file C.cs | public event ↓EventHandler E;\r\n" +
                               "CS0518 Predefined type 'System.Void' is not defined or imported\r\n" +
                               "  at line 5 and character 34 in file C.cs | public event EventHandler ↓E;\r\n" +
                               "CS0518 Predefined type 'System.Void' is not defined or imported\r\n" +
                               "  at line 5 and character 34 in file C.cs | public event EventHandler ↓E;\r\n" +
                               "CS1729 'object' does not contain a constructor that takes 0 arguments\r\n" +
                               "  at line 3 and character 10 in file C.cs | class ↓C\r\n" +
                               "First source file with error is:\r\n\r\n" +
                               "namespace N\r\n" +
                               "{\r\n" +
                               "    class C\r\n" +
                               "    {\r\n" +
                               "        public event EventHandler E;\r\n" +
                               "    }\r\n" +
                               "}\r\n";
                CodeAssert.AreEqual(expected, exception.Message);
            }

            [Test]
            public static void DuplicateId()
            {
                var expected = "SyntaxNodeAnalyzer.SupportedDiagnostics has more than one descriptor with ID 'ID1'.";
                var analyzer = new SyntaxNodeAnalyzer(Descriptors.Id1, Descriptors.Id1Duplicate);
                var fix = new DuplicateIdFix();
                var expectedDiagnostic = ExpectedDiagnostic.Create(Descriptors.Id1);
                var exception = Assert.Throws<AssertException>(() => RoslynAssert.FixAll(analyzer, fix, expectedDiagnostic, string.Empty, string.Empty));
                Assert.AreEqual(expected, exception.Message);
            }
        }
    }
}
