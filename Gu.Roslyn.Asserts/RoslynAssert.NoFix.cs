namespace Gu.Roslyn.Asserts
{
    using System;
    using System.Collections.Generic;
    using System.Collections.Immutable;
    using System.Linq;
    using Gu.Roslyn.Asserts.Internals;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CodeFixes;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.Diagnostics;

    public static partial class RoslynAssert
    {
        /// <summary>
        /// Verifies that
        /// 1. <paramref name="codeWithErrorsIndicated"/> produces the expected diagnostics
        /// 2. The code fix does not change the code.
        /// </summary>
        /// <typeparam name="TAnalyzer">The type of the analyzer.</typeparam>
        /// <typeparam name="TCodeFix">The type of the code fix.</typeparam>
        /// <param name="codeWithErrorsIndicated">The code with error positions indicated.</param>
        public static void NoFix<TAnalyzer, TCodeFix>(params string[] codeWithErrorsIndicated)
            where TAnalyzer : DiagnosticAnalyzer, new()
            where TCodeFix : CodeFixProvider, new()
        {
            NoFix(
                new TAnalyzer(),
                new TCodeFix(),
                DiagnosticsAndSources.CreateFromCodeWithErrorsIndicated(new TAnalyzer(), codeWithErrorsIndicated),
                SuppressedDiagnostics,
                MetadataReferences);
        }

        /// <summary>
        /// Verifies that
        /// 1. <paramref name="code"/> produces the expected diagnostics
        /// 2. The code fix does not change the code.
        /// </summary>
        /// <typeparam name="TCodeFix">The type of the code fix.</typeparam>
        /// <param name="expectedDiagnostic">The expected diagnostic.</param>
        /// <param name="code">The code to analyze.</param>
        public static void NoFix<TCodeFix>(ExpectedDiagnostic expectedDiagnostic, params string[] code)
            where TCodeFix : CodeFixProvider, new()
        {
            NoFix(
                new PlaceholderAnalyzer(expectedDiagnostic.Id),
                new TCodeFix(),
                DiagnosticsAndSources.Create(expectedDiagnostic, code),
                SuppressedDiagnostics,
                MetadataReferences);
        }

        /// <summary>
        /// Verifies that
        /// 1. <paramref name="codeWithErrorsIndicated"/> produces the expected diagnostics
        /// 2. The code fix does not change the code.
        /// </summary>
        /// <typeparam name="TAnalyzer">The type of the analyzer.</typeparam>
        /// <typeparam name="TCodeFix">The type of the code fix.</typeparam>
        /// <param name="codeWithErrorsIndicated">The code to analyze.</param>
        public static void NoFix<TAnalyzer, TCodeFix>(IReadOnlyList<string> codeWithErrorsIndicated)
            where TAnalyzer : DiagnosticAnalyzer, new()
            where TCodeFix : CodeFixProvider, new()
        {
            NoFix(
                new TAnalyzer(),
                new TCodeFix(),
                DiagnosticsAndSources.CreateFromCodeWithErrorsIndicated(new TAnalyzer(), codeWithErrorsIndicated),
                SuppressedDiagnostics,
                MetadataReferences);
        }

        /// <summary>
        /// Verifies that
        /// 1. <paramref name="code"/> produces the expected diagnostics
        /// 2. The code fix does not change the code.
        /// </summary>
        /// <typeparam name="TAnalyzer">The type of the analyzer.</typeparam>
        /// <typeparam name="TCodeFix">The type of the code fix.</typeparam>
        /// <param name="expectedDiagnostic">The expected diagnostic.</param>
        /// <param name="code">The code to analyze.</param>
        public static void NoFix<TAnalyzer, TCodeFix>(ExpectedDiagnostic expectedDiagnostic, params string[] code)
            where TAnalyzer : DiagnosticAnalyzer, new()
            where TCodeFix : CodeFixProvider, new()
        {
            NoFix(
                new TAnalyzer(),
                new TCodeFix(),
                DiagnosticsAndSources.Create(expectedDiagnostic, code),
                SuppressedDiagnostics,
                MetadataReferences);
        }

        /// <summary>
        /// Verifies that
        /// 1. <paramref name="codeWithErrorsIndicated"/> produces the expected diagnostics
        /// 2. The code fix does not change the code.
        /// </summary>
        /// <param name="analyzer">The type of the analyzer.</param>
        /// <param name="codeFix">The type of the code fix.</param>
        /// <param name="codeWithErrorsIndicated">The code with error positions indicated.</param>
        public static void NoFix(DiagnosticAnalyzer analyzer, CodeFixProvider codeFix, params string[] codeWithErrorsIndicated)
        {
            NoFix(
                analyzer,
                codeFix,
                DiagnosticsAndSources.CreateFromCodeWithErrorsIndicated(analyzer, codeWithErrorsIndicated),
                SuppressedDiagnostics,
                MetadataReferences);
        }

        /// <summary>
        /// Verifies that
        /// 1. <paramref name="code"/> produces the expected diagnostics
        /// 2. The code fix does not change the code.
        /// </summary>
        /// <param name="analyzer">The type of the analyzer.</param>
        /// <param name="codeFix">The type of the code fix.</param>
        /// <param name="expectedDiagnostic">The expected diagnostic.</param>
        /// <param name="code">The code to analyze.</param>
        public static void NoFix(DiagnosticAnalyzer analyzer, CodeFixProvider codeFix, ExpectedDiagnostic expectedDiagnostic, params string[] code)
        {
            NoFix(
                analyzer,
                codeFix,
                DiagnosticsAndSources.Create(expectedDiagnostic, code),
                SuppressedDiagnostics,
                MetadataReferences);
        }

        /// <summary>
        /// Verifies that
        /// 1. <paramref name="code"/> produces the expected diagnostics
        /// 2. The code fix does not change the code.
        /// </summary>
        /// <param name="analyzer">The type of the analyzer.</param>
        /// <param name="codeFix">The type of the code fix.</param>
        /// <param name="expectedDiagnostic">The expected diagnostic.</param>
        /// <param name="code">The code to analyze.</param>
        public static void NoFix(DiagnosticAnalyzer analyzer, CodeFixProvider codeFix, ExpectedDiagnostic expectedDiagnostic, IReadOnlyList<string> code)
        {
            NoFix(
                analyzer,
                codeFix,
                DiagnosticsAndSources.Create(expectedDiagnostic, code),
                SuppressedDiagnostics,
                MetadataReferences);
        }

        /// <summary>
        /// Verifies that
        /// 1. <paramref name="code"/> produces the expected diagnostics
        /// 2. The code fix does not change the code.
        /// </summary>
        /// <param name="analyzer">The type of the analyzer.</param>
        /// <param name="codeFix">The type of the code fix.</param>
        /// <param name="expectedDiagnostics">The expected diagnostic.</param>
        /// <param name="code">The code to analyze.</param>
        public static void NoFix(DiagnosticAnalyzer analyzer, CodeFixProvider codeFix, IReadOnlyList<ExpectedDiagnostic> expectedDiagnostics, params string[] code)
        {
            NoFix(
                analyzer,
                codeFix,
                new DiagnosticsAndSources(expectedDiagnostics, code),
                SuppressedDiagnostics,
                MetadataReferences);
        }

        /// <summary>
        /// Verifies that
        /// 1. <paramref name="diagnosticsAndSources"/> produces the expected diagnostics
        /// 2. The code fix does not change the code.
        /// </summary>
        /// <param name="analyzer">The type of the analyzer.</param>
        /// <param name="codeFix">The type of the code fix.</param>
        /// <param name="diagnosticsAndSources">The code to analyze.</param>
        /// <param name="suppressedDiagnostics">Ids of diagnostics to suppress.</param>
        /// <param name="metadataReferences">Collection of <see cref="MetadataReference"/> to use when compiling.</param>
        public static void NoFix(DiagnosticAnalyzer analyzer, CodeFixProvider codeFix, DiagnosticsAndSources diagnosticsAndSources, IEnumerable<string> suppressedDiagnostics, IEnumerable<MetadataReference> metadataReferences)
        {
            VerifyAnalyzerSupportsDiagnostics(analyzer, diagnosticsAndSources.ExpectedDiagnostics);
            VerifyCodeFixSupportsAnalyzer(analyzer, codeFix);
            var sln = CodeFactory.CreateSolution(diagnosticsAndSources, analyzer, null, suppressedDiagnostics, metadataReferences);
            var diagnostics = Analyze.GetDiagnostics(analyzer, sln);
            VerifyDiagnostics(diagnosticsAndSources, diagnostics);
            VerifyNoFix(sln, diagnostics, codeFix);
        }

        /// <summary>
        /// Verifies that
        /// 1. <paramref name="codeWithErrorsIndicated"/> produces the expected diagnostics
        /// 2. The code fix does not change the code.
        /// </summary>
        /// <param name="analyzer">The type of the analyzer.</param>
        /// <param name="codeFix">The type of the code fix.</param>
        /// <param name="codeWithErrorsIndicated">The code to analyze.</param>
        /// <param name="compilationOptions">The <see cref="CSharpCompilationOptions"/> to use.</param>
        /// <param name="metadataReferences">The meta data references to use when compiling.</param>
        public static void NoFix(DiagnosticAnalyzer analyzer, CodeFixProvider codeFix, IReadOnlyList<string> codeWithErrorsIndicated, CSharpCompilationOptions compilationOptions, IEnumerable<MetadataReference> metadataReferences)
        {
            var diagnosticsAndSources = DiagnosticsAndSources.CreateFromCodeWithErrorsIndicated(analyzer, codeWithErrorsIndicated);
            VerifyAnalyzerSupportsDiagnostics(analyzer, diagnosticsAndSources.ExpectedDiagnostics);
            VerifyCodeFixSupportsAnalyzer(analyzer, codeFix);
            var sln = CodeFactory.CreateSolution(diagnosticsAndSources.Code, compilationOptions, metadataReferences);
            var diagnostics = Analyze.GetDiagnostics(analyzer, sln);
            if (diagnostics.SelectMany(x => x).All(d => !codeFix.FixableDiagnosticIds.Contains(d.Id)))
            {
                throw new InvalidOperationException("Analyzing the code did not produce any diagnostics fixable by the code fix.");
            }

            VerifyNoFix(sln, diagnostics, codeFix);
        }

        /// <summary>
        /// Verifies that
        /// 1. <paramref name="codeWithErrorsIndicated"/> produces the expected diagnostics
        /// 2. The code fix does not change the code.
        /// </summary>
        /// <param name="codeFix">The type of the code fix.</param>
        /// <param name="codeWithErrorsIndicated">The code to analyze.</param>
        /// <param name="compilationOptions">The <see cref="CSharpCompilationOptions"/> to use.</param>
        /// <param name="metadataReferences">The meta data references to use when compiling.</param>
        public static void NoFix(CodeFixProvider codeFix, IReadOnlyList<string> codeWithErrorsIndicated, CSharpCompilationOptions compilationOptions, IEnumerable<MetadataReference> metadataReferences)
        {
            var diagnosticsAndSources = DiagnosticsAndSources.CreateFromCodeWithErrorsIndicated(new PlaceholderAnalyzer("None"), codeWithErrorsIndicated);
            var sln = CodeFactory.CreateSolution(diagnosticsAndSources.Code, compilationOptions, metadataReferences);
            var diagnostics = Analyze.GetDiagnostics(sln);
            if (diagnostics.SelectMany(x => x).All(d => !codeFix.FixableDiagnosticIds.Contains(d.Id)))
            {
                throw new InvalidOperationException("Analyzing the code did not produce any diagnostics fixable by the code fix.");
            }

            VerifyNoFix(sln, diagnostics, codeFix);
        }

        private static void VerifyNoFix(Solution sln, IReadOnlyList<ImmutableArray<Diagnostic>> diagnostics, CodeFixProvider fix)
        {
            var fixableDiagnostics = diagnostics.SelectMany(x => x)
                                         .Where(x => fix.FixableDiagnosticIds.Contains(x.Id))
                                         .ToArray();

            foreach (var fixableDiagnostic in fixableDiagnostics)
            {
                var actions = Fix.GetActions(sln, fix, fixableDiagnostic);
                if (actions.Any())
                {
                    var builder = StringBuilderPool.Borrow()
                                                   .AppendLine("Expected code to have no fixable diagnostics.")
                                                   .AppendLine("The following actions were registered:");

                    foreach (var action in actions)
                    {
                        builder.AppendLine(action.Title);
                    }

                    throw new AssertException(builder.Return());
                }
            }
        }
    }
}
