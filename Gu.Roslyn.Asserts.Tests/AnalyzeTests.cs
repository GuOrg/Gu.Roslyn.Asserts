namespace Gu.Roslyn.Asserts.Tests
{
    using System;
    using System.IO;
    using System.Reflection;
    using System.Threading.Tasks;
    using Microsoft.CodeAnalysis;
    using NUnit.Framework;

    public class AnalyzeTests
    {
        private static readonly MetadataReference[] MetadataReferences = new MetadataReference[0];

        [Test]
        public async Task TryFindFileInParentDirectory()
        {
            var dllFile = new Uri(Assembly.GetExecutingAssembly().CodeBase, UriKind.Absolute).LocalPath;
            Assert.AreEqual(true, CodeFactory.TryFindFileInParentDirectory(new DirectoryInfo(Path.GetDirectoryName(dllFile)), Path.GetFileNameWithoutExtension(dllFile) + ".csproj", out FileInfo projectFile));
            var diagnostics = await Analyze.GetDiagnosticsAsync(new FieldNameMustNotBeginWithUnderscore(), projectFile, MetadataReferences)
                                           .ConfigureAwait(false);
        }

        [Test]
        public async Task TryFindProjectFile()
        {
            var dllFile = new Uri(Assembly.GetExecutingAssembly().CodeBase, UriKind.Absolute).LocalPath;
            Assert.AreEqual(true, CodeFactory.TryFindProjectFile(new FileInfo(dllFile), out FileInfo projectFile));
            var diagnostics = await Analyze.GetDiagnosticsAsync(new FieldNameMustNotBeginWithUnderscore(), projectFile, MetadataReferences)
                                           .ConfigureAwait(false);
        }
    }
}