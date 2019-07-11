// ReSharper disable RedundantNameQualifier
namespace Gu.Roslyn.Asserts.Tests
{
    using System;
    using Gu.Roslyn.Asserts.Tests.CodeFixes;
    using Microsoft.CodeAnalysis;
    using NUnit.Framework;

    [TestFixture]
    public static partial class RoslynAssertTests
    {
        public static class CodeFixFail
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
            public static void SingleDocumentExplicitTitle()
            {
                var before = @"
namespace RoslynSandbox
{
    class Foo
    {
        private readonly int ↓_value;
    }
}";

                var after = @"
namespace RoslynSandbox
{
    class Foo
    {
        private readonly int value;
    }
}";
                var expected = "Did not find a code fix with title WRONG.\r\n" +
                               "Found:\r\n" +
                               "Rename to: value\r\n";

                var analyzer = new FieldNameMustNotBeginWithUnderscore();
                var fix = new DontUseUnderscoreCodeFixProvider();
                var exception = Assert.Throws<AssertException>(() => RoslynAssert.CodeFix(analyzer, fix, before, after, fixTitle: "WRONG"));
                Assert.AreEqual(expected, exception.Message);

                exception = Assert.Throws<AssertException>(() => RoslynAssert.CodeFix(analyzer, fix, new[] { before }, after, fixTitle: "WRONG"));
                Assert.AreEqual(expected, exception.Message);

                exception = Assert.Throws<AssertException>(() => RoslynAssert.CodeFix(analyzer, fix, new[] { before }, new[] { after }, fixTitle: "WRONG"));
                Assert.AreEqual(expected, exception.Message);
            }

            [Test]
            public static void SingleDocumentTwoIndicatedErrors()
            {
                var before = @"
namespace RoslynSandbox
{
    class Foo
    {
        private readonly int ↓_value1;
        private readonly int ↓_value2;
    }
}";

                var after = @"
namespace RoslynSandbox
{
    class Foo
    {
        private readonly int value1;
        private readonly int value2;
    }
}";
                var expected = "Code analyzed with Gu.Roslyn.Asserts.Tests.FieldNameMustNotBeginWithUnderscore generated more than one diagnostic fixable by Gu.Roslyn.Asserts.Tests.CodeFixes.DontUseUnderscoreCodeFixProvider.\r\n" +
                               "The analyzed code contained the following diagnostics: {SA1309, SA1309}\r\n" +
                               "The code fix supports the following diagnostics: {SA1309, ID1, ID2}\r\n" +
                               "Maybe you meant to call AnalyzerAssert.FixAll?";

                var analyzer = new FieldNameMustNotBeginWithUnderscore();
                var fix = new DontUseUnderscoreCodeFixProvider();

                var exception = Assert.Throws<AssertException>(() => RoslynAssert.CodeFix(analyzer, fix, before, after));
                Assert.AreEqual(expected, exception.Message);

                exception = Assert.Throws<AssertException>(() => RoslynAssert.CodeFix(analyzer, fix, new[] { before }, after));
                Assert.AreEqual(expected, exception.Message);

                exception = Assert.Throws<AssertException>(() => RoslynAssert.CodeFix(analyzer, fix, new[] { before }, after, fixTitle: "Rename to value"));
                Assert.AreEqual(expected, exception.Message);

                exception = Assert.Throws<AssertException>(() => RoslynAssert.CodeFix(analyzer, fix, new[] { before }, new[] { after }));
                Assert.AreEqual(expected, exception.Message);

                exception = Assert.Throws<AssertException>(() => RoslynAssert.CodeFix(analyzer, fix, new[] { before }, new[] { after }, fixTitle: "Rename to value"));
                Assert.AreEqual(expected, exception.Message);
            }

            [Test]
            public static void SingleDocumentTwoErrors()
            {
                var before = @"
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
                var exception = Assert.Throws<AssertException>(() => RoslynAssert.CodeFix(analyzer, fix, before, null));
                var expected = "Expected and actual diagnostics do not match.\r\n" +
                               "Actual:\r\n" +
                               "SA1309 Field '_value2' must not begin with an underscore\r\n" +
                               "  at line 6 and character 29 in file Foo.cs | private readonly int ↓_value2;\r\n";
                CodeAssert.AreEqual(expected, exception.Message);
            }

            [Test]
            public static void SingleDocumentOneErrorWrongPosition()
            {
                var before = @"
namespace RoslynSandbox
{
    class Foo
    {
        private ↓readonly int _value1;
    }
}";
                var analyzer = new FieldNameMustNotBeginWithUnderscore();
                var fix = new DontUseUnderscoreCodeFixProvider();
                var exception = Assert.Throws<AssertException>(() => RoslynAssert.CodeFix(analyzer, fix, before, null));
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
            public static void SingleDocumentOneErrorNoFix()
            {
                var before = @"
namespace RoslynSandbox
{
    class Foo
    {
        private readonly int ↓_value;
    }
}";
                var analyzer = new FieldNameMustNotBeginWithUnderscore();
                var fix = new NoCodeFixProvider();
                var exception = Assert.Throws<AssertException>(() => RoslynAssert.CodeFix(analyzer, fix, before, string.Empty));
                var expected = "Expected one code fix, was 0.";
                Assert.AreEqual(expected, exception.Message);
            }

            [Test]
            public static void SingleDocumentOneErrorTwoFixes()
            {
                var before = @"
namespace RoslynSandbox
{
    class Foo
    {
        private readonly int ↓_value;
    }
}";
                var analyzer = new FieldNameMustNotBeginWithUnderscore();
                var fix = new TwoFixProvider();
                var exception = Assert.Throws<AssertException>(() => RoslynAssert.CodeFix(analyzer, fix, before, string.Empty));
                var expected = "Expected only one code fix, found 2:\r\n" +
                               "Rename to: value1\r\n" +
                               "Rename to: value2\r\n" +
                               "Use the overload that specifies title.";
                Assert.AreEqual(expected, exception.Message);
            }

            [Test]
            public static void SingleDocumentOneErrorEmptyFix()
            {
                var before = @"
namespace RoslynSandbox
{
    class Foo
    {
        private readonly int ↓_value;
    }
}";

                var analyzer = new FieldNameMustNotBeginWithUnderscore();
                var fix = new EmptyCodeFixProvider();
                var exception = Assert.Throws<AssertException>(() => RoslynAssert.CodeFix(analyzer, fix, before, string.Empty));
                var expected = "Gu.Roslyn.Asserts.Tests.CodeFixes.EmptyCodeFixProvider did not change any document.";
                Console.Write(exception.Message);
                Assert.AreEqual(expected, exception.Message);
            }

            [Test]
            public static void FixDoesNotMatchAnalyzer()
            {
                var before = @"
namespace RoslynSandbox
{
    class Foo
    {
        private readonly int ↓_value;
    }
}";

                var after = @"
namespace RoslynSandbox
{
    class Foo
    {
        private readonly int bar;
    }
}";
                var analyzer = new NoErrorAnalyzer();
                var fix = new DontUseUnderscoreCodeFixProvider();
                var exception = Assert.Throws<AssertException>(() => RoslynAssert.CodeFix(analyzer, fix, before, after));
                var expected = "Analyzer Gu.Roslyn.Asserts.Tests.NoErrorAnalyzer does not produce diagnostics fixable by Gu.Roslyn.Asserts.Tests.CodeFixes.DontUseUnderscoreCodeFixProvider.\r\n" +
                               "The analyzer produces the following diagnostics: {NoError}\r\n" +
                               "The code fix supports the following diagnostics: {SA1309, ID1, ID2}";
                Assert.AreEqual(expected, exception.Message);
            }

            [Test]
            public static void OneErrorWhenFixedCodeDoesNotMatchExpected()
            {
                var before = @"
namespace RoslynSandbox
{
    class Foo
    {
        private readonly int ↓_value;
    }
}";

                var after = @"
namespace RoslynSandbox
{
    class Foo
    {
        private readonly int bar;
    }
}";
                var analyzer = new FieldNameMustNotBeginWithUnderscore();
                var fix = new DontUseUnderscoreCodeFixProvider();
                var exception = Assert.Throws<AssertException>(() => RoslynAssert.CodeFix(analyzer, fix, before, after));
                var expected = "Mismatch on line 6 of file Foo.cs.\r\n" +
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
            public static void TwoDocumentsOneErrorWhenFixedCodeDoesNotMatchExpected()
            {
                var barCode = @"
namespace RoslynSandbox
{
    class Bar
    {
        private readonly int value;
    }
}";

                var before = @"
namespace RoslynSandbox
{
    class Foo
    {
        private readonly int ↓_value;
    }
}";

                var after = @"
namespace RoslynSandbox
{
    class Foo
    {
        private readonly int bar;
    }
}";
                var expected = "Mismatch on line 6 of file Foo.cs.\r\n" +
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
                var analyzer = new FieldNameMustNotBeginWithUnderscore();
                var fix = new DontUseUnderscoreCodeFixProvider();
                var exception = Assert.Throws<AssertException>(() => RoslynAssert.CodeFix(analyzer, fix, new[] { barCode, before }, after));
                CodeAssert.AreEqual(expected, exception.Message);
            }

            [Test]
            public static void PartialTwoDocumentsOneFix()
            {
                var part1 = @"
namespace RoslynSandbox
{
    public partial class Foo
    {
        private readonly int ↓_value;
    }
}";

                var part2 = @"
namespace RoslynSandbox
{
    public partial class Foo
    {
    }
}";

                var after = @"
namespace RoslynSandbox
{
    public partial class Foo
    {
        private readonly int wrong;
    }
}";
                RoslynAssert.MetadataReferences.Add(MetadataReference.CreateFromFile(typeof(EventHandler).Assembly.Location));
                var expectedDiagnostic = ExpectedDiagnostic.Create(FieldNameMustNotBeginWithUnderscore.Descriptor);
                var expected = "Mismatch on line 6.\r\n" +
                               "Expected:         private readonly int wrong;\r\n" +
                               "Actual:           private readonly int value;\r\n" +
                               "                                       ^\r\n" +
                               "Expected:\r\n" +
                               "\r\n" +
                               "namespace RoslynSandbox\r\n" +
                               "{\r\n" +
                               "    public partial class Foo\r\n" +
                               "    {\r\n" +
                               "        private readonly int wrong;\r\n" +
                               "    }\r\n" +
                               "}\r\n" +
                               "Actual:\r\n" +
                               "\r\n" +
                               "namespace RoslynSandbox\r\n" +
                               "{\r\n" +
                               "    public partial class Foo\r\n" +
                               "    {\r\n" +
                               "        private readonly int value;\r\n" +
                               "    }\r\n" +
                               "}\r\n";
                var analyzer = new FieldNameMustNotBeginWithUnderscore();
                var fix = new DontUseUnderscoreCodeFixProvider();
                var exception = Assert.Throws<AssertException>(() => RoslynAssert.CodeFix(analyzer, fix, new[] { part1, part2 }, after));
                CodeAssert.AreEqual(expected, exception.Message);
                exception = Assert.Throws<AssertException>(() => RoslynAssert.CodeFix(analyzer, fix, expectedDiagnostic, new[] { part1, part2 }, after));
                CodeAssert.AreEqual(expected, exception.Message);
                exception = Assert.Throws<AssertException>(() => RoslynAssert.CodeFix(analyzer, fix, expectedDiagnostic, new[] { part1, part2 }, new[] { after, part2 }));
                CodeAssert.AreEqual(expected, exception.Message);
            }

            [Test]
            public static void PartialTwoDocumentsFixOnly()
            {
                var part1 = @"
namespace RoslynSandbox
{
    using System;

    public partial class Foo
    {
        public event EventHandler ↓Bar;
    }
}";

                var part2 = @"
namespace RoslynSandbox
{
    public partial class Foo
    {
    }
}";

                var after = @"
namespace RoslynSandbox
{
    using System;

    public partial class Foo
    {
        // mismatch
    }
}";
                RoslynAssert.MetadataReferences.Add(MetadataReference.CreateFromFile(typeof(EventHandler).Assembly.Location));
                var expectedDiagnostic = ExpectedDiagnostic.Create("CS0067");
                var expected = "Mismatch on line 8.\r\n" +
                               "Expected:         // mismatch\r\n" +
                               "Actual:       }\r\n" +
                               "              ^\r\n" +
                               "Expected:\r\n" +
                               "\r\n" +
                               "namespace RoslynSandbox\r\n" +
                               "{\r\n" +
                               "    using System;\r\n" +
                               "\r\n" +
                               "    public partial class Foo\r\n" +
                               "    {\r\n" +
                               "        // mismatch\r\n" +
                               "    }\r\n" +
                               "}\r\n" +
                               "Actual:\r\n" +
                               "\r\n" +
                               "namespace RoslynSandbox\r\n" +
                               "{\r\n" +
                               "    using System;\r\n" +
                               "\r\n" +
                               "    public partial class Foo\r\n" +
                               "    {\r\n" +
                               "    }\r\n" +
                               "}\r\n";
                var fix = new RemoveUnusedFixProvider();
                var exception = Assert.Throws<AssertException>(() => RoslynAssert.CodeFix(fix, expectedDiagnostic, new[] { part1, part2 }, after));
                CodeAssert.AreEqual(expected, exception.Message);
                exception = Assert.Throws<AssertException>(() => RoslynAssert.CodeFix(fix, expectedDiagnostic, new[] { part1, part2 }, new[] { after, part2 }));
                CodeAssert.AreEqual(expected, exception.Message);
            }

            [Test]
            public static void PartialTwoDocumentsFixOnlyWrongPosition()
            {
                var part1 = @"
namespace RoslynSandbox
{
    public partial class ↓Foo
    {
        public event EventHandler Bar;
    }
}";

                var part2 = @"
namespace RoslynSandbox
{
    public partial class Foo
    {
    }
}";

                RoslynAssert.MetadataReferences.Add(MetadataReference.CreateFromFile(typeof(EventHandler).Assembly.Location));
                var expectedDiagnostic = ExpectedDiagnostic.Create("CS0067");
                var expected = "Expected and actual diagnostics do not match.\r\n" +
                               "Expected:\r\n" +
                               "CS0067 \r\n" +
                               "  at line 3 and character 25 in file Unknown | public partial class ↓Foo\r\n" +
                               "Actual:\r\n" +
                               "CS0246 The type or namespace name 'EventHandler' could not be found (are you missing a using directive or an assembly reference?)\r\n" +
                               "  at line 5 and character 21 in file Unknown | public event ↓EventHandler Bar;\r\n" +
                               "CS0067 The event 'Foo.Bar' is never used\r\n" +
                               "  at line 5 and character 34 in file Unknown | public event EventHandler ↓Bar;\r\n";
                var fix = new RemoveUnusedFixProvider();
                var exception = Assert.Throws<AssertException>(() => RoslynAssert.CodeFix(fix, expectedDiagnostic, new[] { part1, part2 }, string.Empty));
                CodeAssert.AreEqual(expected, exception.Message);
                exception = Assert.Throws<AssertException>(() => RoslynAssert.CodeFix(fix, expectedDiagnostic, new[] { part1, part2 }, Array.Empty<string>()));
                CodeAssert.AreEqual(expected, exception.Message);
            }

            [Test]
            public static void TwoDocumentsOneErrorFixTouchingBothDocumentsWhenFixedCodeDoesNotMatchExpected()
            {
                var code1 = @"
namespace RoslynSandbox
{
    class C1
    {
        public int ↓WrongName { get; }
    }
}";

                var code2 = @"
namespace RoslynSandbox
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
namespace RoslynSandbox
{
    class C1
    {
        public int Foo { get; }
    }
}";

                var after2 = @"
namespace RoslynSandbox
{
    class C2
    {
        public C2(C1 c1)
        {
            var x = c1.Foo;
        }
    }
}";
                RoslynAssert.MetadataReferences.Add(MetadataReference.CreateFromFile(typeof(int).Assembly.Location));
                var analyzer = new PropertyMustBeNamedFooAnalyzer();
                var fix = new RenameToFooCodeFixProvider();
                var expected = "Mismatch on line 6 of file C1.cs.\r\n" +
                               "Expected:         public int ↓WrongName { get; }\r\n" +
                               "Actual:           public int Foo { get; }\r\n" +
                               "                             ^\r\n" +
                               "Expected:\r\n" +
                               "\r\n" +
                               "namespace RoslynSandbox\r\n" +
                               "{\r\n" +
                               "    class C1\r\n" +
                               "    {\r\n" +
                               "        public int ↓WrongName { get; }\r\n" +
                               "    }\r\n" +
                               "}\r\n" +
                               "Actual:\r\n" +
                               "\r\n" +
                               "namespace RoslynSandbox\r\n" +
                               "{\r\n" +
                               "    class C1\r\n" +
                               "    {\r\n" +
                               "        public int Foo { get; }\r\n" +
                               "    }\r\n" +
                               "}\r\n";

                var exception = Assert.Throws<AssertException>(() => RoslynAssert.CodeFix(analyzer, fix, new[] { code1, code2 }, new[] { code1, after2 }));
                CodeAssert.AreEqual(expected, exception.Message);
                exception = Assert.Throws<AssertException>(() => RoslynAssert.CodeFix(analyzer, fix, new[] { code2, code1 }, new[] { code1, after2 }));
                CodeAssert.AreEqual(expected, exception.Message);
                exception = Assert.Throws<AssertException>(() => RoslynAssert.CodeFix(analyzer, fix, new[] { code1, code2 }, new[] { after2, code1 }));
                CodeAssert.AreEqual(expected, exception.Message);
                exception = Assert.Throws<AssertException>(() => RoslynAssert.CodeFix(analyzer, fix, new[] { code2, code1 }, new[] { after2, code1 }));
                CodeAssert.AreEqual(expected, exception.Message);

                exception = Assert.Throws<AssertException>(() => RoslynAssert.CodeFix(analyzer, fix, new[] { code1, code2 }, new[] { code1, after2 }, fixTitle: "Rename to: Foo"));
                CodeAssert.AreEqual(expected, exception.Message);
                exception = Assert.Throws<AssertException>(() => RoslynAssert.CodeFix(analyzer, fix, new[] { code2, code1 }, new[] { code1, after2 }, fixTitle: "Rename to: Foo"));
                CodeAssert.AreEqual(expected, exception.Message);
                exception = Assert.Throws<AssertException>(() => RoslynAssert.CodeFix(analyzer, fix, new[] { code1, code2 }, new[] { after2, code1 }, fixTitle: "Rename to: Foo"));
                CodeAssert.AreEqual(expected, exception.Message);
                exception = Assert.Throws<AssertException>(() => RoslynAssert.CodeFix(analyzer, fix, new[] { code2, code1 }, new[] { after2, code1 }, fixTitle: "Rename to: Foo"));
                CodeAssert.AreEqual(expected, exception.Message);

                expected = "Mismatch on line 8 of file C2.cs.\r\n" +
                           "Expected:             var x = c1.WrongName;\r\n" +
                           "Actual:               var x = c1.Foo;\r\n" +
                           "                                 ^\r\n" +
                           "Expected:\r\n" +
                           "\r\n" +
                           "namespace RoslynSandbox\r\n" +
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
                           "namespace RoslynSandbox\r\n" +
                           "{\r\n" +
                           "    class C2\r\n" +
                           "    {\r\n" +
                           "        public C2(C1 c1)\r\n" +
                           "        {\r\n" +
                           "            var x = c1.Foo;\r\n" +
                           "        }\r\n" +
                           "    }\r\n" +
                           "}\r\n";

                exception = Assert.Throws<AssertException>(() => RoslynAssert.CodeFix(analyzer, fix, new[] { code1, code2 }, new[] { after1, code2 }));
                CodeAssert.AreEqual(expected, exception.Message);
                exception = Assert.Throws<AssertException>(() => RoslynAssert.CodeFix(analyzer, fix, new[] { code2, code1 }, new[] { after1, code2 }));
                CodeAssert.AreEqual(expected, exception.Message);
                exception = Assert.Throws<AssertException>(() => RoslynAssert.CodeFix(analyzer, fix, new[] { code1, code2 }, new[] { code2, after1 }));
                CodeAssert.AreEqual(expected, exception.Message);
                exception = Assert.Throws<AssertException>(() => RoslynAssert.CodeFix(analyzer, fix, new[] { code2, code1 }, new[] { code2, after1 }));
                CodeAssert.AreEqual(expected, exception.Message);

                exception = Assert.Throws<AssertException>(() => RoslynAssert.CodeFix(analyzer, fix, new[] { code1, code2 }, new[] { after1, code2 }, fixTitle: "Rename to: Foo"));
                CodeAssert.AreEqual(expected, exception.Message);
                exception = Assert.Throws<AssertException>(() => RoslynAssert.CodeFix(analyzer, fix, new[] { code2, code1 }, new[] { after1, code2 }, fixTitle: "Rename to: Foo"));
                CodeAssert.AreEqual(expected, exception.Message);
                exception = Assert.Throws<AssertException>(() => RoslynAssert.CodeFix(analyzer, fix, new[] { code1, code2 }, new[] { code2, after1 }, fixTitle: "Rename to: Foo"));
                CodeAssert.AreEqual(expected, exception.Message);
                exception = Assert.Throws<AssertException>(() => RoslynAssert.CodeFix(analyzer, fix, new[] { code2, code1 }, new[] { code2, after1 }, fixTitle: "Rename to: Foo"));
                CodeAssert.AreEqual(expected, exception.Message);
            }

            [Test]
            public static void CodeFixAddingDocument()
            {
                var before = @"
namespace RoslynSandbox
{
    class C
    {
        public static C Create() => ↓new C();
    }
}";

                var after = @"
namespace RoslynSandbox
{
    class C
    {
        public static C Create() => new C().Id();
    }
}";

                var extensionMethodCode = @"namespace RoslynSandbox
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
                               "namespace RoslynSandbox\r\n" +
                               "{\r\n" +
                               "    public static class Extensions\r\n" +
                               "    {\r\n" +
                               "    }\r\n" +
                               "}\r\n" +
                               "Actual:\r\n" +
                               "namespace RoslynSandbox\r\n" +
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

                extensionMethodCode = @"namespace RoslynSandbox
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
                           "namespace RoslynSandbox\r\n" +
                           "{\r\n" +
                           "    class C\r\n" +
                           "    {\r\n" +
                           "        public static C Create() => ↓new C();\r\n" +
                           "    }\r\n" +
                           "}\r\n" +
                           "Actual:\r\n" +
                           "\r\n" +
                           "namespace RoslynSandbox\r\n" +
                           "{\r\n" +
                           "    class C\r\n" +
                           "    {\r\n" +
                           "        public static C Create() => new C().Id();\r\n" +
                           "    }\r\n" +
                           "}\r\n";

                exception = Assert.Throws<AssertException>(() => RoslynAssert.CodeFix(analyzer, fix, new[] { before }, new[] { before, extensionMethodCode }));
                CodeAssert.AreEqual(expected, exception.Message);
                exception = Assert.Throws<AssertException>(() => RoslynAssert.CodeFix(analyzer, fix, new[] { before }, new[] { before, extensionMethodCode }));
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
namespace RoslynSandbox
{
    class C
    {
        public static C Create() => ↓new C();
    }
}";

                var after = @"
namespace RoslynSandbox
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
            public static void WhenFixIntroducesSyntaxErrors()
            {
                var before = @"
namespace RoslynSandbox
{
    ↓class Foo
    {
    }
}";

                var after = @"
namespace RoslynSandbox
{
    class Foo
    {
        public event EventHandler SomeEvent;
    }
}";

                var expected = @"Gu.Roslyn.Asserts.Tests.CodeFixes.InsertEventFixProvider introduced syntax error.
CS0246 The type or namespace name 'EventHandler' could not be found (are you missing a using directive or an assembly reference?)
  at line 5 and character 21 in file Foo.cs | public event ↓EventHandler SomeEvent;
First source file with error is:

namespace RoslynSandbox
{
    class Foo
    {
        public event EventHandler SomeEvent;
    }
}
";
                var analyzer = new ClassMustHaveEventAnalyzer();
                var fix = new InsertEventFixProvider();
                var exception = Assert.Throws<AssertException>(() => RoslynAssert.CodeFix(analyzer, fix, before, after));
                CodeAssert.AreEqual(expected, exception.Message);
            }

            [Test]
            public static void IndicatedAndActualPositionDoNotMatch()
            {
                var before = @"
namespace RoslynSandbox
{
    class Foo
    {
        private ↓readonly int _value1;
    }
}";
                var expected = "Expected and actual diagnostics do not match.\r\n" +
                               "Expected:\r\n" +
                               "SA1309 \r\n" +
                               "  at line 5 and character 16 in file Foo.cs | private ↓readonly int _value1;\r\n" +
                               "Actual:\r\n" +
                               "SA1309 Field '_value1' must not begin with an underscore\r\n" +
                               "  at line 5 and character 29 in file Foo.cs | private readonly int ↓_value1;\r\n";
                var analyzer = new FieldNameMustNotBeginWithUnderscore();
                var fix = new DontUseUnderscoreCodeFixProvider();
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
