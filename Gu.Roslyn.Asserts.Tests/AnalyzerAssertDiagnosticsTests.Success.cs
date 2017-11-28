namespace Gu.Roslyn.Asserts.Tests
{
    using System;
    using NUnit.Framework;

    [TestFixture]
    public partial class AnalyzerAssertDiagnosticsTests
    {
        public class Success
        {
            [Test]
            public void OneError()
            {
                var code = @"
namespace RoslynSandbox
{
    class Foo
    {
        private readonly int ↓_value;
    }
}";
                AnalyzerAssert.Diagnostics<FieldNameMustNotBeginWithUnderscore>(code);
                AnalyzerAssert.Diagnostics(typeof(FieldNameMustNotBeginWithUnderscore), code);
                AnalyzerAssert.Diagnostics(new FieldNameMustNotBeginWithUnderscore(), code);
            }

            [Test]
            public void TwoErrorsSamePosition()
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

                AnalyzerAssert.Diagnostics<FieldNameMustNotBeginWithUnderscoreReportsTwo>(new[] { expectedDiagnostic1, expectedDiagnostic2 }, code);
            }

            [Test]
            public void OneErrorWithExpectedDiagnostic()
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
                AnalyzerAssert.Diagnostics<FieldNameMustNotBeginWithUnderscore>(expectedDiagnostic, code);
                AnalyzerAssert.Diagnostics(typeof(FieldNameMustNotBeginWithUnderscore), expectedDiagnostic, code);
                AnalyzerAssert.Diagnostics(new FieldNameMustNotBeginWithUnderscore(), expectedDiagnostic, code);
            }

            [Test]
            public void OneErrorWithExpectedDiagnostics()
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
                AnalyzerAssert.Diagnostics<FieldNameMustNotBeginWithUnderscore>(new[] { expectedDiagnostic }, code);
                AnalyzerAssert.Diagnostics(typeof(FieldNameMustNotBeginWithUnderscore), new[] { expectedDiagnostic }, code);
                AnalyzerAssert.Diagnostics(new FieldNameMustNotBeginWithUnderscore(), new[] { expectedDiagnostic }, code);
            }

            [Test]
            public void OneErrorWithExpectedDiagnosticPositionFromIndicatedCode()
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

                AnalyzerAssert.Diagnostics<FieldNameMustNotBeginWithUnderscore>(expectedDiagnostic, code);
                AnalyzerAssert.Diagnostics(typeof(FieldNameMustNotBeginWithUnderscore), expectedDiagnostic, code);
                AnalyzerAssert.Diagnostics(new FieldNameMustNotBeginWithUnderscore(), expectedDiagnostic, code);
                AnalyzerAssert.Diagnostics<FieldNameMustNotBeginWithUnderscore>(new[] { expectedDiagnostic }, code);
                AnalyzerAssert.Diagnostics(typeof(FieldNameMustNotBeginWithUnderscore), new[] { expectedDiagnostic }, code);
                AnalyzerAssert.Diagnostics(new FieldNameMustNotBeginWithUnderscore(), new[] { expectedDiagnostic }, code);
            }

            [Test]
            public void OneErrorWithExpectedDiagnosticWithMessageAndPosition()
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

                AnalyzerAssert.Diagnostics<FieldNameMustNotBeginWithUnderscore>(expectedDiagnostic, code);
                var analyzer = new FieldNameMustNotBeginWithUnderscore();
                AnalyzerAssert.Diagnostics(analyzer.GetType(), expectedDiagnostic, code);
                AnalyzerAssert.Diagnostics(analyzer, expectedDiagnostic, code);
                AnalyzerAssert.Diagnostics<FieldNameMustNotBeginWithUnderscore>(new[] { expectedDiagnostic }, code);
                AnalyzerAssert.Diagnostics(analyzer.GetType(), new[] { expectedDiagnostic }, code);
                AnalyzerAssert.Diagnostics(analyzer, new[] { expectedDiagnostic }, code);
            }

            [Test]
            public void OneErrorWithExpectedDiagnosticWithMessageAndErrorIndicatedInCode()
            {
                var code = @"
namespace RoslynSandbox
{
    class Foo
    {
        private readonly int ↓_value1;
    }
}";

                var expectedDiagnostic = ExpectedDiagnostic.Create("SA1309", "Field '_value1' must not begin with an underscore");

                AnalyzerAssert.Diagnostics<FieldNameMustNotBeginWithUnderscore>(expectedDiagnostic, code);
                var analyzer = new FieldNameMustNotBeginWithUnderscore();
                AnalyzerAssert.Diagnostics(analyzer.GetType(), expectedDiagnostic, code);
                AnalyzerAssert.Diagnostics(analyzer, expectedDiagnostic, code);
            }

            [Test]
            public void OneErrorWithExpectedDiagnosticsWithMessageAndErrorIndicatedInCode()
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
                AnalyzerAssert.Diagnostics<FieldNameMustNotBeginWithUnderscore>(new[] { expectedDiagnostic }, code);
                AnalyzerAssert.Diagnostics(analyzer.GetType(), new[] { expectedDiagnostic }, code);
                AnalyzerAssert.Diagnostics(analyzer, new[] { expectedDiagnostic }, code);
            }

            [Test]
            public void OneErrorWithExpectedDiagnosticWithMessageWhenAnalyzerSupportsTwoDiagnostics()
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

                AnalyzerAssert.Diagnostics<FieldNameMustNotBeginWithUnderscoreDifferentDiagnosticsForPublic>(expectedDiagnostic, code);
                var analyzer = new FieldNameMustNotBeginWithUnderscoreDifferentDiagnosticsForPublic();
                AnalyzerAssert.Diagnostics(analyzer.GetType(), expectedDiagnostic, code);
                AnalyzerAssert.Diagnostics(analyzer, expectedDiagnostic, code);
                AnalyzerAssert.Diagnostics<FieldNameMustNotBeginWithUnderscoreDifferentDiagnosticsForPublic>(new[] { expectedDiagnostic }, code);
                AnalyzerAssert.Diagnostics(analyzer.GetType(), new[] { expectedDiagnostic }, code);
                AnalyzerAssert.Diagnostics(analyzer, new[] { expectedDiagnostic }, code);
            }

            [TestCase(typeof(FieldNameMustNotBeginWithUnderscore))]
            [TestCase(typeof(FieldNameMustNotBeginWithUnderscoreDisabled))]
            public void SingleClassOneErrorType(Type type)
            {
                var code = @"
namespace RoslynSandbox
{
    class Foo
    {
        private readonly int ↓_value = 1;
    }
}";
                AnalyzerAssert.Diagnostics(type, code);
            }

            [Test]
            public void SingleClassOneErrorPassingAnalyzer()
            {
                var code = @"
namespace RoslynSandbox
{
    class Foo
    {
        private readonly int ↓_value = 1;
    }
}";
                AnalyzerAssert.Diagnostics(new FieldNameMustNotBeginWithUnderscore(), code);
            }

            [Test]
            public void SingleClassTwoErrorsIndicated()
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

                AnalyzerAssert.Diagnostics<FieldNameMustNotBeginWithUnderscore>(code);
            }

            [Test]
            public void TwoClassesOneError()
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
                AnalyzerAssert.Diagnostics<FieldNameMustNotBeginWithUnderscore>(foo1, foo2);
            }

            [Test]
            public void TwoClassesTwoErrorsGeneric()
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
                AnalyzerAssert.Diagnostics<FieldNameMustNotBeginWithUnderscore>(foo1, foo2);
            }

            [TestCase(typeof(FieldNameMustNotBeginWithUnderscore))]
            [TestCase(typeof(FieldNameMustNotBeginWithUnderscoreDisabled))]
            public void TwoClassesTwoErrorsType(Type type)
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
                AnalyzerAssert.Diagnostics(type, foo1, foo2);
            }
        }
    }
}