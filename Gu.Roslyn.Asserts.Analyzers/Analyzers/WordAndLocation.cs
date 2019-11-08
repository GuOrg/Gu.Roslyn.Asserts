namespace Gu.Roslyn.Asserts.Analyzers
{
    using Microsoft.CodeAnalysis;

    internal struct WordAndLocation
    {
        internal readonly string Word;
        internal readonly Location Location;

        internal WordAndLocation(string before, Location location)
        {
            this.Word = before;
            this.Location = location;
        }
    }
}
