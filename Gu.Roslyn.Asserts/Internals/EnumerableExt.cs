namespace Gu.Roslyn.Asserts.Internals
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Helper class for applying code fixes.
    /// </summary>
    internal static partial class EnumerableExt
    {
        /// <summary>
        /// Return the element at index if exists.
        /// </summary>
        /// <typeparam name="T">The type of the items in the collection.</typeparam>
        /// <param name="source">The source collection.</param>
        /// <param name="index">The index.</param>
        /// <param name="result">The single item.</param>
        /// <returns>True if the collection contains an item at the specified index.</returns>
        internal static bool TryElementAt<T>(this IEnumerable<T> source, int index, out T result)
        {
            result = default(T);
            if (source == null)
            {
                return false;
            }

            var current = 0;
            using (var e = source.GetEnumerator())
            {
                while (e.MoveNext())
                {
                    if (current == index)
                    {
                        result = e.Current;
                        return true;
                    }

                    current++;
                }
            }

            return false;
        }

        /// <summary>
        /// Return the single element if it exists.
        /// </summary>
        /// <typeparam name="T">The type of the items in the collection.</typeparam>
        /// <param name="source">The source collection.</param>
        /// <param name="result">The single item.</param>
        /// <returns>True if the collection contains exactly one non null item.</returns>
        internal static bool TrySingle<T>(this IEnumerable<T> source, out T result)
        {
            result = default(T);
            if (source == null)
            {
                return false;
            }

            using (var e = source.GetEnumerator())
            {
                if (e.MoveNext())
                {
                    result = e.Current;
                    if (!e.MoveNext())
                    {
                        return true;
                    }

                    return false;
                }

                result = default(T);
                return false;
            }
        }

        /// <summary>
        /// Return the single element matching predicate if it exists.
        /// </summary>
        /// <typeparam name="T">The type of the items in the collection.</typeparam>
        /// <param name="source">The source collection.</param>
        /// <param name="selector">The predicate.</param>
        /// <param name="result">The single item.</param>
        /// <returns>True if the collection contains exactly one non null item.</returns>
        internal static bool TrySingle<T>(this IEnumerable<T> source, Func<T, bool> selector, out T result)
        {
            result = default(T);
            if (source == null)
            {
                return false;
            }

            using (var e = source.GetEnumerator())
            {
                while (e.MoveNext())
                {
                    result = e.Current;
                    if (selector(result))
                    {
                        while (e.MoveNext())
                        {
                            if (selector(e.Current))
                            {
                                result = default(T);
                                return false;
                            }
                        }

                        return true;
                    }
                }
            }

            result = default(T);
            return false;
        }

        /// <summary>
        /// Return the single element matching predicate if it exists.
        /// </summary>
        /// <typeparam name="T">The type of the items in the collection.</typeparam>
        /// <typeparam name="TResult">The type to find.</typeparam>
        /// <param name="source">The source collection.</param>
        /// <param name="result">The single item.</param>
        /// <returns>True if the collection contains exactly one non null item.</returns>
        internal static bool TrySingleOfType<T, TResult>(this IEnumerable<T> source, out TResult result)
            where TResult : T
        {
            result = default(TResult);
            if (source == null)
            {
                return false;
            }

            using (var e = source.GetEnumerator())
            {
                while (e.MoveNext())
                {
                    if (e.Current is TResult candidate)
                    {
                        while (e.MoveNext())
                        {
                            if (e.Current is TResult)
                            {
                                return false;
                            }
                        }

                        result = candidate;
                        return true;
                    }
                }
            }

            return false;
        }

        /// <summary>
        /// Return the first element if it exists.
        /// </summary>
        /// <typeparam name="T">The type of the items in the collection.</typeparam>
        /// <param name="source">The source collection.</param>
        /// <param name="result">The single item.</param>
        /// <returns>True if the collection contains exactly one non null item.</returns>
        internal static bool TryFirst<T>(this IEnumerable<T> source, out T result)
        {
            result = default(T);
            if (source == null)
            {
                return false;
            }

            using (var e = source.GetEnumerator())
            {
                if (e.MoveNext())
                {
                    result = e.Current;
                    return true;
                }

                return false;
            }
        }

        /// <summary>
        /// Return the first element matching predicate if it exists.
        /// </summary>
        /// <typeparam name="T">The type of the items in the collection.</typeparam>
        /// <param name="source">The source collection.</param>
        /// <param name="selector">The predicate.</param>
        /// <param name="result">The single item.</param>
        /// <returns>True if the collection contains exactly one non null item.</returns>
        internal static bool TryFirst<T>(this IEnumerable<T> source, Func<T, bool> selector, out T result)
        {
            if (source == null)
            {
                result = default(T);
                return false;
            }

            using (var e = source.GetEnumerator())
            {
                while (e.MoveNext())
                {
                    result = e.Current;
                    if (selector(result))
                    {
                       return true;
                    }
                }
            }

            result = default(T);
            return false;
        }

        /// <summary>
        /// Return the item with minimum keyed by <paramref name="selector"/>.
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
        /// Return the item with minimum keyed by <paramref name="selector"/>.
        /// </summary>
        /// <typeparam name="TSource">The type of the items in the collection.</typeparam>
        /// <typeparam name="TKey">The type of the value to compare.</typeparam>
        /// <param name="source">The source collection.</param>
        /// <param name="selector">The single item.</param>
        /// <param name="comparer">The <see cref="IComparer{TKey}"/>.</param>
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
