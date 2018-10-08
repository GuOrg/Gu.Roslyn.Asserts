namespace Gu.Roslyn.Asserts.Internals
{
    using System.Collections.Generic;
    using System.Linq;
    using Microsoft.CodeAnalysis;

    internal static partial class EnumerableExt
    {
        /// <summary>
        /// A hack implementation for comparing if <paramref name="expecteds"/> matches <paramref name="actuals"/>
        /// Performance should not be an issue as we only expect small collections, optimize if needed.
        /// </summary>
        /// <param name="expecteds">The expected diagnostics.</param>
        /// <param name="actuals">The actual diagnostics.</param>
        /// <returns>True if expected matches actuals.</returns>
        internal static bool SetEquals(this IReadOnlyList<ExpectedDiagnostic> expecteds, IReadOnlyList<Diagnostic> actuals)
        {
            foreach (var actual in actuals)
            {
                if (expecteds.Any(e => e.Matches(actual)))
                {
                    continue;
                }

                return false;
            }

            foreach (var expected in expecteds)
            {
                if (actuals.Any(a => expected.Matches(a)))
                {
                    continue;
                }

                return false;
            }

            return true;
        }

        /// <summary>
        /// A hack implementation for getting the elements in <paramref name="expecteds"/> that are not in <paramref name="actuals"/>.
        /// </summary>
        /// <param name="expecteds">The expected diagnostics.</param>
        /// <param name="actuals">The actual diagnostics.</param>
        /// <returns>True if expected matches actuals.</returns>
        internal static IReadOnlyList<ExpectedDiagnostic> Except(this IReadOnlyList<ExpectedDiagnostic> expecteds, IReadOnlyList<Diagnostic> actuals)
        {
            return expecteds.Where(e => actuals.All(a => !e.Matches(a))).ToArray();
        }

        /// <summary>
        /// A hack implementation for getting the elements in <paramref name="actuals"/> that are not in <paramref name="expecteds"/>.
        /// </summary>
        /// <param name="actuals">The actual diagnostics.</param>
        /// <param name="expecteds">The expected diagnostics.</param>
        /// <returns>True if expected matches actuals.</returns>
        internal static IReadOnlyList<Diagnostic> Except(this IReadOnlyList<Diagnostic> actuals, IReadOnlyList<ExpectedDiagnostic> expecteds)
        {
            return actuals.Where(a => expecteds.All(e => !e.Matches(a))).ToArray();
        }
    }
}