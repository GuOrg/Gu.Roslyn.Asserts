namespace Gu.Roslyn.Asserts
{
    using System.Collections.Generic;
    using System.ComponentModel;

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

                static ReportDiagnostic WarnOrError(DiagnosticSeverity severity)
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

            return options.WithSpecificDiagnosticOptions(diagnosticOptions);
        }

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
    }
}
