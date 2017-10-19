namespace Gu.Roslyn.Asserts
{
    using System.Collections.Generic;

    public static class CodeComparer
    {
        /// <summary>
        /// Verify that two strings of code are equal. Agnostic to end of line characters.
        /// </summary>
        /// <param name="xs">The expected code.</param>
        /// <param name="ys">The actual code.</param>
        /// <returns>True if the code is found to be equal</returns>
        public static bool Equals(IReadOnlyList<string> xs, IReadOnlyList<string> ys)
        {
            for (var i = 0; i < xs.Count; i++)
            {
                if (!Equals(xs[i], ys[i]))
                {
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// Checks if two strings of code are equal. Agnostic to end of line characters.
        /// </summary>
        /// <param name="x">The expected code.</param>
        /// <param name="y">The actual code.</param>
        /// <returns>True if the code is found to be equal</returns>
        public static bool Equals(string x, string y)
        {
            var pos = 0;
            var otherPos = 0;
            while (pos < x.Length && otherPos < y.Length)
            {
                if (x[pos] == '\r')
                {
                    pos++;
                    continue;
                }

                if (y[otherPos] == '\r')
                {
                    otherPos++;
                    continue;
                }

                if (x[pos] != y[otherPos])
                {
                    return false;
                }

                pos++;
                otherPos++;
            }

            while (pos < x.Length && x[pos] == '\r')
            {
                pos++;
            }

            while (otherPos < y.Length && y[otherPos] == '\r')
            {
                otherPos++;
            }

            return pos == x.Length && otherPos == y.Length;
        }
    }
}