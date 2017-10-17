namespace Gu.Roslyn.Asserts.Tests
{
    using System.Linq;
    using System.Threading.Tasks;
    using Microsoft.CodeAnalysis.Diagnostics;
    using NUnit.Framework;

    public class BenchmarkTests
    {
        [Test]
        public async Task Solution()
        {
            var analyzer = new FieldNameMustNotBeginWithUnderscore();
            var sln = CodeFactory.CreateSolution(
                CodeFactory.FindSolutionFile("Gu.Roslyn.Asserts.sln"),
                new DiagnosticAnalyzer[] { analyzer },
                MetadataReferences.Transitive(typeof(BenchmarkTests).Assembly).ToArray());
            var benchmark = await Benchmark.CreateAsync(sln, analyzer).ConfigureAwait(false);
            CollectionAssert.IsNotEmpty(benchmark.ContextAndActions);
            benchmark.Run();
        }

        [Test]
        public async Task Project()
        {
            var analyzer = new FieldNameMustNotBeginWithUnderscore();
            var sln = CodeFactory.CreateSolution(
                CodeFactory.FindProjectFile("Gu.Roslyn.Asserts.csproj"),
                new DiagnosticAnalyzer[] { analyzer },
                MetadataReferences.Transitive(typeof(Benchmark).Assembly).ToArray());
            var benchmark = await Benchmark.CreateAsync(sln.Projects.Single(), analyzer).ConfigureAwait(false);
            CollectionAssert.IsNotEmpty(benchmark.ContextAndActions);
            benchmark.Run();
        }
    }
}
