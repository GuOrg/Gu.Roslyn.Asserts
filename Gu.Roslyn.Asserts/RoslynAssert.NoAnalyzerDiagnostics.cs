namespace Gu.Roslyn.Asserts
{
    using System;
    using System.Collections.Generic;
    using System.IO;
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
        public static void NoAnalyzerDiagnostics(DiagnosticAnalyzer analyzer, params string[] code)
        {
            var sln = CodeFactory.CreateSolution(code, CodeFactory.DefaultCompilationOptions(analyzer, SuppressedDiagnostics), MetadataReferences);
            var diagnostics = Analyze.GetDiagnostics(analyzer, sln);
            NoDiagnostics(diagnostics);
        }

        /// <summary>
        /// Verifies that <paramref name="code"/> produces no diagnostics when analyzed with <paramref name="analyzer"/>.
        /// </summary>
        /// <param name="analyzer">The <see cref="DiagnosticAnalyzer"/> to check <paramref name="code"/> with.</param>
        /// <param name="code">The code to analyze.</param>
        public static void NoAnalyzerDiagnostics(DiagnosticAnalyzer analyzer, IReadOnlyList<string> code)
        {
            var sln = CodeFactory.CreateSolution(code, CodeFactory.DefaultCompilationOptions(analyzer, SuppressedDiagnostics), MetadataReferences);
            var diagnostics = Analyze.GetDiagnostics(analyzer, sln);
            NoDiagnostics(diagnostics);
        }

        /// <summary>
        /// Verifies that <paramref name="code"/> produces no diagnostics when analyzed with <paramref name="analyzer"/>.
        /// </summary>
        /// <param name="analyzer">The <see cref="DiagnosticAnalyzer"/> to check <paramref name="code"/> with.</param>
        /// <param name="code">
        /// The code to create the solution from.
        /// Can be a .cs, .csproj or .sln file.
        /// </param>
        public static void NoAnalyzerDiagnostics(DiagnosticAnalyzer analyzer, FileInfo code)
        {
            var sln = CodeFactory.CreateSolution(code, CodeFactory.DefaultCompilationOptions(analyzer, SuppressedDiagnostics), MetadataReferences);
            var diagnostics = Analyze.GetDiagnostics(analyzer, sln);
            NoDiagnostics(diagnostics);
        }

        /// <summary>
        /// Verifies that <paramref name="code"/> produces no diagnostics when analyzed with <paramref name="analyzer"/>.
        /// </summary>
        /// <param name="analyzer">The <see cref="DiagnosticAnalyzer"/> to check <paramref name="code"/> with.</param>
        /// <param name="descriptor">The <see cref="ExpectedDiagnostic"/> with information about the expected <see cref="Diagnostic"/>. If <paramref name="analyzer"/> supports more than one <see cref="DiagnosticDescriptor.Id"/> this must be provided.</param>
        /// <param name="code">The code to analyze.</param>
        public static void NoAnalyzerDiagnostics(DiagnosticAnalyzer analyzer, DiagnosticDescriptor descriptor, params string[] code)
        {
            VerifyAnalyzerSupportsDiagnostic(analyzer, descriptor);
            var sln = CodeFactory.CreateSolution(code, CodeFactory.DefaultCompilationOptions(analyzer, descriptor, SuppressedDiagnostics), MetadataReferences);
            var diagnostics = Analyze.GetDiagnostics(analyzer, sln);
            NoDiagnostics(diagnostics);
        }

        /// <summary>
        /// Verifies that <paramref name="code"/> produces no diagnostics when analyzed with <paramref name="analyzer"/>.
        /// </summary>
        /// <param name="analyzer">The <see cref="DiagnosticAnalyzer"/> to check <paramref name="code"/> with.</param>
        /// <param name="descriptors">The <see cref="ExpectedDiagnostic"/> with information about the expected <see cref="Diagnostic"/>. If <paramref name="analyzer"/> supports more than one <see cref="DiagnosticDescriptor.Id"/> this must be provided.</param>
        /// <param name="code">The code to analyze.</param>
        public static void NoAnalyzerDiagnostics(DiagnosticAnalyzer analyzer, IReadOnlyList<DiagnosticDescriptor> descriptors, params string[] code)
        {
            VerifyAnalyzerSupportsDiagnostics(analyzer, descriptors);
            var sln = CodeFactory.CreateSolution(code, CodeFactory.DefaultCompilationOptions(analyzer, descriptors, SuppressedDiagnostics), MetadataReferences);
            var diagnostics = Analyze.GetDiagnostics(analyzer, sln);
            NoDiagnostics(diagnostics);
        }

        /// <summary>
        /// Verifies that <paramref name="code"/> produces no diagnostics when analyzed with <paramref name="analyzer"/>.
        /// </summary>
        /// <param name="analyzer">The <see cref="DiagnosticAnalyzer"/> to check <paramref name="code"/> with.</param>
        /// <param name="descriptor">The <see cref="ExpectedDiagnostic"/> with information about the expected <see cref="Diagnostic"/>. If <paramref name="analyzer"/> supports more than one <see cref="DiagnosticDescriptor.Id"/> this must be provided.</param>
        /// <param name="code">The code to analyze.</param>
        public static void NoAnalyzerDiagnostics(DiagnosticAnalyzer analyzer, DiagnosticDescriptor descriptor, IReadOnlyList<string> code)
        {
            VerifyAnalyzerSupportsDiagnostic(analyzer, descriptor);
            var sln = CodeFactory.CreateSolution(code, CodeFactory.DefaultCompilationOptions(analyzer, descriptor, SuppressedDiagnostics), MetadataReferences);
            var diagnostics = Analyze.GetDiagnostics(analyzer, sln);
            NoDiagnostics(diagnostics);
        }

        /// <summary>
        /// Verifies that <paramref name="code"/> produces no diagnostics when analyzed with <paramref name="analyzer"/>.
        /// </summary>
        /// <param name="analyzer">The <see cref="DiagnosticAnalyzer"/> to check <paramref name="code"/> with.</param>
        /// <param name="descriptor">The <see cref="ExpectedDiagnostic"/> with information about the expected <see cref="Diagnostic"/>. If <paramref name="analyzer"/> supports more than one <see cref="DiagnosticDescriptor.Id"/> this must be provided.</param>
        /// <param name="code">
        /// The code to create the solution from.
        /// Can be a .cs, .csproj or .sln file.
        /// </param>
        public static void NoAnalyzerDiagnostics(DiagnosticAnalyzer analyzer, DiagnosticDescriptor descriptor, FileInfo code)
        {
            VerifyAnalyzerSupportsDiagnostic(analyzer, descriptor);
            var sln = CodeFactory.CreateSolution(code, CodeFactory.DefaultCompilationOptions(analyzer, descriptor, SuppressedDiagnostics), MetadataReferences);
            var diagnostics = Analyze.GetDiagnostics(analyzer, sln);
            NoDiagnostics(diagnostics);
        }

        /// <summary>
        /// Verifies that <paramref name="solution"/> produces no diagnostics when analyzed with <paramref name="analyzer"/>.
        /// </summary>
        /// <param name="analyzer">The <see cref="DiagnosticAnalyzer"/> to check <paramref name="solution"/> with.</param>
        /// <param name="solution">The <see cref="Solution"/> for which no errors or warnings are expected.</param>
        public static void NoAnalyzerDiagnostics(DiagnosticAnalyzer analyzer, Solution solution)
        {
            var diagnostics = Analyze.GetDiagnostics(analyzer, solution);
            NoDiagnostics(diagnostics);
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
        public static void NoAnalyzerDiagnostics(DiagnosticAnalyzer analyzer, FileInfo code, CSharpCompilationOptions compilationOptions, IEnumerable<MetadataReference> metadataReferences)
        {
            var sln = CodeFactory.CreateSolution(code, compilationOptions, metadataReferences);
            var diagnostics = Analyze.GetDiagnostics(analyzer, sln);
            NoDiagnostics(diagnostics);
        }

        /// <summary>
        /// Verifies that <paramref name="code"/> produces no diagnostics when analyzed with <paramref name="analyzer"/>.
        /// </summary>
        /// <param name="analyzer">The <see cref="DiagnosticAnalyzer"/> to check <paramref name="code"/> with.</param>
        /// <param name="code">The code to analyze.</param>
        /// <param name="compilationOptions">The <see cref="CSharpCompilationOptions"/> to use.</param>
        /// <param name="metadataReferences">The metadata references to use when compiling.</param>
        public static void NoAnalyzerDiagnostics(DiagnosticAnalyzer analyzer, string code, CSharpCompilationOptions compilationOptions, IEnumerable<MetadataReference> metadataReferences)
        {
            var sln = CodeFactory.CreateSolution(code, compilationOptions, metadataReferences);
            var diagnostics = Analyze.GetDiagnostics(analyzer, sln);
            NoDiagnostics(diagnostics);
        }

        /// <summary>
        /// Verifies that <paramref name="code"/> produces no diagnostics when analyzed with <paramref name="analyzer"/>.
        /// </summary>
        /// <param name="analyzer">The <see cref="DiagnosticAnalyzer"/> to check <paramref name="code"/> with.</param>
        /// <param name="code">The code to analyze.</param>
        /// <param name="compilationOptions">The <see cref="CSharpCompilationOptions"/> to use.</param>
        /// <param name="metadataReferences">The metadata references to use when compiling.</param>
        public static void NoAnalyzerDiagnostics(DiagnosticAnalyzer analyzer, IReadOnlyList<string> code, CSharpCompilationOptions compilationOptions, IEnumerable<MetadataReference> metadataReferences)
        {
            var sln = CodeFactory.CreateSolution(code, compilationOptions, metadataReferences);
            var diagnostics = Analyze.GetDiagnostics(analyzer, sln);
            NoDiagnostics(diagnostics);
        }

        /// <summary>
        /// Verifies that <paramref name="code"/> produces no diagnostics when analyzed with <paramref name="analyzerType"/>.
        /// </summary>
        /// <param name="analyzerType">The type of <see cref="DiagnosticAnalyzer"/> to check <paramref name="code"/> with.</param>
        /// <param name="code">The code to analyze.</param>
        public static void NoAnalyzerDiagnostics(Type analyzerType, params string[] code)
        {
            NoAnalyzerDiagnostics((DiagnosticAnalyzer)Activator.CreateInstance(analyzerType), code);
        }

        /// <summary>
        /// Verifies that <paramref name="code"/> produces no diagnostics when analyzed with <paramref name="analyzerType"/>.
        /// </summary>
        /// <param name="analyzerType">The type of <see cref="DiagnosticAnalyzer"/> to check <paramref name="code"/> with.</param>
        /// <param name="code">
        /// The code to create the solution from.
        /// Can be a .cs, .csproj or .sln file.
        /// </param>
        public static void NoAnalyzerDiagnostics(Type analyzerType, FileInfo code)
        {
            NoAnalyzerDiagnostics((DiagnosticAnalyzer)Activator.CreateInstance(analyzerType), code);
        }

        /// <summary>
        /// Verifies that <paramref name="code"/> produces no diagnostics when analyzed with <paramref name="analyzerType"/>.
        /// </summary>
        /// <param name="analyzerType">The type of <see cref="DiagnosticAnalyzer"/> to check <paramref name="code"/> with.</param>
        /// <param name="descriptor">The <see cref="ExpectedDiagnostic"/> with information about the expected <see cref="Diagnostic"/>.</param>
        /// <param name="code">The code to analyze.</param>
        public static void NoAnalyzerDiagnostics(Type analyzerType, DiagnosticDescriptor descriptor, params string[] code)
        {
            NoAnalyzerDiagnostics((DiagnosticAnalyzer)Activator.CreateInstance(analyzerType), descriptor, code);
        }

        /// <summary>
        /// Verifies that <paramref name="code"/> produces no diagnostics when analyzed with <paramref name="analyzerType"/>.
        /// </summary>
        /// <param name="analyzerType">The type of <see cref="DiagnosticAnalyzer"/> to check <paramref name="code"/> with.</param>
        /// <param name="descriptor">The <see cref="ExpectedDiagnostic"/> with information about the expected <see cref="Diagnostic"/>.</param>
        /// <param name="code">
        /// The code to create the solution from.
        /// Can be a .cs, .csproj or .sln file.
        /// </param>
        public static void NoAnalyzerDiagnostics(Type analyzerType, DiagnosticDescriptor descriptor, FileInfo code)
        {
            NoAnalyzerDiagnostics((DiagnosticAnalyzer)Activator.CreateInstance(analyzerType), descriptor, code);
        }

        /// <summary>
        /// Verifies that <paramref name="code"/> produces no diagnostics when analyzed with <paramref name="analyzerType"/>.
        /// </summary>
        /// <param name="analyzerType">The type of <see cref="DiagnosticAnalyzer"/> to check <paramref name="code"/> with.</param>
        /// <param name="descriptors">The <see cref="ExpectedDiagnostic"/> with information about the expected <see cref="Diagnostic"/>.</param>
        /// <param name="code">The code to analyze.</param>
        public static void NoAnalyzerDiagnostics(Type analyzerType, IReadOnlyList<DiagnosticDescriptor> descriptors, params string[] code)
        {
            NoAnalyzerDiagnostics((DiagnosticAnalyzer)Activator.CreateInstance(analyzerType), descriptors, code);
        }

        /// <summary>
        /// Verifies that <paramref name="solution"/> produces no diagnostics when analyzed with <paramref name="analyzerType"/>.
        /// </summary>
        /// <param name="analyzerType">The type of <see cref="DiagnosticAnalyzer"/> to check <paramref name="solution"/> with.</param>
        /// <param name="solution">The <see cref="Solution"/> for which no errors or warnings are expected.</param>
        public static void NoAnalyzerDiagnostics(Type analyzerType, Solution solution)
        {
            NoAnalyzerDiagnostics((DiagnosticAnalyzer)Activator.CreateInstance(analyzerType), solution);
        }
    }
}
