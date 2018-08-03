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
        public static readonly AstWriterSettings Everything = new AstWriterSettings(json: false);

        /// <summary>
        /// For dumping all the things in JSON format.
        /// </summary>
        public static readonly AstWriterSettings EverythingJson = new AstWriterSettings(json: true);

        /// <summary>
        /// Initializes a new instance of the <see cref="AstWriterSettings"/> class.
        /// </summary>
        /// <param name="json">If the dump should be formatted as JSON.</param>
        public AstWriterSettings(bool json)
        {
            this.Json = json;
        }

        /// <summary>
        /// Gets a value indicating whether the dump should be formatted as JSON.
        /// </summary>
        public bool Json { get; }
    }
}
