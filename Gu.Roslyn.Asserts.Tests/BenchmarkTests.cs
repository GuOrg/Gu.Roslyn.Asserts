namespace Gu.Roslyn.Asserts.Tests
{
    using System.Linq;
    using System.Threading.Tasks;
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
            CollectionAssert.IsNotEmpty(benchmark.ContextAndActions);
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
            CollectionAssert.IsNotEmpty(benchmark.ContextAndActions);
            benchmark.Run();
        }
    }
}
