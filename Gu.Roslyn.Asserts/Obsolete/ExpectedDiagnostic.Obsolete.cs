namespace Gu.Roslyn.Asserts;

using System.Collections.Generic;

public partial class ExpectedDiagnostic
{
    /// <summary>
    /// Create a new instance of <see cref="ExpectedDiagnostic"/>.
    /// </summary>
    /// <param name="diagnosticId">The expected diagnostic id.</param>
    /// <param name="code">The code with error position indicated..</param>
    /// <param name="cleanedSources"><paramref name="code"/> without error indicator.</param>
    /// <returns>A new instance of <see cref="ExpectedDiagnostic"/>.</returns>
    public static ExpectedDiagnostic CreateFromCodeWithErrorsIndicated(string diagnosticId, string code, out string cleanedSources) => FromMarkup(diagnosticId, code, out cleanedSources);

    /// <summary>
    /// Create a new instance of <see cref="ExpectedDiagnostic"/>.
    /// </summary>
    /// <param name="diagnosticId">The expected diagnostic id.</param>
    /// <param name="message">The expected message.</param>
    /// <param name="code">The code with error position indicated..</param>
    /// <param name="cleanedSources"><paramref name="code"/> without error indicator.</param>
    /// <returns>A new instance of <see cref="ExpectedDiagnostic"/>.</returns>
    public static ExpectedDiagnostic CreateFromCodeWithErrorsIndicated(string diagnosticId, string? message, string code, out string cleanedSources) => FromMarkup(diagnosticId, message, code, out cleanedSources);

    /// <summary>
    /// Create a new instance of <see cref="ExpectedDiagnostic"/>.
    /// </summary>
    /// <param name="diagnosticId">The expected diagnostic id.</param>
    /// <param name="message">The expected message.</param>
    /// <param name="codeWithErrorsIndicated">The code with error position indicated..</param>
    /// <param name="cleanedSources"><paramref name="codeWithErrorsIndicated"/> without errors indicated.</param>
    /// <returns>A new instance of <see cref="ExpectedDiagnostic"/>.</returns>
    public static IReadOnlyList<ExpectedDiagnostic> CreateManyFromCodeWithErrorsIndicated(string diagnosticId, string message, string codeWithErrorsIndicated, out string cleanedSources) => ManyFromMarkup(diagnosticId, message, codeWithErrorsIndicated, out cleanedSources);
}
