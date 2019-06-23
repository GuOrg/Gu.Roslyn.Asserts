// ReSharper disable RedundantNameQualifier
namespace Gu.Roslyn.Asserts.Tests
{
    using System;
    using Gu.Roslyn.Asserts.Tests.CodeFixes;
    using Microsoft.CodeAnalysis;
    using NUnit.Framework;

    [TestFixture]
    public partial class AnalyzerAssertCodeFixTests
    {
        public class Fail
        {
            [TearDown]
            public void TearDown()
            {
                RoslynAssert.MetadataReferences.Clear();
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
                var expected = "Did not find a code fix with title WRONG.\r\n" +
                               "Found:\r\n" +
                               "Rename to: value\r\n";

                RoslynAssert.MetadataReferences.Add(MetadataReference.CreateFromFile(typeof(int).Assembly.Location));

                var exception = Assert.Throws<AssertException>(() => RoslynAssert.CodeFix<FieldNameMustNotBeginWithUnderscore, DontUseUnderscoreCodeFixProvider>(code, fixedCode, "WRONG"));
                Assert.AreEqual(expected, exception.Message);

                exception = Assert.Throws<AssertException>(() => RoslynAssert.CodeFix<FieldNameMustNotBeginWithUnderscore, DontUseUnderscoreCodeFixProvider>(new[] { code }, fixedCode, "WRONG"));
                Assert.AreEqual(expected, exception.Message);

                var analyzer = new FieldNameMustNotBeginWithUnderscore();
                var fix = new DontUseUnderscoreCodeFixProvider();
                exception = Assert.Throws<AssertException>(() => RoslynAssert.CodeFix(analyzer, fix, new[] { code }, fixedCode, fixTitle: "WRONG"));
                Assert.AreEqual(expected, exception.Message);
                exception = Assert.Throws<AssertException>(() => RoslynAssert.CodeFix(analyzer, fix, new[] { code }, new[] { fixedCode }, fixTitle: "WRONG"));
                Assert.AreEqual(expected, exception.Message);
            }

            [Test]
            public void SingleDocumentTwoIndicatedErrors()
            {
                var code = @"
namespace RoslynSandbox
{
    class Foo
    {
        private readonly int ↓_value1;
        private readonly int ↓_value2;
    }
}";

                var fixedCode = @"
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

                var exception = Assert.Throws<AssertException>(() => RoslynAssert.CodeFix<FieldNameMustNotBeginWithUnderscore, DontUseUnderscoreCodeFixProvider>(code, fixedCode));
                Assert.AreEqual(expected, exception.Message);

                exception = Assert.Throws<AssertException>(() => RoslynAssert.CodeFix<FieldNameMustNotBeginWithUnderscore, DontUseUnderscoreCodeFixProvider>(new[] { code }, fixedCode));
                Assert.AreEqual(expected, exception.Message);

                var analyzer = new FieldNameMustNotBeginWithUnderscore();
                var fix = new DontUseUnderscoreCodeFixProvider();

                exception = Assert.Throws<AssertException>(() => RoslynAssert.CodeFix(analyzer, fix, new[] { code }, fixedCode));
                Assert.AreEqual(expected, exception.Message);

                exception = Assert.Throws<AssertException>(() => RoslynAssert.CodeFix(analyzer, fix, new[] { code }, fixedCode, fixTitle: "Rename to value"));
                Assert.AreEqual(expected, exception.Message);

                exception = Assert.Throws<AssertException>(() => RoslynAssert.CodeFix(analyzer, fix, new[] { code }, new[] { fixedCode }));
                Assert.AreEqual(expected, exception.Message);

                exception = Assert.Throws<AssertException>(() => RoslynAssert.CodeFix(analyzer, fix, new[] { code }, new[] { fixedCode }, fixTitle: "Rename to value"));
                Assert.AreEqual(expected, exception.Message);
            }

            [Test]
            public void SingleDocumentTwoErrors()
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

                var exception = Assert.Throws<AssertException>(() => RoslynAssert.CodeFix<FieldNameMustNotBeginWithUnderscore, DontUseUnderscoreCodeFixProvider>(code, null));
                var expected = "Expected and actual diagnostics do not match.\r\n" +
                               "Actual:\r\n" +
                               "SA1309 Field '_value2' must not begin with an underscore\r\n" +
                               "  at line 6 and character 29 in file Foo.cs | private readonly int ↓_value2;\r\n";
                CodeAssert.AreEqual(expected, exception.Message);
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

                var exception = Assert.Throws<AssertException>(() => RoslynAssert.CodeFix<FieldNameMustNotBeginWithUnderscore, DontUseUnderscoreCodeFixProvider>(code, null));
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
            public void SingleDocumentOneErrorNoFix()
            {
                var code = @"
namespace RoslynSandbox
{
    class Foo
    {
        private readonly int ↓_value;
    }
}";

                var exception = Assert.Throws<AssertException>(() => RoslynAssert.CodeFix<FieldNameMustNotBeginWithUnderscore, NoCodeFixProvider>(code, string.Empty));
                var expected = "Expected one code fix, was 0.";
                Assert.AreEqual(expected, exception.Message);
            }

            [Test]
            public void SingleDocumentOneErrorTwoFixes()
            {
                var code = @"
namespace RoslynSandbox
{
    class Foo
    {
        private readonly int ↓_value;
    }
}";

                var exception = Assert.Throws<AssertException>(() => RoslynAssert.CodeFix<FieldNameMustNotBeginWithUnderscore, TwoFixProvider>(code, string.Empty));
                var expected = "Expected only one code fix, found 2:\r\n" +
                               "Rename to: value1\r\n" +
                               "Rename to: value2\r\n" +
                               "Use the overload that specifies title.";
                Assert.AreEqual(expected, exception.Message);
            }

            [Test]
            public void SingleDocumentOneErrorEmptyFix()
            {
                var code = @"
namespace RoslynSandbox
{
    class Foo
    {
        private readonly int ↓_value;
    }
}";

                var temp = code.AssertReplace("↓", string.Empty);
                var exception = Assert.Throws<AssertException>(() => RoslynAssert.CodeFix<FieldNameMustNotBeginWithUnderscore, EmptyCodeFixProvider>(code, temp));
                var expected = "Gu.Roslyn.Asserts.Tests.CodeFixes.EmptyCodeFixProvider did not change any document.";
                Console.Write(exception.Message);
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
        private readonly int bar;
    }
}";

                var exception = Assert.Throws<AssertException>(() => RoslynAssert.CodeFix<NoErrorAnalyzer, DontUseUnderscoreCodeFixProvider>(code, fixedCode));
                var expected = "Analyzer Gu.Roslyn.Asserts.Tests.NoErrorAnalyzer does not produce diagnostics fixable by Gu.Roslyn.Asserts.Tests.CodeFixes.DontUseUnderscoreCodeFixProvider.\r\n" +
                               "The analyzer produces the following diagnostics: {NoError}\r\n" +
                               "The code fix supports the following diagnostics: {SA1309, ID1, ID2}";
                Assert.AreEqual(expected, exception.Message);
            }

            [Test]
            public void OneErrorWhenFixedCodeDoesNotMatchExpected()
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
                var exception = Assert.Throws<AssertException>(() => RoslynAssert.CodeFix<FieldNameMustNotBeginWithUnderscore, DontUseUnderscoreCodeFixProvider>(code, fixedCode));
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
            public void TwoDocumentsOneErrorWhenFixedCodeDoesNotMatchExpected()
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
                var exception = Assert.Throws<AssertException>(() => RoslynAssert.CodeFix<FieldNameMustNotBeginWithUnderscore, DontUseUnderscoreCodeFixProvider>(new[] { barCode, code }, fixedCode));
                CodeAssert.AreEqual(expected, exception.Message);
            }

            [Test]
            public void TwoDocumentsOneErrorFixTouchingBothDocumentsWhenFixedCodeDoesNotMatchExpected()
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

                var fixedCode1 = @"
namespace RoslynSandbox
{
    class C1
    {
        public int Foo { get; }
    }
}";

                var fixedCode2 = @"
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

                var exception = Assert.Throws<AssertException>(() => RoslynAssert.CodeFix(analyzer, fix, new[] { code1, code2 }, new[] { code1, fixedCode2 }));
                CodeAssert.AreEqual(expected, exception.Message);
                exception = Assert.Throws<AssertException>(() => RoslynAssert.CodeFix(analyzer, fix, new[] { code2, code1 }, new[] { code1, fixedCode2 }));
                CodeAssert.AreEqual(expected, exception.Message);
                exception = Assert.Throws<AssertException>(() => RoslynAssert.CodeFix(analyzer, fix, new[] { code1, code2 }, new[] { fixedCode2, code1 }));
                CodeAssert.AreEqual(expected, exception.Message);
                exception = Assert.Throws<AssertException>(() => RoslynAssert.CodeFix(analyzer, fix, new[] { code2, code1 }, new[] { fixedCode2, code1 }));
                CodeAssert.AreEqual(expected, exception.Message);

                exception = Assert.Throws<AssertException>(() => RoslynAssert.CodeFix(analyzer, fix, new[] { code1, code2 }, new[] { code1, fixedCode2 }, fixTitle: "Rename to: Foo"));
                CodeAssert.AreEqual(expected, exception.Message);
                exception = Assert.Throws<AssertException>(() => RoslynAssert.CodeFix(analyzer, fix, new[] { code2, code1 }, new[] { code1, fixedCode2 }, fixTitle: "Rename to: Foo"));
                CodeAssert.AreEqual(expected, exception.Message);
                exception = Assert.Throws<AssertException>(() => RoslynAssert.CodeFix(analyzer, fix, new[] { code1, code2 }, new[] { fixedCode2, code1 }, fixTitle: "Rename to: Foo"));
                CodeAssert.AreEqual(expected, exception.Message);
                exception = Assert.Throws<AssertException>(() => RoslynAssert.CodeFix(analyzer, fix, new[] { code2, code1 }, new[] { fixedCode2, code1 }, fixTitle: "Rename to: Foo"));
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

                exception = Assert.Throws<AssertException>(() => RoslynAssert.CodeFix(analyzer, fix, new[] { code1, code2 }, new[] { fixedCode1, code2 }));
                CodeAssert.AreEqual(expected, exception.Message);
                exception = Assert.Throws<AssertException>(() => RoslynAssert.CodeFix(analyzer, fix, new[] { code2, code1 }, new[] { fixedCode1, code2 }));
                CodeAssert.AreEqual(expected, exception.Message);
                exception = Assert.Throws<AssertException>(() => RoslynAssert.CodeFix(analyzer, fix, new[] { code1, code2 }, new[] { code2, fixedCode1 }));
                CodeAssert.AreEqual(expected, exception.Message);
                exception = Assert.Throws<AssertException>(() => RoslynAssert.CodeFix(analyzer, fix, new[] { code2, code1 }, new[] { code2, fixedCode1 }));
                CodeAssert.AreEqual(expected, exception.Message);

                exception = Assert.Throws<AssertException>(() => RoslynAssert.CodeFix(analyzer, fix, new[] { code1, code2 }, new[] { fixedCode1, code2 }, fixTitle: "Rename to: Foo"));
                CodeAssert.AreEqual(expected, exception.Message);
                exception = Assert.Throws<AssertException>(() => RoslynAssert.CodeFix(analyzer, fix, new[] { code2, code1 }, new[] { fixedCode1, code2 }, fixTitle: "Rename to: Foo"));
                CodeAssert.AreEqual(expected, exception.Message);
                exception = Assert.Throws<AssertException>(() => RoslynAssert.CodeFix(analyzer, fix, new[] { code1, code2 }, new[] { code2, fixedCode1 }, fixTitle: "Rename to: Foo"));
                CodeAssert.AreEqual(expected, exception.Message);
                exception = Assert.Throws<AssertException>(() => RoslynAssert.CodeFix(analyzer, fix, new[] { code2, code1 }, new[] { code2, fixedCode1 }, fixTitle: "Rename to: Foo"));
                CodeAssert.AreEqual(expected, exception.Message);
            }

            [Test]
            public void CodeFixAddingDocument()
            {
                var code = @"
namespace RoslynSandbox
{
    class C
    {
        public static C Create() => ↓new C();
    }
}";

                var fixedCode = @"
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
                RoslynAssert.MetadataReferences.Add(MetadataReference.CreateFromFile(typeof(int).Assembly.Location));
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

                var exception = Assert.Throws<AssertException>(() => RoslynAssert.CodeFix(analyzer, fix, new[] { code }, new[] { fixedCode, extensionMethodCode }));
                CodeAssert.AreEqual(expected, exception.Message);
                exception = Assert.Throws<AssertException>(() => RoslynAssert.CodeFix(analyzer, fix, new[] { code }, new[] { fixedCode, extensionMethodCode }));
                CodeAssert.AreEqual(expected, exception.Message);

                exception = Assert.Throws<AssertException>(() => RoslynAssert.CodeFix(analyzer, fix, new[] { code }, new[] { fixedCode, extensionMethodCode }, fixTitle: "Call ID()"));
                CodeAssert.AreEqual(expected, exception.Message);
                exception = Assert.Throws<AssertException>(() => RoslynAssert.CodeFix(analyzer, fix, new[] { code }, new[] { fixedCode, extensionMethodCode }, fixTitle: "Call ID()"));
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

                exception = Assert.Throws<AssertException>(() => RoslynAssert.CodeFix(analyzer, fix, new[] { code }, new[] { code, extensionMethodCode }));
                CodeAssert.AreEqual(expected, exception.Message);
                exception = Assert.Throws<AssertException>(() => RoslynAssert.CodeFix(analyzer, fix, new[] { code }, new[] { code, extensionMethodCode }));
                CodeAssert.AreEqual(expected, exception.Message);

                exception = Assert.Throws<AssertException>(() => RoslynAssert.CodeFix(analyzer, fix, new[] { code }, new[] { code, extensionMethodCode }, fixTitle: "Call ID()"));
                CodeAssert.AreEqual(expected, exception.Message);
                exception = Assert.Throws<AssertException>(() => RoslynAssert.CodeFix(analyzer, fix, new[] { code }, new[] { code, extensionMethodCode }, fixTitle: "Call ID()"));
                CodeAssert.AreEqual(expected, exception.Message);
            }

            [Test]
            public void CodeFixAddingDocumentWhenExpectedAddedDocIsNotProvided()
            {
                var code = @"
namespace RoslynSandbox
{
    class C
    {
        public static C Create() => ↓new C();
    }
}";

                var fixedCode = @"
namespace RoslynSandbox
{
    class C
    {
        public static C Create() => new C().Id();
    }
}";

                RoslynAssert.MetadataReferences.Add(MetadataReference.CreateFromFile(typeof(int).Assembly.Location));
                var analyzer = new CallIdAnalyzer();
                var fix = new CallIdFix();
                var exception = Assert.Throws<AssertException>(() => RoslynAssert.CodeFix(analyzer, fix, code, fixedCode));
                CodeAssert.AreEqual("Expected 1 documents the fixed solution has 2 documents.", exception.Message);
                exception = Assert.Throws<AssertException>(() => RoslynAssert.CodeFix(analyzer, fix, new[] { code }, fixedCode));
                CodeAssert.AreEqual("Expected 1 documents the fixed solution has 2 documents.", exception.Message);
                exception = Assert.Throws<AssertException>(() => RoslynAssert.CodeFix(analyzer, fix, new[] { code }, new[] { fixedCode }));
                CodeAssert.AreEqual("Expected 1 documents the fixed solution has 2 documents.", exception.Message);
            }

            [Test]
            public void WhenFixIntroducesCompilerErrors()
            {
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
                var exception = Assert.Throws<AssertException>(() => RoslynAssert.CodeFix<ClassMustHaveEventAnalyzer, InsertEventFixProvider>(code, fixedCode));
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
            public void WhenFixIsCorrectWithMetadataReferences()
            {
                var code = @"
namespace RoslynSandbox
{
    using System;

    ↓class Foo
    {
    }
}";

                var fixedCode = @"
namespace RoslynSandbox
{
    using System;

    class Foo
    {
        public event EventHandler SomeEvent;
    }
}";
                RoslynAssert.MetadataReferences.Add(MetadataReference.CreateFromFile(typeof(EventHandler).Assembly.Location));
                RoslynAssert.CodeFix<ClassMustHaveEventAnalyzer, InsertEventFixProvider>(code, fixedCode);
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

                var exception = Assert.Throws<AssertException>(() => RoslynAssert.CodeFix<FieldNameMustNotBeginWithUnderscore, DontUseUnderscoreCodeFixProvider>(code, null));
                var expected = "Expected and actual diagnostics do not match.\r\n" +
                               "Expected:\r\n" +
                               "SA1309 \r\n" +
                               "  at line 5 and character 16 in file Foo.cs | private ↓readonly int _value1;\r\n" +
                               "Actual:\r\n" +
                               "SA1309 Field '_value1' must not begin with an underscore\r\n" +
                               "  at line 5 and character 29 in file Foo.cs | private readonly int ↓_value1;\r\n";
                Assert.AreEqual(expected, exception.Message);
            }
        }
    }
}
