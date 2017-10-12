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
        }
    }
}