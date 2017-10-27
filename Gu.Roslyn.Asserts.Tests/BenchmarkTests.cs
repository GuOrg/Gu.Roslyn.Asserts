namespace Gu.Roslyn.Asserts.Tests
{
    using System.Linq;
    using System.Threading.Tasks;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using NUnit.Framework;

    public class BenchmarkTests
    {
        [Test]
        public async Task Solution()
        {
            var analyzer = new FieldNameMustNotBeginWithUnderscore();
            var sln = CodeFactory.CreateSolution(
                CodeFactory.FindSolutionFile("Gu.Roslyn.Asserts.sln"),
                MetadataReferences.Transitive(typeof(BenchmarkTests).Assembly).ToArray());
            var benchmark = await Benchmark.CreateAsync(sln, analyzer).ConfigureAwait(false);
            CollectionAssert.IsNotEmpty(benchmark.SyntaxNodeActions);
            CollectionAssert.AllItemsAreInstancesOfType(benchmark.SyntaxNodeActions.Select(x => x.Context.Node), typeof(FieldDeclarationSyntax));
            CollectionAssert.AllItemsAreInstancesOfType(benchmark.SyntaxNodeActions.Select(x => x.Context.ContainingSymbol), typeof(IFieldSymbol));
            Assert.AreSame(analyzer, benchmark.Analyzer);
            benchmark.Run();
            benchmark.Run();
        }

        [Test]
        public async Task Project()
        {
            var analyzer = new FieldNameMustNotBeginWithUnderscore();
            var sln = CodeFactory.CreateSolution(
                CodeFactory.FindProjectFile("Gu.Roslyn.Asserts.csproj"),
                MetadataReferences.Transitive(typeof(Benchmark).Assembly).ToArray());
            var benchmark = await Benchmark.CreateAsync(sln.Projects.Single(), analyzer).ConfigureAwait(false);
            CollectionAssert.IsNotEmpty(benchmark.SyntaxNodeActions);
            Assert.AreSame(analyzer, benchmark.Analyzer);
            benchmark.Run();
            benchmark.Run();
        }
    }
}
