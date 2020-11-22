namespace Gu.Roslyn.Asserts
{
    /// <summary>
    /// Specifies if a code fix is allowed to introduce compiler errors.
    /// </summary>
    public enum AllowCompilationErrors
    {
        /// <summary>
        /// Errors not allowed.
        /// </summary>
        No,

        /// <summary>
        /// Errors allowed.
        /// </summary>
        Yes,
    }
}
