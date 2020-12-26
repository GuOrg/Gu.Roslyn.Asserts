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
        /// <param name="code">The code to analyze using <paramref name="analyzer"/>. Analyzing the code is expected to produce no errors or warnings.</param>
        public static void NoAnalyzerDiagnostics(DiagnosticAnalyzer analyzer, params string[] code)
        {
            if (analyzer is null)
            {
                throw new ArgumentNullException(nameof(analyzer));
            }

#pragma warning disable CS0618 // Suppress until removed. Will be replaced with MetadataReferences.FromAttributes()
            var sln = CodeFactory.CreateSolution(code, CodeFactory.DefaultCompilationOptions(analyzer, SuppressedDiagnostics), MetadataReferences);
#pragma warning restore CS0618 // Suppress until removed. Will be replaced with MetadataReferences.FromAttributes()
            var diagnostics = Analyze.GetDiagnostics(analyzer, sln);
            NoDiagnostics(diagnostics);
        }

        /// <summary>
        /// Verifies that <paramref name="code"/> produces no diagnostics when analyzed with <paramref name="analyzer"/>.
        /// </summary>
        /// <param name="analyzer">The <see cref="DiagnosticAnalyzer"/> to check <paramref name="code"/> with.</param>
        /// <param name="code">The code to analyze using <paramref name="analyzer"/>. Analyzing the code is expected to produce no errors or warnings.</param>
        public static void NoAnalyzerDiagnostics(DiagnosticAnalyzer analyzer, IReadOnlyList<string> code)
        {
            if (analyzer is null)
            {
                throw new ArgumentNullException(nameof(analyzer));
            }

            if (code is null)
            {
                throw new ArgumentNullException(nameof(code));
            }

#pragma warning disable CS0618 // Suppress until removed. Will be replaced with MetadataReferences.FromAttributes()
            var sln = CodeFactory.CreateSolution(code, CodeFactory.DefaultCompilationOptions(analyzer, SuppressedDiagnostics), MetadataReferences);
#pragma warning restore CS0618 // Suppress until removed. Will be replaced with MetadataReferences.FromAttributes()
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
            if (analyzer is null)
            {
                throw new ArgumentNullException(nameof(analyzer));
            }

            if (code is null)
            {
                throw new ArgumentNullException(nameof(code));
            }

#pragma warning disable CS0618 // Suppress until removed. Will be replaced with MetadataReferences.FromAttributes()
            var sln = CodeFactory.CreateSolution(code, CodeFactory.DefaultCompilationOptions(analyzer, SuppressedDiagnostics), MetadataReferences);
#pragma warning restore CS0618 // Suppress until removed. Will be replaced with MetadataReferences.FromAttributes()
            var diagnostics = Analyze.GetDiagnostics(analyzer, sln);
            NoDiagnostics(diagnostics);
        }

        /// <summary>
        /// Verifies that <paramref name="code"/> produces no diagnostics when analyzed with <paramref name="analyzer"/>.
        /// </summary>
        /// <param name="analyzer">The <see cref="DiagnosticAnalyzer"/> to check <paramref name="code"/> with.</param>
        /// <param name="descriptor">The <see cref="ExpectedDiagnostic"/> with information about the expected <see cref="Diagnostic"/>. If <paramref name="analyzer"/> supports more than one <see cref="DiagnosticDescriptor.Id"/> this must be provided.</param>
        /// <param name="code">The code to analyze using <paramref name="analyzer"/>. Analyzing the code is expected to produce no errors or warnings.</param>
        public static void NoAnalyzerDiagnostics(DiagnosticAnalyzer analyzer, DiagnosticDescriptor descriptor, params string[] code)
        {
            if (analyzer is null)
            {
                throw new ArgumentNullException(nameof(analyzer));
            }

            if (descriptor is null)
            {
                throw new ArgumentNullException(nameof(descriptor));
            }

            VerifyAnalyzerSupportsDiagnostic(analyzer, descriptor);
#pragma warning disable CS0618 // Suppress until removed. Will be replaced with MetadataReferences.FromAttributes()
            var sln = CodeFactory.CreateSolution(code, CodeFactory.DefaultCompilationOptions(analyzer, descriptor, SuppressedDiagnostics), MetadataReferences);
#pragma warning restore CS0618 // Suppress until removed. Will be replaced with MetadataReferences.FromAttributes()
            var diagnostics = Analyze.GetDiagnostics(analyzer, sln);
            NoDiagnostics(diagnostics);
        }

        /// <summary>
        /// Verifies that <paramref name="code"/> produces no diagnostics when analyzed with <paramref name="analyzer"/>.
        /// </summary>
        /// <param name="analyzer">The <see cref="DiagnosticAnalyzer"/> to check <paramref name="code"/> with.</param>
        /// <param name="descriptors">The <see cref="ExpectedDiagnostic"/> with information about the expected <see cref="Diagnostic"/>. If <paramref name="analyzer"/> supports more than one <see cref="DiagnosticDescriptor.Id"/> this must be provided.</param>
        /// <param name="code">The code to analyze using <paramref name="analyzer"/>. Analyzing the code is expected to produce no errors or warnings.</param>
        public static void NoAnalyzerDiagnostics(DiagnosticAnalyzer analyzer, IReadOnlyList<DiagnosticDescriptor> descriptors, params string[] code)
        {
            if (analyzer is null)
            {
                throw new ArgumentNullException(nameof(analyzer));
            }

            if (descriptors is null)
            {
                throw new ArgumentNullException(nameof(descriptors));
            }

            VerifyAnalyzerSupportsDiagnostics(analyzer, descriptors);
#pragma warning disable CS0618 // Suppress until removed. Will be replaced with MetadataReferences.FromAttributes()
            var sln = CodeFactory.CreateSolution(code, CodeFactory.DefaultCompilationOptions(analyzer, descriptors, SuppressedDiagnostics), MetadataReferences);
#pragma warning restore CS0618 // Suppress until removed. Will be replaced with MetadataReferences.FromAttributes()
            var diagnostics = Analyze.GetDiagnostics(analyzer, sln);
            NoDiagnostics(diagnostics);
        }

        /// <summary>
        /// Verifies that <paramref name="code"/> produces no diagnostics when analyzed with <paramref name="analyzer"/>.
        /// </summary>
        /// <param name="analyzer">The <see cref="DiagnosticAnalyzer"/> to check <paramref name="code"/> with.</param>
        /// <param name="descriptor">The <see cref="ExpectedDiagnostic"/> with information about the expected <see cref="Diagnostic"/>. If <paramref name="analyzer"/> supports more than one <see cref="DiagnosticDescriptor.Id"/> this must be provided.</param>
        /// <param name="code">The code to analyze using <paramref name="analyzer"/>. Analyzing the code is expected to produce no errors or warnings.</param>
        public static void NoAnalyzerDiagnostics(DiagnosticAnalyzer analyzer, DiagnosticDescriptor descriptor, IReadOnlyList<string> code)
        {
            if (analyzer is null)
            {
                throw new ArgumentNullException(nameof(analyzer));
            }

            if (descriptor is null)
            {
                throw new ArgumentNullException(nameof(descriptor));
            }

            if (code is null)
            {
                throw new ArgumentNullException(nameof(code));
            }

            VerifyAnalyzerSupportsDiagnostic(analyzer, descriptor);
#pragma warning disable CS0618 // Suppress until removed. Will be replaced with MetadataReferences.FromAttributes()
            var sln = CodeFactory.CreateSolution(code, CodeFactory.DefaultCompilationOptions(analyzer, descriptor, SuppressedDiagnostics), MetadataReferences);
#pragma warning restore CS0618 // Suppress until removed. Will be replaced with MetadataReferences.FromAttributes()
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
            if (analyzer is null)
            {
                throw new ArgumentNullException(nameof(analyzer));
            }

            if (descriptor is null)
            {
                throw new ArgumentNullException(nameof(descriptor));
            }

            if (code is null)
            {
                throw new ArgumentNullException(nameof(code));
            }

            VerifyAnalyzerSupportsDiagnostic(analyzer, descriptor);
#pragma warning disable CS0618 // Suppress until removed. Will be replaced with MetadataReferences.FromAttributes()
            var sln = CodeFactory.CreateSolution(code, CodeFactory.DefaultCompilationOptions(analyzer, descriptor, SuppressedDiagnostics), MetadataReferences);
#pragma warning restore CS0618 // Suppress until removed. Will be replaced with MetadataReferences.FromAttributes()
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
            if (analyzer is null)
            {
                throw new ArgumentNullException(nameof(analyzer));
            }

            if (solution is null)
            {
                throw new ArgumentNullException(nameof(solution));
            }

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
        /// <param name="code">The code to analyze using <paramref name="analyzer"/>. Analyzing the code is expected to produce no errors or warnings.</param>
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
        /// <param name="code">The code to analyze using <paramref name="analyzer"/>. Analyzing the code is expected to produce no errors or warnings.</param>
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
        /// <param name="code">The code to analyze using <paramref name="analyzerType"/>. Analyzing the code is expected to produce no errors or warnings.</param>
        public static void NoAnalyzerDiagnostics(Type analyzerType, params string[] code)
        {
            NoAnalyzerDiagnostics((DiagnosticAnalyzer)Activator.CreateInstance(analyzerType, nonPublic: true)!, code);
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
            NoAnalyzerDiagnostics((DiagnosticAnalyzer)Activator.CreateInstance(analyzerType, nonPublic: true)!, code);
        }

        /// <summary>
        /// Verifies that <paramref name="code"/> produces no diagnostics when analyzed with <paramref name="analyzerType"/>.
        /// </summary>
        /// <param name="analyzerType">The type of <see cref="DiagnosticAnalyzer"/> to check <paramref name="code"/> with.</param>
        /// <param name="descriptor">The <see cref="ExpectedDiagnostic"/> with information about the expected <see cref="Diagnostic"/>.</param>
        /// <param name="code">The code to analyze using <paramref name="analyzerType"/>. Analyzing the code is expected to produce no errors or warnings.</param>
        public static void NoAnalyzerDiagnostics(Type analyzerType, DiagnosticDescriptor descriptor, params string[] code)
        {
            NoAnalyzerDiagnostics((DiagnosticAnalyzer)Activator.CreateInstance(analyzerType, nonPublic: true)!, descriptor, code);
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
            NoAnalyzerDiagnostics((DiagnosticAnalyzer)Activator.CreateInstance(analyzerType, nonPublic: true)!, descriptor, code);
        }

        /// <summary>
        /// Verifies that <paramref name="code"/> produces no diagnostics when analyzed with <paramref name="analyzerType"/>.
        /// </summary>
        /// <param name="analyzerType">The type of <see cref="DiagnosticAnalyzer"/> to check <paramref name="code"/> with.</param>
        /// <param name="descriptors">The <see cref="ExpectedDiagnostic"/> with information about the expected <see cref="Diagnostic"/>.</param>
        /// <param name="code">The code to analyze using <paramref name="analyzerType"/>. Analyzing the code is expected to produce no errors or warnings.</param>
        public static void NoAnalyzerDiagnostics(Type analyzerType, IReadOnlyList<DiagnosticDescriptor> descriptors, params string[] code)
        {
            NoAnalyzerDiagnostics((DiagnosticAnalyzer)Activator.CreateInstance(analyzerType, nonPublic: true)!, descriptors, code);
        }

        /// <summary>
        /// Verifies that <paramref name="solution"/> produces no diagnostics when analyzed with <paramref name="analyzerType"/>.
        /// </summary>
        /// <param name="analyzerType">The type of <see cref="DiagnosticAnalyzer"/> to check <paramref name="solution"/> with.</param>
        /// <param name="solution">The <see cref="Solution"/> for which no errors or warnings are expected.</param>
        public static void NoAnalyzerDiagnostics(Type analyzerType, Solution solution)
        {
            NoAnalyzerDiagnostics((DiagnosticAnalyzer)Activator.CreateInstance(analyzerType, nonPublic: true)!, solution);
        }
    }
}
