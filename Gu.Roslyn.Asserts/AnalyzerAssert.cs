namespace Gu.Roslyn.Asserts
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Gu.Roslyn.Asserts.Internals;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CodeFixes;
    using Microsoft.CodeAnalysis.Diagnostics;

    /// <summary>
    /// The AnalyzerAssert class contains a collection of static methods used for assertions on the behavior of analyzers and code fixes.
    /// </summary>
    public static partial class AnalyzerAssert
    {
        /// <summary>
        /// The metadata references used when creating the projects created in the tests.
        /// </summary>
        public static readonly List<MetadataReference> MetadataReferences = Asserts.MetadataReferences.GetMetadataReferences();

        /// <summary>
        /// Resets <see cref="MetadataReferences"/> to <see cref="Asserts.MetadataReferences.GetMetadataReferences()"/>
        /// </summary>
        public static void ResetMetadataReferences()
        {
            MetadataReferences.Clear();
            MetadataReferences.AddRange(Asserts.MetadataReferences.GetMetadataReferences());
        }

        private static void AssertCodeFixCanFixDiagnosticsFromAnalyzer(DiagnosticAnalyzer analyzer, CodeFixProvider codeFix)
        {
            if (!analyzer.SupportedDiagnostics.Select(d => d.Id).Intersect(codeFix.FixableDiagnosticIds).Any())
            {
                var message = $"Analyzer {analyzer} does not produce diagnostics fixable by {codeFix}.{Environment.NewLine}" +
                              $"The analyzer produces the following diagnostics: {{{string.Join(", ", analyzer.SupportedDiagnostics.Select(d => d.Id))}}}{Environment.NewLine}" +
                              $"The code fix supports the following diagnostics: {{{string.Join(", ", codeFix.FixableDiagnosticIds)}}}";
                throw AssertException.Create(message);
            }
        }

        private static async Task AreEqualAsync(IReadOnlyList<string> expected, Solution actual, string messageHeader)
        {
            foreach (var project in actual.Projects)
            {
                for (var i = 0; i < project.DocumentIds.Count; i++)
                {
                    var fixedSource = await CodeReader.GetStringFromDocumentAsync(project.GetDocument(project.DocumentIds[i]), CancellationToken.None).ConfigureAwait(false);
                    CodeAssert.AreEqual(expected[i], fixedSource, messageHeader);
                }
            }
        }

        private static async Task AssertNoCompilerErrorsAsync(CodeFixProvider codeFix, Solution fixedSolution)
        {
            var diagnostics = await Analyze.GetDiagnosticsAsync(fixedSolution).ConfigureAwait(false);
            var introducedDiagnostics = diagnostics
                .SelectMany(x => x)
                .Where(d => d.Severity == DiagnosticSeverity.Error)
                .ToArray();
            if (introducedDiagnostics.Select(x => x.Id)
                                     .Except(IgnoredErrors.Get())
                                     .Any())
            {
                var error = StringBuilderPool.Borrow();
                error.AppendLine($"{codeFix} introduced syntax error{(introducedDiagnostics.Length > 1 ? "s" : string.Empty)}.");
                foreach (var introducedDiagnostic in introducedDiagnostics)
                {
                    var errorInfo = await introducedDiagnostic.ToStringAsync(fixedSolution).ConfigureAwait(false);
                    error.AppendLine($"{errorInfo}");
                }

                throw AssertException.Create(StringBuilderPool.ReturnAndGetText(error));
            }
        }
    }
}