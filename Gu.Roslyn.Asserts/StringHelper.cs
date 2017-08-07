namespace Gu.Roslyn.Asserts
{
    using Gu.Roslyn.Asserts.Internals;

    /// <summary>
    /// Helper with methods for manipulating strings.
    /// </summary>
    public static class StringHelper
    {
        /// <summary>
        /// Call string.Replace(<paramref name="oldValue"/>, <paramref name="newValue"/>) but check that string contains oldvalue first.
        /// </summary>
        /// <param name="text">The text.</param>
        /// <param name="oldValue">The text to replace.</param>
        /// <param name="newValue">The text to replace with.</param>
        /// <returns>The sting with the replaced text.</returns>
        public static string AssertReplace(this string text, string oldValue, string newValue)
        {
            if (!text.Contains(oldValue))
            {
                throw Fail.CreateException($"AssertReplace failed, expected {oldValue} to be in {text}");
            }

            return text.Replace(oldValue, newValue);
        }
    }
}