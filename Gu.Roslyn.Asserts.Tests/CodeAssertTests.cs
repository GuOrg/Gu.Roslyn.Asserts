// ReSharper disable RedundantNameQualifier
namespace Gu.Roslyn.Asserts.Tests;

using System.Linq;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Editing;
using NUnit.Framework;

public static class CodeAssertTests
{
    [Test]
    public static void WhenEqual()
    {
        var expected = @"
namespace N
{
    class C
    {
        private readonly int _value;
    }
}";

        var actual = @"
namespace N
{
    class C
    {
        private readonly int _value;
    }
}";

        CodeAssert.AreEqual(expected, actual);
    }

    [Test]
    public static void WhenEqualWhitespaceEnd()
    {
        var expected = @"
namespace N
{
    class C
    {
        private readonly int _value;
    }
}

a
";

        var actual = @"
namespace N
{
    class C
    {
        private readonly int _value;
    }
}

a
";

        CodeAssert.AreEqual(expected, actual);
    }

    [TestCase("\r\nExpected:\r\n\r\nnamespace N", "\r\nExpected:\r\n\nnamespace N")]
    public static void WhenEqualMixedNewLines(string expected, string actual)
    {
        CodeAssert.AreEqual(expected, actual);
    }

    [TestCase("\\r\\n", "\\r\\n")]
    [TestCase("\\r\\n", "\\n")]
    [TestCase("\\n", "\\n")]
    public static void WhenCodeContainsNewLines(string x, string y)
    {
        CodeAssert.AreEqual(x, y);
    }

    [Test]
    public static void WhenNotEqual()
    {
        var expectedCode = @"
namespace N
{
    class C
    {
        private readonly int _value;
    }
}";

        var actual = @"
namespace N
{
    class C
    {
        private readonly int bar;
    }
}";

        var exception = Assert.Throws<AssertException>(() => CodeAssert.AreEqual(expectedCode, actual));
        var expected = "Mismatch on line 6 of file C.cs.\r\n" +
                       "Expected:         private readonly int _value;\r\n" +
                       "Actual:           private readonly int bar;\r\n" +
                       "                                       ^\r\n" +
                       "Expected:\r\n\r\n" +
                       "namespace N\r\n" +
                       "{\r\n" +
                       "    class C\r\n" +
                       "    {\r\n" +
                       "        private readonly int _value;\r\n" +
                       "    }\r\n" +
                       "}\r\n" +
                       "Actual:\r\n\r\n" +
                       "namespace N\r\n" +
                       "{\r\n" +
                       "    class C\r\n" +
                       "    {\r\n" +
                       "        private readonly int bar;\r\n" +
                       "    }\r\n" +
                       "}\r\n";
        CodeAssert.AreEqual(expected, exception.Message);
    }

    [Test]
    public static void WhenNotEqualUnknownFileName()
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
    public static void WhenManyLines()
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
    public static void WhenNotEqualEndWhenExpectedHasNewLine()
    {
        var exception = Assert.Throws<AssertException>(() => CodeAssert.AreEqual("a\r\n", "a"));
        var expected = "Mismatch at end.\r\n" +
                       "Expected: a\\r\\n\r\n" +
                       "Actual:   a\r\n" +
                       "           ^\r\n";
        CodeAssert.AreEqual(expected, exception.Message);
    }

    [Test]
    public static void WhenNotEqualEndWhenActualHasNewLine()
    {
        var exception = Assert.Throws<AssertException>(() => CodeAssert.AreEqual("a", "a\r\n"));
        var expected = "Mismatch at end.\r\n" +
                       "Expected: a\r\n" +
                       "Actual:   a\\r\\n\r\n" +
                       "           ^\r\n";
        CodeAssert.AreEqual(expected, exception.Message);
    }

    [Test]
    public static void WhenNotEqualEnd()
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
    public static void WhenNotEqualEndMissingNewLine()
    {
        var exception = Assert.Throws<AssertException>(() => CodeAssert.AreEqual("class C {}\n", "class C {}"));
        var expected = "Mismatch at end of file C.cs.\n" +
                       "Expected: class C {}\\n\n" +
                       "Actual:   class C {}\n" +
                       "                    ^\n";
        CodeAssert.AreEqual(expected, exception.Message);
    }

    [Test]
    public static async Task MakeSealed()
    {
        var testCode = @"
namespace N
{
    public class C
    {
    }
}";
        var sln = CodeFactory.CreateSolution(testCode);
        var editor = await DocumentEditor.CreateAsync(sln.Projects.First().Documents.First()).ConfigureAwait(false);
        var type = editor.OriginalRoot.SyntaxTree.Find<ClassDeclarationSyntax>("C");
        var expected = @"
namespace N
{
    public sealed class C
    {
    }
}";
        editor.SetModifiers(type, DeclarationModifiers.From(editor.SemanticModel.GetDeclaredSymbol(type)).WithIsSealed(isSealed: true));
        CodeAssert.AreEqual(expected, editor.GetChangedDocument());
    }
}
