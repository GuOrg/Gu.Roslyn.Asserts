namespace Gu.Roslyn.Asserts
{
    using System;
    using Microsoft.CodeAnalysis;

    [Obsolete("Use RoslynAssert")]
    public static partial class AnalyzerAssert
    {
        /// <summary>
        /// Serializes the syntax tree and compares the strings.
        /// This can be useful when having trouble getting whitespace right.
        /// </summary>
        /// <typeparam name="T">The node type.</typeparam>
        /// <param name="expected">The expected shape of the AST.</param>
        /// <param name="actual">The actual node.</param>
        /// <param name="settings"><see cref="AstWriterSettings"/>.</param>
        public static void Ast<T>(T expected, T actual, AstWriterSettings settings = null)
            where T : SyntaxNode
        {
            CodeAssert.AreEqual(AstWriter.Serialize(expected, settings), AstWriter.Serialize(actual, settings));
        }
    }
}
