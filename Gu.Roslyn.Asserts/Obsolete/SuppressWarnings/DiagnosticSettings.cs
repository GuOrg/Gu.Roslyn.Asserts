namespace Gu.Roslyn.Asserts;

using System;
using System.Reflection;

/// <summary>
/// Helper for getting ignored errors from <see cref="IgnoredErrorsAttribute"/>.
/// </summary>
[Obsolete("Use settings.")]
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
            var attribute = assembly.GetCustomAttribute<AllowedDiagnosticsAttribute>();
            if (attribute != null)
            {
                staticAllowedDiagnostics = attribute.AllowedDiagnostics;
                break;
            }
        }

        return staticAllowedDiagnostics ??= Asserts.AllowedDiagnostics.Warnings;
    }
}
