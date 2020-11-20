namespace Gu.Roslyn.Asserts.Tests.RoslynAssertTests
{
    using Gu.Roslyn.Asserts.Tests.Refactorings;
    using Microsoft.CodeAnalysis.Text;
    using NUnit.Framework;

    public static partial class Refactoring
    {
        public static class Fail
        {
            [Test]
            public static void WithPositionIndicatedWhenAfterDoesNotMatch()
            {
                var before = @"
class ↓c
{
}";

                var after = @"
class WRONG
{
}";

                var refactoring = new ClassNameToUpperCaseRefactoringProvider();
                var exception = Assert.Throws<AssertException>(() => RoslynAssert.Refactoring(refactoring, before, after));
                var expected = @"Mismatch on line 2 of file WRONG.cs.
Expected: class WRONG
Actual:   class C
                ^
Expected:

class WRONG
{
}
Actual:

class C
{
}
";
                CodeAssert.AreEqual(expected, exception.Message);
                exception = Assert.Throws<AssertException>(() => RoslynAssert.Refactoring(refactoring, before, after, title: "To uppercase"));
                CodeAssert.AreEqual(expected, exception.Message);
            }

            [Test]
            public static void WithSpanWhenAfterDoesNotMatch()
            {
#pragma warning disable GURA02 // Indicate position.
                var before = @"
class c
{
}";
#pragma warning restore GURA02 // Indicate position.

                var after = @"
class WRONG
{
}";

                var refactoring = new ClassNameToUpperCaseRefactoringProvider();
                var exception = Assert.Throws<AssertException>(() => RoslynAssert.Refactoring(refactoring, before, new TextSpan(8, 3), after));
                var expected = @"Mismatch on line 2 of file WRONG.cs.
Expected: class WRONG
Actual:   class C
                ^
Expected:

class WRONG
{
}
Actual:

class C
{
}
";
                CodeAssert.AreEqual(expected, exception.Message);
                exception = Assert.Throws<AssertException>(() => RoslynAssert.Refactoring(refactoring, before, new TextSpan(8, 3), after, title: "To uppercase"));
                CodeAssert.AreEqual(expected, exception.Message);
            }
        }
    }
}
