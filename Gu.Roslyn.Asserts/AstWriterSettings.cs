namespace Gu.Roslyn.Asserts
{
    public class AstWriterSettings
    {
        public static readonly AstWriterSettings Everything = new AstWriterSettings(json: false, writeEmptyTrivia: true);

        public static readonly AstWriterSettings EverythingJson = new AstWriterSettings(json: false, writeEmptyTrivia: true);

        public AstWriterSettings(bool json, bool writeEmptyTrivia)
        {
            this.Json = json;
            this.WriteEmptyTrivia = writeEmptyTrivia;
        }

        /// <summary>
        /// If the dump should be formatted as JSON.
        /// </summary>
        public bool Json { get; }

        /// <summary>
        /// If empty trivia should be included.
        /// </summary>
        public bool WriteEmptyTrivia { get; }
    }
}
