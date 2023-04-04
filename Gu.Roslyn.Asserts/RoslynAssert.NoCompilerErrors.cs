namespace Gu.Roslyn.Asserts;

using System.Collections.Generic;
using Microsoft.CodeAnalysis;

public static partial class RoslynAssert
{
    /// <summary>
    /// Check that the solution has no compiler errors and warnings.
    /// </summary>
    /// <param name="code">The code to analyze.</param>
    public static void NoCompilerDiagnostics(params string[] code)
    {
        var settings = Settings.Default;
        var solution = CodeFactory.CreateSolution(code, settings);
        NoDiagnostics(Analyze.GetAllDiagnostics(solution));
    }

    /// <summary>
    /// Check that the solution has no compiler errors and warnings.
    /// </summary>
    /// <param name="settings">The <see cref="Settings"/>.</param>
    /// <param name="code">The code to analyze.</param>
    public static void NoCompilerDiagnostics(string code, Settings? settings = null)
    {
        var solution = CodeFactory.CreateSolution(code, settings ?? Settings.Default);
        NoDiagnostics(Analyze.GetAllDiagnostics(solution));
    }

    /// <summary>
    /// Check that the solution has no compiler errors and warnings.
    /// </summary>
    /// <param name="settings">The <see cref="Settings"/>.</param>
    /// <param name="code">The code to analyze.</param>
    public static void NoCompilerDiagnostics(IEnumerable<string> code, Settings? settings = null)
    {
        var solution = CodeFactory.CreateSolution(code, settings ?? Settings.Default);
        NoDiagnostics(Analyze.GetAllDiagnostics(solution));
    }

    /// <summary>
    /// Check that the solution has no compiler errors and warnings.
    /// </summary>
    public static void NoCompilerDiagnostics(Solution solution)
    {
        NoDiagnostics(Analyze.GetAllDiagnostics(solution));
    }
}
