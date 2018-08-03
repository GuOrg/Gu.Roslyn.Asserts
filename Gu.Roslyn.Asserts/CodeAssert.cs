namespace Gu.Roslyn.Asserts
{
    using System;
    using Gu.Roslyn.Asserts.Internals;
    using Microsoft.CodeAnalysis;

    /// <summary>
    /// Assert for testing if code equals
    /// </summary>
    public static class CodeAssert
    {
        /// <summary>
        /// Verify that two strings of code are equal. Agnostic to end of line characters.
        /// </summary>
        /// <param name="expected">The expected code.</param>
        /// <param name="actual">The actual code.</param>
        public static void AreEqual(string expected, string actual)
        {
            AreEqual(expected, actual, null);
        }

        /// <summary>
        /// Verify that two strings of code are equal. Agnostic to end of line characters.
        /// </summary>
        /// <param name="expected">The expected code.</param>
        /// <param name="actual">The actual code.</param>
        public static void AreEqual(string expected, Document actual)
        {
            AreEqual(expected, actual.GetCode(null), null);
        }

        /// <summary>
        /// Verify that two strings of code are equal. Agnostic to end of line characters.
        /// </summary>
        /// <param name="expected">The expected code.</param>
        /// <param name="actual">The actual code.</param>
        public static void AreEqual(Document expected, Document actual)
        {
            AreEqual(expected.GetCode(null), actual.GetCode(null), null);
        }

        /// <summary>
        /// Verify that two strings of code are equal. Agnostic to end of line characters.
        /// </summary>
        /// <param name="expected">The expected code.</param>
        /// <param name="actual">The actual code.</param>
        public static void AreEqual(Document expected, string actual)
        {
            AreEqual(
                expected.GetCode(null),
                actual,
                null);
        }

        /// <summary>
        /// Verify that two strings of code are equal. Agnostic to end of line characters.
        /// </summary>
        /// <param name="expected">The expected code.</param>
        /// <param name="actual">The actual code.</param>
        /// <param name="messageHeader">The first line to add to the exception message on error.</param>
        internal static void AreEqual(string expected, string actual, string messageHeader)
        {
            var expectedPos = 0;
            var actualPos = 0;
            var line = 1;
            while (expectedPos < expected.Length && actualPos < actual.Length)
            {
                var ec = expected[expectedPos];
                var ac = actual[actualPos];
                if (ec == '\r' || ac == '\r')
                {
                    if (ec == '\r')
                    {
                        expectedPos++;
                    }

                    if (ac == '\r')
                    {
                        actualPos++;
                    }

                    continue;
                }

                if (ec != ac)
                {
                    if (IsAt(expected, expectedPos, "\\r") ||
                        IsAt(actual, actualPos, "\\r"))
                    {
                        if (IsAt(expected, expectedPos, "\\r"))
                        {
                            expectedPos += 2;
                        }

                        if (IsAt(actual, actualPos, "\\r"))
                        {
                            actualPos += 2;
                        }

                        continue;
                    }

                    var errorBuilder = StringBuilderPool.Borrow();
                    if (messageHeader != null)
                    {
                        errorBuilder.AppendLine(messageHeader);
                    }

                    errorBuilder.AppendLine($"Mismatch on line {line} of file {CodeReader.FileName(expected)}");
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

                    errorBuilder.AppendLine($"Expected: {expectedLine}")
                                .AppendLine($"Actual:   {actualLine}")
                                .AppendLine($"          {new string(' ', diffPos)}^")
                                .AppendLine("Expected:")
                                .Append(expected)
                                .AppendLine()
                                .AppendLine("Actual:")
                                .Append(actual)
                                .AppendLine();

                    throw AssertException.Create(errorBuilder.Return());
                }

                if (ec == '\n')
                {
                    line++;
                }

                expectedPos++;
                actualPos++;
            }

            while (expectedPos < expected.Length && expected[expectedPos] == '\r')
            {
                expectedPos++;
            }

            while (actualPos < actual.Length && actual[actualPos] == '\r')
            {
                actualPos++;
            }

            if (expectedPos == expected.Length && actualPos == actual.Length)
            {
                return;
            }

            if (messageHeader != null)
            {
                var message = StringBuilderPool.Borrow()
                                               .AppendLine(messageHeader)
                                               .AppendLine($"Mismatch at end of file {CodeReader.FileName(expected)}")
                                               .AppendLine("Expected:")
                                               .Append(expected)
                                               .AppendLine()
                                               .AppendLine("Actual:")
                                               .Append(actual)
                                               .AppendLine()
                                               .Return();
                throw AssertException.Create(message);
            }
            else
            {
                var message = StringBuilderPool.Borrow()
                                               .AppendLine($"Mismatch at end of file {CodeReader.FileName(expected)}")
                                               .AppendLine("Expected:")
                                               .Append(expected)
                                               .AppendLine()
                                               .AppendLine("Actual:")
                                               .Append(actual)
                                               .AppendLine()
                                               .Return();
                throw AssertException.Create(message);
            }
        }

        private static bool IsAt(string text, int pos, string toMatch)
        {
            if (text.Length <= pos)
            {
                return false;
            }

            var start = pos - toMatch.Length + 1;
            if (start < 0)
            {
                return false;
            }

            for (var i = toMatch.Length - 1; i >= 0; i--)
            {
                if (text[start + i] != toMatch[i])
                {
                    return false;
                }
            }

            return true;
        }
    }
}
