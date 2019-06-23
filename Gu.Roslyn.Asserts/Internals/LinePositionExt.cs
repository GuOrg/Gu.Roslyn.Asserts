namespace Gu.Roslyn.Asserts.Internals
{
    using Microsoft.CodeAnalysis.Text;

    /// <summary>
    /// Extension methods for <see cref="LinePosition"/>.
    /// </summary>
    internal static class LinePositionExt
    {
        /// <summary>
        /// returns the zero based position in the string if it exists.
        /// </summary>
        /// <param name="position">The <see cref="LinePosition"/>.</param>
        /// <param name="text">The text.</param>
        /// <param name="index">The zero based index.</param>
        /// <returns>True if <paramref name="text"/> contains <paramref name="position"/>.</returns>
        internal static bool TryGetIndex(this LinePosition position, string text, out int index)
        {
            if (TryGetLineStartIndex(out index) &&
                TryGetCharacter(index, out index))
            {
                return true;
            }

            index = -1;
            return false;

            bool TryGetLineStartIndex(out int result)
            {
                var line = 0;
                for (var i = 0; i < text.Length; i++)
                {
                    if (text[i] == '\n')
                    {
                        i++;
                        line++;
                    }

                    if (line == position.Line)
                    {
                        result = i;
                        return result < text.Length;
                    }
                }

                result = -1;
                return false;
            }

            bool TryGetCharacter(int start, out int result)
            {
                for (var i = start; i < text.Length; i++)
                {
                    if (text[i] == '\n')
                    {
                        result = -1;
                        return false;
                    }

                    if (i - start == position.Character)
                    {
                        result = i;
                        return true;
                    }
                }

                result = -1;
                return false;
            }
        }
    }
}
