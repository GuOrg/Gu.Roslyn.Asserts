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
        public static void NoRefactoring(CodeRefactoringProvider refactoring, string code)
        {
            var position = GetPosition(code, out var testCode);
#pragma warning disable CS0618 // Suppress until removed. Will be replaced with MetadataReferences.FromAttributes()
            var actions = Refactor.CodeActions(refactoring, testCode, position, MetadataReferences);
#pragma warning restore CS0618 // Suppress until removed. Will be replaced with MetadataReferences.FromAttributes()
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
        public static void NoRefactoring(CodeRefactoringProvider refactoring, string code, string title)
        {
            var position = GetPosition(code, out var testCode);
#pragma warning disable CS0618 // Suppress until removed. Will be replaced with MetadataReferences.FromAttributes()
            var actions = Refactor.CodeActions(refactoring, testCode, position, MetadataReferences);
#pragma warning restore CS0618 // Suppress until removed. Will be replaced with MetadataReferences.FromAttributes()
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
        public static void NoRefactoring(CodeRefactoringProvider refactoring, string code, TextSpan span)
        {
#pragma warning disable CS0618 // Suppress until removed. Will be replaced with MetadataReferences.FromAttributes()
            var actions = Refactor.CodeActions(refactoring, code, span, MetadataReferences);
#pragma warning restore CS0618 // Suppress until removed. Will be replaced with MetadataReferences.FromAttributes()
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
        public static void NoRefactoring(CodeRefactoringProvider refactoring, string code, TextSpan span, string title)
        {
#pragma warning disable CS0618 // Suppress until removed. Will be replaced with MetadataReferences.FromAttributes()
            var actions = Refactor.CodeActions(refactoring, code, span, MetadataReferences);
#pragma warning restore CS0618 // Suppress until removed. Will be replaced with MetadataReferences.FromAttributes()
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
        public static void Refactoring(CodeRefactoringProvider refactoring, string before, string after)
        {
            var position = GetPosition(before, out var testCode);
#pragma warning disable CS0618 // Suppress until removed. Will be replaced with MetadataReferences.FromAttributes()
            var refactored = Refactor.Apply(refactoring, testCode, position, MetadataReferences);
#pragma warning restore CS0618 // Suppress until removed. Will be replaced with MetadataReferences.FromAttributes()
            CodeAssert.AreEqual(after, refactored);
        }

        /// <summary>
        /// For testing a <see cref="CodeRefactoringProvider"/>.
        /// </summary>
        /// <param name="refactoring">The <see cref="CodeRefactoringProvider"/>.</param>
        /// <param name="before">The code to analyze with <paramref name="refactoring"/>. Indicate position with ↓ (alt + 25).</param>
        /// <param name="after">The expected code produced by <paramref name="refactoring"/>.</param>
        /// <param name="title">The title of the refactoring to apply.</param>
        public static void Refactoring(CodeRefactoringProvider refactoring, string before, string after, string title)
        {
            var position = GetPosition(before, out var testCode);
#pragma warning disable CS0618 // Suppress until removed. Will be replaced with MetadataReferences.FromAttributes()
            var refactored = Refactor.Apply(refactoring, testCode, position, title, MetadataReferences);
#pragma warning restore CS0618 // Suppress until removed. Will be replaced with MetadataReferences.FromAttributes()
            CodeAssert.AreEqual(after, refactored);
        }

        /// <summary>
        /// For testing a <see cref="CodeRefactoringProvider"/>.
        /// </summary>
        /// <param name="refactoring">The <see cref="CodeRefactoringProvider"/>.</param>
        /// <param name="before">The code to analyze with <paramref name="refactoring"/>. Position is provided by <paramref name="span"/>.</param>
        /// <param name="span">A <see cref="TextSpan"/> indicating the position.</param>
        /// <param name="after">The expected code produced by <paramref name="refactoring"/>.</param>
        public static void Refactoring(CodeRefactoringProvider refactoring, string before, TextSpan span, string after)
        {
#pragma warning disable CS0618 // Suppress until removed. Will be replaced with MetadataReferences.FromAttributes()
            var refactored = Refactor.Apply(refactoring, before, span, MetadataReferences);
#pragma warning restore CS0618 // Suppress until removed. Will be replaced with MetadataReferences.FromAttributes()
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
        public static void Refactoring(CodeRefactoringProvider refactoring, string before, TextSpan span, string after, string title)
        {
#pragma warning disable CS0618 // Suppress until removed. Will be replaced with MetadataReferences.FromAttributes()
            var refactored = Refactor.Apply(refactoring, before, span, title, MetadataReferences);
#pragma warning restore CS0618 // Suppress until removed. Will be replaced with MetadataReferences.FromAttributes()
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
