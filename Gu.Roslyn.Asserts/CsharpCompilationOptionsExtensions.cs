namespace Gu.Roslyn.Asserts
{
    using System.Collections.Generic;
    using System.Collections.Immutable;
    using System.ComponentModel;
    using System.Linq;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;

    /// <summary>
    /// Extension methods for <see cref="CSharpCompilationOptions"/>
    /// </summary>
    public static class CsharpCompilationOptionsExtensions
    {
        /// <summary>
        /// Configure all <paramref name="descriptors"/> to report at least warning.
        /// </summary>
        /// <param name="options">The <see cref="CSharpCompilationOptions"/>.</param>
        /// <param name="descriptors">The descriptors.</param>
        /// <returns>A <see cref="CSharpCompilationOptions"/></returns>
        /// <exception cref="System.ArgumentNullException"></exception>
        public static CSharpCompilationOptions WithWarningOrError(this CSharpCompilationOptions options, IEnumerable<DiagnosticDescriptor> descriptors)
        {
            if (options is null)
            {
                throw new System.ArgumentNullException(nameof(options));
            }

            if (descriptors is null)
            {
                throw new System.ArgumentNullException(nameof(descriptors));
            }

            var diagnosticOptions = options.SpecificDiagnosticOptions;
            foreach (var descriptor in descriptors)
            {
                diagnosticOptions = diagnosticOptions.Add(descriptor.Id, WarnOrError(descriptor.DefaultSeverity));
            }

            return options.WithSpecificDiagnosticOptions(diagnosticOptions);
        }

        /// <summary>
        /// Configure all <paramref name="descriptors"/> to report at least warning.
        /// </summary>
        /// <param name="options">The <see cref="CSharpCompilationOptions"/>.</param>
        /// <param name="descriptors">The descriptors.</param>
        /// <returns>A <see cref="CSharpCompilationOptions"/></returns>
        /// <exception cref="System.ArgumentNullException"></exception>
        public static CSharpCompilationOptions WithWarningOrError(this CSharpCompilationOptions options, params DiagnosticDescriptor[] descriptors)
            => WithWarningOrError(options, (IEnumerable<DiagnosticDescriptor>)descriptors);

        /// <summary>
        /// Configure all <paramref name="ids"/> suppressed.
        /// </summary>
        /// <param name="options">The <see cref="CSharpCompilationOptions"/>.</param>
        /// <param name="ids">The diagnostic ids.</param>
        /// <returns>A <see cref="CSharpCompilationOptions"/></returns>
        /// <exception cref="System.ArgumentNullException"></exception>
        public static CSharpCompilationOptions WithSuppressedDiagnostics(this CSharpCompilationOptions options, IEnumerable<string> ids)
        {
            if (options is null)
            {
                throw new System.ArgumentNullException(nameof(options));
            }

            if (ids is null)
            {
                throw new System.ArgumentNullException(nameof(ids));
            }

            var diagnosticOptions = options.SpecificDiagnosticOptions;
            foreach (var id in ids)
            {
                diagnosticOptions = diagnosticOptions.Add(id, ReportDiagnostic.Suppress);
            }

            return options.WithSpecificDiagnosticOptions(diagnosticOptions);
        }

        /// <summary>
        /// Configure all <paramref name="ids"/> suppressed.
        /// </summary>
        /// <param name="options">The <see cref="CSharpCompilationOptions"/>.</param>
        /// <param name="ids">The diagnostic ids.</param>
        /// <returns>A <see cref="CSharpCompilationOptions"/></returns>
        /// <exception cref="System.ArgumentNullException"></exception>
        public static CSharpCompilationOptions WithSuppressedDiagnostics(this CSharpCompilationOptions options, params string[] ids)
            => WithSuppressedDiagnostics(options, (IEnumerable<string>)ids);

        internal static CSharpCompilationOptions WithSpecific(this CSharpCompilationOptions options, ImmutableArray<DiagnosticDescriptor> supportedDiagnostics, DiagnosticDescriptor descriptor)
        {
            if (options is null)
            {
                throw new System.ArgumentNullException(nameof(options));
            }

            var diagnosticOptions = options.SpecificDiagnosticOptions;
            foreach (var supported in supportedDiagnostics)
            {
                diagnosticOptions = diagnosticOptions.Add(supported.Id, Report());

                ReportDiagnostic Report()
                {
                    if (supported.Id == descriptor.Id)
                    {
                        return WarnOrError(descriptor.DefaultSeverity);
                    }

                    return ReportDiagnostic.Suppress;
                }
            }

            return options.WithSpecificDiagnosticOptions(diagnosticOptions);
        }

        internal static CSharpCompilationOptions WithSpecific(this CSharpCompilationOptions options, ImmutableArray<DiagnosticDescriptor> supportedDiagnostics, IReadOnlyList<ExpectedDiagnostic> expecteds)
        {
            if (options is null)
            {
                throw new System.ArgumentNullException(nameof(options));
            }

            var diagnosticOptions = options.SpecificDiagnosticOptions;
            foreach (var supported in supportedDiagnostics)
            {
                diagnosticOptions = diagnosticOptions.Add(supported.Id, Report());

                ReportDiagnostic Report()
                {
                    if (expecteds.Any(x => x.Id == supported.Id))
                    {
                        return WarnOrError(supported.DefaultSeverity);
                    }

                    return ReportDiagnostic.Suppress;
                }
            }

            return options.WithSpecificDiagnosticOptions(diagnosticOptions);
        }

        private static ReportDiagnostic WarnOrError(DiagnosticSeverity severity)
        {
            return severity switch
            {
                DiagnosticSeverity.Error => ReportDiagnostic.Error,
                DiagnosticSeverity.Hidden or DiagnosticSeverity.Info or DiagnosticSeverity.Warning => ReportDiagnostic.Warn,
                _ => throw new InvalidEnumArgumentException(nameof(severity), (int)severity, typeof(DiagnosticSeverity)),
            };
        }
    }
}
