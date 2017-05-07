namespace Gu.Roslyn.Asserts
{
    using System;
    using System.Collections.Generic;
    using System.Collections.Immutable;
    using System.Linq;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.Diagnostics;

    public static class CodeFactory
    {
        //internal static Project CreateProject(IEnumerable<DiagnosticAnalyzer> analyzers)
        //{
        //    var projFile = ProjFile(typeof(KnownSymbol)).FullName;
        //    var projectName = Path.GetFileNameWithoutExtension(projFile);
        //    var projectId = ProjectId.CreateNewId(projectName);
        //    var solution = CreateSolution(projectId, projectName);
        //    var doc = XDocument.Parse(File.ReadAllText(projFile));
        //    var directory = Path.GetDirectoryName(projFile);
        //    var compiles = doc.Descendants(XName.Get("Compile", "http://schemas.microsoft.com/developer/msbuild/2003"))
        //        .ToArray();
        //    if (compiles.Length == 0)
        //    {
        //        throw new InvalidOperationException("Parsing failed, no <Compile ... /> found.");
        //    }

        //    foreach (var compile in compiles)
        //    {
        //        var csFile = Path.Combine(directory, compile.Attribute("Include").Value);
        //        var documentId = DocumentId.CreateNewId(projectId);
        //        using (var stream = File.OpenRead(csFile))
        //        {
        //            solution = solution.AddDocument(documentId, csFile, SourceText.From(stream));
        //        }
        //    }

        //    var project = solution.GetProject(projectId);
        //    return ApplyCompilationOptions(project, analyzers);
        //}

        //[SuppressMessage("ReSharper", "PossibleNullReferenceException")]
        //private static FileInfo ProjFile(Type typeInAssembly)
        //{
        //    return new FileInfo(new Uri(typeInAssembly.Assembly.CodeBase).LocalPath)
        //        .Directory
        //        .Parent
        //        .Parent
        //        .Parent
        //        .EnumerateFiles("Gu.Analyzers.Analyzers.csproj", SearchOption.AllDirectories)
        //        .Single();
        //}

        public static Solution CreateSolution(string[] code, params MetadataReference[] metadataReferences)
        {
            throw new NotImplementedException();
            //var solution = new AdhocWorkspace()
            //    .CurrentSolution
            //    .AddProject(projectId, projectName, projectName, LanguageNames.CSharp)
            //    .WithProjectCompilationOptions(
            //        projectId,
            //        new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary, allowUnsafe: true))
            //    .AddMetadataReferences(projectId, metadataReferences);
            //var parseOptions = solution.GetProject(projectId).ParseOptions;
            //return solution.WithProjectParseOptions(projectId, parseOptions.WithDocumentationMode(DocumentationMode.Diagnose));
        }

        private static Project ApplyCompilationOptions(Project project, IEnumerable<DiagnosticAnalyzer> analyzer)
        {
            // update the project compilation options
            var diagnostics = ImmutableDictionary.CreateRange(
                analyzer.SelectMany(
                    a => a.SupportedDiagnostics.Select(
                        x => new KeyValuePair<string, ReportDiagnostic>(x.Id, ReportDiagnostic.Warn))));

            var modifiedSpecificDiagnosticOptions = diagnostics.SetItems(project.CompilationOptions.SpecificDiagnosticOptions);
            var modifiedCompilationOptions = project.CompilationOptions.WithSpecificDiagnosticOptions(modifiedSpecificDiagnosticOptions);

            var solution = project.Solution.WithProjectCompilationOptions(project.Id, modifiedCompilationOptions);
            return solution.GetProject(project.Id);
        }
    }
}