namespace Gu.Roslyn.Asserts
{
    using Microsoft.CodeAnalysis;

    public static partial class RoslynAssert
    {
        /// <summary>
        /// Serializes the syntax tree and compares the strings.
        /// This can be useful when having trouble getting whitespace right.
        /// </summary>
        /// <typeparam name="T">The node type.</typeparam>
        /// <param name="expected">The expected shape of the AST.</param>
        /// <param name="actual">The actual node.</param>
        public static void Ast<T>(T expected, T actual)
            where T : SyntaxNode
        {
            CodeAssert.AreEqual(SyntaxFactoryWriter.Serialize(expected), SyntaxFactoryWriter.Serialize(actual));
        }
    }
}
