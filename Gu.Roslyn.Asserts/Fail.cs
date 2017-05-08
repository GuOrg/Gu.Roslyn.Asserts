namespace Gu.Roslyn.Asserts
{
    using System;

    internal static class Fail
    {
        private static Action<string> action;

        public static void WithMessage(string message)
        {
            if (action == null)
            {
                var type = Type.GetType(
                    "NUnit.Framework.AssertionException, nunit.framework, Version=3.6.1.0, Culture=neutral, PublicKeyToken=2638cd05610744eb",
                    false);
                if (type == null)
                {
                    action = text => throw new AssertException(text);
                }
                else
                {
                    action = text => throw (Exception)Activator.CreateInstance(type, text);
                }
            }

            action(message);
        }
    }
}
