#pragma warning disable IDE0079 // Remove unnecessary suppression
namespace Gu.Roslyn.Asserts.Tests.RoslynAssertTests;

using Gu.Roslyn.Asserts.Tests.Refactorings;
using Microsoft.CodeAnalysis.Text;
using NUnit.Framework;

public static partial class Refactoring
{
    public static class Success
    {
        [Test]
        public static void WithPositionIndicated()
        {
            var before = """

                class ↓c
                {
                }
                """;

            var after = """

                class C
                {
                }
                """;

            var refactoring = new ClassNameToUpperCaseRefactoringProvider();
            RoslynAssert.Refactoring(refactoring, before, after);
            RoslynAssert.Refactoring(refactoring, before, after, title: "To uppercase");
        }

        [Test]
        public static void WithSpan()
        {
#pragma warning disable GURA02 // Indicate position.
            var before = """

                class c
                {
                }
                """;
#pragma warning restore GURA02 // Indicate position.

            var after = """

                class C
                {
                }
                """;

            var refactoring = new ClassNameToUpperCaseRefactoringProvider();
            RoslynAssert.Refactoring(refactoring, before, new TextSpan(8, 3), after);
            RoslynAssert.Refactoring(refactoring, before, new TextSpan(8, 3), after, title: "To uppercase");
        }
    }
}
