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

    public partial class CodeFactoryTests
    {
        public class FromFiles
        {
            private static readonly FileInfo ExecutingAssemblyDll = new FileInfo(new Uri(Assembly.GetExecutingAssembly().CodeBase, UriKind.Absolute).LocalPath);

            [Test]
            public void TryFindProjectFileInParentDirectory()
            {
                var directory = ExecutingAssemblyDll.Directory;
                var projectFileName = Path.GetFileNameWithoutExtension(ExecutingAssemblyDll.FullName) + ".csproj";
                Assert.AreEqual(true, CodeFactory.TryFindFileInParentDirectory(directory, projectFileName, out var projectFile));
                Assert.AreEqual(projectFileName, projectFile.Name);
            }

            [Test]
            public void TryFindSolutionFileInParentDirectory()
            {
                var directory = ExecutingAssemblyDll.Directory;
                Assert.AreEqual(true, CodeFactory.TryFindFileInParentDirectory(directory, "Gu.Roslyn.Asserts.sln", out var projectFile));
                Assert.AreEqual("Gu.Roslyn.Asserts.sln", projectFile.Name);
            }

            [Test]
            public void TryFindProjectFileFromDll()
            {
                Assert.AreEqual(true, CodeFactory.TryFindProjectFile(ExecutingAssemblyDll, out var projectFile));
                Assert.AreEqual(Path.GetFileNameWithoutExtension(ExecutingAssemblyDll.FullName) + ".csproj", projectFile.Name);
            }

            [TestCase("Gu.Roslyn.Asserts.Tests.csproj")]
            [TestCase("WpfApp1.csproj")]
            public void TryFindProjectFileFromName(string name)
            {
                Assert.AreEqual(true, CodeFactory.TryFindProjectFile(name, out var projectFile));
                Assert.AreEqual(name, projectFile.Name);
            }

            [Test]
            public void CreateSolutionFromProjectFile()
            {
                Assert.AreEqual(true, CodeFactory.TryFindProjectFile(ExecutingAssemblyDll, out var projectFile));
                var solution = CodeFactory.CreateSolution(
                    projectFile,
                    new[] { new FieldNameMustNotBeginWithUnderscore(), },
                    CreateMetadataReferences(typeof(object)));
                Assert.AreEqual(Path.GetFileNameWithoutExtension(ExecutingAssemblyDll.FullName), solution.Projects.Single().Name);
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
            public void CreateSolutionFromWpfApp1()
            {
                Assert.AreEqual(true, CodeFactory.TryFindProjectFile("WpfApp1.csproj", out var projectFile));
                var solution = CodeFactory.CreateSolution(
                    projectFile,
                    new[] { new FieldNameMustNotBeginWithUnderscore(), },
                    CreateMetadataReferences(typeof(object)));
                Assert.AreEqual("WpfApp1", solution.Projects.Single().Name);
                var expected = new[]
                {
                    "App.xaml.cs",
                    "AssemblyInfo.cs",
                    "Class1.cs",
                    "MainWindow.xaml.cs",
                    "Resources.Designer.cs",
                    "Settings.Designer.cs"
                };
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
                Assert.AreEqual(true, CodeFactory.TryFindSolutionFile("Gu.Roslyn.Asserts.sln", out var solutionFile));
                var solution = CodeFactory.CreateSolution(
                    solutionFile,
                    new[] { new FieldNameMustNotBeginWithUnderscore(), },
                    CreateMetadataReferences(typeof(object)));
                var expectedProjects = new[]
                                   {
                                       "WpfApp1",
                                       "Gu.Roslyn.Asserts",
                                       "Gu.Roslyn.Asserts.Tests",
                                       "Gu.Roslyn.Asserts.Tests.WithMetadataReferencesAttribute",
                                       "Gu.Roslyn.Asserts.XUnit"
                                   };

                CollectionAssert.AreEquivalent(expectedProjects, solution.Projects.Select(p => p.Name));

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

            [Test]
            public void CreateSolutionFromSolutionFileAddsDependencies()
            {
                Assert.AreEqual(true, CodeFactory.TryFindFileInParentDirectory(ExecutingAssemblyDll.Directory, "Gu.Roslyn.Asserts.sln", out var solutionFile));
                var sln = CodeFactory.CreateSolution(
                    solutionFile,
                    new[] { new FieldNameMustNotBeginWithUnderscore() },
                    CreateMetadataReferences(typeof(object)));
                var assertsProject = sln.Projects.Single(x => x.Name == "Gu.Roslyn.Asserts");
                CollectionAssert.IsEmpty(assertsProject.AllProjectReferences);

                var testProject = sln.Projects.Single(x => x.Name == "Gu.Roslyn.Asserts.Tests");
                CollectionAssert.AreEqual(new[] { assertsProject.Id }, testProject.AllProjectReferences.Select(x => x.ProjectId).ToArray());
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
}