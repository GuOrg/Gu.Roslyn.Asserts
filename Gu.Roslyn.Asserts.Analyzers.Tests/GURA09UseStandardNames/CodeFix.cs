namespace Gu.Roslyn.Asserts.Analyzers.Tests.GURA09UseStandardNames;

public static partial class CodeFix
{
    private static readonly DiagnosticFixAssert Assert = RoslynAssert.Create<InvocationAnalyzer, StandardNamesFix>(
        ExpectedDiagnostic.Create(Descriptors.GURA09UseStandardNames));
}
