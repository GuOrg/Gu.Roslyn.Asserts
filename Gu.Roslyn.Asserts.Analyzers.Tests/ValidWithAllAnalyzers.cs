﻿namespace Gu.Roslyn.Asserts.Analyzers.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.Diagnostics;

    using NUnit.Framework;

    public static class ValidWithAllAnalyzers
    {
        private static readonly IReadOnlyList<DiagnosticAnalyzer> AllAnalyzers =
            typeof(Descriptors)
                .Assembly.GetTypes()
                .Where(x => typeof(DiagnosticAnalyzer).IsAssignableFrom(x) && !x.IsAbstract)
                .Select(t => (DiagnosticAnalyzer)Activator.CreateInstance(t)!)
                .ToArray();

        private static readonly Solution AnalyzersTests = CodeFactory.CreateSolution(
            ProjectFile.Find("Gu.Roslyn.Asserts.Analyzers.Tests.csproj"));

        private static readonly Solution AssertsTests = CodeFactory.CreateSolution(
            ProjectFile.Find("Gu.Roslyn.Asserts.Tests.csproj"));

        [Test]
        public static void NotEmpty()
        {
            CollectionAssert.IsNotEmpty(AllAnalyzers);
            Assert.Pass($"Count: {AllAnalyzers.Count}");
        }

        //[TestCaseSource(nameof(AllAnalyzers))]
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

        [Explicit("Temp suppress.")]
        [TestCaseSource(nameof(AllAnalyzers))]
        public static void AssertsTestsProject(DiagnosticAnalyzer analyzer)
        {
            Assert.Inconclusive("VS does not understand [Explicit]");
            switch (analyzer)
            {
                case InvocationAnalyzer _:
                case MethodDeclarationAnalyzer _:
                    _ = Analyze.GetDiagnostics(analyzer, AssertsTests);
                    break;
                default:
                    RoslynAssert.Valid(analyzer, AssertsTests);
                    break;
            }
        }
    }
}
