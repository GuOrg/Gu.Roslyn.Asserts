namespace Gu.Roslyn.Asserts
{
    using System;

    public class AssertException : Exception
    {
        public AssertException(string message)
            : base(message)
        {
        }

        public AssertException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        public override string ToString() => base.Message;
    }
}