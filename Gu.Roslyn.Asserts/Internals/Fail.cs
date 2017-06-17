namespace Gu.Roslyn.Asserts.Internals
{
    using System;
    using System.Diagnostics;
    using System.IO;
    using System.Reflection;

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
                if (TryGetExceptionType(out Type type))
                {
                    createFromText = text => (Exception)Activator.CreateInstance(type, text);
                }
                else
                {
                    createFromText = text => new AssertException(text);
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
                if (TryGetExceptionType(out Type type))
                {
                    createFromException = exception => (Exception)Activator.CreateInstance(type, exception.Message, exception);
                }
                else
                {
                    createFromException = exception => new AssertException(exception.Message, exception);
                }
            }

            return createFromException(innerException);
        }

        private static bool TryGetExceptionType(out Type type)
        {
            type = null;
            var nunit = Path.Combine(AppContext.BaseDirectory, "nunit.framework.dll");
            if (File.Exists(nunit))
            {
                var assembly = Assembly.Load(new AssemblyName($"nunit.framework, Version={FileVersionInfo.GetVersionInfo(nunit).ProductVersion}, Culture=neutral, PublicKeyToken=2638cd05610744eb"));
                type = assembly.GetType("NUnit.Framework.AssertionException");
            }

            return type != null;
        }
    }
}
