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
        /// Compiler warnings are allowed in the code.
        /// </summary>
        Warnings,

        /// <summary>
        /// Compiler warnings and errors are allowed in the code.
        /// </summary>
        WarningsAndErrors,
    }
}
