namespace Gu.Roslyn.Asserts.Internals
{
    using System.Collections.Generic;
    using Microsoft.CodeAnalysis;

    internal static class DiagnosticExt
    {
        internal static string ToString(this Diagnostic diagnostic, IReadOnlyList<string> sources)
        {
            return IdAndPosition.Create(diagnostic).ToString(sources);
        }
    }
}
