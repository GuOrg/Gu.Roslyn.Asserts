namespace Gu.Roslyn.Asserts
{
    /// <summary>
    /// Specifies if a code fix is allowed to introduce compiler errors.
    /// </summary>
    public enum AllowCompilationDiagnostics
    {
        /// <summary>
        /// Errors and warnings not allowed.
        /// </summary>
        None,

        /// <summary>
        /// Errors allowed.
        /// </summary>
        Yes,
    }
}
