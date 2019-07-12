namespace Gu.Roslyn.Asserts
{
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
        /// 1. <paramref name="code"/> produces the expected diagnostics
        /// 2. The code fix does not change the code.
        /// </summary>
        /// <param name="analyzer">The <see cref="DiagnosticAnalyzer"/> to check <paramref name="code"/> with.</param>
        /// <param name="fix">The <see cref="CodeFixProvider"/> to apply on the <see cref="Diagnostic"/> reported.</param>
        /// <param name="code">The code to analyze with <paramref name="analyzer"/>. Indicate error position with ↓ (alt + 25).</param>
        public static void NoFix(DiagnosticAnalyzer analyzer, CodeFixProvider fix, params string[] code)
        {
            NoFix(
                analyzer,
                fix,
                DiagnosticsAndSources.CreateFromCodeWithErrorsIndicated(analyzer, code));
        }

        /// <summary>
        /// Verifies that
        /// 1. <paramref name="code"/> produces the expected diagnostics
        /// 2. The code fix does not change the code.
        /// </summary>
        /// <param name="analyzer">The <see cref="DiagnosticAnalyzer"/> to check <paramref name="code"/> with.</param>
        /// <param name="fix">The <see cref="CodeFixProvider"/> to apply on the <see cref="Diagnostic"/> reported.</param>
        /// <param name="expectedDiagnostic">The <see cref="ExpectedDiagnostic"/> with information about the expected <see cref="Diagnostic"/>. If <paramref name="analyzer"/> supports more than one <see cref="DiagnosticDescriptor.Id"/> this must be provided.</param>
        /// <param name="code">The code to analyze with <paramref name="analyzer"/>. Indicate error position with ↓ (alt + 25).</param>
        public static void NoFix(DiagnosticAnalyzer analyzer, CodeFixProvider fix, ExpectedDiagnostic expectedDiagnostic, params string[] code)
        {
            NoFix(
                analyzer,
                fix,
                DiagnosticsAndSources.Create(expectedDiagnostic, code));
        }

        /// <summary>
        /// Verifies that
        /// 1. <paramref name="code"/> produces the expected diagnostics
        /// 2. The code fix does not change the code.
        /// </summary>
        /// <param name="analyzer">The <see cref="DiagnosticAnalyzer"/> to check <paramref name="code"/> with.</param>
        /// <param name="fix">The <see cref="CodeFixProvider"/> to apply on the <see cref="Diagnostic"/> reported.</param>
        /// <param name="expectedDiagnostic">The <see cref="ExpectedDiagnostic"/> with information about the expected <see cref="Diagnostic"/>. If <paramref name="analyzer"/> supports more than one <see cref="DiagnosticDescriptor.Id"/> this must be provided.</param>
        /// <param name="code">The code to analyze with <paramref name="analyzer"/>. Indicate error position with ↓ (alt + 25).</param>
        /// <param name="allowCompilationErrors">Specify if compilation errors are accepted in the fixed code. This can be for example syntax errors. Default value is <see cref="AllowCompilationErrors.No"/>.</param>
        /// <param name="suppressedDiagnostics">A collection of <see cref="DiagnosticDescriptor.Id"/> to suppress when analyzing the code. Default is <see langword="null" /> meaning <see cref="SuppressedDiagnostics"/> are used.</param>
        /// <param name="metadataReferences">A collection of <see cref="MetadataReference"/> to use when compiling. Default is <see langword="null" /> meaning <see cref="MetadataReferences"/> are used.</param>
        /// <param name="compilationOptions">The <see cref="CSharpCompilationOptions"/>.</param>
        public static void NoFix(
            DiagnosticAnalyzer analyzer,
            CodeFixProvider fix,
            ExpectedDiagnostic expectedDiagnostic,
            IReadOnlyList<string> code,
            AllowCompilationErrors allowCompilationErrors = AllowCompilationErrors.No,
            IEnumerable<string> suppressedDiagnostics = null,
            IEnumerable<MetadataReference> metadataReferences = null,
            CSharpCompilationOptions compilationOptions = null)
        {
            NoFix(
                analyzer: analyzer,
                fix: fix,
                diagnosticsAndSources: DiagnosticsAndSources.Create(expectedDiagnostic, code),
                allowCompilationErrors: allowCompilationErrors,
                suppressedDiagnostics: suppressedDiagnostics,
                metadataReferences: metadataReferences,
                compilationOptions: compilationOptions);
        }

        /// <summary>
        /// Verifies that
        /// 1. <paramref name="code"/> produces the expected diagnostics
        /// 2. The code fix does not change the code.
        /// </summary>
        /// <param name="analyzer">The <see cref="DiagnosticAnalyzer"/> to check <paramref name="code"/> with.</param>
        /// <param name="fix">The <see cref="CodeFixProvider"/> to apply on the <see cref="Diagnostic"/> reported.</param>
        /// <param name="expectedDiagnostics">The <see cref="ExpectedDiagnostic"/> with information about the expected <see cref="Diagnostic"/>. If <paramref name="analyzer"/> supports more than one <see cref="DiagnosticDescriptor.Id"/> this must be provided.</param>
        /// <param name="code">The code to analyze with <paramref name="analyzer"/>. Indicate error position with ↓ (alt + 25).</param>
        public static void NoFix(DiagnosticAnalyzer analyzer, CodeFixProvider fix, IReadOnlyList<ExpectedDiagnostic> expectedDiagnostics, params string[] code)
        {
            NoFix(
                analyzer: analyzer,
                fix: fix,
                diagnosticsAndSources: new DiagnosticsAndSources(expectedDiagnostics, code));
        }

        /// <summary>
        /// Verifies that
        /// 1. <paramref name="code"/> produces the expected diagnostics
        /// 2. The code fix does not change the code.
        /// </summary>
        /// <param name="analyzer">The <see cref="DiagnosticAnalyzer"/> to check <paramref name="code"/> with.</param>
        /// <param name="fix">The <see cref="CodeFixProvider"/> to apply on the <see cref="Diagnostic"/> reported.</param>
        /// <param name="code">The code to analyze with <paramref name="analyzer"/>. Indicate error position with ↓ (alt + 25).</param>
        /// <param name="allowCompilationErrors">Specify if compilation errors are accepted in the fixed code. This can be for example syntax errors. Default value is <see cref="AllowCompilationErrors.No"/>.</param>
        /// <param name="suppressedDiagnostics">A collection of <see cref="DiagnosticDescriptor.Id"/> to suppress when analyzing the code. Default is <see langword="null" /> meaning <see cref="SuppressedDiagnostics"/> are used.</param>
        /// <param name="metadataReferences">A collection of <see cref="MetadataReference"/> to use when compiling. Default is <see langword="null" /> meaning <see cref="MetadataReferences"/> are used.</param>
        /// <param name="compilationOptions">The <see cref="CSharpCompilationOptions"/>.</param>
        public static void NoFix(
            DiagnosticAnalyzer analyzer,
            CodeFixProvider fix,
            IReadOnlyList<string> code,
            AllowCompilationErrors allowCompilationErrors = AllowCompilationErrors.No,
            IEnumerable<string> suppressedDiagnostics = null,
            IEnumerable<MetadataReference> metadataReferences = null,
            CSharpCompilationOptions compilationOptions = null)
        {
            NoFix(
                analyzer: analyzer,
                fix: fix,
                diagnosticsAndSources: DiagnosticsAndSources.CreateFromCodeWithErrorsIndicated(analyzer, code),
                allowCompilationErrors: allowCompilationErrors,
                suppressedDiagnostics: suppressedDiagnostics,
                metadataReferences: metadataReferences,
                compilationOptions: compilationOptions);
        }

        /// <summary>
        /// Verifies that
        /// 1. <paramref name="code"/> produces the expected diagnostics
        /// 2. The code fix does not change the code.
        /// </summary>
        /// <param name="fix">The <see cref="CodeFixProvider"/> to apply on the <see cref="Diagnostic"/> reported.</param>
        /// <param name="expectedDiagnostic">The <see cref="ExpectedDiagnostic"/> with information about the expected <see cref="Diagnostic"/>.</param>
        /// <param name="code">The code to analyze. Indicate error position with ↓ (alt + 25).</param>
        /// <param name="allowCompilationErrors">Specify if compilation errors are accepted in the fixed code. This can be for example syntax errors. Default value is <see cref="AllowCompilationErrors.No"/>.</param>
        /// <param name="suppressedDiagnostics">A collection of <see cref="DiagnosticDescriptor.Id"/> to suppress when analyzing the code. Default is <see langword="null" /> meaning <see cref="SuppressedDiagnostics"/> are used.</param>
        /// <param name="metadataReferences">A collection of <see cref="MetadataReference"/> to use when compiling. Default is <see langword="null" /> meaning <see cref="MetadataReferences"/> are used.</param>
        /// <param name="compilationOptions">The <see cref="CSharpCompilationOptions"/>.</param>
        public static void NoFix(
            CodeFixProvider fix,
            ExpectedDiagnostic expectedDiagnostic,
            IReadOnlyList<string> code,
            AllowCompilationErrors allowCompilationErrors = AllowCompilationErrors.No,
            IEnumerable<string> suppressedDiagnostics = null,
            IEnumerable<MetadataReference> metadataReferences = null,
            CSharpCompilationOptions compilationOptions = null)
        {
            var analyzer = new PlaceholderAnalyzer(expectedDiagnostic.Id);
            NoFix(
                analyzer: analyzer,
                fix: fix,
                diagnosticsAndSources: DiagnosticsAndSources.CreateFromCodeWithErrorsIndicated(analyzer, code),
                allowCompilationErrors: allowCompilationErrors,
                suppressedDiagnostics: suppressedDiagnostics,
                metadataReferences: metadataReferences,
                compilationOptions: compilationOptions);
        }

        /// <summary>
        /// Verifies that
        /// 1. <paramref name="diagnosticsAndSources"/> produces the expected diagnostics
        /// 2. The code fix does not change the code.
        /// </summary>
        /// <param name="analyzer">The <see cref="DiagnosticAnalyzer"/> to check <paramref name="diagnosticsAndSources"/> with.</param>
        /// <param name="fix">The <see cref="CodeFixProvider"/> to apply on the <see cref="Diagnostic"/> reported.</param>
        /// <param name="diagnosticsAndSources">The code to analyze with <paramref name="analyzer"/>. Indicate error position with ↓ (alt + 25).</param>
        /// <param name="allowCompilationErrors">Specify if compilation errors are accepted in the fixed code. This can be for example syntax errors. Default value is <see cref="AllowCompilationErrors.No"/>.</param>
        /// <param name="suppressedDiagnostics">A collection of <see cref="DiagnosticDescriptor.Id"/> to suppress when analyzing the code. Default is <see langword="null" /> meaning <see cref="SuppressedDiagnostics"/> are used.</param>
        /// <param name="metadataReferences">A collection of <see cref="MetadataReference"/> to use when compiling. Default is <see langword="null" /> meaning <see cref="MetadataReferences"/> are used.</param>
        /// <param name="compilationOptions">The <see cref="CSharpCompilationOptions"/>.</param>
        public static void NoFix(
            DiagnosticAnalyzer analyzer,
            CodeFixProvider fix,
            DiagnosticsAndSources diagnosticsAndSources,
            AllowCompilationErrors allowCompilationErrors = AllowCompilationErrors.No,
            IEnumerable<string> suppressedDiagnostics = null,
            IEnumerable<MetadataReference> metadataReferences = null,
            CSharpCompilationOptions compilationOptions = null)
        {
            VerifyAnalyzerSupportsDiagnostics(analyzer, diagnosticsAndSources.ExpectedDiagnostics);
            VerifyCodeFixSupportsAnalyzer(analyzer, fix);
            var sln = CodeFactory.CreateSolution(
                diagnosticsAndSources,
                analyzer,
                compilationOptions,
                suppressedDiagnostics ?? SuppressedDiagnostics,
                metadataReferences ?? MetadataReferences);
            var diagnostics = Analyze.GetDiagnostics(analyzer, sln);
            VerifyDiagnostics(diagnosticsAndSources, diagnostics, sln);
            if (allowCompilationErrors == AllowCompilationErrors.No)
            {
                NoCompilerErrors(sln);
            }

            VerifyNoFix(sln, diagnostics, fix);
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
