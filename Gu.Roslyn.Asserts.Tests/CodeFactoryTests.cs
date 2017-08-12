// ReSharper disable AssignNullToNotNullAttribute
// ReSharper disable PossibleNullReferenceException
namespace Gu.Roslyn.Asserts.Tests
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using Microsoft.CodeAnalysis;
    using NUnit.Framework;

    public class CodeFactoryTests
    {
        [Test]
        public void TryFindProjectFileInParentDirectory()
        {
            var codeBase = Assembly.GetExecutingAssembly().CodeBase;
            var dllFile = new Uri(codeBase, UriKind.Absolute).LocalPath;
            var directory = new DirectoryInfo(Path.GetDirectoryName(dllFile));
            Assert.AreEqual(true, CodeFactory.TryFindFileInParentDirectory(directory, "Gu.Roslyn.Asserts.Tests.csproj", out FileInfo projectFile));
            Assert.AreEqual("Gu.Roslyn.Asserts.Tests.csproj", projectFile.Name);
        }

        [Test]
        public void TryFindSolutionFileInParentDirectory()
        {
            var codeBase = Assembly.GetExecutingAssembly().CodeBase;
            var dllFile = new Uri(codeBase, UriKind.Absolute).LocalPath;
            var directory = new DirectoryInfo(Path.GetDirectoryName(dllFile));
            Assert.AreEqual(true, CodeFactory.TryFindFileInParentDirectory(directory, "Gu.Roslyn.Asserts.sln", out FileInfo projectFile));
            Assert.AreEqual("Gu.Roslyn.Asserts.sln", projectFile.Name);
        }

        [Test]
        public void TryFindProjectFile()
        {
            var codeBase = Assembly.GetExecutingAssembly().CodeBase;
            var dllFile = new FileInfo(new Uri(codeBase, UriKind.Absolute).LocalPath);
            Assert.AreEqual(true, CodeFactory.TryFindProjectFile(dllFile, out FileInfo projectFile));
            Assert.AreEqual("Gu.Roslyn.Asserts.Tests.csproj", projectFile.Name);
        }

        [Test]
        public void CreateSolutionFromProjectFile()
        {
            var codeBase = Assembly.GetExecutingAssembly().CodeBase;
            var dllFile = new FileInfo(new Uri(codeBase, UriKind.Absolute).LocalPath);
            Assert.AreEqual(true, CodeFactory.TryFindProjectFile(dllFile, out FileInfo projectFile));
            var solution = CodeFactory.CreateSolution(
                projectFile,
                new[] { new FieldNameMustNotBeginWithUnderscore(), },
                CreateMetadataReferences(typeof(object)));
            Assert.AreEqual("Gu.Roslyn.Asserts.Tests", solution.Projects.Single().Name);
            var expected = projectFile.Directory
                                      .EnumerateFiles("*.cs", SearchOption.AllDirectories)
                                      .Select(f => f.Name)
                                      .Where(x => !x.StartsWith("TemporaryGeneratedFile_"))
                                      .OrderBy(x => x)
                                      .ToArray();
            var actual = solution.Projects
                                 .SelectMany(p => p.Documents)
                                 .Select(d => d.Name)
                                 .OrderBy(x => x)
                                 .ToArray();
            //// ReSharper disable UnusedVariable for debug.
            var expectedString = string.Join(Environment.NewLine, expected);
            var actualString = string.Join(Environment.NewLine, actual);
            //// ReSharper restore UnusedVariable
            CollectionAssert.AreEqual(expected, actual);
        }

        [Test]
        public void CreateSolutionFromSolutionFile()
        {
            var codeBase = Assembly.GetExecutingAssembly().CodeBase;
            var dllFile = new FileInfo(new Uri(codeBase, UriKind.Absolute).LocalPath);
            Assert.AreEqual(true, CodeFactory.TryFindFileInParentDirectory(dllFile.Directory, "Gu.Roslyn.Asserts.sln", out FileInfo solutionFile));
            var solution = CodeFactory.CreateSolution(
                solutionFile,
                new[] { new FieldNameMustNotBeginWithUnderscore(), },
                CreateMetadataReferences(typeof(object)));
            var expectedSlns = new[]
            {
                "Gu.Roslyn.Asserts",
                "Gu.Roslyn.Asserts.Tests",
                "Gu.Roslyn.Asserts.Tests.WithMetadataReferencesAttribute",
                "Gu.Roslyn.Asserts.XUnit"
            };

            CollectionAssert.AreEquivalent(expectedSlns, solution.Projects.Select(p => p.Name));

            var expected = solutionFile.Directory
                                       .EnumerateFiles("*.cs", SearchOption.AllDirectories)
                                       .Select(f => f.Name)
                                       .Where(x => !x.StartsWith("TemporaryGeneratedFile_"))
                                       .Distinct()
                                       .OrderBy(x => x)
                                       .ToArray();
            var actual = solution.Projects
                                 .SelectMany(p => p.Documents)
                                 .Select(d => d.Name)
                                 .Distinct()
                                 .OrderBy(x => x)
                                 .ToArray();
            //// ReSharper disable UnusedVariable for debug.
            var expectedString = string.Join(Environment.NewLine, expected);
            var actualString = string.Join(Environment.NewLine, actual);
            //// ReSharper restore UnusedVariable

            CollectionAssert.AreEqual(expected, actual);
        }

        private static IReadOnlyList<MetadataReference> CreateMetadataReferences(params Type[] types)
        {
            return types.Select(type => type.GetTypeInfo().Assembly)
                        .Distinct()
                        .Select(assembly => MetadataReference.CreateFromFile(assembly.Location))
                        .ToArray();
        }
    }
}