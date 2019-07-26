namespace Gu.Roslyn.Asserts.Tests
{
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using NUnit.Framework;

    public static class SyntaxNodeExtTests
    {
        [Test]
        public static void FindClassDeclaration()
        {
            var syntaxTree = CSharpSyntaxTree.ParseText(
                @"
namespace N
{
    internal class C { }
}");
            var node = syntaxTree.FindClassDeclaration("C");
            Assert.AreEqual("internal class C { }", node.ToString());

            node = syntaxTree.Find<ClassDeclarationSyntax>("C");
            Assert.AreEqual("internal class C { }", node.ToString());
        }

        [Test]
        public static void FindTypeDeclaration()
        {
            var syntaxTree = CSharpSyntaxTree.ParseText(
                @"
namespace N
{
    internal class C { }
}");
            var node = syntaxTree.FindTypeDeclaration("C");
            Assert.AreEqual("internal class C { }", node.ToString());

            node = syntaxTree.Find<TypeDeclarationSyntax>("C");
            Assert.AreEqual("internal class C { }", node.ToString());
        }

        [TestCase("var temp = 1;", "var temp = 1;")]
        [TestCase("temp = 2;", "temp = 2;")]
        public static void FindStatement(string statement, string expected)
        {
            var syntaxTree = CSharpSyntaxTree.ParseText(
                @"
namespace N
{
    internal class C
    {
        internal C()
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
        public static void FindEqualsValueClause(string text, string expected)
        {
            var syntaxTree = CSharpSyntaxTree.ParseText(
                @"
namespace N
{
    internal class C
    {
        internal C()
        {
            int temp = 1;
            temp = 2;
        }

        internal C(int i)
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
        public static void FindAssignmentExpression(string text, string expected)
        {
            var syntaxTree = CSharpSyntaxTree.ParseText(
                @"
namespace N
{
    internal class C
    {
        internal C()
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
        public static void FindAssignmentExpressionDemo()
        {
            var syntaxTree = CSharpSyntaxTree.ParseText(
                @"
namespace N
{
    internal class C
    {
        internal C()
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
        public static void FindConstructorDeclarationSyntax()
        {
            var syntaxTree = CSharpSyntaxTree.ParseText(
                @"
namespace N
{
    internal class C
    {
        internal C()
        {
        }
    }
}");
            var expected = "internal C()\r\n        {\r\n        }";
            var node = syntaxTree.FindConstructorDeclaration("internal C()");
            CodeAssert.AreEqual(expected, node.ToString());

            node = syntaxTree.Find<ConstructorDeclarationSyntax>("internal C()");
            CodeAssert.AreEqual(expected, node.ToString());
        }

        [Test]
        public static void FindMethodDeclaration()
        {
            var syntaxTree = CSharpSyntaxTree.ParseText(
                @"
namespace N
{
    internal class C
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
        public static void FindFieldDeclaration()
        {
            var syntaxTree = CSharpSyntaxTree.ParseText(
                @"
namespace N
{
    internal class C
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
        public static void FindPropertyDeclaration()
        {
            var syntaxTree = CSharpSyntaxTree.ParseText(
                @"
namespace N
{
    internal class C
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
        public static void FindInvocation()
        {
            var syntaxTree = CSharpSyntaxTree.ParseText(
                @"
namespace N
{
    using System;

    internal class C
    {
        public C()
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
        public static void FindInvocationWhenArgumentIsInvocation(string invocation)
        {
            var testCode = @"
namespace N
{
    public class C
    {
        public C()
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
        public static void FindOnPropertyChangedInvocation(string call)
        {
            var syntaxTree = CSharpSyntaxTree.ParseText(
                @"
namespace N
{
    using System;
    using System.ComponentModel;
    using System.Linq.Expressions;
    using System.Runtime.CompilerServices;

    public class C : INotifyPropertyChanged
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
        public static void FindArgument()
        {
            var syntaxTree = CSharpSyntaxTree.ParseText(
                @"
namespace N
{
    using System;

    internal class C
    {
        public C()
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

        [TestCase("null")]
        [TestCase("1")]
        [TestCase("\"abc\"")]
        public static void FindLiteralExpression(string text)
        {
            var syntaxTree = CSharpSyntaxTree.ParseText(
                @"
namespace N
{
    using System;

    internal class C
    {
        public C()
        {
            Console.WriteLine(null);
        }
    }
}".AssertReplace("null", text));
            var node = syntaxTree.FindLiteralExpression(text);
            Assert.AreEqual(text, node.ToString());

            node = syntaxTree.Find<LiteralExpressionSyntax>(text);
            Assert.AreEqual(text, node.ToString());
        }

        [TestCase("Console")]
        [TestCase("WriteLine")]
        public static void FindIdentifierName(string text)
        {
            var syntaxTree = CSharpSyntaxTree.ParseText(
                @"
namespace N
{
    using System;

    internal class C
    {
        public C()
        {
            Console.WriteLine(null);
        }
    }
}".AssertReplace("null", text));
            var node = syntaxTree.FindIdentifierName(text);
            Assert.AreEqual(text, node.ToString());

            node = syntaxTree.Find<IdentifierNameSyntax>(text);
            Assert.AreEqual(text, node.ToString());
        }

        [TestCase("int i")]
        [TestCase("int j")]
        public static void FindParameter(string parameter)
        {
            var syntaxTree = CSharpSyntaxTree.ParseText(
                @"
namespace N
{
    using System;

    internal class C
    {
        public C(int i, int j)
        {
        }
    }
}");
            var node = syntaxTree.FindParameter(parameter);
            Assert.AreEqual(parameter, node.ToString());

            node = syntaxTree.Find<ParameterSyntax>(parameter);
            Assert.AreEqual(parameter, node.ToString());
        }

        [TestCase("get => this.value;")]
        [TestCase("set => this.value = value;")]
        public static void FindAccessorDeclaration(string accessor)
        {
            var testCode = @"
namespace N
{
    public class C
    {
        private int value;

        public int Value
        {
            get => this.value;
            set => this.value = value;
        }
    }
}";
            var syntaxTree = CSharpSyntaxTree.ParseText(testCode);
            Assert.AreEqual(accessor, syntaxTree.FindAccessorDeclaration(accessor).ToString());
            Assert.AreEqual(accessor, syntaxTree.Find<AccessorDeclarationSyntax>(accessor).ToString());
        }

        [Test]
        public static void FindExpression()
        {
            var testCode = @"
namespace N
{
    public class C
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

        [TestCase("C.Get<IComparable>(1)")]
        [TestCase("C.Get<System.IComparable>(1)")]
        [TestCase("C.Get<int>(1)")]
        [TestCase("this.C.Get<int>(1)")]
        [TestCase("this.C.Inner.Get<int>(1)")]
        [TestCase("this.C.Inner.C.Get<int>(1)")]
        [TestCase("this.C?.Get<int>(1)")]
        [TestCase("this.C?.C.Get<int>(1)")]
        [TestCase("this.Inner?.Inner.Get<int>(1)")]
        [TestCase("this.Inner?.C.Get<int>(1)")]
        [TestCase("this.Inner?.C?.Get<int>(1)")]
        [TestCase("this.Inner.C?.Get<int>(1)")]
        [TestCase("this.Inner?.C?.Inner?.Get<int>(1)")]
        [TestCase("((C)meh).Get<int>(1)")]
        [TestCase("((C)this.meh).Get<int>(1)")]
        [TestCase("((C)this.Inner.meh).Get<int>(1)")]
        [TestCase("(meh as C).Get<int>(1)")]
        [TestCase("(this.meh as C).Get<int>(1)")]
        [TestCase("(this.Inner.meh as C).Get<int>(1)")]
        [TestCase("(this.Inner.meh as C)?.Get<int>(1)")]
        [TestCase("(meh as C)?.Get<int>(1)")]
        public static void FindExpressionComplicated(string code)
        {
            var testCode = @"
namespace N
{
    using System;

    public sealed class C : IDisposable
    {
        private readonly object meh;
        private readonly C C;

        public C Inner => this.C;

        public void Dispose()
        {
            var temp = this.C.Get<int>(1);
        }

        private T Get<T>(int value) => default(T);
    }
}";
            testCode = testCode.AssertReplace("this.C.Get<int>(1)", code);
            var syntaxTree = CSharpSyntaxTree.ParseText(testCode);
            var expression = syntaxTree.FindExpression(code);
            Assert.AreEqual(code, expression.ToString());
        }

        [Test]
        public static void FindBinaryExpression()
        {
            var testCode = @"
namespace N
{
    public class C
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
        public static void FindAttribute()
        {
            var testCode = @"
namespace N
{
    using System;

    public class C
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
