namespace Gu.Roslyn.Asserts
{
    using System;

    /// <summary>
    /// Thrown when an assertion failed.
    /// </summary>
    public class AssertException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AssertException"/> class.
        /// </summary>
        /// <param name="message"> The error message that explains the reason for the exception.</param>
        public AssertException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AssertException"/> class.
        /// </summary>
        /// <param name="message"> The error message that explains the reason for the exception.</param>
        /// <param name="innerException"> The exception that caused the current exception.</param>
        public AssertException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        /// <inheritdoc />
        public override string ToString() => this.Message;
    }
}