namespace Gu.Roslyn.Asserts
{
    using System;

    [Flags]
    public enum AstTrivia
    {
        /// <summary>
        /// Caller did not specify.
        /// </summary>
        Unspecified = 0,

        /// <summary>
        /// Include trivia for tokens only.
        /// </summary>
        Token = 1 << 0,

        /// <summary>
        /// Include trivia for nodes.
        /// </summary>
        Node = 1 << 1,

        /// <summary>
        /// Don't include trivia.
        /// </summary>
        Ignore = 1 << 2,
    }
}
