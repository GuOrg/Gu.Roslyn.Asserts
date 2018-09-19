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
        public void WhenEqualWhitespaceEnd()
        {
            var expected = @"
namespace RoslynSandbox
{
    class Foo
    {
        private readonly int _value;
    }
}

a
";

            var actual = @"
namespace RoslynSandbox
{
    class Foo
    {
        private readonly int _value;
    }
}

a
";

            CodeAssert.AreEqual(expected, actual);
        }

        [TestCase("\r\nExpected:\r\n\r\nnamespace RoslynSandbox", "\r\nExpected:\r\n\nnamespace RoslynSandbox")]
        public void WhenEqualMixedNewLines(string expected, string actual)
        {
            CodeAssert.AreEqual(expected, actual);
        }

        [TestCase("\\r\\n", "\\r\\n")]
        [TestCase("\\r\\n", "\\n")]
        [TestCase("\\n", "\\n")]
        public void WhenCodeContainsNewLines(string x, string y)
        {
            CodeAssert.AreEqual(x, y);
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

            var actual = @"
namespace RoslynSandbox
{
    class Foo
    {
        private readonly int bar;
    }
}";

            var exception = Assert.Throws<AssertException>(() => CodeAssert.AreEqual(expectedCode, actual));
            var expected = "Mismatch on line 6 of file Foo.cs.\r\n" +
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
            CodeAssert.AreEqual(expected, exception.Message);
        }

        [Test]
        public void WhenNotEqualUnknownFileName()
        {
            var expectedCode = @"        private readonly int _value;";

            var actual = @"        private readonly int bar;";

            var exception = Assert.Throws<AssertException>(() => CodeAssert.AreEqual(expectedCode, actual));
            var expected = "Expected:         private readonly int _value;\r\n" +
                           "Actual:           private readonly int bar;\r\n" +
                           "                                       ^\r\n";
            CodeAssert.AreEqual(expected, exception.Message);
        }

        [Test]
        public void WhenManyLines()
        {
            var expectedCode = "line1\r\n" +
                               "line2";

            var actual = "line1WRONG\r\n" +
                         "line2";

            var exception = Assert.Throws<AssertException>(() => CodeAssert.AreEqual(expectedCode, actual));
            var expected = "Mismatch on line 1.\r\n" +
                           "Expected: line1\r\n" +
                           "Actual:   line1WRONG\r\n" +
                           "               ^\r\n" +
                           "Expected:\r\n" +
                           "line1\r\n" +
                           "line2\r\n" +
                           "Actual:\r\n" +
                           "line1WRONG\r\n" +
                           "line2\r\n";
            CodeAssert.AreEqual(expected, exception.Message);
        }

        [Test]
        public void WhenNotEqualEndWhenExpectedHasNewLine()
        {
            var exception = Assert.Throws<AssertException>(() => CodeAssert.AreEqual("a\r\n", "a"));
            var expected = "Mismatch at end.\r\n" +
                           "Expected: a\\r\\n\r\n" +
                           "Actual:   a\r\n" +
                           "           ^\r\n";
            CodeAssert.AreEqual(expected, exception.Message);
        }

        [Test]
        public void WhenNotEqualEndWhenActualHasNewLine()
        {
            var exception = Assert.Throws<AssertException>(() => CodeAssert.AreEqual("a", "a\r\n"));
            var expected = "Mismatch at end.\r\n" +
                           "Expected: a\r\n" +
                           "Actual:   a\\r\\n\r\n" +
                           "           ^\r\n";
            CodeAssert.AreEqual(expected, exception.Message);
        }

        [Test]
        public void WhenNotEqualEnd()
        {
            var exception = Assert.Throws<AssertException>(() => CodeAssert.AreEqual("\r\na\r\n", "\r\na\r\n\r\n"));
            var expected = "Mismatch at end.\r\n" +
                           "Expected: a\\r\\n\r\n" +
                           "Actual:   a\\r\\n\\r\\n\r\n" +
                           "               ^\r\n" +
                           "Expected:\r\n" +
                           "\r\na\r\n\r\n" +
                           "Actual:\r\n" +
                           "\r\na\r\n\r\n\r\n";
            CodeAssert.AreEqual(expected, exception.Message);
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
            var type = editor.OriginalRoot.SyntaxTree.Find<ClassDeclarationSyntax>("Foo");
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
