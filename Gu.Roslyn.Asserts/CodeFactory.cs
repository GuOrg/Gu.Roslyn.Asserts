using System.Collections.Immutable;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Gu.Roslyn.Asserts
{
    using System.Collections.Generic;
    using System.Linq;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;

    public static class CodeFactory
    {
        public static Solution CreateSolution(string[] code, params MetadataReference[] metadataReferences)
        {
            return CreateSolution(code, (IEnumerable<MetadataReference>)metadataReferences);
        }

        public static Solution CreateSolution(IEnumerable<string> code, IEnumerable<MetadataReference> metadataReferences)
        {
            var solution = new AdhocWorkspace()
                .CurrentSolution;
            var byNamespaces = code.Select(c => new WithMetaData(c))
                .GroupBy(c => c.Namespace);
            foreach (var byNamespace in byNamespaces)
            {
                var assemblyName = byNamespace.Key;
                var projectId = ProjectId.CreateNewId(assemblyName);
                solution = solution.AddProject(projectId, assemblyName, assemblyName, LanguageNames.CSharp)
                    .WithProjectCompilationOptions(
                        projectId,
                        new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary, allowUnsafe: true))
                    .AddMetadataReferences(projectId, metadataReferences ?? Enumerable.Empty<MetadataReference>());
                foreach (var file in byNamespace)
                {
                    var documentId = DocumentId.CreateNewId(projectId);
                    solution = solution.AddDocument(documentId, file.FileName, file.Code);
                }
            }

            return solution;
        }

        /// <summary>
        /// Create a Solution with diagnostic options set to warning for all supported diagnostics in <paramref name="analyzers"/>
        /// </summary>
        /// <param name="code">The code to create the sln from.</param>
        /// <param name="analyzers">The analyzers to add diagnostic options for.</param>
        /// <param name="metadataReferences">The metadata references.</param>
        /// <returns></returns>
        public static Solution CreateSolution(IEnumerable<string> code, IEnumerable<DiagnosticAnalyzer> analyzers, IEnumerable<MetadataReference> metadataReferences)
        {
            var solution = new AdhocWorkspace()
                .CurrentSolution;
            var byNamespaces = code.Select(c => new WithMetaData(c))
                                   .GroupBy(c => c.Namespace);

            var diagnosticOptions = analyzers.SelectMany(a => a.SupportedDiagnostics)
                                             .ToDictionary(d => d.Id, d => ReportDiagnostic.Warn);
            diagnosticOptions.Add("AD0001", ReportDiagnostic.Error);
            foreach (var byNamespace in byNamespaces)
            {
                var assemblyName = byNamespace.Key;
                var projectId = ProjectId.CreateNewId(assemblyName);
                solution = solution.AddProject(projectId, assemblyName, assemblyName, LanguageNames.CSharp)
                                   .WithProjectCompilationOptions(
                                       projectId,
                                       new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary, allowUnsafe: true)
                                                .WithSpecificDiagnosticOptions(diagnosticOptions))
                                   .AddMetadataReferences(projectId, metadataReferences ?? Enumerable.Empty<MetadataReference>());
                foreach (var file in byNamespace)
                {
                    var documentId = DocumentId.CreateNewId(projectId);
                    solution = solution.AddDocument(documentId, file.FileName, file.Code);
                }
            }

            return solution;
        }

        private struct WithMetaData
        {
            public WithMetaData(string code)
            {
                this.Code = code;
                this.FileName = CodeReader.FileName(code);
                this.Namespace = CodeReader.Namespace(code);
            }

            internal string Code { get; }

            internal string FileName { get; }

            internal string Namespace { get; }
        }
    }
}