namespace Gu.Roslyn.Asserts
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Microsoft.CodeAnalysis.Diagnostics;

    public static partial class AnalyzerAssert
    {
        public static void NoDiagnostics<TAnalyzer>(params string[] code)
            where TAnalyzer : DiagnosticAnalyzer, new()
        {
            NoDiagnostics(new TAnalyzer(), code);
        }

        public static void NoDiagnostics(Type analyzerType, params string[] code)
        {
            NoDiagnostics((DiagnosticAnalyzer)Activator.CreateInstance(analyzerType), code);
        }

        public static void NoDiagnostics(DiagnosticAnalyzer analyzer, params string[] code)
        {
            NoDiagnostics(analyzer, (IEnumerable<string>)code);
        }

        public static void NoDiagnostics(DiagnosticAnalyzer analyzer, IEnumerable<string> code)
        {
            try
            {
                NoDiagnosticsAsync(analyzer, code).Wait();
            }
            catch (AggregateException e)
            {
                Fail.WithMessage(e.InnerExceptions[0].Message);
            }
        }

        public static async Task NoDiagnosticsAsync(DiagnosticAnalyzer analyzer, IEnumerable<string> code)
        {
            var diagnostics = await CodeFactory.GetDiagnosticsAsync(analyzer, code, References)
                                               .ConfigureAwait(false);

            if (diagnostics.SelectMany(x => x).Any())
            {
                throw new AssertException(string.Join(Environment.NewLine, diagnostics.SelectMany(x => x)));
            }
        }
    }
}
