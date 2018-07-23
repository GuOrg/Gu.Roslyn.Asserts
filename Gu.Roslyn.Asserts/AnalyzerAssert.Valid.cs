namespace Gu.Roslyn.Asserts
{
    using System;
    using System.Collections.Generic;
    using System.Collections.Immutable;
    using System.IO;
    using System.Linq;
    using System.Threading.Tasks;
    using Gu.Roslyn.Asserts.Internals;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.Diagnostics;

    public static partial class AnalyzerAssert
    {
        /// <summary>
        /// Verifies that <paramref name="code"/> produces no diagnostics when analyzed with <typeparamref name="TAnalyzer"/>.
        /// </summary>
        /// <typeparam name="TAnalyzer">The type of the analyzer.</typeparam>
        /// <param name="code">The code to analyze.</param>
        public static void Valid<TAnalyzer>(params string[] code)
            where TAnalyzer : DiagnosticAnalyzer, new()
        {
            var analyzer = new TAnalyzer();
            var sln = CodeFactory.CreateSolution(code, CodeFactory.DefaultCompilationOptions(analyzer, SuppressedDiagnostics), MetadataReferences);
            var diagnostics = Analyze.GetDiagnostics(analyzer, sln);
            NoDiagnostics(diagnostics);
        }

        /// <summary>
        /// Verifies that <paramref name="code"/> produces no diagnostics when analyzed with <paramref name="analyzerType"/>.
        /// </summary>
        /// <param name="analyzerType">The type of the analyzer.</param>
        /// <param name="code">The code to analyze.</param>
        public static void Valid(Type analyzerType, params string[] code)
        {
            var analyzer = (DiagnosticAnalyzer)Activator.CreateInstance(analyzerType);
            var sln = CodeFactory.CreateSolution(code, CodeFactory.DefaultCompilationOptions(analyzer, SuppressedDiagnostics), MetadataReferences);
            var diagnostics = Analyze.GetDiagnostics(analyzer, sln);
            NoDiagnostics(diagnostics);
        }

        /// <summary>
        /// Verifies that <paramref name="code"/> produces no diagnostics when analyzed with <paramref name="analyzer"/>.
        /// </summary>
        /// <param name="analyzer">The analyzer.</param>
        /// <param name="code">The code to analyze.</param>
        public static void Valid(DiagnosticAnalyzer analyzer, params string[] code)
        {
            var sln = CodeFactory.CreateSolution(code, CodeFactory.DefaultCompilationOptions(analyzer, SuppressedDiagnostics), MetadataReferences);
            var diagnostics = Analyze.GetDiagnostics(analyzer, sln);
            NoDiagnostics(diagnostics);
        }

        /// <summary>
        /// Verifies that <paramref name="code"/> produces no diagnostics when analyzed with <paramref name="analyzer"/>.
        /// </summary>
        /// <param name="analyzer">The analyzer.</param>
        /// <param name="code">The code to analyze.</param>
        public static void Valid(DiagnosticAnalyzer analyzer, IReadOnlyList<string> code)
        {
            var sln = CodeFactory.CreateSolution(code, CodeFactory.DefaultCompilationOptions(analyzer, SuppressedDiagnostics), MetadataReferences);
            var diagnostics = Analyze.GetDiagnostics(analyzer, sln);
            NoDiagnostics(diagnostics);
        }

        /// <summary>
        /// Verifies that <paramref name="code"/> produces no diagnostics when analyzed with <paramref name="analyzer"/>.
        /// </summary>
        /// <param name="analyzer">The analyzer.</param>
        /// <param name="code">The code to analyze.</param>
        /// <param name="metadataReferences">The metadata references to use when compiling.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        public static async Task ValidAsync(DiagnosticAnalyzer analyzer, IReadOnlyList<string> code, IReadOnlyList<MetadataReference> metadataReferences)
        {
            var sln = CodeFactory.CreateSolution(code, CodeFactory.DefaultCompilationOptions(analyzer, SuppressedDiagnostics), metadataReferences);
            await ValidAsync(analyzer, sln);
        }

        /// <summary>
        /// Verifies that <paramref name="code"/> produces no diagnostics when analyzed with <paramref name="analyzer"/>.
        /// </summary>
        /// <param name="analyzer">The analyzer.</param>
        /// <param name="code">The code to analyze.</param>
        /// <param name="compilationOptions">The <see cref="CSharpCompilationOptions"/> to use.</param>
        /// <param name="metadataReferences">The metadata references to use when compiling.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        public static async Task ValidAsync(DiagnosticAnalyzer analyzer, IReadOnlyList<string> code, CSharpCompilationOptions compilationOptions, IReadOnlyList<MetadataReference> metadataReferences)
        {
            var sln = CodeFactory.CreateSolution(code, CodeFactory.DefaultCompilationOptions(analyzer, SuppressedDiagnostics), metadataReferences);
            await ValidAsync(analyzer, sln);
        }

        /// <summary>
        /// Verifies that <paramref name="code"/> produces no diagnostics when analyzed with <typeparamref name="TAnalyzer"/>.
        /// </summary>
        /// <typeparam name="TAnalyzer">The type of the analyzer.</typeparam>
        /// <param name="code">The code to analyze.</param>
        public static void Valid<TAnalyzer>(FileInfo code)
            where TAnalyzer : DiagnosticAnalyzer, new()
        {
            var analyzer = new TAnalyzer();
            var sln = CodeFactory.CreateSolution(code, CodeFactory.DefaultCompilationOptions(analyzer, SuppressedDiagnostics), MetadataReferences);
            var diagnostics = Analyze.GetDiagnostics(analyzer, sln);
            NoDiagnostics(diagnostics);
        }

        /// <summary>
        /// Verifies that <paramref name="code"/> produces no diagnostics when analyzed with <paramref name="analyzerType"/>.
        /// </summary>
        /// <param name="analyzerType">The type of the analyzer.</param>
        /// <param name="code">
        /// The code to create the solution from.
        /// Can be a .cs, .csproj or .sln file
        /// </param>
        public static void Valid(Type analyzerType, FileInfo code)
        {
            var analyzer = (DiagnosticAnalyzer)Activator.CreateInstance(analyzerType);
            var sln = CodeFactory.CreateSolution(code, CodeFactory.DefaultCompilationOptions(analyzer, SuppressedDiagnostics), MetadataReferences);
            var diagnostics = Analyze.GetDiagnostics(analyzer, sln);
            NoDiagnostics(diagnostics);
        }

        /// <summary>
        /// Verifies that <paramref name="code"/> produces no diagnostics when analyzed with <paramref name="analyzer"/>.
        /// </summary>
        /// <param name="analyzer">The analyzer.</param>
        /// <param name="code">
        /// The code to create the solution from.
        /// Can be a .cs, .csproj or .sln file
        /// </param>
        public static void Valid(DiagnosticAnalyzer analyzer, FileInfo code)
        {
            var sln = CodeFactory.CreateSolution(code, CodeFactory.DefaultCompilationOptions(analyzer, SuppressedDiagnostics), MetadataReferences);
            var diagnostics = Analyze.GetDiagnostics(analyzer, sln);
            NoDiagnostics(diagnostics);
        }

        /// <summary>
        /// Verifies that <paramref name="code"/> produces no diagnostics when analyzed with <typeparamref name="TAnalyzer"/>.
        /// </summary>
        /// <typeparam name="TAnalyzer">The type of the analyzer.</typeparam>
        /// <param name="expectedDiagnostic">The expected diagnostic.</param>
        /// <param name="code">The code to analyze.</param>
        public static void Valid<TAnalyzer>(ExpectedDiagnostic expectedDiagnostic, params string[] code)
            where TAnalyzer : DiagnosticAnalyzer, new()
        {
            var analyzer = new TAnalyzer();
            AnalyzerSupportsDiagnostic(analyzer, expectedDiagnostic);
            var sln = CodeFactory.CreateSolution(code, CodeFactory.DefaultCompilationOptions(analyzer, expectedDiagnostic, SuppressedDiagnostics), MetadataReferences);
            var diagnostics = Analyze.GetDiagnostics(analyzer, sln);
            NoDiagnostics(diagnostics);
        }

        /// <summary>
        /// Verifies that <paramref name="code"/> produces no diagnostics when analyzed with <paramref name="analyzerType"/>.
        /// </summary>
        /// <param name="analyzerType">The type of the analyzer.</param>
        /// <param name="expectedDiagnostic">The expected diagnostic.</param>
        /// <param name="code">The code to analyze.</param>
        public static void Valid(Type analyzerType, ExpectedDiagnostic expectedDiagnostic, params string[] code)
        {
            var analyzer = (DiagnosticAnalyzer)Activator.CreateInstance(analyzerType);
            AnalyzerSupportsDiagnostic(analyzer, expectedDiagnostic);
            var sln = CodeFactory.CreateSolution(code, CodeFactory.DefaultCompilationOptions(analyzer, expectedDiagnostic, SuppressedDiagnostics), MetadataReferences);
            var diagnostics = Analyze.GetDiagnostics(analyzer, sln);
            NoDiagnostics(diagnostics);
        }

        /// <summary>
        /// Verifies that <paramref name="code"/> produces no diagnostics when analyzed with <paramref name="analyzer"/>.
        /// </summary>
        /// <param name="analyzer">The analyzer.</param>
        /// <param name="expectedDiagnostic">The expected diagnostic.</param>
        /// <param name="code">The code to analyze.</param>
        public static void Valid(DiagnosticAnalyzer analyzer, ExpectedDiagnostic expectedDiagnostic, params string[] code)
        {
            AnalyzerSupportsDiagnostic(analyzer, expectedDiagnostic);
            var sln = CodeFactory.CreateSolution(code, CodeFactory.DefaultCompilationOptions(analyzer, expectedDiagnostic, SuppressedDiagnostics), MetadataReferences);
            var diagnostics = Analyze.GetDiagnostics(analyzer, sln);
            NoDiagnostics(diagnostics);
        }

        /// <summary>
        /// Verifies that <paramref name="code"/> produces no diagnostics when analyzed with <typeparamref name="TAnalyzer"/>.
        /// </summary>
        /// <typeparam name="TAnalyzer">The type of the analyzer.</typeparam>
        /// <param name="expectedDiagnostics">The expected diagnostic.</param>
        /// <param name="code">The code to analyze.</param>
        public static void Valid<TAnalyzer>(IReadOnlyList<ExpectedDiagnostic> expectedDiagnostics, params string[] code)
            where TAnalyzer : DiagnosticAnalyzer, new()
        {
            var analyzer = new TAnalyzer();
            AnalyzerSupportsDiagnostics(analyzer, expectedDiagnostics);
            var sln = CodeFactory.CreateSolution(code, CodeFactory.DefaultCompilationOptions(analyzer, expectedDiagnostics, SuppressedDiagnostics), MetadataReferences);
            var diagnostics = Analyze.GetDiagnostics(analyzer, sln);
            NoDiagnostics(diagnostics);
        }

        /// <summary>
        /// Verifies that <paramref name="code"/> produces no diagnostics when analyzed with <paramref name="analyzerType"/>.
        /// </summary>
        /// <param name="analyzerType">The type of the analyzer.</param>
        /// <param name="expectedDiagnostics">The expected diagnostic.</param>
        /// <param name="code">The code to analyze.</param>
        public static void Valid(Type analyzerType, IReadOnlyList<ExpectedDiagnostic> expectedDiagnostics, params string[] code)
        {
            var analyzer = (DiagnosticAnalyzer)Activator.CreateInstance(analyzerType);
            AnalyzerSupportsDiagnostics(analyzer, expectedDiagnostics);
            var sln = CodeFactory.CreateSolution(code, CodeFactory.DefaultCompilationOptions(analyzer, expectedDiagnostics, SuppressedDiagnostics), MetadataReferences);
            var diagnostics = Analyze.GetDiagnostics(analyzer, sln);
            NoDiagnostics(diagnostics);
        }

        /// <summary>
        /// Verifies that <paramref name="code"/> produces no diagnostics when analyzed with <paramref name="analyzer"/>.
        /// </summary>
        /// <param name="analyzer">The analyzer.</param>
        /// <param name="expectedDiagnostics">The expected diagnostic.</param>
        /// <param name="code">The code to analyze.</param>
        public static void Valid(DiagnosticAnalyzer analyzer, IReadOnlyList<ExpectedDiagnostic> expectedDiagnostics, params string[] code)
        {
            AnalyzerSupportsDiagnostics(analyzer, expectedDiagnostics);
            var sln = CodeFactory.CreateSolution(code, CodeFactory.DefaultCompilationOptions(analyzer, expectedDiagnostics, SuppressedDiagnostics), MetadataReferences);
            var diagnostics = Analyze.GetDiagnostics(analyzer, sln);
            NoDiagnostics(diagnostics);
        }

        /// <summary>
        /// Verifies that <paramref name="code"/> produces no diagnostics when analyzed with <paramref name="analyzer"/>.
        /// </summary>
        /// <param name="analyzer">The analyzer.</param>
        /// <param name="expectedDiagnostic">The expected diagnostic.</param>
        /// <param name="code">The code to analyze.</param>
        public static void Valid(DiagnosticAnalyzer analyzer, ExpectedDiagnostic expectedDiagnostic, IReadOnlyList<string> code)
        {
            AnalyzerSupportsDiagnostic(analyzer, expectedDiagnostic);
            var sln = CodeFactory.CreateSolution(code, CodeFactory.DefaultCompilationOptions(analyzer, expectedDiagnostic, SuppressedDiagnostics), MetadataReferences);
            var diagnostics = Analyze.GetDiagnostics(analyzer, sln);
            NoDiagnostics(diagnostics);
        }

        /// <summary>
        /// Verifies that <paramref name="code"/> produces no diagnostics when analyzed with <paramref name="analyzer"/>.
        /// </summary>
        /// <param name="analyzer">The analyzer.</param>
        /// <param name="expectedDiagnostic">The expected diagnostic.</param>
        /// <param name="code">The code to analyze.</param>
        /// <param name="metadataReferences">The metadata references to use when compiling.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        public static async Task ValidAsync(DiagnosticAnalyzer analyzer, ExpectedDiagnostic expectedDiagnostic, IReadOnlyList<string> code, IReadOnlyList<MetadataReference> metadataReferences)
        {
            AnalyzerSupportsDiagnostic(analyzer, expectedDiagnostic);
            var sln = CodeFactory.CreateSolution(code, CodeFactory.DefaultCompilationOptions(analyzer, expectedDiagnostic, SuppressedDiagnostics), metadataReferences);
            await ValidAsync(analyzer, sln);
        }

        /// <summary>
        /// Verifies that <paramref name="code"/> produces no diagnostics when analyzed with <typeparamref name="TAnalyzer"/>.
        /// </summary>
        /// <typeparam name="TAnalyzer">The type of the analyzer.</typeparam>
        /// <param name="expectedDiagnostic">The expected diagnostic.</param>
        /// <param name="code">The code to analyze.</param>
        public static void Valid<TAnalyzer>(ExpectedDiagnostic expectedDiagnostic, FileInfo code)
            where TAnalyzer : DiagnosticAnalyzer, new()
        {
            var analyzer = new TAnalyzer();
            AnalyzerSupportsDiagnostic(analyzer, expectedDiagnostic);
            var sln = CodeFactory.CreateSolution(code, CodeFactory.DefaultCompilationOptions(analyzer, expectedDiagnostic, SuppressedDiagnostics), MetadataReferences);
            var diagnostics = Analyze.GetDiagnostics(analyzer, sln);
            NoDiagnostics(diagnostics);
        }

        /// <summary>
        /// Verifies that <paramref name="code"/> produces no diagnostics when analyzed with <paramref name="analyzerType"/>.
        /// </summary>
        /// <param name="analyzerType">The type of the analyzer.</param>
        /// <param name="expectedDiagnostic">The expected diagnostic.</param>
        /// <param name="code">
        /// The code to create the solution from.
        /// Can be a .cs, .csproj or .sln file
        /// </param>
        public static void Valid(Type analyzerType, ExpectedDiagnostic expectedDiagnostic, FileInfo code)
        {
            var analyzer = (DiagnosticAnalyzer)Activator.CreateInstance(analyzerType);
            AnalyzerSupportsDiagnostic(analyzer, expectedDiagnostic);
            var sln = CodeFactory.CreateSolution(code, CodeFactory.DefaultCompilationOptions(analyzer, expectedDiagnostic, SuppressedDiagnostics), MetadataReferences);
            var diagnostics = Analyze.GetDiagnostics(analyzer, sln);
            NoDiagnostics(diagnostics);
        }

        /// <summary>
        /// Verifies that <paramref name="code"/> produces no diagnostics when analyzed with <paramref name="analyzer"/>.
        /// </summary>
        /// <param name="analyzer">The analyzer.</param>
        /// <param name="expectedDiagnostic">The expected diagnostic.</param>
        /// <param name="code">
        /// The code to create the solution from.
        /// Can be a .cs, .csproj or .sln file
        /// </param>
        public static void Valid(DiagnosticAnalyzer analyzer, ExpectedDiagnostic expectedDiagnostic, FileInfo code)
        {
            AnalyzerSupportsDiagnostic(analyzer, expectedDiagnostic);
            var sln = CodeFactory.CreateSolution(code, CodeFactory.DefaultCompilationOptions(analyzer, expectedDiagnostic, SuppressedDiagnostics), MetadataReferences);
            var diagnostics = Analyze.GetDiagnostics(analyzer, sln);
            NoDiagnostics(diagnostics);
        }

        /// <summary>
        /// Verifies that <paramref name="solution"/> produces no diagnostics when analyzed with <typeparamref name="TAnalyzer"/>.
        /// </summary>
        /// <typeparam name="TAnalyzer">The type of the analyzer.</typeparam>
        /// <param name="solution">The <see cref="Solution"/> for which no errors or warnings are expected.</param>
        public static void Valid<TAnalyzer>(Solution solution)
            where TAnalyzer : DiagnosticAnalyzer, new()
        {
            var analyzer = new TAnalyzer();
            var diagnostics = Analyze.GetDiagnostics(analyzer, solution);
            NoDiagnostics(diagnostics);
        }

        /// <summary>
        /// Verifies that <paramref name="solution"/> produces no diagnostics when analyzed with <paramref name="analyzerType"/>.
        /// </summary>
        /// <param name="analyzerType">The type of the analyzer.</param>
        /// <param name="solution">The <see cref="Solution"/> for which no errors or warnings are expected.</param>
        public static void Valid(Type analyzerType, Solution solution)
        {
            var analyzer = (DiagnosticAnalyzer)Activator.CreateInstance(analyzerType);
            var diagnostics = Analyze.GetDiagnostics(analyzer, solution);
            NoDiagnostics(diagnostics);
        }

        /// <summary>
        /// Verifies that <paramref name="solution"/> produces no diagnostics when analyzed with <paramref name="analyzer"/>.
        /// </summary>
        /// <param name="analyzer">The analyzer.</param>
        /// <param name="solution">The <see cref="Solution"/> for which no errors or warnings are expected.</param>
        public static void Valid(DiagnosticAnalyzer analyzer, Solution solution)
        {
            var diagnostics = Analyze.GetDiagnostics(solution, analyzer);
            NoDiagnostics(diagnostics);
        }

        /// <summary>
        /// Verifies that <paramref name="solution"/> produces no diagnostics when analyzed with <paramref name="analyzer"/>.
        /// </summary>
        /// <param name="analyzer">The <see cref="DiagnosticAnalyzer"/>.</param>
        /// <param name="solution">The <see cref="Solution"/> for which no errors or warnings are expected.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        public static async Task ValidAsync(DiagnosticAnalyzer analyzer, Solution solution)
        {
            var diagnostics = await Analyze.GetDiagnosticsAsync(solution, analyzer)
                                           .ConfigureAwait(false);
            NoDiagnostics(diagnostics);
        }

        /// <summary>
        /// Verifies that <paramref name="code"/> produces no diagnostics when analyzed with <paramref name="analyzer"/>.
        /// </summary>
        /// <param name="analyzer">The analyzer.</param>
        /// <param name="code">
        /// The code to create the solution from.
        /// Can be a .cs, .csproj or .sln file
        /// </param>
        /// <param name="compilationOptions">The <see cref="CSharpCompilationOptions"/> to use.</param>
        /// <param name="metadataReferences">The metadata references to use when compiling.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        public static async Task ValidAsync(DiagnosticAnalyzer analyzer, FileInfo code, CSharpCompilationOptions compilationOptions, IReadOnlyList<MetadataReference> metadataReferences)
        {
            var diagnostics = await Analyze.GetDiagnosticsAsync(analyzer, code, compilationOptions, metadataReferences)
                                           .ConfigureAwait(false);

            NoDiagnostics(diagnostics);
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

                throw AssertException.Create(builder.Return());
            }
        }
    }
}
