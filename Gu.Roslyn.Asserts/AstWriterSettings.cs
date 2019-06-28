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
        public static readonly AstWriterSettings Default = new AstWriterSettings(AstFormat.Light);

        /// <summary>
        /// For dumping all the things in JSON format.
        /// </summary>
        public static readonly AstWriterSettings DefaultJson = new AstWriterSettings(AstFormat.Json);

        /// <summary>
        /// Initializes a new instance of the <see cref="AstWriterSettings"/> class.
        /// </summary>
        /// <param name="json">If the dump should be formatted as JSON.</param>
        public AstWriterSettings(AstFormat format)
        {
            this.Format = format;
        }

        /// <summary>
        /// Gets a value indicating whether the dump should be formatted as JSON.
        /// </summary>
        public AstFormat Format { get; }
    }
}
