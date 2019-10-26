namespace Gu.Roslyn.Asserts
{
    using System;
    using System.Reflection;

    /// <summary>
    /// Helper for getting ignored errors from <see cref="IgnoredErrorsAttribute"/>.
    /// </summary>
    internal static class DiagnosticSettings
    {
        private static AllowedDiagnostics? staticAllowedDiagnostics;

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
#pragma warning disable CS0618 // Suppress until removed. Will be replaced with Metadatareferences.FromAttributes()
                var attribute = assembly.GetCustomAttribute<AllowedDiagnosticsAttribute>();
#pragma warning restore CS0618 // Suppress until removed. Will be replaced with Metadatareferences.FromAttributes()
                if (attribute != null)
                {
                    staticAllowedDiagnostics = attribute.AllowedDiagnostics;
                    break;
                }
            }

            if (staticAllowedDiagnostics is null)
            {
                staticAllowedDiagnostics = Asserts.AllowedDiagnostics.Warnings;
            }

            return staticAllowedDiagnostics.Value;
        }
    }
}
