namespace Gu.Roslyn.Asserts.Internals
{
    using System.Text;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.Text;

    /// <summary>
    /// A <see cref="TextLoader"/> for documents passed as strings
    /// </summary>
    internal class StringLoader : TextLoader
    {
        private readonly Task<TextAndVersion> textAndVersion;

        /// <summary>
        /// Initializes a new instance of the <see cref="StringLoader"/> class.
        /// </summary>
        /// <param name="document">The code of the document.</param>
        internal StringLoader(string document)
        {
            this.Code = document;
            this.textAndVersion = Task.FromResult(
                TextAndVersion.Create(
                    SourceText.From(document, (Encoding)null, SourceHashAlgorithm.Sha1),
                    VersionStamp.Default));
        }

        /// <summary>
        /// Gets the code in the document.
        /// </summary>
        public string Code { get; }

        /// <inheritdoc />
        public override Task<TextAndVersion> LoadTextAndVersionAsync(Workspace workspace, DocumentId documentId, CancellationToken cancellationToken) => this.textAndVersion;

        internal static StringLoader Create(string document) => new StringLoader(document);
    }
}
