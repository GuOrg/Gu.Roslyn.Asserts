namespace Gu.Roslyn.Asserts
{
    using Microsoft.CodeAnalysis.CodeRefactorings;

    public static partial class AnalyzerAssert
    {
        public static void Refactoring(CodeRefactoringProvider refactoring, string codeWithPositionIndicated, string fixedCode)
        {
            var position = codeWithPositionIndicated.IndexOf("↓");
            var testCode = codeWithPositionIndicated.AssertReplace("↓", string.Empty);
            var refactored = Refactor.Apply(refactoring, testCode, position, MetadataReferences);
            CodeAssert.AreEqual(refactored, fixedCode);
        }
    }
}
