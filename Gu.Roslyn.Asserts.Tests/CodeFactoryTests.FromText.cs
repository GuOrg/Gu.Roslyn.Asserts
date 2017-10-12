namespace Gu.Roslyn.Asserts.Tests
{
    using System.Linq;
    using NUnit.Framework;

    public partial class CodeFactoryTests
    {
        public class FromText
        {
            [Test]
            public void CreateSolutionFromSource()
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
            public void CreateSolutionFromSources()
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
                var sln = CodeFactory.CreateSolution(new[] { code1, code2 }, new[] { new FieldNameMustNotBeginWithUnderscore() });
                CollectionAssert.AreEqual(new[] { "Project1", "Project2" }, sln.Projects.Select(x => x.Name));
                Assert.AreEqual(new[] { "Foo1.cs", "Foo2.cs" }, sln.Projects.Select(x => x.Documents.Single().Name));
            }

            [Test]
            public void CreateSolutionWithDependenciesFromUsings()
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
            public void CreateSolutionWithDependenciesFromQualified()
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
        private readonly Project1.Foo1 _value;
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
        }
    }
}