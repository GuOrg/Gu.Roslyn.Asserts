namespace Gu.Roslyn.Asserts.Internals
{
    using System.Collections.Concurrent;
    using System.Text;

    /// <summary>
    /// Cache for <see cref="StringBuilder"/>
    /// </summary>
    internal static class StringBuilderPool
    {
        private static readonly ConcurrentQueue<StringBuilder> Cache = new ConcurrentQueue<StringBuilder>();

        /// <summary>
        /// Get an instance from cache, remember to return it.
        /// </summary>
        /// <returns>A <see cref="StringBuilder"/></returns>
        internal static StringBuilder Borrow()
        {
            return Cache.TryDequeue(out var result)
                    ? result
                    : new StringBuilder();
        }

        /// <summary>
        /// Return <paramref name="stringBuilder"/> to the cache.
        /// </summary>
        /// <param name="stringBuilder">The instance.</param>
        internal static void Return(StringBuilder stringBuilder)
        {
            stringBuilder.Clear();
            Cache.Enqueue(stringBuilder);
        }

        /// <summary>
        /// Return <paramref name="stringBuilder"/> to the cache.
        /// </summary>
        /// <param name="stringBuilder">The instance.</param>
        /// <returns>The contents of <paramref name="stringBuilder"/></returns>
        internal static string ReturnAndGetText(StringBuilder stringBuilder)
        {
            var text = stringBuilder.ToString();
            Return(stringBuilder);
            return text;
        }
    }
}
