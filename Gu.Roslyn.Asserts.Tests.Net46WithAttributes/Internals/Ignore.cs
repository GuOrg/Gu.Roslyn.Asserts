namespace Gu.Roslyn.Asserts.Tests.Net46WithAttributes
{
    /// <summary>
    /// For suppressing GU0011 in a clear way.
    /// </summary>
    internal static class Ignore
    {
        /// <summary>
        /// For suppressing GU0011 in a clear way.
        /// </summary>
        /// <typeparam name="T">The type.</typeparam>
        /// <param name="returnValue">The value.</param>
        // ReSharper disable once UnusedParameter.Global
        internal static void IgnoreReturnValue<T>(this T returnValue)
        {
        }
    }
}
