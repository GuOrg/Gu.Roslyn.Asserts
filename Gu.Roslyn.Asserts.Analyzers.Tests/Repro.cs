namespace Gu.Roslyn.Asserts.Analyzers.Tests
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.Diagnostics;
    using NUnit.Framework;

#pragma warning disable GURA04, GURA06
    [Explicit("Only for digging out test cases.")]
    public static class Repro
    {
        // ReSharper disable once UnusedMember.Local
        private static readonly IReadOnlyList<DiagnosticAnalyzer> AllAnalyzers =
            typeof(Descriptors).Assembly.GetTypes()
                               .Where(typeof(DiagnosticAnalyzer).IsAssignableFrom)
                               .Select(t => (DiagnosticAnalyzer)Activator.CreateInstance(t))
                               .ToArray();

        private static readonly Solution Solution = CodeFactory.CreateSolution(
            new FileInfo(@"C:\Git\_GuOrg\Gu.Analyzers\Gu.Analyzers.sln"),
            AllAnalyzers,
            MetadataReferences.FromAttributes());

        [TestCaseSource(nameof(AllAnalyzers))]
        public static void Run(DiagnosticAnalyzer analyzer)
        {
            Assert.Inconclusive("VS does not understand [Explicit]");
            var diagnostics = Analyze.GetDiagnostics(Solution, analyzer);
            RoslynAssert.NoDiagnostics(diagnostics);
        }
    }
}
