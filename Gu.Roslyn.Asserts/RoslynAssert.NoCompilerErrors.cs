namespace Gu.Roslyn.Asserts
{
    using System.Collections.Generic;
    using Microsoft.CodeAnalysis;

    public static partial class RoslynAssert
    {
        /// <summary>
        /// Check that the solution has no compiler errors and warnings.
        /// </summary>
        /// <param name="code">The code to analyze.</param>
        public static void NoCompilerErrors(params string[] code)
        {
            var solution = CodeFactory.CreateSolution(code, Settings.Default);
            NoDiagnostics(Analyze.GetDiagnostics(solution));
        }

        /// <summary>
        /// Check that the solution has no compiler errors and warnings.
        /// </summary>
        /// <param name="settings">The <see cref="Settings"/>.</param>
        /// <param name="code">The code to analyze.</param>
        public static void NoCompilerErrors(string code, Settings? settings = null)
        {
            var solution = CodeFactory.CreateSolution(code, settings ?? Settings.Default);
            NoDiagnostics(Analyze.GetDiagnostics(solution));
        }

        /// <summary>
        /// Check that the solution has no compiler errors and warnings.
        /// </summary>
        /// <param name="settings">The <see cref="Settings"/>.</param>
        /// <param name="code">The code to analyze.</param>
        public static void NoCompilerErrors(IEnumerable<string> code, Settings? settings = null)
        {
            var solution = CodeFactory.CreateSolution(code, settings ?? Settings.Default);
            NoDiagnostics(Analyze.GetDiagnostics(solution));
        }

        /// <summary>
        /// Check that the solution has no compiler errors and warnings.
        /// </summary>
        public static void NoCompilerErrors(Solution solution)
        {
            NoDiagnostics(Analyze.GetDiagnostics(solution));
        }
    }
}
