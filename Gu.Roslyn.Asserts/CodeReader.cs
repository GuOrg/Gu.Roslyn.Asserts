namespace Gu.Roslyn.Asserts
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Text.RegularExpressions;
    using Microsoft.CodeAnalysis.Text;

    /// <summary>
    /// Helper exposing methods for parsing code.
    /// </summary>
    public static class CodeReader
    {
        /// <summary>
        /// Get the filename from code as a string.
        /// </summary>
        /// <param name="code">The code to parse.</param>
        /// <returns>The file name </returns>
        public static string FileName(string code)
        {
            string fileName;
            if (string.IsNullOrEmpty(code))
            {
                return $"Empty.cs";
            }

            var match = Regex.Match(code, @"(class|struct|enum|interface) (?<name>\w+)(<(?<type>\w+)(, ?(?<type>\w+))*>)?", RegexOptions.ExplicitCapture);
            if (!match.Success)
            {
                return "AssemblyInfo.cs";
            }

            fileName = match.Groups["name"].Value;
            if (match.Groups["type"].Success)
            {
                var args = string.Join(
                    ",",
                    match.Groups["type"]
                         .Captures.OfType<Capture>()
                         .Select(c => c.Value.Trim()));
                fileName += $"{{{args}}}";
            }

            return $"{fileName}.cs";
        }

        /// <summary>
        /// Get the namespace from code as a string.
        /// </summary>
        /// <param name="code">The code to parse.</param>
        /// <returns>The namespace </returns>
        public static string Namespace(string code)
        {
            const string nameSpacePattern = @"(?<name>\w+(\.\w+)*)";
            var match = Regex.Match(code, $"namespace {nameSpacePattern}", RegexOptions.ExplicitCapture);
            if (match.Success)
            {
                return match.Groups["name"].Value;
            }

            match = Regex.Match(code, $@"\[assembly: AssemblyTitle\(""{nameSpacePattern}""\)\]", RegexOptions.ExplicitCapture);
            if (match.Success)
            {
                return match.Groups["name"].Value;
            }

            return "Unknown";
        }

        /// <summary>
        /// Get the diagnostics from code as a string.
        /// </summary>
        /// <param name="code">The code to parse.</param>
        /// <returns>The positions of the expected diagnostics. </returns>
        public static IEnumerable<LinePosition> FindDiagnosticsPositions(string code)
        {
            var line = 0;
            var character = 0;
            foreach (var c in code)
            {
                if (c == '\n')
                {
                    line++;
                    character = 0;
                    continue;
                }

                if (c == '↓')
                {
                    yield return new LinePosition(line, character);
                    continue;
                }

                character++;
            }
        }
    }
}