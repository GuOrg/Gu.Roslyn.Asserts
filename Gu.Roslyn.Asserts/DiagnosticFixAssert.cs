namespace Gu.Roslyn.Asserts
{
    using System;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CodeFixes;
    using Microsoft.CodeAnalysis.Diagnostics;

    /// <summary>
    /// Provides assertions against the specified diagnostic analyzer and code fix provider. Use <see
    /// cref="RoslynAssert.Create{TDiagnosticAnalyzer, TCodeFixProvider}"/> to obtain an instance.
    /// </summary>
    public sealed class DiagnosticFixAssert : DiagnosticAssert
    {
        private readonly Func<CodeFixProvider> createCodeFixProvider;
        private readonly ExpectedDiagnostic? expectedDiagnostic;

        /// <summary>
        /// Initializes a new instance of the <see cref="DiagnosticFixAssert"/> class. Use <see
        /// cref="RoslynAssert.Create{TDiagnosticAnalyzer, TCodeFixProvider}"/> to obtain an instance.
        /// </summary>
        /// <param name="createAnalyzer">
        /// Constructs the <see cref="DiagnosticAnalyzer"/> to use in asserts.
        /// </param>
        /// <param name="createCodeFixProvider">
        /// Constructs the <see cref="CodeFixProvider"/> to use in asserts.
        /// </param>
        /// <param name="expectedDiagnostic">
        /// The <see cref="ExpectedDiagnostic"/> with information about the expected <see cref="Diagnostic"/>. If the
        /// analyzer supports more than one <see cref="DiagnosticDescriptor.Id"/>, this must be provided.
        /// </param>
        internal DiagnosticFixAssert(Func<DiagnosticAnalyzer> createAnalyzer, Func<CodeFixProvider> createCodeFixProvider, ExpectedDiagnostic? expectedDiagnostic = null)
            : base(createAnalyzer, expectedDiagnostic?.Id)
        {
            this.createCodeFixProvider = createCodeFixProvider ?? throw new ArgumentNullException(nameof(createCodeFixProvider));
            this.expectedDiagnostic = expectedDiagnostic;
        }

        /// <summary>
        /// Constructs the <see cref="CodeFixProvider"/> to use in asserts.
        /// </summary>
        private CodeFixProvider CreateCodeFixProvider() => this.createCodeFixProvider();

        /// <summary>
        /// Verifies that
        /// 1. <paramref name="code"/> produces the expected diagnostics
        /// 2. The code fix does not change the code.
        /// </summary>
        /// <param name="code">
        /// The code to analyze with the current analyzer. Indicate error position with ↓ (alt + 25).
        /// </param>
        public void NoFix(params string[] code)
        {
            if (this.expectedDiagnostic is null)
            {
                throw new InvalidOperationException("Either pass an ExpectedDiagnostic instance to RoslynAssert.Create or to the NoFix overload that accepts one.");
            }

            this.NoFix(this.expectedDiagnostic, code);
        }

        /// <summary>
        /// Verifies that
        /// 1. <paramref name="code"/> produces the expected diagnostics
        /// 2. The code fix does not change the code.
        /// </summary>
        /// <param name="expectedDiagnostic">
        /// The <see cref="ExpectedDiagnostic"/> with information about the expected <see cref="Diagnostic"/>. If the
        /// current analyzer supports more than one <see cref="DiagnosticDescriptor.Id"/> this must be provided.
        /// </param>
        /// <param name="code">
        /// The code to analyze with the current analyzer. Indicate error position with ↓ (alt + 25).
        /// </param>
        public void NoFix(ExpectedDiagnostic expectedDiagnostic, params string[] code)
        {
            RoslynAssert.NoFix(this.CreateAnalyzer(), this.CreateCodeFixProvider(), expectedDiagnostic, code);
        }
    }
}
