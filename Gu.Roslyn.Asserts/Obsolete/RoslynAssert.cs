namespace Gu.Roslyn.Asserts
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;

    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CodeFixes;
    using Microsoft.CodeAnalysis.Diagnostics;

    public static partial class RoslynAssert
    {
        /// <summary>
        /// The metadata references used when creating the projects created in the tests.
        /// </summary>
        [Obsolete("This will be removed. Use [assembly: SuppressWarnings(...) or pass in explicit in test.")]
#pragma warning disable CA1002 // Do not expose generic lists
        public static readonly List<string> SuppressedDiagnostics = SuppressWarnings.FromAttributes().ToList();
#pragma warning restore CA1002 // Do not expose generic lists

        /// <summary>
        /// Resets <see cref="SuppressedDiagnostics"/> to <see cref="SuppressWarnings.FromAttributes()"/>.
        /// </summary>
        [Obsolete("This will be removed. Use [assembly: SuppressWarnings(...) or pass in explicit in test.")]
        public static void ResetSuppressedDiagnostics()
        {
            SuppressedDiagnostics.Clear();
            SuppressedDiagnostics.AddRange(SuppressWarnings.FromAttributes());
        }
    }
}
