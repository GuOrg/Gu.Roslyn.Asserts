namespace Gu.Roslyn.Asserts.Analyzers.Tests.RenameObsoleteFixTests
{
    using NUnit.Framework;

    public static partial class CodeFix
    {
        [Test]
        public static void ChangeToMetadataReferencesFromAttributes()
        {
            var before = @"
namespace N
{
    using Gu.Roslyn.Asserts;
    using Microsoft.CodeAnalysis;

    class C
    {
        private static readonly Solution Sln = CodeFactory.CreateSolution(
            ProjectFile.Find(""Gu.Roslyn.Asserts.Analyzers.Tests.csproj""),
            ↓RoslynAssert.MetadataReferences);
    }
}";

            var after = @"
namespace N
{
    using Gu.Roslyn.Asserts;
    using Microsoft.CodeAnalysis;

    class C
    {
        private static readonly Solution Sln = CodeFactory.CreateSolution(
            ProjectFile.Find(""Gu.Roslyn.Asserts.Analyzers.Tests.csproj""),
            MetadataReferences.FromAttributes());
    }
}";
            var expectedDiagnostic = ExpectedDiagnostic.Create("CS0618");
            RoslynAssert.CodeFix(Fix, expectedDiagnostic, new[] { Code.PlaceholderAnalyzer, before }, after);
        }
    }
}
