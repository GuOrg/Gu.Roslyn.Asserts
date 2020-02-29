namespace Gu.Roslyn.Asserts
{
    using System;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.Diagnostics;

    /// <summary>
    /// Provides assertions against the specified diagnostic analyzer. Use <see
    /// cref="RoslynAssert.Create{TDiagnosticAnalyzer}"/> to obtain an instance.
    /// </summary>
    public class DiagnosticAssert
    {
        private readonly Func<DiagnosticAnalyzer> createAnalyzer;
        private readonly string? descriptorId;

        /// <summary>
        /// Initializes a new instance of the <see cref="DiagnosticAssert"/> class. Use <see
        /// cref="RoslynAssert.Create{TDiagnosticAnalyzer}"/> to obtain an instance.
        /// </summary>
        /// <param name="createAnalyzer">
        /// Constructs the <see cref="DiagnosticAnalyzer"/> to use in asserts.
        /// </param>
        /// <param name="descriptorId">
        /// The ID of the expected <see cref="Diagnostic"/>. If the analyzer supports more than one <see
        /// cref="DiagnosticDescriptor.Id"/>, this must be provided.
        /// </param>
        internal DiagnosticAssert(Func<DiagnosticAnalyzer> createAnalyzer, string? descriptorId = null)
        {
            this.createAnalyzer = createAnalyzer ?? throw new ArgumentNullException(nameof(createAnalyzer));
            this.descriptorId = descriptorId;
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
            var analyzer = this.CreateAnalyzer();

            if (this.descriptorId is null)
            {
                RoslynAssert.Valid(analyzer, code);
            }
            else
            {
                RoslynAssert.VerifyAnalyzerSupportsDiagnostic(analyzer, this.descriptorId);
                RoslynAssert.Valid(
                    analyzer,
#pragma warning disable CS0618 // Suppress until removed. Will be replaced with Metadatareferences.FromAttributes()
                    CodeFactory.CreateSolution(code, CodeFactory.DefaultCompilationOptions(analyzer, this.descriptorId, RoslynAssert.SuppressedDiagnostics), RoslynAssert.MetadataReferences));
#pragma warning restore CS0618 // Suppress until removed. Will be replaced with Metadatareferences.FromAttributes()
            }
        }

        /// <summary>
        /// Constructs the <see cref="DiagnosticAnalyzer"/> to use in asserts.
        /// </summary>
        protected DiagnosticAnalyzer CreateAnalyzer() => this.createAnalyzer();
    }
}
