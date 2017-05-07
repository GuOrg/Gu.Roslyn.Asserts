namespace Gu.Roslyn.Asserts
{
    public static class StringHelper
    {
        public static string AssertReplace(this string text, string oldValue, string newValue)
        {
            if (!text.Contains(oldValue))
            {
                throw new AssertException($"AssertReplace failed, expected {oldValue} to be in {text}");
            }

            return text.Replace(oldValue, newValue);
        }
    }
}