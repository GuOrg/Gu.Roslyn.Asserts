namespace Gu.Roslyn.Asserts.Tests
{
    using Gu.Roslyn.Asserts.Tests.Refactorings;
    using Microsoft.CodeAnalysis.Text;
    using NUnit.Framework;

    public static partial class RoslynAssertTests
    {
        public static class Refactoring
        {
            [Test]
            public static void WithPositionIndicated()
            {
                var before = @"
class ↓Foo
{
}";

                var after = @"
class FOO
{
}";

                var refactoring = new ClassNameToUpperCaseRefactoringProvider();
                RoslynAssert.Refactoring(refactoring, before, after);
                RoslynAssert.Refactoring(refactoring, before, after, title: "To uppercase");
            }

            [Test]
            public static void WithPositionIndicatedWhenNotExpectedCode()
            {
                var before = @"
class ↓Foo
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
                exception = Assert.Throws<AssertException>(() => RoslynAssert.Refactoring(refactoring, before, after, title: "To uppercase"));
                CodeAssert.AreEqual(expected, exception.Message);
            }

            [Test]
            public static void WithSpan()
            {
                var before = @"
class Foo
{
}";

                var after = @"
class FOO
{
}";

                var refactoring = new ClassNameToUpperCaseRefactoringProvider();
                RoslynAssert.Refactoring(refactoring, before, new TextSpan(8, 3), after);
                RoslynAssert.Refactoring(refactoring, before, new TextSpan(8, 3), after, title: "To uppercase");
            }

            [Test]
            public static void WithSpanWhenNotExpectedCode()
            {
                var before = @"
class Foo
{
}";

                var after = @"
class WRONG
{
}";

                var refactoring = new ClassNameToUpperCaseRefactoringProvider();
                var exception = Assert.Throws<AssertException>(() => RoslynAssert.Refactoring(refactoring, before, new TextSpan(8, 3), after));
                var expected = @"Mismatch on line 2 of file WRONG.cs.
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
                exception = Assert.Throws<AssertException>(() => RoslynAssert.Refactoring(refactoring, before, new TextSpan(8, 3), after, title: "To uppercase"));
                CodeAssert.AreEqual(expected, exception.Message);
            }
        }
    }
}
