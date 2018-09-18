[assembly: Gu.Roslyn.Asserts.MetadataReference(typeof(object), new[] { "global", "mscorlib" })]
[assembly: Gu.Roslyn.Asserts.MetadataReference(typeof(System.Diagnostics.Debug), new[] { "global", "System" })]
[assembly: Gu.Roslyn.Asserts.MetadataReferences(
    typeof(System.Linq.Enumerable), // System.Core
    typeof(System.Net.WebClient))] // System.Net
namespace Gu.Roslyn.Asserts.Tests.WithMetadataReferencesAttribute
{
    using System.Collections.Immutable;
    using Gu.Roslyn.Asserts.Tests.WithMetadataReferencesAttribute.AnalyzersAndFixes;
    using Microsoft.CodeAnalysis;
    using NUnit.Framework;

    public class AnalyzerAssertTests
    {
        [Test]
        public void ResetMetadataReferences()
        {
            var expected = new[]
                           {
                               MetadataReference.CreateFromFile(typeof(object).Assembly.Location)
                                                .WithAliases(ImmutableArray.Create("global", "mscorlib")),
                               MetadataReference.CreateFromFile(typeof(System.Diagnostics.Debug).Assembly.Location)
                                                .WithAliases(ImmutableArray.Create("global", "System")),
                               MetadataReference.CreateFromFile(typeof(System.Linq.Enumerable).Assembly.Location),
                               MetadataReference.CreateFromFile(typeof(System.Net.WebClient).Assembly.Location),
                           };

            CollectionAssert.AreEqual(expected, AnalyzerAssert.MetadataReferences, MetadataReferenceComparer.Default);

            AnalyzerAssert.MetadataReferences.Clear();
            AnalyzerAssert.ResetMetadataReferences();
            CollectionAssert.AreEqual(expected, AnalyzerAssert.MetadataReferences, MetadataReferenceComparer.Default);
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
    }
}
