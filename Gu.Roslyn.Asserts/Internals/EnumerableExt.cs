namespace Gu.Roslyn.Asserts.Internals
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;

    /// <summary>
    /// Helper class for applying code fixes.
    /// </summary>
    internal static class EnumerableExt
    {
        /// <summary>
        /// Return the element at index if exists.
        /// </summary>
        /// <typeparam name="T">The type of the items in the collection.</typeparam>
        /// <param name="source">The source collection.</param>
        /// <param name="index">The index.</param>
        /// <param name="result">The single item.</param>
        /// <returns>True if the collection contains an item at the specified index.</returns>
        internal static bool TryElementAt<T>(this IEnumerable<T> source, int index, [MaybeNullWhen(false)]out T result)
        {
            result = default!;
            if (source is null)
            {
                return false;
            }

            var current = 0;
            using var e = source.GetEnumerator();
            while (e.MoveNext())
            {
                if (current == index)
                {
                    result = e.Current;
                    return true;
                }

                current++;
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
        internal static bool TrySingle<T>(this IEnumerable<T> source, [MaybeNullWhen(false)]out T result)
        {
            result = default!;
            if (source is null)
            {
                return false;
            }

            using var e = source.GetEnumerator();
            if (e.MoveNext())
            {
                result = e.Current;
                if (!e.MoveNext())
                {
                    return true;
                }

                return false;
            }

            result = default!;
            return false;
        }

        /// <summary>
        /// Return the single element matching predicate if it exists.
        /// </summary>
        /// <typeparam name="T">The type of the items in the collection.</typeparam>
        /// <param name="source">The source collection.</param>
        /// <param name="selector">The predicate.</param>
        /// <param name="result">The single item.</param>
        /// <returns>True if the collection contains exactly one non null item.</returns>
        internal static bool TrySingle<T>(this IEnumerable<T> source, Func<T, bool> selector, [MaybeNullWhen(false)]out T result)
        {
            result = default!;
            if (source is null)
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
                                result = default!;
                                return false;
                            }
                        }

                        return true;
                    }
                }
            }

            result = default!;
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
        internal static bool TrySingleOfType<T, TResult>(this IEnumerable<T> source, [MaybeNullWhen(false)]out TResult result)
            where TResult : T
        {
            result = default!;
            if (source is null)
            {
                return false;
            }

            using var e = source.GetEnumerator();
            while (e.MoveNext())
            {
#pragma warning disable CA1508 // Avoid dead conditional code
                if (e.Current is TResult candidate)
#pragma warning restore CA1508 // Avoid dead conditional code
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

            return false;
        }

        /// <summary>
        /// Return the first element if it exists.
        /// </summary>
        /// <typeparam name="T">The type of the items in the collection.</typeparam>
        /// <param name="source">The source collection.</param>
        /// <param name="result">The single item.</param>
        /// <returns>True if the collection contains exactly one non null item.</returns>
        internal static bool TryFirst<T>(this IEnumerable<T> source, [MaybeNullWhen(false)]out T result)
        {
            result = default!;
            if (source is null)
            {
                return false;
            }

            using var e = source.GetEnumerator();
            if (e.MoveNext())
            {
                result = e.Current;
                return true;
            }

            return false;
        }

        /// <summary>
        /// Return the first element matching predicate if it exists.
        /// </summary>
        /// <typeparam name="T">The type of the items in the collection.</typeparam>
        /// <param name="source">The source collection.</param>
        /// <param name="selector">The predicate.</param>
        /// <param name="result">The single item.</param>
        /// <returns>True if the collection contains exactly one non null item.</returns>
        internal static bool TryFirst<T>(this IEnumerable<T> source, Func<T, bool> selector, [MaybeNullWhen(false)]out T result)
        {
            if (source is null)
            {
                result = default!;
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

            result = default!;
            return false;
        }

        /// <summary>
        /// Try getting the first element in <paramref name="source"/>.
        /// </summary>
        /// <typeparam name="T">The type of the elements in <paramref name="source"/>.</typeparam>
        /// <param name="source">The source collection, can be null.</param>
        /// <param name="result">The first element, can be null.</param>
        /// <returns>True if an element was found.</returns>
        internal static bool TryLast<T>(this IEnumerable<T> source, [MaybeNullWhen(false)]out T result)
        {
            result = default!;
            if (source is null)
            {
                return false;
            }

            using var e = source.GetEnumerator();
            if (!e.MoveNext())
            {
                return false;
            }

            while (e.MoveNext())
            {
                result = e.Current;
            }

            return true;
        }

        /// <summary>
        /// Try getting the first element in <paramref name="source"/>.
        /// </summary>
        /// <typeparam name="T">The type of the elements in <paramref name="source"/>.</typeparam>
        /// <param name="source">The source collection, can be null.</param>
        /// <param name="predicate">The filter.</param>
        /// <param name="result">The first element, can be null.</param>
        /// <returns>True if an element was found.</returns>
        internal static bool TryLast<T>(this IEnumerable<T> source, Func<T, bool> predicate, [MaybeNullWhen(false)]out T result)
        {
            result = default!;
            if (source is null)
            {
                return false;
            }

            using var e = source.GetEnumerator();
            if (!e.MoveNext())
            {
                return false;
            }

            var found = false;
            do
            {
                if (e.Current is { } item &&
                    predicate(item))
                {
                    result = item;
                    found = true;
                }
            }
            while (e.MoveNext());
            return found;
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
        internal static TSource MinBy<TSource, TKey>(this IEnumerable<TSource> source, Func<TSource, TKey> selector, IComparer<TKey>? comparer)
        {
            if (source is null)
            {
                throw new ArgumentNullException(nameof(source));
            }

            if (selector is null)
            {
                throw new ArgumentNullException(nameof(selector));
            }

            comparer ??= Comparer<TKey>.Default;
            using var sourceIterator = source.GetEnumerator();
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

        /// <summary>
        /// Return the item with minimum keyed by <paramref name="selector"/>.
        /// </summary>
        /// <typeparam name="TSource">The type of the items in the collection.</typeparam>
        /// <typeparam name="TKey">The type of the value to compare.</typeparam>
        /// <param name="source">The source collection.</param>
        /// <param name="selector">The single item.</param>
        /// <returns>The item with minimum keyed by <paramref name="selector"/>.</returns>
        internal static TSource MaxBy<TSource, TKey>(this IEnumerable<TSource> source, Func<TSource, TKey> selector)
        {
            return source.MaxBy(selector, null);
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
        internal static TSource MaxBy<TSource, TKey>(this IEnumerable<TSource> source, Func<TSource, TKey> selector, IComparer<TKey>? comparer)
        {
            if (source is null)
            {
                throw new ArgumentNullException(nameof(source));
            }

            if (selector is null)
            {
                throw new ArgumentNullException(nameof(selector));
            }

            comparer ??= Comparer<TKey>.Default;
            using var sourceIterator = source.GetEnumerator();
            if (!sourceIterator.MoveNext())
            {
                throw new InvalidOperationException("Sequence contains no elements");
            }

            var max = sourceIterator.Current;
            var maxKey = selector(max);
            while (sourceIterator.MoveNext())
            {
                var candidate = sourceIterator.Current;
                var candidateKey = selector(candidate);
                if (comparer.Compare(candidateKey, maxKey) > 0)
                {
                    max = candidate;
                    maxKey = candidateKey;
                }
            }

            return max;
        }
    }
}
