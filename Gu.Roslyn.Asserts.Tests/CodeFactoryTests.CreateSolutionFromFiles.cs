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

    public static partial class CodeFactoryTests
    {
        public static class CreateSolutionFromFiles
        {
            private static readonly FileInfo ExecutingAssemblyDll = new FileInfo(new Uri(Assembly.GetExecutingAssembly().CodeBase, UriKind.Absolute).LocalPath);

            [Test]
            public static void CreateSolutionFromProjectFile()
            {
                Assert.AreEqual(true, ProjectFile.TryFind(ExecutingAssemblyDll, out var projectFile));
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
            public static void CreateSolutionFromWpfApp1()
            {
                Assert.AreEqual(true, ProjectFile.TryFind("WpfApp1.csproj", out var projectFile));
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
                var solution = CodeFactory.CreateSolution(
                    projectFile,
                    new[] { new FieldNameMustNotBeginWithUnderscore(), },
                    CreateMetadataReferences(typeof(object)));
                Assert.AreEqual("ClassLibrary1", solution.Projects.Single().Name);
                var expected = new[]
                               {
                                   "AllowCompilationErrors.cs",
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
                var solution = CodeFactory.CreateSolution(
                    projectFile,
                    new[] { new FieldNameMustNotBeginWithUnderscore(), },
                    CreateMetadataReferences(typeof(object)));
                Assert.AreEqual("ClassLibrary2", solution.Projects.Single().Name);
                var expected = new[]
                               {
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
                var solution = CodeFactory.CreateSolution(
                    solutionFile,
                    new[] { new FieldNameMustNotBeginWithUnderscore(), },
                    CreateMetadataReferences(typeof(object)));
                var expectedProjects = new[]
                {
                    "AstView",
                    "WpfApp1",
                    "ClassLibrary1",
                    "ClassLibrary2",
                    "Gu.Roslyn.Asserts",
                    "Gu.Roslyn.Asserts.Analyzers",
                    "Gu.Roslyn.Asserts.Analyzers.Tests",
                    "Gu.Roslyn.Asserts.Tests",
                    "Gu.Roslyn.Asserts.Tests.Net472WithAttributes",
                    "Gu.Roslyn.Asserts.Tests.NetCoreWithAttributes",
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
            public static void CreateSolutionFromSolutionFileAddsDependencies()
            {
                var sln = CodeFactory.CreateSolution(
                    SolutionFile.Find("Gu.Roslyn.Asserts.sln"),
                    new[] { new FieldNameMustNotBeginWithUnderscore() },
                    CreateMetadataReferences(typeof(object)));
                var assertsProject = sln.Projects.Single(x => x.Name == "Gu.Roslyn.Asserts");
                CollectionAssert.IsEmpty(assertsProject.AllProjectReferences);

                var testProject = sln.Projects.Single(x => x.Name == "Gu.Roslyn.Asserts.Tests");
                CollectionAssert.AreEqual(new[] { assertsProject.Id }, testProject.AllProjectReferences.Select(x => x.ProjectId).ToArray());
            }

            [Test]
            public static void CreateSolutionWithTwoAnalyzersReportingSameDiagnostic()
            {
                Assert.AreEqual(true, ProjectFile.TryFind("ClassLibrary1.csproj", out var projectFile));
                var solution = CodeFactory.CreateSolution(
                    projectFile,
                    new[] { new SyntaxNodeAnalyzer(Descriptors.Id1), new SyntaxNodeAnalyzer(Descriptors.Id1) },
                    CreateMetadataReferences(typeof(object)));
                Assert.AreEqual("ClassLibrary1", solution.Projects.Single().Name);
                var expected = new[]
                               {
                                   "AllowCompilationErrors.cs",
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
