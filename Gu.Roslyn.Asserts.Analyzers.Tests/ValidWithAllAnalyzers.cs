namespace Gu.Roslyn.Asserts.Analyzers.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.Diagnostics;
    using NUnit.Framework;

#pragma warning disable GURA04, GURA06
    public static class ValidWithAllAnalyzers
    {
        private static readonly IReadOnlyList<DiagnosticAnalyzer> AllAnalyzers = typeof(Descriptors)
                                                                                 .Assembly.GetTypes()
                                                                                 .Where(typeof(DiagnosticAnalyzer).IsAssignableFrom)
                                                                                 .Select(t => (DiagnosticAnalyzer)Activator.CreateInstance(t))
                                                                                 .ToArray();

        private static readonly Solution AnalyzersTests = CodeFactory.CreateSolution(
            ProjectFile.Find("Gu.Roslyn.Asserts.Analyzers.Tests.csproj"),
            MetadataReferences.FromAttributes());

        private static readonly Solution AssertsTests = CodeFactory.CreateSolution(
            ProjectFile.Find("Gu.Roslyn.Asserts.Tests.csproj"),
            MetadataReferences.FromAttributes());

        [Test]
        public static void NotEmpty()
        {
            CollectionAssert.IsNotEmpty(AllAnalyzers);
            Assert.Pass($"Count: {AllAnalyzers.Count}");
        }

        [TestCaseSource(nameof(AllAnalyzers))]
        public static void AnalyzersTestsProject(DiagnosticAnalyzer analyzer)
        {
            switch (analyzer)
            {
                case InvocationAnalyzer _:
                case MethodDeclarationAnalyzer _:
                    _ = Analyze.GetDiagnostics(analyzer, AnalyzersTests);
                    break;
                default:
                    RoslynAssert.Valid(analyzer, AnalyzersTests);
                    break;
            }
        }

        [TestCaseSource(nameof(AllAnalyzers))]
        public static void AssertsTestsProject(DiagnosticAnalyzer analyzer)
        {
            RoslynAssert.Valid(analyzer, AssertsTests);
            //switch (analyzer)
            //{
            //    case InvocationAnalyzer _:
            //    case MethodDeclarationAnalyzer _:
            //        _ = Analyze.GetDiagnostics(analyzer, AssertsTests);
            //        break;
            //    default:
            //        RoslynAssert.Valid(analyzer, AssertsTests);
            //        break;
            //}
        }
    }
}
