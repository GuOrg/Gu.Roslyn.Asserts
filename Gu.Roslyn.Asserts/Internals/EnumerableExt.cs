namespace Gu.Roslyn.Asserts.Internals
{
    using System.Collections.Generic;

    /// <summary>
    /// Helper class for applying code fixes
    /// </summary>
    internal static class EnumerableExt
    {
        /// <summary>
        /// Return the single element or null
        /// </summary>
        /// <typeparam name="T">The type of the items in the collection.</typeparam>
        /// <param name="source">The source collection.</param>
        /// <param name="single">The single item.</param>
        /// <returns>True if the collection contains exactly one non null item.</returns>
        internal static bool TryGetSingle<T>(this IEnumerable<T> source, out T single)
            where T : class
        {
            single = null;
            foreach (var item in source)
            {
                if (single == null)
                {
                    single = item;
                }
                else
                {
                    single = null;
                    return false;
                }
            }

            return single != null;
        }
    }
}