namespace Gu.Roslyn.Asserts
{
    using Microsoft.CodeAnalysis.CodeRefactorings;
    using Microsoft.CodeAnalysis.Text;

    public static partial class AnalyzerAssert
    {
        public static void Refactoring(CodeRefactoringProvider refactoring, string codeWithPositionIndicated, string fixedCode)
        {
            var position = codeWithPositionIndicated.IndexOf("↓");
            var testCode = codeWithPositionIndicated.AssertReplace("↓", string.Empty);
            var refactored = Refactor.Apply(refactoring, testCode, position, MetadataReferences);
            CodeAssert.AreEqual(refactored, fixedCode);
        }

        public static void Refactoring(CodeRefactoringProvider refactoring, string codeWithPositionIndicated, int index, string fixedCode)
        {
            var position = codeWithPositionIndicated.IndexOf("↓");
            var testCode = codeWithPositionIndicated.AssertReplace("↓", string.Empty);
            var refactored = Refactor.Apply(refactoring, testCode, position, index, MetadataReferences);
            CodeAssert.AreEqual(refactored, fixedCode);
        }

        public static void Refactoring(CodeRefactoringProvider refactoring, string code, TextSpan span, string fixedCode)
        {
            var refactored = Refactor.Apply(refactoring, code, span, MetadataReferences);
            CodeAssert.AreEqual(refactored, fixedCode);
        }

        public static void Refactoring(CodeRefactoringProvider refactoring, string code, TextSpan span, int index, string fixedCode)
        {
            var refactored = Refactor.Apply(refactoring, code, span, index, MetadataReferences);
            CodeAssert.AreEqual(refactored, fixedCode);
        }
    }
}
