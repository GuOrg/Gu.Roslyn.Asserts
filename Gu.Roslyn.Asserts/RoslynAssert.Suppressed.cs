namespace Gu.Roslyn.Asserts
{
    using System;
    using System.Collections.Generic;
    using System.Collections.Immutable;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using Gu.Roslyn.Asserts.Internals;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.Diagnostics;

    public static partial class RoslynAssert
    {
        /// <summary>
        /// Verifies that any diagnostics in <paramref name="code"/> are not suppressed when analyzed with <paramref name="suppressor"/>.
        /// </summary>
        /// <param name="suppressor">The <see cref="DiagnosticSuppressor"/> to check <paramref name="code"/> with.</param>
        /// <param name="code">The code to analyze using <paramref name="suppressor"/>. Analyzing the code is not expected to suppress any errors or warnings.</param>
        /// <param name="settings">The <see cref="Settings"/>.</param>
        public static void NotSuppressed(DiagnosticSuppressor suppressor, string code, Settings? settings = null)
        {
            if (suppressor is null)
            {
                throw new ArgumentNullException(nameof(suppressor));
            }

            var diagnostics = GetSuppressableDiagnostics(suppressor, code, settings);

            // Nothing should be suppressed.
            NoDiagnostics(diagnostics.Where(d => d.IsSuppressed));
        }

        /// <summary>
        /// Verifies that any diagnostics in <paramref name="code"/> are suppressed when analyzed with <paramref name="suppressor"/>.
        /// </summary>
        /// <param name="suppressor">The <see cref="DiagnosticSuppressor"/> to check <paramref name="code"/> with.</param>
        /// <param name="code">The code to analyze using <paramref name="suppressor"/>. Analyzing the code is expected to suppress the specified errors.</param>
        /// <param name="settings">The <see cref="Settings"/>.</param>
        public static void Suppressed(DiagnosticSuppressor suppressor, string code, Settings? settings = null)
        {
            if (suppressor is null)
            {
                throw new ArgumentNullException(nameof(suppressor));
            }

            var diagnostics = GetSuppressableDiagnostics(suppressor, code, settings);

            // Everything should be suppressed.
            NoDiagnostics(diagnostics.Where(d => !d.IsSuppressed));
        }

        /// <summary>
        /// Verifies that any diagnostics matching <paramref name="suppressionDescriptor"/> in <paramref name="code"/>
        /// are suppressed when analyzed with <paramref name="suppressor"/>.
        /// </summary>
        /// <param name="suppressor">The <see cref="DiagnosticSuppressor"/> to check <paramref name="code"/> with.</param>
        /// <param name="suppressionDescriptor">The <see cref="SuppressionDescriptor"/> we expect to be suppressed.</param>
        /// <param name="code">The code to analyze using <paramref name="suppressor"/>. Analyzing the code is expected to suppress the specified errors.</param>
        /// <param name="settings">The <see cref="Settings"/>.</param>
        public static void SuppressedBy(DiagnosticSuppressor suppressor, SuppressionDescriptor suppressionDescriptor, string code, Settings? settings = null)
        {
            if (suppressor is null)
            {
                throw new ArgumentNullException(nameof(suppressor));
            }

            if (suppressionDescriptor is null)
            {
                throw new ArgumentNullException(nameof(suppressionDescriptor));
            }

            if (!suppressor.SupportedSuppressions.Contains(suppressionDescriptor))
            {
                throw new ArgumentException("This descriptor is not supported by the suppressor", nameof(suppressionDescriptor));
            }

            var diagnostics = GetSuppressableDiagnostics(suppressor, code, settings);

            // Everything should be suppressed.
            NoDiagnostics(diagnostics.Where(d => !d.IsSuppressed));

            // Verify that this is suppressed by the expected SuppressionDescriptor
            StringBuilderPool.PooledStringBuilder? builder = null;
            foreach (var diagnostic in diagnostics)
            {
                var justification = GetProgrammaticSuppressionInfo(diagnostic);
                if (!justification.Contains((suppressionDescriptor.Id, suppressionDescriptor.Justification)))
                {
                    builder ??= StringBuilderPool.Borrow().AppendLine($"Expected diagnostic to be suppressed by {suppressionDescriptor.Id}:{suppressionDescriptor.Justification} but was:");
                    builder.AppendLine(string.Join(Environment.NewLine, justification.Select(x => $"{x.Id}:{x.Justification}")));
                }
            }

            if (builder is not null)
            {
                throw new AssertException(builder.Return());
            }
        }

        private static IEnumerable<Diagnostic> GetSuppressableDiagnostics(DiagnosticSuppressor suppressor, string code, Settings? settings)
        {
            settings ??= Settings.Default;

            var sln = CodeFactory.CreateSolution(suppressor, new[] { code }, settings);
            var diagnostics = Analyze.GetAllDiagnostics(sln);

            var suppressableIds = new HashSet<string>(suppressor.SupportedSuppressions.Select(s => s.SuppressedDiagnosticId));

            var suppressibleDiagnostics = diagnostics.Where(d => suppressableIds.Contains(d.Id));
            var nonSuppressibleDiagnostics = diagnostics.Where(d => !suppressableIds.Contains(d.Id));

            NoDiagnostics(nonSuppressibleDiagnostics);

            if (!suppressibleDiagnostics.Any())
            {
                throw new AssertException("Found no errors to suppress");
            }

            diagnostics = Analyze.GetAllDiagnostics(suppressor, sln);

            return diagnostics;
        }

        private static ImmutableHashSet<(string Id, LocalizableString Justification)> GetProgrammaticSuppressionInfo(Diagnostic diagnostic)
        {
            // We cannot check for the correct suppression nicely.
            // ProgrammaticSuppressionInfo is an internal property returning an internal class.
            // Resorting to reflection ...
            const string programmaticSuppressionInfoPropertyName = "ProgrammaticSuppressionInfo";
            const string programmaticPropertyInfoSuppressionsPropertyName = "Suppressions";

            var programmaticSuppressionInfoProperty = diagnostic.GetType().GetProperty(
                programmaticSuppressionInfoPropertyName,
                BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly);

            if (programmaticSuppressionInfoProperty is not null)
            {
                var programmaticSuppressionInfo = programmaticSuppressionInfoProperty.GetValue(diagnostic);

                if (programmaticSuppressionInfo is not null)
                {
                    var suppressionsProperty = programmaticSuppressionInfo.GetType().GetProperty(
                        programmaticPropertyInfoSuppressionsPropertyName,
                        BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);

                    if (suppressionsProperty != null)
                    {
                        var suppressions = suppressionsProperty.GetValue(programmaticSuppressionInfo);

                        if (suppressions is ImmutableHashSet<(string Id, LocalizableString Justification)> suppressionsHashSet)
                        {
                            return suppressionsHashSet;
                        }
                    }
                }
            }

            throw new AssertException("Cannot find the suppresion information on the Diagnostic");
        }
    }
}
