namespace Gu.Roslyn.Asserts
{
    using System;
    using System.Collections.Generic;
    using System.Collections.Immutable;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.Diagnostics;

    public partial class AnalyzerAssert
    {
        public static readonly List<MetadataReference> References = new List<MetadataReference>();

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
                throw e.InnerException;
            }
        }

        public static async Task NoDiagnosticsAsync(DiagnosticAnalyzer analyzer, IEnumerable<string> code)
        {
            var sln = CodeFactory.CreateSolution(code, References);
            var results = new List<ImmutableArray<Diagnostic>>();
            foreach (var project in sln.Projects)
            {
                var compilation = await project.GetCompilationAsync(CancellationToken.None)
                                               .ConfigureAwait(false);

                var withAnalyzers = compilation.WithAnalyzers(
                    ImmutableArray.Create(analyzer),
                    project.AnalyzerOptions,
                    CancellationToken.None);
                results.Add(await withAnalyzers.GetAnalyzerDiagnosticsAsync(CancellationToken.None)
                                               .ConfigureAwait(false));
            }

            if (results.SelectMany(x => x).Any())
            {
                throw new AssertException(string.Join(Environment.NewLine, results.SelectMany(x => x)));
            }
        }
    }
}
