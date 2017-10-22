// ReSharper disable RedundantNameQualifier
// ReSharper disable AssignNullToNotNullAttribute
namespace Gu.Roslyn.Asserts.Tests
{
    using NUnit.Framework;

    [TestFixture]
    public partial class AnalyzerAssertValidTests
    {
        public class Success
        {
            [Test]
            public void SingleClassNoErrorAnalyzer()
            {
                var code = @"
namespace RoslynSandbox
{
    class Foo
    {
    }
}";
                AnalyzerAssert.Valid<NoErrorAnalyzer>(code);
                AnalyzerAssert.Valid(typeof(NoErrorAnalyzer), code);
                AnalyzerAssert.Valid(new NoErrorAnalyzer(), code);
            }

            [Test]
            public void ProjectFileNoErrorAnalyzer()
            {
                Assert.AreEqual(true, CodeFactory.TryFindProjectFile("Gu.Roslyn.Asserts.csproj", out var csproj));
                AnalyzerAssert.Valid<NoErrorAnalyzer>(csproj);
                AnalyzerAssert.Valid(typeof(NoErrorAnalyzer), csproj);
                AnalyzerAssert.Valid(new NoErrorAnalyzer(), csproj);
            }

            [Test]
            public void SolutionFileNoErrorAnalyzer()
            {
                Assert.AreEqual(true, CodeFactory.TryFindSolutionFile("Gu.Roslyn.Asserts.sln", out var sln));
                AnalyzerAssert.Valid<NoErrorAnalyzer>(sln);
                AnalyzerAssert.Valid(typeof(NoErrorAnalyzer), sln);
                AnalyzerAssert.Valid(new NoErrorAnalyzer(), sln);
            }

            [Test]
            public void SolutionNoErrorAnalyzer()
            {
                Assert.AreEqual(true, CodeFactory.TryFindSolutionFile("Gu.Roslyn.Asserts.sln", out var sln));
                var solution = CodeFactory.CreateSolution(sln, new[] { new NoErrorAnalyzer() }, AnalyzerAssert.MetadataReferences);
                AnalyzerAssert.Valid<NoErrorAnalyzer>(solution);
                AnalyzerAssert.Valid(typeof(NoErrorAnalyzer), solution);
                AnalyzerAssert.Valid(new NoErrorAnalyzer(), solution);
            }

            [Test]
            public void TwoClassesNoErrorAnalyzer()
            {
                var code1 = @"
namespace RoslynSandbox
{
    class Code1
    {
    }
}";
                var code2 = @"
namespace RoslynSandbox
{
    class Code2
    {
    }
}";
                AnalyzerAssert.Valid<NoErrorAnalyzer>(code1, code2);
                AnalyzerAssert.Valid(typeof(NoErrorAnalyzer), code1, code2);
                AnalyzerAssert.Valid(new NoErrorAnalyzer(), code1, code2);
            }

            [Test]
            public void TwoProjectsNoErrorAnalyzer()
            {
                var code1 = @"
namespace Project1
{
    class Code1
    {
    }
}";
                var code2 = @"
namespace Project2
{
    class Code2
    {
    }
}";
                AnalyzerAssert.Valid<NoErrorAnalyzer>(code1, code2);
                AnalyzerAssert.Valid(typeof(NoErrorAnalyzer), code1, code2);
                AnalyzerAssert.Valid(new NoErrorAnalyzer(), code1, code2);
            }

            [Test]
            public void WithExpectedDiagnostic()
            {
                var code = @"
namespace RoslynSandbox
{
    class Foo
    {
        private readonly int value1;
    }
}";

                var expectedDiagnostic = ExpectedDiagnostic.Create(FieldNameMustNotBeginWithUnderscore.DiagnosticId);
                AnalyzerAssert.Valid<FieldNameMustNotBeginWithUnderscore>(expectedDiagnostic, code);
                AnalyzerAssert.Valid(typeof(FieldNameMustNotBeginWithUnderscore), expectedDiagnostic, code);
                AnalyzerAssert.Valid(new FieldNameMustNotBeginWithUnderscore(), expectedDiagnostic, code);
                AnalyzerAssert.Valid<FieldNameMustNotBeginWithUnderscore>(new[] { expectedDiagnostic }, code);
                AnalyzerAssert.Valid(typeof(FieldNameMustNotBeginWithUnderscore), new[] { expectedDiagnostic }, code);
                AnalyzerAssert.Valid(new FieldNameMustNotBeginWithUnderscore(), new[] { expectedDiagnostic }, code);
            }
        }
    }
}