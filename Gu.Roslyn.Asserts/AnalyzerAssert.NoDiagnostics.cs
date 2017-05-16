namespace Gu.Roslyn.Asserts
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Threading.Tasks;
    using Gu.Roslyn.Asserts.Internals;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.Diagnostics;

    public static partial class AnalyzerAssert
    {
        /// <summary>
        /// Verifies that <paramref name="code"/> produces no diagnostics when analyzed with <typeparamref name="TAnalyzer"/>.
        /// </summary>
        /// <typeparam name="TAnalyzer">The type of the analyzer.</typeparam>
        /// <param name="code">The code with error positions indicated.</param>
        public static void NoDiagnostics<TAnalyzer>(params string[] code)
            where TAnalyzer : DiagnosticAnalyzer, new()
        {
            NoDiagnostics(new TAnalyzer(), code);
        }

        /// <summary>
        /// Verifies that <paramref name="code"/> produces no diagnostics when analyzed with <paramref name="analyzerType"/>.
        /// </summary>
        /// <param name="analyzerType">The type of the analyzer.</param>
        /// <param name="code">The code with error positions indicated.</param>
        public static void NoDiagnostics(Type analyzerType, params string[] code)
        {
            NoDiagnostics((DiagnosticAnalyzer)Activator.CreateInstance(analyzerType), code);
        }

        /// <summary>
        /// Verifies that <paramref name="code"/> produces no diagnostics when analyzed with <paramref name="analyzer"/>.
        /// </summary>
        /// <param name="analyzer">The analyzer.</param>
        /// <param name="code">The code with error positions indicated.</param>
        public static void NoDiagnostics(DiagnosticAnalyzer analyzer, params string[] code)
        {
            NoDiagnostics(analyzer, (IReadOnlyList<string>)code);
        }

        /// <summary>
        /// Verifies that <paramref name="code"/> produces no diagnostics when analyzed with <paramref name="analyzer"/>.
        /// </summary>
        /// <param name="analyzer">The analyzer.</param>
        /// <param name="code">The code with error positions indicated.</param>
        public static void NoDiagnostics(DiagnosticAnalyzer analyzer, IReadOnlyList<string> code)
        {
            try
            {
                NoDiagnosticsAsync(analyzer, code, MetadataReference).Wait();
            }
            catch (AggregateException e)
            {
                throw Fail.CreateException(e.InnerExceptions[0]);
            }
        }

        /// <summary>
        /// Verifies that <paramref name="code"/> produces no diagnostics when analyzed with <paramref name="analyzer"/>.
        /// </summary>
        /// <param name="analyzer">The analyzer.</param>
        /// <param name="code">The code with error positions indicated.</param>
        /// <param name="metadataReferences">The metadata references to use when compiling.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        public static async Task NoDiagnosticsAsync(DiagnosticAnalyzer analyzer, IReadOnlyList<string> code, IReadOnlyList<MetadataReference> metadataReferences)
        {
            var diagnostics = await Analyze.GetDiagnosticsAsync(analyzer, code, metadataReferences)
                                           .ConfigureAwait(false);

            if (diagnostics.SelectMany(x => x).Any())
            {
                throw new AssertException(string.Join(Environment.NewLine, diagnostics.SelectMany(x => x)));
            }
        }

        /// <summary>
        /// Verifies that <paramref name="code"/> produces no diagnostics when analyzed with <typeparamref name="TAnalyzer"/>.
        /// </summary>
        /// <typeparam name="TAnalyzer">The type of the analyzer.</typeparam>
        /// <param name="code">The code with error positions indicated.</param>
        public static void NoDiagnostics<TAnalyzer>(FileInfo code)
            where TAnalyzer : DiagnosticAnalyzer, new()
        {
            NoDiagnostics(new TAnalyzer(), code);
        }

        /// <summary>
        /// Verifies that <paramref name="code"/> produces no diagnostics when analyzed with <paramref name="analyzerType"/>.
        /// </summary>
        /// <param name="analyzerType">The type of the analyzer.</param>
        /// <param name="code">
        /// The code to create the solution from.
        /// Can be a .cs, .csproj or .sln file
        /// </param>
        public static void NoDiagnostics(Type analyzerType, FileInfo code)
        {
            NoDiagnostics((DiagnosticAnalyzer)Activator.CreateInstance(analyzerType), code);
        }

        /// <summary>
        /// Verifies that <paramref name="code"/> produces no diagnostics when analyzed with <paramref name="analyzer"/>.
        /// </summary>
        /// <param name="analyzer">The analyzer.</param>
        /// <param name="code">
        /// The code to create the solution from.
        /// Can be a .cs, .csproj or .sln file
        /// </param>
        public static void NoDiagnostics(DiagnosticAnalyzer analyzer, FileInfo code)
        {
            try
            {
                NoDiagnosticsAsync(analyzer, code, MetadataReference).Wait();
            }
            catch (AggregateException e)
            {
                throw Fail.CreateException(e.InnerExceptions[0]);
            }
        }

        /// <summary>
        /// Verifies that <paramref name="code"/> produces no diagnostics when analyzed with <paramref name="analyzer"/>.
        /// </summary>
        /// <param name="analyzer">The analyzer.</param>
        /// <param name="code">
        /// The code to create the solution from.
        /// Can be a .cs, .csproj or .sln file
        /// </param>
        /// <param name="metadataReferences">The metadata references to use when compiling.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        public static async Task NoDiagnosticsAsync(DiagnosticAnalyzer analyzer, FileInfo code, IReadOnlyList<MetadataReference> metadataReferences)
        {
            var diagnostics = await Analyze.GetDiagnosticsAsync(analyzer, code, metadataReferences)
                                           .ConfigureAwait(false);

            if (diagnostics.SelectMany(x => x).Any())
            {
                throw new AssertException(string.Join(Environment.NewLine, diagnostics.SelectMany(x => x)));
            }
        }
    }
}
