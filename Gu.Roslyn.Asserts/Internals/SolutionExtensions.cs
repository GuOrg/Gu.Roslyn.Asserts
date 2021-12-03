namespace Gu.Roslyn.Asserts.Internals
{
    using System.Collections.Generic;

    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;

    internal static class SolutionExtensions
    {
        public static Solution WithWarningOrError(this Solution solution, IEnumerable<DiagnosticDescriptor> descriptors)
        {
            var updated = solution;
            foreach (var project in solution.Projects)
            {
                updated = updated.GetProject(project.Id)!
                                 .WithCompilationOptions(((CSharpCompilationOptions)project.CompilationOptions).WithWarningOrError(descriptors))
                                 .Solution;
            }

            return updated;
        }
    }
}
