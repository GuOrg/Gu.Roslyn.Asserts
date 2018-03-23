namespace Gu.Roslyn.Asserts.Internals
{
    using System.Threading.Tasks;

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

        /// <summary>
        /// For suppressing GU0011 in a clear way.
        /// </summary>
        /// <typeparam name="T">The type.</typeparam>
        /// <param name="returnValue">The value.</param>
        // ReSharper disable once UnusedParameter.Global
        internal static void IgnoreReturnValue<T>(this Task<T> returnValue)
        {
            returnValue.GetAwaiter()
                       .GetResult()
                       .IgnoreReturnValue();
        }

        /// <summary>
        /// For suppressing GU0011 in a clear way.
        /// </summary>
        /// <param name="returnValue">The value.</param>
        // ReSharper disable once UnusedParameter.Global
        internal static void IgnoreReturnValue(this Task returnValue)
        {
            returnValue.GetAwaiter()
                       .GetResult();
        }
    }
}
