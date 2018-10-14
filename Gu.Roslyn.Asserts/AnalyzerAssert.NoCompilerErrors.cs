namespace Gu.Roslyn.Asserts
{
    using System.Collections.Generic;
    using System.Collections.Immutable;
    using System.Globalization;
    using System.Linq;
    using Gu.Roslyn.Asserts.Internals;
    using Microsoft.CodeAnalysis;

    public static partial class AnalyzerAssert
    {
        /// <summary>
        /// Check the solution for compiler errors and warnings, uses:
        /// DiagnosticSettings.AllowedErrorIds()
        /// MetadataReferences.
        /// </summary>
        public static void NoCompilerErrors(params string[] code)
        {
            var solution = CodeFactory.CreateSolution(code, MetadataReferences);
            NoCompilerErrors(solution, DiagnosticSettings.AllowedErrorIds(), DiagnosticSettings.AllowedDiagnostics());
        }

        /// <summary>
        /// Check the solution for compiler errors and warnings, uses:
        /// DiagnosticSettings.AllowedErrorIds()
        /// DiagnosticSettings.AllowedDiagnostics().
        /// </summary>
        public static void NoCompilerErrors(IEnumerable<MetadataReference> metadataReferences, params string[] code)
        {
            var solution = CodeFactory.CreateSolution(code, metadataReferences);
            NoCompilerErrors(solution, DiagnosticSettings.AllowedErrorIds(), DiagnosticSettings.AllowedDiagnostics());
        }

        /// <summary>
        /// Check the solution for compiler errors and warnings, uses:
        /// DiagnosticSettings.AllowedErrorIds()
        /// DiagnosticSettings.AllowedDiagnostics().
        /// </summary>
        public static void NoCompilerErrors(Solution solution)
        {
            NoCompilerErrors(solution, DiagnosticSettings.AllowedErrorIds(), DiagnosticSettings.AllowedDiagnostics());
        }

        /// <summary>
        /// Check the solution for compiler errors and warnings.
        /// </summary>
        public static void NoCompilerErrors(Solution solution, IReadOnlyList<string> allowedIds, AllowedDiagnostics allowedDiagnostics)
        {
            var diagnostics = Analyze.GetDiagnostics(solution);
            NoCompilerErrors(diagnostics, allowedIds, allowedDiagnostics);
        }

        private static void NoCompilerErrors(IReadOnlyList<ImmutableArray<Diagnostic>> diagnostics, IReadOnlyList<string> allowedIds, AllowedDiagnostics allowedDiagnostics)
        {
            var introducedDiagnostics = diagnostics
                                        .SelectMany(x => x)
                                        .Where(x => IsIncluded(x, allowedDiagnostics))
                                        .Where(x => IsExcluded(x))
                                        .ToArray();
            if (introducedDiagnostics.Select(x => x.Id)
                                     .Except(allowedIds ?? Enumerable.Empty<string>())
                                     .Any())
            {
                var error = StringBuilderPool.Borrow();
                error.AppendLine($"Found error{(introducedDiagnostics.Length > 1 ? "s" : string.Empty)}.");
                foreach (var introducedDiagnostic in introducedDiagnostics)
                {
                    error.AppendLine($"{introducedDiagnostic.ToErrorString()}");
                }

                throw new AssertException(StringBuilderPool.Return(error));
            }

            bool IsExcluded(Diagnostic diagnostic)
            {
                switch (diagnostic.Id)
                {
                        case "CS1061" when diagnostic.GetMessage(CultureInfo.InvariantCulture).Contains("does not contain a definition for 'InitializeComponent' and no extension method 'InitializeComponent' accepting a first argument of type"):
                            return true;
                }

                return false;
            }
        }
    }
}
