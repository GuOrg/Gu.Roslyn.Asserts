namespace Gu.Roslyn.Asserts.Tests
{
    using Gu.Roslyn.Asserts.Tests.Refactorings;
    using Microsoft.CodeAnalysis.Text;
    using NUnit.Framework;

    public partial class RoslynAssertTests
    {
        public class Refactoring
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
                RoslynAssert.Refactoring(refactoring, testCode, fixedCode);
                RoslynAssert.Refactoring(refactoring, testCode, "To uppercase", fixedCode);
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
                RoslynAssert.Refactoring(refactoring, testCode, new TextSpan(8, 3), fixedCode);
                RoslynAssert.Refactoring(refactoring, testCode, new TextSpan(8, 3), "To uppercase", fixedCode);
            }
        }
    }
}
