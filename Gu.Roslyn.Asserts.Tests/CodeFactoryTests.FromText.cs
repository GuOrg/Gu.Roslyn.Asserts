namespace Gu.Roslyn.Asserts.Tests
{
    using System.Linq;
    using NUnit.Framework;

    public static partial class CodeFactoryTests
    {
        public static class FromText
        {
            [Test]
            public static void CreateSolutionFromSource()
            {
                var code = @"
namespace RoslynSandbox
{
    class Foo
    {
        private readonly int _value;
    }
}";
                var sln = CodeFactory.CreateSolution(code, new[] { new FieldNameMustNotBeginWithUnderscore() });
                Assert.AreEqual("RoslynSandbox", sln.Projects.Single().Name);
                Assert.AreEqual("Foo.cs", sln.Projects.Single().Documents.Single().Name);
            }

            [Test]
            public static void CreateSolutionFromSources()
            {
                var code1 = @"
namespace Project1
{
    class Foo1
    {
        private readonly int _value;
    }
}";

                var code2 = @"
namespace Project2
{
    class Foo2
    {
        private readonly int _value;
    }
}";
                var sln = CodeFactory.CreateSolution(new[] { code1, code2 });
                CollectionAssert.AreEqual(new[] { "Project1", "Project2" }, sln.Projects.Select(x => x.Name));
                Assert.AreEqual(new[] { "Foo1.cs", "Foo2.cs" }, sln.Projects.Select(x => x.Documents.Single().Name));

                sln = CodeFactory.CreateSolution(new[] { code2, code1 });
                CollectionAssert.AreEqual(new[] { "Project1", "Project2" }, sln.Projects.Select(x => x.Name));
                Assert.AreEqual(new[] { "Foo1.cs", "Foo2.cs" }, sln.Projects.Select(x => x.Documents.Single().Name));
            }

            [Test]
            public static void CreateSolutionWithDependenciesFromUsings()
            {
                var code1 = @"
namespace Project1
{
    class Foo1
    {
        private readonly int _value;
    }
}";

                var code2 = @"
namespace Project2
{
    using Project1;

    class Foo2
    {
        private readonly Foo1 _value;
    }
}";
                var sln = CodeFactory.CreateSolution(new[] { code1, code2 }, new[] { new FieldNameMustNotBeginWithUnderscore() });
                CollectionAssert.AreEqual(new[] { "Project1", "Project2" }, sln.Projects.Select(x => x.Name));
                Assert.AreEqual(new[] { "Foo1.cs", "Foo2.cs" }, sln.Projects.Select(x => x.Documents.Single().Name));
                var project1 = sln.Projects.Single(x => x.Name == "Project1");
                CollectionAssert.IsEmpty(project1.AllProjectReferences);
                var project2 = sln.Projects.Single(x => x.Name == "Project2");
                CollectionAssert.AreEqual(new[] { project1.Id }, project2.AllProjectReferences.Select(x => x.ProjectId));
            }

            [Test]
            public static void CreateSolutionWithDependenciesFromQualified()
            {
                var code1 = @"
namespace Project1
{
    public class Foo1
    {
        private readonly int _value;
    }
}";

                var code2 = @"
namespace Project2
{
    public class Foo2
    {
        private readonly Project1.Foo1 _value;
    }
}";
                var sln = CodeFactory.CreateSolution(new[] { code1, code2 }, new[] { new FieldNameMustNotBeginWithUnderscore() });
                CollectionAssert.AreEqual(new[] { "Project1", "Project2" }, sln.Projects.Select(x => x.Name));
                CollectionAssert.AreEqual(new[] { "Foo1.cs", "Foo2.cs" }, sln.Projects.Select(x => x.Documents.Single().Name));
                var project1 = sln.Projects.Single(x => x.Name == "Project1");
                CollectionAssert.IsEmpty(project1.AllProjectReferences);
                var project2 = sln.Projects.Single(x => x.Name == "Project2");
                CollectionAssert.AreEqual(new[] { project1.Id }, project2.AllProjectReferences.Select(x => x.ProjectId));
            }

            [Test]
            public static void CreateSolutionWithInheritQualified()
            {
                var code1 = @"
namespace RoslynSandbox.Core
{
    public class Foo1
    {
        private readonly int _value;
    }
}";

                var code2 = @"
namespace RoslynSandbox.Client
{
    public class Foo2 : RoslynSandbox.Core.Foo1
    {
    }
}";
                foreach (var sources in new[] { new[] { code1, code2 }, new[] { code2, code1 } })
                {
                    var sln = CodeFactory.CreateSolution(sources, new[] { new FieldNameMustNotBeginWithUnderscore() });
                    CollectionAssert.AreEquivalent(new[] { "RoslynSandbox.Core", "RoslynSandbox.Client" }, sln.Projects.Select(x => x.Name));
                    CollectionAssert.AreEquivalent(new[] { "Foo1.cs", "Foo2.cs" }, sln.Projects.Select(x => x.Documents.Single().Name));
                    var project1 = sln.Projects.Single(x => x.Name == "RoslynSandbox.Core");
                    CollectionAssert.IsEmpty(project1.AllProjectReferences);
                    var project2 = sln.Projects.Single(x => x.Name == "RoslynSandbox.Client");
                    CollectionAssert.AreEqual(new[] { project1.Id }, project2.AllProjectReferences.Select(x => x.ProjectId));
                }
            }

            [Test]
            public static void CreateSolutionWithOneProject()
            {
                var code1 = @"
namespace RoslynSandbox.Core
{
    public class Foo1
    {
        private readonly int _value;
    }
}";

                var code2 = @"
namespace RoslynSandbox.Bar
{
    public class Foo2 : RoslynSandbox.Core.Foo1
    {
    }
}";
                foreach (var sources in new[] { new[] { code1, code2 }, new[] { code2, code1 } })
                {
                    var sln = CodeFactory.CreateSolutionWithOneProject(sources, new[] { new FieldNameMustNotBeginWithUnderscore() });
                    var project = sln.Projects.Single();
                    Assert.AreEqual("RoslynSandbox", project.AssemblyName);
                    CollectionAssert.AreEquivalent(new[] { "Foo1.cs", "Foo2.cs" }, project.Documents.Select(x => x.Name));
                }
            }

            [Test]
            public static void CreateSolutionWhenNestedNamespaces()
            {
                var resourcesCode = @"
namespace RoslynSandbox.Properties
{
    public class Resources
    {
    }
}";

                var testCode = @"
namespace RoslynSandbox
{
    using RoslynSandbox.Properties;

    public class Foo
    {
    }
}";
                foreach (var sources in new[] { new[] { resourcesCode, testCode }, new[] { resourcesCode, testCode } })
                {
                    var sln = CodeFactory.CreateSolution(sources);
                    var project = sln.Projects.Single();
                    Assert.AreEqual("RoslynSandbox", project.AssemblyName);
                    CollectionAssert.AreEquivalent(new[] { "Resources.cs", "Foo.cs" }, project.Documents.Select(x => x.Name));
                }
            }
        }
    }
}
