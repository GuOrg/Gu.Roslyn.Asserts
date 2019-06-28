namespace Gu.Roslyn.Asserts.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using NUnit.Framework;

    public static class BenchmarkTests
    {
        private static readonly Solution SolutionWithClassLibrary1 = CodeFactory.CreateSolution(ProjectFile.Find("ClassLibrary1.csproj"));

        [Test]
        public static async Task Solution()
        {
            var analyzer = new FieldNameMustNotBeginWithUnderscore();
            var sln = CodeFactory.CreateSolution(SolutionFile.Find("Gu.Roslyn.Asserts.sln"));
            var benchmark = await Benchmark.CreateAsync(sln, analyzer).ConfigureAwait(false);
            CollectionAssert.IsNotEmpty(benchmark.SyntaxNodeActions);
            CollectionAssert.AllItemsAreInstancesOfType(benchmark.SyntaxNodeActions.Select(x => x.Context.Node), typeof(FieldDeclarationSyntax));
            CollectionAssert.AllItemsAreInstancesOfType(benchmark.SyntaxNodeActions.Select(x => x.Context.ContainingSymbol), typeof(IFieldSymbol));
            Assert.AreSame(analyzer, benchmark.Analyzer);
            benchmark.Run();
            benchmark.Run();
        }

        [Test]
        public static async Task Project()
        {
            var analyzer = new FieldNameMustNotBeginWithUnderscore();
            var sln = CodeFactory.CreateSolution(ProjectFile.Find("Gu.Roslyn.Asserts.csproj"));
            var benchmark = await Benchmark.CreateAsync(sln.Projects.Single(), analyzer).ConfigureAwait(false);
            CollectionAssert.IsNotEmpty(benchmark.SyntaxNodeActions);
            Assert.AreSame(analyzer, benchmark.Analyzer);
            benchmark.Run();
            benchmark.Run();
        }

        [Test]
        public static async Task ClassLibrary1FieldNameMustNotBeginWithUnderscore()
        {
            var analyzer = new FieldNameMustNotBeginWithUnderscore();
            var benchmark = await Benchmark.CreateAsync(SolutionWithClassLibrary1, analyzer).ConfigureAwait(false);
            var expected = new[] { "private int _value;" };
            CollectionAssert.AreEqual(expected, benchmark.SyntaxNodeActions.Select(x => x.Context.Node.ToString()));

            expected = new[] { "ClassLibrary1.ClassLibrary1Class1._value" };
            CollectionAssert.AreEqual(expected, benchmark.SyntaxNodeActions.Select(x => x.Context.ContainingSymbol.ToString()));
            Assert.AreSame(analyzer, benchmark.Analyzer);
            benchmark.Run();
            benchmark.Run();
        }

        [Test]
        public static async Task ClassLibrary1FieldDeclarations()
        {
            var analyzer = new SyntaxNodeAnalyzer(SyntaxKind.FieldDeclaration);
            var benchmark = await Benchmark.CreateAsync(SolutionWithClassLibrary1, analyzer).ConfigureAwait(false);
            var expected = new List<string> { "private int _value;" };
            CollectionAssert.AreEqual(expected, benchmark.SyntaxNodeActions.Select(x => x.Context.Node.ToString()));
            CollectionAssert.IsEmpty(analyzer.Contexts);

            benchmark.Run();
            CollectionAssert.AreEqual(expected, analyzer.Contexts.Select(x => x.Node.ToString()));

            expected.AddRange(expected);
            benchmark.Run();
            CollectionAssert.AreEqual(expected, analyzer.Contexts.Select(x => x.Node.ToString()));
        }

        [Test]
        public static async Task ClassLibrary1FieldSymbols()
        {
            var analyzer = new SymbolAnalyzer(SymbolKind.Field);
            var benchmark = await Benchmark.CreateAsync(SolutionWithClassLibrary1, analyzer).ConfigureAwait(false);
            var expected = new List<string>
                           {
                               "Gu.Roslyn.Asserts.AllowCompilationErrors.No",
                               "Gu.Roslyn.Asserts.AllowCompilationErrors.Yes",
                               "ClassLibrary1.ClassLibrary1Class1._value",
                           };

            CollectionAssert.AreEquivalent(expected, benchmark.SymbolActions.Select(x => x.Context.Symbol.ToString()));
            CollectionAssert.IsEmpty(analyzer.Contexts);

            benchmark.Run();
            CollectionAssert.AreEquivalent(expected, analyzer.Contexts.Select(x => x.Symbol.ToString()));

            expected.AddRange(expected);
            benchmark.Run();
            CollectionAssert.AreEquivalent(expected, analyzer.Contexts.Select(x => x.Symbol.ToString()));
        }

        // ReSharper disable once UnusedMember.Local
        private static void Dump(IEnumerable<string> strings)
        {
            foreach (var s in strings)
            {
                Console.WriteLine($"\"{s}\",");
            }
        }
    }
}
