namespace Gu.Roslyn.Asserts.Tests
{
    using Gu.Roslyn.Asserts.Tests.Refactorings;
    using Microsoft.CodeAnalysis.Text;
    using NUnit.Framework;

    public class AnalyzerAssertRefactoringTests
    {
        [Test]
        public void WithPositionIndicated()
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
            AnalyzerAssert.Refactoring(refactoring, testCode, fixedCode);
            AnalyzerAssert.Refactoring(refactoring, testCode, 0, fixedCode);
        }

        [Test]
        public void WithSpan()
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
            AnalyzerAssert.Refactoring(refactoring, testCode, new TextSpan(8, 3), fixedCode);
            AnalyzerAssert.Refactoring(refactoring, testCode, new TextSpan(8, 3), 0, fixedCode);
        }
    }
}
