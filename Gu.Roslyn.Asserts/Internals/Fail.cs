namespace Gu.Roslyn.Asserts.Internals
{
    using System;

    /// <summary>
    /// Helper for throwing assert exceptions.
    /// </summary>
    internal static class Fail
    {
        private static Func<string, Exception> createFromText;
        private static Func<Exception, Exception> createFromException;

        /// <summary>
        /// Throw an <see cref="AssertException"/> or an exception specific to the testing library used if found.
        /// </summary>
        /// <param name="message">The message explaining why the assertion failed.</param>
        internal static void WithMessage(string message)
        {
            throw CreateException(message);
        }

        /// <summary>
        /// Create an assert exception or an exception specific to the currently used test framework if found.
        /// </summary>
        /// <param name="message">The message explaining why the assertion failed.</param>
        /// <returns>The exception.</returns>
        internal static Exception CreateException(string message)
        {
            if (createFromText == null)
            {
                var type = Type.GetType(
                    "NUnit.Framework.AssertionException, nunit.framework, Version=3.6.1.0, Culture=neutral, PublicKeyToken=2638cd05610744eb",
                    throwOnError: false);
                if (type == null)
                {
                    createFromText = text => new AssertException(text);
                }
                else
                {
                    createFromText = text => (Exception)Activator.CreateInstance(type, text);
                }
            }

            return createFromText(message);
        }

        /// <summary>
        /// Create an assert exception or an exception specific to the currently used test framework if found.
        /// </summary>
        /// <param name="innerException">The message explaining why the assertion failed.</param>
        /// <returns>The exception.</returns>
        internal static Exception CreateException(Exception innerException)
        {
            if (createFromException == null)
            {
                var type = Type.GetType(
                    "NUnit.Framework.AssertionException, nunit.framework, Version=3.6.1.0, Culture=neutral, PublicKeyToken=2638cd05610744eb",
                    throwOnError: false);
                if (type == null)
                {
                    createFromException = exception => new AssertException(exception.Message, exception);
                }
                else
                {
                    createFromException = exception => (Exception)Activator.CreateInstance(type, exception.Message, exception);
                }
            }

            return createFromException(innerException);
        }
    }
}
