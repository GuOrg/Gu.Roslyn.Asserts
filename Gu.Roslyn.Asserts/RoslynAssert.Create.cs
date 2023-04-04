namespace Gu.Roslyn.Asserts;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Diagnostics;

public static partial class RoslynAssert
{
    /// <summary>
    /// Obtains an instance which provides assertions against the specified diagnostic analyzer type.
    /// </summary>
    /// <typeparam name="TDiagnosticAnalyzer">The <see cref="DiagnosticAnalyzer"/> to use in asserts.</typeparam>
    /// <param name="descriptor">
    /// The <see cref="DiagnosticDescriptor"/> with information about the expected <see cref="Diagnostic"/>. If
    /// <typeparamref name="TDiagnosticAnalyzer"/> supports more than one <see cref="DiagnosticDescriptor.Id"/>,
    /// this must be provided.
    /// </param>
    public static DiagnosticAssert Create<TDiagnosticAnalyzer>(
        DiagnosticDescriptor? descriptor = null)
        where TDiagnosticAnalyzer : DiagnosticAnalyzer, new()
    {
        return new DiagnosticAssert(() => new TDiagnosticAnalyzer(), descriptor?.Id);
    }

    /// <summary>
    /// Obtains an instance which provides assertions against the specified diagnostic analyzer and code fix
    /// provider types.
    /// </summary>
    /// <typeparam name="TDiagnosticAnalyzer">The <see cref="DiagnosticAnalyzer"/> to use in asserts.</typeparam>
    /// <typeparam name="TCodeFixProvider">The <see cref="CodeFixProvider"/> to use in asserts.</typeparam>
    /// <param name="expectedDiagnostic">
    /// The <see cref="ExpectedDiagnostic"/> with information about the expected <see cref="Diagnostic"/>. If
    /// <typeparamref name="TDiagnosticAnalyzer"/> supports more than one <see cref="DiagnosticDescriptor.Id"/>,
    /// this must be provided.
    /// </param>
    public static DiagnosticFixAssert Create<TDiagnosticAnalyzer, TCodeFixProvider>(
        ExpectedDiagnostic? expectedDiagnostic = null)
        where TDiagnosticAnalyzer : DiagnosticAnalyzer, new()
        where TCodeFixProvider : CodeFixProvider, new()
    {
        return new DiagnosticFixAssert(() => new TDiagnosticAnalyzer(), () => new TCodeFixProvider(), expectedDiagnostic);
    }

    /// <summary>
    /// Obtains an instance which provides assertions against the specified code fix provider type.
    /// </summary>
    /// <typeparam name="TCodeFixProvider">The <see cref="CodeFixProvider"/> to use in asserts.</typeparam>
    /// <param name="expectedDiagnostic">
    /// The <see cref="ExpectedDiagnostic"/> with information about the expected <see cref="Diagnostic"/>.
    /// </param>
    public static FixAssert CreateWithoutAnalyzer<TCodeFixProvider>(ExpectedDiagnostic expectedDiagnostic)
        where TCodeFixProvider : CodeFixProvider, new()
    {
        return new FixAssert(() => new TCodeFixProvider(), expectedDiagnostic);
    }
}
