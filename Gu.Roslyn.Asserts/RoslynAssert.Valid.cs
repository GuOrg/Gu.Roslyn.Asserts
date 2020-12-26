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
        /// <param name="code">The code to analyze using <paramref name="analyzer"/>. Analyzing the code is expected to produce no errors or warnings.</param>
        public static void Valid(DiagnosticAnalyzer analyzer, params string[] code)
        {
            if (analyzer is null)
            {
                throw new ArgumentNullException(nameof(analyzer));
            }

            Valid(
                analyzer,
#pragma warning disable CS0618 // Suppress until removed. Will be replaced with MetadataReferences.FromAttributes()
                CodeFactory.CreateSolution(code, CodeFactory.DefaultCompilationOptions(analyzer, SuppressedDiagnostics), MetadataReferences));
#pragma warning restore CS0618 // Suppress until removed. Will be replaced with MetadataReferences.FromAttributes()
        }

        /// <summary>
        /// Verifies that <paramref name="code"/> produces no diagnostics when analyzed with <paramref name="analyzer"/>.
        /// </summary>
        /// <param name="analyzer">The <see cref="DiagnosticAnalyzer"/> to check <paramref name="code"/> with.</param>
        /// <param name="code">The code to analyze using <paramref name="analyzer"/>. Analyzing the code is expected to produce no errors or warnings.</param>
        /// <param name="suppressWarnings">A collection of <see cref="DiagnosticDescriptor.Id"/> to suppress when analyzing the code. Default is <see langword="null" /> meaning <see cref="SuppressedDiagnostics"/> are used.</param>
        /// <param name="metadataReferences">A collection of <see cref="MetadataReference"/> to use when compiling. Default is <see langword="null" /> meaning <see cref="MetadataReferences"/> are used.</param>
        /// <param name="compilationOptions">The <see cref="CSharpCompilationOptions"/>.</param>
        public static void Valid(
            DiagnosticAnalyzer analyzer,
            IReadOnlyList<string> code,
            IEnumerable<string>? suppressWarnings = null,
            IEnumerable<MetadataReference>? metadataReferences = null,
            CSharpCompilationOptions? compilationOptions = null)
        {
            if (analyzer is null)
            {
                throw new ArgumentNullException(nameof(analyzer));
            }

            if (code is null)
            {
                throw new ArgumentNullException(nameof(code));
            }

            Valid(
                analyzer,
                CodeFactory.CreateSolution(
                code,
#pragma warning disable CS0618 // Suppress until removed. Will be replaced with MetadataReferences.FromAttributes()
                compilationOptions ?? CodeFactory.DefaultCompilationOptions(analyzer, suppressWarnings ?? SuppressedDiagnostics),
                metadataReferences ?? MetadataReferences));
#pragma warning restore CS0618 // Suppress until removed. Will be replaced with MetadataReferences.FromAttributes()
        }

        /// <summary>
        /// Verifies that <paramref name="code"/> produces no diagnostics when analyzed with <paramref name="analyzerType"/>.
        /// </summary>
        /// <param name="analyzerType">The type of <see cref="DiagnosticAnalyzer"/> to check <paramref name="code"/> with.</param>
        /// <param name="descriptor">The <see cref="DiagnosticDescriptor"/> with information about the expected <see cref="Diagnostic"/>. If <paramref name="analyzerType"/> supports more than one <see cref="DiagnosticDescriptor.Id"/> this must be provided.</param>
        /// <param name="code">The code to analyze using <paramref name="analyzerType"/>. Analyzing the code is expected to produce no errors or warnings.</param>
        public static void Valid(Type analyzerType, DiagnosticDescriptor descriptor, params string[] code)
        {
            if (analyzerType is null)
            {
                throw new ArgumentNullException(nameof(analyzerType));
            }

            if (descriptor is null)
            {
                throw new ArgumentNullException(nameof(descriptor));
            }

            Valid((DiagnosticAnalyzer)Activator.CreateInstance(analyzerType, nonPublic: true)!, descriptor, code);
        }

        /// <summary>
        /// Verifies that <paramref name="code"/> produces no diagnostics when analyzed with <paramref name="analyzer"/>.
        /// </summary>
        /// <param name="analyzer">The <see cref="DiagnosticAnalyzer"/> to check <paramref name="code"/> with.</param>
        /// <param name="descriptor">The <see cref="DiagnosticDescriptor"/> with information about the expected <see cref="Diagnostic"/>. If <paramref name="analyzer"/> supports more than one <see cref="DiagnosticDescriptor.Id"/> this must be provided.</param>
        /// <param name="code">The code to analyze using <paramref name="analyzer"/>. Analyzing the code is expected to produce no errors or warnings.</param>
        public static void Valid(DiagnosticAnalyzer analyzer, DiagnosticDescriptor descriptor, params string[] code)
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
            Valid(
                analyzer,
#pragma warning disable CS0618 // Suppress until removed. Will be replaced with MetadataReferences.FromAttributes()
                CodeFactory.CreateSolution(code, CodeFactory.DefaultCompilationOptions(analyzer, descriptor, SuppressedDiagnostics), MetadataReferences));
#pragma warning restore CS0618 // Suppress until removed. Will be replaced with MetadataReferences.FromAttributes()
        }

        /// <summary>
        /// Verifies that <paramref name="code"/> produces no diagnostics when analyzed with <paramref name="analyzer"/>.
        /// </summary>
        /// <param name="analyzer">The <see cref="DiagnosticAnalyzer"/> to check <paramref name="code"/> with.</param>
        /// <param name="descriptor">The <see cref="DiagnosticDescriptor"/> with information about the expected <see cref="Diagnostic"/>. If <paramref name="analyzer"/> supports more than one <see cref="DiagnosticDescriptor.Id"/> this must be provided.</param>
        /// <param name="code">The code to analyze using <paramref name="analyzer"/>. Analyzing the code is expected to produce no errors or warnings.</param>
        /// <param name="suppressWarnings">A collection of <see cref="DiagnosticDescriptor.Id"/> to suppress when analyzing the code. Default is <see langword="null" /> meaning <see cref="SuppressedDiagnostics"/> are used.</param>
        /// <param name="metadataReferences">A collection of <see cref="MetadataReference"/> to use when compiling. Default is <see langword="null" /> meaning <see cref="MetadataReferences"/> are used.</param>
        /// <param name="compilationOptions">The <see cref="CSharpCompilationOptions"/>.</param>
        public static void Valid(
            DiagnosticAnalyzer analyzer,
            DiagnosticDescriptor descriptor,
            IReadOnlyList<string> code,
            IEnumerable<string>? suppressWarnings = null,
            IEnumerable<MetadataReference>? metadataReferences = null,
            CSharpCompilationOptions? compilationOptions = null)
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
            Valid(
                analyzer,
                CodeFactory.CreateSolution(
                code,
#pragma warning disable CS0618 // Suppress until removed. Will be replaced with MetadataReferences.FromAttributes()
                compilationOptions ?? CodeFactory.DefaultCompilationOptions(analyzer, suppressWarnings ?? SuppressedDiagnostics),
                metadataReferences ?? MetadataReferences));
#pragma warning restore CS0618 // Suppress until removed. Will be replaced with MetadataReferences.FromAttributes()
        }

        /// <summary>
        /// Verifies that <paramref name="code"/> produces no diagnostics when analyzed with <paramref name="analyzer"/>.
        /// </summary>
        /// <param name="analyzer">The <see cref="DiagnosticAnalyzer"/> to check <paramref name="code"/> with.</param>
        /// <param name="descriptor">The <see cref="DiagnosticDescriptor"/> with information about the expected <see cref="Diagnostic"/>. If <paramref name="analyzer"/> supports more than one <see cref="DiagnosticDescriptor.Id"/> this must be provided.</param>
        /// <param name="code">
        /// The code to create the solution from.
        /// Can be a .cs, .csproj or .sln file.
        /// </param>
        /// <param name="suppressWarnings">A collection of <see cref="DiagnosticDescriptor.Id"/> to suppress when analyzing the code. Default is <see langword="null" /> meaning <see cref="SuppressedDiagnostics"/> are used.</param>
        /// <param name="metadataReferences">A collection of <see cref="MetadataReference"/> to use when compiling. Default is <see langword="null" /> meaning <see cref="MetadataReferences"/> are used.</param>
        /// <param name="compilationOptions">The <see cref="CSharpCompilationOptions"/>.</param>
        public static void Valid(
            DiagnosticAnalyzer analyzer,
            DiagnosticDescriptor descriptor,
            FileInfo code,
            IEnumerable<string>? suppressWarnings = null,
            IEnumerable<MetadataReference>? metadataReferences = null,
            CSharpCompilationOptions? compilationOptions = null)
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
            Valid(
                analyzer,
                CodeFactory.CreateSolution(
                code,
#pragma warning disable CS0618 // Suppress until removed. Will be replaced with MetadataReferences.FromAttributes()
                compilationOptions ?? CodeFactory.DefaultCompilationOptions(analyzer, suppressWarnings ?? SuppressedDiagnostics),
                metadataReferences ?? MetadataReferences));
#pragma warning restore CS0618 // Suppress until removed. Will be replaced with MetadataReferences.FromAttributes()
        }

        /// <summary>
        /// Verifies that <paramref name="code"/> produces no diagnostics when analyzed with <paramref name="analyzer"/>.
        /// </summary>
        /// <param name="analyzer">The <see cref="DiagnosticAnalyzer"/> to check <paramref name="code"/> with.</param>
        /// <param name="code">
        /// The code to create the solution from.
        /// Can be a .cs, .csproj or .sln file.
        /// </param>
        /// <param name="suppressWarnings">A collection of <see cref="DiagnosticDescriptor.Id"/> to suppress when analyzing the code. Default is <see langword="null" /> meaning <see cref="SuppressedDiagnostics"/> are used.</param>
        /// <param name="metadataReferences">A collection of <see cref="MetadataReference"/> to use when compiling. Default is <see langword="null" /> meaning <see cref="MetadataReferences"/> are used.</param>
        /// <param name="compilationOptions">The <see cref="CSharpCompilationOptions"/>.</param>
        public static void Valid(
            DiagnosticAnalyzer analyzer,
            FileInfo code,
            IEnumerable<string>? suppressWarnings = null,
            IEnumerable<MetadataReference>? metadataReferences = null,
            CSharpCompilationOptions? compilationOptions = null)
        {
            if (analyzer is null)
            {
                throw new ArgumentNullException(nameof(analyzer));
            }

            if (code is null)
            {
                throw new ArgumentNullException(nameof(code));
            }

            Valid(
                analyzer,
                CodeFactory.CreateSolution(
                code,
#pragma warning disable CS0618 // Suppress until removed. Will be replaced with MetadataReferences.FromAttributes()
                compilationOptions ?? CodeFactory.DefaultCompilationOptions(analyzer, suppressWarnings ?? SuppressedDiagnostics),
                metadataReferences ?? MetadataReferences));
#pragma warning restore CS0618 // Suppress until removed. Will be replaced with MetadataReferences.FromAttributes()
        }

        /// <summary>
        /// Verifies that <paramref name="code"/> produces no diagnostics when analyzed with <paramref name="analyzer"/>.
        /// </summary>
        /// <param name="analyzer">The <see cref="DiagnosticAnalyzer"/> to check <paramref name="code"/> with.</param>
        /// <param name="code">The code to analyze using <paramref name="analyzer"/>. Analyzing the code is expected to produce no errors or warnings.</param>
        /// <param name="suppressWarnings">A collection of <see cref="DiagnosticDescriptor.Id"/> to suppress when analyzing the code. Default is <see langword="null" /> meaning <see cref="SuppressedDiagnostics"/> are used.</param>
        /// <param name="metadataReferences">A collection of <see cref="MetadataReference"/> to use when compiling. Default is <see langword="null" /> meaning <see cref="MetadataReferences"/> are used.</param>
        /// <param name="compilationOptions">The <see cref="CSharpCompilationOptions"/>.</param>
        public static void Valid(
            DiagnosticAnalyzer analyzer,
            string code,
            IEnumerable<string>? suppressWarnings = null,
            IEnumerable<MetadataReference>? metadataReferences = null,
            CSharpCompilationOptions? compilationOptions = null)
        {
            if (analyzer is null)
            {
                throw new ArgumentNullException(nameof(analyzer));
            }

            if (code is null)
            {
                throw new ArgumentNullException(nameof(code));
            }

            Valid(
                analyzer,
                CodeFactory.CreateSolution(
                code,
#pragma warning disable CS0618 // Suppress until removed. Will be replaced with MetadataReferences.FromAttributes()
                compilationOptions ?? CodeFactory.DefaultCompilationOptions(analyzer, suppressWarnings ?? SuppressedDiagnostics),
                metadataReferences ?? MetadataReferences));
#pragma warning restore CS0618 // Suppress until removed. Will be replaced with MetadataReferences.FromAttributes()
        }

        /// <summary>
        /// Verifies that <paramref name="code"/> produces no diagnostics when analyzed with <paramref name="analyzerType"/>.
        /// </summary>
        /// <param name="analyzerType">The type of <see cref="DiagnosticAnalyzer"/> to check <paramref name="code"/> with.</param>
        /// <param name="code">The code to analyze using <paramref name="analyzerType"/>. Analyzing the code is expected to produce no errors or warnings.</param>
        public static void Valid(Type analyzerType, params string[] code)
        {
            if (analyzerType is null)
            {
                throw new ArgumentNullException(nameof(analyzerType));
            }

            Valid((DiagnosticAnalyzer)Activator.CreateInstance(analyzerType, nonPublic: true)!, code);
        }

        /// <summary>
        /// Verifies that <paramref name="solution"/> produces no diagnostics when analyzed with <paramref name="analyzerType"/>.
        /// </summary>
        /// <param name="analyzerType">The type of <see cref="DiagnosticAnalyzer"/> to check <paramref name="solution"/> with.</param>
        /// <param name="solution">The <see cref="Solution"/> for which no errors or warnings are expected.</param>
        public static void Valid(Type analyzerType, Solution solution)
        {
            Valid((DiagnosticAnalyzer)Activator.CreateInstance(analyzerType, nonPublic: true)!, solution);
        }

        /// <summary>
        /// Verifies that <paramref name="code"/> produces no diagnostics when analyzed with <paramref name="analyzerType"/>.
        /// </summary>
        /// <param name="analyzerType">The type of <see cref="DiagnosticAnalyzer"/> to check <paramref name="code"/> with.</param>
        /// <param name="code">
        /// The code to create the solution from.
        /// Can be a .cs, .csproj or .sln file.
        /// </param>
        /// <param name="suppressWarnings">A collection of <see cref="DiagnosticDescriptor.Id"/> to suppress when analyzing the code. Default is <see langword="null" /> meaning <see cref="SuppressedDiagnostics"/> are used.</param>
        /// <param name="metadataReferences">A collection of <see cref="MetadataReference"/> to use when compiling. Default is <see langword="null" /> meaning <see cref="MetadataReferences"/> are used.</param>
        /// <param name="compilationOptions">The <see cref="CSharpCompilationOptions"/>.</param>
        public static void Valid(
            Type analyzerType,
            FileInfo code,
            IEnumerable<string>? suppressWarnings = null,
            IEnumerable<MetadataReference>? metadataReferences = null,
            CSharpCompilationOptions? compilationOptions = null)
        {
            if (analyzerType is null)
            {
                throw new ArgumentNullException(nameof(analyzerType));
            }

            if (code is null)
            {
                throw new ArgumentNullException(nameof(code));
            }

            Valid(
                (DiagnosticAnalyzer)Activator.CreateInstance(analyzerType, nonPublic: true)!,
                code,
                suppressWarnings,
                metadataReferences,
                compilationOptions);
        }

        /// <summary>
        /// Verifies that <paramref name="code"/> produces no diagnostics when analyzed with <paramref name="analyzerType"/>.
        /// </summary>
        /// <param name="analyzerType">The type of <see cref="DiagnosticAnalyzer"/> to check <paramref name="code"/> with.</param>
        /// <param name="descriptor">The <see cref="DiagnosticDescriptor"/> with information about the expected <see cref="Diagnostic"/>. If <paramref name="analyzerType"/> supports more than one <see cref="DiagnosticDescriptor.Id"/> this must be provided.</param>
        /// <param name="code">
        /// The code to create the solution from.
        /// Can be a .cs, .csproj or .sln file.
        /// </param>
        /// <param name="suppressWarnings">A collection of <see cref="DiagnosticDescriptor.Id"/> to suppress when analyzing the code. Default is <see langword="null" /> meaning <see cref="SuppressedDiagnostics"/> are used.</param>
        /// <param name="metadataReferences">A collection of <see cref="MetadataReference"/> to use when compiling. Default is <see langword="null" /> meaning <see cref="MetadataReferences"/> are used.</param>
        /// <param name="compilationOptions">The <see cref="CSharpCompilationOptions"/>.</param>
        public static void Valid(
            Type analyzerType,
            DiagnosticDescriptor descriptor,
            FileInfo code,
            IEnumerable<string>? suppressWarnings = null,
            IEnumerable<MetadataReference>? metadataReferences = null,
            CSharpCompilationOptions? compilationOptions = null)
        {
            if (analyzerType is null)
            {
                throw new ArgumentNullException(nameof(analyzerType));
            }

            if (descriptor is null)
            {
                throw new ArgumentNullException(nameof(descriptor));
            }

            if (code is null)
            {
                throw new ArgumentNullException(nameof(code));
            }

            Valid(
                (DiagnosticAnalyzer)Activator.CreateInstance(analyzerType, nonPublic: true)!,
                descriptor,
                code,
                suppressWarnings,
                metadataReferences,
                compilationOptions);
        }

        /// <summary>
        /// Verifies that <paramref name="solution"/> produces no diagnostics when analyzed with <paramref name="analyzer"/>.
        /// </summary>
        /// <param name="analyzer">The <see cref="DiagnosticAnalyzer"/> to check <paramref name="solution"/> with.</param>
        /// <param name="solution">The <see cref="Solution"/> for which no errors or warnings are expected.</param>
        public static void Valid(DiagnosticAnalyzer analyzer, Solution solution)
        {
            if (analyzer is null)
            {
                throw new ArgumentNullException(nameof(analyzer));
            }

            if (solution is null)
            {
                throw new ArgumentNullException(nameof(solution));
            }

            var diagnosticsAndErrors = Analyze.GetDiagnosticsAndErrors(analyzer, solution);
            NoDiagnosticsOrErrors(diagnosticsAndErrors);
        }

        /// <summary>
        /// Assert that <paramref name="diagnostics"/> is empty. Throw an AssertException with details if not.
        /// </summary>
        /// <param name="diagnostics">The diagnostics.</param>
        public static void NoDiagnostics(IReadOnlyList<ImmutableArray<Diagnostic>> diagnostics)
        {
            if (diagnostics is null)
            {
                throw new ArgumentNullException(nameof(diagnostics));
            }

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

        private static void NoDiagnosticsOrErrors(Analyze.DiagnosticsAndErrors diagnosticsAndErrors)
        {
            NoDiagnostics(diagnosticsAndErrors.AnalyzerDiagnostics);
#pragma warning disable CS0618 // Suppress until removed. Will be replaced with MetadataReferences.FromAttributes()
            NoCompilerErrors(diagnosticsAndErrors.Errors, SuppressedDiagnostics, DiagnosticSettings.AllowedDiagnostics());
#pragma warning restore CS0618 // Suppress until removed. Will be replaced with MetadataReferences.FromAttributes()
        }
    }
}
