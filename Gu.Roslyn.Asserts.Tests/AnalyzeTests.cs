namespace Gu.Roslyn.Asserts.Tests
{
    using System;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using System.Threading.Tasks;
    using Microsoft.CodeAnalysis;
    using NUnit.Framework;

    public class AnalyzeTests
    {
        private static readonly MetadataReference[] MetadataReferences = new MetadataReference[0];

        [Test]
        public async Task AnalyzeProjectFile()
        {
            var dllFile = new FileInfo(new Uri(Assembly.GetExecutingAssembly().CodeBase, UriKind.Absolute).LocalPath);
            Assert.AreEqual(true, CodeFactory.TryFindFileInParentDirectory(dllFile.Directory, "Gu.Roslyn.Asserts.Tests.csproj", out FileInfo projectFile));
            var diagnostics = await Analyze.GetDiagnosticsAsync(new FieldNameMustNotBeginWithUnderscore(), projectFile, MetadataReferences)
                                           .ConfigureAwait(false);
            CollectionAssert.IsEmpty(diagnostics.SelectMany(x => x));
        }

        [Test]
        public async Task AnalyzeSolutionFile()
        {
            var dllFile = new FileInfo(new Uri(Assembly.GetExecutingAssembly().CodeBase, UriKind.Absolute).LocalPath);
            Assert.AreEqual(true, CodeFactory.TryFindFileInParentDirectory(dllFile.Directory, "Gu.Roslyn.Asserts.sln", out FileInfo solutionFile));
            var diagnostics = await Analyze.GetDiagnosticsAsync(new FieldNameMustNotBeginWithUnderscore(), solutionFile, MetadataReferences)
                                           .ConfigureAwait(false);
            CollectionAssert.IsEmpty(diagnostics.SelectMany(x => x));
        }
    }
}