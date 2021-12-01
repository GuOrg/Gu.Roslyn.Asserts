namespace Gu.Roslyn.Asserts
{
    using System;
    using System.Collections.Generic;
    using System.Collections.Immutable;
    using System.IO;
    using System.Linq;

    using Gu.Roslyn.Asserts.Internals;

    using Microsoft.CodeAnalysis;
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
                CodeFactory.CreateSolution(code, analyzer, Settings.Default));
        }

        /// <summary>
        /// Verifies that <paramref name="code"/> produces no diagnostics when analyzed with <paramref name="analyzer"/>.
        /// </summary>
        /// <param name="analyzer">The <see cref="DiagnosticAnalyzer"/> to check <paramref name="code"/> with.</param>
        /// <param name="code">The code to analyze using <paramref name="analyzer"/>. Analyzing the code is expected to produce no errors or warnings.</param>
        /// <param name="settings">The <see cref="Settings"/>.</param>
        public static void Valid(
            DiagnosticAnalyzer analyzer,
            IReadOnlyList<string> code,
            Settings? settings = null)
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
                CodeFactory.CreateSolution(code, analyzer, settings ?? Settings.Default));
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
                CodeFactory.CreateSolution(code, analyzer, descriptor, Settings.Default));
        }

        /// <summary>
        /// Verifies that <paramref name="code"/> produces no diagnostics when analyzed with <paramref name="analyzer"/>.
        /// </summary>
        /// <param name="analyzer">The <see cref="DiagnosticAnalyzer"/> to check <paramref name="code"/> with.</param>
        /// <param name="descriptor">The <see cref="DiagnosticDescriptor"/> with information about the expected <see cref="Diagnostic"/>. If <paramref name="analyzer"/> supports more than one <see cref="DiagnosticDescriptor.Id"/> this must be provided.</param>
        /// <param name="code">The code to analyze using <paramref name="analyzer"/>. Analyzing the code is expected to produce no errors or warnings.</param>
        /// <param name="settings">The <see cref="Settings"/>.</param>
        public static void Valid(
            DiagnosticAnalyzer analyzer,
            DiagnosticDescriptor descriptor,
            string code,
            Settings? settings = null)
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
                CodeFactory.CreateSolution(new[] { code }, analyzer, descriptor, settings ?? Settings.Default));
        }

        /// <summary>
        /// Verifies that <paramref name="code"/> produces no diagnostics when analyzed with <paramref name="analyzer"/>.
        /// </summary>
        /// <param name="analyzer">The <see cref="DiagnosticAnalyzer"/> to check <paramref name="code"/> with.</param>
        /// <param name="descriptor">The <see cref="DiagnosticDescriptor"/> with information about the expected <see cref="Diagnostic"/>. If <paramref name="analyzer"/> supports more than one <see cref="DiagnosticDescriptor.Id"/> this must be provided.</param>
        /// <param name="code">The code to analyze using <paramref name="analyzer"/>. Analyzing the code is expected to produce no errors or warnings.</param>
        /// <param name="settings">The <see cref="Settings"/>.</param>
        public static void Valid(
            DiagnosticAnalyzer analyzer,
            DiagnosticDescriptor descriptor,
            IReadOnlyList<string> code,
            Settings? settings = null)
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
                CodeFactory.CreateSolution(code, analyzer, descriptor, settings ?? Settings.Default));
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
        /// <param name="settings">The <see cref="Settings"/>.</param>
        public static void Valid(
            DiagnosticAnalyzer analyzer,
            DiagnosticDescriptor descriptor,
            FileInfo code,
            Settings? settings = null)
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
                CodeFactory.CreateSolution(code, analyzer, descriptor, settings ?? Settings.Default));
        }

        /// <summary>
        /// Verifies that <paramref name="code"/> produces no diagnostics when analyzed with <paramref name="analyzer"/>.
        /// </summary>
        /// <param name="analyzer">The <see cref="DiagnosticAnalyzer"/> to check <paramref name="code"/> with.</param>
        /// <param name="code">
        /// The code to create the solution from.
        /// Can be a .cs, .csproj or .sln file.
        /// </param>
        /// <param name="settings">The <see cref="Settings"/>.</param>
        public static void Valid(
            DiagnosticAnalyzer analyzer,
            FileInfo code,
            Settings? settings = null)
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
                CodeFactory.CreateSolution(code, analyzer, settings ?? Settings.Default));
        }

        /// <summary>
        /// Verifies that <paramref name="code"/> produces no diagnostics when analyzed with <paramref name="analyzer"/>.
        /// </summary>
        /// <param name="analyzer">The <see cref="DiagnosticAnalyzer"/> to check <paramref name="code"/> with.</param>
        /// <param name="code">The code to analyze using <paramref name="analyzer"/>. Analyzing the code is expected to produce no errors or warnings.</param>
        /// <param name="settings">The <see cref="Settings"/>.</param>
        public static void Valid(
            DiagnosticAnalyzer analyzer,
            string code,
            Settings? settings = null)
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
                CodeFactory.CreateSolution(new[] { code }, analyzer, settings ?? Settings.Default));
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
        /// <param name="settings">The <see cref="Settings"/>.</param>
        public static void Valid(
            Type analyzerType,
            FileInfo code,
            Settings? settings = null)
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
                settings);
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
        /// <param name="settings">The <see cref="Settings"/>.</param>
        public static void Valid(
            Type analyzerType,
            DiagnosticDescriptor descriptor,
            FileInfo code,
            Settings? settings = null)
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
                settings);
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
