namespace Gu.Roslyn.Asserts
{
    using System;

    public static class StringAssert
    {
        public static string AssertReplace(this string text, string oldValue, string newValue)
        {
            if (!text.Contains(oldValue))
            {
                throw new InvalidOperationException($"AssertReplace failed, expected {oldValue} to be in {text}");
            }

            return text.Replace(oldValue, newValue);
        }
    }
}