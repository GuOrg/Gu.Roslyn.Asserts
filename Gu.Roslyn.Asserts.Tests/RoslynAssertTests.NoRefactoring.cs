namespace Gu.Roslyn.Asserts.Tests
{
    using Gu.Roslyn.Asserts.Tests.Refactorings;
    using Microsoft.CodeAnalysis.Text;
    using NUnit.Framework;

    public static partial class RoslynAssertTests
    {
        public static class NoRefactoring
        {
            [Test]
            public static void WhenNoActionWithPositionIndicated()
            {
                var testCode = @"
class ↓FOO
{
}";

                var refactoring = new ClassNameToUpperCaseRefactoringProvider();
                RoslynAssert.NoRefactoring(refactoring, testCode);
                RoslynAssert.NoRefactoring(refactoring, testCode, title: "To uppercase");
            }

            [Test]
            public static void WhenNoActionWithSPan()
            {
                var testCode = @"
class FOO
{
}";

                var refactoring = new ClassNameToUpperCaseRefactoringProvider();
                RoslynAssert.NoRefactoring(refactoring, testCode, new TextSpan(8, 3));
                RoslynAssert.NoRefactoring(refactoring, testCode, new TextSpan(8, 3), title: "To uppercase");
            }

            [Test]
            public static void WhenActionIsRegistered()
            {
                var testCode = @"
class ↓Foo
{
}";

                var refactoring = new ClassNameToUpperCaseRefactoringProvider();
                var expected = "Expected the refactoring to not register any code actions.";
                var exception = Assert.Throws<AssertException>(() => RoslynAssert.NoRefactoring(refactoring, testCode));
                Assert.AreEqual(expected, exception.Message);
                exception = Assert.Throws<AssertException>(() => RoslynAssert.NoRefactoring(refactoring, testCode, title: "To uppercase"));
                Assert.AreEqual(expected, exception.Message);
            }

            [Test]
            public static void WhenActionIsRegisteredWithSpan()
            {
                var testCode = @"
class Foo
{
}";

                var refactoring = new ClassNameToUpperCaseRefactoringProvider();
                var expected = "Expected the refactoring to not register any code actions.";
                var exception = Assert.Throws<AssertException>(() => RoslynAssert.NoRefactoring(refactoring, testCode, new TextSpan(8, 3)));
                Assert.AreEqual(expected, exception.Message);
                exception = Assert.Throws<AssertException>(() => RoslynAssert.NoRefactoring(refactoring, testCode, new TextSpan(8, 3), title: "To uppercase"));
                Assert.AreEqual(expected, exception.Message);
            }
        }
    }
}
