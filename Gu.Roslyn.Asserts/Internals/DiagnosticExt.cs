namespace Gu.Roslyn.Asserts.Internals
{
    using System.Globalization;
    using System.Threading;
    using Microsoft.CodeAnalysis;

    /// <summary>
    /// Helper for working with <see cref="Diagnostic"/>.
    /// </summary>
    internal static class DiagnosticExt
    {
        /// <summary>
        /// Writes the diagnostic and the offending code.
        /// </summary>
        /// <returns>A string for use in assert exception.</returns>
        internal static string ToErrorString(this Diagnostic diagnostic)
        {
            var idAndPosition = diagnostic.Location.GetMappedLineSpan();
            var code = diagnostic.Location.SourceTree?.GetText(CancellationToken.None).ToString() ?? string.Empty;
            var line = CodeReader.GetLineWithErrorIndicated(code, idAndPosition.StartLinePosition);
            return $"{diagnostic.Id} {diagnostic.GetMessage(CultureInfo.InvariantCulture)}\r\n" +
                   $"  at line {idAndPosition.StartLinePosition.Line} and character {idAndPosition.StartLinePosition.Character} in file {idAndPosition.Path} | {line.TrimStart(' ')}";
        }
    }
}
