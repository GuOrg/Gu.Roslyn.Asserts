namespace Gu.Roslyn.Asserts
{
    public class AstWriterSettings
    {
        public static readonly AstWriterSettings EveryThing = new AstWriterSettings(writeNodeText: true, writeEmptyTrivia: true);

        public AstWriterSettings(bool writeNodeText, bool writeEmptyTrivia)
        {
            this.WriteNodeText = writeNodeText;
            this.WriteEmptyTrivia = writeEmptyTrivia;
        }

        public bool WriteNodeText { get; }

        public bool WriteEmptyTrivia { get; }
    }
}
