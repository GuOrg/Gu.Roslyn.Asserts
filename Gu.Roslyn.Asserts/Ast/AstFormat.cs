namespace Gu.Roslyn.Asserts
{
    /// <summary>
    /// For configuring output from <see cref="AstWriter"/>.
    /// </summary>
    public enum AstFormat
    {
        /// <summary>
        /// Custom format with less noise than JSON.
        /// </summary>
        Light,

        /// <summary>
        /// JSON.
        /// </summary>
        Json,
    }
}
