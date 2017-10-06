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
        public static readonly List<MetadataReference> MetadataReferences = Asserts.MetadataReferences.FromAttributes().ToList();

        /// <summary>
        /// The metadata references used when creating the projects created in the tests.
        /// </summary>
        public static readonly List<string> SuppressedDiagnostics = DiagnosticSettings.AllowedErrorIds().ToList();

        /// <summary>
        /// Resets <see cref="MetadataReferences"/> to <see cref="Asserts.MetadataReferences.FromAttributes"/>
        /// </summary>
        public static void ResetMetadataReferences()
        {
            MetadataReferences.Clear();
            MetadataReferences.AddRange(Asserts.MetadataReferences.FromAttributes());
        }

        /// <summary>
        /// Resets <see cref="MetadataReferences"/> to <see cref="Asserts.MetadataReferences.FromAttributes"/>
        /// </summary>
        public static void ResetMetadataSuppressedDiagnostics()
        {
            SuppressedDiagnostics.Clear();
            SuppressedDiagnostics.AddRange(DiagnosticSettings.AllowedErrorIds());
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
                .Where(IsIncluded)
                .ToArray();
            if (introducedDiagnostics.Select(x => x.Id)
                                     .Except(DiagnosticSettings.AllowedErrorIds())
                                     .Any())
            {
                var error = StringBuilderPool.Borrow();
                error.AppendLine($"{codeFix} introduced syntax error{(introducedDiagnostics.Length > 1 ? "s" : string.Empty)}.");
                foreach (var introducedDiagnostic in introducedDiagnostics)
                {
                    var errorInfo = await introducedDiagnostic.ToStringAsync(fixedSolution).ConfigureAwait(false);
                    error.AppendLine($"{errorInfo}");
                }

                error.AppendLine("First source file with error is:");
                var first = introducedDiagnostics.First();
                var sources = await Task.WhenAll(fixedSolution.Projects.SelectMany(p => p.Documents).Select(d => CodeReader.GetStringFromDocumentAsync(d, CancellationToken.None)));
                var idAndPosition = IdAndPosition.Create(first);
                var match = sources.SingleOrDefault(x => CodeReader.FileName(x) == idAndPosition.Span.Path);
                error.Append(match);
                error.AppendLine();
                throw AssertException.Create(StringBuilderPool.ReturnAndGetText(error));
            }
        }

        private static bool IsIncluded(Diagnostic diagnostic)
        {
            return IsIncluded(diagnostic, DiagnosticSettings.AllowedDiagnostics());
        }

        private static bool IsIncluded(Diagnostic diagnostic, AllowedDiagnostics allowedDiagnostics)
        {
            switch (allowedDiagnostics)
            {
                case AllowedDiagnostics.Warnings:
                    return diagnostic.Severity == DiagnosticSeverity.Error;
                case AllowedDiagnostics.None:
                    return diagnostic.Severity == DiagnosticSeverity.Error || diagnostic.Severity == DiagnosticSeverity.Warning;
                case AllowedDiagnostics.WarningsAndErrors:
                    return false;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}