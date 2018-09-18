namespace Gu.Roslyn.Asserts
{
    using System;
    using System.Collections.Generic;
    using System.Reflection;

    /// <summary>
    /// Helper for getting ignored errors from <see cref="IgnoredErrorsAttribute"/>.
    /// </summary>
    internal static class DiagnosticSettings
    {
#pragma warning disable 169
        private static IReadOnlyList<string> staticErrors;
        private static AllowedDiagnostics? staticAllowedDiagnostics;
#pragma warning restore 169

        /// <summary>
        /// Get the metadata references specified with <see cref="MetadataReferenceAttribute"/> and <see cref="MetadataReferencesAttribute"/> in the test assembly.
        /// </summary>
        internal static IReadOnlyList<string> AllowedErrorIds()
        {
            if (staticErrors != null)
            {
                return staticErrors;
            }

            var errors = new List<string>();
            staticErrors = errors;
            var assemblies = AppDomain.CurrentDomain.GetAssemblies();
            foreach (var assembly in assemblies)
            {
                var attribute = assembly.GetCustomAttribute<IgnoredErrorsAttribute>();
                if (attribute?.ErrorIds != null)
                {
                    errors.AddRange(attribute.ErrorIds);
                }
            }

            return staticErrors;
        }

        /// <summary>
        /// Get the metadata references specified with <see cref="MetadataReferenceAttribute"/> and <see cref="MetadataReferencesAttribute"/> in the test assembly.
        /// </summary>
        internal static AllowedDiagnostics AllowedDiagnostics()
        {
            if (staticAllowedDiagnostics != null)
            {
                return staticAllowedDiagnostics.Value;
            }

            var assemblies = AppDomain.CurrentDomain.GetAssemblies();
            foreach (var assembly in assemblies)
            {
                var attribute = assembly.GetCustomAttribute<AllowedDiagnosticsAttribute>();
                if (attribute != null)
                {
                    staticAllowedDiagnostics = attribute.AllowedDiagnostics;
                    break;
                }
            }

            if (staticAllowedDiagnostics == null)
            {
                staticAllowedDiagnostics = Asserts.AllowedDiagnostics.Warnings;
            }

            return staticAllowedDiagnostics.Value;
        }
    }
}
