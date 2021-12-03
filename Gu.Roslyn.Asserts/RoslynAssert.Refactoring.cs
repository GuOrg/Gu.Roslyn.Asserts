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
        /// <param name="code">The code to analyze with <paramref name="refactoring"/>. Indicate position with ↓ (alt + 25).</param>
        /// <param name="settings">The <see cref="Settings"/>.</param>
        public static void NoRefactoring(CodeRefactoringProvider refactoring, string code, Settings? settings = null)
        {
            if (refactoring is null)
            {
                throw new ArgumentNullException(nameof(refactoring));
            }

            if (code is null)
            {
                throw new ArgumentNullException(nameof(code));
            }

            var position = GetPosition(code, out var testCode);
            var actions = Refactor.CodeActions(refactoring, testCode, position, settings);
            if (actions.Any())
            {
                throw new AssertException("Expected the refactoring to not register any code actions.");
            }
        }

        /// <summary>
        /// For testing a <see cref="CodeRefactoringProvider"/>.
        /// </summary>
        /// <param name="refactoring">The <see cref="CodeRefactoringProvider"/>.</param>
        /// <param name="code">The code to analyze with <paramref name="refactoring"/>. Indicate position with ↓ (alt + 25).</param>
        /// <param name="title">The title of the refactoring to apply.</param>
        /// <param name="settings">The <see cref="Settings"/>.</param>
        public static void NoRefactoring(CodeRefactoringProvider refactoring, string code, string title, Settings? settings = null)
        {
            if (refactoring is null)
            {
                throw new ArgumentNullException(nameof(refactoring));
            }

            if (code is null)
            {
                throw new ArgumentNullException(nameof(code));
            }

            if (title is null)
            {
                throw new ArgumentNullException(nameof(title));
            }

            var position = GetPosition(code, out var testCode);
            var actions = Refactor.CodeActions(refactoring, testCode, position, settings);
            if (actions.Any(x => x.Title == title))
            {
                throw new AssertException("Expected the refactoring to not register any code actions.");
            }
        }

        /// <summary>
        /// For testing a <see cref="CodeRefactoringProvider"/>.
        /// </summary>
        /// <param name="refactoring">The <see cref="CodeRefactoringProvider"/>.</param>
        /// <param name="code">The code to analyze with <paramref name="refactoring"/>. Position is provided by <paramref name="span"/>.</param>
        /// <param name="span">A <see cref="TextSpan"/> indicating the position.</param>
        /// <param name="settings">The <see cref="Settings"/>.</param>
        public static void NoRefactoring(CodeRefactoringProvider refactoring, string code, TextSpan span, Settings? settings = null)
        {
            if (refactoring is null)
            {
                throw new ArgumentNullException(nameof(refactoring));
            }

            if (code is null)
            {
                throw new ArgumentNullException(nameof(code));
            }

            var actions = Refactor.CodeActions(refactoring, code, span, settings);
            if (actions.Any())
            {
                throw new AssertException("Expected the refactoring to not register any code actions.");
            }
        }

        /// <summary>
        /// For testing a <see cref="CodeRefactoringProvider"/>.
        /// </summary>
        /// <param name="refactoring">The <see cref="CodeRefactoringProvider"/>.</param>
        /// <param name="code">The code to analyze with <paramref name="refactoring"/>. Position is provided by <paramref name="span"/>.</param>
        /// <param name="span">A <see cref="TextSpan"/> indicating the position.</param>
        /// <param name="title">The title of the refactoring to apply.</param>
        /// <param name="settings">The <see cref="Settings"/>.</param>
        public static void NoRefactoring(CodeRefactoringProvider refactoring, string code, TextSpan span, string title, Settings? settings = null)
        {
            if (refactoring is null)
            {
                throw new ArgumentNullException(nameof(refactoring));
            }

            if (code is null)
            {
                throw new ArgumentNullException(nameof(code));
            }

            if (title is null)
            {
                throw new ArgumentNullException(nameof(title));
            }

            var actions = Refactor.CodeActions(refactoring, code, span, settings);
            if (actions.Any(x => x.Title == title))
            {
                throw new AssertException("Expected the refactoring to not register any code actions.");
            }
        }

        /// <summary>
        /// For testing a <see cref="CodeRefactoringProvider"/>.
        /// </summary>
        /// <param name="refactoring">The <see cref="CodeRefactoringProvider"/>.</param>
        /// <param name="before">The code to analyze with <paramref name="refactoring"/>. Indicate position with ↓ (alt + 25).</param>
        /// <param name="after">The expected code produced by <paramref name="refactoring"/>.</param>
        /// <param name="settings">The <see cref="Settings"/>.</param>
        public static void Refactoring(CodeRefactoringProvider refactoring, string before, string after, Settings? settings = null)
        {
            if (refactoring is null)
            {
                throw new ArgumentNullException(nameof(refactoring));
            }

            if (before is null)
            {
                throw new ArgumentNullException(nameof(before));
            }

            if (after is null)
            {
                throw new ArgumentNullException(nameof(after));
            }

            var position = GetPosition(before, out var testCode);
            var refactored = Refactor.Apply(refactoring, testCode, position, settings);
            CodeAssert.AreEqual(after, refactored);
        }

        /// <summary>
        /// For testing a <see cref="CodeRefactoringProvider"/>.
        /// </summary>
        /// <param name="refactoring">The <see cref="CodeRefactoringProvider"/>.</param>
        /// <param name="before">The code to analyze with <paramref name="refactoring"/>. Indicate position with ↓ (alt + 25).</param>
        /// <param name="after">The expected code produced by <paramref name="refactoring"/>.</param>
        /// <param name="title">The title of the refactoring to apply.</param>
        /// <param name="settings">The <see cref="Settings"/>.</param>
        public static void Refactoring(CodeRefactoringProvider refactoring, string before, string after, string title, Settings? settings = null)
        {
            if (refactoring is null)
            {
                throw new ArgumentNullException(nameof(refactoring));
            }

            if (before is null)
            {
                throw new ArgumentNullException(nameof(before));
            }

            if (after is null)
            {
                throw new ArgumentNullException(nameof(after));
            }

            if (title is null)
            {
                throw new ArgumentNullException(nameof(title));
            }

            var position = GetPosition(before, out var testCode);
            var refactored = Refactor.Apply(refactoring, testCode, position, title, settings);
            CodeAssert.AreEqual(after, refactored);
        }

        /// <summary>
        /// For testing a <see cref="CodeRefactoringProvider"/>.
        /// </summary>
        /// <param name="refactoring">The <see cref="CodeRefactoringProvider"/>.</param>
        /// <param name="before">The code to analyze with <paramref name="refactoring"/>. Position is provided by <paramref name="span"/>.</param>
        /// <param name="span">A <see cref="TextSpan"/> indicating the position.</param>
        /// <param name="after">The expected code produced by <paramref name="refactoring"/>.</param>
        /// <param name="settings">The <see cref="Settings"/>.</param>
        public static void Refactoring(CodeRefactoringProvider refactoring, string before, TextSpan span, string after, Settings? settings = null)
        {
            if (refactoring is null)
            {
                throw new ArgumentNullException(nameof(refactoring));
            }

            if (before is null)
            {
                throw new ArgumentNullException(nameof(before));
            }

            if (after is null)
            {
                throw new ArgumentNullException(nameof(after));
            }

            var refactored = Refactor.Apply(refactoring, before, span, settings);
            CodeAssert.AreEqual(after, refactored);
        }

        /// <summary>
        /// For testing a <see cref="CodeRefactoringProvider"/>.
        /// </summary>
        /// <param name="refactoring">The <see cref="CodeRefactoringProvider"/>.</param>
        /// <param name="before">The code to analyze with <paramref name="refactoring"/>. Position is provided by <paramref name="span"/>.</param>
        /// <param name="span">A <see cref="TextSpan"/> indicating the position.</param>
        /// <param name="after">The expected code produced by <paramref name="refactoring"/>.</param>
        /// <param name="title">The title of the refactoring to apply.</param>
        /// <param name="settings">The <see cref="Settings"/>.</param>
        public static void Refactoring(CodeRefactoringProvider refactoring, string before, TextSpan span, string after, string title, Settings? settings = null)
        {
            if (refactoring is null)
            {
                throw new ArgumentNullException(nameof(refactoring));
            }

            if (before is null)
            {
                throw new ArgumentNullException(nameof(before));
            }

            if (after is null)
            {
                throw new ArgumentNullException(nameof(after));
            }

            if (title is null)
            {
                throw new ArgumentNullException(nameof(title));
            }

            var refactored = Refactor.Apply(refactoring, before, span, title, settings);
            CodeAssert.AreEqual(after, refactored);
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
