#pragma warning disable IDE0079 // Remove unnecessary suppression
namespace Gu.Roslyn.Asserts.Tests.RoslynAssertTests;

using System.Linq;
using NUnit.Framework;

[TestFixture]
public partial class Diagnostics
{
    public static class Success
    {
        [Test]
        public static void OneErrorIndicatedPosition()
        {
            var code = @"
namespace N
{
    class C
    {
        private readonly int ↓_f = 1;

        public int M() => _f;
    }
}";
            var analyzer = new FieldNameMustNotBeginWithUnderscore();
            var expectedDiagnostic = ExpectedDiagnostic.Create(FieldNameMustNotBeginWithUnderscore.DiagnosticId);
            RoslynAssert.Diagnostics(analyzer, code);
            RoslynAssert.Diagnostics(analyzer, expectedDiagnostic, code);
        }

        [Test]
        public static void OneErrorIndicatedPositionSuppressedWarning()
        {
            var code = @"
namespace N
{
    class C
    {
        private readonly int ↓_f;
    }
}";
            var analyzer = new FieldNameMustNotBeginWithUnderscore();
            var expectedDiagnostic = ExpectedDiagnostic.Create(FieldNameMustNotBeginWithUnderscore.DiagnosticId);
            var settings = Settings.Default.WithCompilationOptions(x => x.WithSuppressedDiagnostics("CS0169"));
            RoslynAssert.Diagnostics(analyzer, code, settings);
            RoslynAssert.Diagnostics(analyzer, expectedDiagnostic, code, settings);
        }

        [Test]
        public static void OneErrorIndicatedPositionAllowWarnings()
        {
            var code = """

                namespace N;

                class C
                {
                    private readonly int ↓_f;
                }
                """;
            var analyzer = new FieldNameMustNotBeginWithUnderscore();
            var expectedDiagnostic = ExpectedDiagnostic.Create(FieldNameMustNotBeginWithUnderscore.DiagnosticId);
            var settings = Settings.Default.WithAllowedCompilerDiagnostics(AllowedCompilerDiagnostics.Warnings);
            RoslynAssert.Diagnostics(analyzer, code, settings);
            RoslynAssert.Diagnostics(analyzer, expectedDiagnostic, code, settings);
        }

        [Test]
        public static void OneErrorIndicatedPositionAllowWarningsDefaultDisabledAnalyzer()
        {
            var code = """

                namespace N;

                class C
                {
                    private readonly int ↓_f;
                }
                """;
            var analyzer = new FieldNameMustNotBeginWithUnderscoreDisabled();
            var expectedDiagnostic = ExpectedDiagnostic.Create(FieldNameMustNotBeginWithUnderscoreDisabled.Descriptor.Id);
            var settings = Settings.Default.WithAllowedCompilerDiagnostics(AllowedCompilerDiagnostics.Warnings);
            RoslynAssert.Diagnostics(analyzer, code, settings);
            RoslynAssert.Diagnostics(analyzer, expectedDiagnostic, code, settings);
        }

        [Test]
        public static void TwoErrorsSamePosition()
        {
            var code = @"
namespace N
{
    class C
    {
        private readonly int ↓_f = 1;

        public int M() => _f;
    }
}";
            var expectedDiagnostic1 = ExpectedDiagnostic.FromMarkup(
                FieldNameMustNotBeginWithUnderscoreReportsTwo.DiagnosticId1,
                code,
                out _);

            var expectedDiagnostic2 = ExpectedDiagnostic.FromMarkup(
                FieldNameMustNotBeginWithUnderscoreReportsTwo.DiagnosticId2,
                code,
                out code);
            var analyzer = new FieldNameMustNotBeginWithUnderscoreReportsTwo();
            RoslynAssert.Diagnostics(analyzer, new[] { expectedDiagnostic1, expectedDiagnostic2 }, code);
        }

        [Test]
        public static void OneErrorWithExpectedDiagnosticIdOnly()
        {
#pragma warning disable GURA02 // Indicate position.
            var code = @"
namespace N
{
    class C
    {
        private readonly int _f = 1;

        public int M() => _f;
    }
}";
#pragma warning restore GURA02 // Indicate position.

            var expectedDiagnostic = ExpectedDiagnostic.Create("SA1309");
            RoslynAssert.Diagnostics(new FieldNameMustNotBeginWithUnderscore(), expectedDiagnostic, code);
        }

        [Test]
        public static void OneErrorWithExpectedDiagnosticIdAndMessage()
        {
#pragma warning disable GURA02 // Indicate position.
            var code = @"
namespace N
{
    class C
    {
        private readonly int _f = 1;

        public int M() => _f;
    }
}";
#pragma warning restore GURA02 // Indicate position.

            var expectedDiagnostic = ExpectedDiagnostic.Create("SA1309", "Field '_f' must not begin with an underscore");
            var analyzer = new FieldNameMustNotBeginWithUnderscore();
            RoslynAssert.Diagnostics(analyzer, expectedDiagnostic, code);
        }

        [Test]
        public static void OneErrorWithExpectedDiagnosticIdAndPosition()
        {
#pragma warning disable GURA02 // Indicate position.
            var code = @"
namespace N
{
    class C
    {
        private readonly int _f = 1;

        public int M() => _f;
    }
}";
#pragma warning restore GURA02 // Indicate position.

            var expectedDiagnostic = ExpectedDiagnostic.Create("SA1309", 5, 29);
            var analyzer = new FieldNameMustNotBeginWithUnderscore();
            RoslynAssert.Diagnostics(analyzer, expectedDiagnostic, code);
        }

        [Test]
        public static void OneErrorWithExpectedDiagnostics()
        {
            var code = @"
namespace N
{
    class C
    {
        private readonly int ↓_f = 1;

        public int M() => _f;
    }
}";

            var expectedDiagnostic = ExpectedDiagnostic.FromMarkup("SA1309", code, out code);
            var analyzer = new FieldNameMustNotBeginWithUnderscore();
            RoslynAssert.Diagnostics(analyzer, expectedDiagnostic, code);
            RoslynAssert.Diagnostics(analyzer, new[] { expectedDiagnostic }, code);
        }

        [Test]
        public static void OneErrorWithExpectedDiagnosticPositionFromIndicatedCode()
        {
            var code = @"
namespace N
{
    class C
    {
        private readonly int ↓_f = 1;

        public int M() => _f;
    }
}";

            var expectedDiagnostic = ExpectedDiagnostic.FromMarkup("SA1309", code, out code);
            var analyzer = new FieldNameMustNotBeginWithUnderscore();
            RoslynAssert.Diagnostics(analyzer, expectedDiagnostic, code);
            RoslynAssert.Diagnostics(analyzer, new[] { expectedDiagnostic }, code);
        }

        [Test]
        public static void OneErrorWithExpectedDiagnosticWithMessageAndPosition()
        {
            var code = @"
namespace N
{
    class C
    {
        private readonly int ↓_f = 1;

        public int M() => _f;
    }
}";

            var expectedDiagnostic = ExpectedDiagnostic.FromMarkup("SA1309", "Field '_f' must not begin with an underscore", code, out code);

            var analyzer = new FieldNameMustNotBeginWithUnderscore();
            RoslynAssert.Diagnostics(analyzer, expectedDiagnostic, code);
            RoslynAssert.Diagnostics(analyzer, expectedDiagnostic, new[] { code });
            RoslynAssert.Diagnostics(analyzer, new[] { expectedDiagnostic }, code);
        }

        [Test]
        public static void OneErrorWithExpectedDiagnosticWithMessageAndErrorIndicatedInCode()
        {
            var code = @"
namespace N
{
    class C
    {
        private readonly int ↓_f = 1;

        public int M() => _f;
    }
}";

            var expectedDiagnostic = ExpectedDiagnostic.Create(FieldNameMustNotBeginWithUnderscore.DiagnosticId, "Field '_f' must not begin with an underscore");

            var analyzer = new FieldNameMustNotBeginWithUnderscore();
            RoslynAssert.Diagnostics(analyzer, expectedDiagnostic, code);
        }

        [Test]
        public static void OneErrorWithExpectedDiagnosticsWithMessageAndErrorIndicatedInCode()
        {
            var code = @"
namespace N
{
    class C
    {
        private readonly int ↓_f = 1;

        public int M() => _f;
    }
}";

            var expectedDiagnostic = ExpectedDiagnostic.FromMarkup("SA1309", "Field '_f' must not begin with an underscore", code, out code);
            var analyzer = new FieldNameMustNotBeginWithUnderscore();
            RoslynAssert.Diagnostics(analyzer, new[] { expectedDiagnostic }, code);
        }

        [Test]
        public static void OneErrorWithExpectedDiagnosticWithMessageWhenAnalyzerSupportsTwoDiagnostics()
        {
            var code = @"
namespace N
{
    class C
    {
        private readonly int ↓_f = 1;

        public int M() => _f;
    }
}";

            var expectedDiagnostic = ExpectedDiagnostic.FromMarkup("SA1309b", "Field '_f' must not begin with an underscore", code, out code);
            var analyzer = new FieldNameMustNotBeginWithUnderscoreDifferentDiagnosticsForPublic();
            RoslynAssert.Diagnostics(analyzer, expectedDiagnostic, code);
            RoslynAssert.Diagnostics(analyzer, new[] { expectedDiagnostic }, code);
        }

        [TestCase(null)]
        [TestCase("_")]
        [TestCase("m_")]
        public static void AnalyzerWithConfiguration(string? prefix)
        {
            var analyzerConfig = prefix is null
                ? null
                : $"dotnet_diagnostic.SA1309.field_name_prefix = {prefix}";

            var code = @"
namespace N
{
    public class C
    {
        private readonly int ↓f_value = 1;

        public int M() => this.f_value;
    }
}";
            var analyzer = new FieldNameMustHaveProperPrefix();
            var settings = Settings.Default.WithAnalyzerConfig(analyzerConfig);
            RoslynAssert.Diagnostics(analyzer, code, settings);
        }

        [Test]
        public static void SingleDocumentOneErrorPassingAnalyzer()
        {
            var code = @"
namespace N
{
    class C
    {
        private readonly int ↓_f = 1;

        public int M() => _f;
    }
}";
            var analyzer = new FieldNameMustNotBeginWithUnderscore();
            RoslynAssert.Diagnostics(analyzer, code);
        }

        [Test]
        public static void WhenCompilationError()
        {
            var code = @"
namespace N
{
    class C
    {
        private readonly int ↓_f = 1;
        INCOMPLETE
    }
}";
            var analyzer = new FieldNameMustNotBeginWithUnderscore();
            RoslynAssert.Diagnostics(analyzer, code, Settings.Default.WithAllowedCompilerDiagnostics(AllowedCompilerDiagnostics.WarningsAndErrors));
        }

        [Test]
        public static void SingleDocumentTwoErrorsIndicated()
        {
            var code = @"
namespace N
{
    class C
    {
        private readonly int ↓_f1 = 1;
        private readonly int ↓_f2 = 2;

        public int M() => _f1 + _f2;
    }
}";
            var analyzer = new FieldNameMustNotBeginWithUnderscore();
            RoslynAssert.Diagnostics(analyzer, code);
        }

        [Test]
        public static void TwoDocumentsOneError()
        {
            var c2 = @"
namespace N
{
    class C2
    {
    }
}";

            var code = @"
namespace N
{
    class C1
    {
        private readonly int ↓_f = 1;

        public int M() => _f;
    }
}";
            var analyzer = new FieldNameMustNotBeginWithUnderscore();
            RoslynAssert.Diagnostics(analyzer, code, c2);
        }

        [Test]
        public static void TwoDocumentsTwoErrors()
        {
            var c1 = @"
namespace N
{
    class C1
    {
        private readonly int ↓_f1 = 1;

        public int M1() => _f1;
    }
}";
            var c2 = @"
namespace N
{
    class C2
    {
        private readonly int ↓_f2 = 2;

        public int M2() => _f2;
    }
}";
            var analyzer = new FieldNameMustNotBeginWithUnderscore();
            RoslynAssert.Diagnostics(analyzer, c1, c2);
        }

        [Test]
        public static void TwoDocumentsTwoErrorsDefaultDisabledAnalyzer()
        {
            var c1 = @"
namespace N
{
    class C1
    {
        private readonly int ↓_f1 = 1;

        public int M1() => _f1;
    }
}";
            var c2 = @"
namespace N
{
    class C2
    {
        private readonly int ↓_f2 = 2;

        public int M2() => _f2;
    }
}";
            var analyzer = new FieldNameMustNotBeginWithUnderscoreDisabled();
            RoslynAssert.Diagnostics(analyzer, c1, c2);
        }

        [Test]
        public static void WithExpectedDiagnosticWhenOneReportsError()
        {
            var code = @"
namespace N
{
    class C
    {
        private int ↓wrongName = 1;
        
        public int WrongName
        { 
            get => this.wrongName;
            set => this.wrongName = value;
        }
    }
}";

            var expectedDiagnostic = ExpectedDiagnostic.Create(FieldAndPropertyMustBeNamedValueAnalyzer.FieldDiagnosticId);
            var analyzer = new FieldAndPropertyMustBeNamedValueAnalyzer();
            RoslynAssert.Diagnostics(analyzer, expectedDiagnostic, code);

            code = @"
namespace N
{
    class C
    {
        private int wrongName = 1;
        
        public int ↓WrongName
        { 
            get => this.wrongName;
            set => this.wrongName = value;
        }
    }
}";

            expectedDiagnostic = ExpectedDiagnostic.Create(FieldAndPropertyMustBeNamedValueAnalyzer.PropertyDiagnosticId);
            RoslynAssert.Diagnostics(analyzer, expectedDiagnostic, code);
        }

        [Test]
        public static void AdditionalLocation()
        {
            var code = @"
namespace N
{
    ↓class C
    {
        ↓private readonly int f = 1;

        public int M() => this.f;
    }
}";

            var analyzer = new FieldWithAdditionalLocationClassAnalyzer();
            RoslynAssert.Diagnostics(analyzer, code);

            var expectedDiagnostic = ExpectedDiagnostic.Create(analyzer.SupportedDiagnostics.Single());
            RoslynAssert.Diagnostics(analyzer, expectedDiagnostic, code);
        }
    }
}
