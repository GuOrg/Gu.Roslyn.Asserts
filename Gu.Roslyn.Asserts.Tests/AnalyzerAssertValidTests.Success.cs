// ReSharper disable RedundantNameQualifier
// ReSharper disable AssignNullToNotNullAttribute
namespace Gu.Roslyn.Asserts.Tests
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
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
                Assert.AreEqual(true, ProjectFile.TryFind("Gu.Roslyn.Asserts.csproj", out var csproj));
                AnalyzerAssert.Valid<NoErrorAnalyzer>(csproj);
                AnalyzerAssert.Valid(typeof(NoErrorAnalyzer), csproj);
                AnalyzerAssert.Valid(new NoErrorAnalyzer(), csproj);
            }

            [Test]
            public async Task SolutionFileNoErrorAnalyzer()
            {
                var sln = SolutionFile.Find("Gu.Roslyn.Asserts.sln");
                var analyzer = new NoErrorAnalyzer();
                AnalyzerAssert.Valid<NoErrorAnalyzer>(sln);
                AnalyzerAssert.Valid(typeof(NoErrorAnalyzer), sln);
                AnalyzerAssert.Valid(analyzer, sln);

                var expectedDiagnostic = ExpectedDiagnostic.Create(NoErrorAnalyzer.DiagnosticId);
                AnalyzerAssert.Valid<NoErrorAnalyzer>(expectedDiagnostic, sln);
                AnalyzerAssert.Valid(typeof(NoErrorAnalyzer), expectedDiagnostic, sln);
                AnalyzerAssert.Valid(analyzer, expectedDiagnostic, sln);
                await AnalyzerAssert.ValidAsync(analyzer, sln, CodeFactory.DefaultCompilationOptions(analyzer, expectedDiagnostic, null), AnalyzerAssert.MetadataReferences).ConfigureAwait(false);
            }

            [Test]
            public async Task SolutionNoErrorAnalyzer()
            {
                var sln = SolutionFile.Find("Gu.Roslyn.Asserts.sln");
                var solution = CodeFactory.CreateSolution(sln, new[] { new NoErrorAnalyzer() }, AnalyzerAssert.MetadataReferences);
                AnalyzerAssert.Valid<NoErrorAnalyzer>(solution);
                AnalyzerAssert.Valid(typeof(NoErrorAnalyzer), solution);
                AnalyzerAssert.Valid(new NoErrorAnalyzer(), solution);
                await AnalyzerAssert.ValidAsync(new NoErrorAnalyzer(), solution).ConfigureAwait(false);
            }

            [Test]
            public async Task TwoClassesNoErrorAnalyzer()
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
                var analyzer = new NoErrorAnalyzer();
                AnalyzerAssert.Valid(analyzer, code1, code2);

                AnalyzerAssert.Valid(analyzer, new List<string> { code1, code2 });
                await AnalyzerAssert.ValidAsync(analyzer, new[] { code1, code2 }, AnalyzerAssert.MetadataReferences).ConfigureAwait(false);
                await AnalyzerAssert.ValidAsync(analyzer, new[] { code1, code2 }, CodeFactory.DefaultCompilationOptions(analyzer, AnalyzerAssert.SuppressedDiagnostics), AnalyzerAssert.MetadataReferences).ConfigureAwait(false);
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
            public async Task WithExpectedDiagnostic()
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
                AnalyzerAssert.Valid(new FieldNameMustNotBeginWithUnderscore(), expectedDiagnostic, new List<string> { code });
                await AnalyzerAssert.ValidAsync(new FieldNameMustNotBeginWithUnderscore(), expectedDiagnostic, new List<string> { code }, AnalyzerAssert.MetadataReferences).ConfigureAwait(false);
            }

            [Test]
            public void WithExpectedDiagnosticWhenOtherReportsError()
            {
                var code = @"
namespace RoslynSandbox
{
    class Foo
    {
        private readonly int foo;
        
        public int WrongName { get; set; }
    }
}";

                var expectedDiagnostic = ExpectedDiagnostic.Create(FieldAndPropertyMustBeNamedFooAnalyzer.FieldDiagnosticId);
                AnalyzerAssert.Valid<FieldAndPropertyMustBeNamedFooAnalyzer>(expectedDiagnostic, code);
                AnalyzerAssert.Valid(typeof(FieldAndPropertyMustBeNamedFooAnalyzer), expectedDiagnostic, code);
                AnalyzerAssert.Valid(new FieldAndPropertyMustBeNamedFooAnalyzer(), expectedDiagnostic, code);
                AnalyzerAssert.Valid<FieldAndPropertyMustBeNamedFooAnalyzer>(new[] { expectedDiagnostic }, code);
                AnalyzerAssert.Valid(typeof(FieldAndPropertyMustBeNamedFooAnalyzer), new[] { expectedDiagnostic }, code);
                AnalyzerAssert.Valid(new FieldAndPropertyMustBeNamedFooAnalyzer(), new[] { expectedDiagnostic }, code);
            }

            [Test]
            public void WithExpectedDiagnosticWhenAnalyzerSupportsTwoDiagnostics()
            {
                var code = @"
namespace RoslynSandbox
{
    class Foo
    {
        private readonly int value1;
    }
}";

                var expectedDiagnostic = ExpectedDiagnostic.Create(FieldNameMustNotBeginWithUnderscoreReportsTwo.DiagnosticId1);
                AnalyzerAssert.Valid<FieldNameMustNotBeginWithUnderscoreReportsTwo>(expectedDiagnostic, code);
                AnalyzerAssert.Valid(typeof(FieldNameMustNotBeginWithUnderscoreReportsTwo), expectedDiagnostic, code);
                AnalyzerAssert.Valid(new FieldNameMustNotBeginWithUnderscoreReportsTwo(), expectedDiagnostic, code);
                AnalyzerAssert.Valid<FieldNameMustNotBeginWithUnderscoreReportsTwo>(new[] { expectedDiagnostic }, code);
                AnalyzerAssert.Valid(typeof(FieldNameMustNotBeginWithUnderscoreReportsTwo), new[] { expectedDiagnostic }, code);
                AnalyzerAssert.Valid(new FieldNameMustNotBeginWithUnderscoreReportsTwo(), new[] { expectedDiagnostic }, code);
            }

            [Test]
            public void Issue53()
            {
                var resourcesCode = @"
namespace RoslynSandbox.Properties
{
    public class Resources
    {
    }
}";

                var testCode = @"
namespace RoslynSandbox
{
    using RoslynSandbox.Properties;

    public class Foo
    {
    }
}";
                AnalyzerAssert.Valid<FieldNameMustNotBeginWithUnderscoreReportsTwo>(resourcesCode, testCode);
                AnalyzerAssert.Valid(new FieldNameMustNotBeginWithUnderscoreReportsTwo(), resourcesCode, testCode);
                AnalyzerAssert.Valid(typeof(FieldNameMustNotBeginWithUnderscoreReportsTwo), resourcesCode, testCode);
            }
        }
    }
}
