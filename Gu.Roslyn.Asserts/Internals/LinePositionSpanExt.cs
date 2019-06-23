namespace Gu.Roslyn.Asserts.Internals
{
    using Microsoft.CodeAnalysis.Text;

    /// <summary>
    /// Extension methods for <see cref="LinePositionSpan"/>.
    /// </summary>
    internal static class LinePositionSpanExt
    {
        /// <summary>
        /// Check if <paramref name="text"/> contains <paramref name="span"/>.
        /// </summary>
        /// <param name="span">The <see cref="LinePositionSpan"/>.</param>
        /// <param name="text">The <see cref="string"/>.</param>
        /// <returns>True if <paramref name="text"/> contains <paramref name="span"/>.</returns>
        internal static bool ExistsIn(this LinePositionSpan span, string text)
        {
            return span.Start.TryGetIndex(text, out _) &&
                   span.End.TryGetIndex(text, out _);
        }
    }
}
