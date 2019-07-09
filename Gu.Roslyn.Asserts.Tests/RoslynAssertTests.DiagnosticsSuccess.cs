namespace Gu.Roslyn.Asserts.Tests
{
    using Microsoft.CodeAnalysis;
    using NUnit.Framework;

    [TestFixture]
    public partial class RoslynAssertTests
    {
        public static class DiagnosticsSuccess
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
            public static void OneErrorIndicatedPosition()
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
                var expectedDiagnostic = ExpectedDiagnostic.Create(FieldNameMustNotBeginWithUnderscore.DiagnosticId);
                RoslynAssert.Diagnostics(analyzer, code);
                RoslynAssert.Diagnostics(analyzer, expectedDiagnostic, code);
            }

            [Test]
            public static void TwoErrorsSamePosition()
            {
                var code = @"
namespace RoslynSandbox
{
    class Foo
    {
        private readonly int ↓_value;
    }
}";
                var expectedDiagnostic1 = ExpectedDiagnostic.CreateFromCodeWithErrorsIndicated(
                    FieldNameMustNotBeginWithUnderscoreReportsTwo.DiagnosticId1,
                    code,
                    out _);

                var expectedDiagnostic2 = ExpectedDiagnostic.CreateFromCodeWithErrorsIndicated(
                    FieldNameMustNotBeginWithUnderscoreReportsTwo.DiagnosticId2,
                    code,
                    out code);
                var analyzer = new FieldNameMustNotBeginWithUnderscoreReportsTwo();
                RoslynAssert.Diagnostics(analyzer, new[] { expectedDiagnostic1, expectedDiagnostic2 }, code);
            }

            [Test]
            public static void OneErrorWithExpectedDiagnosticIdOnly()
            {
                var code = @"
namespace RoslynSandbox
{
    class Foo
    {
        private readonly int _value1;
    }
}";

                var expectedDiagnostic = ExpectedDiagnostic.Create("SA1309");
                RoslynAssert.Diagnostics(new FieldNameMustNotBeginWithUnderscore(), expectedDiagnostic, code);
            }

            [Test]
            public static void OneErrorWithExpectedDiagnosticIdAndMessage()
            {
                var code = @"
namespace RoslynSandbox
{
    class Foo
    {
        private readonly int _value1;
    }
}";

                var expectedDiagnostic = ExpectedDiagnostic.Create("SA1309", "Field '_value1' must not begin with an underscore");
                var analyzer = new FieldNameMustNotBeginWithUnderscore();
                RoslynAssert.Diagnostics(analyzer, expectedDiagnostic, code);
            }

            [Test]
            public static void OneErrorWithExpectedDiagnosticIdAndPosition()
            {
                var code = @"
namespace RoslynSandbox
{
    class Foo
    {
        private readonly int _value1;
    }
}";

                var expectedDiagnostic = ExpectedDiagnostic.Create("SA1309", 5, 29);
                var analyzer = new FieldNameMustNotBeginWithUnderscore();
                RoslynAssert.Diagnostics(analyzer, expectedDiagnostic, code);
            }

            [Test]
            public static void OneErrorWithExpectedDiagnostics()
            {
                var code = @"
namespace RoslynSandbox
{
    class Foo
    {
        private readonly int ↓_value1;
    }
}";

                var expectedDiagnostic = ExpectedDiagnostic.CreateFromCodeWithErrorsIndicated("SA1309", code, out code);
                var analyzer = new FieldNameMustNotBeginWithUnderscore();
                RoslynAssert.Diagnostics(analyzer, expectedDiagnostic, code);
                RoslynAssert.Diagnostics(analyzer, new[] { expectedDiagnostic }, code);
            }

            [Test]
            public static void OneErrorWithExpectedDiagnosticPositionFromIndicatedCode()
            {
                var code = @"
namespace RoslynSandbox
{
    class Foo
    {
        private readonly int ↓_value1;
    }
}";

                var expectedDiagnostic = ExpectedDiagnostic.CreateFromCodeWithErrorsIndicated("SA1309", code, out code);
                var analyzer = new FieldNameMustNotBeginWithUnderscore();
                RoslynAssert.Diagnostics(analyzer, expectedDiagnostic, code);
                RoslynAssert.Diagnostics(analyzer, new[] { expectedDiagnostic }, code);
            }

            [Test]
            public static void OneErrorWithExpectedDiagnosticWithMessageAndPosition()
            {
                var code = @"
namespace RoslynSandbox
{
    class Foo
    {
        private readonly int ↓_value1;
    }
}";

                var expectedDiagnostic = ExpectedDiagnostic.CreateFromCodeWithErrorsIndicated("SA1309", "Field '_value1' must not begin with an underscore", code, out code);

                var analyzer = new FieldNameMustNotBeginWithUnderscore();
                RoslynAssert.Diagnostics(analyzer, expectedDiagnostic, code);
                RoslynAssert.Diagnostics(analyzer, expectedDiagnostic, new[] { code });
                RoslynAssert.Diagnostics(analyzer, new[] { expectedDiagnostic }, code);
            }

            [Test]
            public static void OneErrorWithExpectedDiagnosticWithMessageAndErrorIndicatedInCode()
            {
                var code = @"
namespace RoslynSandbox
{
    class Foo
    {
        private readonly int ↓_value1;
    }
}";

                var expectedDiagnostic = ExpectedDiagnostic.Create(FieldNameMustNotBeginWithUnderscore.DiagnosticId, "Field '_value1' must not begin with an underscore");

                var analyzer = new FieldNameMustNotBeginWithUnderscore();
                RoslynAssert.Diagnostics(analyzer, expectedDiagnostic, code);
            }

            [Test]
            public static void OneErrorWithExpectedDiagnosticsWithMessageAndErrorIndicatedInCode()
            {
                var code = @"
namespace RoslynSandbox
{
    class Foo
    {
        private readonly int ↓_value1;
    }
}";

                var expectedDiagnostic = ExpectedDiagnostic.CreateFromCodeWithErrorsIndicated("SA1309", "Field '_value1' must not begin with an underscore", code, out code);
                var analyzer = new FieldNameMustNotBeginWithUnderscore();
                RoslynAssert.Diagnostics(analyzer, new[] { expectedDiagnostic }, code);
            }

            [Test]
            public static void OneErrorWithExpectedDiagnosticWithMessageWhenAnalyzerSupportsTwoDiagnostics()
            {
                var code = @"
namespace RoslynSandbox
{
    class Foo
    {
        private readonly int ↓_value1;
    }
}";

                var expectedDiagnostic = ExpectedDiagnostic.CreateFromCodeWithErrorsIndicated("ID2", "Field '_value1' must not begin with an underscore", code, out code);
                var analyzer = new FieldNameMustNotBeginWithUnderscoreDifferentDiagnosticsForPublic();
                RoslynAssert.Diagnostics(analyzer, expectedDiagnostic, code);
                RoslynAssert.Diagnostics(analyzer, new[] { expectedDiagnostic }, code);
            }

            [Test]
            public static void SingleDocumentOneErrorPassingAnalyzer()
            {
                var code = @"
namespace RoslynSandbox
{
    class Foo
    {
        private readonly int ↓_value = 1;
    }
}";
                var analyzer = new FieldNameMustNotBeginWithUnderscore();
                RoslynAssert.Diagnostics(analyzer, code);
            }

            [Test]
            public static void WhenCompilationError()
            {
                var code = @"
namespace RoslynSandbox
{
    class Foo
    {
        private readonly int ↓_value = 1;
        INCOMPLETE
    }
}";
                var analyzer = new FieldNameMustNotBeginWithUnderscore();
                RoslynAssert.Diagnostics(analyzer, code, AllowCompilationErrors.Yes);
            }

            [Test]
            public static void SingleDocumentTwoErrorsIndicated()
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

                RoslynAssert.Diagnostics<FieldNameMustNotBeginWithUnderscore>(code);
            }

            [Test]
            public static void TwoDocumentsOneError()
            {
                var foo1 = @"
namespace RoslynSandbox
{
    class Foo1
    {
        private readonly int ↓_value = 1;
    }
}";
                var foo2 = @"
namespace RoslynSandbox
{
    class Foo2
    {
    }
}";
                RoslynAssert.Diagnostics<FieldNameMustNotBeginWithUnderscore>(foo1, foo2);
            }

            [Test]
            public static void TwoDocumentsTwoErrors()
            {
                var foo1 = @"
namespace RoslynSandbox
{
    class Foo1
    {
        private readonly int ↓_value = 1;
    }
}";
                var foo2 = @"
namespace RoslynSandbox
{
    class Foo2
    {
        private readonly int ↓_value = 1;
    }
}";
                RoslynAssert.Diagnostics<FieldNameMustNotBeginWithUnderscore>(foo1, foo2);
                RoslynAssert.Diagnostics(typeof(FieldNameMustNotBeginWithUnderscore), foo1, foo2);
                RoslynAssert.Diagnostics(new FieldNameMustNotBeginWithUnderscore(), foo1, foo2);
            }

            [Test]
            public static void TwoDocumentsTwoErrorsDefaultDisabledAnalyzer()
            {
                var foo1 = @"
namespace RoslynSandbox
{
    class Foo1
    {
        private readonly int ↓_value = 1;
    }
}";
                var foo2 = @"
namespace RoslynSandbox
{
    class Foo2
    {
        private readonly int ↓_value = 1;
    }
}";
                RoslynAssert.Diagnostics<FieldNameMustNotBeginWithUnderscoreDisabled>(foo1, foo2);
                RoslynAssert.Diagnostics(typeof(FieldNameMustNotBeginWithUnderscoreDisabled), foo1, foo2);
                RoslynAssert.Diagnostics(new FieldNameMustNotBeginWithUnderscoreDisabled(), foo1, foo2);
            }

            [Test]
            public static void WithExpectedDiagnosticWhenOneReportsError()
            {
                var code = @"
namespace RoslynSandbox
{
    class Foo
    {
        private readonly int ↓wrongName;
        
        public int WrongName { get; set; }
    }
}";

                var expectedDiagnostic = ExpectedDiagnostic.Create(FieldAndPropertyMustBeNamedFooAnalyzer.FieldDiagnosticId);
                RoslynAssert.Diagnostics<FieldAndPropertyMustBeNamedFooAnalyzer>(expectedDiagnostic, code);
                RoslynAssert.Diagnostics(typeof(FieldAndPropertyMustBeNamedFooAnalyzer), expectedDiagnostic, code);
                RoslynAssert.Diagnostics(new FieldAndPropertyMustBeNamedFooAnalyzer(), expectedDiagnostic, code);

                code = @"
namespace RoslynSandbox
{
    class Foo
    {
        private readonly int wrongName;
        
        public int ↓WrongName { get; set; }
    }
}";

                expectedDiagnostic = ExpectedDiagnostic.Create(FieldAndPropertyMustBeNamedFooAnalyzer.PropertyDiagnosticId);
                RoslynAssert.Diagnostics<FieldAndPropertyMustBeNamedFooAnalyzer>(expectedDiagnostic, code);
                RoslynAssert.Diagnostics(typeof(FieldAndPropertyMustBeNamedFooAnalyzer), expectedDiagnostic, code);
                RoslynAssert.Diagnostics(new FieldAndPropertyMustBeNamedFooAnalyzer(), expectedDiagnostic, code);
            }
        }
    }
}
