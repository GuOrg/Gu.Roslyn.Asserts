namespace Gu.Roslyn.Asserts
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using Microsoft.CodeAnalysis;

    /// <summary>
    /// A type for validating against expected message for a diagnostic.
    /// </summary>
    [Obsolete("Use ExpectedDiagnostic")]
    public class ExpectedMessage
    {
        private ExpectedMessage(string message, IReadOnlyList<object> args)
        {
            this.Message = message;
            this.Args = args;
        }

        /// <summary>
        /// Gets the expected message as text
        /// </summary>
        public string Message { get; }

        /// <summary>
        /// Gets the arguments passed to the message format string.
        /// </summary>
        public IReadOnlyList<object> Args { get; }

        /// <summary>
        /// Create an expected message from the expected text
        /// </summary>
        public static ExpectedMessage Create(string message) => new ExpectedMessage(message, null);

        /// <summary>
        /// Create an expected message from the expected arguments to the format
        /// </summary>
        public static ExpectedMessage Create(IReadOnlyList<object> args) => new ExpectedMessage(null, args);

        /// <summary>
        /// Check that expected and diagnostic message matches.
        /// </summary>
        public void AssertIsMatch(Diagnostic diagnostic)
        {
            var expected = this.Message ??
                           string.Format(CultureInfo.InvariantCulture, diagnostic.Descriptor.MessageFormat.ToString(CultureInfo.InvariantCulture), this.Args.ToArray());

            var actual = diagnostic.GetMessage(CultureInfo.InvariantCulture);
            TextAssert.AreEqual(expected, actual, $"Expected and actual diagnostic message for the diagnostic {diagnostic} does not match");
        }
    }
}