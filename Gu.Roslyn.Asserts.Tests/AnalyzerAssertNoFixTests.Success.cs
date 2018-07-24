// ReSharper disable RedundantNameQualifier
namespace Gu.Roslyn.Asserts.Tests
{
    using System.Collections.Generic;
    using Gu.Roslyn.Asserts.Tests.CodeFixes;
    using NUnit.Framework;

    [TestFixture]
    public partial class AnalyzerAssertNoFixTests
    {
        public class Success
        {
            [Test]
            public void SingleClassOneErrorNoFix()
            {
                var code = @"
namespace RoslynSandbox
{
    class Foo
    {
        private readonly int ↓_value;
    }
}";
                var expectedDiagnostic = ExpectedDiagnostic.Create(FieldNameMustNotBeginWithUnderscore.DiagnosticId);
                AnalyzerAssert.NoFix<FieldNameMustNotBeginWithUnderscore, NoCodeFixProvider>(code);
                AnalyzerAssert.NoFix<FieldNameMustNotBeginWithUnderscore, NoCodeFixProvider>(expectedDiagnostic, code);
                AnalyzerAssert.NoFix<FieldNameMustNotBeginWithUnderscore, NoCodeFixProvider>((IReadOnlyList<string>)new[] { code });
                AnalyzerAssert.NoFix(new FieldNameMustNotBeginWithUnderscore(), new NoCodeFixProvider(), code);
                AnalyzerAssert.NoFix(new FieldNameMustNotBeginWithUnderscore(), new NoCodeFixProvider(), expectedDiagnostic, code);
                AnalyzerAssert.NoFix(new FieldNameMustNotBeginWithUnderscore(), new NoCodeFixProvider(), expectedDiagnostic, new List<string> { code });
                AnalyzerAssert.NoFix(new FieldNameMustNotBeginWithUnderscore(), new NoCodeFixProvider(), new[] { expectedDiagnostic.WithPositionFromCodeWithErrorsIndicated(code, out code) }, code);
            }

            [Test]
            public void TwoClassOneErrorNoFix()
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

                AnalyzerAssert.NoFix<FieldNameMustNotBeginWithUnderscore, NoCodeFixProvider>(barCode, code);
                AnalyzerAssert.NoFix<FieldNameMustNotBeginWithUnderscore, NoCodeFixProvider>(new List<string> { barCode, code });
                AnalyzerAssert.NoFix(new FieldNameMustNotBeginWithUnderscore(), new NoCodeFixProvider(), barCode, code);

                var expectedDiagnostic = ExpectedDiagnostic.Create(FieldNameMustNotBeginWithUnderscore.DiagnosticId);
                AnalyzerAssert.NoFix<FieldNameMustNotBeginWithUnderscore, NoCodeFixProvider>(expectedDiagnostic, barCode, code);
                AnalyzerAssert.NoFix(new FieldNameMustNotBeginWithUnderscore(), new NoCodeFixProvider(), expectedDiagnostic, barCode, code);
            }
        }
    }
}
