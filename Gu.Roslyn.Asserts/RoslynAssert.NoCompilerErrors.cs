namespace Gu.Roslyn.Asserts
{
    using System.Collections.Generic;
    using System.Collections.Immutable;
    using System.Globalization;
    using System.Linq;
    using Gu.Roslyn.Asserts.Internals;
    using Microsoft.CodeAnalysis;

    public static partial class RoslynAssert
    {
        /// <summary>
        /// Check the solution for compiler errors and warnings, uses:
        /// DiagnosticSettings.AllowedErrorIds()
        /// MetadataReferences.
        /// </summary>
        /// <param name="code">The code to analyze.</param>
        public static void NoCompilerErrors(params string[] code)
        {
#pragma warning disable CS0618 // Suppress until removed. Will be replaced with MetadataReferences.FromAttributes()
            var solution = CodeFactory.CreateSolution(code, Settings.Default);
            NoCompilerErrors(solution, SuppressedDiagnostics, DiagnosticSettings.AllowedDiagnostics());
#pragma warning restore CS0618 // Suppress until removed. Will be replaced with MetadataReferences.FromAttributes()
        }

        /// <summary>
        /// Check the solution for compiler errors and warnings, uses:
        /// DiagnosticSettings.AllowedErrorIds()
        /// DiagnosticSettings.AllowedDiagnostics().
        /// </summary>
        /// <param name="metadataReferences">A collection of <see cref="MetadataReference"/> to use when compiling. Default is <see langword="null" /> meaning <see cref="MetadataReferences"/> are used.</param>
        /// <param name="code">The code to analyze.</param>
        public static void NoCompilerErrors(IEnumerable<MetadataReference> metadataReferences, params string[] code)
        {
#pragma warning disable CS0618 // Suppress until removed. Will be replaced with MetadataReferences.FromAttributes()
            var solution = CodeFactory.CreateSolution(code, metadataReferences);
            NoCompilerErrors(solution, SuppressedDiagnostics, DiagnosticSettings.AllowedDiagnostics());
#pragma warning restore CS0618 // Suppress until removed. Will be replaced with MetadataReferences.FromAttributes()
        }

        /// <summary>
        /// Check the solution for compiler errors and warnings, uses:
        /// DiagnosticSettings.AllowedErrorIds()
        /// DiagnosticSettings.AllowedDiagnostics().
        /// </summary>
        public static void NoCompilerErrors(Solution solution)
        {
#pragma warning disable CS0618 // Suppress until removed. Will be replaced with MetadataReferences.FromAttributes()
            NoCompilerErrors(solution, SuppressedDiagnostics, DiagnosticSettings.AllowedDiagnostics());
#pragma warning restore CS0618 // Suppress until removed. Will be replaced with MetadataReferences.FromAttributes()
        }

        /// <summary>
        /// Check the solution for compiler errors and warnings.
        /// </summary>
        public static void NoCompilerErrors(Solution solution, IReadOnlyList<string> allowedIds, AllowedDiagnostics allowedDiagnostics)
        {
            var diagnostics = Analyze.GetDiagnostics(solution);
            NoCompilerErrors(diagnostics.SelectMany(x => x), allowedIds, allowedDiagnostics);
        }

        private static void NoCompilerErrors(IEnumerable<Diagnostic> diagnostics, IReadOnlyList<string> allowedIds, AllowedDiagnostics allowedDiagnostics)
        {
            var introducedDiagnostics = diagnostics
                                        .Where(x => IsIncluded(x, allowedDiagnostics))
                                        .Where(x => !IsExcluded(x))
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
                if (allowedIds.Contains(diagnostic.Id))
                {
                    return true;
                }

                return diagnostic.Id switch
                {
                    "CS1061" when diagnostic.GetMessage(CultureInfo.InvariantCulture).Contains("does not contain a definition for 'InitializeComponent' and no accessible extension method 'InitializeComponent' accepting a first argument of type") => true,
                    _ => false,
                };
            }
        }
    }
}
