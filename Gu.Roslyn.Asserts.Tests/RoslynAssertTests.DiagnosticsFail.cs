// ReSharper disable RedundantNameQualifier
namespace Gu.Roslyn.Asserts.Tests
{
    using System;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using NUnit.Framework;

    [TestFixture]
    public static partial class RoslynAssertTests
    {
        public static class DiagnosticsFail
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
            public static void MessageDoNotMatch()
            {
                var code = @"
namespace N
{
    class C
    {
        private int ↓_value;
    }
}";
                var expected = "Expected and actual messages do not match.\r\n" +
                               "Expected: WRONG\r\n" +
                               "Actual:   Field '_value' must not begin with an underscore\r\n" +
                               "          ^\r\n";

                var expectedDiagnostic = ExpectedDiagnostic.Create(FieldNameMustNotBeginWithUnderscore.DiagnosticId, "WRONG");
                var analyzer = new FieldNameMustNotBeginWithUnderscore();
                var exception = Assert.Throws<AssertException>(() => RoslynAssert.Diagnostics(analyzer, expectedDiagnostic, code));
                CodeAssert.AreEqual(expected, exception.Message);
            }

            [Test]
            public static void NoErrorIndicated()
            {
                var code = @"
namespace N
{
    class C
    {
    }
}";
                var expected = "Expected code to have at least one error position indicated with '↓'";
                var analyzer = new FieldNameMustNotBeginWithUnderscore();
                var exception = Assert.Throws<InvalidOperationException>(() => RoslynAssert.Diagnostics(analyzer, code));
                Assert.AreEqual(expected, exception.Message);
            }

            [Test]
            public static void NoErrorIndicatedNopAnalyzer()
            {
                var code = @"
namespace N
{
    class C
    {
    }
}";
                var expected = "Expected code to have at least one error position indicated with '↓'";
                var exception = Assert.Throws<InvalidOperationException>(() => RoslynAssert.Diagnostics(new NopAnalyzer(), code));
                Assert.AreEqual(expected, exception.Message);
            }

            [Test]
            public static void NoDiagnosticOrError()
            {
                var code = @"
namespace N
{
    class C
    {
        private int ↓i;
        
        C(int i)
        {
            this.i = i;
        }

        public override string ToString() => this.i.ToString();
    }
}";
                var expected = "Expected and actual diagnostics do not match.\r\n" +
                               "Expected:\r\n" +
                               "SA1309 \r\n" +
                               "  at line 5 and character 20 in file C.cs | private int ↓i;\r\n" +
                               "Actual: <no errors>\r\n";
                var analyzer = new FieldNameMustNotBeginWithUnderscore();
                var exception = Assert.Throws<AssertException>(() => RoslynAssert.Diagnostics(analyzer, code));
                Assert.AreEqual(expected, exception.Message);
            }

            [Test]
            public static void NoDiagnosticButSyntaxError()
            {
                var code = @"
namespace N
{
    class C
    {
        private int ↓i = SYNTAX_ERROR;
    }
}";
                var expected = "Expected and actual diagnostics do not match.\r\n" +
                               "Expected:\r\n" +
                               "SA1309 \r\n" +
                               "  at line 5 and character 20 in file C.cs | private int ↓i = SYNTAX_ERROR;\r\n" +
                               "Actual:\r\n" +
                               "CS0103 The name 'SYNTAX_ERROR' does not exist in the current context\r\n" +
                               "  at line 5 and character 24 in file C.cs | private int i = ↓SYNTAX_ERROR;\r\n";
                var analyzer = new FieldNameMustNotBeginWithUnderscore();
                var exception = Assert.Throws<AssertException>(() => RoslynAssert.Diagnostics(analyzer, code));
                Assert.AreEqual(expected, exception.Message);
            }

            [Test]
            public static void TwoDocumentsNoErrorIndicated()
            {
                var code1 = @"
namespace N
{
    class Code1
    {
    }
}";
                var code2 = @"
namespace N
{
    class Code2
    {
    }
}";
                var expected = "Expected code to have at least one error position indicated with '↓'";
                var analyzer = new FieldNameMustNotBeginWithUnderscore();
                var exception = Assert.Throws<InvalidOperationException>(() => RoslynAssert.Diagnostics(analyzer, code1, code2));
                Assert.AreEqual(expected, exception.Message);
            }

            [Test]
            public static void TwoDocumentsNoErrorInCode()
            {
                var code1 = @"
namespace N
{
    ↓class Code1
    {
    }
}";
                var code2 = @"
namespace N
{
    class Code2
    {
    }
}";
                var analyzer = new FieldNameMustNotBeginWithUnderscore();
                var exception = Assert.Throws<AssertException>(() => RoslynAssert.Diagnostics(analyzer, code1, code2));
                var expected = "Expected and actual diagnostics do not match.\r\n" +
                               "Expected:\r\n" +
                               "SA1309 \r\n" +
                               "  at line 3 and character 4 in file Code1.cs | ↓class Code1\r\n" +
                               "Actual: <no errors>\r\n";
                Assert.AreEqual(expected, exception.Message);
            }

            [Test]
            public static void IndicatedAndActualPositionDoNotMatchFieldNameMustNotBeginWithUnderscore()
            {
                var code = @"
namespace N
{
    class C
    {
        private ↓readonly int _value1;
    }
}";
                var expected = "Expected and actual diagnostics do not match.\r\n" +
                               "Expected:\r\n" +
                               "SA1309 \r\n" +
                               "  at line 5 and character 16 in file C.cs | private ↓readonly int _value1;\r\n" +
                               "Actual:\r\n" +
                               "SA1309 Field '_value1' must not begin with an underscore\r\n" +
                               "  at line 5 and character 29 in file C.cs | private readonly int ↓_value1;\r\n";
                var analyzer = new FieldNameMustNotBeginWithUnderscore();
                var exception = Assert.Throws<AssertException>(() => RoslynAssert.Diagnostics(analyzer, code));
                Assert.AreEqual(expected, exception.Message);
            }

            [Test]
            public static void IndicatedAndActualPositionDoNotMatchFieldNameMustNotBeginWithUnderscoreWithExpectedDiagnostic()
            {
                var code = @"
namespace N
{
    class C
    {
        private ↓readonly int _value1;
    }
}";
                var expected = "Expected and actual diagnostics do not match.\r\n" +
                               "Expected:\r\n" +
                               "SA1309 \r\n" +
                               "  at line 5 and character 16 in file C.cs | private ↓readonly int _value1;\r\n" +
                               "Actual:\r\n" +
                               "SA1309 Field '_value1' must not begin with an underscore\r\n" +
                               "  at line 5 and character 29 in file C.cs | private readonly int ↓_value1;\r\n";

                var expectedDiagnostic = ExpectedDiagnostic.CreateFromCodeWithErrorsIndicated("SA1309", code, out code);
                var analyzer = new FieldNameMustNotBeginWithUnderscore();
                var exception = Assert.Throws<AssertException>(() => RoslynAssert.Diagnostics(analyzer, expectedDiagnostic, code));
                Assert.AreEqual(expected, exception.Message);

                exception = Assert.Throws<AssertException>(() => RoslynAssert.Diagnostics(analyzer, new[] { expectedDiagnostic }, code));
                Assert.AreEqual(expected, exception.Message);
            }

            [Test]
            public static void WithExpectedDiagnosticWithWrongId()
            {
                var code = @"
namespace N
{
    class C
    {
        private readonly int value;
    }
}";
                var expected = "FieldNameMustNotBeginWithUnderscore does not produce a diagnostic with ID 'WRONG'.\r\n" +
                               "FieldNameMustNotBeginWithUnderscore.SupportedDiagnostics: 'SA1309'.\r\n" +
                               "The expected diagnostic is: 'WRONG'.";

                var expectedDiagnostic = ExpectedDiagnostic.Create("WRONG");

                var analyzer = new FieldNameMustNotBeginWithUnderscore();
                var exception = Assert.Throws<AssertException>(() => RoslynAssert.Diagnostics(analyzer, expectedDiagnostic, code));
                Assert.AreEqual(expected, exception.Message);

                exception = Assert.Throws<AssertException>(() => RoslynAssert.Diagnostics(analyzer, new[] { expectedDiagnostic }, code));
                Assert.AreEqual(expected, exception.Message);
            }

            [Test]
            public static void WithExpectedDiagnosticWithWrongMessage()
            {
                var code = @"
namespace N
{
    class C
    {
        private readonly int _value1;
    }
}";
                var expected = "Expected and actual messages do not match.\r\n" +
                               "Expected: WRONG MESSAGE\r\n" +
                               "Actual:   Field \'_value1\' must not begin with an underscore\r\n" +
                               "          ^\r\n";

                var expectedDiagnostic = ExpectedDiagnostic.Create("SA1309", "WRONG MESSAGE");
                var analyzer = new FieldNameMustNotBeginWithUnderscore();
                var exception = Assert.Throws<AssertException>(() => RoslynAssert.Diagnostics(analyzer, expectedDiagnostic, code));
                Assert.AreEqual(expected, exception.Message);

                exception = Assert.Throws<AssertException>(() => RoslynAssert.Diagnostics(analyzer, new[] { expectedDiagnostic }, code));
                Assert.AreEqual(expected, exception.Message);
            }

            [Test]
            public static void WithExpectedDiagnosticWithWrongPosition()
            {
                var code = @"
namespace N
{
    class C
    {
        private readonly int _value1;
    }
}";
                var expected = "Expected and actual diagnostics do not match.\r\n" +
                               "Expected:\r\n" +
                               "SA1309 Field '_value1' must not begin with an underscore\r\n" +
                               "  at line 5 and character 8 in file C.cs | ↓private readonly int _value1;\r\n" +
                               "Actual:\r\n" +
                               "SA1309 Field '_value1' must not begin with an underscore\r\n" +
                               "  at line 5 and character 29 in file C.cs | private readonly int ↓_value1;\r\n";

                var expectedDiagnostic = ExpectedDiagnostic.Create("SA1309", "Field '_value1' must not begin with an underscore", 5, 8);
                var analyzer = new FieldNameMustNotBeginWithUnderscore();
                var exception = Assert.Throws<AssertException>(() => RoslynAssert.Diagnostics(analyzer, expectedDiagnostic, code));
                Assert.AreEqual(expected, exception.Message);

                exception = Assert.Throws<AssertException>(() => RoslynAssert.Diagnostics(analyzer, new[] { expectedDiagnostic }, code));
                Assert.AreEqual(expected, exception.Message);
            }

            [Test]
            public static void TwoDocumentsExpectedDiagnosticWithoutPath()
            {
                var code1 = @"
namespace N
{
    class Code1
    {
    }
}";
                var code2 = @"
namespace N
{
    class Code2
    {
    }
}";
                var expected = "Expected diagnostic must specify path when more than one document is tested.\r\n" +
                               "Either specify path or indicate expected error position with ↓";

                var expectedDiagnostic = ExpectedDiagnostic.Create("SA1309", "ANY", 1, 2);
                var analyzer = new FieldNameMustNotBeginWithUnderscore();
                var exception = Assert.Throws<InvalidOperationException>(() => RoslynAssert.Diagnostics(analyzer, expectedDiagnostic, code1, code2));
                Assert.AreEqual(expected, exception.Message);

                exception = Assert.Throws<InvalidOperationException>(() => RoslynAssert.Diagnostics(analyzer, new[] { expectedDiagnostic }, code1, code2));
                Assert.AreEqual(expected, exception.Message);
            }

            [Test]
            public static void IndicatedAndActualPositionDoNotMatchFieldNameMustNotBeginWithUnderscoreWithExpectedDiagnosticWithMessageWrongPosition()
            {
                var code = @"
namespace N
{
    class C
    {
        private ↓readonly int _value1;
    }
}";
                var expected = "Expected and actual diagnostics do not match.\r\n" +
                               "Expected:\r\n" +
                               "SA1309 Field '_value1' must not begin with an underscore\r\n" +
                               "  at line 5 and character 16 in file C.cs | private ↓readonly int _value1;\r\n" +
                               "Actual:\r\n" +
                               "SA1309 Field '_value1' must not begin with an underscore\r\n" +
                               "  at line 5 and character 29 in file C.cs | private readonly int ↓_value1;\r\n";

                var expectedDiagnostic = ExpectedDiagnostic.CreateFromCodeWithErrorsIndicated("SA1309", "Field '_value1' must not begin with an underscore", code, out code);
                var analyzer = new FieldNameMustNotBeginWithUnderscore();
                var exception = Assert.Throws<AssertException>(() => RoslynAssert.Diagnostics(analyzer, expectedDiagnostic, code));
                Assert.AreEqual(expected, exception.Message);

                exception = Assert.Throws<AssertException>(() => RoslynAssert.Diagnostics(analyzer, new[] { expectedDiagnostic }, code));
                Assert.AreEqual(expected, exception.Message);
            }

            [Test]
            public static void IndicatedAndActualPositionDoNotMatchWithWrongMessage()
            {
                var code = @"
namespace N
{
    class C
    {
        ↓private readonly int _value1;
    }
}";
                var expected = "Expected and actual diagnostics do not match.\r\n" +
                               "Expected:\r\n" +
                               "SA1309 Wrong message\r\n" +
                               "  at line 5 and character 8 in file C.cs | ↓private readonly int _value1;\r\n" +
                               "Actual:\r\n" +
                               "SA1309 Field '_value1' must not begin with an underscore\r\n" +
                               "  at line 5 and character 29 in file C.cs | private readonly int ↓_value1;\r\n";

                var expectedDiagnostic = ExpectedDiagnostic.CreateFromCodeWithErrorsIndicated("SA1309", "Wrong message", code, out code);

                var analyzer = new FieldNameMustNotBeginWithUnderscore();
                var exception = Assert.Throws<AssertException>(() => RoslynAssert.Diagnostics(analyzer, expectedDiagnostic, code));
                Assert.AreEqual(expected, exception.Message);

                exception = Assert.Throws<AssertException>(() => RoslynAssert.Diagnostics(analyzer, new[] { expectedDiagnostic }, code));
                Assert.AreEqual(expected, exception.Message);
            }

            [Test]
            public static void IndicatedAndActualPositionDoNotMatchFieldNameMustNotBeginWithUnderscoreDisabled()
            {
                var code = @"
namespace N
{
    class C
    {
        private ↓readonly int _value1;
    }
}";
                var expected = "Expected and actual diagnostics do not match.\r\n" +
                               "Expected:\r\n" +
                               "SA13090 \r\n" +
                               "  at line 5 and character 16 in file C.cs | private ↓readonly int _value1;\r\n" +
                               "Actual:\r\n" +
                               "SA13090 Field '_value1' must not begin with an underscore\r\n" +
                               "  at line 5 and character 29 in file C.cs | private readonly int ↓_value1;\r\n";
                var analyzer = new FieldNameMustNotBeginWithUnderscoreDisabled();
                var exception = Assert.Throws<AssertException>(() => RoslynAssert.Diagnostics(analyzer, code));
                Assert.AreEqual(expected, exception.Message);
            }

            [Test]
            public static void TwoErrorsOnlyOneIndicated()
            {
                var code = @"
namespace N
{
    class C
    {
        private readonly int ↓_value1;
        private readonly int _value2;
    }
}";
                var analyzer = new FieldNameMustNotBeginWithUnderscore();
                var exception = Assert.Throws<AssertException>(() => RoslynAssert.Diagnostics(analyzer, code));
                var expected = "Expected and actual diagnostics do not match.\r\n" +
                               "Actual:\r\n" +
                               "SA1309 Field '_value2' must not begin with an underscore\r\n" +
                               "  at line 6 and character 29 in file C.cs | private readonly int ↓_value2;\r\n";
                Assert.AreEqual(expected, exception.Message);
            }

            [Test]
            public static void TwoDocumentsIndicatedAndActualPositionDoNotMatch()
            {
                var code1 = @"
namespace N
{
    class C1
    {
        private readonly int _value1;
    }
}";

                var code2 = @"
namespace N
{
    class C2
    {
        private readonly int ↓value2;
    }
}";

                var analyzer = new FieldNameMustNotBeginWithUnderscore();
                var exception = Assert.Throws<AssertException>(() => RoslynAssert.Diagnostics(analyzer, code1, code2));
                var expected = "Expected and actual diagnostics do not match.\r\n" +
                               "Expected:\r\n" +
                               "SA1309 \r\n" +
                               "  at line 5 and character 29 in file C2.cs | private readonly int ↓value2;\r\n" +
                               "Actual:\r\n" +
                               "SA1309 Field '_value1' must not begin with an underscore\r\n" +
                               "  at line 5 and character 29 in file C1.cs | private readonly int ↓_value1;\r\n";
                Assert.AreEqual(expected, exception.Message);
            }

            [Test]
            public static void WhenEmpty()
            {
                var exception = Assert.Throws<AssertException>(() => RoslynAssert.Diagnostics(new SyntaxNodeAnalyzer(Array.Empty<DiagnosticDescriptor>(), SyntaxKind.IdentifierName), string.Empty));
                var expected = "SyntaxNodeAnalyzer.SupportedDiagnostics returns an empty array.";
                Assert.AreEqual(expected, exception.Message);
            }

            [Test]
            public static void WhenSingleNull()
            {
                var exception = Assert.Throws<AssertException>(() => RoslynAssert.Diagnostics(new SyntaxNodeAnalyzer(new DiagnosticDescriptor[] { null }, SyntaxKind.IdentifierName), string.Empty));
                var expected = "SyntaxNodeAnalyzer.SupportedDiagnostics[0] returns null.";
                Assert.AreEqual(expected, exception.Message);
            }

            [Test]
            public static void WhenMoreThanOne()
            {
                var analyzer = new SyntaxNodeAnalyzer(Descriptors.Id1, Descriptors.Id2);
                var exception = Assert.Throws<AssertException>(() => RoslynAssert.Diagnostics(analyzer, string.Empty));
                var expected = "This can only be used for analyzers with one SupportedDiagnostics.\r\n" +
                               "Prefer overload with ExpectedDiagnostic.";
                Assert.AreEqual(expected, exception.Message);
            }

            [Test]
            public static void DuplicateId()
            {
                var expected = "SyntaxNodeAnalyzer.SupportedDiagnostics has more than one descriptor with ID 'ID1'.";
                var analyzer = new SyntaxNodeAnalyzer(Descriptors.Id1, Descriptors.Id1Duplicate);
                var exception = Assert.Throws<AssertException>(() => RoslynAssert.Diagnostics(analyzer, ExpectedDiagnostic.Create(Descriptors.Id1), string.Empty));
                Assert.AreEqual(expected, exception.Message);
            }

            [Test]
            public static void WhenSyntaxError()
            {
                var code = @"
namespace N
{
    class C
    {
        private readonly int ↓_value = 1;
        INCOMPLETE
    }
}";
                var analyzer = new FieldNameMustNotBeginWithUnderscore();
                var expected = @"Found error.
CS1519 Invalid token '}' in class, struct, or interface member declaration
  at line 7 and character 4 in file C.cs | ↓}
";
                var exception = Assert.Throws<AssertException>(() => RoslynAssert.Diagnostics(analyzer, code));
                CodeAssert.AreEqual(expected, exception.Message);
            }
        }
    }
}
