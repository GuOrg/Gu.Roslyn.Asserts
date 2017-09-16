// ReSharper disable RedundantNameQualifier
namespace Gu.Roslyn.Asserts.Tests
{
    using System.Linq;
    using System.Threading.Tasks;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using Microsoft.CodeAnalysis.Editing;
    using NUnit.Framework;

    public class CodeAssertTests
    {
        [Test]
        public void WhenEqual()
        {
            var expected = @"
namespace RoslynSandbox
{
    class Foo
    {
        private readonly int _value;
    }
}";

            var actual = @"
namespace RoslynSandbox
{
    class Foo
    {
        private readonly int _value;
    }
}";
            CodeAssert.AreEqual(expected, actual);
        }

        [Test]
        public void WhenNotEqual()
        {
            var expectedCode = @"
namespace RoslynSandbox
{
    class Foo
    {
        private readonly int _value;
    }
}";

            var actualCode = @"
namespace RoslynSandbox
{
    class Foo
    {
        private readonly int bar;
    }
}";
            var exception = Assert.Throws<NUnit.Framework.AssertionException>(() => CodeAssert.AreEqual(expectedCode, actualCode));
            var expected = "Mismatch on line 6 of file Foo.cs\r\n" +
                           "Expected:         private readonly int _value;\r\n" +
                           "Actual:           private readonly int bar;\r\n" +
                           "                                       ^\r\n" +
                           "Expected:\r\n\r\n" +
                           "namespace RoslynSandbox\r\n" +
                           "{\r\n" +
                           "    class Foo\r\n" +
                           "    {\r\n" +
                           "        private readonly int _value;\r\n" +
                           "    }\r\n" +
                           "}\r\n" +
                           "Actual:\r\n\r\n" +
                           "namespace RoslynSandbox\r\n" +
                           "{\r\n" +
                           "    class Foo\r\n" +
                           "    {\r\n" +
                           "        private readonly int bar;\r\n" +
                           "    }\r\n" +
                           "}\r\n";
            Assert.AreEqual(expected, exception.Message);
        }

        [Test]
        public async Task MakeSealed()
        {
            var testCode = @"
namespace RoslynSandbox
{
    public class Foo
    {
    }
}";
            var sln = CodeFactory.CreateSolution(testCode);
            var editor = await DocumentEditor.CreateAsync(sln.Projects.First().Documents.First()).ConfigureAwait(false);
            var type = editor.OriginalRoot.SyntaxTree.FindBestMatch<ClassDeclarationSyntax>("Foo");
            var expected = @"
namespace RoslynSandbox
{
    public sealed class Foo
    {
    }
}";
            editor.SetModifiers(type, DeclarationModifiers.From(editor.SemanticModel.GetDeclaredSymbol(type)).WithIsSealed(isSealed: true));
            CodeAssert.AreEqual(expected, editor.GetChangedDocument());
        }
    }
}
