namespace Gu.Roslyn.Asserts.Tests;

using System.Linq;
using NUnit.Framework;

public static partial class CodeFactoryTests
{
    public static class FromText
    {
        [Test]
        public static void CreateSolutionFromSourceFileScoped()
        {
            var code = """
                namespace N;

                class C
                {
                    private readonly int _value;
                }
                """;
            var sln = CodeFactory.CreateSolution(code);
            Assert.AreEqual("N", sln.Projects.Single().Name);
            Assert.AreEqual("C.cs", sln.Projects.Single().Documents.Single().Name);
        }

        [Test]
        public static void CreateSolutionFromSourceLegacy()
        {
            var code = """
                namespace N
                {
                    class C
                    {
                        private readonly int _value;
                    }
                }
                """;
            var sln = CodeFactory.CreateSolution(code);
            Assert.AreEqual("N", sln.Projects.Single().Name);
            Assert.AreEqual("C.cs", sln.Projects.Single().Documents.Single().Name);
        }

        [Test]
        public static void CreateSolutionFromSourcesFileScoped()
        {
            var code1 = """
                namespace Project1;

                class C1
                {
                    private readonly int _value;
                }
                """;

            var code2 = """
                namespace Project2;

                class C2
                {
                    private readonly int _value;
                }
                """;
            var sln = CodeFactory.CreateSolution(new[] { code1, code2 });
            CollectionAssert.AreEqual(new[] { "Project1", "Project2" }, sln.Projects.Select(x => x.Name));
            Assert.AreEqual(new[] { "C1.cs", "C2.cs" }, sln.Projects.Select(x => x.Documents.Single().Name));

            sln = CodeFactory.CreateSolution(new[] { code2, code1 });
            CollectionAssert.AreEqual(new[] { "Project1", "Project2" }, sln.Projects.Select(x => x.Name));
            Assert.AreEqual(new[] { "C1.cs", "C2.cs" }, sln.Projects.Select(x => x.Documents.Single().Name));
        }

        [Test]
        public static void CreateSolutionFromSourcesLegacy()
        {
            var code1 = """
                namespace Project1
                {
                    class C1
                    {
                        private readonly int _value;
                    }
                }
                """;

            var code2 = """
                namespace Project2
                {
                    class C2
                    {
                        private readonly int _value;
                    }
                }
                """;
            var sln = CodeFactory.CreateSolution(new[] { code1, code2 });
            CollectionAssert.AreEqual(new[] { "Project1", "Project2" }, sln.Projects.Select(x => x.Name));
            Assert.AreEqual(new[] { "C1.cs", "C2.cs" }, sln.Projects.Select(x => x.Documents.Single().Name));

            sln = CodeFactory.CreateSolution(new[] { code2, code1 });
            CollectionAssert.AreEqual(new[] { "Project1", "Project2" }, sln.Projects.Select(x => x.Name));
            Assert.AreEqual(new[] { "C1.cs", "C2.cs" }, sln.Projects.Select(x => x.Documents.Single().Name));
        }

        [Test]
        public static void CreateSolutionWithDependenciesFromUsings()
        {
            var code1 = """
                namespace Project1
                {
                    class C1
                    {
                        private readonly int _value;
                    }
                }
                """;

            var code2 = """
                namespace Project2
                {
                    using Project1;

                    class C2
                    {
                        private readonly C1 _value;
                    }
                }
                """;
            var sln = CodeFactory.CreateSolution(new[] { code1, code2 });
            CollectionAssert.AreEqual(new[] { "Project1", "Project2" }, sln.Projects.Select(x => x.Name));
            Assert.AreEqual(new[] { "C1.cs", "C2.cs" }, sln.Projects.Select(x => x.Documents.Single().Name));
            var project1 = sln.Projects.Single(x => x.Name == "Project1");
            CollectionAssert.IsEmpty(project1.AllProjectReferences);
            var project2 = sln.Projects.Single(x => x.Name == "Project2");
            CollectionAssert.AreEqual(new[] { project1.Id }, project2.AllProjectReferences.Select(x => x.ProjectId));
        }

        [Test]
        public static void CreateSolutionWithDependenciesFromQualified()
        {
            var code1 = """
                namespace Project1
                {
                    public class C1
                    {
                        private readonly int _value;
                    }
                }
                """;

            var code2 = """
                namespace Project2
                {
                    public class C2
                    {
                        private readonly Project1.C1 _value;
                    }
                }
                """;
            var sln = CodeFactory.CreateSolution(new[] { code1, code2 });
            CollectionAssert.AreEqual(new[] { "Project1", "Project2" }, sln.Projects.Select(x => x.Name));
            CollectionAssert.AreEqual(new[] { "C1.cs", "C2.cs" }, sln.Projects.Select(x => x.Documents.Single().Name));
            var project1 = sln.Projects.Single(x => x.Name == "Project1");
            CollectionAssert.IsEmpty(project1.AllProjectReferences);
            var project2 = sln.Projects.Single(x => x.Name == "Project2");
            CollectionAssert.AreEqual(new[] { project1.Id }, project2.AllProjectReferences.Select(x => x.ProjectId));
        }

        [Test]
        public static void CreateSolutionWithInheritQualified()
        {
            var code1 = """
                namespace N.Core
                {
                    public class C1
                    {
                        private readonly int _value;
                    }
                }
                """;

            var code2 = """
                namespace N.Client
                {
                    public class C2 : N.Core.C1
                    {
                    }
                }
                """;
            foreach (var sources in new[] { new[] { code1, code2 }, new[] { code2, code1 } })
            {
                var sln = CodeFactory.CreateSolution(sources);
                CollectionAssert.AreEquivalent(new[] { "N.Core", "N.Client" }, sln.Projects.Select(x => x.Name));
                CollectionAssert.AreEquivalent(new[] { "C1.cs", "C2.cs" }, sln.Projects.Select(x => x.Documents.Single().Name));
                var project1 = sln.Projects.Single(x => x.Name == "N.Core");
                CollectionAssert.IsEmpty(project1.AllProjectReferences);
                var project2 = sln.Projects.Single(x => x.Name == "N.Client");
                CollectionAssert.AreEqual(new[] { project1.Id }, project2.AllProjectReferences.Select(x => x.ProjectId));
            }
        }

        [Test]
        public static void CreateSolutionWithOneProject()
        {
            var code1 = """
                namespace N.Core
                {
                    public class C1
                    {
                        private readonly int _value;
                    }
                }
                """;

            var code2 = """
                namespace N.Bar
                {
                    public class C2 : N.Core.C1
                    {
                    }
                }
                """;
            foreach (var sources in new[] { new[] { code1, code2 }, new[] { code2, code1 } })
            {
                var sln = CodeFactory.CreateSolutionWithOneProject(sources);
                var project = sln.Projects.Single();
                Assert.AreEqual("N", project.AssemblyName);
                CollectionAssert.AreEquivalent(new[] { "C1.cs", "C2.cs" }, project.Documents.Select(x => x.Name));
            }
        }

        [Test]
        public static void CreateSolutionWhenNestedNamespaces()
        {
            var resourcesCode = """
                namespace N.Properties
                {
                    public class Resources
                    {
                    }
                }
                """;

            var testCode = """
                namespace N
                {
                    using N.Properties;

                    public class C
                    {
                    }
                }
                """;
            foreach (var sources in new[] { new[] { resourcesCode, testCode }, new[] { resourcesCode, testCode } })
            {
                var sln = CodeFactory.CreateSolution(sources);
                var project = sln.Projects.Single();
                Assert.AreEqual("N", project.AssemblyName);
                CollectionAssert.AreEquivalent(new[] { "Resources.cs", "C.cs" }, project.Documents.Select(x => x.Name));
            }
        }
    }
}
