namespace Gu.Roslyn.Asserts
{
    /// <summary>
    /// Settings for how <see cref="AstWriter"/> serializes the tree.
    /// </summary>
    public sealed class AstWriterSettings
    {
        /// <summary>
        /// For dumping all the things in light format.
        /// </summary>
        public static readonly AstWriterSettings Default = new AstWriterSettings(AstFormat.Light, AstTrivia.Token, ignoreEmptyTriva: false);

        /// <summary>
        /// For dumping all the things in JSON format.
        /// </summary>
        public static readonly AstWriterSettings DefaultJson = new AstWriterSettings(AstFormat.Json, AstTrivia.Token, ignoreEmptyTriva: false);

        /// <summary>
        /// Initializes a new instance of the <see cref="AstWriterSettings"/> class.
        /// </summary>
        /// <param name="format">Specifies the format of the dump.</param>
        /// <param name="trivia">Specifies what trivia to include.</param>
        /// <param name="ignoreEmptyTriva">Specifies if empty whitespace trivia should be ignored.</param>
        public AstWriterSettings(AstFormat format, AstTrivia trivia, bool ignoreEmptyTriva)
        {
            this.Format = format;
            this.Trivia = trivia;
            this.IgnoreEmptyTriva = ignoreEmptyTriva;
        }

        /// <summary>
        /// Gets a value indicating the format of the dump <see cref="AstFormat"/>.
        /// </summary>
        public AstFormat Format { get; }

        /// <summary>
        /// Gets a value indicating what trivia to include <see cref="AstTrivia"/>.
        /// </summary>
        public AstTrivia Trivia { get; }

        /// <summary>
        /// Gets a value indicating whether empty whitespace trivia should be ignored.
        /// SyntaxFactory.Whitespace(string.Empty).
        /// </summary>
        public bool IgnoreEmptyTriva { get; }
    }
}
