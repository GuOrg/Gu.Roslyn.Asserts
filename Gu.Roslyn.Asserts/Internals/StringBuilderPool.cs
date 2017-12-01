#pragma warning disable SA1600 // Elements must be documented
namespace Gu.Roslyn.Asserts.Internals
{
    using System;
    using System.Collections.Concurrent;
    using System.Text;

    internal static class StringBuilderPool
    {
        private static readonly ConcurrentQueue<PooledStringBuilder> Cache = new ConcurrentQueue<PooledStringBuilder>();

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
            private readonly StringBuilder inner = new StringBuilder();

            public bool IsEmpty => this.inner.Length == 0;

            public int Length => this.inner.Length;

            public void Clear() => this.inner.Clear();

            public PooledStringBuilder AppendLine(string text)
            {
                this.inner.AppendLine(text);
                return this;
            }

            public PooledStringBuilder AppendLine()
            {
                this.inner.AppendLine();
                return this;
            }

            public PooledStringBuilder Append(string value)
            {
                this.inner.Append(value);
                return this;
            }

            public PooledStringBuilder Append(char value)
            {
                this.inner.Append(value);
                return this;
            }

            public string Return()
            {
                return StringBuilderPool.Return(this);
            }

            [Obsolete("Use StringBuilderPool.Return", true)]
#pragma warning disable CS0809 // Obsolete member overrides non-obsolete member
            public override string ToString() => throw new InvalidOperationException("Use StringBuilderPool.Return");
#pragma warning restore CS0809 // Obsolete member overrides non-obsolete member

            public string GetTextAndClear()
            {
                var text = this.inner.ToString();
                this.inner.Clear();
                return text;
            }
        }
    }
}
