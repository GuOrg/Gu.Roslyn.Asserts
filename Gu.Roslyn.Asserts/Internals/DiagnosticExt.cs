namespace Gu.Roslyn.Asserts.Internals
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.CodeAnalysis;

    /// <summary>
    /// Helper for working with <see cref="Diagnostic"/>
    /// </summary>
    internal static class DiagnosticExt
    {
        /// <summary>
        /// Writes the diagnostic and the offending code.
        /// </summary>
        /// <returns>A string for use in assert exception</returns>
        internal static string ToString(this Diagnostic diagnostic, IReadOnlyList<string> sources)
        {
            return IdAndPosition.Create(diagnostic).ToString(sources);
        }

        /// <summary>
        /// Writes the diagnostic and the offending code.
        /// </summary>
        /// <returns>A string for use in assert exception</returns>
        internal static async Task<string> ToStringAsync(this Diagnostic diagnostic, Solution solution)
        {
            var sources = await Task.WhenAll(solution.Projects.SelectMany(p => p.Documents)
                                                     .Select(d => CodeReader.GetStringFromDocumentAsync(d, CancellationToken.None)));
            return diagnostic.ToString(sources);
        }
    }
}
