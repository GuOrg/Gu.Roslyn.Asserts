namespace Gu.Roslyn.Asserts
{
    using System;
    using System.Collections.Generic;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CodeFixes;

    /// <summary>
    /// Provides assertions against the specified code fix provider. Use <see
    /// cref="RoslynAssert.CreateWithoutAnalyzer{TCodeFixProvider}"/> to obtain an instance.
    /// </summary>
    public sealed class FixAssert
    {
        private readonly Func<CodeFixProvider> createCodeFixProvider;
        private readonly ExpectedDiagnostic expectedDiagnostic;
        private readonly Settings? settings;

        /// <summary>
        /// Initializes a new instance of the <see cref="FixAssert"/> class. Use <see
        /// cref="RoslynAssert.CreateWithoutAnalyzer{TCodeFixProvider}"/> to obtain an instance.
        /// </summary>
        /// <param name="createCodeFixProvider">
        /// Constructs the <see cref="CodeFixProvider"/> to use in asserts.
        /// </param>
        /// <param name="expectedDiagnostic">
        /// The <see cref="ExpectedDiagnostic"/> with information about the expected <see cref="Diagnostic"/>.
        /// </param>
        /// <param name="settings">The <see cref="Settings"/>.</param>
        internal FixAssert(Func<CodeFixProvider> createCodeFixProvider, ExpectedDiagnostic expectedDiagnostic, Settings? settings = null)
        {
            this.createCodeFixProvider = createCodeFixProvider ?? throw new ArgumentNullException(nameof(createCodeFixProvider));
            this.expectedDiagnostic = expectedDiagnostic ?? throw new ArgumentNullException(nameof(expectedDiagnostic));
            this.settings = settings;
        }

        /// <summary>
        /// Verifies that
        /// 1. The current code fix provider supports fixing diagnostics reported by the current analyzer.
        /// 2. <paramref name="before"/> produces diagnostics fixable by the current code fix provider.
        /// 3. Applying the current code fix provider results in <paramref name="after"/>.
        /// </summary>
        /// <param name="before">The code to analyze with the current analyzer. Indicate error position with ↓ (alt + 25).</param>
        /// <param name="after">The expected code produced by applying the current code fix provider.</param>
        /// <param name="fixTitle">The expected title of the fix. Must be provided if more than one code action is registered.</param>
        /// <param name="settings">The <see cref="Settings"/>.</param>
        public void CodeFix(
            string before,
            string after,
            string? fixTitle = null,
            Settings? settings = null)
        {
            RoslynAssert.CodeFix(
                this.CreateCodeFixProvider(),
                this.expectedDiagnostic,
                before,
                after,
                fixTitle,
                settings ?? this.settings ?? Settings.Default);
        }

        /// <summary>
        /// Verifies that
        /// 1. The current code fix provider supports fixing diagnostics reported by the current analyzer.
        /// 2. <paramref name="before"/> produces diagnostics fixable by the current code fix provider.
        /// 3. Applying the current code fix provider results in <paramref name="after"/>.
        /// </summary>
        /// <param name="before">The code to analyze with the current analyzer. Indicate error position with ↓ (alt + 25).</param>
        /// <param name="after">The expected code produced by applying the current code fix provider.</param>
        /// <param name="fixTitle">The expected title of the fix. Must be provided if more than one code action is registered.</param>
        /// <param name="settings">The <see cref="Settings"/>.</param>
        public void CodeFix(
            IReadOnlyList<string> before,
            string after,
            string? fixTitle = null,
            Settings? settings = null)
        {
            RoslynAssert.CodeFix(
                this.CreateCodeFixProvider(),
                this.expectedDiagnostic,
                before,
                after,
                fixTitle,
                settings ?? this.settings ?? Settings.Default);
        }

        /// <summary>
        /// Verifies that
        /// 1. The current code fix provider supports fixing diagnostics reported by the current analyzer.
        /// 2. <paramref name="before"/> produces diagnostics fixable by the current code fix provider.
        /// 3. Applying the current code fix provider results in <paramref name="after"/>.
        /// </summary>
        /// <param name="before">The code to analyze with the current analyzer. Indicate error position with ↓ (alt + 25).</param>
        /// <param name="after">The expected code produced by applying the current code fix provider.</param>
        /// <param name="fixTitle">The expected title of the fix. Must be provided if more than one code action is registered.</param>
        /// <param name="settings">The <see cref="Settings"/>.</param>
        public void CodeFix(
            IReadOnlyList<string> before,
            IReadOnlyList<string> after,
            string? fixTitle = null,
            Settings? settings = null)
        {
            RoslynAssert.CodeFix(
                this.CreateCodeFixProvider(),
                this.expectedDiagnostic,
                before,
                after,
                fixTitle,
                settings ?? this.settings ?? Settings.Default);
        }

        /// <summary>
        /// Verifies that
        /// 1. The current code fix provider supports fixing diagnostics reported by the current analyzer.
        /// 2. <paramref name="before"/> produces diagnostics fixable by the current code fix provider.
        /// 3. Applying the current code fix provider results in <paramref name="after"/>.
        /// </summary>
        /// <param name="expectedDiagnostic">
        /// The <see cref="ExpectedDiagnostic"/> with information about the expected <see cref="Diagnostic"/>. If the
        /// current analyzer supports more than one <see cref="DiagnosticDescriptor.Id"/> this must be provided.
        /// </param>
        /// <param name="before">The code to analyze with the current analyzer. Indicate error position with ↓ (alt + 25).</param>
        /// <param name="after">The expected code produced by applying the current code fix provider.</param>
        /// <param name="fixTitle">The expected title of the fix. Must be provided if more than one code action is registered.</param>
        /// <param name="settings">The <see cref="Settings"/>.</param>
        public void CodeFix(
            ExpectedDiagnostic expectedDiagnostic,
            string before,
            string after,
            string? fixTitle = null,
            Settings? settings = null)
        {
            RoslynAssert.CodeFix(
                this.CreateCodeFixProvider(),
                expectedDiagnostic,
                before,
                after,
                fixTitle,
                settings ?? this.settings ?? Settings.Default);
        }

        /// <summary>
        /// Verifies that
        /// 1. The current code fix provider supports fixing diagnostics reported by the current analyzer.
        /// 2. <paramref name="before"/> produces diagnostics fixable by the current code fix provider.
        /// 3. Applying the current code fix provider results in <paramref name="after"/>.
        /// </summary>
        /// <param name="expectedDiagnostic">
        /// The <see cref="ExpectedDiagnostic"/> with information about the expected <see cref="Diagnostic"/>. If the
        /// current analyzer supports more than one <see cref="DiagnosticDescriptor.Id"/> this must be provided.
        /// </param>
        /// <param name="before">The code to analyze with the current analyzer. Indicate error position with ↓ (alt + 25).</param>
        /// <param name="after">The expected code produced by applying the current code fix provider.</param>
        /// <param name="fixTitle">The expected title of the fix. Must be provided if more than one code action is registered.</param>
        /// <param name="settings">The <see cref="Settings"/>.</param>
        public void CodeFix(
            ExpectedDiagnostic expectedDiagnostic,
            IReadOnlyList<string> before,
            string after,
            string? fixTitle = null,
            Settings? settings = null)
        {
            RoslynAssert.CodeFix(
                this.CreateCodeFixProvider(),
                expectedDiagnostic,
                before,
                after,
                fixTitle,
                settings ?? this.settings ?? Settings.Default);
        }

        /// <summary>
        /// Verifies that
        /// 1. The current code fix provider supports fixing diagnostics reported by the current analyzer.
        /// 2. <paramref name="before"/> produces diagnostics fixable by the current code fix provider.
        /// 3. Applying the current code fix provider results in <paramref name="after"/>.
        /// </summary>
        /// <param name="expectedDiagnostic">
        /// The <see cref="ExpectedDiagnostic"/> with information about the expected <see cref="Diagnostic"/>. If the
        /// current analyzer supports more than one <see cref="DiagnosticDescriptor.Id"/> this must be provided.
        /// </param>
        /// <param name="before">The code to analyze with the current analyzer. Indicate error position with ↓ (alt + 25).</param>
        /// <param name="after">The expected code produced by applying the current code fix provider.</param>
        /// <param name="fixTitle">The expected title of the fix. Must be provided if more than one code action is registered.</param>
        /// <param name="settings">The <see cref="Settings"/>.</param>
        public void CodeFix(
            ExpectedDiagnostic expectedDiagnostic,
            IReadOnlyList<string> before,
            IReadOnlyList<string> after,
            string? fixTitle = null,
            Settings? settings = null)
        {
            RoslynAssert.CodeFix(
                this.CreateCodeFixProvider(),
                expectedDiagnostic,
                before,
                after,
                fixTitle,
                settings ?? this.settings ?? Settings.Default);
        }

        /// <summary>
        /// Verifies that
        /// 1. <paramref name="before"/> produces the expected diagnostics
        /// 2. The code fix fixes the code.
        /// </summary>
        /// <param name="before">The code to analyze for the current expected diagnostic. Indicate error position with ↓ (alt + 25).</param>
        /// <param name="after">The expected code produced by applying the current code fix.</param>
        /// <param name="fixTitle">The expected title of the fix. Must be provided if more than one code action is registered.</param>
        /// <param name="settings">The <see cref="Settings"/>.</param>
        public void FixAll(
            IReadOnlyList<string> before,
            string after,
            string? fixTitle = null,
            Settings? settings = null)
        {
            RoslynAssert.FixAll(
                this.CreateCodeFixProvider(),
                this.expectedDiagnostic,
                before,
                after,
                fixTitle,
                settings ?? this.settings ?? Settings.Default);
        }

        /// <summary>
        /// Verifies that
        /// 1. <paramref name="before"/> produces the expected diagnostics
        /// 2. The code fix fixes the code.
        /// </summary>
        /// <param name="before">The code to analyze for the current expected diagnostic. Indicate error position with ↓ (alt + 25).</param>
        /// <param name="after">The expected code produced by applying the current code fix.</param>
        /// <param name="fixTitle">The expected title of the fix. Must be provided if more than one code action is registered.</param>
        /// <param name="settings">The <see cref="Settings"/>.</param>
        public void FixAll(
            IReadOnlyList<string> before,
            IReadOnlyList<string> after,
            string? fixTitle = null,
            Settings? settings = null)
        {
            RoslynAssert.FixAll(
                this.CreateCodeFixProvider(),
                this.expectedDiagnostic,
                before,
                after,
                fixTitle,
                settings ?? this.settings ?? Settings.Default);
        }

        /// <summary>
        /// Constructs the <see cref="CodeFixProvider"/> to use in asserts.
        /// </summary>
        private CodeFixProvider CreateCodeFixProvider() => this.createCodeFixProvider();
    }
}
