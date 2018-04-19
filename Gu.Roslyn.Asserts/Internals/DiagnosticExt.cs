namespace Gu.Roslyn.Asserts.Internals
{
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.Formatting;
    using Microsoft.CodeAnalysis.Text;

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
            var idAndPosition = diagnostic.Location.GetMappedLineSpan();
            var match = sources.SingleOrDefault(x => CodeReader.FileName(x) == idAndPosition.Path);
            var line = match != null ? CodeReader.GetLineWithErrorIndicated(match, idAndPosition.StartLinePosition) : string.Empty;
            return $"{diagnostic.Id} {diagnostic.GetMessage(CultureInfo.InvariantCulture)}\r\n" +
                   $"  at line {idAndPosition.StartLinePosition.Line} and character {idAndPosition.StartLinePosition.Character} in file {idAndPosition.Path} | {line.TrimStart(' ')}";
        }

        /// <summary>
        /// Writes the diagnostic and the offending code.
        /// </summary>
        /// <returns>A string for use in assert exception</returns>
        internal static string ToErrorString(this Diagnostic diagnostic)
        {
            SourceText text = diagnostic.Location.SourceTree.GetText(CancellationToken.None);
            var idAndPosition = diagnostic.Location.GetMappedLineSpan();
            var code = text.ToString();
            var line = CodeReader.GetLineWithErrorIndicated(code, idAndPosition.StartLinePosition);
            return $"{diagnostic.Id} {diagnostic.GetMessage(CultureInfo.InvariantCulture)}\r\n" +
                   $"  at line {idAndPosition.StartLinePosition.Line} and character {idAndPosition.StartLinePosition.Character} in file {idAndPosition.Path} | {line.TrimStart(' ')}";
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
