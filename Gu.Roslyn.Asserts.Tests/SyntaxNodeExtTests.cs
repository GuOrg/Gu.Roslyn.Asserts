namespace Gu.Roslyn.Asserts.Tests
{
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using NUnit.Framework;

    internal class SyntaxNodeExtTests
    {
        [Test]
        public void FindClassDeclaration()
        {
            var syntaxTree = CSharpSyntaxTree.ParseText(
                @"
namespace RoslynSandbox
{
    internal class Foo { }
}");
            var node = syntaxTree.FindClassDeclaration("Foo");
            Assert.AreEqual("internal class Foo { }", node.ToString());

            node = syntaxTree.Find<ClassDeclarationSyntax>("Foo");
            Assert.AreEqual("internal class Foo { }", node.ToString());
        }

        [Test]
        public void FindTypeDeclaration()
        {
            var syntaxTree = CSharpSyntaxTree.ParseText(
                @"
namespace RoslynSandbox
{
    internal class Foo { }
}");
            var node = syntaxTree.FindTypeDeclaration("Foo");
            Assert.AreEqual("internal class Foo { }", node.ToString());

            node = syntaxTree.Find<TypeDeclarationSyntax>("Foo");
            Assert.AreEqual("internal class Foo { }", node.ToString());
        }

        [TestCase("var temp = 1;", "var temp = 1;")]
        [TestCase("temp = 2;", "temp = 2;")]
        public void FindStatement(string statement, string expected)
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
            var node = syntaxTree.FindStatement(statement);
            Assert.AreEqual(expected, node.ToString());

            node = syntaxTree.Find<StatementSyntax>(statement);
            Assert.AreEqual(expected, node.ToString());
        }

        [TestCase("temp = 1;", "= 1")]
        [TestCase("var temp = 1;", "= 1")]
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
            int temp = 1;
            temp = 2;
        }

        internal Foo(int i)
        {
            var temp = 1;
            temp = 2;
        }
    }
}");
            var node = syntaxTree.FindEqualsValueClause(text);
            Assert.AreEqual(expected, node.ToString());

            node = syntaxTree.Find<EqualsValueClauseSyntax>(text);
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

            node = syntaxTree.Find<AssignmentExpressionSyntax>(text);
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
            var node = syntaxTree.FindConstructorDeclaration("internal Foo()");
            CodeAssert.AreEqual(expected, node.ToString());

            node = syntaxTree.Find<ConstructorDeclarationSyntax>("internal Foo()");
            CodeAssert.AreEqual(expected, node.ToString());
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
            CodeAssert.AreEqual(expected, node.ToString());

            node = syntaxTree.Find<MethodDeclarationSyntax>("internal void Bar()");
            CodeAssert.AreEqual(expected, node.ToString());
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

            node = syntaxTree.Find<FieldDeclarationSyntax>("bar");
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

            node = syntaxTree.Find<PropertyDeclarationSyntax>("Bar");
            Assert.AreEqual(expected, node.ToString());
        }

        [Test]
        public void FindInvocation()
        {
            var syntaxTree = CSharpSyntaxTree.ParseText(
                @"
namespace RoslynSandbox
{
    using System;

    internal class Foo
    {
        public Foo()
        {
            Console.WriteLine();
        }
    }
}");
            var node = syntaxTree.FindInvocation("WriteLine");
            Assert.AreEqual("Console.WriteLine()", node.ToString());

            node = syntaxTree.Find<InvocationExpressionSyntax>("WriteLine");
            Assert.AreEqual("Console.WriteLine()", node.ToString());
        }

        [TestCase("Id(\"abc\")")]
        [TestCase("Id(nameof(Id))")]
        [TestCase("this.Id(nameof(Id))")]
        public void FindInvocationWhenArgumentIsInvocation(string invocation)
        {
            var testCode = @"
namespace RoslynSandbox
{
    public class Foo
    {
        public Foo()
        {
            var name = Id(nameof(Id));
        }

        private string Id(string name) => name;
    }
}";

            testCode = testCode.AssertReplace("Id(nameof(Id))", invocation);
            var syntaxTree = CSharpSyntaxTree.ParseText(testCode);
            Assert.AreEqual(invocation, syntaxTree.FindInvocation(invocation).ToString());
            Assert.AreEqual(invocation, syntaxTree.Find<InvocationExpressionSyntax>(invocation).ToString());
        }

        [TestCase("this.OnPropertyChanged()")]
        [TestCase("this.OnPropertyChanged(\"Bar\")")]
        [TestCase("this.OnPropertyChanged(nameof(Bar))")]
        [TestCase("this.OnPropertyChanged(nameof(this.Bar))")]
        [TestCase("this.OnPropertyChanged(() => Bar)")]
        [TestCase("this.OnPropertyChanged(() => this.Bar)")]
        [TestCase("this.OnPropertyChanged(new PropertyChangedEventArgs(\"Bar\"))")]
        [TestCase("this.OnPropertyChanged(new PropertyChangedEventArgs(nameof(Bar)))")]
        [TestCase("this.OnPropertyChanged(new PropertyChangedEventArgs(nameof(this.Bar)))")]
        [TestCase("this.OnPropertyChanged(Cached)")]
        [TestCase("this.OnPropertyChanged(args)")]
        public void FindOnPropertyChangedInvocation(string call)
        {
            var syntaxTree = CSharpSyntaxTree.ParseText(
                @"
namespace RoslynSandbox
{
    using System;
    using System.ComponentModel;
    using System.Linq.Expressions;
    using System.Runtime.CompilerServices;

    public class Foo : INotifyPropertyChanged
    {
        private static readonly PropertyChangedEventArgs Cached = new PropertyChangedEventArgs(""Bar"");

        private int bar;

        public event PropertyChangedEventHandler PropertyChanged;

        public int Bar
        {
            get
            {
                return this.bar;
            }

            set
            {
                if (value == this.bar)
                {
                    return;
                }

                this.bar = value;
                this.OnPropertyChanged();
                this.OnPropertyChanged(""Bar"");
                this.OnPropertyChanged(nameof(Bar));
                this.OnPropertyChanged(nameof(this.Bar));
                this.OnPropertyChanged(() => Bar);
                this.OnPropertyChanged(() => this.Bar);
                this.OnPropertyChanged(new PropertyChangedEventArgs(""Bar""));
                this.OnPropertyChanged(new PropertyChangedEventArgs(nameof(Bar)));
                this.OnPropertyChanged(new PropertyChangedEventArgs(nameof(this.Bar)));
                this.OnPropertyChanged(Cached);
                var args = new PropertyChangedEventArgs(""Bar"");
                this.OnPropertyChanged(args);
            }
        }

        protected virtual void OnPropertyChanged<T>(Expression<Func<T>> property)
        {
            this.OnPropertyChanged(((MemberExpression)property.Body).Member.Name);
        }

        protected virtual void OnPropertyChanged(PropertyChangedEventArgs e)
        {
            this.PropertyChanged?.Invoke(this, e);
        }

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}");
            Assert.AreEqual(call, syntaxTree.FindInvocation(call).ToString());
            Assert.AreEqual(call, syntaxTree.Find<InvocationExpressionSyntax>(call).ToString());
        }

        [Test]
        public void FindArgument()
        {
            var syntaxTree = CSharpSyntaxTree.ParseText(
                @"
namespace RoslynSandbox
{
    using System;

    internal class Foo
    {
        public Foo()
        {
            Console.WriteLine(string.Empty);
        }
    }
}");
            var node = syntaxTree.FindArgument("string.Empty");
            Assert.AreEqual("string.Empty", node.ToString());

            node = syntaxTree.Find<ArgumentSyntax>("string.Empty");
            Assert.AreEqual("string.Empty", node.ToString());
        }

        [TestCase("int i")]
        [TestCase("int j")]
        public void FindParameter(string parameter)
        {
            var syntaxTree = CSharpSyntaxTree.ParseText(
                @"
namespace RoslynSandbox
{
    using System;

    internal class Foo
    {
        public Foo(int i, int j)
        {
        }
    }
}");
            var node = syntaxTree.FindParameter(parameter);
            Assert.AreEqual(parameter, node.ToString());

            node = syntaxTree.Find<ParameterSyntax>(parameter);
            Assert.AreEqual(parameter, node.ToString());
        }

        [TestCase("get { return this.value; }")]
        [TestCase("set { this.value = value; }")]
        public void FindAccessorDeclaration(string accessor)
        {
            var testCode = @"
namespace RoslynSandbox
{
    public class Foo
    {
        private int value;

        public int Value
        {
            get { return this.value; }
            set { this.value = value; }
        }
    }
}";
            var syntaxTree = CSharpSyntaxTree.ParseText(testCode);
            Assert.AreEqual(accessor, syntaxTree.FindAccessorDeclaration(accessor).ToString());
            Assert.AreEqual(accessor, syntaxTree.Find<AccessorDeclarationSyntax>(accessor).ToString());
        }

        [Test]
        public void FindExpression()
        {
            var testCode = @"
namespace RoslynSandbox
{
    public class Foo
    {
        private int value;

        public int Value => this.value;
    }
}";
            var syntaxTree = CSharpSyntaxTree.ParseText(testCode);
            var expression = "this.value";
            Assert.AreEqual(expression, syntaxTree.FindExpression(expression).ToString());
            Assert.AreEqual(expression, syntaxTree.Find<ExpressionSyntax>(expression).ToString());
        }

        [Test]
        public void FindBinaryExpression()
        {
            var testCode = @"
namespace RoslynSandbox
{
    public class Foo
    {
        private int value;

        public bool Bar => this.value == 1;
    }
}";
            var syntaxTree = CSharpSyntaxTree.ParseText(testCode);
            var expression = "this.value == 1";
            Assert.AreEqual(expression, syntaxTree.FindBinaryExpression(expression).ToString());
            Assert.AreEqual(expression, syntaxTree.Find<BinaryExpressionSyntax>(expression).ToString());
        }

        [Test]
        public void FindAttribute()
        {
            var testCode = @"
namespace RoslynSandbox
{
    using System;

    public class Foo
    {
        [Obsolete]
        public int Value { get; } = 1;
    }
}";
            var syntaxTree = CSharpSyntaxTree.ParseText(testCode);
            var attribute = "Obsolete";
            Assert.AreEqual(attribute, syntaxTree.FindAttribute(attribute).ToString());
            Assert.AreEqual(attribute, syntaxTree.Find<AttributeSyntax>(attribute).ToString());
        }
    }
}
