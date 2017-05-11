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
        public static readonly List<MetadataReference> MetadataReference = new List<MetadataReference>();

        private static void AssertCodeFixCanFixDiagnosticsFromAnalyzer(DiagnosticAnalyzer analyzer, CodeFixProvider codeFix)
        {
            if (!analyzer.SupportedDiagnostics.Select(d => d.Id).Intersect(codeFix.FixableDiagnosticIds).Any())
            {
                var message = $"Analyzer {analyzer} does not produce diagnostics fixable by {codeFix}.{Environment.NewLine}" +
                              $"The analyzer produces the following diagnostics: {{{string.Join(", ", analyzer.SupportedDiagnostics.Select(d => d.Id))}}}{Environment.NewLine}" +
                              $"The code fix supports the following diagnostics: {{{string.Join(", ", codeFix.FixableDiagnosticIds)}}}";
                throw Fail.CreateException(message);
            }
        }

        private static async Task AreEqualAsync(IReadOnlyList<string> expected, Solution actual)
        {
            foreach (var project in actual.Projects)
            {
                for (var i = 0; i < project.DocumentIds.Count; i++)
                {
                    var fixedSource = await CodeReader.GetStringFromDocumentAsync(project.GetDocument(project.DocumentIds[i]), CancellationToken.None).ConfigureAwait(false);
                    CodeAssert.AreEqual(expected[i], fixedSource);
                }
            }
        }
    }
}