namespace Gu.Roslyn.Asserts.Internals
{
    using System.Text;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.Text;

    internal class StringLoader : TextLoader
    {
        private readonly Task<TextAndVersion> textAndVersion;

        internal StringLoader(string document)
        {
            this.Code = document;
            this.textAndVersion = Task.FromResult(
                TextAndVersion.Create(
                    SourceText.From(document, (Encoding)null, SourceHashAlgorithm.Sha1),
                    VersionStamp.Default));
        }

        public string Code { get; }

        public override Task<TextAndVersion> LoadTextAndVersionAsync(Workspace workspace, DocumentId documentId, CancellationToken cancellationToken)
        {
            return this.textAndVersion;
        }

        internal static StringLoader Create(string document) => new StringLoader(document);
    }
}
