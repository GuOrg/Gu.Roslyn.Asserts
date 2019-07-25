namespace Gu.Roslyn.Asserts.Tests
{
    using Gu.Roslyn.Asserts.Tests.Refactorings;
    using Microsoft.CodeAnalysis.Text;
    using NUnit.Framework;

    public static partial class NoRefactoring
    {
        public static class Fail
        {
            [Test]
            public static void WhenActionIsRegistered()
            {
                var code = @"
class ↓c
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
class c
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
