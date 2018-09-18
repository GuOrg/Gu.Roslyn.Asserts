// ReSharper disable RedundantNameQualifier
// ReSharper disable AssignNullToNotNullAttribute
namespace Gu.Roslyn.Asserts.Tests
{
    using System.Collections.Generic;
    using Microsoft.CodeAnalysis;
    using NUnit.Framework;

    [TestFixture]
    public partial class AnalyzerAssertValidTests
    {
        public class Success
        {
            [OneTimeSetUp]
            public void OneTimeSetUp()
            {
                AnalyzerAssert.MetadataReferences.Add(MetadataReference.CreateFromFile(typeof(object).Assembly.Location).WithAliases(new[] { "global", "mscorlib" }));
                AnalyzerAssert.MetadataReferences.Add(MetadataReference.CreateFromFile(typeof(System.Diagnostics.Debug).Assembly.Location).WithAliases(new[] { "global", "System" }));
                AnalyzerAssert.MetadataReferences.AddRange(MetadataReferences.Transitive(
                    typeof(Microsoft.CodeAnalysis.CSharp.CSharpCompilation),
                    typeof(Microsoft.CodeAnalysis.CodeFixes.CodeFixProvider),
                    typeof(System.Runtime.CompilerServices.InternalsVisibleToAttribute)));
            }

            [OneTimeTearDown]
            public void OneTimeTearDown()
            {
                // Usually this is not needed but we want everything reset when testing the AnalyzerAssert.
                AnalyzerAssert.MetadataReferences.Clear();
            }

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
                var analyzer = new NoErrorAnalyzer();
                AnalyzerAssert.Valid<NoErrorAnalyzer>(code);
                AnalyzerAssert.Valid(typeof(NoErrorAnalyzer), code);
                AnalyzerAssert.Valid(analyzer, code);
                AnalyzerAssert.Valid(analyzer, code, CodeFactory.DefaultCompilationOptions(analyzer, AnalyzerAssert.SuppressedDiagnostics), AnalyzerAssert.MetadataReferences);
                AnalyzerAssert.Valid(analyzer, new[] { code }, CodeFactory.DefaultCompilationOptions(analyzer, AnalyzerAssert.SuppressedDiagnostics), AnalyzerAssert.MetadataReferences);

                var expectedDiagnostic = ExpectedDiagnostic.Create(NoErrorAnalyzer.DiagnosticId);
                AnalyzerAssert.Valid<NoErrorAnalyzer>(expectedDiagnostic, code);
                AnalyzerAssert.Valid(typeof(NoErrorAnalyzer), expectedDiagnostic, code);
                AnalyzerAssert.Valid(analyzer, expectedDiagnostic, code);

                AnalyzerAssert.Valid<NoErrorAnalyzer>(new[] { expectedDiagnostic }, code);
                AnalyzerAssert.Valid(typeof(NoErrorAnalyzer), new[] { expectedDiagnostic }, code);
                AnalyzerAssert.Valid(analyzer, new[] { expectedDiagnostic }, code);
            }

            [Test]
            public void ProjectFileNoErrorAnalyzer()
            {
                var csproj = ProjectFile.Find("Gu.Roslyn.Asserts.csproj");
                var analyzer = new NoErrorAnalyzer();

                AnalyzerAssert.Valid<NoErrorAnalyzer>(csproj);
                AnalyzerAssert.Valid(typeof(NoErrorAnalyzer), csproj);
                AnalyzerAssert.Valid(analyzer, csproj);

                var expectedDiagnostic = ExpectedDiagnostic.Create(NoErrorAnalyzer.DiagnosticId);
                AnalyzerAssert.Valid<NoErrorAnalyzer>(expectedDiagnostic, csproj);
                AnalyzerAssert.Valid(typeof(NoErrorAnalyzer), expectedDiagnostic, csproj);
                AnalyzerAssert.Valid(analyzer, expectedDiagnostic, csproj);
                AnalyzerAssert.Valid(analyzer, csproj, CodeFactory.DefaultCompilationOptions(analyzer, expectedDiagnostic, null), AnalyzerAssert.MetadataReferences);
            }

            [Test]
            public void SolutionFileNoErrorAnalyzer()
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
            }

            [Test]
            public void SolutionNoErrorAnalyzer()
            {
                var sln = SolutionFile.Find("Gu.Roslyn.Asserts.sln");
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
                var analyzer = new NoErrorAnalyzer();
                AnalyzerAssert.Valid(analyzer, code1, code2);

                AnalyzerAssert.Valid(analyzer, new List<string> { code1, code2 });
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
                var analyzer = new FieldNameMustNotBeginWithUnderscore();
                var expectedDiagnostic = ExpectedDiagnostic.Create(FieldNameMustNotBeginWithUnderscore.DiagnosticId);
                AnalyzerAssert.Valid<FieldNameMustNotBeginWithUnderscore>(expectedDiagnostic, code);
                AnalyzerAssert.Valid(typeof(FieldNameMustNotBeginWithUnderscore), expectedDiagnostic, code);
                AnalyzerAssert.Valid(analyzer, expectedDiagnostic, code);
                AnalyzerAssert.Valid<FieldNameMustNotBeginWithUnderscore>(new[] { expectedDiagnostic }, code);
                AnalyzerAssert.Valid(typeof(FieldNameMustNotBeginWithUnderscore), new[] { expectedDiagnostic }, code);
                AnalyzerAssert.Valid(analyzer, new[] { expectedDiagnostic }, code);
                AnalyzerAssert.Valid(analyzer, expectedDiagnostic, new List<string> { code });
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

            [Test]
            public void AnalyzerWithTwoDiagnostics()
            {
                var testCode = @"
namespace RoslynSandbox
{
    public class Foo
    {
        private int foo;
    }
}";
                AnalyzerAssert.Valid(new FieldAndPropertyMustBeNamedFooAnalyzer(), testCode);
            }
        }
    }
}
