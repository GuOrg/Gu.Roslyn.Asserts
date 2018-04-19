namespace Gu.Roslyn.Asserts.Tests
{
    using System.Linq;
    using Gu.Roslyn.Asserts.Tests.CodeFixes;
    using Microsoft.CodeAnalysis;
    using NUnit.Framework;

    public class FixTests
    {
        [Test]
        public void SingleClassOneErrorCorrectFix()
        {
            var code = @"
namespace RoslynSandbox
{
    class Foo
    {
        private readonly int _value;
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
            var analyzer = new FieldNameMustNotBeginWithUnderscore();
            var cSharpCompilationOptions = CodeFactory.DefaultCompilationOptions(analyzer);
            var metadataReferences = new[] { MetadataReference.CreateFromFile(typeof(int).Assembly.Location) };
            var sln = CodeFactory.CreateSolution(code, cSharpCompilationOptions, metadataReferences);
            var diagnostics = Analyze.GetDiagnostics(sln, analyzer);
            var fixedSln = Fix.Apply(sln, new DontUseUnderscoreCodeFixProvider(), diagnostics.Single().Single());
            CodeAssert.AreEqual(fixedCode, fixedSln.Projects.Single().Documents.Single());
        }

        [Test]
        public void SingleClassOneErrorCorrectFixAll()
        {
            var code = @"
namespace RoslynSandbox
{
    class Foo
    {
        private readonly int _value;
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
            var analyzer = new FieldNameMustNotBeginWithUnderscore();
            var cSharpCompilationOptions = CodeFactory.DefaultCompilationOptions(analyzer);
            var metadataReferences = new[] { MetadataReference.CreateFromFile(typeof(int).Assembly.Location) };
            var sln = CodeFactory.CreateSolution(code, cSharpCompilationOptions, metadataReferences);
            var diagnostics = Analyze.GetDiagnostics(sln, analyzer);
            var fixedSln = Fix.Apply(sln, new DontUseUnderscoreCodeFixProvider(), diagnostics);
            CodeAssert.AreEqual(fixedCode, fixedSln.Projects.Single().Documents.Single());
        }
    }
}
