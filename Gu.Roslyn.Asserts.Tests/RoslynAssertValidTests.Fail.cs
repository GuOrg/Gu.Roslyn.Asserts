// ReSharper disable RedundantNameQualifier
namespace Gu.Roslyn.Asserts.Tests
{
    using System.Collections.Generic;
    using NUnit.Framework;

    [TestFixture]
    public static partial class RoslynAssertValidTests
    {
        public static class Fail
        {
            [Test]
            public static void SingleDocumentFieldNameMustNotBeginWithUnderscore()
            {
                var code = @"
namespace RoslynSandbox
{
    class Foo
    {
        private readonly int _value = 1;
    }
}";
                var expected = "Expected no diagnostics, found:\r\n" +
                               "SA1309 Field '_value' must not begin with an underscore\r\n" +
                               "  at line 5 and character 29 in file Foo.cs | private readonly int ↓_value = 1;\r\n";
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
namespace RoslynSandbox
{
    class Foo
    {
        private readonly int _value = 1;
    }
}";
                var expected = "Expected no diagnostics, found:\r\n" +
                               "SA13090 Field \'_value\' must not begin with an underscore\r\n" +
                               "  at line 5 and character 29 in file Foo.cs | private readonly int ↓_value = 1;\r\n";

                var analyzer = new FieldNameMustNotBeginWithUnderscoreDisabled();
                var exception = Assert.Throws<AssertException>(() => RoslynAssert.Valid(analyzer, code));
                Assert.AreEqual(expected, exception.Message);

                exception = Assert.Throws<AssertException>(() => RoslynAssert.Valid(typeof(FieldNameMustNotBeginWithUnderscoreDisabled), code));
                Assert.AreEqual(expected, exception.Message);
            }

            [Test]
            public static void TwoDocumentsOneError()
            {
                var foo1 = @"
namespace RoslynSandbox
{
    class Foo1
    {
        private readonly int _value = 1;
    }
}";
                var foo2 = @"
namespace RoslynSandbox
{
    class Foo2
    {
    }
}";
                var expected = "Expected no diagnostics, found:\r\n" +
                               "SA1309 Field '_value' must not begin with an underscore\r\n" +
                               "  at line 5 and character 29 in file Foo1.cs | private readonly int ↓_value = 1;\r\n";

                var analyzer = new FieldNameMustNotBeginWithUnderscore();
                var exception = Assert.Throws<AssertException>(() => RoslynAssert.Valid(analyzer, foo1, foo2));
                Assert.AreEqual(expected, exception.Message);

                exception = Assert.Throws<AssertException>(() => RoslynAssert.Valid(typeof(FieldNameMustNotBeginWithUnderscore), foo1, foo2));
                Assert.AreEqual(expected, exception.Message);
            }

            [Test]
            public static void TwoDocumentsTwoErrors()
            {
                var foo1 = @"
namespace RoslynSandbox
{
    class Foo1
    {
        private readonly int _value1 = 1;
    }
}";
                var foo2 = @"
namespace RoslynSandbox
{
    class Foo2
    {
        private readonly int _value2 = 2;
    }
}";
                var analyzer = new FieldNameMustNotBeginWithUnderscore();
                var exception = Assert.Throws<AssertException>(() => RoslynAssert.Valid(analyzer, foo1, foo2));
                StringAssert.Contains("SA1309 Field '_value1' must not begin with an underscore", exception.Message);
                StringAssert.Contains("SA1309 Field '_value2' must not begin with an underscore", exception.Message);

                exception = Assert.Throws<AssertException>(() => RoslynAssert.Valid(typeof(FieldNameMustNotBeginWithUnderscore), foo1, foo2));
                StringAssert.Contains("SA1309 Field '_value1' must not begin with an underscore", exception.Message);
                StringAssert.Contains("SA1309 Field '_value2' must not begin with an underscore", exception.Message);
            }

            [Test]
            public static void TwoProjectsTwoErrors()
            {
                var foo1 = @"
namespace Project1
{
    class Foo1
    {
        private readonly int _value1 = 1;
    }
}";
                var foo2 = @"
namespace Project2
{
    class Foo2
    {
        private readonly int _value2 = 2;
    }
}";
                var analyzer = new FieldNameMustNotBeginWithUnderscore();
                var exception = Assert.Throws<AssertException>(() => RoslynAssert.Valid(analyzer, foo1, foo2));
                StringAssert.Contains("SA1309 Field '_value1' must not begin with an underscore", exception.Message);
                StringAssert.Contains("SA1309 Field '_value2' must not begin with an underscore", exception.Message);

                exception = Assert.Throws<AssertException>(() => RoslynAssert.Valid(analyzer, new List<string> { foo1, foo2 }));
                StringAssert.Contains("SA1309 Field '_value1' must not begin with an underscore", exception.Message);
                StringAssert.Contains("SA1309 Field '_value2' must not begin with an underscore", exception.Message);

                exception = Assert.Throws<AssertException>(() => RoslynAssert.Valid(typeof(FieldNameMustNotBeginWithUnderscore), foo1, foo2));
                StringAssert.Contains("SA1309 Field '_value1' must not begin with an underscore", exception.Message);
                StringAssert.Contains("SA1309 Field '_value2' must not begin with an underscore", exception.Message);
            }

            [Test]
            public static void ClassLibrary1ProjectFileFieldNameMustNotBeginWithUnderscoreDisabled()
            {
                var code = ProjectFile.Find("ClassLibrary1.csproj");
                var expected = "Expected no diagnostics, found:\r\n" +
                               "SA13090 Field \'_value\' must not begin with an underscore\r\n" +
                              $"  at line 9 and character 20 in file {code.DirectoryName}\\ClassLibrary1Class1.cs | private int ↓_value;";
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

                exception = Assert.Throws<AssertException>(() => RoslynAssert.Valid(analyzer, code, compilationOptions: CodeFactory.DefaultCompilationOptions(analyzer, descriptor, null)));
                StringAssert.Contains(expected, exception.Message);
            }

            [Test]
            public static void ClassLibrary2ProjectFileFieldNameMustNotBeginWithUnderscoreDisabled()
            {
                var code = ProjectFile.Find("ClassLibrary2.csproj");
                var expected = "Expected no diagnostics, found:\r\n" +
                               "SA13090 Field \'_value\' must not begin with an underscore\r\n" +
                               $"  at line 9 and character 20 in file {code.DirectoryName}\\ClassLibrary2Class1.cs | private int ↓_value;";
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
                               $"  at line 9 and character 20 in file {code.DirectoryName}\\ClassLibrary1Class1.cs | private int ↓_value;";
                var solution = CodeFactory.CreateSolution(code, new[] { new FieldNameMustNotBeginWithUnderscoreDisabled() }, RoslynAssert.MetadataReferences);
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
                               $"  at line 9 and character 20 in file {code.DirectoryName}\\ClassLibrary2Class1.cs | private int ↓_value;";
                var solution = CodeFactory.CreateSolution(code, new[] { new FieldNameMustNotBeginWithUnderscoreDisabled() }, RoslynAssert.MetadataReferences);
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
namespace RoslynSandbox
{
    class Foo
    {
        private readonly int value1;
    }
}";
                var expected = "Analyzer Gu.Roslyn.Asserts.Tests.FieldNameMustNotBeginWithUnderscore does not produce a diagnostic with ID NoError.\r\n" +
                               "The analyzer produces the following diagnostics: {SA1309}\r\n" +
                               "The expected diagnostic is: NoError";

                var descriptor = NoErrorAnalyzer.Descriptor;
                var analyzer = new FieldNameMustNotBeginWithUnderscore();
                var exception = Assert.Throws<AssertException>(() => RoslynAssert.Valid(analyzer, descriptor, code));
                Assert.AreEqual(expected, exception.Message);

                exception = Assert.Throws<AssertException>(() => RoslynAssert.Valid(analyzer.GetType(), descriptor, code));
                Assert.AreEqual(expected, exception.Message);
            }

            [Test]
            public static void WithExpectedDiagnosticWhenOneReportsError()
            {
                var code = @"
namespace RoslynSandbox
{
    class Foo
    {
        private readonly int wrongName;
        
        public int WrongName { get; set; }
    }
}";

                var descriptor = FieldAndPropertyMustBeNamedFooAnalyzer.FieldDescriptor;
                var expected = "Expected no diagnostics, found:\r\n" +
                                  "Field Message format.\r\n" +
                                  "  at line 5 and character 29 in file Foo.cs | private readonly int ↓wrongName;\r\n";
                var analyzer = new FieldAndPropertyMustBeNamedFooAnalyzer();
                var exception = Assert.Throws<AssertException>(() => RoslynAssert.Valid(analyzer, descriptor, code));
                Assert.AreEqual(expected, exception.Message);

                exception = Assert.Throws<AssertException>(() => RoslynAssert.Valid(analyzer.GetType(), descriptor, code));
                Assert.AreEqual(expected, exception.Message);
            }

            [Test]
            public static void DuplicateId()
            {
                var descriptor = DuplicateIdAnalyzer.Descriptor1;
                var expected = "Analyzer Gu.Roslyn.Asserts.Tests.DuplicateIdAnalyzer has more than one diagnostic with ID 0.";
                var analyzer = new DuplicateIdAnalyzer();

                var exception = Assert.Throws<AssertException>(() => RoslynAssert.Valid(analyzer, descriptor, string.Empty));
                Assert.AreEqual(expected, exception.Message);

                exception = Assert.Throws<AssertException>(() => RoslynAssert.Valid(analyzer.GetType(), descriptor, string.Empty));
                Assert.AreEqual(expected, exception.Message);
            }
        }
    }
}
