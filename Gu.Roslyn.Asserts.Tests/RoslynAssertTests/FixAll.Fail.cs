// ReSharper disable RedundantNameQualifier
namespace Gu.Roslyn.Asserts.Tests.RoslynAssertTests
{
    using System.Linq;
    using Gu.Roslyn.Asserts.Tests.CodeFixes;
    using Microsoft.CodeAnalysis;
    using NUnit.Framework;

    [TestFixture]
    public static partial class FixAll
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
        private readonly int ↓_f1 = 1;
        private readonly int _f2 = 2;

        public int M() => _f1 + _f2;
    }
}";

                var analyzer = new FieldNameMustNotBeginWithUnderscore();
                var fix = new DoNotUseUnderscoreFix();
                var expected = @"Expected and actual diagnostics do not match.
Expected:
  SA1309 
    at line 5 and character 29 in file C.cs | private readonly int ↓_f1 = 1;
Actual:
  SA1309 Field '_f1' must not begin with an underscore
    at line 5 and character 29 in file C.cs | private readonly int ↓_f1 = 1;
  SA1309 Field '_f2' must not begin with an underscore
    at line 6 and character 29 in file C.cs | private readonly int ↓_f2 = 2;
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
        private ↓readonly int _f = 1;

        public int M() => _f;
    }
}";
                var analyzer = new FieldNameMustNotBeginWithUnderscore();
                var fix = new DoNotUseUnderscoreFix();
                var exception = Assert.Throws<AssertException>(() => RoslynAssert.FixAll(analyzer, fix, before, string.Empty));
                var expected = "Expected and actual diagnostics do not match.\r\n" +
                               "Expected:\r\n" +
                               "  SA1309 \r\n" +
                               "    at line 5 and character 16 in file C.cs | private ↓readonly int _f = 1;\r\n" +
                               "Actual:\r\n" +
                               "  SA1309 Field '_f' must not begin with an underscore\r\n" +
                               "    at line 5 and character 29 in file C.cs | private readonly int ↓_f = 1;\r\n";
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
        private readonly int ↓_f = 1;

        public int M() => _f;
    }
}";

                var after = @"
namespace N
{
    class C
    {
        private readonly int f;

        public int M() => this.f;
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
    class C1
    {
        private readonly int f1 = 1;

        public int M1() => this.f1;
    }
}";

                var before = @"
namespace N
{
    class C2
    {
        private readonly int ↓_f2 = 2;

        public int M2() => _f2;
    }
}";

                var after = @"
namespace N
{
    class C2
    {
        private readonly int f2 = 2;

        public int M2() => this.f2;
    }
}";
                var analyzer = new FieldNameMustNotBeginWithUnderscore();
                var fix = new DoNotUseUnderscoreFix();
                var exception = Assert.Throws<AssertException>(() => RoslynAssert.FixAll(analyzer, fix, new[] { barCode, before }, new[] { barCode, after }));
                var expected = @"Applying fixes one by one failed.
Mismatch on line 8 of file C2.cs.
Expected:         public int M2() => this.f2;
Actual:           public int M2() => f2;
                                     ^
Expected:

namespace N
{
    class C2
    {
        private readonly int f2 = 2;

        public int M2() => this.f2;
    }
}
Actual:

namespace N
{
    class C2
    {
        private readonly int f2 = 2;

        public int M2() => f2;
    }
}
";
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
        private ↓readonly int _f = 1;

        public int M() => _f;
    }
}";
                var analyzer = new FieldNameMustNotBeginWithUnderscore();
                var fix = new DoNotUseUnderscoreFix();
                var exception = Assert.Throws<AssertException>(() => RoslynAssert.FixAll(analyzer, fix, before, string.Empty));
                var expected = "Expected and actual diagnostics do not match.\r\n" +
                               "Expected:\r\n" +
                               "  SA1309 \r\n" +
                               "    at line 5 and character 16 in file C.cs | private ↓readonly int _f = 1;\r\n" +
                               "Actual:\r\n" +
                               "  SA1309 Field '_f' must not begin with an underscore\r\n" +
                               "    at line 5 and character 29 in file C.cs | private readonly int ↓_f = 1;\r\n";
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
        public EventHandler? M() => null;
    }
}";
                var analyzer = new ClassMustHaveMethodAnalyzer();
                var fix = InsertMethodFix.ReturnNullableEventHandler;
                var exception = Assert.Throws<AssertException>(() => RoslynAssert.FixAll(analyzer, fix, before, after, settings: Settings.Default.WithMetadataReferences(Enumerable.Empty<MetadataReference>())));
                var expected = @"InsertMethodFix introduced syntax errors.
CS0518 Predefined type 'System.Object' is not defined or imported
  at line 3 and character 10 in file C.cs | class ↓C
CS0518 Predefined type 'System.Object' is not defined or imported
  at line 5 and character 15 in file C.cs | public ↓EventHandler? M() => null;
CS0246 The type or namespace name 'EventHandler' could not be found (are you missing a using directive or an assembly reference?)
  at line 5 and character 15 in file C.cs | public ↓EventHandler? M() => null;
CS0518 Predefined type 'System.Nullable`1' is not defined or imported
  at line 5 and character 15 in file C.cs | public ↓EventHandler? M() => null;
CS1729 'object' does not contain a constructor that takes 0 arguments
  at line 3 and character 10 in file C.cs | class ↓C
First source file with error is:

namespace N
{
    class C
    {
        public EventHandler? M() => null;
    }
}
";
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
