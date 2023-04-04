namespace Gu.Roslyn.Asserts.Analyzers;

using Gu.Roslyn.AnalyzerExtensions;

internal static class KnownSymbols
{
    internal static readonly QualifiedType Object = Create("System.Object", "object");
    internal static readonly QualifiedType RoslynAssert = Create("Gu.Roslyn.Asserts.RoslynAssert");
    internal static readonly QualifiedType DiagnosticAnalyzer = Create("Microsoft.CodeAnalysis.Diagnostics.DiagnosticAnalyzer");
    internal static readonly QualifiedType DiagnosticDescriptor = Create("Microsoft.CodeAnalysis.DiagnosticDescriptor");
    internal static readonly QualifiedType CodeFixProvider = Create("Microsoft.CodeAnalysis.CodeFixes.CodeFixProvider");
    internal static readonly QualifiedType CodeRefactoringProvider = Create("Microsoft.CodeAnalysis.CodeRefactorings.CodeRefactoringProvider");
    internal static readonly QualifiedType NUnitTestAttribute = Create("NUnit.Framework.TestAttribute");
    internal static readonly QualifiedType NUnitTestCaseAttribute = Create("NUnit.Framework.TestCaseAttribute");

    private static QualifiedType Create(string qualifiedName, string? alias = null)
    {
        return new QualifiedType(qualifiedName, alias);
    }
}
