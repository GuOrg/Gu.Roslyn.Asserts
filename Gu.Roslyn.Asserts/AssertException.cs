namespace Gu.Roslyn.Asserts
{
    using System;

    /// <summary>
    /// Thrown when an assertion failed.
    /// </summary>
    public class AssertException : Exception
    {
        private static Type exceptionType;

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

        /// <summary>
        /// Create an <see cref="AssertException"/> or an exception specific to the currently used test framework if found.
        /// </summary>
        /// <param name="message">The message explaining why the assertion failed.</param>
        /// <returns>The exception.</returns>
        public static Exception Create(string message)
        {
            if (TryGetExceptionType(out Type type))
            {
                return (Exception)Activator.CreateInstance(type, message);
            }

            return new AssertException(message);
        }

        /// <summary>
        /// Create an <see cref="AssertException"/> or an exception specific to the currently used test framework if found.
        /// </summary>
        /// <param name="innerException">The message explaining why the assertion failed.</param>
        /// <returns>The exception.</returns>
        public static Exception Create(Exception innerException)
        {
            if (TryGetExceptionType(out Type type))
            {
                return (Exception)Activator.CreateInstance(type, innerException.Message, innerException);
            }

            return new AssertException(innerException.Message, innerException);
        }

        /// <inheritdoc />
        public override string ToString() => this.Message;

        private static bool TryGetExceptionType(out Type type)
        {
            if (exceptionType == null)
            {
                exceptionType = Type.GetType("NUnit.Framework.AssertionException,nunit.framework", throwOnError: false, ignoreCase: false) ??
                                Type.GetType("Xunit.Sdk.XunitException,xunit.assert", throwOnError: false, ignoreCase: false) ??
                                typeof(AssertException);
            }

            type = exceptionType;
            return type != typeof(AssertException);
        }
    }
}