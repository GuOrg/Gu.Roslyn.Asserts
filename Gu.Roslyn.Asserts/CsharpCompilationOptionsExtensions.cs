namespace Gu.Roslyn.Asserts
{
    using System.Collections.Generic;
    using System.Collections.Immutable;
    using System.ComponentModel;
    using System.Linq;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;

    public static class CsharpCompilationOptionsExtensions
    {
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

        public static CSharpCompilationOptions WithWarningOrError(this CSharpCompilationOptions options, params DiagnosticDescriptor[] descriptors)
            => WithWarningOrError(options, (IEnumerable<DiagnosticDescriptor>)descriptors);

        public static CSharpCompilationOptions WithSuppressed(this CSharpCompilationOptions options, IEnumerable<string> ids)
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

        public static CSharpCompilationOptions WithSuppressed(this CSharpCompilationOptions options, params string[] ids)
            => WithSuppressed(options, (IEnumerable<string>)ids);

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
            switch (severity)
            {
                case DiagnosticSeverity.Error:
                    return ReportDiagnostic.Error;
                case DiagnosticSeverity.Hidden:
                case DiagnosticSeverity.Info:
                case DiagnosticSeverity.Warning:
                    return ReportDiagnostic.Warn;
                default:
                    throw new InvalidEnumArgumentException(nameof(severity), (int)severity, typeof(DiagnosticSeverity));
            }
        }
    }
}
