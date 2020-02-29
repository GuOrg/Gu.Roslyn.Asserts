namespace Gu.Roslyn.Asserts
{
    using Microsoft.CodeAnalysis;
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
            return new DiagnosticAssert(() => new TDiagnosticAnalyzer(), descriptor);
        }
    }
}
