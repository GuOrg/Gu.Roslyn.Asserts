namespace Gu.Roslyn.Asserts
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using System.Threading;
    using System.Threading.Tasks;
    using Gu.Roslyn.Asserts.Internals;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CodeFixes;
    using Microsoft.CodeAnalysis.Diagnostics;
    using Microsoft.CodeAnalysis.Formatting;

    /// <summary>
    /// The AnalyzerAssert class contains a collection of static methods used for assertions on the behavior of analyzers and code fixes.
    /// </summary>
    public static partial class AnalyzerAssert
    {
        /// <summary>
        /// The metadata references used when creating the projects created in the tests.
        /// </summary>
        public static readonly MetaDataReferencesCollection MetadataReferences = new MetaDataReferencesCollection(Asserts.MetadataReferences.FromAttributes().ToList());

        /// <summary>
        /// The metadata references used when creating the projects created in the tests.
        /// </summary>
        public static readonly List<string> SuppressedDiagnostics = DiagnosticSettings.AllowedErrorIds().ToList();

        /// <summary>
        /// Add <paramref name="assembly"/> and all assemblies referenced by it.
        /// </summary>
        /// <param name="assembly">The <see cref="Assembly"/></param>
        public static void AddTransitiveMetadataReferences(Assembly assembly)
        {
            MetadataReferences.AddRange(Asserts.MetadataReferences.Transitive(assembly));
        }

        /// <summary>
        /// Resets <see cref="MetadataReferences"/> to <see cref="Asserts.MetadataReferences.FromAttributes"/>
        /// </summary>
        public static void ResetMetadataReferences()
        {
            MetadataReferences.Clear();
            MetadataReferences.AddRange(Asserts.MetadataReferences.FromAttributes());
        }

        /// <summary>
        /// Resets <see cref="SuppressedDiagnostics"/> to <see cref="DiagnosticSettings.AllowedErrorIds()"/>
        /// </summary>
        public static void ResetSuppressedDiagnostics()
        {
            SuppressedDiagnostics.Clear();
            SuppressedDiagnostics.AddRange(DiagnosticSettings.AllowedErrorIds());
        }

        /// <summary>
        /// Resets <see cref="SuppressedDiagnostics"/> and <see cref="MetadataReferences"/>
        /// </summary>
        public static void ResetAll()
        {
            ResetMetadataReferences();
            ResetSuppressedDiagnostics();
        }

        /// <summary>
        /// Check that the <paramref name="analyzer"/> exports <paramref name="expectedDiagnostic"/>
        /// </summary>
        /// <param name="analyzer">The <see cref="DiagnosticAnalyzer"/></param>
        /// <param name="expectedDiagnostic">The <see cref="ExpectedDiagnostic"/></param>
        public static void VerifyAnalyzerSupportsDiagnostic(DiagnosticAnalyzer analyzer, ExpectedDiagnostic expectedDiagnostic)
        {
            VerifyAnalyzerSupportsDiagnostic(analyzer, expectedDiagnostic.Id);
        }

        /// <summary>
        /// Check that the <paramref name="analyzer"/> exports <paramref name="descriptor"/>
        /// </summary>
        /// <param name="analyzer">The <see cref="DiagnosticAnalyzer"/></param>
        /// <param name="descriptor">The <see cref="DiagnosticDescriptor"/></param>
        public static void VerifyAnalyzerSupportsDiagnostic(DiagnosticAnalyzer analyzer, DiagnosticDescriptor descriptor)
        {
            VerifyAnalyzerSupportsDiagnostic(analyzer, descriptor.Id);
        }

        /// <summary>
        /// Check that the <paramref name="analyzer"/> exports <paramref name="expectedDiagnostics"/>
        /// </summary>
        /// <param name="analyzer">The <see cref="DiagnosticAnalyzer"/></param>
        /// <param name="expectedDiagnostics">The <see cref="ExpectedDiagnostic"/></param>
        internal static void VerifyAnalyzerSupportsDiagnostics(DiagnosticAnalyzer analyzer, IReadOnlyList<ExpectedDiagnostic> expectedDiagnostics)
        {
            foreach (var expectedDiagnostic in expectedDiagnostics)
            {
                VerifyAnalyzerSupportsDiagnostic(analyzer, expectedDiagnostic.Id);
            }
        }

        /// <summary>
        /// Check that the <paramref name="analyzer"/> exports <paramref name="descriptors"/>
        /// </summary>
        /// <param name="analyzer">The <see cref="DiagnosticAnalyzer"/></param>
        /// <param name="descriptors">The <see cref="DiagnosticDescriptor"/></param>
        internal static void VerifyAnalyzerSupportsDiagnostics(DiagnosticAnalyzer analyzer, IReadOnlyList<DiagnosticDescriptor> descriptors)
        {
            foreach (var expectedDiagnostic in descriptors)
            {
                VerifyAnalyzerSupportsDiagnostic(analyzer, expectedDiagnostic.Id);
            }
        }

        /// <summary>
        /// Check that the analyzer supports exactly one diagnostic.
        /// </summary>
        /// <param name="analyzer">The <see cref="DiagnosticAnalyzer"/></param>
        /// <param name="descriptor">The descriptor of the supported diagnostic</param>
        internal static void VerifySingleSupportedDiagnostic(DiagnosticAnalyzer analyzer, out DiagnosticDescriptor descriptor)
        {
            if (analyzer.SupportedDiagnostics.Length == 0)
            {
                var message = $"{analyzer.GetType().FullName}.SupportedDiagnostics returns an empty array.";
                throw new AssertException(message);
            }

            if (analyzer.SupportedDiagnostics.Length > 1)
            {
                var message = "This can only be used for analyzers with one SupportedDiagnostics.\r\n" +
                              "Prefer overload with ExpectedDiagnostic.";
                throw new AssertException(message);
            }

            descriptor = analyzer.SupportedDiagnostics[0];
            if (descriptor == null)
            {
                var message = $"{analyzer.GetType().FullName}.SupportedDiagnostics[0] returns null.";
                throw new AssertException(message);
            }
        }

        internal static void VerifyAnalyzerSupportsDiagnostic(DiagnosticAnalyzer analyzer, string expectedId)
        {
            if (analyzer.SupportedDiagnostics.Length > 0 &&
                analyzer.SupportedDiagnostics.Length != analyzer.SupportedDiagnostics.Select(x => x.Id).Distinct().Count())
            {
                var message = $"Analyzer {analyzer} has more than one diagnostic with ID {analyzer.SupportedDiagnostics.ToLookup(x => x.Id).First(x => x.Count() > 1).Key}.";
                throw new AssertException(message);
            }

            var descriptors = analyzer.SupportedDiagnostics.Count(x => x.Id == expectedId);
            if (descriptors == 0)
            {
                var message = $"Analyzer {analyzer} does not produce a diagnostic with ID {expectedId}.{Environment.NewLine}" +
                              $"The analyzer produces the following diagnostics: {{{string.Join(", ", analyzer.SupportedDiagnostics.Select(d => d.Id))}}}{Environment.NewLine}" +
                              $"The expected diagnostic is: {expectedId}";
                throw new AssertException(message);
            }

            if (descriptors > 1)
            {
                var message = $"Analyzer {analyzer} supports multiple diagnostics with ID {expectedId}.{Environment.NewLine}" +
                              $"The analyzer produces the following diagnostics: {{{string.Join(", ", analyzer.SupportedDiagnostics.Select(d => d.Id))}}}{Environment.NewLine}" +
                              $"The expected diagnostic is: {expectedId}";
                throw new AssertException(message);
            }
        }

        private static void VerifyCodeFixSupportsAnalyzer(DiagnosticAnalyzer analyzer, CodeFixProvider codeFix)
        {
            if (!analyzer.SupportedDiagnostics.Select(d => d.Id).Intersect(codeFix.FixableDiagnosticIds).Any())
            {
                var message = $"Analyzer {analyzer} does not produce diagnostics fixable by {codeFix}.{Environment.NewLine}" +
                              $"The analyzer produces the following diagnostics: {{{string.Join(", ", analyzer.SupportedDiagnostics.Select(d => d.Id))}}}{Environment.NewLine}" +
                              $"The code fix supports the following diagnostics: {{{string.Join(", ", codeFix.FixableDiagnosticIds)}}}";
                throw new AssertException(message);
            }
        }

        private static async Task AreEqualAsync(IReadOnlyList<string> expected, Solution actual, string messageHeader)
        {
            var actualCount = actual.Projects.SelectMany(x => x.Documents).Count();
            if (expected.Count != actualCount)
            {
                throw new AssertException($"Expected {expected.Count} documents the fixed solution has {actualCount} documents.");
            }

            foreach (var project in actual.Projects)
            {
                foreach (var document in project.Documents)
                {
                    var fixedSource = await CodeReader.GetStringFromDocumentAsync(document, Formatter.Annotation, CancellationToken.None).ConfigureAwait(false);
                    CodeAssert.AreEqual(FindExpected(fixedSource), fixedSource, messageHeader);
                }
            }

            string FindExpected(string fixedSource)
            {
                var fixedNamespace = CodeReader.Namespace(fixedSource);
                var fixedFileName = CodeReader.FileName(fixedSource);
                foreach (var candidate in expected)
                {
                    if (CodeReader.Namespace(candidate) == fixedNamespace &&
                        CodeReader.FileName(candidate) == fixedFileName)
                    {
                        return candidate;
                    }
                }

                throw new AssertException($"The fixed solution contains a document {fixedFileName} in namespace {fixedNamespace} that is not in the expected documents.");
            }
        }

        private static async Task VerifyNoCompilerErrorsAsync(CodeFixProvider codeFix, Solution fixedSolution)
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
                var errorBuilder = StringBuilderPool.Borrow();
                errorBuilder.AppendLine($"{codeFix} introduced syntax error{(introducedDiagnostics.Length > 1 ? "s" : string.Empty)}.");
                foreach (var introducedDiagnostic in introducedDiagnostics)
                {
                    errorBuilder.AppendLine($"{introducedDiagnostic.ToErrorString()}");
                }

                errorBuilder.AppendLine("First source file with error is:");
                var sources = await Task.WhenAll(fixedSolution.Projects.SelectMany(p => p.Documents).Select(d => CodeReader.GetStringFromDocumentAsync(d, Formatter.Annotation, CancellationToken.None)));
                var lineSpan = introducedDiagnostics.First().Location.GetMappedLineSpan();
                var match = sources.SingleOrDefault(x => CodeReader.FileName(x) == lineSpan.Path);
                errorBuilder.Append(match);
                errorBuilder.AppendLine();
                throw new AssertException(errorBuilder.Return());
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
