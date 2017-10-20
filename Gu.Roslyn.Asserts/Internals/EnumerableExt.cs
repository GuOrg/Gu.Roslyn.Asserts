namespace Gu.Roslyn.Asserts.Internals
{
    using System;
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

        /// <summary>
        /// Return the item with minimum keyed by <paramref name="selector"/>
        /// </summary>
        /// <typeparam name="TSource">The type of the items in the collection.</typeparam>
        /// <typeparam name="TKey">The type of the value to compare.</typeparam>
        /// <param name="source">The source collection.</param>
        /// <param name="selector">The single item.</param>
        /// <returns>The item with minimum keyed by <paramref name="selector"/>.</returns>
        internal static TSource MinBy<TSource, TKey>(this IEnumerable<TSource> source, Func<TSource, TKey> selector)
        {
            return source.MinBy(selector, null);
        }

        /// <summary>
        /// Return the item with minimum keyed by <paramref name="selector"/>
        /// </summary>
        /// <typeparam name="TSource">The type of the items in the collection.</typeparam>
        /// <typeparam name="TKey">The type of the value to compare.</typeparam>
        /// <param name="source">The source collection.</param>
        /// <param name="selector">The single item.</param>
        /// <param name="comparer">The <see cref="IComparer{TKey}"/></param>
        /// <returns>The item with minimum keyed by <paramref name="selector"/>.</returns>
        internal static TSource MinBy<TSource, TKey>(this IEnumerable<TSource> source, Func<TSource, TKey> selector, IComparer<TKey> comparer)
        {
            if (source == null)
            {
                throw new ArgumentNullException(nameof(source));
            }

            if (selector == null)
            {
                throw new ArgumentNullException(nameof(selector));
            }

            comparer = comparer ?? Comparer<TKey>.Default;
            using (var sourceIterator = source.GetEnumerator())
            {
                if (!sourceIterator.MoveNext())
                {
                    throw new InvalidOperationException("Sequence contains no elements");
                }

                var min = sourceIterator.Current;
                var minKey = selector(min);
                while (sourceIterator.MoveNext())
                {
                    var candidate = sourceIterator.Current;
                    var candidateKey = selector(candidate);
                    if (comparer.Compare(candidateKey, minKey) < 0)
                    {
                        min = candidate;
                        minKey = candidateKey;
                    }
                }

                return min;
            }
        }
    }
}