namespace Gu.Roslyn.Asserts.Tests.RoslynAssertTests;

using NUnit.Framework;

public static class NoAnalyzerDiagnostics
{
    public static class Fail
    {
        [Test]
        public static void SingleDocumentFieldNameMustNotBeginWithUnderscore()
        {
            var code = @"
namespace N
{
    class C
    {
        private readonly int _value = 1;
    }
}";
            var expected = "Expected no diagnostics, found:\r\n" +
                                 "SA1309 Field '_value' must not begin with an underscore\r\n" +
                                 "  at line 5 and character 29 in file C.cs | private readonly int ↓_value = 1;\r\n";

            var exception = Assert.Throws<AssertException>(() =>
                RoslynAssert.NoAnalyzerDiagnostics(new FieldNameMustNotBeginWithUnderscore(), code));
            Assert.AreEqual(expected, exception.Message);

            exception = Assert.Throws<AssertException>(() =>
                RoslynAssert.NoAnalyzerDiagnostics(typeof(FieldNameMustNotBeginWithUnderscore), code));
            Assert.AreEqual(expected, exception.Message);
        }

        [Test]
        public static void WhenAnalyzerThrows()
        {
            var code = @"namespace N
{
    class C
    {
    }
}";
            var expected = "Expected no diagnostics, found:\r\n" +
                                "AD0001 Analyzer 'Gu.Roslyn.Asserts.Tests.ThrowingAnalyzer' threw an exception of type 'System.InvalidOperationException' with message 'Analyzer threw this.'.\r\n" +
                                "Exception occurred with following context:\r\n" +
                                "Compilation: N\r\n" +
                                "SyntaxTree: C.cs\r\n" +
                                "SyntaxNode: N [IdentifierNameSyntax]@[10..11) (0,10)-(0,11)\r\n" +
                                "\r\n" +
                                "System.InvalidOperationException: Analyzer threw this.\r\n" +
                                "   at Gu.Roslyn.Asserts.Tests.ThrowingAnalyzer.<>c.<Initialize>b__7_0(SyntaxNodeAnalysisContext x) in $(SolutionDir)\\Gu.Roslyn.Asserts.Tests\\TestHelpers\\Analyzers\\ThrowingAnalyzer.cs:line 37\r\n" +
                                "   at Microsoft.CodeAnalysis.Diagnostics.AnalyzerExecutor.<>c__62`1.<ExecuteSyntaxNodeAction>b__62_0(ValueTuple`2 data)\r\n" +
                                "   at Microsoft.CodeAnalysis.Diagnostics.AnalyzerExecutor.ExecuteAndCatchIfThrows_NoLock[TArg](DiagnosticAnalyzer analyzer, Action`1 analyze, TArg argument, Nullable`1 info)\r\n" +
                                "-----\r\n" +
                                "\r\n" +
                                "Suppress the following diagnostics to disable this analyzer: ID1\r\n" +
                                "  at line 0 and character 0 in file  | Code did not have position 0,0\r\n";

            expected = expected.AssertReplace("$(SolutionDir)", SolutionFile.Find("Gu.Roslyn.Asserts.sln").DirectoryName);

            var exception = Assert.Throws<AssertException>(() =>
                RoslynAssert.NoAnalyzerDiagnostics(new ThrowingAnalyzer(), code));
            Assert.AreEqual(expected, exception.Message);

            exception = Assert.Throws<AssertException>(() =>
                RoslynAssert.NoAnalyzerDiagnostics(typeof(ThrowingAnalyzer), code));
            Assert.AreEqual(expected, exception.Message);
        }
    }
}
