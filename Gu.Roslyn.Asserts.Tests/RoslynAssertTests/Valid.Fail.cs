// ReSharper disable RedundantNameQualifier
namespace Gu.Roslyn.Asserts.Tests.RoslynAssertTests
{
    using System.Collections.Generic;
    using NUnit.Framework;

    public static partial class Valid
    {
        public static class Fail
        {
            [Test]
            public static void SingleDocumentFieldNameMustNotBeginWithUnderscore()
            {
                var code = @"
namespace N
{
    class C
    {
        private readonly int _value = 1;

        public int M() => _value;
    }
}";
                var expected = "Expected no diagnostics, found:\r\n" +
                               "SA1309 Field '_value' must not begin with an underscore\r\n" +
                               "  at line 5 and character 29 in file C.cs | private readonly int ↓_value = 1;\r\n";
                var analyzer = new FieldNameMustNotBeginWithUnderscore();
                var exception = Assert.Throws<AssertException>(() => RoslynAssert.Valid(analyzer, code));
                Assert.AreEqual(expected, exception.Message);

                exception = Assert.Throws<AssertException>(() => RoslynAssert.Valid(typeof(FieldNameMustNotBeginWithUnderscore), code));
                Assert.AreEqual(expected, exception.Message);
            }

            [Test]
            public static void SingleDocumentFieldNameMustNotBeginWithUnderscoreDisabled()
            {
                var code = @"
namespace N
{
    class C
    {
        private readonly int _value = 1;

        public int M() => _value;
    }
}";
                var expected = "Expected no diagnostics, found:\r\n" +
                               "SA13090 Field \'_value\' must not begin with an underscore\r\n" +
                               "  at line 5 and character 29 in file C.cs | private readonly int ↓_value = 1;\r\n";

                var analyzer = new FieldNameMustNotBeginWithUnderscoreDisabled();
                var exception = Assert.Throws<AssertException>(() => RoslynAssert.Valid(analyzer, code));
                Assert.AreEqual(expected, exception.Message);

                exception = Assert.Throws<AssertException>(() => RoslynAssert.Valid(typeof(FieldNameMustNotBeginWithUnderscoreDisabled), code));
                Assert.AreEqual(expected, exception.Message);
            }

            [Test]
            public static void TwoDocumentsOneError()
            {
                var c1 = @"
namespace N
{
    class C1
    {
        private readonly int _value = 1;

        public int M() => _value;
    }
}";
                var c2 = @"
namespace N
{
    class C2
    {
    }
}";
                var expected = "Expected no diagnostics, found:\r\n" +
                               "SA1309 Field '_value' must not begin with an underscore\r\n" +
                               "  at line 5 and character 29 in file C1.cs | private readonly int ↓_value = 1;\r\n";

                var analyzer = new FieldNameMustNotBeginWithUnderscore();
                var exception = Assert.Throws<AssertException>(() => RoslynAssert.Valid(analyzer, c1, c2));
                Assert.AreEqual(expected, exception.Message);

                exception = Assert.Throws<AssertException>(() => RoslynAssert.Valid(typeof(FieldNameMustNotBeginWithUnderscore), c1, c2));
                Assert.AreEqual(expected, exception.Message);
            }

            [Test]
            public static void TwoDocumentsTwoErrors()
            {
                var c1 = @"
namespace N
{
    class C1
    {
        private readonly int _value1 = 1;
    }
}";
                var c2 = @"
namespace N
{
    class C2
    {
        private readonly int _value2 = 2;
    }
}";
                var analyzer = new FieldNameMustNotBeginWithUnderscore();
                var exception = Assert.Throws<AssertException>(() => RoslynAssert.Valid(analyzer, c1, c2));
                StringAssert.Contains("SA1309 Field '_value1' must not begin with an underscore", exception.Message);
                StringAssert.Contains("SA1309 Field '_value2' must not begin with an underscore", exception.Message);

                exception = Assert.Throws<AssertException>(() => RoslynAssert.Valid(typeof(FieldNameMustNotBeginWithUnderscore), c1, c2));
                StringAssert.Contains("SA1309 Field '_value1' must not begin with an underscore", exception.Message);
                StringAssert.Contains("SA1309 Field '_value2' must not begin with an underscore", exception.Message);
            }

            [Test]
            public static void TwoProjectsTwoErrors()
            {
                var c1 = @"
namespace Project1
{
    class C1
    {
        private readonly int _value1 = 1;
    }
}";
                var c2 = @"
namespace Project2
{
    class C2
    {
        private readonly int _value2 = 2;
    }
}";
                var analyzer = new FieldNameMustNotBeginWithUnderscore();
                var exception = Assert.Throws<AssertException>(() => RoslynAssert.Valid(analyzer, c1, c2));
                StringAssert.Contains("SA1309 Field '_value1' must not begin with an underscore", exception.Message);
                StringAssert.Contains("SA1309 Field '_value2' must not begin with an underscore", exception.Message);

                exception = Assert.Throws<AssertException>(() => RoslynAssert.Valid(analyzer, new List<string> { c1, c2 }));
                StringAssert.Contains("SA1309 Field '_value1' must not begin with an underscore", exception.Message);
                StringAssert.Contains("SA1309 Field '_value2' must not begin with an underscore", exception.Message);

                exception = Assert.Throws<AssertException>(() => RoslynAssert.Valid(typeof(FieldNameMustNotBeginWithUnderscore), c1, c2));
                StringAssert.Contains("SA1309 Field '_value1' must not begin with an underscore", exception.Message);
                StringAssert.Contains("SA1309 Field '_value2' must not begin with an underscore", exception.Message);
            }

            [Test]
            public static void ClassLibrary1ProjectFileFieldNameMustNotBeginWithUnderscoreDisabled()
            {
                var code = ProjectFile.Find("ClassLibrary1.csproj");
                var expected = "Expected no diagnostics, found:\r\n" +
                               "SA13090 Field \'_value\' must not begin with an underscore\r\n" +
                              $"  at line 5 and character 20 in file {code.DirectoryName}\\ClassLibrary1Class1.cs | private int ↓_value;";
                var analyzer = new FieldNameMustNotBeginWithUnderscoreDisabled();

                var exception = Assert.Throws<AssertException>(() => RoslynAssert.Valid(analyzer, code));
                StringAssert.Contains(expected, exception.Message);

                exception = Assert.Throws<AssertException>(() => RoslynAssert.Valid(typeof(FieldNameMustNotBeginWithUnderscoreDisabled), code));
                StringAssert.Contains(expected, exception.Message);

                var descriptor = FieldNameMustNotBeginWithUnderscoreDisabled.Descriptor;

                exception = Assert.Throws<AssertException>(() => RoslynAssert.Valid(typeof(FieldNameMustNotBeginWithUnderscoreDisabled), descriptor, code));
                StringAssert.Contains(expected, exception.Message);

                exception = Assert.Throws<AssertException>(() => RoslynAssert.Valid(analyzer, descriptor, code));
                StringAssert.Contains(expected, exception.Message);

                exception = Assert.Throws<AssertException>(() => RoslynAssert.Valid(analyzer, code, Settings.Default));
                StringAssert.Contains(expected, exception.Message);
            }

            [Test]
            public static void ClassLibrary2ProjectFileFieldNameMustNotBeginWithUnderscoreDisabled()
            {
                var code = ProjectFile.Find("ClassLibrary2.csproj");
                var expected = "Expected no diagnostics, found:\r\n" +
                               "SA13090 Field \'_value\' must not begin with an underscore\r\n" +
                               $"  at line 5 and character 20 in file {code.DirectoryName}\\ClassLibrary2Class1.cs | private int ↓_value;";
                var analyzer = new FieldNameMustNotBeginWithUnderscoreDisabled();
                var exception = Assert.Throws<AssertException>(() => RoslynAssert.Valid(analyzer, code));
                StringAssert.Contains(expected, exception.Message);

                exception = Assert.Throws<AssertException>(() => RoslynAssert.Valid(analyzer.GetType(), code));
                StringAssert.Contains(expected, exception.Message);
            }

            [Test]
            public static void ClassLibrary1SolutionFileFieldNameMustNotBeginWithUnderscoreDisabled()
            {
                var code = ProjectFile.Find("ClassLibrary1.csproj");
                var expected = "Expected no diagnostics, found:\r\n" +
                               "SA13090 Field \'_value\' must not begin with an underscore\r\n" +
                               $"  at line 5 and character 20 in file {code.DirectoryName}\\ClassLibrary1Class1.cs | private int ↓_value;";
                var solution = CodeFactory.CreateSolution(code);
                var analyzer = new FieldNameMustNotBeginWithUnderscoreDisabled();
                var exception = Assert.Throws<AssertException>(() => RoslynAssert.Valid(analyzer, solution));
                StringAssert.Contains(expected, exception.Message);

                exception = Assert.Throws<AssertException>(() => RoslynAssert.Valid(analyzer.GetType(), solution));
                StringAssert.Contains(expected, exception.Message);
            }

            [Test]
            public static void ClassLibrary2SolutionFileFieldNameMustNotBeginWithUnderscoreDisabled()
            {
                var code = ProjectFile.Find("ClassLibrary2.csproj");
                var expected = "Expected no diagnostics, found:\r\n" +
                               "SA13090 Field \'_value\' must not begin with an underscore\r\n" +
                               $"  at line 5 and character 20 in file {code.DirectoryName}\\ClassLibrary2Class1.cs | private int ↓_value;";
                var solution = CodeFactory.CreateSolution(code);
                var analyzer = new FieldNameMustNotBeginWithUnderscoreDisabled();
                var exception = Assert.Throws<AssertException>(() => RoslynAssert.Valid(analyzer, solution));
                StringAssert.Contains(expected, exception.Message);

                exception = Assert.Throws<AssertException>(() => RoslynAssert.Valid(analyzer.GetType(), solution));
                StringAssert.Contains(expected, exception.Message);
            }

            [Test]
            public static void WithExpectedDiagnosticWithWrongId()
            {
                var code = @"
namespace N
{
    class C
    {
        private readonly int value1;
    }
}";
                var expected = "SyntaxNodeAnalyzer does not produce a diagnostic with ID 'ID2'.\r\n" +
                               "SyntaxNodeAnalyzer.SupportedDiagnostics: 'ID1'.\r\n" +
                               "The expected diagnostic is: 'ID2'.";

                var descriptor = Descriptors.Id2;
                var analyzer = new SyntaxNodeAnalyzer(Descriptors.Id1);
                var exception = Assert.Throws<AssertException>(() => RoslynAssert.Valid(analyzer, descriptor, code));
                Assert.AreEqual(expected, exception.Message);
            }

            [Test]
            public static void WithExpectedDiagnosticWhenOneReportsError()
            {
                var code = @"
namespace N
{
    class C
    {
        private readonly int wrongName = 1;
        
        public int WrongName { get; set; }

        public int M() => this.wrongName;
    }
}";

                var descriptor = FieldAndPropertyMustBeNamedValueAnalyzer.FieldDescriptor;
                var expected = "Expected no diagnostics, found:\r\n" +
                                  "Field Message format\r\n" +
                                  "  at line 5 and character 29 in file C.cs | private readonly int ↓wrongName = 1;\r\n";
                var analyzer = new FieldAndPropertyMustBeNamedValueAnalyzer();
                var exception = Assert.Throws<AssertException>(() => RoslynAssert.Valid(analyzer, descriptor, code));
                Assert.AreEqual(expected, exception.Message);

                exception = Assert.Throws<AssertException>(() => RoslynAssert.Valid(analyzer.GetType(), descriptor, code));
                Assert.AreEqual(expected, exception.Message);
            }

            [Test]
            public static void WhenCompilerWarnings()
            {
                var code = @"
namespace N
{
    public class C
    {
        public string M() => null;
    }
}";
                var expected = "Expected no diagnostics, found:\r\n" +
                               "CS8603 Possible null reference return.\r\n" +
                               "  at line 5 and character 29 in file C.cs | public string M() => ↓null;\r\n";
                var analyzer = new NopAnalyzer();
                var exception = Assert.Throws<AssertException>(() => RoslynAssert.Valid(analyzer, code));
                Assert.AreEqual(expected, exception.Message);

                exception = Assert.Throws<AssertException>(() => RoslynAssert.Valid(analyzer, analyzer.SupportedDiagnostics[0], code));
                Assert.AreEqual(expected, exception.Message);

                exception = Assert.Throws<AssertException>(() => RoslynAssert.Valid(analyzer.GetType(), code));
                Assert.AreEqual(expected, exception.Message);
            }
        }
    }
}
