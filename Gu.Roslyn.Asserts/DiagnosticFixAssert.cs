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
    }
}
