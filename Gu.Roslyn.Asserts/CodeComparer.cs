namespace Gu.Roslyn.Asserts
{
    using System.Collections.Generic;

    /// <summary>
    /// Exposes methods for comparing code.
    /// </summary>
    public static class CodeComparer
    {
        /// <summary>
        /// Verify that two strings of code are equal. Agnostic to end of line characters.
        /// </summary>
        /// <param name="xs">The expected code.</param>
        /// <param name="ys">The actual code.</param>
        /// <returns>True if the code is found to be equal.</returns>
        public static bool Equals(IReadOnlyList<string> xs, IReadOnlyList<string> ys)
        {
            if (xs is null &&
                ys is null)
            {
                return true;
            }

            if (xs is null ||
                ys is null)
            {
                return false;
            }

            if (xs.Count != ys.Count)
            {
                return false;
            }

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
        /// <returns>True if the code is found to be equal.</returns>
        public static bool Equals(string x, string y)
        {
            if (x is null &&
                y is null)
            {
                return true;
            }

            if (x is null ||
                y is null)
            {
                return false;
            }

            var xPos = 0;
            var yPos = 0;
            while (xPos < x.Length && yPos < y.Length)
            {
                if (x[xPos] == '\r' || y[yPos] == '\r')
                {
                    if (x[xPos] == '\r')
                    {
                        xPos++;
                    }

                    if (y[yPos] == '\r')
                    {
                        yPos++;
                    }

                    continue;
                }

                if (x[xPos] != y[yPos])
                {
                    return false;
                }

                xPos++;
                yPos++;
            }

            while (xPos < x.Length && x[xPos] == '\r')
            {
                xPos++;
            }

            while (yPos < y.Length && y[yPos] == '\r')
            {
                yPos++;
            }

            return xPos == x.Length && yPos == y.Length;
        }
    }
}
