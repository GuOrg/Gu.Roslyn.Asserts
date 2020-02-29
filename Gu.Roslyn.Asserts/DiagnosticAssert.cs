namespace Gu.Roslyn.Asserts
{
    using System;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.Diagnostics;

    /// <summary>
    /// Provides assertions against the specified diagnostic analyzer. Use <see
    /// cref="RoslynAssert.Create{TDiagnosticAnalyzer}"/> to obtain an instance.
    /// </summary>
    public sealed class DiagnosticAssert
    {
        private readonly Func<DiagnosticAnalyzer> createAnalyzer;
        private readonly DiagnosticDescriptor? descriptor;

        /// <summary>
        /// Initializes a new instance of the <see cref="DiagnosticAssert"/> class. Use <see
        /// cref="RoslynAssert.Create{TDiagnosticAnalyzer}"/> to obtain an instance.
        /// </summary>
        /// <param name="createAnalyzer">
        /// Constructs the <see cref="DiagnosticAnalyzer"/> to use in asserts.
        /// </param>
        /// <param name="descriptor">
        /// The <see cref="DiagnosticDescriptor"/> with information about the expected <see cref="Diagnostic"/>. If the
        /// analyzer supports more than one <see cref="DiagnosticDescriptor.Id"/>, this must be provided.
        /// </param>
        public DiagnosticAssert(Func<DiagnosticAnalyzer> createAnalyzer, DiagnosticDescriptor? descriptor = null)
        {
            this.createAnalyzer = createAnalyzer ?? throw new ArgumentNullException(nameof(createAnalyzer));
            this.descriptor = descriptor;
        }

        /// <summary>
        /// Verifies that <paramref name="code"/> produces no diagnostics when analyzed with the current analyzer.
        /// </summary>
        /// <param name="code">
        /// The code to analyze using the current analyzer. Analyzing the code is expected to produce no errors or
        /// warnings.
        /// </param>
        public void Valid(params string[] code)
        {
            if (this.descriptor is null)
            {
                RoslynAssert.Valid(this.createAnalyzer(), code);
            }
            else
            {
                RoslynAssert.Valid(this.createAnalyzer(), this.descriptor, code);
            }
        }
    }
}
