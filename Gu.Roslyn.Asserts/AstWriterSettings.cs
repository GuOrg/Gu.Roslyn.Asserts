namespace Gu.Roslyn.Asserts
{
    public struct AstWriterSettings
    {
        public static readonly AstWriterSettings EveryThing = new AstWriterSettings(true, true);

        public AstWriterSettings(bool writeNodeText, bool writeEmptyTrivia)
        {
            this.WriteNodeText = writeNodeText;
            this.WriteEmptyTrivia = writeEmptyTrivia;
        }

        public bool WriteNodeText { get; }

        public bool WriteEmptyTrivia { get; }
    }
}