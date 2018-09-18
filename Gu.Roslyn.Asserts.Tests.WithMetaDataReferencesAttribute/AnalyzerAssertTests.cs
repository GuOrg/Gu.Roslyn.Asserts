namespace Gu.Roslyn.Asserts.Tests.WithMetadataReferencesAttribute
{
    using Gu.Roslyn.Asserts.Tests.WithMetadataReferencesAttribute.AnalyzersAndFixes;
    using NUnit.Framework;

    public class AnalyzerAssertTests
    {
        [Test]
        public void ResetMetadataReferences()
        {
            CollectionAssert.IsNotEmpty(AnalyzerAssert.MetadataReferences);

            AnalyzerAssert.MetadataReferences.Clear();
            CollectionAssert.IsEmpty(AnalyzerAssert.MetadataReferences);

            AnalyzerAssert.ResetMetadataReferences();
            CollectionAssert.IsNotEmpty(AnalyzerAssert.MetadataReferences);
        }

        [Test]
        public void CodeFixSingleClassOneErrorCorrectFix()
        {
            var code = @"
namespace RoslynSandbox
{
    class Foo
    {
        private readonly int ↓_value;
    }
}";

            var fixedCode = @"
namespace RoslynSandbox
{
    class Foo
    {
        private readonly int value;
    }
}";
            AnalyzerAssert.CodeFix<FieldNameMustNotBeginWithUnderscore, DontUseUnderscoreCodeFixProvider>(code, fixedCode);
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
    }
}
