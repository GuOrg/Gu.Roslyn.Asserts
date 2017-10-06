namespace Gu.Roslyn.Asserts.Tests
{
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using NUnit.Framework;

    internal class SyntaxNodeExtTests
    {
        [TestCase("var temp = 1;", "var temp = 1;")]
        [TestCase("var temp = 1;", "var temp = 1;")]
        [TestCase("temp = 2;", "temp = 2;")]
        public void FindStatement(string text, string expected)
        {
            var syntaxTree = CSharpSyntaxTree.ParseText(
                @"
namespace RoslynSandbox
{
    internal class Foo
    {
        internal Foo()
        {
            var temp = 1;
            temp = 2;
        }
    }
}");
            var node = syntaxTree.FindStatement(text);
            Assert.AreEqual(expected, node.ToString());

            node = syntaxTree.FindBestMatch<StatementSyntax>(text);
            Assert.AreEqual(expected, node.ToString());
        }

        [TestCase("temp = 1;", "= 1")]
        public void FindEqualsValueClause(string text, string expected)
        {
            var syntaxTree = CSharpSyntaxTree.ParseText(
                @"
namespace RoslynSandbox
{
    internal class Foo
    {
        internal Foo()
        {
            var temp = 1;
            temp = 2;
        }
    }
}");
            var node = syntaxTree.FindEqualsValueClause(text);
            Assert.AreEqual(expected, node.ToString());

            node = syntaxTree.FindBestMatch<EqualsValueClauseSyntax>(text);
            Assert.AreEqual(expected, node.ToString());
        }

        [TestCase("temp = 2;", "temp = 2")]
        public void FindAssignmentExpression(string text, string expected)
        {
            var syntaxTree = CSharpSyntaxTree.ParseText(
                @"
namespace RoslynSandbox
{
    internal class Foo
    {
        internal Foo()
        {
            var temp = 1;
            temp = 2;
        }
    }
}");
            var node = syntaxTree.FindAssignmentExpression(text);
            Assert.AreEqual(expected, node.ToString());

            node = syntaxTree.FindBestMatch<AssignmentExpressionSyntax>(text);
            Assert.AreEqual(expected, node.ToString());
        }

        [Test]
        public void FindAssignmentExpressionDemo()
        {
            var syntaxTree = CSharpSyntaxTree.ParseText(
                @"
namespace RoslynSandbox
{
    internal class Foo
    {
        internal Foo()
        {
            var temp = 1;
            temp = 2;
        }
    }
}");
            var compilation = CSharpCompilation.Create("test", new[] { syntaxTree }, new[] { MetadataReference.CreateFromFile(typeof(object).Assembly.Location), });
            var semanticModel = compilation.GetSemanticModel(syntaxTree);
            var assignment = syntaxTree.FindAssignmentExpression("temp = 2");
            Assert.AreEqual("temp = 2", assignment.ToString());
            Assert.AreEqual("int", semanticModel.GetTypeInfo(assignment.Right).Type.ToDisplayString());
        }

        [Test]
        public void FindConstructorDeclarationSyntax()
        {
            var syntaxTree = CSharpSyntaxTree.ParseText(
                @"
namespace RoslynSandbox
{
    internal class Foo
    {
        internal Foo()
        {
        }
    }
}");
            var expected = "internal Foo()\r\n        {\r\n        }";
            var node = syntaxTree.FindConstructorDeclarationSyntax("internal Foo()");
            Assert.AreEqual(expected, node.ToString());

            node = syntaxTree.FindBestMatch<ConstructorDeclarationSyntax>("internal Foo()");
            Assert.AreEqual(expected, node.ToString());
        }

        [Test]
        public void FindMethodDeclaration()
        {
            var syntaxTree = CSharpSyntaxTree.ParseText(
                @"
namespace RoslynSandbox
{
    internal class Foo
    {
        internal void Bar()
        {
        }
    }
}");
            var expected = "internal void Bar()\r\n        {\r\n        }";
            var node = syntaxTree.FindMethodDeclaration("internal void Bar()");
            Assert.AreEqual(expected, node.ToString());

            node = syntaxTree.FindBestMatch<MethodDeclarationSyntax>("internal void Bar()");
            Assert.AreEqual(expected, node.ToString());
        }

        [Test]
        public void FindFieldDeclaration()
        {
            var syntaxTree = CSharpSyntaxTree.ParseText(
                @"
namespace RoslynSandbox
{
    internal class Foo
    {
        internal readonly int bar;
    }
}");
            var expected = "internal readonly int bar;";
            var node = syntaxTree.FindFieldDeclaration("bar");
            Assert.AreEqual(expected, node.ToString());

            node = syntaxTree.FindBestMatch<FieldDeclarationSyntax>("bar");
            Assert.AreEqual(expected, node.ToString());
        }

        [Test]
        public void FindPropertyDeclaration()
        {
            var syntaxTree = CSharpSyntaxTree.ParseText(
                @"
namespace RoslynSandbox
{
    internal class Foo
    {
        public int Bar { get; set; }
    }
}");
            var expected = "public int Bar { get; set; }";
            var node = syntaxTree.FindPropertyDeclaration("Bar");
            Assert.AreEqual(expected, node.ToString());

            node = syntaxTree.FindBestMatch<PropertyDeclarationSyntax>("Bar");
            Assert.AreEqual(expected, node.ToString());
        }
    }
}
