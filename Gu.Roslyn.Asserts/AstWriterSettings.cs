namespace Gu.Roslyn.Asserts
{
    /// <summary>
    /// Settings for how <see cref="AstWriter"/> serializes the tree.
    /// </summary>
    public class AstWriterSettings
    {
        /// <summary>
        /// For dumping all the things in light format.
        /// </summary>
        public static readonly AstWriterSettings Everything = new AstWriterSettings(json: false, writeEmptyTrivia: true);

        /// <summary>
        /// For dumping all the things in JSON format.
        /// </summary>
        public static readonly AstWriterSettings EverythingJson = new AstWriterSettings(json: true, writeEmptyTrivia: true);

        /// <summary>
        /// Initializes a new instance of the <see cref="AstWriterSettings"/> class.
        /// </summary>
        /// <param name="json">If the dump should be formatted as JSON.</param>
        /// <param name="writeEmptyTrivia">If empty trivia should be included.</param>
        public AstWriterSettings(bool json, bool writeEmptyTrivia)
        {
            this.Json = json;
            this.WriteEmptyTrivia = writeEmptyTrivia;
        }

        /// <summary>
        /// Gets a value indicating whether the dump should be formatted as JSON.
        /// </summary>
        public bool Json { get; }

        /// <summary>
        /// Gets a value indicating whether empty trivia should be included.
        /// </summary>
        public bool WriteEmptyTrivia { get; }
    }
}
