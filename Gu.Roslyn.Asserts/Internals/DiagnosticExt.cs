namespace Gu.Roslyn.Asserts.Internals
{
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.Formatting;

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
            var idAndPosition = IdAndPosition.Create(diagnostic);
            var match = sources.SingleOrDefault(x => CodeReader.FileName(x) == idAndPosition.Span.Path);
            var line = match != null ? CodeReader.GetLineWithErrorIndicated(match, idAndPosition.Span.StartLinePosition) : string.Empty;
            return $"{diagnostic.Id} {diagnostic.GetMessage(CultureInfo.InvariantCulture)}\r\n" +
                   $"  at line {idAndPosition.Span.StartLinePosition.Line} and character {idAndPosition.Span.StartLinePosition.Character} in file {idAndPosition.Span.Path} |{line}";
        }

        /// <summary>
        /// Writes the diagnostic and the offending code.
        /// </summary>
        /// <returns>A string for use in assert exception</returns>
        internal static async Task<string> ToStringAsync(this Diagnostic diagnostic, Solution solution)
        {
            var sources = await Task.WhenAll(solution.Projects.SelectMany(p => p.Documents).Select(d => CodeReader.GetStringFromDocumentAsync(d, Formatter.Annotation, CancellationToken.None)));
            return diagnostic.ToString(sources);
        }
    }
}
