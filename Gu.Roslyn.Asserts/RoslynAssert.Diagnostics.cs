namespace Gu.Roslyn.Asserts
{
    using System.Collections.Generic;
    using System.Collections.Immutable;
    using System.Globalization;
    using System.Linq;
    using Gu.Roslyn.Asserts.Internals;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.Diagnostics;

    public static partial class RoslynAssert
    {
        /// <summary>
        /// Verifies that <paramref name="code"/> produces the expected diagnostics.
        /// </summary>
        /// <param name="analyzer">The <see cref="DiagnosticAnalyzer"/> to check <paramref name="code"/> with.</param>
        /// <param name="code">The code with error positions indicated.</param>
        public static void Diagnostics(DiagnosticAnalyzer analyzer, params string[] code)
        {
            Diagnostics(
                analyzer,
                DiagnosticsAndSources.CreateFromCodeWithErrorsIndicated(analyzer, code),
                allowCompilationErrors: AllowCompilationErrors.No,
                suppressedDiagnostics: null,
                metadataReferences: null,
                compilationOptions: null);
        }

        /// <summary>
        /// Verifies that <paramref name="code"/> produces the expected diagnostics.
        /// </summary>
        /// <param name="analyzer">The <see cref="DiagnosticAnalyzer"/> to check <paramref name="code"/> with.</param>
        /// <param name="expectedDiagnostic">The <see cref="ExpectedDiagnostic"/> with information about the expected <see cref="Diagnostic"/>. If <paramref name="analyzer"/> supports more than one <see cref="DiagnosticDescriptor.Id"/> this must be provided.</param>
        /// <param name="code">The code to analyze.</param>
        public static void Diagnostics(DiagnosticAnalyzer analyzer, ExpectedDiagnostic expectedDiagnostic, params string[] code)
        {
            Diagnostics(
                analyzer,
                DiagnosticsAndSources.Create(expectedDiagnostic, code),
                allowCompilationErrors: AllowCompilationErrors.No,
                suppressedDiagnostics: null,
                metadataReferences: null,
                compilationOptions: null);
        }

        /// <summary>
        /// Verifies that <paramref name="code"/> produces the expected diagnostics.
        /// </summary>
        /// <param name="analyzer">The <see cref="DiagnosticAnalyzer"/> to check <paramref name="code"/> with.</param>
        /// <param name="expectedDiagnostics">The expected diagnostics.</param>
        /// <param name="code">The code to analyze.</param>
        public static void Diagnostics(DiagnosticAnalyzer analyzer, IReadOnlyList<ExpectedDiagnostic> expectedDiagnostics, params string[] code)
        {
            Diagnostics(
                analyzer,
                new DiagnosticsAndSources(expectedDiagnostics, code),
                allowCompilationErrors: AllowCompilationErrors.No,
                suppressedDiagnostics: null,
                metadataReferences: null,
                compilationOptions: null);
        }

        /// <summary>
        /// Verifies that <paramref name="code"/> produces the expected diagnostics.
        /// </summary>
        /// <param name="analyzer">The <see cref="DiagnosticAnalyzer"/> to check <paramref name="code"/> with.</param>
        /// <param name="code">The code with error positions indicated.</param>
        /// <param name="allowCompilationErrors">Specify if compilation errors are accepted in the fixed code. This can be for example syntax errors. Default value is <see cref="AllowCompilationErrors.No"/>.</param>
        /// <param name="suppressedDiagnostics">A collection of <see cref="DiagnosticDescriptor.Id"/> to suppress when analyzing the code. Default is <see langword="null" /> meaning no warnings or errors are suppressed.</param>
        /// <param name="metadataReferences">Collection of <see cref="MetadataReference"/> to use when compiling.</param>
        /// <param name="compilationOptions">The <see cref="CSharpCompilationOptions"/>.</param>
        public static void Diagnostics(
            DiagnosticAnalyzer analyzer,
            string code,
            AllowCompilationErrors allowCompilationErrors = AllowCompilationErrors.No,
            IEnumerable<string> suppressedDiagnostics = null,
            IEnumerable<MetadataReference> metadataReferences = null,
            CSharpCompilationOptions compilationOptions = null)
        {
            Diagnostics(
                analyzer,
                DiagnosticsAndSources.CreateFromCodeWithErrorsIndicated(analyzer, code),
                allowCompilationErrors: allowCompilationErrors,
                suppressedDiagnostics: suppressedDiagnostics,
                metadataReferences: metadataReferences,
                compilationOptions: compilationOptions);
        }

        /// <summary>
        /// Verifies that <paramref name="code"/> produces the expected diagnostics.
        /// </summary>
        /// <param name="analyzer">The <see cref="DiagnosticAnalyzer"/> to check <paramref name="code"/> with.</param>
        /// <param name="code">The code with error positions indicated.</param>
         /// <param name="allowCompilationErrors">Specify if compilation errors are accepted in the fixed code. This can be for example syntax errors. Default value is <see cref="AllowCompilationErrors.No"/>.</param>
        /// <param name="suppressedDiagnostics">A collection of <see cref="DiagnosticDescriptor.Id"/> to suppress when analyzing the code. Default is <see langword="null" /> meaning no warnings or errors are suppressed.</param>
        /// <param name="metadataReferences">Collection of <see cref="MetadataReference"/> to use when compiling.</param>
        /// <param name="compilationOptions">The <see cref="CSharpCompilationOptions"/>.</param>
        public static void Diagnostics(
            DiagnosticAnalyzer analyzer,
            IReadOnlyList<string> code,
            AllowCompilationErrors allowCompilationErrors = AllowCompilationErrors.No,
            IEnumerable<string> suppressedDiagnostics = null,
            IEnumerable<MetadataReference> metadataReferences = null,
            CSharpCompilationOptions compilationOptions = null)
        {
            Diagnostics(
                analyzer,
                DiagnosticsAndSources.CreateFromCodeWithErrorsIndicated(analyzer, code),
                allowCompilationErrors: allowCompilationErrors,
                suppressedDiagnostics: suppressedDiagnostics,
                metadataReferences: metadataReferences,
                compilationOptions: compilationOptions);
        }

        /// <summary>
        /// Verifies that <paramref name="diagnosticsAndSources"/> produces the expected diagnostics.
        /// </summary>
        /// <param name="analyzer">The <see cref="DiagnosticAnalyzer"/> to check <paramref name="diagnosticsAndSources"/> with.</param>
        /// <param name="diagnosticsAndSources">The code to analyze.</param>
        /// <param name="allowCompilationErrors">Specify if compilation errors are accepted in the fixed code. This can be for example syntax errors. Default value is <see cref="AllowCompilationErrors.No"/>.</param>
        /// <param name="suppressedDiagnostics">A collection of <see cref="DiagnosticDescriptor.Id"/> to suppress when analyzing the code. Default is <see langword="null" /> meaning no warnings or errors are suppressed.</param>
        /// <param name="metadataReferences">Collection of <see cref="MetadataReference"/> to use when compiling.</param>
        /// <param name="compilationOptions">The <see cref="CSharpCompilationOptions"/>.</param>
        public static void Diagnostics(
            DiagnosticAnalyzer analyzer,
            DiagnosticsAndSources diagnosticsAndSources,
            AllowCompilationErrors allowCompilationErrors = AllowCompilationErrors.No,
            IEnumerable<string> suppressedDiagnostics = null,
            IEnumerable<MetadataReference> metadataReferences = null,
            CSharpCompilationOptions compilationOptions = null)
        {
            VerifyAnalyzerSupportsDiagnostics(analyzer, diagnosticsAndSources.ExpectedDiagnostics);
            var sln = CodeFactory.CreateSolution(
                diagnosticsAndSources,
                analyzer,
                compilationOptions,
                suppressedDiagnostics ?? SuppressedDiagnostics,
                metadataReferences ?? MetadataReferences);
            var diagnostics = Analyze.GetDiagnostics(analyzer, sln);
            VerifyDiagnostics(diagnosticsAndSources, diagnostics);
            if (allowCompilationErrors == AllowCompilationErrors.No)
            {
                NoCompilerErrors(sln);
            }
        }

        private static void VerifyDiagnostics(DiagnosticsAndSources diagnosticsAndSources, IReadOnlyList<ImmutableArray<Diagnostic>> actuals, string expectedMessage = null)
        {
            VerifyDiagnostics(diagnosticsAndSources, actuals.SelectMany(x => x).ToArray(), expectedMessage);
        }

        private static void VerifyDiagnostics(DiagnosticsAndSources diagnosticsAndSources, IReadOnlyList<Diagnostic> actuals, string expectedMessage = null)
        {
            if (diagnosticsAndSources.ExpectedDiagnostics.Count == 0)
            {
                throw new AssertException("Expected code to have at least one error position indicated with '↓'");
            }

            if (diagnosticsAndSources.ExpectedDiagnostics.SetEquals(actuals))
            {
                if (expectedMessage != null)
                {
                    foreach (var actual in actuals)
                    {
                        var actualMessage = actual.GetMessage(CultureInfo.InvariantCulture);
                        TextAssert.AreEqual(expectedMessage, actualMessage, $"Expected and actual diagnostic message for the diagnostic {actual} does not match");
                    }
                }

                return;
            }

            var error = StringBuilderPool.Borrow();
            if (actuals.Count == 1 &&
                diagnosticsAndSources.ExpectedDiagnostics.Count == 1 &&
                diagnosticsAndSources.ExpectedDiagnostics[0].Id == actuals[0].Id)
            {
                if (diagnosticsAndSources.ExpectedDiagnostics[0].PositionMatches(actuals[0]) &&
                    !diagnosticsAndSources.ExpectedDiagnostics[0].MessageMatches(actuals[0]))
                {
                    CodeAssert.AreEqual(diagnosticsAndSources.ExpectedDiagnostics[0].Message, actuals[0].GetMessage(CultureInfo.InvariantCulture), "Expected and actual messages do not match.");
                }
            }

            error.AppendLine("Expected and actual diagnostics do not match.");
            var missingExpected = diagnosticsAndSources.ExpectedDiagnostics.Except(actuals);
            for (var i = 0; i < missingExpected.Count; i++)
            {
                if (i == 0)
                {
                    error.Append("Expected:\r\n");
                }

                var expected = missingExpected[i];
                error.AppendLine(expected.ToString(diagnosticsAndSources.Code));
            }

            if (actuals.Count == 0)
            {
                error.AppendLine("Actual: <no errors>");
            }

            var missingActual = actuals.Except(diagnosticsAndSources.ExpectedDiagnostics);
            if (actuals.Count > 0 && missingActual.Count == 0)
            {
                error.AppendLine("Actual: <missing>");
            }

            for (var i = 0; i < missingActual.Count; i++)
            {
                if (i == 0)
                {
                    error.Append("Actual:\r\n");
                }

                var actual = missingActual[i];
                error.AppendLine(actual.ToErrorString());
            }

            throw new AssertException(error.Return());
        }
    }
}
