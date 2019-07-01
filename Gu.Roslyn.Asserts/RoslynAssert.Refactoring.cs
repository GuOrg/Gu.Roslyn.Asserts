namespace Gu.Roslyn.Asserts
{
    using System;
    using System.Linq;
    using Microsoft.CodeAnalysis.CodeRefactorings;
    using Microsoft.CodeAnalysis.Text;

    public static partial class RoslynAssert
    {
        /// <summary>
        /// For testing a <see cref="CodeRefactoringProvider"/>.
        /// </summary>
        /// <param name="refactoring">The <see cref="CodeRefactoringProvider"/>.</param>
        /// <param name="codeWithPositionIndicated">The code that is not supposed to trigger a refactoring with position indicated with ↓.</param>
        public static void NoRefactoring(CodeRefactoringProvider refactoring, string codeWithPositionIndicated)
        {
            var position = GetPosition(codeWithPositionIndicated, out var testCode);
            var actions = Refactor.CodeActions(refactoring, testCode, position, MetadataReferences);
            if (actions.Any())
            {
                throw new AssertException("Expected the refactoring to not register any code actions.");
            }
        }

        /// <summary>
        /// For testing a <see cref="CodeRefactoringProvider"/>.
        /// </summary>
        /// <param name="refactoring">The <see cref="CodeRefactoringProvider"/>.</param>
        /// <param name="codeWithPositionIndicated">The code that is not supposed to trigger a refactoring with position indicated with ↓.</param>
        /// <param name="title">The title of the refactoring to apply.</param>
        public static void NoRefactoring(CodeRefactoringProvider refactoring, string codeWithPositionIndicated, string title)
        {
            var position = GetPosition(codeWithPositionIndicated, out var testCode);
            var actions = Refactor.CodeActions(refactoring, testCode, position, MetadataReferences);
            if (actions.Any(x => x.Title == title))
            {
                throw new AssertException("Expected the refactoring to not register any code actions.");
            }
        }

        /// <summary>
        /// For testing a <see cref="CodeRefactoringProvider"/>.
        /// </summary>
        /// <param name="refactoring">The <see cref="CodeRefactoringProvider"/>.</param>
        /// <param name="code">The code that is not supposed to trigger a refactoring.</param>
        /// <param name="span">The position.</param>
        public static void NoRefactoring(CodeRefactoringProvider refactoring, string code, TextSpan span)
        {
            var actions = Refactor.CodeActions(refactoring, code, span, MetadataReferences);
            if (actions.Any())
            {
                throw new AssertException("Expected the refactoring to not register any code actions.");
            }
        }

        /// <summary>
        /// For testing a <see cref="CodeRefactoringProvider"/>.
        /// </summary>
        /// <param name="refactoring">The <see cref="CodeRefactoringProvider"/>.</param>
        /// <param name="code">The code that is not supposed to trigger a refactoring.</param>
        /// <param name="span">The position.</param>
        /// <param name="title">The title of the refactoring to apply.</param>
        public static void NoRefactoring(CodeRefactoringProvider refactoring, string code, TextSpan span, string title)
        {
            var actions = Refactor.CodeActions(refactoring, code, span, MetadataReferences);
            if (actions.Any(x => x.Title == title))
            {
                throw new AssertException("Expected the refactoring to not register any code actions.");
            }
        }

        /// <summary>
        /// For testing a <see cref="CodeRefactoringProvider"/>.
        /// </summary>
        /// <param name="refactoring">The <see cref="CodeRefactoringProvider"/>.</param>
        /// <param name="codeWithPositionIndicated">The code to refactor with position indicated with ↓.</param>
        /// <param name="fixedCode">The expected code produced by the refactoring.</param>
        public static void Refactoring(CodeRefactoringProvider refactoring, string codeWithPositionIndicated, string fixedCode)
        {
            var position = GetPosition(codeWithPositionIndicated, out var testCode);
            var refactored = Refactor.Apply(refactoring, testCode, position, MetadataReferences);
            CodeAssert.AreEqual(fixedCode, refactored);
        }

        /// <summary>
        /// For testing a <see cref="CodeRefactoringProvider"/>.
        /// </summary>
        /// <param name="refactoring">The <see cref="CodeRefactoringProvider"/>.</param>
        /// <param name="codeWithPositionIndicated">The code to refactor with position indicated with ↓.</param>
        /// <param name="fixedCode">The expected code produced by the refactoring.</param>
        /// <param name="title">The title of the refactoring to apply.</param>
        public static void Refactoring(CodeRefactoringProvider refactoring, string codeWithPositionIndicated, string fixedCode, string title)
        {
            var position = GetPosition(codeWithPositionIndicated, out var testCode);
            var refactored = Refactor.Apply(refactoring, testCode, position, title, MetadataReferences);
            CodeAssert.AreEqual(fixedCode, refactored);
        }

        /// <summary>
        /// For testing a <see cref="CodeRefactoringProvider"/>.
        /// </summary>
        /// <param name="refactoring">The <see cref="CodeRefactoringProvider"/>.</param>
        /// <param name="code">The code to refactor.</param>
        /// <param name="span">The position.</param>
        /// <param name="fixedCode">The expected code produced by the refactoring.</param>
        public static void Refactoring(CodeRefactoringProvider refactoring, string code, TextSpan span, string fixedCode)
        {
            var refactored = Refactor.Apply(refactoring, code, span, MetadataReferences);
            CodeAssert.AreEqual(fixedCode, refactored);
        }

        /// <summary>
        /// For testing a <see cref="CodeRefactoringProvider"/>.
        /// </summary>
        /// <param name="refactoring">The <see cref="CodeRefactoringProvider"/>.</param>
        /// <param name="code">The code to refactor.</param>
        /// <param name="span">The position.</param>
        /// <param name="fixedCode">The expected code produced by the refactoring.</param>
        /// <param name="title">The title of the refactoring to apply.</param>
        public static void Refactoring(CodeRefactoringProvider refactoring, string code, TextSpan span, string fixedCode, string title)
        {
            var refactored = Refactor.Apply(refactoring, code, span, title, MetadataReferences);
            CodeAssert.AreEqual(fixedCode, refactored);
        }

        private static int GetPosition(string codeWithPositionIndicated, out string code)
        {
            var position = codeWithPositionIndicated.IndexOf("↓", StringComparison.Ordinal);
            if (position >= 0 &&
                codeWithPositionIndicated.IndexOf("↓", position + 1, StringComparison.Ordinal) < 0)
            {
                code = codeWithPositionIndicated.AssertReplace("↓", string.Empty);
                return position;
            }

            throw new InvalidOperationException("Expected exactly one position indicated with ↓");
        }
    }
}
