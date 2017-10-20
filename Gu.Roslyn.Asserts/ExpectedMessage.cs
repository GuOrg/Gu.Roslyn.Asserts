namespace Gu.Roslyn.Asserts
{
    using System;
    using System.Collections.Generic;

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
    }
}