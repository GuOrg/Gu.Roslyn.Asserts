namespace Gu.Roslyn.Asserts.Tests
{
    using System.Linq;
    using System.Threading.Tasks;
    using Gu.Roslyn.Asserts.Tests.CodeFixes;
    using Microsoft.CodeAnalysis;
    using NUnit.Framework;

    public static class FixTests
    {
        [Test]
        public static async Task SingleDocumentOneError()
        {
            var code = @"
namespace RoslynSandbox
{
    class Foo
    {
        private readonly int _value;
    }
}";

            var after = @"
namespace RoslynSandbox
{
    class Foo
    {
        private readonly int value;
    }
}";
            var analyzer = new FieldNameMustNotBeginWithUnderscore();
            var cSharpCompilationOptions = CodeFactory.DefaultCompilationOptions(analyzer);
            var metadataReferences = new[] { MetadataReference.CreateFromFile(typeof(int).Assembly.Location) };
            var sln = CodeFactory.CreateSolution(code, cSharpCompilationOptions, metadataReferences);
            var diagnostic = Analyze.GetDiagnostics(sln, analyzer).SelectMany(x => x).Single();
            var fixedSln = Fix.Apply(sln, new DontUseUnderscoreCodeFixProvider(), diagnostic);
            CodeAssert.AreEqual(after, fixedSln.Projects.Single().Documents.Single());

            fixedSln = await Fix.ApplyAsync(sln, new DontUseUnderscoreCodeFixProvider(), diagnostic).ConfigureAwait(false);
            CodeAssert.AreEqual(after, fixedSln.Projects.Single().Documents.Single());
        }

        [Test]
        public static void SingleDocumentTwoErrors()
        {
            var code = @"
namespace RoslynSandbox
{
    class Foo
    {
        private readonly int _value1;
        private readonly int _value2;
    }
}";

            var after = @"
namespace RoslynSandbox
{
    class Foo
    {
        private readonly int value1;
        private readonly int value2;
    }
}";
            var analyzer = new FieldNameMustNotBeginWithUnderscore();
            var compilationOptions = CodeFactory.DefaultCompilationOptions(analyzer);
            var metadataReferences = new[] { MetadataReference.CreateFromFile(typeof(int).Assembly.Location) };
            var sln = CodeFactory.CreateSolution(code, compilationOptions, metadataReferences);
            var diagnostics = Analyze.GetDiagnostics(sln, analyzer);
            var fixedSln = Fix.Apply(sln, new DontUseUnderscoreCodeFixProvider(), diagnostics);
            CodeAssert.AreEqual(after, fixedSln.Projects.Single().Documents.Single());
        }

        [Test]
        public static void SingleDocumentOneErrorCorrectFixAll()
        {
            var code = @"
namespace RoslynSandbox
{
    class Foo
    {
        private readonly int _value;
    }
}";

            var after = @"
namespace RoslynSandbox
{
    class Foo
    {
        private readonly int value;
    }
}";
            var analyzer = new FieldNameMustNotBeginWithUnderscore();
            var cSharpCompilationOptions = CodeFactory.DefaultCompilationOptions(analyzer);
            var metadataReferences = new[] { MetadataReference.CreateFromFile(typeof(int).Assembly.Location) };
            var sln = CodeFactory.CreateSolution(code, cSharpCompilationOptions, metadataReferences);
            var diagnostics = Analyze.GetDiagnostics(sln, analyzer);
            var fixedSln = Fix.Apply(sln, new DontUseUnderscoreCodeFixProvider(), diagnostics);
            CodeAssert.AreEqual(after, fixedSln.Projects.Single().Documents.Single());
        }
    }
}
