namespace Gu.Roslyn.Asserts.Internals
{
    using System;
    using System.Collections.Concurrent;
    using System.Text;

    internal static class StringBuilderPool
    {
        private static readonly ConcurrentQueue<PooledStringBuilder> Cache = new();

        internal static PooledStringBuilder Borrow()
        {
            if (Cache.TryDequeue(out var item))
            {
                return item;
            }

            return new PooledStringBuilder();
        }

        internal static string Return(this PooledStringBuilder stringBuilder)
        {
            var text = stringBuilder.GetTextAndClear();
            Cache.Enqueue(stringBuilder);
            return text;
        }

        internal class PooledStringBuilder
        {
            private readonly StringBuilder inner = new();

            internal bool IsEmpty => this.inner.Length == 0;

            internal int Length => this.inner.Length;

#pragma warning disable CS0809 // Obsolete member overrides non-obsolete member
            [Obsolete("Use StringBuilderPool.Return", true)]
            public override string ToString() => throw new InvalidOperationException("Use StringBuilderPool.Return");
#pragma warning restore CS0809 // Obsolete member overrides non-obsolete member

            internal PooledStringBuilder AppendLine(string text)
            {
                this.inner.AppendLine(text);
                return this;
            }

            internal PooledStringBuilder AppendLine()
            {
                this.inner.AppendLine();
                return this;
            }

            internal PooledStringBuilder Append(string value)
            {
                this.inner.Append(value);
                return this;
            }

            internal PooledStringBuilder Insert(int index, string value)
            {
                this.inner.Insert(index, value);
                return this;
            }

            internal PooledStringBuilder Insert(int index, char value)
            {
                this.inner.Insert(index, value);
                return this;
            }

            internal PooledStringBuilder Append(char value)
            {
                this.inner.Append(value);
                return this;
            }

            internal string Return()
            {
                return StringBuilderPool.Return(this);
            }

            internal string GetTextAndClear()
            {
                var text = this.inner.ToString();
                this.inner.Clear();
                return text;
            }
        }
    }
}
