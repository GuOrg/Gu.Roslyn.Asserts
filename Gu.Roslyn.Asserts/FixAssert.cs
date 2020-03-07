namespace Gu.Roslyn.Asserts
{
    using System;
    using System.Collections.Generic;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CodeFixes;
    using Microsoft.CodeAnalysis.CSharp;

    /// <summary>
    /// Provides assertions against the specified code fix provider. Use <see
    /// cref="RoslynAssert.CreateWithoutAnalyzer{TCodeFixProvider}"/> to obtain an instance.
    /// </summary>
    public sealed class FixAssert
    {
        private readonly Func<CodeFixProvider> createCodeFixProvider;
        private readonly ExpectedDiagnostic expectedDiagnostic;

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
        internal FixAssert(Func<CodeFixProvider> createCodeFixProvider, ExpectedDiagnostic expectedDiagnostic)
        {
            this.createCodeFixProvider = createCodeFixProvider ?? throw new ArgumentNullException(nameof(createCodeFixProvider));
            this.expectedDiagnostic = expectedDiagnostic ?? throw new ArgumentNullException(nameof(expectedDiagnostic));
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
        /// <param name="allowCompilationErrors">Specify if compilation errors are accepted in the fixed code. This can be for example syntax errors. Default value is <see cref="AllowCompilationErrors.No"/>.</param>
        /// <param name="suppressWarnings">A collection of <see cref="DiagnosticDescriptor.Id"/> to suppress when analyzing the code. Default is <see langword="null" /> meaning <see cref="RoslynAssert.SuppressedDiagnostics"/> are used.</param>
        /// <param name="metadataReferences">A collection of <see cref="MetadataReference"/> to use when compiling. Default is <see langword="null" /> meaning <see cref="MetadataReferences"/> are used.</param>
        /// <param name="compilationOptions">The <see cref="CSharpCompilationOptions"/>.</param>
        public void CodeFix(
            string before,
            string after,
            string? fixTitle = null,
            AllowCompilationErrors allowCompilationErrors = AllowCompilationErrors.No,
            IEnumerable<string>? suppressWarnings = null,
            IEnumerable<MetadataReference>? metadataReferences = null,
            CSharpCompilationOptions? compilationOptions = null)
        {
            RoslynAssert.CodeFix(
                this.CreateCodeFixProvider(),
                this.expectedDiagnostic,
                before,
                after,
                fixTitle,
                allowCompilationErrors,
                suppressWarnings,
                metadataReferences,
                compilationOptions);
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
        /// <param name="allowCompilationErrors">Specify if compilation errors are accepted in the fixed code. This can be for example syntax errors. Default value is <see cref="AllowCompilationErrors.No"/>.</param>
        /// <param name="suppressWarnings">A collection of <see cref="DiagnosticDescriptor.Id"/> to suppress when analyzing the code. Default is <see langword="null" /> meaning <see cref="RoslynAssert.SuppressedDiagnostics"/> are used.</param>
        /// <param name="metadataReferences">A collection of <see cref="MetadataReference"/> to use when compiling. Default is <see langword="null" /> meaning <see cref="MetadataReferences"/> are used.</param>
        /// <param name="compilationOptions">The <see cref="CSharpCompilationOptions"/>.</param>
        public void CodeFix(
            IReadOnlyList<string> before,
            string after,
            string? fixTitle = null,
            AllowCompilationErrors allowCompilationErrors = AllowCompilationErrors.No,
            IEnumerable<string>? suppressWarnings = null,
            IEnumerable<MetadataReference>? metadataReferences = null,
            CSharpCompilationOptions? compilationOptions = null)
        {
            RoslynAssert.CodeFix(
                this.CreateCodeFixProvider(),
                this.expectedDiagnostic,
                before,
                after,
                fixTitle,
                allowCompilationErrors,
                suppressWarnings,
                metadataReferences,
                compilationOptions);
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
        /// <param name="allowCompilationErrors">Specify if compilation errors are accepted in the fixed code. This can be for example syntax errors. Default value is <see cref="AllowCompilationErrors.No"/>.</param>
        /// <param name="suppressWarnings">A collection of <see cref="DiagnosticDescriptor.Id"/> to suppress when analyzing the code. Default is <see langword="null" /> meaning <see cref="RoslynAssert.SuppressedDiagnostics"/> are used.</param>
        /// <param name="metadataReferences">A collection of <see cref="MetadataReference"/> to use when compiling. Default is <see langword="null" /> meaning <see cref="MetadataReferences"/> are used.</param>
        /// <param name="compilationOptions">The <see cref="CSharpCompilationOptions"/>.</param>
        public void CodeFix(
            IReadOnlyList<string> before,
            IReadOnlyList<string> after,
            string? fixTitle = null,
            AllowCompilationErrors allowCompilationErrors = AllowCompilationErrors.No,
            IEnumerable<string>? suppressWarnings = null,
            IEnumerable<MetadataReference>? metadataReferences = null,
            CSharpCompilationOptions? compilationOptions = null)
        {
            RoslynAssert.CodeFix(
                this.CreateCodeFixProvider(),
                this.expectedDiagnostic,
                before,
                after,
                fixTitle,
                allowCompilationErrors,
                suppressWarnings,
                metadataReferences,
                compilationOptions);
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
        /// <param name="allowCompilationErrors">Specify if compilation errors are accepted in the fixed code. This can be for example syntax errors. Default value is <see cref="AllowCompilationErrors.No"/>.</param>
        /// <param name="suppressWarnings">A collection of <see cref="DiagnosticDescriptor.Id"/> to suppress when analyzing the code. Default is <see langword="null" /> meaning <see cref="RoslynAssert.SuppressedDiagnostics"/> are used.</param>
        /// <param name="metadataReferences">A collection of <see cref="MetadataReference"/> to use when compiling. Default is <see langword="null" /> meaning <see cref="MetadataReferences"/> are used.</param>
        /// <param name="compilationOptions">The <see cref="CSharpCompilationOptions"/>.</param>
        public void CodeFix(
            ExpectedDiagnostic expectedDiagnostic,
            string before,
            string after,
            string? fixTitle = null,
            AllowCompilationErrors allowCompilationErrors = AllowCompilationErrors.No,
            IEnumerable<string>? suppressWarnings = null,
            IEnumerable<MetadataReference>? metadataReferences = null,
            CSharpCompilationOptions? compilationOptions = null)
        {
            RoslynAssert.CodeFix(
                this.CreateCodeFixProvider(),
                expectedDiagnostic,
                before,
                after,
                fixTitle,
                allowCompilationErrors,
                suppressWarnings,
                metadataReferences,
                compilationOptions);
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
        /// <param name="allowCompilationErrors">Specify if compilation errors are accepted in the fixed code. This can be for example syntax errors. Default value is <see cref="AllowCompilationErrors.No"/>.</param>
        /// <param name="suppressWarnings">A collection of <see cref="DiagnosticDescriptor.Id"/> to suppress when analyzing the code. Default is <see langword="null" /> meaning <see cref="RoslynAssert.SuppressedDiagnostics"/> are used.</param>
        /// <param name="metadataReferences">A collection of <see cref="MetadataReference"/> to use when compiling. Default is <see langword="null" /> meaning <see cref="MetadataReferences"/> are used.</param>
        /// <param name="compilationOptions">The <see cref="CSharpCompilationOptions"/>.</param>
        public void CodeFix(
            ExpectedDiagnostic expectedDiagnostic,
            IReadOnlyList<string> before,
            string after,
            string? fixTitle = null,
            AllowCompilationErrors allowCompilationErrors = AllowCompilationErrors.No,
            IEnumerable<string>? suppressWarnings = null,
            IEnumerable<MetadataReference>? metadataReferences = null,
            CSharpCompilationOptions? compilationOptions = null)
        {
            RoslynAssert.CodeFix(
                this.CreateCodeFixProvider(),
                expectedDiagnostic,
                before,
                after,
                fixTitle,
                allowCompilationErrors,
                suppressWarnings,
                metadataReferences,
                compilationOptions);
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
        /// <param name="allowCompilationErrors">Specify if compilation errors are accepted in the fixed code. This can be for example syntax errors. Default value is <see cref="AllowCompilationErrors.No"/>.</param>
        /// <param name="suppressWarnings">A collection of <see cref="DiagnosticDescriptor.Id"/> to suppress when analyzing the code. Default is <see langword="null" /> meaning <see cref="RoslynAssert.SuppressedDiagnostics"/> are used.</param>
        /// <param name="metadataReferences">A collection of <see cref="MetadataReference"/> to use when compiling. Default is <see langword="null" /> meaning <see cref="MetadataReferences"/> are used.</param>
        /// <param name="compilationOptions">The <see cref="CSharpCompilationOptions"/>.</param>
        public void CodeFix(
            ExpectedDiagnostic expectedDiagnostic,
            IReadOnlyList<string> before,
            IReadOnlyList<string> after,
            string? fixTitle = null,
            AllowCompilationErrors allowCompilationErrors = AllowCompilationErrors.No,
            IEnumerable<string>? suppressWarnings = null,
            IEnumerable<MetadataReference>? metadataReferences = null,
            CSharpCompilationOptions? compilationOptions = null)
        {
            RoslynAssert.CodeFix(
                this.CreateCodeFixProvider(),
                expectedDiagnostic,
                before,
                after,
                fixTitle,
                allowCompilationErrors,
                suppressWarnings,
                metadataReferences,
                compilationOptions);
        }

        /// <summary>
        /// Verifies that
        /// 1. <paramref name="before"/> produces the expected diagnostics
        /// 2. The code fix fixes the code.
        /// </summary>
        /// <param name="before">The code to analyze for the current expected diagnostic. Indicate error position with ↓ (alt + 25).</param>
        /// <param name="after">The expected code produced by applying the current code fix.</param>
        /// <param name="fixTitle">The expected title of the fix. Must be provided if more than one code action is registered.</param>
        /// <param name="allowCompilationErrors">Specify if compilation errors are accepted in the fixed code. This can be for example syntax errors. Default value is <see cref="AllowCompilationErrors.No"/>.</param>
        /// <param name="suppressWarnings">A collection of <see cref="DiagnosticDescriptor.Id"/> to suppress when analyzing the code. Default is <see langword="null" /> meaning <see cref="RoslynAssert.SuppressedDiagnostics"/> are used.</param>
        /// <param name="metadataReferences">A collection of <see cref="MetadataReference"/> to use when compiling. Default is <see langword="null" /> meaning <see cref="MetadataReferences"/> are used.</param>
        /// <param name="compilationOptions">The <see cref="CSharpCompilationOptions"/>.</param>
        public void FixAll(
            IReadOnlyList<string> before,
            string after,
            string? fixTitle = null,
            AllowCompilationErrors allowCompilationErrors = AllowCompilationErrors.No,
            IEnumerable<string>? suppressWarnings = null,
            IEnumerable<MetadataReference>? metadataReferences = null,
            CSharpCompilationOptions? compilationOptions = null)
        {
            RoslynAssert.FixAll(
                this.CreateCodeFixProvider(),
                this.expectedDiagnostic,
                before,
                after,
                fixTitle,
                allowCompilationErrors,
                suppressWarnings,
                metadataReferences,
                compilationOptions);
        }

        /// <summary>
        /// Verifies that
        /// 1. <paramref name="before"/> produces the expected diagnostics
        /// 2. The code fix fixes the code.
        /// </summary>
        /// <param name="before">The code to analyze for the current expected diagnostic. Indicate error position with ↓ (alt + 25).</param>
        /// <param name="after">The expected code produced by applying the current code fix.</param>
        /// <param name="fixTitle">The expected title of the fix. Must be provided if more than one code action is registered.</param>
        /// <param name="allowCompilationErrors">Specify if compilation errors are accepted in the fixed code. This can be for example syntax errors. Default value is <see cref="AllowCompilationErrors.No"/>.</param>
        /// <param name="suppressWarnings">A collection of <see cref="DiagnosticDescriptor.Id"/> to suppress when analyzing the code. Default is <see langword="null" /> meaning <see cref="RoslynAssert.SuppressedDiagnostics"/> are used.</param>
        /// <param name="metadataReferences">A collection of <see cref="MetadataReference"/> to use when compiling. Default is <see langword="null" /> meaning <see cref="MetadataReferences"/> are used.</param>
        /// <param name="compilationOptions">The <see cref="CSharpCompilationOptions"/>.</param>
        public void FixAll(
            IReadOnlyList<string> before,
            IReadOnlyList<string> after,
            string? fixTitle = null,
            AllowCompilationErrors allowCompilationErrors = AllowCompilationErrors.No,
            IEnumerable<string>? suppressWarnings = null,
            IEnumerable<MetadataReference>? metadataReferences = null,
            CSharpCompilationOptions? compilationOptions = null)
        {
            RoslynAssert.FixAll(
                this.CreateCodeFixProvider(),
                this.expectedDiagnostic,
                before,
                after,
                fixTitle,
                allowCompilationErrors,
                suppressWarnings,
                metadataReferences,
                compilationOptions);
        }

        /// <summary>
        /// Constructs the <see cref="CodeFixProvider"/> to use in asserts.
        /// </summary>
        private CodeFixProvider CreateCodeFixProvider() => this.createCodeFixProvider();
    }
}
