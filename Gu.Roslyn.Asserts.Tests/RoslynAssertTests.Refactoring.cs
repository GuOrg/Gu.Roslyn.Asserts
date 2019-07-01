namespace Gu.Roslyn.Asserts.Tests
{
    using Gu.Roslyn.Asserts.Tests.Refactorings;
    using Microsoft.CodeAnalysis.Text;
    using NUnit.Framework;

    public static partial class RoslynAssertTests
    {
        public static class Refactoring
        {
            private static string expected;

            [Test]
            public static void WithPositionIndicated()
            {
                var testCode = @"
class ↓Foo
{
}";

                var fixedCode = @"
class FOO
{
}";

                var refactoring = new ClassNameToUpperCaseRefactoringProvider();
                RoslynAssert.Refactoring(refactoring, testCode, fixedCode);
                RoslynAssert.Refactoring(refactoring, testCode, fixedCode, title: "To uppercase");
            }

            [Test]
            public static void WithPositionIndicatedWhenNotExpectedCode()
            {
                var testCode = @"
class ↓Foo
{
}";

                var fixedCode = @"
class WRONG
{
}";

                var refactoring = new ClassNameToUpperCaseRefactoringProvider();
                var exception = Assert.Throws<AssertException>(() => RoslynAssert.Refactoring(refactoring, testCode, fixedCode));
                expected = @"Mismatch on line 2 of file WRONG.cs.
Expected: class WRONG
Actual:   class FOO
                ^
Expected:

class WRONG
{
}
Actual:

class FOO
{
}
";
                CodeAssert.AreEqual(expected, exception.Message);
                exception = Assert.Throws<AssertException>(() => RoslynAssert.Refactoring(refactoring, testCode, fixedCode, title: "To uppercase"));
                CodeAssert.AreEqual(expected, exception.Message);
            }

            [Test]
            public static void WithSpan()
            {
                var testCode = @"
class Foo
{
}";

                var fixedCode = @"
class FOO
{
}";

                var refactoring = new ClassNameToUpperCaseRefactoringProvider();
                RoslynAssert.Refactoring(refactoring, testCode, new TextSpan(8, 3), fixedCode);
                RoslynAssert.Refactoring(refactoring, testCode, new TextSpan(8, 3), fixedCode, title: "To uppercase");
            }

            [Test]
            public static void WithSpanWhenNotExpectedCode()
            {
                var testCode = @"
class ↓Foo
{
}";

                var fixedCode = @"
class WRONG
{
}";

                var refactoring = new ClassNameToUpperCaseRefactoringProvider();
                var exception = Assert.Throws<AssertException>(() => RoslynAssert.Refactoring(refactoring, testCode, new TextSpan(8, 3), fixedCode));
                expected = @"Mismatch on line 2 of file WRONG.cs.
Expected: class WRONG
Actual:   class FOO
                ^
Expected:

class WRONG
{
}
Actual:

class FOO
{
}
";
                CodeAssert.AreEqual(expected, exception.Message);
                exception = Assert.Throws<AssertException>(() => RoslynAssert.Refactoring(refactoring, testCode, new TextSpan(8, 3), fixedCode, title: "To uppercase"));
                CodeAssert.AreEqual(expected, exception.Message);
            }
        }
    }
}
