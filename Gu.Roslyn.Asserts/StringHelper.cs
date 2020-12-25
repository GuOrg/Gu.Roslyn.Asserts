namespace Gu.Roslyn.Asserts
{
    /// <summary>
    /// Helper with methods for manipulating strings.
    /// </summary>
    public static class StringHelper
    {
        /// <summary>
        /// Call string.Replace(<paramref name="oldValue"/>, <paramref name="newValue"/>) but check that string contains <paramref name="oldValue"/> first.
        /// </summary>
        /// <param name="text">The text.</param>
        /// <param name="oldValue">The text to replace.</param>
        /// <param name="newValue">The text to replace with.</param>
        /// <returns>The sting with the replaced text.</returns>
        public static string AssertReplace(this string text, string oldValue, string newValue)
        {
            if (text is null)
            {
                throw new System.ArgumentNullException(nameof(text));
            }

            if (oldValue is null)
            {
                throw new System.ArgumentNullException(nameof(oldValue));
            }

            if (!text.Contains(oldValue))
            {
                throw new AssertException($"AssertReplace failed, expected {oldValue} to be in {text}");
            }

            return text.Replace(oldValue, newValue);
        }
    }
}
