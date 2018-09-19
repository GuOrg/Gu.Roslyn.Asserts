namespace Gu.Roslyn.Asserts
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Gu.Roslyn.Asserts.Internals;
    using Microsoft.CodeAnalysis;

    public static partial class AnalyzerAssert
    {
        /// <summary>
        /// Check the solution for compiler errors and warnings, uses:
        /// DiagnosticSettings.AllowedErrorIds()
        /// MetadataReferences
        /// </summary>
        public static void NoCompilerErrors(params string[] code)
        {
            var solution = CodeFactory.CreateSolution(code, MetadataReferences);
            NoCompilerErrors(solution, DiagnosticSettings.AllowedErrorIds(), DiagnosticSettings.AllowedDiagnostics());
        }

        /// <summary>
        /// Check the solution for compiler errors and warnings, uses:
        /// DiagnosticSettings.AllowedErrorIds()
        /// DiagnosticSettings.AllowedDiagnostics()
        /// </summary>
        public static void NoCompilerErrors(IEnumerable<MetadataReference> metadataReferences, params string[] code)
        {
            var solution = CodeFactory.CreateSolution(code, metadataReferences);
            NoCompilerErrors(solution, DiagnosticSettings.AllowedErrorIds(), DiagnosticSettings.AllowedDiagnostics());
        }

        /// <summary>
        /// Check the solution for compiler errors and warnings, uses:
        /// DiagnosticSettings.AllowedErrorIds()
        /// DiagnosticSettings.AllowedDiagnostics()
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
            var introducedDiagnostics = diagnostics
                                        .SelectMany(x => x)
                                        .Where(x => IsIncluded(x, allowedDiagnostics))
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

                throw AssertException.Create(StringBuilderPool.Return(error));
            }
        }

        /// <summary>
        /// Check the solution for compiler errors and warnings, uses:
        /// DiagnosticSettings.AllowedErrorIds()
        /// DiagnosticSettings.AllowedDiagnostics()
        /// </summary>
        [Obsolete("Use sync API.")]
        public static Task NoCompilerErrorsAsync(Solution solution)
        {
            return NoCompilerErrorsAsync(solution, DiagnosticSettings.AllowedErrorIds(), DiagnosticSettings.AllowedDiagnostics());
        }

        /// <summary>
        /// Check the solution for compiler errors and warnings, uses:
        /// </summary>
        [Obsolete("Use sync API.")]
        public static async Task NoCompilerErrorsAsync(Solution solution, IReadOnlyList<string> allowedIds, AllowedDiagnostics allowedDiagnostics)
        {
            var diagnostics = await Analyze.GetDiagnosticsAsync(solution).ConfigureAwait(false);
            var introducedDiagnostics = diagnostics
                .SelectMany(x => x)
                .Where(x => IsIncluded(x, allowedDiagnostics))
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

                throw AssertException.Create(StringBuilderPool.Return(error));
            }
        }
    }
}
