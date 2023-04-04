namespace Gu.Roslyn.Asserts.Tests;

using System.Linq;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using NUnit.Framework;

public static class AnalyzeTests
{
    [Test]
    public static void GetDiagnosticsProjectFile()
    {
        Assert.AreEqual(true, ProjectFile.TryFind("Gu.Roslyn.Asserts.csproj", out var projectFile));
        var diagnostics = Analyze.GetDiagnostics(new FieldNameMustNotBeginWithUnderscore(), projectFile, Settings.Default);
        CollectionAssert.IsEmpty(diagnostics.SelectMany(x => x.AnalyzerDiagnostics));
    }

    [Test]
    public static void GetDiagnosticsSolutionFile()
    {
        var solutionFile = SolutionFile.Find("Gu.Roslyn.Asserts.sln");
        var diagnostics = Analyze.GetDiagnostics(new FieldNameMustNotBeginWithUnderscore(), solutionFile, Settings.Default);
        var expected = new[]
        {
            "ClassLibrary1Class1.cs(6,21): warning SA1309: Field '_value' must not begin with an underscore",
            "ClassLibrary2Class1.cs(6,21): warning SA1309: Field '_value' must not begin with an underscore",
        };
        CollectionAssert.AreEquivalent(expected, diagnostics.SelectMany(x => x.AnalyzerDiagnostics).Select(SkipDirectory));
    }

    [Test]
    public static async Task GetDiagnosticsAsyncSolution()
    {
        var solutionFile = SolutionFile.Find("Gu.Roslyn.Asserts.sln");
        var expected = new[]
        {
            "ClassLibrary1Class1.cs(6,21): warning SA1309: Field '_value' must not begin with an underscore",
            "ClassLibrary2Class1.cs(6,21): warning SA1309: Field '_value' must not begin with an underscore",
        };

        var sln = CodeFactory.CreateSolution(solutionFile);
        var analyzer = new FieldNameMustNotBeginWithUnderscore();
        var diagnostics = await Analyze.GetDiagnosticsAsync(analyzer, sln).ConfigureAwait(false);
        CollectionAssert.AreEquivalent(expected, diagnostics.SelectMany(x => x.AnalyzerDiagnostics).Select(SkipDirectory));

        diagnostics = Analyze.GetDiagnostics(analyzer, sln);
        CollectionAssert.AreEquivalent(expected, diagnostics.SelectMany(x => x.AnalyzerDiagnostics).Select(SkipDirectory));
    }

    private static string SkipDirectory(Diagnostic diagnostic)
    {
        var text = diagnostic.ToString();
        return text[(text.LastIndexOf('\\') + 1)..];
    }
}
