namespace Gu.Roslyn.Asserts
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Gu.Roslyn.Asserts.Internals;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CodeActions;
    using Microsoft.CodeAnalysis.CodeFixes;
    using Microsoft.CodeAnalysis.Diagnostics;

    public static partial class AnalyzerAssert
    {
        /// <summary>
        /// Verifies that
        /// 1. <paramref name="codeWithErrorsIndicated"/> produces the expected diagnostics
        /// 2. The code fix fixes the code.
        /// </summary>
        /// <typeparam name="TAnalyzer">The type of the analyzer.</typeparam>
        /// <typeparam name="TCodeFix">The type of the code fix.</typeparam>
        /// <param name="codeWithErrorsIndicated">The code with error positions indicated.</param>
        /// <param name="fixedCode">The expected code produced by the code fix.</param>
        public static void CodeFix<TAnalyzer, TCodeFix>(string codeWithErrorsIndicated, string fixedCode)
            where TAnalyzer : DiagnosticAnalyzer, new()
            where TCodeFix : CodeFixProvider, new()
        {
            CodeFix(new TAnalyzer(), new TCodeFix(), new[] { codeWithErrorsIndicated }, new[] { fixedCode });
        }

        /// <summary>
        /// Verifies that
        /// 1. <paramref name="codeWithErrorsIndicated"/> produces the expected diagnostics
        /// 2. The code fix fixes the code.
        /// </summary>
        /// <typeparam name="TCodeFix">The type of the code fix.</typeparam>
        /// <param name="id">The id of the expected diagnostic.</param>
        /// <param name="codeWithErrorsIndicated">The code with error positions indicated.</param>
        /// <param name="fixedCode">The expected code produced by the code fix.</param>
        public static void CodeFix<TCodeFix>(string id, string codeWithErrorsIndicated, string fixedCode)
            where TCodeFix : CodeFixProvider, new()
        {
            CodeFix(new PlaceholderAnalyzer(id), new TCodeFix(), new[] { codeWithErrorsIndicated }, new[] { fixedCode });
        }

        /// <summary>
        /// Verifies that
        /// 1. <paramref name="codeWithErrorsIndicated"/> produces the expected diagnostics
        /// 2. The code fix fixes the code.
        /// </summary>
        /// <typeparam name="TAnalyzer">The type of the analyzer.</typeparam>
        /// <typeparam name="TCodeFix">The type of the code fix.</typeparam>
        /// <param name="codeWithErrorsIndicated">The code with error positions indicated.</param>
        /// <param name="fixedCode">The expected code produced by the code fix.</param>
        public static void CodeFix<TAnalyzer, TCodeFix>(IEnumerable<string> codeWithErrorsIndicated, IEnumerable<string> fixedCode)
            where TAnalyzer : DiagnosticAnalyzer, new()
            where TCodeFix : CodeFixProvider, new()
        {
            CodeFix(new TAnalyzer(), new TCodeFix(), codeWithErrorsIndicated, fixedCode);
        }

        /// <summary>
        /// Verifies that
        /// 1. <paramref name="codeWithErrorsIndicated"/> produces the expected diagnostics
        /// 2. The code fix fixes the code.
        /// </summary>
        /// <typeparam name="TCodeFix">The type of the code fix.</typeparam>
        /// <param name="id">The id of the expected diagnostic.</param>
        /// <param name="codeWithErrorsIndicated">The code with error positions indicated.</param>
        /// <param name="fixedCode">The expected code produced by the code fix.</param>
        public static void CodeFix<TCodeFix>(string id, IEnumerable<string> codeWithErrorsIndicated, IEnumerable<string> fixedCode)
            where TCodeFix : CodeFixProvider, new()
        {
            CodeFix(new PlaceholderAnalyzer(id), new TCodeFix(), codeWithErrorsIndicated, fixedCode);
        }

        /// <summary>
        /// Verifies that
        /// 1. <paramref name="codeWithErrorsIndicated"/> produces the expected diagnostics
        /// 2. The code fix fixes the code.
        /// </summary>
        /// <param name="analyzer">The analyzer to run on the code..</param>
        /// <param name="codeFix">The code fix to apply.</param>
        /// <param name="codeWithErrorsIndicated">The code with error positions indicated.</param>
        /// <param name="fixedCode">The expected code produced by the code fix.</param>
        public static void CodeFix(DiagnosticAnalyzer analyzer, CodeFixProvider codeFix, IEnumerable<string> codeWithErrorsIndicated, IEnumerable<string> fixedCode)
        {
            try
            {
                CodeFixAsync(analyzer, codeFix, codeWithErrorsIndicated, fixedCode, MetadataReference).Wait();
            }
            catch (AggregateException e)
            {
                throw Fail.CreateException(e.InnerExceptions[0].Message);
            }
        }

        /// <summary>
        /// Verifies that
        /// 1. <paramref name="codeWithErrorsIndicated"/> produces the expected diagnostics
        /// 2. The code fix fixes the code.
        /// </summary>
        /// <param name="analyzer">The analyzer to run on the code..</param>
        /// <param name="codeFix">The code fix to apply.</param>
        /// <param name="codeWithErrorsIndicated">The code with error positions indicated.</param>
        /// <param name="fixedCode">The expected code produced by the code fix.</param>
        /// <param name="references">The meta data references to add to the compilation.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        public static async Task CodeFixAsync(DiagnosticAnalyzer analyzer, CodeFixProvider codeFix, IEnumerable<string> codeWithErrorsIndicated, IEnumerable<string> fixedCode, IEnumerable<MetadataReference> references)
        {
            if (analyzer.SupportedDiagnostics.Length != 1)
            {
                var message = $"The analyzer supports multiple diagnostics {{{string.Join(", ", analyzer.SupportedDiagnostics.Select(d => d.Id))}}}.{Environment.NewLine}" +
                              $"This method can only be used with analyzers that have exactly one SupportedDiagnostic";
                throw Fail.CreateException(message);
            }

            AssertCodeFixCanFixDiagnosticsFromAnalyzer(analyzer, codeFix);
            var data = await DiagnosticsWithMetaDataAsync(analyzer, codeWithErrorsIndicated, references).ConfigureAwait(false);

            if (CodeReader.AreEqual(data.Sources, fixedCode))
            {
                throw Fail.CreateException("Fixed code is identical to provided code. Did you mean to call NoFix?");
            }

            var fixableDiagnostics = data.ActualDiagnostics.SelectMany(x => x)
                                         .Where(x => codeFix.FixableDiagnosticIds.Contains(x.Id))
                                         .ToArray();
            if (fixableDiagnostics.Length == 0)
            {
                var message = $"Code analyzed with {analyzer} did not generate any diagnostics fixable by {codeFix}.{Environment.NewLine}" +
                              $"The analyzed code contained the following diagnostics: {{{string.Join(", ", data.ExpectedDiagnostics.Select(d => d.Analyzer.SupportedDiagnostics[0].Id))}}}{Environment.NewLine}" +
                              $"The code fix supports the following diagnostics: {{{string.Join(", ", codeFix.FixableDiagnosticIds)}}}";
                throw Fail.CreateException(message);
            }

            if (fixableDiagnostics.Length > 1)
            {
                var message = $"Code analyzed with {analyzer} generated more than one diagnostic fixable by {codeFix}.{Environment.NewLine}" +
                              $"The analyzed code contained the following diagnostics: {{{string.Join(", ", data.ExpectedDiagnostics.Select(d => d.Analyzer.SupportedDiagnostics[0].Id))}}}{Environment.NewLine}" +
                              $"The code fix supports the following diagnostics: {{{string.Join(", ", codeFix.FixableDiagnosticIds)}}}{Environment.NewLine}" +
                              $"Maybe you meant to call AnalyzerAssert.FixAll?";
                throw Fail.CreateException(message);
            }

            var diagnostic = fixableDiagnostics.Single();
            var actions = new List<CodeAction>();
            foreach (var project in data.Solution.Projects)
            {
                var document = data.Solution.GetDocument(diagnostic.Location.SourceTree);
                actions.Clear();
                var context = new CodeFixContext(
                    document,
                    diagnostic,
                    (a, d) => actions.Add(a),
                    CancellationToken.None);
                await codeFix.RegisterCodeFixesAsync(context).ConfigureAwait(false);
                if (actions.Count == 0)
                {
                    continue;
                }

                if (actions.Count > 1)
                {
                    throw Fail.CreateException("Expected only one action");
                }

                var fixedProject = await ApplyFixAsync(project, actions[0], CancellationToken.None).ConfigureAwait(false);
                if (ReferenceEquals(fixedProject, project))
                {
                    throw Fail.CreateException($"{codeFix} did not change the document.");
                }

                for (var i = 0; i < fixedProject.DocumentIds.Count; i++)
                {
                    var fixedSource = await GetStringFromDocumentAsync(fixedProject.GetDocument(project.DocumentIds[i]), CancellationToken.None).ConfigureAwait(false);
                    //// ReSharper disable once PossibleMultipleEnumeration
                    CodeAssert.AreEqual(fixedCode.ElementAt(i), fixedSource);
                }
            }

            var sln = CodeFactory.CreateSolution(fixedCode, references);
            var diagnostics = await Analyze.GetDiagnosticsAsync(sln);
            if (diagnostics.SelectMany(x => x).Any(x => x.Severity == DiagnosticSeverity.Error))
            {
                var message = $"{codeFix} introduced syntax error.\r\n" +
                              $"{string.Join(", ", diagnostics.SelectMany(x => x).Where(d => d.Severity == DiagnosticSeverity.Error))}";
                throw Fail.CreateException(message);
            }
        }
    }
}