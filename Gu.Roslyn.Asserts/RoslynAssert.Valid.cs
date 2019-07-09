namespace Gu.Roslyn.Asserts
{
    using System;
    using System.Collections.Generic;
    using System.Collections.Immutable;
    using System.IO;
    using System.Linq;
    using Gu.Roslyn.Asserts.Internals;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.Diagnostics;

    public static partial class RoslynAssert
    {
        /// <summary>
        /// Verifies that <paramref name="code"/> produces no diagnostics when analyzed with <paramref name="analyzer"/>.
        /// </summary>
        /// <param name="analyzer">The <see cref="DiagnosticAnalyzer"/> to check <paramref name="code"/> with.</param>
        /// <param name="code">The code to analyze.</param>
        public static void Valid(DiagnosticAnalyzer analyzer, params string[] code)
        {
            var sln = CodeFactory.CreateSolution(code, CodeFactory.DefaultCompilationOptions(analyzer, SuppressedDiagnostics), MetadataReferences);
            var diagnosticsAndErrors = Analyze.GetDiagnosticsAndErrors(analyzer, sln);
            NoDiagnosticsOrErrors(diagnosticsAndErrors);
        }

        /// <summary>
        /// Verifies that <paramref name="code"/> produces no diagnostics when analyzed with <paramref name="analyzer"/>.
        /// </summary>
        /// <param name="analyzer">The <see cref="DiagnosticAnalyzer"/> to check <paramref name="code"/> with.</param>
        /// <param name="code">The code to analyze.</param>
        public static void Valid(DiagnosticAnalyzer analyzer, IReadOnlyList<string> code)
        {
            var sln = CodeFactory.CreateSolution(code, CodeFactory.DefaultCompilationOptions(analyzer, SuppressedDiagnostics), MetadataReferences);
            var diagnosticsAndErrors = Analyze.GetDiagnosticsAndErrors(analyzer, sln);
            NoDiagnosticsOrErrors(diagnosticsAndErrors);
        }

        /// <summary>
        /// Verifies that <paramref name="code"/> produces no diagnostics when analyzed with <paramref name="analyzerType"/>.
        /// </summary>
        /// <param name="analyzerType">The type of <see cref="T:Microsoft.CodeAnalysis.Diagnostics.DiagnosticAnalyzer"/> to check <paramref name="code"/> with.</param>
        /// <param name="descriptor">The expected diagnostic.</param>
        /// <param name="code">The code to analyze.</param>
        public static void Valid(Type analyzerType, DiagnosticDescriptor descriptor, params string[] code)
        {
            Valid((DiagnosticAnalyzer)Activator.CreateInstance(analyzerType), descriptor, code);
        }

        /// <summary>
        /// Verifies that <paramref name="code"/> produces no diagnostics when analyzed with <paramref name="analyzer"/>.
        /// </summary>
        /// <param name="analyzer">The <see cref="DiagnosticAnalyzer"/> to check <paramref name="code"/> with.</param>
        /// <param name="descriptor">The expected diagnostic.</param>
        /// <param name="code">The code to analyze.</param>
        public static void Valid(DiagnosticAnalyzer analyzer, DiagnosticDescriptor descriptor, params string[] code)
        {
            VerifyAnalyzerSupportsDiagnostic(analyzer, descriptor);
            var sln = CodeFactory.CreateSolution(code, CodeFactory.DefaultCompilationOptions(analyzer, descriptor, SuppressedDiagnostics), MetadataReferences);
            var diagnosticsAndErrors = Analyze.GetDiagnosticsAndErrors(analyzer, sln);
            NoDiagnosticsOrErrors(diagnosticsAndErrors);
        }

        /// <summary>
        /// Verifies that <paramref name="code"/> produces no diagnostics when analyzed with <paramref name="analyzer"/>.
        /// </summary>
        /// <param name="analyzer">The <see cref="DiagnosticAnalyzer"/> to check <paramref name="code"/> with.</param>
        /// <param name="descriptors">The expected diagnostic.</param>
        /// <param name="code">The code to analyze.</param>
        public static void Valid(DiagnosticAnalyzer analyzer, IReadOnlyList<DiagnosticDescriptor> descriptors, params string[] code)
        {
            VerifyAnalyzerSupportsDiagnostics(analyzer, descriptors);
            var sln = CodeFactory.CreateSolution(code, CodeFactory.DefaultCompilationOptions(analyzer, descriptors, SuppressedDiagnostics), MetadataReferences);
            var diagnosticsAndErrors = Analyze.GetDiagnosticsAndErrors(analyzer, sln);
            NoDiagnosticsOrErrors(diagnosticsAndErrors);
        }

        /// <summary>
        /// Verifies that <paramref name="code"/> produces no diagnostics when analyzed with <paramref name="analyzer"/>.
        /// </summary>
        /// <param name="analyzer">The <see cref="DiagnosticAnalyzer"/> to check <paramref name="code"/> with.</param>
        /// <param name="descriptor">The expected diagnostic.</param>
        /// <param name="code">The code to analyze.</param>
        public static void Valid(DiagnosticAnalyzer analyzer, DiagnosticDescriptor descriptor, IReadOnlyList<string> code)
        {
            VerifyAnalyzerSupportsDiagnostic(analyzer, descriptor);
            var sln = CodeFactory.CreateSolution(code, CodeFactory.DefaultCompilationOptions(analyzer, descriptor, SuppressedDiagnostics), MetadataReferences);
            var diagnosticsAndErrors = Analyze.GetDiagnosticsAndErrors(analyzer, sln);
            NoDiagnosticsOrErrors(diagnosticsAndErrors);
        }

        /// <summary>
        /// Verifies that <paramref name="code"/> produces no diagnostics when analyzed with <paramref name="analyzer"/>.
        /// </summary>
        /// <param name="analyzer">The <see cref="DiagnosticAnalyzer"/> to check <paramref name="code"/> with.</param>
        /// <param name="descriptor">The expected diagnostic.</param>
        /// <param name="code">
        /// The code to create the solution from.
        /// Can be a .cs, .csproj or .sln file.
        /// </param>
        public static void Valid(DiagnosticAnalyzer analyzer, DiagnosticDescriptor descriptor, FileInfo code)
        {
            VerifyAnalyzerSupportsDiagnostic(analyzer, descriptor);
            var sln = CodeFactory.CreateSolution(code, CodeFactory.DefaultCompilationOptions(analyzer, descriptor, SuppressedDiagnostics), MetadataReferences);
            var diagnosticsAndErrors = Analyze.GetDiagnosticsAndErrors(analyzer, sln);
            NoDiagnosticsOrErrors(diagnosticsAndErrors);
        }

        /// <summary>
        /// Verifies that <paramref name="solution"/> produces no diagnostics when analyzed with <paramref name="analyzer"/>.
        /// </summary>
        /// <param name="analyzer">The <see cref="DiagnosticAnalyzer"/> to check <paramref name="solution"/> with.</param>
        /// <param name="solution">The <see cref="Solution"/> for which no errors or warnings are expected.</param>
        public static void Valid(DiagnosticAnalyzer analyzer, Solution solution)
        {
            var diagnosticsAndErrors = Analyze.GetDiagnosticsAndErrors(analyzer, solution);
            NoDiagnosticsOrErrors(diagnosticsAndErrors);
        }

        /// <summary>
        /// Verifies that <paramref name="code"/> produces no diagnostics when analyzed with <paramref name="analyzer"/>.
        /// </summary>
        /// <param name="analyzer">The <see cref="DiagnosticAnalyzer"/> to check <paramref name="code"/> with.</param>
        /// <param name="code">
        /// The code to create the solution from.
        /// Can be a .cs, .csproj or .sln file.
        /// </param>
        /// <param name="compilationOptions">The <see cref="CSharpCompilationOptions"/> to use.</param>
        /// <param name="metadataReferences">The metadata references to use when compiling.</param>
        public static void Valid(DiagnosticAnalyzer analyzer, FileInfo code, CSharpCompilationOptions compilationOptions = null, IEnumerable<MetadataReference> metadataReferences = null)
        {
            var sln = CodeFactory.CreateSolution(
                code,
                compilationOptions ?? CodeFactory.DefaultCompilationOptions(analyzer, SuppressedDiagnostics),
                metadataReferences ?? MetadataReferences);
            var diagnosticsAndErrors = Analyze.GetDiagnosticsAndErrors(analyzer, sln);
            NoDiagnosticsOrErrors(diagnosticsAndErrors);
        }

        /// <summary>
        /// Verifies that <paramref name="code"/> produces no diagnostics when analyzed with <paramref name="analyzer"/>.
        /// </summary>
        /// <param name="analyzer">The <see cref="DiagnosticAnalyzer"/> to check <paramref name="code"/> with.</param>
        /// <param name="code">The code to analyze.</param>
        /// <param name="compilationOptions">The <see cref="CSharpCompilationOptions"/> to use.</param>
        /// <param name="metadataReferences">The metadata references to use when compiling.</param>
        public static void Valid(DiagnosticAnalyzer analyzer, string code, CSharpCompilationOptions compilationOptions = null, IEnumerable<MetadataReference> metadataReferences = null)
        {
            var sln = CodeFactory.CreateSolution(
                code,
                compilationOptions ?? CodeFactory.DefaultCompilationOptions(analyzer, SuppressedDiagnostics),
                metadataReferences ?? MetadataReferences);
            var diagnosticsAndErrors = Analyze.GetDiagnosticsAndErrors(analyzer, sln);
            NoDiagnosticsOrErrors(diagnosticsAndErrors);
        }

        /// <summary>
        /// Verifies that <paramref name="code"/> produces no diagnostics when analyzed with <paramref name="analyzer"/>.
        /// </summary>
        /// <param name="analyzer">The <see cref="DiagnosticAnalyzer"/> to check <paramref name="code"/> with.</param>
        /// <param name="code">The code to analyze.</param>
        /// <param name="compilationOptions">The <see cref="CSharpCompilationOptions"/> to use.</param>
        /// <param name="metadataReferences">The metadata references to use when compiling.</param>
        public static void Valid(DiagnosticAnalyzer analyzer, IReadOnlyList<string> code, CSharpCompilationOptions compilationOptions, IEnumerable<MetadataReference> metadataReferences)
        {
            var sln = CodeFactory.CreateSolution(code, compilationOptions, metadataReferences);
            var diagnosticsAndErrors = Analyze.GetDiagnosticsAndErrors(analyzer, sln);
            NoDiagnosticsOrErrors(diagnosticsAndErrors);
        }

        /// <summary>
        /// Assert that <paramref name="diagnostics"/> is empty. Throw an AssertException with details if not.
        /// </summary>
        /// <param name="diagnostics">The diagnostics.</param>
        public static void NoDiagnostics(IReadOnlyList<ImmutableArray<Diagnostic>> diagnostics)
        {
            if (diagnostics.Any(x => x.Any()))
            {
                var builder = StringBuilderPool.Borrow().AppendLine("Expected no diagnostics, found:");
                foreach (var diagnostic in diagnostics.SelectMany(x => x))
                {
                    builder.AppendLine(diagnostic.ToErrorString());
                }

                throw new AssertException(builder.Return());
            }
        }

        /// <summary>
        /// Verifies that <paramref name="code"/> produces no diagnostics when analyzed with <paramref name="analyzerType"/>.
        /// </summary>
        /// <param name="analyzerType">The type of <see cref="T:Microsoft.CodeAnalysis.Diagnostics.DiagnosticAnalyzer"/> to check <paramref name="code"/> with.</param>
        /// <param name="code">The code to analyze.</param>
        public static void Valid(Type analyzerType, params string[] code)
        {
            Valid((DiagnosticAnalyzer)Activator.CreateInstance(analyzerType), code);
        }

        /// <summary>
        /// Verifies that <paramref name="solution"/> produces no diagnostics when analyzed with <paramref name="analyzerType"/>.
        /// </summary>
        /// <param name="analyzerType">The type of <see cref="T:Microsoft.CodeAnalysis.Diagnostics.DiagnosticAnalyzer"/> to check <paramref name="solution"/> with.</param>
        /// <param name="solution">The <see cref="Solution"/> for which no errors or warnings are expected.</param>
        public static void Valid(Type analyzerType, Solution solution)
        {
            Valid((DiagnosticAnalyzer)Activator.CreateInstance(analyzerType), solution);
        }

        /// <summary>
        /// Verifies that <paramref name="code"/> produces no diagnostics when analyzed with <paramref name="analyzerType"/>.
        /// </summary>
        /// <param name="analyzerType">The type of <see cref="T:Microsoft.CodeAnalysis.Diagnostics.DiagnosticAnalyzer"/> to check <paramref name="code"/> with.</param>
        /// <param name="descriptor">The expected diagnostic.</param>
        /// <param name="code">
        /// The code to create the solution from.
        /// Can be a .cs, .csproj or .sln file.
        /// </param>
        public static void Valid(Type analyzerType, DiagnosticDescriptor descriptor, FileInfo code)
        {
            Valid((DiagnosticAnalyzer)Activator.CreateInstance(analyzerType), descriptor, code);
        }

        /// <summary>
        /// Verifies that <paramref name="code"/> produces no diagnostics when analyzed with <paramref name="analyzerType"/>.
        /// </summary>
        /// <param name="analyzerType">The type of <see cref="T:Microsoft.CodeAnalysis.Diagnostics.DiagnosticAnalyzer"/> to check <paramref name="code"/> with.</param>
        /// <param name="descriptors">The expected diagnostic.</param>
        /// <param name="code">The code to analyze.</param>
        public static void Valid(Type analyzerType, IReadOnlyList<DiagnosticDescriptor> descriptors, params string[] code)
        {
            Valid((DiagnosticAnalyzer)Activator.CreateInstance(analyzerType), descriptors, code);
        }

        /// <summary>
        /// Verifies that <paramref name="code"/> produces no diagnostics when analyzed with <paramref name="analyzerType"/>.
        /// </summary>
        /// <param name="analyzerType">The type of <see cref="T:Microsoft.CodeAnalysis.Diagnostics.DiagnosticAnalyzer"/> to check <paramref name="code"/> with.</param>
        /// <param name="code">
        /// The code to create the solution from.
        /// Can be a .cs, .csproj or .sln file.
        /// </param>
        public static void Valid(Type analyzerType, FileInfo code)
        {
            Valid((DiagnosticAnalyzer)Activator.CreateInstance(analyzerType), code);
        }

        private static void NoDiagnosticsOrErrors(Analyze.DiagnosticsAndErrors diagnosticsAndErrors)
        {
            NoDiagnostics(diagnosticsAndErrors.AnalyzerDiagnostics);
            NoCompilerErrors(diagnosticsAndErrors.Errors, DiagnosticSettings.AllowedErrorIds(), DiagnosticSettings.AllowedDiagnostics());
        }
    }
}
