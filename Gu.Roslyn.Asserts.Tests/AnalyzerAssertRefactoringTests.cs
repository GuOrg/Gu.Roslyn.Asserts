namespace Gu.Roslyn.Asserts.Tests
{
    using Gu.Roslyn.Asserts.Tests.Refactorings;
    using NUnit.Framework;

    public class AnalyzerAssertRefactoringTests
    {
        [Test]
        public void ClassName()
        {
            var testCode = @"
class ↓Foo
{
}";

            var fixedCode = @"
class FOO
{
}";

            AnalyzerAssert.Refactoring(new ClassNameToUpperCaseRefactoringProvider(), testCode, fixedCode);
        }
    }
}
