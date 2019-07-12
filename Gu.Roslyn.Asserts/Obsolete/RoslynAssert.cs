namespace Gu.Roslyn.Asserts
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CodeFixes;
    using Microsoft.CodeAnalysis.Diagnostics;

    public static partial class RoslynAssert
    {
        /// <summary>
        /// Verifies that <paramref name="codeWithErrorsIndicated"/> produces the expected diagnostics.
        /// </summary>
        /// <typeparam name="TAnalyzer">The type of <see cref="DiagnosticAnalyzer"/> to check <paramref name="codeWithErrorsIndicated"/> with.</typeparam>
        /// <param name="codeWithErrorsIndicated">The code with error positions indicated.</param>
        [Obsolete("Use overload taking analyzer as argument.")]
        public static void Diagnostics<TAnalyzer>(params string[] codeWithErrorsIndicated)
            where TAnalyzer : DiagnosticAnalyzer, new()
        {
            Diagnostics(
                new TAnalyzer(),
                DiagnosticsAndSources.CreateFromCodeWithErrorsIndicated(new TAnalyzer(), codeWithErrorsIndicated));
        }

        /// <summary>
        /// Verifies that <paramref name="code"/> produces the expected diagnostics.
        /// </summary>
        /// <typeparam name="TAnalyzer">The type of <see cref="DiagnosticAnalyzer"/> to check <paramref name="code"/> with.</typeparam>
        /// <param name="expectedDiagnostic">The <see cref="ExpectedDiagnostic"/> with information about the expected <see cref="Diagnostic"/>.</param>
        /// <param name="code">The code to analyze.</param>
        [Obsolete("Use overload taking analyzer as argument.")]
        public static void Diagnostics<TAnalyzer>(ExpectedDiagnostic expectedDiagnostic, params string[] code)
            where TAnalyzer : DiagnosticAnalyzer, new()
        {
            Diagnostics(
                new TAnalyzer(),
                DiagnosticsAndSources.Create(expectedDiagnostic, code));
        }

        /// <summary>
        /// Verifies that <paramref name="code"/> produces the expected diagnostics.
        /// </summary>
        /// <typeparam name="TAnalyzer">The type of <see cref="DiagnosticAnalyzer"/> to check <paramref name="code"/> with.</typeparam>
        /// <param name="expectedDiagnostics">The expected diagnostics.</param>
        /// <param name="code">The code to analyze.</param>
        [Obsolete("Use overload taking analyzer as argument.")]
        public static void Diagnostics<TAnalyzer>(IReadOnlyList<ExpectedDiagnostic> expectedDiagnostics, params string[] code)
            where TAnalyzer : DiagnosticAnalyzer, new()
        {
            Diagnostics(
                new TAnalyzer(),
                new DiagnosticsAndSources(expectedDiagnostics, code));
        }

        /// <summary>
        /// Verifies that
        /// 1. <paramref name="codeWithErrorsIndicated"/> produces the expected diagnostics
        /// 2. The code fix fixes the code.
        /// </summary>
        /// <typeparam name="TAnalyzer">The type of <see cref="DiagnosticAnalyzer"/> to check <paramref name="codeWithErrorsIndicated"/> with.</typeparam>
        /// <typeparam name="TCodeFix">The <see cref="CodeFixProvider"/> to apply on the <see cref="Diagnostic"/> reported.</typeparam>
        /// <param name="codeWithErrorsIndicated">The code with error positions indicated.</param>
        /// <param name="after">The expected code produced by applying <typeparamref name="TCodeFix"/>.</param>
        /// <param name="fixTitle">The expected title of the fix. Must be provided if more than one code action is registered.</param>
         /// <param name="allowCompilationErrors">Specify if compilation errors are accepted in the fixed code. This can be for example syntax errors. Default value is <see cref="AllowCompilationErrors.No"/>.</param>
        [Obsolete("Use overload taking analyzer & fix as arguments.")]
        public static void CodeFix<TAnalyzer, TCodeFix>(string codeWithErrorsIndicated, string after, string fixTitle = null, AllowCompilationErrors allowCompilationErrors = AllowCompilationErrors.No)
            where TAnalyzer : DiagnosticAnalyzer, new()
            where TCodeFix : CodeFixProvider, new()
        {
            var analyzer = new TAnalyzer();
            CodeFix(
                analyzer: analyzer,
                fix: new TCodeFix(),
                diagnosticsAndSources: DiagnosticsAndSources.CreateFromCodeWithErrorsIndicated(analyzer, codeWithErrorsIndicated),
                after: new[] { after },
                fixTitle: fixTitle,
                allowCompilationErrors: allowCompilationErrors,
                suppressedDiagnostics: null,
                metadataReferences: null,
                compilationOptions: null);
        }

        /// <summary>
        /// Verifies that
        /// 1. <paramref name="codeWithErrorsIndicated"/> produces the expected diagnostics
        /// 2. The code fix fixes the code.
        /// </summary>
        /// <typeparam name="TAnalyzer">The type of <see cref="DiagnosticAnalyzer"/> to check <paramref name="codeWithErrorsIndicated"/> with.</typeparam>
        /// <typeparam name="TCodeFix">The <see cref="CodeFixProvider"/> to apply on the <see cref="Diagnostic"/> reported.</typeparam>
        /// <param name="codeWithErrorsIndicated">The code with error positions indicated.</param>
        /// <param name="after">The expected code produced by applying <typeparamref name="TCodeFix"/>.</param>
        /// <param name="fixTitle">The expected title of the fix. Must be provided if more than one code action is registered.</param>
         /// <param name="allowCompilationErrors">Specify if compilation errors are accepted in the fixed code. This can be for example syntax errors. Default value is <see cref="AllowCompilationErrors.No"/>.</param>
        [Obsolete("Use overload taking analyzer & fix as arguments.")]
        public static void CodeFix<TAnalyzer, TCodeFix>(IReadOnlyList<string> codeWithErrorsIndicated, string after, string fixTitle = null, AllowCompilationErrors allowCompilationErrors = AllowCompilationErrors.No)
            where TAnalyzer : DiagnosticAnalyzer, new()
            where TCodeFix : CodeFixProvider, new()
        {
            var analyzer = new TAnalyzer();
            CodeFix(
                analyzer: analyzer,
                fix: new TCodeFix(),
                diagnosticsAndSources: DiagnosticsAndSources.CreateFromCodeWithErrorsIndicated(analyzer, codeWithErrorsIndicated),
                after: MergeFixedCode(codeWithErrorsIndicated, after),
                fixTitle: fixTitle,
                allowCompilationErrors: allowCompilationErrors,
                suppressedDiagnostics: null,
                metadataReferences: null,
                compilationOptions: null);
        }

        /// <summary>
        /// Verifies that
        /// 1. <paramref name="before"/> produces the expected diagnostics
        /// 2. The code fix fixes the code.
        /// </summary>
        /// <typeparam name="TAnalyzer">The type of <see cref="DiagnosticAnalyzer"/> to check <paramref name="before"/> with.</typeparam>
        /// <typeparam name="TCodeFix">The <see cref="CodeFixProvider"/> to apply on the <see cref="Diagnostic"/> reported.</typeparam>
        /// <param name="expectedDiagnostic">The <see cref="ExpectedDiagnostic"/> with information about the expected <see cref="Diagnostic"/>.</param>
        /// <param name="before">The code with error positions indicated.</param>
        /// <param name="after">The expected code produced by applying <typeparamref name="TCodeFix"/>.</param>
        /// <param name="fixTitle">The expected title of the fix. Must be provided if more than one code action is registered.</param>
        /// <param name="allowCompilationErrors">Specify if compilation errors are accepted in the fixed code. This can be for example syntax errors. Default value is <see cref="AllowCompilationErrors.No"/>.</param>
        [Obsolete("Use overload taking analyzer & fix as arguments.")]
        public static void CodeFix<TAnalyzer, TCodeFix>(ExpectedDiagnostic expectedDiagnostic, string before, string after, string fixTitle = null, AllowCompilationErrors allowCompilationErrors = AllowCompilationErrors.No)
            where TAnalyzer : DiagnosticAnalyzer, new()
            where TCodeFix : CodeFixProvider, new()
        {
            CodeFix(
                analyzer: new TAnalyzer(),
                fix: new TCodeFix(),
                diagnosticsAndSources: DiagnosticsAndSources.Create(expectedDiagnostic, before),
                after: new[] { after },
                fixTitle: fixTitle,
                allowCompilationErrors: allowCompilationErrors,
                suppressedDiagnostics: null,
                metadataReferences: null,
                compilationOptions: null);
        }

        /// <summary>
        /// Verifies that
        /// 1. <paramref name="code"/> produces the expected diagnostics
        /// 2. The code fix fixes the code.
        /// </summary>
        /// <typeparam name="TAnalyzer">The type of <see cref="DiagnosticAnalyzer"/> to check <paramref name="code"/> with.</typeparam>
        /// <typeparam name="TCodeFix">The <see cref="CodeFixProvider"/> to apply on the <see cref="Diagnostic"/> reported.</typeparam>
        /// <param name="expectedDiagnostic">The <see cref="ExpectedDiagnostic"/> with information about the expected <see cref="Diagnostic"/>.</param>
        /// <param name="code">The code with error positions indicated.</param>
        /// <param name="after">The expected code produced by applying <typeparamref name="TCodeFix"/>.</param>
        /// <param name="fixTitle">The expected title of the fix. Must be provided if more than one code action is registered.</param>
         /// <param name="allowCompilationErrors">Specify if compilation errors are accepted in the fixed code. This can be for example syntax errors. Default value is <see cref="AllowCompilationErrors.No"/>.</param>
        [Obsolete("Use overload taking analyzer & fix as arguments.")]
        public static void CodeFix<TAnalyzer, TCodeFix>(ExpectedDiagnostic expectedDiagnostic, IReadOnlyList<string> code, string after, string fixTitle = null, AllowCompilationErrors allowCompilationErrors = AllowCompilationErrors.No)
            where TAnalyzer : DiagnosticAnalyzer, new()
            where TCodeFix : CodeFixProvider, new()
        {
            CodeFix(
                analyzer: new TAnalyzer(),
                fix: new TCodeFix(),
                diagnosticsAndSources: DiagnosticsAndSources.Create(expectedDiagnostic, code),
                after: MergeFixedCode(code, after),
                fixTitle: fixTitle,
                allowCompilationErrors: allowCompilationErrors,
                suppressedDiagnostics: null,
                metadataReferences: null,
                compilationOptions: null);
        }

        /// <summary>
        /// Verifies that
        /// 1. <paramref name="before"/> produces the expected diagnostics
        /// 2. The code fix fixes the code.
        /// </summary>
        /// <typeparam name="TCodeFix">The <see cref="CodeFixProvider"/> to apply on the <see cref="Diagnostic"/> reported.</typeparam>
        /// <param name="expectedDiagnostic">The <see cref="ExpectedDiagnostic"/> with information about the expected <see cref="Diagnostic"/>.</param>
        /// <param name="before">The code to analyze.</param>
        /// <param name="after">The expected code produced by applying <typeparamref name="TCodeFix"/>.</param>
        /// <param name="fixTitle">The expected title of the fix. Must be provided if more than one code action is registered.</param>
         /// <param name="allowCompilationErrors">Specify if compilation errors are accepted in the fixed code. This can be for example syntax errors. Default value is <see cref="AllowCompilationErrors.No"/>.</param>
        [Obsolete("Use overload taking fix as argument.")]
        public static void CodeFix<TCodeFix>(ExpectedDiagnostic expectedDiagnostic, string before, string after, string fixTitle = null, AllowCompilationErrors allowCompilationErrors = AllowCompilationErrors.No)
            where TCodeFix : CodeFixProvider, new()
        {
            CodeFix(
                analyzer: new PlaceholderAnalyzer(expectedDiagnostic.Id),
                fix: new TCodeFix(),
                diagnosticsAndSources: DiagnosticsAndSources.Create(expectedDiagnostic, before),
                after: new[] { after },
                fixTitle: fixTitle,
                allowCompilationErrors: allowCompilationErrors,
                suppressedDiagnostics: null,
                metadataReferences: null,
                compilationOptions: null);
        }

        /// <summary>
        /// Verifies that
        /// 1. <paramref name="code"/> produces the expected diagnostics
        /// 2. The code fix fixes the code.
        /// </summary>
        /// <typeparam name="TCodeFix">The <see cref="CodeFixProvider"/> to apply on the <see cref="Diagnostic"/> reported.</typeparam>
        /// <param name="expectedDiagnostic">The <see cref="ExpectedDiagnostic"/> with information about the expected <see cref="Diagnostic"/>.</param>
        /// <param name="code">The code to analyze.</param>
        /// <param name="after">The expected code produced by applying <typeparamref name="TCodeFix"/>.</param>
        /// <param name="fixTitle">The expected title of the fix. Must be provided if more than one code action is registered.</param>
         /// <param name="allowCompilationErrors">Specify if compilation errors are accepted in the fixed code. This can be for example syntax errors. Default value is <see cref="AllowCompilationErrors.No"/>.</param>
        [Obsolete("Use overload taking fix as argument.")]
        public static void CodeFix<TCodeFix>(ExpectedDiagnostic expectedDiagnostic, IReadOnlyList<string> code, string after, string fixTitle = null, AllowCompilationErrors allowCompilationErrors = AllowCompilationErrors.No)
            where TCodeFix : CodeFixProvider, new()
        {
            CodeFix(
                analyzer: new PlaceholderAnalyzer(expectedDiagnostic.Id),
                fix: new TCodeFix(),
                diagnosticsAndSources: DiagnosticsAndSources.Create(expectedDiagnostic, code),
                after: MergeFixedCode(code, after),
                fixTitle: fixTitle,
                allowCompilationErrors: allowCompilationErrors,
                suppressedDiagnostics: null,
                metadataReferences: null,
                compilationOptions: null);
        }

        /// <summary>
        /// Verifies that
        /// 1. <paramref name="diagnosticsAndSources"/> produces the expected diagnostics
        /// 2. The code fix fixes the code.
        /// </summary>
        /// <param name="analyzer">The <see cref="DiagnosticAnalyzer"/> to check <paramref name="diagnosticsAndSources"/> with.</param>
        /// <param name="fix">The <see cref="CodeFixProvider"/> to apply on the <see cref="Diagnostic"/> reported.</param>
        /// <param name="diagnosticsAndSources">The code to analyze.</param>
        /// <param name="after">The expected code produced by applying <paramref name="fix"/> name="fix"/>.</param>
        /// <param name="fixTitle">The expected title of the fix. Must be provided if more than one code action is registered.</param>
         /// <param name="allowCompilationErrors">Specify if compilation errors are accepted in the fixed code. This can be for example syntax errors. Default value is <see cref="AllowCompilationErrors.No"/>.</param>
        [Obsolete("Use other overloads.")]
        public static void CodeFix(DiagnosticAnalyzer analyzer, CodeFixProvider fix, DiagnosticsAndSources diagnosticsAndSources, string after, string fixTitle = null, AllowCompilationErrors allowCompilationErrors = AllowCompilationErrors.No)
        {
            CodeFix(
                analyzer: analyzer,
                fix: fix,
                diagnosticsAndSources: diagnosticsAndSources,
                after: MergeFixedCode(diagnosticsAndSources.Code, after),
                fixTitle: fixTitle,
                allowCompilationErrors: allowCompilationErrors,
                suppressedDiagnostics: null,
                metadataReferences: null,
                compilationOptions: null);
        }

        /// <summary>
        /// Verifies that
        /// 1. <paramref name="diagnosticsAndSources"/> produces the expected diagnostics
        /// 2. The code fix fixes the code.
        /// </summary>
        /// <param name="analyzer">The <see cref="DiagnosticAnalyzer"/> to check <paramref name="diagnosticsAndSources"/> with.</param>
        /// <param name="fix">The <see cref="CodeFixProvider"/> to apply on the <see cref="Diagnostic"/> reported.</param>
        /// <param name="diagnosticsAndSources">The code to analyze.</param>
        /// <param name="after">The expected code produced by applying <paramref name="fix"/>.</param>
        /// <param name="suppressedDiagnostics">A collection of <see cref="DiagnosticDescriptor.Id"/> to suppress when analyzing the code. Default is <see langword="null" /> meaning <see cref="SuppressedDiagnostics"/> are used.</param>
        /// <param name="metadataReferences">A collection of <see cref="MetadataReference"/> to use when compiling. Default is <see langword="null" /> meaning <see cref="MetadataReferences"/> are used.</param>
        /// <param name="fixTitle">The expected title of the fix. Must be provided if more than one code action is registered.</param>
         /// <param name="allowCompilationErrors">Specify if compilation errors are accepted in the fixed code. This can be for example syntax errors. Default value is <see cref="AllowCompilationErrors.No"/>.</param>
        [Obsolete("Use other overloads.")]
        public static void CodeFix(DiagnosticAnalyzer analyzer, CodeFixProvider fix, DiagnosticsAndSources diagnosticsAndSources, string after, IEnumerable<string> suppressedDiagnostics = null, IEnumerable<MetadataReference> metadataReferences = null, string fixTitle = null, AllowCompilationErrors allowCompilationErrors = AllowCompilationErrors.No)
        {
            CodeFix(
                analyzer: analyzer,
                fix: fix,
                diagnosticsAndSources: diagnosticsAndSources,
                after: MergeFixedCode(diagnosticsAndSources.Code, after),
                fixTitle: fixTitle,
                allowCompilationErrors: allowCompilationErrors,
                suppressedDiagnostics: suppressedDiagnostics,
                metadataReferences: metadataReferences,
                compilationOptions: null);
        }

        /// <summary>
        /// Verifies that
        /// 1. <paramref name="before"/> produces the expected diagnostics
        /// 2. The code fix fixes the code.
        /// </summary>
        /// <typeparam name="TAnalyzer">The type of <see cref="DiagnosticAnalyzer"/> to check <paramref name="before"/> with.</typeparam>
        /// <typeparam name="TCodeFix">The <see cref="CodeFixProvider"/> to apply on the <see cref="Diagnostic"/> reported.</typeparam>
        /// <param name="expectedDiagnostic">The <see cref="ExpectedDiagnostic"/> with information about the expected <see cref="Diagnostic"/>.</param>
        /// <param name="before">The code with error positions indicated.</param>
        /// <param name="after">The expected code produced by applying <typeparamref name="TCodeFix"/>.</param>
        /// <param name="fixTitle">The expected title of the fix. Must be provided if more than one code action is registered.</param>
        /// <param name="allowCompilationErrors">Specify if compilation errors are accepted in the fixed code. This can be for example syntax errors. Default value is <see cref="AllowCompilationErrors.No"/>.</param>
        [Obsolete("Use overload taking analyzer & fix as arguments.")]
        public static void FixAll<TAnalyzer, TCodeFix>(ExpectedDiagnostic expectedDiagnostic, string before, string after, string fixTitle = null, AllowCompilationErrors allowCompilationErrors = AllowCompilationErrors.No)
            where TAnalyzer : DiagnosticAnalyzer, new()
            where TCodeFix : CodeFixProvider, new()
        {
            var analyzer = new TAnalyzer();
            var fix = new TCodeFix();
            FixAll(
                analyzer,
                fix,
                DiagnosticsAndSources.Create(expectedDiagnostic, new[] { before }),
                new[] { after },
                fixTitle,
                allowCompilationErrors);
        }

        /// <summary>
        /// Verifies that
        /// 1. <paramref name="before"/> produces the expected diagnostics
        /// 2. The code fix fixes the code.
        /// </summary>
        /// <typeparam name="TAnalyzer">The type of <see cref="DiagnosticAnalyzer"/> to check <paramref name="before"/> with.</typeparam>
        /// <typeparam name="TCodeFix">The <see cref="CodeFixProvider"/> to apply on the <see cref="Diagnostic"/> reported.</typeparam>
        /// <param name="expectedDiagnostic">The <see cref="ExpectedDiagnostic"/> with information about the expected <see cref="Diagnostic"/>.</param>
        /// <param name="before">The code with error positions indicated.</param>
        /// <param name="after">The expected code produced by applying <typeparamref name="TCodeFix"/>.</param>
        /// <param name="fixTitle">The expected title of the fix. Must be provided if more than one code action is registered.</param>
        /// <param name="allowCompilationErrors">Specify if compilation errors are accepted in the fixed code. This can be for example syntax errors. Default value is <see cref="AllowCompilationErrors.No"/>.</param>
        [Obsolete("Use overload taking analyzer & fix as arguments.")]
        public static void FixAll<TAnalyzer, TCodeFix>(ExpectedDiagnostic expectedDiagnostic, IReadOnlyList<string> before, string after, string fixTitle = null, AllowCompilationErrors allowCompilationErrors = AllowCompilationErrors.No)
            where TAnalyzer : DiagnosticAnalyzer, new()
            where TCodeFix : CodeFixProvider, new()
        {
            FixAll(
                new TAnalyzer(),
                new TCodeFix(),
                DiagnosticsAndSources.Create(expectedDiagnostic, before),
                MergeFixedCode(before, after),
                fixTitle,
                allowCompilationErrors,
                SuppressedDiagnostics,
                MetadataReferences);
        }

        /// <summary>
        /// Verifies that
        /// 1. <paramref name="before"/> produces the expected diagnostics
        /// 2. The code fix fixes the code.
        /// </summary>
        /// <typeparam name="TAnalyzer">The type of <see cref="DiagnosticAnalyzer"/> to check <paramref name="before"/> with.</typeparam>
        /// <typeparam name="TCodeFix">The <see cref="CodeFixProvider"/> to apply on the <see cref="Diagnostic"/> reported.</typeparam>
        /// <param name="expectedDiagnostic">The <see cref="ExpectedDiagnostic"/> with information about the expected <see cref="Diagnostic"/>.</param>
        /// <param name="before">The code with error positions indicated.</param>
        /// <param name="after">The expected code produced by applying <typeparamref name="TCodeFix"/>.</param>
        /// <param name="fixTitle">The expected title of the fix. Must be provided if more than one code action is registered.</param>
        /// <param name="allowCompilationErrors">Specify if compilation errors are accepted in the fixed code. This can be for example syntax errors. Default value is <see cref="AllowCompilationErrors.No"/>.</param>
        [Obsolete("Use overload taking analyzer & fix as arguments.")]
        public static void FixAll<TAnalyzer, TCodeFix>(ExpectedDiagnostic expectedDiagnostic, IReadOnlyList<string> before, IReadOnlyList<string> after, string fixTitle = null, AllowCompilationErrors allowCompilationErrors = AllowCompilationErrors.No)
            where TAnalyzer : DiagnosticAnalyzer, new()
            where TCodeFix : CodeFixProvider, new()
        {
            FixAll(
                new TAnalyzer(),
                new TCodeFix(),
                DiagnosticsAndSources.Create(expectedDiagnostic, before),
                after,
                fixTitle,
                allowCompilationErrors,
                SuppressedDiagnostics,
                MetadataReferences);
        }

        /// <summary>
        /// Verifies that
        /// 1. <paramref name="before"/> produces the expected diagnostics
        /// 2. The code fix fixes the code.
        /// </summary>
        /// <typeparam name="TCodeFix">The <see cref="CodeFixProvider"/> to apply on the <see cref="Diagnostic"/> reported.</typeparam>
        /// <param name="expectedDiagnostic">The <see cref="ExpectedDiagnostic"/> with information about the expected <see cref="Diagnostic"/>.</param>
        /// <param name="before">The code to analyze.</param>
        /// <param name="after">The expected code produced by applying <typeparamref name="TCodeFix"/>.</param>
        /// <param name="fixTitle">The expected title of the fix. Must be provided if more than one code action is registered.</param>
         /// <param name="allowCompilationErrors">Specify if compilation errors are accepted in the fixed code. This can be for example syntax errors. Default value is <see cref="AllowCompilationErrors.No"/>.</param>
        [Obsolete("Use overload taking analyzer & fix as arguments.")]
        public static void FixAll<TCodeFix>(ExpectedDiagnostic expectedDiagnostic, string before, string after, string fixTitle = null, AllowCompilationErrors allowCompilationErrors = AllowCompilationErrors.No)
            where TCodeFix : CodeFixProvider, new()
        {
            var analyzer = new PlaceholderAnalyzer(expectedDiagnostic.Id);
            FixAll(
                analyzer,
                new TCodeFix(),
                DiagnosticsAndSources.Create(expectedDiagnostic, new[] { before }),
                new[] { after },
                fixTitle,
                allowCompilationErrors,
                SuppressedDiagnostics,
                MetadataReferences);
        }

        /// <summary>
        /// Verifies that
        /// 1. <paramref name="before"/> produces the expected diagnostics
        /// 2. The code fix fixes the code.
        /// </summary>
        /// <typeparam name="TCodeFix">The <see cref="CodeFixProvider"/> to apply on the <see cref="Diagnostic"/> reported.</typeparam>
        /// <param name="expectedDiagnostic">The <see cref="ExpectedDiagnostic"/> with information about the expected <see cref="Diagnostic"/>.</param>
        /// <param name="before">The code to analyze.</param>
        /// <param name="after">The expected code produced by applying <typeparamref name="TCodeFix"/>.</param>
        /// <param name="fixTitle">The expected title of the fix. Must be provided if more than one code action is registered.</param>
         /// <param name="allowCompilationErrors">Specify if compilation errors are accepted in the fixed code. This can be for example syntax errors. Default value is <see cref="AllowCompilationErrors.No"/>.</param>
        [Obsolete("Use overload taking analyzer & fix as arguments.")]
        public static void FixAll<TCodeFix>(ExpectedDiagnostic expectedDiagnostic, IReadOnlyList<string> before, string after, string fixTitle = null, AllowCompilationErrors allowCompilationErrors = AllowCompilationErrors.No)
            where TCodeFix : CodeFixProvider, new()
        {
            var analyzer = new PlaceholderAnalyzer(expectedDiagnostic.Id);
            FixAll(
                analyzer,
                new TCodeFix(),
                DiagnosticsAndSources.Create(expectedDiagnostic, before),
                MergeFixedCode(before, after),
                fixTitle,
                allowCompilationErrors,
                SuppressedDiagnostics,
                MetadataReferences);
        }

        /// <summary>
        /// Verifies that
        /// 1. <paramref name="before"/> produces the expected diagnostics
        /// 2. The code fix fixes the code.
        /// </summary>
        /// <typeparam name="TCodeFix">The <see cref="CodeFixProvider"/> to apply on the <see cref="Diagnostic"/> reported.</typeparam>
        /// <param name="expectedDiagnostic">The <see cref="ExpectedDiagnostic"/> with information about the expected <see cref="Diagnostic"/>.</param>
        /// <param name="before">The code to analyze.</param>
        /// <param name="after">The expected code produced by applying <typeparamref name="TCodeFix"/>.</param>
        /// <param name="fixTitle">The expected title of the fix. Must be provided if more than one code action is registered.</param>
         /// <param name="allowCompilationErrors">Specify if compilation errors are accepted in the fixed code. This can be for example syntax errors. Default value is <see cref="AllowCompilationErrors.No"/>.</param>
        [Obsolete("Use overload taking analyzer & fix as arguments.")]
        public static void FixAll<TCodeFix>(ExpectedDiagnostic expectedDiagnostic, IReadOnlyList<string> before, IReadOnlyList<string> after, string fixTitle = null, AllowCompilationErrors allowCompilationErrors = AllowCompilationErrors.No)
            where TCodeFix : CodeFixProvider, new()
        {
            var analyzer = new PlaceholderAnalyzer(expectedDiagnostic.Id);
            FixAll(
                analyzer,
                new TCodeFix(),
                DiagnosticsAndSources.Create(expectedDiagnostic, before),
                after,
                fixTitle,
                allowCompilationErrors,
                SuppressedDiagnostics,
                MetadataReferences);
        }

        /// <summary>
        /// Verifies that
        /// 1. <paramref name="codeWithErrorsIndicated"/> produces the expected diagnostics
        /// 2. The code fix fixes the code.
        /// </summary>
        /// <typeparam name="TAnalyzer">The type of <see cref="DiagnosticAnalyzer"/> to check <paramref name="codeWithErrorsIndicated"/> with.</typeparam>
        /// <typeparam name="TCodeFix">The <see cref="CodeFixProvider"/> to apply on the <see cref="Diagnostic"/> reported.</typeparam>
        /// <param name="codeWithErrorsIndicated">The code with error positions indicated.</param>
        /// <param name="after">The expected code produced by applying <typeparamref name="TCodeFix"/>.</param>
        /// <param name="fixTitle">The expected title of the fix. Must be provided if more than one code action is registered.</param>
        /// <param name="allowCompilationErrors">Specify if compilation errors are accepted in the fixed code. This can be for example syntax errors. Default value is <see cref="AllowCompilationErrors.No"/>.</param>
        [Obsolete("Use overload taking analyzer & fix as arguments.")]
        public static void FixAll<TAnalyzer, TCodeFix>(string codeWithErrorsIndicated, string after, string fixTitle = null, AllowCompilationErrors allowCompilationErrors = AllowCompilationErrors.No)
            where TAnalyzer : DiagnosticAnalyzer, new()
            where TCodeFix : CodeFixProvider, new()
        {
            var analyzer = new TAnalyzer();
            FixAll(
                analyzer,
                new TCodeFix(),
                DiagnosticsAndSources.CreateFromCodeWithErrorsIndicated(analyzer, new[] { codeWithErrorsIndicated }),
                new[] { after },
                fixTitle,
                allowCompilationErrors,
                SuppressedDiagnostics,
                MetadataReferences);
        }

        /// <summary>
        /// Verifies that
        /// 1. <paramref name="before"/> produces the expected diagnostics
        /// 2. The code fix fixes the code.
        /// </summary>
        /// <typeparam name="TAnalyzer">The type of <see cref="DiagnosticAnalyzer"/> to check <paramref name="before"/> with.</typeparam>
        /// <typeparam name="TCodeFix">The <see cref="CodeFixProvider"/> to apply on the <see cref="Diagnostic"/> reported.</typeparam>
        /// <param name="before">The code with error positions indicated.</param>
        /// <param name="after">The expected code produced by applying <typeparamref name="TCodeFix"/>.</param>
        /// <param name="fixTitle">The expected title of the fix. Must be provided if more than one code action is registered.</param>
        /// <param name="allowCompilationErrors">Specify if compilation errors are accepted in the fixed code. This can be for example syntax errors. Default value is <see cref="AllowCompilationErrors.No"/>.</param>
        [Obsolete("Use overload taking analyzer & fix as arguments.")]
        public static void FixAll<TAnalyzer, TCodeFix>(IReadOnlyList<string> before, string after, string fixTitle = null, AllowCompilationErrors allowCompilationErrors = AllowCompilationErrors.No)
            where TAnalyzer : DiagnosticAnalyzer, new()
            where TCodeFix : CodeFixProvider, new()
        {
            var analyzer = new TAnalyzer();
            FixAll(
                analyzer,
                new TCodeFix(),
                DiagnosticsAndSources.CreateFromCodeWithErrorsIndicated(analyzer, before),
                MergeFixedCode(before, after),
                fixTitle,
                allowCompilationErrors,
                SuppressedDiagnostics,
                MetadataReferences);
        }

        /// <summary>
        /// Verifies that
        /// 1. <paramref name="before"/> produces the expected diagnostics
        /// 2. The code fix fixes the code.
        /// </summary>
        /// <typeparam name="TAnalyzer">The type of <see cref="DiagnosticAnalyzer"/> to check <paramref name="before"/> with.</typeparam>
        /// <typeparam name="TCodeFix">The <see cref="CodeFixProvider"/> to apply on the <see cref="Diagnostic"/> reported.</typeparam>
        /// <param name="before">The code with error positions indicated.</param>
        /// <param name="after">The expected code produced by applying <typeparamref name="TCodeFix"/>.</param>
        /// <param name="fixTitle">The expected title of the fix. Must be provided if more than one code action is registered.</param>
        /// <param name="allowCompilationErrors">Specify if compilation errors are accepted in the fixed code. This can be for example syntax errors. Default value is <see cref="AllowCompilationErrors.No"/>.</param>
        [Obsolete("Use overload taking analyzer & fix as arguments.")]
        public static void FixAll<TAnalyzer, TCodeFix>(IReadOnlyList<string> before, IReadOnlyList<string> after, string fixTitle = null, AllowCompilationErrors allowCompilationErrors = AllowCompilationErrors.No)
            where TAnalyzer : DiagnosticAnalyzer, new()
            where TCodeFix : CodeFixProvider, new()
        {
            var analyzer = new TAnalyzer();
            FixAll(
                analyzer,
                new TCodeFix(),
                DiagnosticsAndSources.CreateFromCodeWithErrorsIndicated(analyzer, before),
                after,
                fixTitle,
                allowCompilationErrors,
                SuppressedDiagnostics,
                MetadataReferences);
        }

        /// <summary>
        /// Verifies that
        /// 1. <paramref name="before"/> produces the expected diagnostics
        /// 2. The code fix fixes the code.
        /// </summary>
        /// <typeparam name="TAnalyzer">The type of <see cref="DiagnosticAnalyzer"/> to check <paramref name="before"/> with.</typeparam>
        /// <typeparam name="TCodeFix">The <see cref="CodeFixProvider"/> to apply on the <see cref="Diagnostic"/> reported.</typeparam>
        /// <param name="before">The code with error positions indicated.</param>
        /// <param name="after">The expected code produced by applying <typeparamref name="TCodeFix"/>.</param>
        /// <param name="fixTitle">The expected title of the fix. Must be provided if more than one code action is registered.</param>
        /// <param name="allowCompilationErrors">Specify if compilation errors are accepted in the fixed code. This can be for example syntax errors. Default value is <see cref="AllowCompilationErrors.No"/>.</param>
        [Obsolete("Use overload taking analyzer & fix as arguments.")]
        public static void FixAllInDocument<TAnalyzer, TCodeFix>(string before, string after, string fixTitle = null, AllowCompilationErrors allowCompilationErrors = AllowCompilationErrors.No)
            where TAnalyzer : DiagnosticAnalyzer, new()
            where TCodeFix : CodeFixProvider, new()
        {
            var analyzer = new TAnalyzer();
            FixAllByScope(
                analyzer,
                new TCodeFix(),
                new[] { before },
                new[] { after },
                FixAllScope.Document,
                fixTitle,
                allowCompilationErrors);
        }

        /// <summary>
        /// Verifies that
        /// 1. <paramref name="before"/> produces the expected diagnostics
        /// 2. The code fix fixes the code.
        /// </summary>
        /// <typeparam name="TAnalyzer">The type of <see cref="DiagnosticAnalyzer"/> to check <paramref name="before"/> with.</typeparam>
        /// <typeparam name="TCodeFix">The <see cref="CodeFixProvider"/> to apply on the <see cref="Diagnostic"/> reported.</typeparam>
        /// <param name="before">The code with error positions indicated.</param>
        /// <param name="after">The expected code produced by applying <typeparamref name="TCodeFix"/>.</param>
        /// <param name="fixTitle">The expected title of the fix. Must be provided if more than one code action is registered.</param>
        /// <param name="allowCompilationErrors">Specify if compilation errors are accepted in the fixed code. This can be for example syntax errors. Default value is <see cref="AllowCompilationErrors.No"/>.</param>
        [Obsolete("Use overload taking analyzer & fix as arguments.")]
        public static void FixAllOneByOne<TAnalyzer, TCodeFix>(string before, string after, string fixTitle = null, AllowCompilationErrors allowCompilationErrors = AllowCompilationErrors.No)
            where TAnalyzer : DiagnosticAnalyzer, new()
            where TCodeFix : CodeFixProvider, new()
        {
            var analyzer = new TAnalyzer();
            var diagnosticsAndSources = DiagnosticsAndSources.CreateFromCodeWithErrorsIndicated(analyzer, before);
            VerifyAnalyzerSupportsDiagnostics(analyzer, diagnosticsAndSources.ExpectedDiagnostics);
            var fix = new TCodeFix();
            VerifyCodeFixSupportsAnalyzer(analyzer, fix);
            var sln = CodeFactory.CreateSolution(diagnosticsAndSources, analyzer, null, SuppressedDiagnostics, MetadataReferences);
            var diagnostics = Analyze.GetDiagnostics(analyzer, sln);
            VerifyDiagnostics(diagnosticsAndSources, diagnostics, sln);
            FixAllOneByOne(analyzer, fix, sln, new[] { after }, fixTitle, allowCompilationErrors);
        }

        /// <summary>
        /// Verifies that <paramref name="code"/> produces no diagnostics when analyzed with <typeparamref name="TAnalyzer"/>.
        /// </summary>
        /// <typeparam name="TAnalyzer">The type of <see cref="DiagnosticAnalyzer"/> to check <paramref name="code"/> with.</typeparam>
        /// <param name="code">The code to analyze.</param>
        [Obsolete("Use overload taking analyzer & fix as arguments.")]
        public static void NoAnalyzerDiagnostics<TAnalyzer>(params string[] code)
            where TAnalyzer : DiagnosticAnalyzer, new()
        {
            NoAnalyzerDiagnostics(new TAnalyzer(), code);
        }

        /// <summary>
        /// Verifies that <paramref name="code"/> produces no diagnostics when analyzed with <typeparamref name="TAnalyzer"/>.
        /// </summary>
        /// <typeparam name="TAnalyzer">The type of <see cref="DiagnosticAnalyzer"/> to check <paramref name="code"/> with.</typeparam>
        /// <param name="descriptor">The <see cref="ExpectedDiagnostic"/> with information about the expected <see cref="Diagnostic"/>.</param>
        /// <param name="code">The code to analyze.</param>
        [Obsolete("Use overload taking analyzer & fix as arguments.")]
        public static void NoAnalyzerDiagnostics<TAnalyzer>(DiagnosticDescriptor descriptor, params string[] code)
            where TAnalyzer : DiagnosticAnalyzer, new()
        {
            NoAnalyzerDiagnostics(new TAnalyzer(), descriptor, code);
        }

        /// <summary>
        /// Verifies that <paramref name="code"/> produces no diagnostics when analyzed with <typeparamref name="TAnalyzer"/>.
        /// </summary>
        /// <typeparam name="TAnalyzer">The type of <see cref="DiagnosticAnalyzer"/> to check <paramref name="code"/> with.</typeparam>
        /// <param name="descriptors">The <see cref="ExpectedDiagnostic"/> with information about the expected <see cref="Diagnostic"/>.</param>
        /// <param name="code">The code to analyze.</param>
        [Obsolete("Use overload taking analyzer & fix as arguments.")]
        public static void NoAnalyzerDiagnostics<TAnalyzer>(IReadOnlyList<DiagnosticDescriptor> descriptors, params string[] code)
            where TAnalyzer : DiagnosticAnalyzer, new()
        {
            NoAnalyzerDiagnostics(new TAnalyzer(), descriptors, code);
        }

        /// <summary>
        /// Verifies that <paramref name="code"/> produces no diagnostics when analyzed with <typeparamref name="TAnalyzer"/>.
        /// </summary>
        /// <typeparam name="TAnalyzer">The type of <see cref="DiagnosticAnalyzer"/> to check <paramref name="code"/> with.</typeparam>
        /// <param name="descriptor">The <see cref="ExpectedDiagnostic"/> with information about the expected <see cref="Diagnostic"/>.</param>
        /// <param name="code">The code to analyze.</param>
        [Obsolete("Use overload taking analyzer & fix as arguments.")]
        public static void NoAnalyzerDiagnostics<TAnalyzer>(DiagnosticDescriptor descriptor, FileInfo code)
            where TAnalyzer : DiagnosticAnalyzer, new()
        {
            NoAnalyzerDiagnostics(new TAnalyzer(), descriptor, code);
        }

        /// <summary>
        /// Verifies that <paramref name="code"/> produces no diagnostics when analyzed with <typeparamref name="TAnalyzer"/>.
        /// </summary>
        /// <typeparam name="TAnalyzer">The type of <see cref="DiagnosticAnalyzer"/> to check <paramref name="code"/> with.</typeparam>
        /// <param name="code">The code to analyze.</param>
        [Obsolete("Use overload taking analyzer & fix as arguments.")]
        public static void NoAnalyzerDiagnostics<TAnalyzer>(FileInfo code)
            where TAnalyzer : DiagnosticAnalyzer, new()
        {
            NoAnalyzerDiagnostics(new TAnalyzer(), code);
        }

        /// <summary>
        /// Verifies that <paramref name="solution"/> produces no diagnostics when analyzed with <typeparamref name="TAnalyzer"/>.
        /// </summary>
        /// <typeparam name="TAnalyzer">The type of <see cref="DiagnosticAnalyzer"/> to check <paramref name="solution"/> with.</typeparam>
        /// <param name="solution">The <see cref="Solution"/> for which no errors or warnings are expected.</param>
        [Obsolete("Use overload taking analyzer & fix as arguments.")]
        public static void NoAnalyzerDiagnostics<TAnalyzer>(Solution solution)
            where TAnalyzer : DiagnosticAnalyzer, new()
        {
            NoAnalyzerDiagnostics(new TAnalyzer(), solution);
        }

        /// <summary>
        /// Verifies that
        /// 1. <paramref name="codeWithErrorsIndicated"/> produces the expected diagnostics
        /// 2. The code fix does not change the code.
        /// </summary>
        /// <typeparam name="TAnalyzer">The type of <see cref="DiagnosticAnalyzer"/> to check <paramref name="codeWithErrorsIndicated"/> with.</typeparam>
        /// <typeparam name="TCodeFix">The <see cref="CodeFixProvider"/> to apply on the <see cref="Diagnostic"/> reported.</typeparam>
        /// <param name="codeWithErrorsIndicated">The code with error positions indicated.</param>
        [Obsolete("Use overload taking analyzer & fix as arguments.")]
        public static void NoFix<TAnalyzer, TCodeFix>(params string[] codeWithErrorsIndicated)
            where TAnalyzer : DiagnosticAnalyzer, new()
            where TCodeFix : CodeFixProvider, new()
        {
            var analyzer = new TAnalyzer();
            NoFix(
                analyzer,
                new TCodeFix(),
                DiagnosticsAndSources.CreateFromCodeWithErrorsIndicated(analyzer, codeWithErrorsIndicated));
        }

        /// <summary>
        /// Verifies that
        /// 1. <paramref name="code"/> produces the expected diagnostics
        /// 2. The code fix does not change the code.
        /// </summary>
        /// <typeparam name="TCodeFix">The <see cref="CodeFixProvider"/> to apply on the <see cref="Diagnostic"/> reported.</typeparam>
        /// <param name="expectedDiagnostic">The <see cref="ExpectedDiagnostic"/> with information about the expected <see cref="Diagnostic"/>.</param>
        /// <param name="code">The code to analyze.</param>
        [Obsolete("Use overload taking analyzer & fix as arguments.")]
        public static void NoFix<TCodeFix>(ExpectedDiagnostic expectedDiagnostic, params string[] code)
            where TCodeFix : CodeFixProvider, new()
        {
            NoFix(
                new PlaceholderAnalyzer(expectedDiagnostic.Id),
                new TCodeFix(),
                DiagnosticsAndSources.Create(expectedDiagnostic, code));
        }

        /// <summary>
        /// Verifies that
        /// 1. <paramref name="codeWithErrorsIndicated"/> produces the expected diagnostics
        /// 2. The code fix does not change the code.
        /// </summary>
        /// <typeparam name="TAnalyzer">The type of <see cref="DiagnosticAnalyzer"/> to check <paramref name="codeWithErrorsIndicated"/> with.</typeparam>
        /// <typeparam name="TCodeFix">The <see cref="CodeFixProvider"/> to apply on the <see cref="Diagnostic"/> reported.</typeparam>
        /// <param name="codeWithErrorsIndicated">The code to analyze.</param>
        [Obsolete("Use overload taking analyzer & fix as arguments.")]
        public static void NoFix<TAnalyzer, TCodeFix>(IReadOnlyList<string> codeWithErrorsIndicated)
            where TAnalyzer : DiagnosticAnalyzer, new()
            where TCodeFix : CodeFixProvider, new()
        {
            NoFix(
                new TAnalyzer(),
                new TCodeFix(),
                DiagnosticsAndSources.CreateFromCodeWithErrorsIndicated(new TAnalyzer(), codeWithErrorsIndicated));
        }

        /// <summary>
        /// Verifies that
        /// 1. <paramref name="code"/> produces the expected diagnostics
        /// 2. The code fix does not change the code.
        /// </summary>
        /// <typeparam name="TAnalyzer">The type of <see cref="DiagnosticAnalyzer"/> to check <paramref name="code"/> with.</typeparam>
        /// <typeparam name="TCodeFix">The <see cref="CodeFixProvider"/> to apply on the <see cref="Diagnostic"/> reported.</typeparam>
        /// <param name="expectedDiagnostic">The <see cref="ExpectedDiagnostic"/> with information about the expected <see cref="Diagnostic"/>.</param>
        /// <param name="code">The code to analyze.</param>
        [Obsolete("Use overload taking analyzer & fix as arguments.")]
        public static void NoFix<TAnalyzer, TCodeFix>(ExpectedDiagnostic expectedDiagnostic, params string[] code)
            where TAnalyzer : DiagnosticAnalyzer, new()
            where TCodeFix : CodeFixProvider, new()
        {
            NoFix(
                new TAnalyzer(),
                new TCodeFix(),
                DiagnosticsAndSources.Create(expectedDiagnostic, code));
        }

        /// <summary>
        /// Verifies that <paramref name="codeWithErrorsIndicated"/> produces the expected diagnostics.
        /// </summary>
        /// <param name="analyzerType">The type of <see cref="DiagnosticAnalyzer"/> to check <paramref name="codeWithErrorsIndicated"/> with.</param>
        /// <param name="codeWithErrorsIndicated">The code with error positions indicated.</param>
        [Obsolete("Will be removed unless someone objects. Not sure when this would ever be useful.")]
        public static void Diagnostics(Type analyzerType, params string[] codeWithErrorsIndicated)
        {
            Diagnostics(
                (DiagnosticAnalyzer)Activator.CreateInstance(analyzerType),
                DiagnosticsAndSources.CreateFromCodeWithErrorsIndicated((DiagnosticAnalyzer)Activator.CreateInstance(analyzerType), codeWithErrorsIndicated));
        }

        /// <summary>
        /// Verifies that <paramref name="code"/> produces the expected diagnostics.
        /// </summary>
        /// <param name="analyzerType">The type of <see cref="DiagnosticAnalyzer"/> to check <paramref name="code"/> with.</param>
        /// <param name="expectedDiagnostic">The <see cref="ExpectedDiagnostic"/> with information about the expected <see cref="Diagnostic"/>.</param>
        /// <param name="code">The code to analyze.</param>
        [Obsolete("Will be removed unless someone objects. Not sure when this would ever be useful.")]
        public static void Diagnostics(Type analyzerType, ExpectedDiagnostic expectedDiagnostic, params string[] code)
        {
            Diagnostics(
                (DiagnosticAnalyzer)Activator.CreateInstance(analyzerType),
                DiagnosticsAndSources.Create(expectedDiagnostic, code));
        }

        /// <summary>
        /// Verifies that <paramref name="code"/> produces the expected diagnostics.
        /// </summary>
        /// <param name="analyzerType">The type of <see cref="DiagnosticAnalyzer"/> to check <paramref name="code"/> with.</param>
        /// <param name="expectedDiagnostics">The expected diagnostics.</param>
        /// <param name="code">The code to analyze.</param>
        [Obsolete("Will be removed unless someone objects. Not sure when this would ever be useful.")]
        public static void Diagnostics(Type analyzerType, IReadOnlyList<ExpectedDiagnostic> expectedDiagnostics, params string[] code)
        {
            Diagnostics(
                (DiagnosticAnalyzer)Activator.CreateInstance(analyzerType),
                new DiagnosticsAndSources(expectedDiagnostics, code));
        }

        /// <summary>
        /// Verifies that <paramref name="code"/> produces no diagnostics when analyzed with <typeparamref name="TAnalyzer"/>.
        /// </summary>
        /// <typeparam name="TAnalyzer">The type of <see cref="DiagnosticAnalyzer"/> to check <paramref name="code"/> with.</typeparam>
        /// <param name="code">The code to analyze.</param>
        [Obsolete("Use overload taking analyzer as argument.")]
        public static void Valid<TAnalyzer>(params string[] code)
            where TAnalyzer : DiagnosticAnalyzer, new()
        {
            Valid(new TAnalyzer(), code);
        }

        /// <summary>
        /// Verifies that <paramref name="code"/> produces no diagnostics when analyzed with <typeparamref name="TAnalyzer"/>.
        /// </summary>
        /// <typeparam name="TAnalyzer">The type of <see cref="DiagnosticAnalyzer"/> to check <paramref name="code"/> with.</typeparam>
        /// <param name="code">The code to analyze.</param>
        [Obsolete("Use overload taking analyzer as argument.")]
        public static void Valid<TAnalyzer>(FileInfo code)
            where TAnalyzer : DiagnosticAnalyzer, new()
        {
            Valid(new TAnalyzer(), code);
        }

        /// <summary>
        /// Verifies that <paramref name="code"/> produces no diagnostics when analyzed with <typeparamref name="TAnalyzer"/>.
        /// </summary>
        /// <typeparam name="TAnalyzer">The type of <see cref="DiagnosticAnalyzer"/> to check <paramref name="code"/> with.</typeparam>
        /// <param name="descriptor">The <see cref="ExpectedDiagnostic"/> with information about the expected <see cref="Diagnostic"/>.</param>
        /// <param name="code">The code to analyze.</param>
        [Obsolete("Use overload taking analyzer as argument.")]
        public static void Valid<TAnalyzer>(DiagnosticDescriptor descriptor, params string[] code)
            where TAnalyzer : DiagnosticAnalyzer, new()
        {
            Valid(new TAnalyzer(), descriptor, code);
        }

        /// <summary>
        /// Verifies that <paramref name="code"/> produces no diagnostics when analyzed with <typeparamref name="TAnalyzer"/>.
        /// </summary>
        /// <typeparam name="TAnalyzer">The type of <see cref="DiagnosticAnalyzer"/> to check <paramref name="code"/> with.</typeparam>
        /// <param name="descriptor">The <see cref="ExpectedDiagnostic"/> with information about the expected <see cref="Diagnostic"/>.</param>
        /// <param name="code">The code to analyze.</param>
        [Obsolete("Use overload taking analyzer as argument.")]
        public static void Valid<TAnalyzer>(DiagnosticDescriptor descriptor, FileInfo code)
            where TAnalyzer : DiagnosticAnalyzer, new()
        {
            Valid(new TAnalyzer(), descriptor, code);
        }

        /// <summary>
        /// Verifies that <paramref name="solution"/> produces no diagnostics when analyzed with <typeparamref name="TAnalyzer"/>.
        /// </summary>
        /// <typeparam name="TAnalyzer">The type of <see cref="DiagnosticAnalyzer"/> to check <paramref name="solution"/> with.</typeparam>
        /// <param name="solution">The <see cref="Solution"/> for which no errors or warnings are expected.</param>
        [Obsolete("Use overload taking analyzer as argument.")]
        public static void Valid<TAnalyzer>(Solution solution)
            where TAnalyzer : DiagnosticAnalyzer, new()
        {
            Valid(new TAnalyzer(), solution);
        }
    }
}
