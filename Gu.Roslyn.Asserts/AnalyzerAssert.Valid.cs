namespace Gu.Roslyn.Asserts
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Threading.Tasks;
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
            ValidAsync(
                    analyzer,
                    code,
                    CodeFactory.DefaultCompilationOptions(analyzer, SuppressedDiagnostics),
                    MetadataReferences)
                .GetAwaiter()
                .GetResult();
        }

        /// <summary>
        /// Verifies that <paramref name="code"/> produces no diagnostics when analyzed with <paramref name="analyzerType"/>.
        /// </summary>
        /// <param name="analyzerType">The type of the analyzer.</param>
        /// <param name="code">The code to analyze.</param>
        public static void Valid(Type analyzerType, params string[] code)
        {
            var analyzer = (DiagnosticAnalyzer)Activator.CreateInstance(analyzerType);
            ValidAsync(
                    analyzer,
                    code,
                    CodeFactory.DefaultCompilationOptions(analyzer, SuppressedDiagnostics),
                    MetadataReferences)
                .GetAwaiter()
                .GetResult();
        }

        /// <summary>
        /// Verifies that <paramref name="code"/> produces no diagnostics when analyzed with <paramref name="analyzer"/>.
        /// </summary>
        /// <param name="analyzer">The analyzer.</param>
        /// <param name="code">The code to analyze.</param>
        public static void Valid(DiagnosticAnalyzer analyzer, params string[] code)
        {
            ValidAsync(
                    analyzer,
                    code,
                    CodeFactory.DefaultCompilationOptions(analyzer, SuppressedDiagnostics),
                    MetadataReferences)
                .GetAwaiter()
                .GetResult();
        }

        /// <summary>
        /// Verifies that <paramref name="code"/> produces no diagnostics when analyzed with <paramref name="analyzer"/>.
        /// </summary>
        /// <param name="analyzer">The analyzer.</param>
        /// <param name="code">The code to analyze.</param>
        public static void Valid(DiagnosticAnalyzer analyzer, IReadOnlyList<string> code)
        {
            ValidAsync(
                    analyzer,
                    code,
                    CodeFactory.DefaultCompilationOptions(analyzer, SuppressedDiagnostics),
                    MetadataReferences)
                .GetAwaiter()
                .GetResult();
        }

        /// <summary>
        /// Verifies that <paramref name="code"/> produces no diagnostics when analyzed with <paramref name="analyzer"/>.
        /// </summary>
        /// <param name="analyzer">The analyzer.</param>
        /// <param name="code">The code to analyze.</param>
        /// <param name="metadataReferences">The metadata references to use when compiling.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        public static Task ValidAsync(DiagnosticAnalyzer analyzer, IReadOnlyList<string> code, IReadOnlyList<MetadataReference> metadataReferences)
        {
            return ValidAsync(
                analyzer,
                code,
                CodeFactory.DefaultCompilationOptions(analyzer, SuppressedDiagnostics),
                metadataReferences);
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
            var diagnostics = await Analyze.GetDiagnosticsAsync(
                                               analyzer,
                                               code,
                                               compilationOptions,
                                               metadataReferences)
                                           .ConfigureAwait(false);

            if (diagnostics.SelectMany(x => x).Any())
            {
                throw AssertException.Create(string.Join(Environment.NewLine, diagnostics.SelectMany(x => x)));
            }
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
            ValidAsync(
                    analyzer,
                    code,
                    CodeFactory.DefaultCompilationOptions(analyzer, SuppressedDiagnostics),
                    MetadataReferences)
                .GetAwaiter()
                .GetResult();
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
            ValidAsync(
                    analyzer,
                    code,
                    CodeFactory.DefaultCompilationOptions(analyzer, SuppressedDiagnostics),
                    MetadataReferences)
                .GetAwaiter()
                .GetResult();
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
            ValidAsync(
                    analyzer,
                    code,
                    CodeFactory.DefaultCompilationOptions(analyzer, SuppressedDiagnostics),
                    MetadataReferences)
                .GetAwaiter()
                .GetResult();
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
            AssertAnalyzerSupportsExpectedDiagnostic(analyzer, expectedDiagnostic, out var descriptor);
            ValidAsync(
                    analyzer,
                    code,
                    CodeFactory.DefaultCompilationOptions(descriptor, SuppressedDiagnostics),
                    MetadataReferences)
                .GetAwaiter()
                .GetResult();
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
            AssertAnalyzerSupportsExpectedDiagnostic(analyzer, expectedDiagnostic, out var descriptor);
            ValidAsync(
                    analyzer,
                    code,
                    CodeFactory.DefaultCompilationOptions(descriptor, SuppressedDiagnostics),
                    MetadataReferences)
                .GetAwaiter()
                .GetResult();
        }

        /// <summary>
        /// Verifies that <paramref name="code"/> produces no diagnostics when analyzed with <paramref name="analyzer"/>.
        /// </summary>
        /// <param name="analyzer">The analyzer.</param>
        /// <param name="expectedDiagnostic">The expected diagnostic.</param>
        /// <param name="code">The code to analyze.</param>
        public static void Valid(DiagnosticAnalyzer analyzer, ExpectedDiagnostic expectedDiagnostic, params string[] code)
        {
            AssertAnalyzerSupportsExpectedDiagnostic(analyzer, expectedDiagnostic, out var descriptor);
            ValidAsync(
                    analyzer,
                    code,
                    CodeFactory.DefaultCompilationOptions(descriptor, SuppressedDiagnostics),
                    MetadataReferences)
                .GetAwaiter()
                .GetResult();
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
            AssertAnalyzerSupportsExpectedDiagnostics(analyzer, expectedDiagnostics, out var descriptors);
            ValidAsync(
                    analyzer,
                    code,
                    CodeFactory.DefaultCompilationOptions(descriptors, SuppressedDiagnostics),
                    MetadataReferences)
                .GetAwaiter()
                .GetResult();
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
            AssertAnalyzerSupportsExpectedDiagnostics(analyzer, expectedDiagnostics, out var descriptors);
            ValidAsync(
                    analyzer,
                    code,
                    CodeFactory.DefaultCompilationOptions(descriptors, SuppressedDiagnostics),
                    MetadataReferences)
                .GetAwaiter()
                .GetResult();
        }

        /// <summary>
        /// Verifies that <paramref name="code"/> produces no diagnostics when analyzed with <paramref name="analyzer"/>.
        /// </summary>
        /// <param name="analyzer">The analyzer.</param>
        /// <param name="expectedDiagnostics">The expected diagnostic.</param>
        /// <param name="code">The code to analyze.</param>
        public static void Valid(DiagnosticAnalyzer analyzer, IReadOnlyList<ExpectedDiagnostic> expectedDiagnostics, params string[] code)
        {
            AssertAnalyzerSupportsExpectedDiagnostics(analyzer, expectedDiagnostics, out var descriptors);
            ValidAsync(
                    analyzer,
                    code,
                    CodeFactory.DefaultCompilationOptions(descriptors, SuppressedDiagnostics),
                    MetadataReferences)
                .GetAwaiter()
                .GetResult();
        }

        /// <summary>
        /// Verifies that <paramref name="code"/> produces no diagnostics when analyzed with <paramref name="analyzer"/>.
        /// </summary>
        /// <param name="analyzer">The analyzer.</param>
        /// <param name="expectedDiagnostic">The expected diagnostic.</param>
        /// <param name="code">The code to analyze.</param>
        public static void Valid(DiagnosticAnalyzer analyzer, ExpectedDiagnostic expectedDiagnostic, IReadOnlyList<string> code)
        {
            AssertAnalyzerSupportsExpectedDiagnostic(analyzer, expectedDiagnostic, out var descriptor);
            ValidAsync(
                    analyzer,
                    code,
                    CodeFactory.DefaultCompilationOptions(descriptor, SuppressedDiagnostics),
                    MetadataReferences)
                .GetAwaiter()
                .GetResult();
        }

        /// <summary>
        /// Verifies that <paramref name="code"/> produces no diagnostics when analyzed with <paramref name="analyzer"/>.
        /// </summary>
        /// <param name="analyzer">The analyzer.</param>
        /// <param name="expectedDiagnostic">The expected diagnostic.</param>
        /// <param name="code">The code to analyze.</param>
        /// <param name="metadataReferences">The metadata references to use when compiling.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        public static Task ValidAsync(DiagnosticAnalyzer analyzer, ExpectedDiagnostic expectedDiagnostic, IReadOnlyList<string> code, IReadOnlyList<MetadataReference> metadataReferences)
        {
            AssertAnalyzerSupportsExpectedDiagnostic(analyzer, expectedDiagnostic, out var descriptor);
            return ValidAsync(
                analyzer,
                code,
                CodeFactory.DefaultCompilationOptions(descriptor, SuppressedDiagnostics),
                metadataReferences);
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
            AssertAnalyzerSupportsExpectedDiagnostic(analyzer, expectedDiagnostic, out var descriptor);
            ValidAsync(
                    analyzer,
                    code,
                    CodeFactory.DefaultCompilationOptions(descriptor, SuppressedDiagnostics),
                    MetadataReferences)
                .GetAwaiter()
                .GetResult();
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
            AssertAnalyzerSupportsExpectedDiagnostic(analyzer, expectedDiagnostic, out var descriptor);
            ValidAsync(
                    analyzer,
                    code,
                    CodeFactory.DefaultCompilationOptions(descriptor, SuppressedDiagnostics),
                    MetadataReferences)
                .GetAwaiter()
                .GetResult();
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
            AssertAnalyzerSupportsExpectedDiagnostic(analyzer, expectedDiagnostic, out var descriptor);
            ValidAsync(
                    analyzer,
                    code,
                    CodeFactory.DefaultCompilationOptions(descriptor, SuppressedDiagnostics),
                    MetadataReferences)
                .GetAwaiter()
                .GetResult();
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
            ValidAsync(
                    analyzer,
                    solution)
                .GetAwaiter()
                .GetResult();
        }

        /// <summary>
        /// Verifies that <paramref name="solution"/> produces no diagnostics when analyzed with <paramref name="analyzerType"/>.
        /// </summary>
        /// <param name="analyzerType">The type of the analyzer.</param>
        /// <param name="solution">The <see cref="Solution"/> for which no errors or warnings are expected.</param>
        public static void Valid(Type analyzerType, Solution solution)
        {
            var analyzer = (DiagnosticAnalyzer)Activator.CreateInstance(analyzerType);
            ValidAsync(
                    analyzer,
                    solution)
                .GetAwaiter()
                .GetResult();
        }

        /// <summary>
        /// Verifies that <paramref name="solution"/> produces no diagnostics when analyzed with <paramref name="analyzer"/>.
        /// </summary>
        /// <param name="analyzer">The analyzer.</param>
        /// <param name="solution">The <see cref="Solution"/> for which no errors or warnings are expected.</param>
        public static void Valid(DiagnosticAnalyzer analyzer, Solution solution)
        {
            ValidAsync(
                    analyzer,
                    solution)
                .GetAwaiter()
                .GetResult();
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

            if (diagnostics.SelectMany(x => x).Any())
            {
                throw AssertException.Create(string.Join(Environment.NewLine, diagnostics.SelectMany(x => x)));
            }
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

            if (diagnostics.SelectMany(x => x).Any())
            {
                throw AssertException.Create(string.Join(Environment.NewLine, diagnostics.SelectMany(x => x)));
            }
        }
    }
}
