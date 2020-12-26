namespace Gu.Roslyn.Asserts
{
    using System;
    using Gu.Roslyn.Asserts.Internals;

    /// <summary>
    /// Assert for testing if code equals.
    /// </summary>
    public static class TextAssert
    {
        /// <summary>
        /// Verify that two strings of code are equal. Agnostic to end of line characters.
        /// </summary>
        /// <param name="expected">The expected code.</param>
        /// <param name="actual">The actual code.</param>
        public static void AreEqual(string expected, string actual)
        {
            if (expected is null)
            {
                throw new ArgumentNullException(nameof(expected));
            }

            if (actual is null)
            {
                throw new ArgumentNullException(nameof(actual));
            }

            AreEqual(expected, actual, null);
        }

        /// <summary>
        /// Verify that two strings of code are equal. Agnostic to end of line characters.
        /// </summary>
        /// <param name="expected">The expected code.</param>
        /// <param name="actual">The actual code.</param>
        /// <param name="messageHeader">The first line to add to the exception message on error.</param>
        internal static void AreEqual(string expected, string actual, string? messageHeader)
        {
            var pos = 0;
            var otherPos = 0;
            var line = 1;
            while (pos < expected.Length && otherPos < actual.Length)
            {
                if (expected[pos] == '\r')
                {
                    pos++;
                    continue;
                }

                if (actual[otherPos] == '\r')
                {
                    otherPos++;
                    continue;
                }

                if (expected[pos] != actual[otherPos])
                {
                    var errorBuilder = StringBuilderPool.Borrow();
                    if (messageHeader != null)
                    {
                        errorBuilder.AppendLine(messageHeader);
                    }

                    errorBuilder.AppendLine($"Mismatch on line {line}");
                    var expectedLine = expected.Split('\n')[line - 1].Trim('\r');
                    var actualLine = actual.Split('\n')[line - 1].Trim('\r');
                    var diffPos = Math.Min(expectedLine.Length, actualLine.Length);
                    for (var i = 0; i < Math.Min(expectedLine.Length, actualLine.Length); i++)
                    {
                        if (expectedLine[i] != actualLine[i])
                        {
                            diffPos = i;
                            break;
                        }
                    }

                    errorBuilder.AppendLine($"Expected: {expectedLine}");
                    errorBuilder.AppendLine($"Actual:   {actualLine}");
                    errorBuilder.AppendLine($"          {new string(' ', diffPos)}^");
                    throw new AssertException(errorBuilder.Return());
                }

                if (expected[pos] == '\n')
                {
                    line++;
                }

                pos++;
                otherPos++;
            }

            while (pos < expected.Length && expected[pos] == '\r')
            {
                pos++;
            }

            while (otherPos < actual.Length && actual[otherPos] == '\r')
            {
                otherPos++;
            }

            if (pos == expected.Length && otherPos == actual.Length)
            {
                return;
            }

            if (messageHeader != null)
            {
                throw new AssertException($"{messageHeader}{Environment.NewLine}" +
                                           $"Mismatch at end of text.");
            }

            throw new AssertException($"Mismatch at end of text.");
        }
    }
}
