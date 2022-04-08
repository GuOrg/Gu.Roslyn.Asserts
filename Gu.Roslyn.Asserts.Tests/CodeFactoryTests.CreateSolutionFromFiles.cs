// ReSharper disable PossibleNullReferenceException
namespace Gu.Roslyn.Asserts.Tests
{
    using System;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using NUnit.Framework;

    public static partial class CodeFactoryTests
    {
        public static class CreateSolutionFromFiles
        {
            private static readonly FileInfo ExecutingAssemblyDll = new(Assembly.GetExecutingAssembly().Location);

            [Test]
            public static void CreateSolutionFromProjectFile()
            {
                Assert.AreEqual(true, ProjectFile.TryFind(ExecutingAssemblyDll, out var projectFile));
                var solution = CodeFactory.CreateSolution(projectFile);
                Assert.AreEqual(Path.GetFileNameWithoutExtension(ExecutingAssemblyDll.FullName), solution.Projects.Single().Name);
                var expected = projectFile!.Directory
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
            public static void CreateSolutionFromWpfApp1()
            {
                Assert.AreEqual(true, ProjectFile.TryFind("WpfApp1.csproj", out var projectFile));
                var solution = CodeFactory.CreateSolution(projectFile);
                Assert.AreEqual("WpfApp1", solution.Projects.Single().Name);
                var expected = new[]
                {
                    "App.xaml.cs",
                    "AssemblyAttributes.cs",
                    "Class1.cs",
                    "MainWindow.xaml.cs",
                    "Resources.Designer.cs",
                    "Settings.Designer.cs",
                    "UserControl1.xaml.cs",
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
            public static void CreateSolutionFromClassLibrary1()
            {
                Assert.AreEqual(true, ProjectFile.TryFind("ClassLibrary1.csproj", out var projectFile));
                var solution = CodeFactory.CreateSolution(projectFile);
                Assert.AreEqual("ClassLibrary1", solution.Projects.Single().Name);
                var expected = new[]
                {
                    "AllowedCompilerDiagnostics.cs",
                    "AssemblyAttributes.cs",
                    "AssemblyInfo.cs",
                    "ClassLibrary1Class1.cs",
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
            public static void CreateSolutionFromClassLibrary2()
            {
                Assert.AreEqual(true, ProjectFile.TryFind("ClassLibrary2.csproj", out var projectFile));
                var solution = CodeFactory.CreateSolution(projectFile);
                Assert.AreEqual("ClassLibrary2", solution.Projects.Single().Name);
                var expected = new[]
                {
                    "AssemblyAttributes.cs",
                    "ClassLibrary2Class1.cs",
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
            public static void CreateSolutionFromSolutionFile()
            {
                Assert.AreEqual(true, SolutionFile.TryFind("Gu.Roslyn.Asserts.sln", out var solutionFile));
                var solution = CodeFactory.CreateSolution(solutionFile);
                var expectedProjects = new[]
                {
                    "AstView",
                    "WpfApp1",
                    "ClassLibrary1",
                    "ClassLibrary2",
                    "Gu.Roslyn.Asserts",
                    "Gu.Roslyn.Asserts.Analyzers",
                    "Gu.Roslyn.Asserts.Analyzers.Tests",
                    "Gu.Roslyn.Asserts.Analyzers.Vsix",
                    "Gu.Roslyn.Asserts.Tests",
                };

                CollectionAssert.AreEquivalent(expectedProjects, solution.Projects.Select(p => p.Name));

                var expected = solutionFile!.Directory
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
            public static void CreateSolutionFromSolutionFileAddsDependencies()
            {
                var sln = CodeFactory.CreateSolution(SolutionFile.Find("Gu.Roslyn.Asserts.sln"));
                var assertsProject = sln.Projects.Single(x => x.Name == "Gu.Roslyn.Asserts");
                var analyzersProject = sln.Projects.Single(x => x.Name == "Gu.Roslyn.Asserts.Analyzers");
                CollectionAssert.IsEmpty(analyzersProject.ProjectReferences);

                var testProject = sln.Projects.Single(x => x.Name == "Gu.Roslyn.Asserts.Tests");
                CollectionAssert.AreEqual(
                    new[] { assertsProject.Id, analyzersProject.Id },
                    testProject.AllProjectReferences.Select(x => x.ProjectId).ToArray());
            }

            [Test]
            public static void CreateSolutionWithTwoAnalyzersReportingSameDiagnostic()
            {
                Assert.AreEqual(true, ProjectFile.TryFind("ClassLibrary1.csproj", out var projectFile));
                var solution = CodeFactory.CreateSolution(projectFile);
                Assert.AreEqual("ClassLibrary1", solution.Projects.Single().Name);
                var expected = new[]
                {
                    "AllowedCompilerDiagnostics.cs",
                    "AssemblyAttributes.cs",
                    "AssemblyInfo.cs",
                    "ClassLibrary1Class1.cs",
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
        }
    }
}
