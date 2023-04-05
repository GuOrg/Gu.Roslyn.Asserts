namespace Gu.Roslyn.Asserts.Tests.RoslynAssertTests;

using Gu.Roslyn.Asserts.Tests.Refactorings;
using Microsoft.CodeAnalysis.Text;
using NUnit.Framework;

public static partial class NoRefactoring
{
    public static class Success
    {
        [Test]
        public static void WhenNoActionWithPositionIndicated()
        {
            var code = """
                class ↓C
                {
                }
                """;

            var refactoring = new ClassNameToUpperCaseRefactoringProvider();
            RoslynAssert.NoRefactoring(refactoring, code);
            RoslynAssert.NoRefactoring(refactoring, code, title: "To uppercase");
        }

        [Test]
        public static void WhenNoActionWithSpan()
        {
            var code = """
                class C
                {
                }
                """;

            var refactoring = new ClassNameToUpperCaseRefactoringProvider();
            RoslynAssert.NoRefactoring(refactoring, code, new TextSpan(8, 3));
            RoslynAssert.NoRefactoring(refactoring, code, new TextSpan(8, 3), title: "To uppercase");
        }
    }
}
