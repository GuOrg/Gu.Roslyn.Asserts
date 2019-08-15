namespace Gu.Roslyn.Asserts
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text.RegularExpressions;
    using System.Threading;
    using System.Threading.Tasks;
    using Gu.Roslyn.Asserts.Internals;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.Formatting;
    using Microsoft.CodeAnalysis.Simplification;
    using Microsoft.CodeAnalysis.Text;

    /// <summary>
    /// Helper exposing methods for parsing code.
    /// </summary>
    public static class CodeReader
    {
        /// <summary>
        /// Verify that two strings of code are equal. Agnostic to end of line characters.
        /// </summary>
        /// <param name="xs">The expected code.</param>
        /// <param name="ys">The actual code.</param>
        /// <returns>True if the code is found to be equal.</returns>
        [Obsolete("To be removed use CodeComparer.Equals")]
        public static bool AreEqual(IReadOnlyList<string> xs, IReadOnlyList<string> ys)
        {
            return CodeComparer.Equals(xs, ys);
        }

        /// <summary>
        /// Checks if two strings of code are equal. Agnostic to end of line characters.
        /// </summary>
        /// <param name="x">The expected code.</param>
        /// <param name="y">The actual code.</param>
        /// <returns>True if the code is found to be equal.</returns>
        [Obsolete("To be removed use CodeComparer.Equals")]
        public static bool AreEqual(string x, string y)
        {
            return CodeComparer.Equals(x, y);
        }

        /// <summary>
        /// Get the code from the document.
        /// Runs simplifier and formatter on the document before returning the source text.
        /// </summary>
        /// <param name="document">The document.</param>
        public static string GetCode(this Document document)
        {
            return GetStringFromDocumentAsync(document, CancellationToken.None).GetAwaiter().GetResult();
        }

        [Obsolete("Use overload without annotation argument.")]
        public static string GetCode(this Document document, SyntaxAnnotation format) => GetCode(document);

        /// <summary>
        /// Get the filename from code as a string.
        /// </summary>
        /// <param name="code">The code to parse.</param>
        /// <param name="fileName">The file to read from <paramref name="code"/>.</param>
        /// <returns>The file name. </returns>
        public static bool TryGetFileName(string code, out string fileName)
        {
            fileName = null;
            if (string.IsNullOrEmpty(code))
            {
                return false;
            }

            var match = Regex.Match(code, @"^ *(↓?(public|internal|static|sealed|abstract) )*↓?(class|struct|enum|interface) ↓?(?<name>\w+)(<(?<type>↓?\w+)(, ?(?<type>↓?\w+))*>)?", RegexOptions.ExplicitCapture | RegexOptions.Multiline);
            if (match.Success)
            {
                fileName = match.Groups["name"].Value.Trim('↓');
                if (match.Groups["type"].Success)
                {
                    var args = string.Join(",", match.Groups["type"].Captures.OfType<Capture>().Select(c => c.Value.Trim(' ', '↓')));
                    fileName += $"{{{args}}}";
                }

                fileName = $"{fileName}.cs";
                return true;
            }

            if (code.Contains("assembly:"))
            {
                fileName = "AssemblyInfo.cs";
                return true;
            }

            if (code.StartsWith("<"))
            {
                if (code.Contains("<resheader name=\"resmimetype\">"))
                {
                    fileName = "Resources.resx";
                    return true;
                }

                return false;
            }

            return false;
        }

        /// <summary>
        /// Get the filename from code as a string.
        /// </summary>
        /// <param name="code">The code to parse.</param>
        /// <returns>The file name. </returns>
        public static string FileName(string code)
        {
            if (string.IsNullOrEmpty(code))
            {
                return $"Empty.cs";
            }

            if (TryGetFileName(code, out var fileName))
            {
                return fileName;
            }

            if (code.StartsWith("<"))
            {
                return "Unknown.xml";
            }

            if (code.StartsWith("{"))
            {
                return "Unknown.json";
            }

            return "Unknown";
        }

        /// <summary>
        /// Get the namespace from code as a string.
        /// </summary>
        /// <param name="code">The code to parse.</param>
        /// <returns>The namespace. </returns>
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
        /// Get the line positions indicated with ↓ from code as a string.
        /// </summary>
        /// <param name="code">The code to parse.</param>
        /// <returns>The positions of the expected diagnostics.</returns>
        public static IEnumerable<LinePosition> FindLinePositions(string code)
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

        /// <summary>
        /// Gets the line indicated by <paramref name="position"/> and inserts ↓ before the character.
        /// </summary>
        /// <param name="code">The code to parse.</param>
        /// <param name="position">The error position.</param>
        /// <returns>A string with the line with error indicated.</returns>
        public static string GetLineWithErrorIndicated(string code, LinePosition position)
        {
            var builder = StringBuilderPool.Borrow();
            var line = 0;
            var character = 0;
            foreach (var c in code)
            {
                if (c == '\r')
                {
                    continue;
                }

                if (c == '\n')
                {
                    if (!builder.IsEmpty)
                    {
                        return builder.Return();
                    }

                    line++;
                    character = 0;
                    continue;
                }

                if (line == position.Line)
                {
                    if (character == position.Character)
                    {
                        builder.Append('↓');
                    }

                    builder.Append(c);
                }

                character++;
            }

            if (!builder.IsEmpty)
            {
                if (position.Character == builder.Length)
                {
                    builder.Append('↓');
                }

                return builder.Return();
            }

            _ = StringBuilderPool.Return(builder);
            return $"Code did not have position {position}";
        }

        /// <summary>
        /// Gets the simplified and formatted text from the document.
        /// </summary>
        /// <param name="document">The document to extract the source code from.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>A <see cref="Task"/> with the source text for the document.</returns>
        internal static async Task<string> GetStringFromDocumentAsync(Document document, CancellationToken cancellationToken)
        {
            document = await Simplifier.ReduceAsync(document, cancellationToken: cancellationToken).ConfigureAwait(false);
            document = await Formatter.FormatAsync(document, Formatter.Annotation, cancellationToken: cancellationToken).ConfigureAwait(false);
            document = await Formatter.FormatAsync(document, SyntaxAnnotation.ElasticAnnotation, cancellationToken: cancellationToken).ConfigureAwait(false);
            var sourceText = await document.GetTextAsync(cancellationToken).ConfigureAwait(false);
            return sourceText.ToString();
        }
    }
}
