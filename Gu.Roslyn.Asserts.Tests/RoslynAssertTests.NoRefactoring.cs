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
                var code = @"
class ↓FOO
{
}";

                var refactoring = new ClassNameToUpperCaseRefactoringProvider();
                RoslynAssert.NoRefactoring(refactoring, code);
                RoslynAssert.NoRefactoring(refactoring, code, title: "To uppercase");
            }

            [Test]
            public static void WhenNoActionWithSPan()
            {
                var code = @"
class FOO
{
}";

                var refactoring = new ClassNameToUpperCaseRefactoringProvider();
                RoslynAssert.NoRefactoring(refactoring, code, new TextSpan(8, 3));
                RoslynAssert.NoRefactoring(refactoring, code, new TextSpan(8, 3), title: "To uppercase");
            }

            [Test]
            public static void WhenActionIsRegistered()
            {
                var code = @"
class ↓Foo
{
}";

                var refactoring = new ClassNameToUpperCaseRefactoringProvider();
                var expected = "Expected the refactoring to not register any code actions.";
                var exception = Assert.Throws<AssertException>(() => RoslynAssert.NoRefactoring(refactoring, code));
                Assert.AreEqual(expected, exception.Message);
                exception = Assert.Throws<AssertException>(() => RoslynAssert.NoRefactoring(refactoring, code, title: "To uppercase"));
                Assert.AreEqual(expected, exception.Message);
            }

            [Test]
            public static void WhenActionIsRegisteredWithSpan()
            {
                var code = @"
class Foo
{
}";

                var refactoring = new ClassNameToUpperCaseRefactoringProvider();
                var expected = "Expected the refactoring to not register any code actions.";
                var exception = Assert.Throws<AssertException>(() => RoslynAssert.NoRefactoring(refactoring, code, new TextSpan(8, 3)));
                Assert.AreEqual(expected, exception.Message);
                exception = Assert.Throws<AssertException>(() => RoslynAssert.NoRefactoring(refactoring, code, new TextSpan(8, 3), title: "To uppercase"));
                Assert.AreEqual(expected, exception.Message);
            }
        }
    }
}
