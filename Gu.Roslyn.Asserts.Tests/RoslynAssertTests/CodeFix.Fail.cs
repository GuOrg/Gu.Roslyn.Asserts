// ReSharper disable RedundantNameQualifier

namespace Gu.Roslyn.Asserts.Tests.RoslynAssertTests
{
    using System;
    using Gu.Roslyn.Asserts.Tests.CodeFixes;
    using NUnit.Framework;

    public static partial class CodeFix
    {
        public static class Fail
        {
            [Test]
            public static void SingleDocumentExplicitTitle()
            {
                var before = @"
namespace N
{
    class C
    {
        private readonly int ↓_f;
    }
}";

                var after = @"
namespace N
{
    class C
    {
        private readonly int f;
    }
}";
                var expected = "Did not find a code fix with title WRONG.\r\n" +
                               "Found:\r\n" +
                               "Rename to: 'f'\r\n";

                var analyzer = new FieldNameMustNotBeginWithUnderscore();
                var fix = new DoNotUseUnderscoreFix();
                var exception = Assert.Throws<AssertException>(() => RoslynAssert.CodeFix(analyzer, fix, before, after, fixTitle: "WRONG"));
                Assert.AreEqual(expected, exception.Message);

                exception = Assert.Throws<AssertException>(() => RoslynAssert.CodeFix(analyzer, fix, new[] { before }, after, fixTitle: "WRONG"));
                Assert.AreEqual(expected, exception.Message);

                exception = Assert.Throws<AssertException>(() => RoslynAssert.CodeFix(analyzer, fix, new[] { before }, new[] { after }, fixTitle: "WRONG"));
                Assert.AreEqual(expected, exception.Message);
            }

            [Test]
            public static void SingleDocumentTwoFixableErrors()
            {
                var before = @"
namespace N
{
    class C
    {
        private readonly int ↓_f1;
        private readonly int ↓_f2;
    }
}";

                var after = @"
namespace N
{
    class C
    {
        private readonly int f1;
        private readonly int f2;
    }
}";
                var expected = @"Expected only one code fix, found 2:
  Rename to: 'f1'
  Rename to: 'f2'
Use the overload that specifies title.
Or maybe you meant to call RoslynAssert.FixAll?
";

                var analyzer = new FieldNameMustNotBeginWithUnderscore();
                var fix = new DoNotUseUnderscoreFix();

                var exception = Assert.Throws<AssertException>(() => RoslynAssert.CodeFix(analyzer, fix, before, after));
                CodeAssert.AreEqual(expected, exception.Message);

                exception = Assert.Throws<AssertException>(() => RoslynAssert.CodeFix(analyzer, fix, new[] { before }, after));
                CodeAssert.AreEqual(expected, exception.Message);

                exception = Assert.Throws<AssertException>(() => RoslynAssert.CodeFix(analyzer, fix, new[] { before }, new[] { after }));
                CodeAssert.AreEqual(expected, exception.Message);
            }

            [Test]
            public static void SingleDocumentTwoErrors()
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
                var exception = Assert.Throws<AssertException>(() => RoslynAssert.CodeFix(analyzer, fix, before, string.Empty));
                var expected = @"Expected and actual diagnostics do not match.
Matched: 1 diagnostic(s).
Missed:
  SA1309 Field '_f2' must not begin with an underscore
    at line 6 and character 29 in file C.cs | private readonly int ↓_f2 = 2;
";
                CodeAssert.AreEqual(expected, exception.Message);
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
                var exception = Assert.Throws<AssertException>(() => RoslynAssert.CodeFix(analyzer, fix, before, string.Empty));
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
            public static void SingleDocumentOneErrorNoFix()
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
                var fix = new CodeFixes.NoFix();
                var exception = Assert.Throws<AssertException>(() => RoslynAssert.CodeFix(analyzer, fix, before, string.Empty));
                var expected = "Expected one code fix, was 0.";
                Assert.AreEqual(expected, exception.Message);
            }

            [Test]
            public static void SingleDocumentOneErrorTwoFixes()
            {
                var before = @"
namespace N
{
    class C
    {
        private readonly int ↓_f;
    }
}";
                var analyzer = new FieldNameMustNotBeginWithUnderscore();
                var fix = new RenameTwoFix();
                var exception = Assert.Throws<AssertException>(() => RoslynAssert.CodeFix(analyzer, fix, before, string.Empty));
                var expected = @"Expected only one code fix, found 2:
  Rename to: f1
  Rename to: f2
Use the overload that specifies title.
Or maybe you meant to call RoslynAssert.FixAll?
";
                CodeAssert.AreEqual(expected, exception.Message);
            }

            [Test]
            public static void SingleDocumentOneErrorEmptyFix()
            {
                var before = @"
namespace N
{
    class C
    {
        private readonly int ↓_f;
    }
}";

                var analyzer = new FieldNameMustNotBeginWithUnderscore();
                var fix = new EmptyFix();
                var exception = Assert.Throws<AssertException>(() => RoslynAssert.CodeFix(analyzer, fix, before, string.Empty));
                var expected = "EmptyFix did not change any document.";
                Console.Write(exception.Message);
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
        private readonly int ↓_f;
    }
}";

                var analyzer = new NopAnalyzer(Descriptors.IdWithNoFix);
                var fix = new DoNotUseUnderscoreFix();
                var expected = "NopAnalyzer does not produce diagnostics fixable by DoNotUseUnderscoreFix.\r\n" +
                               "NopAnalyzer.SupportedDiagnostics: 'IdWithNoFix'.\r\n" +
                               "DoNotUseUnderscoreFix.FixableDiagnosticIds: {SA1309, SA1309a, SA1309b}.";
                var exception = Assert.Throws<AssertException>(() => RoslynAssert.CodeFix(analyzer, fix, before, string.Empty));
                Assert.AreEqual(expected, exception.Message);
            }

            [Test]
            public static void OneErrorWhenFixedCodeDoesNotMatchExpected()
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
        private readonly int wrong = 1;

        public int M() => wrong;
    }
}";
                var analyzer = new FieldNameMustNotBeginWithUnderscore();
                var fix = new DoNotUseUnderscoreFix();
                var exception = Assert.Throws<AssertException>(() => RoslynAssert.CodeFix(analyzer, fix, before, after));
                var expected = @"Mismatch on line 6 of file C.cs.
Expected:         private readonly int wrong = 1;
Actual:           private readonly int f = 1;
                                       ^
Expected:

namespace N
{
    class C
    {
        private readonly int wrong = 1;

        public int M() => wrong;
    }
}
Actual:

namespace N
{
    class C
    {
        private readonly int f = 1;

        public int M() => f;
    }
}
";
                CodeAssert.AreEqual(expected, exception.Message);
            }

            [Test]
            public static void TwoDocumentsOneErrorWhenFixedCodeDoesNotMatchExpected()
            {
                var c1 = @"
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
        private readonly int wrong = 2;

        public int M2() => wrong;
    }
}";
                var expected = @"Mismatch on line 6 of file C2.cs.
Expected:         private readonly int wrong = 2;
Actual:           private readonly int f2 = 2;
                                       ^
Expected:

namespace N
{
    class C2
    {
        private readonly int wrong = 2;

        public int M2() => wrong;
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
                var analyzer = new FieldNameMustNotBeginWithUnderscore();
                var fix = new DoNotUseUnderscoreFix();
                var exception = Assert.Throws<AssertException>(() => RoslynAssert.CodeFix(analyzer, fix, new[] { c1, before }, after));
                CodeAssert.AreEqual(expected, exception.Message);
            }

            [Test]
            public static void PartialTwoDocumentsOneFix()
            {
                var before = @"
namespace N
{
    public partial class C
    {
        private readonly int ↓_f = 1;

        public int M() => _f;
    }
}";

                var part2 = @"
namespace N
{
    public partial class C
    {
    }
}";

                var after = @"
namespace N
{
    public partial class C
    {
        private readonly int wrong;

        public int M() => wrong;
    }
}";
                var expectedDiagnostic = ExpectedDiagnostic.Create(FieldNameMustNotBeginWithUnderscore.Descriptor);
                var expected = @"Mismatch on line 6.
Expected:         private readonly int wrong;
Actual:           private readonly int f = 1;
                                       ^
Expected:

namespace N
{
    public partial class C
    {
        private readonly int wrong;

        public int M() => wrong;
    }
}
Actual:

namespace N
{
    public partial class C
    {
        private readonly int f = 1;

        public int M() => f;
    }
}
";
                var analyzer = new FieldNameMustNotBeginWithUnderscore();
                var fix = new DoNotUseUnderscoreFix();
                var exception = Assert.Throws<AssertException>(() => RoslynAssert.CodeFix(analyzer, fix, new[] { before, part2 }, after));
                CodeAssert.AreEqual(expected, exception.Message);
                exception = Assert.Throws<AssertException>(() => RoslynAssert.CodeFix(analyzer, fix, expectedDiagnostic, new[] { before, part2 }, after));
                CodeAssert.AreEqual(expected, exception.Message);
                exception = Assert.Throws<AssertException>(() => RoslynAssert.CodeFix(analyzer, fix, expectedDiagnostic, new[] { before, part2 }, new[] { after, part2 }));
                CodeAssert.AreEqual(expected, exception.Message);
            }

            [Test]
            public static void PartialTwoDocumentsFixOnly()
            {
                var before = @"
namespace N
{
    using System;

    public partial class C
    {
        public event EventHandler? ↓Bar;
    }
}";

                var part2 = @"
namespace N
{
    public partial class C
    {
    }
}";

                var after = @"
namespace N
{
    using System;

    public partial class C
    {
        // mismatch
    }
}";
                var expectedDiagnostic = ExpectedDiagnostic.Create("CS0067");
                var expected = "Mismatch on line 8.\r\n" +
                               "Expected:         // mismatch\r\n" +
                               "Actual:       }\r\n" +
                               "              ^\r\n" +
                               "Expected:\r\n" +
                               "\r\n" +
                               "namespace N\r\n" +
                               "{\r\n" +
                               "    using System;\r\n" +
                               "\r\n" +
                               "    public partial class C\r\n" +
                               "    {\r\n" +
                               "        // mismatch\r\n" +
                               "    }\r\n" +
                               "}\r\n" +
                               "Actual:\r\n" +
                               "\r\n" +
                               "namespace N\r\n" +
                               "{\r\n" +
                               "    using System;\r\n" +
                               "\r\n" +
                               "    public partial class C\r\n" +
                               "    {\r\n" +
                               "    }\r\n" +
                               "}\r\n";
                var fix = new RemoveUnusedFix();
                var exception = Assert.Throws<AssertException>(() => RoslynAssert.CodeFix(fix, expectedDiagnostic, new[] { before, part2 }, after));
                CodeAssert.AreEqual(expected, exception.Message);
                exception = Assert.Throws<AssertException>(() => RoslynAssert.CodeFix(fix, expectedDiagnostic, new[] { before, part2 }, new[] { after, part2 }));
                CodeAssert.AreEqual(expected, exception.Message);
            }

            [Test]
            public static void PartialTwoDocumentsFixOnlyWrongPosition()
            {
                var before = @"
namespace N
{
    public partial class ↓C
    {
        public event EventHandler Bar;
    }
}";

                var part2 = @"
namespace N
{
    public partial class C
    {
    }
}";

                var expectedDiagnostic = ExpectedDiagnostic.Create("CS0067");
                var expected = "Expected and actual diagnostics do not match.\r\n" +
                               "Expected:\r\n" +
                               "  CS0067 \r\n" +
                               "    at line 3 and character 25 in file Unknown | public partial class ↓C\r\n" +
                               "Actual:\r\n" +
                               "  CS0246 The type or namespace name 'EventHandler' could not be found (are you missing a using directive or an assembly reference?)\r\n" +
                               "    at line 5 and character 21 in file Unknown | public event ↓EventHandler Bar;\r\n" +
                               "  CS0067 The event 'C.Bar' is never used\r\n" +
                               "    at line 5 and character 34 in file Unknown | public event EventHandler ↓Bar;\r\n";
                var fix = new RemoveUnusedFix();
                var exception = Assert.Throws<AssertException>(() => RoslynAssert.CodeFix(fix, expectedDiagnostic, new[] { before, part2 }, string.Empty));
                CodeAssert.AreEqual(expected, exception.Message);

                exception = Assert.Throws<AssertException>(() => RoslynAssert.CodeFix(fix, expectedDiagnostic, new[] { before, part2 }, Array.Empty<string>()));
                CodeAssert.AreEqual(expected, exception.Message);
            }

            [Test]
            public static void TwoDocumentsOneErrorFixTouchingBothDocumentsWhenFixedCodeDoesNotMatchExpected()
            {
                var before = @"
namespace N
{
    class C1
    {
        public int ↓WrongName { get; }
    }
}";

                var code2 = @"
namespace N
{
    class C2
    {
        public C2(C1 c1)
        {
            var x = c1.WrongName;
        }
    }
}";

                var after1 = @"
namespace N
{
    class C1
    {
        public int Value { get; }
    }
}";

                var after2 = @"
namespace N
{
    class C2
    {
        public C2(C1 c1)
        {
            var x = c1.Value;
        }
    }
}";
                var analyzer = new PropertyMustBeNamedValueAnalyzer();
                var fix = new RenameToValueFix();
                var expected = "Mismatch on line 6 of file C1.cs.\r\n" +
                               "Expected:         public int ↓WrongName { get; }\r\n" +
                               "Actual:           public int Value { get; }\r\n" +
                               "                             ^\r\n" +
                               "Expected:\r\n" +
                               "\r\n" +
                               "namespace N\r\n" +
                               "{\r\n" +
                               "    class C1\r\n" +
                               "    {\r\n" +
                               "        public int ↓WrongName { get; }\r\n" +
                               "    }\r\n" +
                               "}\r\n" +
                               "Actual:\r\n" +
                               "\r\n" +
                               "namespace N\r\n" +
                               "{\r\n" +
                               "    class C1\r\n" +
                               "    {\r\n" +
                               "        public int Value { get; }\r\n" +
                               "    }\r\n" +
                               "}\r\n";

                var exception = Assert.Throws<AssertException>(() => RoslynAssert.CodeFix(analyzer, fix, new[] { before, code2 }, new[] { before, after2 }));
                CodeAssert.AreEqual(expected, exception.Message);
                exception = Assert.Throws<AssertException>(() => RoslynAssert.CodeFix(analyzer, fix, new[] { code2, before }, new[] { before, after2 }));
                CodeAssert.AreEqual(expected, exception.Message);
                exception = Assert.Throws<AssertException>(() => RoslynAssert.CodeFix(analyzer, fix, new[] { before, code2 }, new[] { after2, before }));
                CodeAssert.AreEqual(expected, exception.Message);
                exception = Assert.Throws<AssertException>(() => RoslynAssert.CodeFix(analyzer, fix, new[] { code2, before }, new[] { after2, before }));
                CodeAssert.AreEqual(expected, exception.Message);

                exception = Assert.Throws<AssertException>(() => RoslynAssert.CodeFix(analyzer, fix, new[] { before, code2 }, new[] { before, after2 }, fixTitle: "Rename to: Value"));
                CodeAssert.AreEqual(expected, exception.Message);
                exception = Assert.Throws<AssertException>(() => RoslynAssert.CodeFix(analyzer, fix, new[] { code2, before }, new[] { before, after2 }, fixTitle: "Rename to: Value"));
                CodeAssert.AreEqual(expected, exception.Message);
                exception = Assert.Throws<AssertException>(() => RoslynAssert.CodeFix(analyzer, fix, new[] { before, code2 }, new[] { after2, before }, fixTitle: "Rename to: Value"));
                CodeAssert.AreEqual(expected, exception.Message);
                exception = Assert.Throws<AssertException>(() => RoslynAssert.CodeFix(analyzer, fix, new[] { code2, before }, new[] { after2, before }, fixTitle: "Rename to: Value"));
                CodeAssert.AreEqual(expected, exception.Message);

                expected = "Mismatch on line 8 of file C2.cs.\r\n" +
                           "Expected:             var x = c1.WrongName;\r\n" +
                           "Actual:               var x = c1.Value;\r\n" +
                           "                                 ^\r\n" +
                           "Expected:\r\n" +
                           "\r\n" +
                           "namespace N\r\n" +
                           "{\r\n" +
                           "    class C2\r\n" +
                           "    {\r\n" +
                           "        public C2(C1 c1)\r\n" +
                           "        {\r\n" +
                           "            var x = c1.WrongName;\r\n" +
                           "        }\r\n" +
                           "    }\r\n" +
                           "}\r\n" +
                           "Actual:\r\n" +
                           "\r\n" +
                           "namespace N\r\n" +
                           "{\r\n" +
                           "    class C2\r\n" +
                           "    {\r\n" +
                           "        public C2(C1 c1)\r\n" +
                           "        {\r\n" +
                           "            var x = c1.Value;\r\n" +
                           "        }\r\n" +
                           "    }\r\n" +
                           "}\r\n";

                exception = Assert.Throws<AssertException>(() => RoslynAssert.CodeFix(analyzer, fix, new[] { before, code2 }, new[] { after1, code2 }));
                CodeAssert.AreEqual(expected, exception.Message);
                exception = Assert.Throws<AssertException>(() => RoslynAssert.CodeFix(analyzer, fix, new[] { code2, before }, new[] { after1, code2 }));
                CodeAssert.AreEqual(expected, exception.Message);
                exception = Assert.Throws<AssertException>(() => RoslynAssert.CodeFix(analyzer, fix, new[] { before, code2 }, new[] { code2, after1 }));
                CodeAssert.AreEqual(expected, exception.Message);
                exception = Assert.Throws<AssertException>(() => RoslynAssert.CodeFix(analyzer, fix, new[] { code2, before }, new[] { code2, after1 }));
                CodeAssert.AreEqual(expected, exception.Message);

                exception = Assert.Throws<AssertException>(() => RoslynAssert.CodeFix(analyzer, fix, new[] { before, code2 }, new[] { after1, code2 }, fixTitle: "Rename to: Value"));
                CodeAssert.AreEqual(expected, exception.Message);
                exception = Assert.Throws<AssertException>(() => RoslynAssert.CodeFix(analyzer, fix, new[] { code2, before }, new[] { after1, code2 }, fixTitle: "Rename to: Value"));
                CodeAssert.AreEqual(expected, exception.Message);
                exception = Assert.Throws<AssertException>(() => RoslynAssert.CodeFix(analyzer, fix, new[] { before, code2 }, new[] { code2, after1 }, fixTitle: "Rename to: Value"));
                CodeAssert.AreEqual(expected, exception.Message);
                exception = Assert.Throws<AssertException>(() => RoslynAssert.CodeFix(analyzer, fix, new[] { code2, before }, new[] { code2, after1 }, fixTitle: "Rename to: Value"));
                CodeAssert.AreEqual(expected, exception.Message);
            }

            [Test]
            public static void CodeFixAddingDocument()
            {
                var before = @"
namespace N
{
    class C
    {
        public static C Create() => ↓new C();
    }
}";

                var after = @"
namespace N
{
    class C
    {
        public static C Create() => new C().Id();
    }
}";

                var extensionMethodCode = @"namespace N
{
    public static class Extensions
    {
    }
}";
                var analyzer = new CallIdAnalyzer();
                var fix = new CallIdFix();
                var expected = "Mismatch on line 5 of file Extensions.cs.\r\n" +
                               "Expected:     }\r\n" +
                               "Actual:           public static T Id<T>(this T t) => t;\r\n" +
                               "              ^\r\n" +
                               "Expected:\r\n" +
                               "namespace N\r\n" +
                               "{\r\n" +
                               "    public static class Extensions\r\n" +
                               "    {\r\n" +
                               "    }\r\n" +
                               "}\r\n" +
                               "Actual:\r\n" +
                               "namespace N\r\n" +
                               "{\r\n" +
                               "    public static class Extensions\r\n" +
                               "    {\r\n" +
                               "        public static T Id<T>(this T t) => t;\r\n" +
                               "    }\r\n" +
                               "}\r\n";

                var exception = Assert.Throws<AssertException>(() => RoslynAssert.CodeFix(analyzer, fix, new[] { before }, new[] { after, extensionMethodCode }));
                CodeAssert.AreEqual(expected, exception.Message);
                exception = Assert.Throws<AssertException>(() => RoslynAssert.CodeFix(analyzer, fix, new[] { before }, new[] { after, extensionMethodCode }));
                CodeAssert.AreEqual(expected, exception.Message);

                exception = Assert.Throws<AssertException>(() => RoslynAssert.CodeFix(analyzer, fix, new[] { before }, new[] { after, extensionMethodCode }, fixTitle: "Call ID()"));
                CodeAssert.AreEqual(expected, exception.Message);
                exception = Assert.Throws<AssertException>(() => RoslynAssert.CodeFix(analyzer, fix, new[] { before }, new[] { after, extensionMethodCode }, fixTitle: "Call ID()"));
                CodeAssert.AreEqual(expected, exception.Message);

                extensionMethodCode = @"namespace N
{
    public static class Extensions
    {
        public static T Id<T>(this T t) => t;
    }
}";
                expected = "Mismatch on line 6 of file C.cs.\r\n" +
                           "Expected:         public static C Create() => ↓new C();\r\n" +
                           "Actual:           public static C Create() => new C().Id();\r\n" +
                           "                                              ^\r\n" +
                           "Expected:\r\n" +
                           "\r\n" +
                           "namespace N\r\n" +
                           "{\r\n" +
                           "    class C\r\n" +
                           "    {\r\n" +
                           "        public static C Create() => ↓new C();\r\n" +
                           "    }\r\n" +
                           "}\r\n" +
                           "Actual:\r\n" +
                           "\r\n" +
                           "namespace N\r\n" +
                           "{\r\n" +
                           "    class C\r\n" +
                           "    {\r\n" +
                           "        public static C Create() => new C().Id();\r\n" +
                           "    }\r\n" +
                           "}\r\n";

                exception = Assert.Throws<AssertException>(() =>
                    RoslynAssert.CodeFix(analyzer, fix, new[] { before }, new[] { before, extensionMethodCode }));
                CodeAssert.AreEqual(expected, exception.Message);
                exception = Assert.Throws<AssertException>(() =>
                    RoslynAssert.CodeFix(analyzer, fix, new[] { before }, new[] { before, extensionMethodCode }));
                CodeAssert.AreEqual(expected, exception.Message);

                exception = Assert.Throws<AssertException>(() => RoslynAssert.CodeFix(analyzer, fix, new[] { before }, new[] { before, extensionMethodCode }, fixTitle: "Call ID()"));
                CodeAssert.AreEqual(expected, exception.Message);
                exception = Assert.Throws<AssertException>(() => RoslynAssert.CodeFix(analyzer, fix, new[] { before }, new[] { before, extensionMethodCode }, fixTitle: "Call ID()"));
                CodeAssert.AreEqual(expected, exception.Message);
            }

            [Test]
            public static void CodeFixAddingDocumentWhenExpectedAddedDocIsNotProvided()
            {
                var before = @"
namespace N
{
    class C
    {
        public static C Create() => ↓new C();
    }
}";

                var after = @"
namespace N
{
    class C
    {
        public static C Create() => new C().Id();
    }
}";

                var analyzer = new CallIdAnalyzer();
                var fix = new CallIdFix();
                var exception = Assert.Throws<AssertException>(() => RoslynAssert.CodeFix(analyzer, fix, before, after));
                CodeAssert.AreEqual("Expected 1 documents the fixed solution has 2 documents.", exception.Message);
                exception = Assert.Throws<AssertException>(() => RoslynAssert.CodeFix(analyzer, fix, new[] { before }, after));
                CodeAssert.AreEqual("Expected 1 documents the fixed solution has 2 documents.", exception.Message);
                exception = Assert.Throws<AssertException>(() => RoslynAssert.CodeFix(analyzer, fix, new[] { before }, new[] { after }));
                CodeAssert.AreEqual("Expected 1 documents the fixed solution has 2 documents.", exception.Message);
            }

            [Test]
            public static void WhenFixIntroducesDiagnostics()
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
                var fix = InsertMethodFix.ReturnEventHandler;
                var exception = Assert.Throws<AssertException>(() => RoslynAssert.CodeFix(analyzer, fix, before, after));
                var expected = @"The fixed code by InsertMethodFix contains compiler diagnostic.
  - fix the code used in the test
  - suppress the warning in the test code using for example pragma
  - suppress the warning by providing Settings to the assert.
CS0246 The type or namespace name 'EventHandler' could not be found (are you missing a using directive or an assembly reference?)
  at line 5 and character 15 in file C.cs | public ↓EventHandler? M() => null;
First source file with diagnostic is:

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
                var expected = "Expected and actual diagnostics do not match.\r\n" +
                               "Expected:\r\n" +
                               "  SA1309 \r\n" +
                               "    at line 5 and character 16 in file C.cs | private ↓readonly int _f = 1;\r\n" +
                               "Actual:\r\n" +
                               "  SA1309 Field '_f' must not begin with an underscore\r\n" +
                               "    at line 5 and character 29 in file C.cs | private readonly int ↓_f = 1;\r\n";
                var analyzer = new FieldNameMustNotBeginWithUnderscore();
                var fix = new DoNotUseUnderscoreFix();
                var exception = Assert.Throws<AssertException>(() => RoslynAssert.CodeFix(analyzer, fix, new[] { before }, string.Empty));
                Assert.AreEqual(expected, exception.Message);
                exception = Assert.Throws<AssertException>(() => RoslynAssert.CodeFix(analyzer, fix, new[] { before }, new[] { string.Empty }));
                Assert.AreEqual(expected, exception.Message);
                exception = Assert.Throws<AssertException>(() => RoslynAssert.CodeFix(analyzer, fix, new[] { before }, string.Empty, fixTitle: "WRONG"));
                Assert.AreEqual(expected, exception.Message);
                exception = Assert.Throws<AssertException>(() => RoslynAssert.CodeFix(analyzer, fix, new[] { before }, new[] { string.Empty }, fixTitle: "WRONG"));
                Assert.AreEqual(expected, exception.Message);
            }
        }
    }
}
