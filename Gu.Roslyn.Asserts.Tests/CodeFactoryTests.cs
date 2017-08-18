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
        private static readonly FileInfo ExecutingAssemblyDll = new FileInfo(new Uri(Assembly.GetExecutingAssembly().CodeBase, UriKind.Absolute).LocalPath);

        [Test]
        public void TryFindProjectFileInParentDirectory()
        {
            var directory = ExecutingAssemblyDll.Directory;
            Assert.AreEqual(true, CodeFactory.TryFindFileInParentDirectory(directory, "Gu.Roslyn.Asserts.Tests.csproj", out FileInfo projectFile));
            Assert.AreEqual("Gu.Roslyn.Asserts.Tests.csproj", projectFile.Name);
        }

        [Test]
        public void TryFindSolutionFileInParentDirectory()
        {
            var directory = ExecutingAssemblyDll.Directory;
            Assert.AreEqual(true, CodeFactory.TryFindFileInParentDirectory(directory, "Gu.Roslyn.Asserts.sln", out FileInfo projectFile));
            Assert.AreEqual("Gu.Roslyn.Asserts.sln", projectFile.Name);
        }

        [Test]
        public void TryFindProjectFile()
        {
            Assert.AreEqual(true, CodeFactory.TryFindProjectFile(ExecutingAssemblyDll, out FileInfo projectFile));
            Assert.AreEqual("Gu.Roslyn.Asserts.Tests.csproj", projectFile.Name);
        }

        [Test]
        public void CreateSolutionFromProjectFile()
        {
            Assert.AreEqual(true, CodeFactory.TryFindProjectFile(ExecutingAssemblyDll, out FileInfo projectFile));
            var solution = CodeFactory.CreateSolution(
                projectFile,
                new[] { new FieldNameMustNotBeginWithUnderscore(), },
                CreateMetadataReferences(typeof(object)));
            Assert.AreEqual("Gu.Roslyn.Asserts.Tests", solution.Projects.Single().Name);
            var expected = projectFile.Directory
                                      .EnumerateFiles("*.cs", SearchOption.AllDirectories)
                                      .Where(f => !f.DirectoryName.Contains("bin"))
                                      .Where(f => !f.DirectoryName.Contains("obj"))
                                      .Select(f => f.Name)
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
            Assert.AreEqual(true, CodeFactory.TryFindFileInParentDirectory(ExecutingAssemblyDll.Directory, "Gu.Roslyn.Asserts.sln", out FileInfo solutionFile));
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
                                       .Where(f => !f.DirectoryName.Contains(".vs"))
                                       .Where(f => !f.DirectoryName.Contains(".git"))
                                       .Where(f => !f.DirectoryName.Contains("bin"))
                                       .Where(f => !f.DirectoryName.Contains("obj"))
                                       .Select(f => f.Name)
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