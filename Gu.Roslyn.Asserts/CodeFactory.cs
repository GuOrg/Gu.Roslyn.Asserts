namespace Gu.Roslyn.Asserts
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using Gu.Roslyn.Asserts.Internals;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.Diagnostics;

    /// <summary>
    /// A helper for creating projects and solutions from strings of code.
    /// </summary>
    public static class CodeFactory
    {
        /// <summary>
        /// The workspace used when creating solutions.
        /// </summary>
        public static readonly AdhocWorkspace Workspace = new AdhocWorkspace();

        /// <summary>
        /// An empty solution.
        /// </summary>
        public static readonly Solution EmptySolution = Workspace.CurrentSolution;

        /// <summary>
        /// Creates a solution for <paramref name="code"/>
        /// </summary>
        /// <param name="code">The sources as strings.</param>
        /// <param name="metadataReferences">The <see cref="MetadataReference"/> to use when compiling.</param>
        /// <returns>A list with diagnostics per document.</returns>
        public static Solution CreateSolution(string code, params MetadataReference[] metadataReferences)
        {
            return CreateSolution(new[] { code }, (IReadOnlyList<MetadataReference>)metadataReferences);
        }

        /// <summary>
        /// Creates a solution for <paramref name="code"/>
        /// Each unique namespace in <paramref name="code"/> is added as a project.
        /// </summary>
        /// <param name="code">The sources as strings.</param>
        /// <param name="metadataReferences">The <see cref="MetadataReference"/> to use when compiling.</param>
        /// <returns>A list with diagnostics per document.</returns>
        public static Solution CreateSolution(IReadOnlyList<string> code, params MetadataReference[] metadataReferences)
        {
            return CreateSolution(code, (IReadOnlyList<MetadataReference>)metadataReferences);
        }

        /// <summary>
        /// Creates a solution for <paramref name="code"/>
        /// </summary>
        /// <param name="code">The sources as strings.</param>
        /// <param name="metadataReferences">The <see cref="MetadataReference"/> to use when compiling.</param>
        /// <returns>A list with diagnostics per document.</returns>
        public static Solution CreateSolution(string code, IReadOnlyList<MetadataReference> metadataReferences)
        {
            return CreateSolution(new[] { code }, metadataReferences);
        }

        /// <summary>
        /// Creates a solution for <paramref name="code"/>
        /// Each unique namespace in <paramref name="code"/> is added as a project.
        /// </summary>
        /// <param name="code">The sources as strings.</param>
        /// <param name="metadataReferences">The <see cref="MetadataReference"/> to use when compiling.</param>
        /// <returns>A list with diagnostics per document.</returns>
        public static Solution CreateSolution(IReadOnlyList<string> code, IReadOnlyList<MetadataReference> metadataReferences)
        {
            return CreateSolution(code, DefaultCompilationOptions((IReadOnlyList<DiagnosticAnalyzer>)null, null), metadataReferences);
        }

        /// <summary>
        /// Create a Solution with diagnostic options set to warning for all supported diagnostics in <paramref name="analyzers"/>
        /// </summary>
        /// <param name="code">The code to create the solution from.</param>
        /// <param name="analyzers">The analyzers to add diagnostic options for.</param>
        /// <param name="metadataReferences">The metadata references.</param>
        /// <returns>A <see cref="Solution"/></returns>
        public static Solution CreateSolution(string code, IReadOnlyList<DiagnosticAnalyzer> analyzers, IReadOnlyList<MetadataReference> metadataReferences = null)
        {
            return CreateSolution(new[] { code }, analyzers, metadataReferences);
        }

        /// <summary>
        /// Create a Solution with diagnostic options set to warning for all supported diagnostics in <paramref name="analyzers"/>
        /// </summary>
        /// <param name="code">The code to create the solution from.</param>
        /// <param name="analyzers">The analyzers to add diagnostic options for.</param>
        /// <param name="metadataReferences">The metadata references.</param>
        /// <returns>A <see cref="Solution"/></returns>
        public static Solution CreateSolutionWithOneProject(string code, IReadOnlyList<DiagnosticAnalyzer> analyzers, IReadOnlyList<MetadataReference> metadataReferences = null)
        {
            return CreateSolutionWithOneProject(new[] { code }, analyzers, metadataReferences);
        }

        /// <summary>
        /// Create a Solution with diagnostic options set to warning for all supported diagnostics in <paramref name="analyzers"/>
        /// Each unique namespace in <paramref name="code"/> is added as a project.
        /// </summary>
        /// <param name="code">The code to create the solution from.</param>
        /// <param name="analyzers">The analyzers to add diagnostic options for.</param>
        /// <param name="metadataReferences">The metadata references.</param>
        /// <returns>A <see cref="Solution"/></returns>
        public static Solution CreateSolution(IReadOnlyList<string> code, IReadOnlyList<DiagnosticAnalyzer> analyzers, IReadOnlyList<MetadataReference> metadataReferences = null)
        {
            return CreateSolution(code, DefaultCompilationOptions(analyzers, null), metadataReferences);
        }

        /// <summary>
        /// Create a Solution with diagnostic options set to warning for all supported diagnostics in <paramref name="analyzers"/>
        /// Each unique namespace in <paramref name="code"/> is added as a project.
        /// </summary>
        /// <param name="code">The code to create the solution from.</param>
        /// <param name="analyzers">The analyzers to add diagnostic options for.</param>
        /// <param name="metadataReferences">The metadata references.</param>
        /// <returns>A <see cref="Solution"/></returns>
        public static Solution CreateSolutionWithOneProject(IReadOnlyList<string> code, IReadOnlyList<DiagnosticAnalyzer> analyzers, IReadOnlyList<MetadataReference> metadataReferences = null)
        {
            return CreateSolutionWithOneProject(code, DefaultCompilationOptions(analyzers, null), metadataReferences);
        }

        /// <summary>
        /// Create a <see cref="Solution"/> for <paramref name="code"/>
        /// </summary>
        /// <param name="code">The code to create the solution from.</param>
        /// <param name="compilationOptions">The <see cref="CSharpCompilationOptions"/>.</param>
        /// <param name="metadataReferences">The metadata references.</param>
        /// <returns>A <see cref="Solution"/></returns>
        public static Solution CreateSolution(string code, CSharpCompilationOptions compilationOptions, IReadOnlyList<MetadataReference> metadataReferences = null)
        {
            return CreateSolution(new[] { code }, compilationOptions, metadataReferences);
        }

        /// <summary>
        /// Create a <see cref="Solution"/> for <paramref name="code"/>
        /// </summary>
        /// <param name="code">The code to create the solution from.</param>
        /// <param name="compilationOptions">The <see cref="CSharpCompilationOptions"/>.</param>
        /// <param name="metadataReferences">The metadata references.</param>
        /// <returns>A <see cref="Solution"/></returns>
        public static Solution CreateSolutionWithOneProject(string code, CSharpCompilationOptions compilationOptions, IReadOnlyList<MetadataReference> metadataReferences = null)
        {
            return CreateSolutionWithOneProject(new[] { code }, compilationOptions, metadataReferences);
        }

        /// <summary>
        /// Create a <see cref="Solution"/> for <paramref name="code"/>
        /// Each unique namespace in <paramref name="code"/> is added as a project.
        /// </summary>
        /// <param name="code">The code to create the solution from.</param>
        /// <param name="compilationOptions">The <see cref="CSharpCompilationOptions"/>.</param>
        /// <param name="metadataReferences">The metadata references.</param>
        /// <returns>A <see cref="Solution"/></returns>
        public static Solution CreateSolution(IEnumerable<string> code, CSharpCompilationOptions compilationOptions, IEnumerable<MetadataReference> metadataReferences = null)
        {
            IReadOnlyList<ProjectReference> FindReferences(ProjectMetadata project, IEnumerable<ProjectMetadata> allProjects)
            {
                var references = new List<ProjectReference>();
                foreach (var projectMetadata in allProjects.Where(x => x.Id != project.Id))
                {
                    if (project.Sources.Any(x => x.Code.Contains($"using {projectMetadata.Name}")) ||
                        project.Sources.Any(x => x.Code.Contains($"{projectMetadata.Name}.")))
                    {
                        references.Add(new ProjectReference(projectMetadata.Id));
                    }
                }

                return references;
            }

            var solution = EmptySolution;
            var byNamespaces = code.Select(c => new SourceMetadata(c))
                                   .GroupBy(c => c.Namespace)
                                   .Select(x => new ProjectMetadata(x.Key, ProjectId.CreateNewId(x.Key), x.ToArray()))
                                   .ToArray();

            foreach (var project in byNamespaces)
            {
                var assemblyName = project.Name;
                var id = project.Id;
                solution = solution.AddProject(id, assemblyName, assemblyName, LanguageNames.CSharp)
                                   .WithProjectCompilationOptions(id, compilationOptions)
                                   .AddMetadataReferences(id, metadataReferences ?? Enumerable.Empty<MetadataReference>());

                foreach (var file in project.Sources)
                {
                    var documentId = DocumentId.CreateNewId(id);
                    solution = solution.AddDocument(documentId, file.FileName, file.Code);
                }
            }

            foreach (var project in byNamespaces)
            {
                var references = FindReferences(project, byNamespaces);
                if (references.Any())
                {
                    solution = solution.AddProjectReferences(project.Id, references);
                }
            }

            return solution;
        }

        /// <summary>
        /// Create a <see cref="Solution"/> for <paramref name="code"/>
        /// Each unique namespace in <paramref name="code"/> is added as a project.
        /// </summary>
        /// <param name="code">The code to create the solution from.</param>
        /// <param name="compilationOptions">The <see cref="CSharpCompilationOptions"/>.</param>
        /// <param name="metadataReferences">The metadata references.</param>
        /// <returns>A <see cref="Solution"/></returns>
        public static Solution CreateSolutionWithOneProject(IEnumerable<string> code, CSharpCompilationOptions compilationOptions, IEnumerable<MetadataReference> metadataReferences = null)
        {
            var solution = EmptySolution;
            var sources = code.Select(c => new SourceMetadata(c)).ToArray();
            var assemblyName = sources.Where(x => !string.IsNullOrEmpty(x.Namespace))
                                              .MinBy(x => x.Namespace.Length)
                                              .Namespace;
            var id = ProjectId.CreateNewId();
            solution = solution.AddProject(id, assemblyName, assemblyName, LanguageNames.CSharp)
                               .WithProjectCompilationOptions(id, compilationOptions)
                               .AddMetadataReferences(id, metadataReferences ?? Enumerable.Empty<MetadataReference>());

            foreach (var file in sources)
            {
                var documentId = DocumentId.CreateNewId(id);
                solution = solution.AddDocument(documentId, file.FileName, file.Code);
            }

            return solution;
        }

        /// <summary>
        /// Create default compilation options for <paramref name="analyzer"/>
        /// AD0001 is reported as error.
        /// </summary>
        /// <param name="analyzer">The analyzer to report warning or error for.</param>
        /// <param name="suppressed">The analyzer IDs to suppress.</param>
        /// <returns>An instance of <see cref="CSharpCompilationOptions"/></returns>
        public static CSharpCompilationOptions DefaultCompilationOptions(DiagnosticAnalyzer analyzer, IEnumerable<string> suppressed = null)
        {
            return DefaultCompilationOptions(new[] { analyzer }, suppressed);
        }

        /// <summary>
        /// Create default compilation options for <paramref name="analyzers"/>
        /// AD0001 is reported as error.
        /// </summary>
        /// <param name="analyzers">The analyzers to report warning or error for.</param>
        /// <param name="suppressed">The analyzer IDs to suppress.</param>
        /// <returns>An instance of <see cref="CSharpCompilationOptions"/></returns>
        public static CSharpCompilationOptions DefaultCompilationOptions(IReadOnlyList<DiagnosticAnalyzer> analyzers, IEnumerable<string> suppressed = null)
        {
            return new CSharpCompilationOptions(
                OutputKind.DynamicallyLinkedLibrary,
                allowUnsafe: true,
                specificDiagnosticOptions: CreateSpecificDiagnosticOptions(analyzers, suppressed));
        }

        /// <summary>
        /// Create default compilation options for <paramref name="descriptor"/>
        /// AD0001 is reported as error.
        /// </summary>
        /// <param name="descriptor">The analyzers to report warning or error for.</param>
        /// <param name="suppressed">The analyzer IDs to suppress.</param>
        /// <returns>An instance of <see cref="CSharpCompilationOptions"/></returns>
        public static CSharpCompilationOptions DefaultCompilationOptions(DiagnosticDescriptor descriptor, IEnumerable<string> suppressed = null)
        {
            return new CSharpCompilationOptions(
                OutputKind.DynamicallyLinkedLibrary,
                allowUnsafe: true,
                specificDiagnosticOptions: CreateSpecificDiagnosticOptions(new[] { descriptor }, suppressed));
        }

        /// <summary>
        /// Create default compilation options for <paramref name="descriptors"/>
        /// AD0001 is reported as error.
        /// </summary>
        /// <param name="descriptors">The analyzers to report warning or error for.</param>
        /// <param name="suppressed">The analyzer IDs to suppress.</param>
        /// <returns>An instance of <see cref="CSharpCompilationOptions"/></returns>
        public static CSharpCompilationOptions DefaultCompilationOptions(IReadOnlyList<DiagnosticDescriptor> descriptors, IEnumerable<string> suppressed = null)
        {
            return new CSharpCompilationOptions(
                OutputKind.DynamicallyLinkedLibrary,
                allowUnsafe: true,
                specificDiagnosticOptions: CreateSpecificDiagnosticOptions(descriptors, suppressed));
        }

        /// <summary>
        /// Create a Solution.
        /// </summary>
        /// <param name="code">
        /// The code to create the solution from.
        /// Can be a .cs, .csproj or .sln file
        /// </param>
        /// <param name="metadataReferences">The metadata references.</param>
        /// <returns>A <see cref="Solution"/></returns>
        public static Solution CreateSolution(FileInfo code, IReadOnlyList<MetadataReference> metadataReferences = null)
        {
            return CreateSolution(
                code,
                DefaultCompilationOptions((IReadOnlyList<DiagnosticAnalyzer>)null, null),
                metadataReferences);
        }

        /// <summary>
        /// Create a Solution with diagnostic options set to warning for all supported diagnostics in <paramref name="analyzers"/>
        /// </summary>
        /// <param name="code">
        /// The code to create the solution from.
        /// Can be a .cs, .csproj or .sln file
        /// </param>
        /// <param name="analyzers">The analyzers to add diagnostic options for.</param>
        /// <param name="metadataReferences">The metadata references.</param>
        /// <returns>A <see cref="Solution"/></returns>
        public static Solution CreateSolution(FileInfo code, IReadOnlyList<DiagnosticAnalyzer> analyzers, IReadOnlyList<MetadataReference> metadataReferences = null)
        {
            var compilationOptions = DefaultCompilationOptions(analyzers, null);
            return CreateSolution(code, compilationOptions, metadataReferences);
        }

        /// <summary>
        /// Create a Solution.
        /// </summary>
        /// <param name="code">
        /// The code to create the solution from.
        /// Can be a .cs, .csproj or .sln file
        /// </param>
        /// <param name="compilationOptions">The <see cref="CompilationOptions"/> to use when compiling.</param>
        /// <param name="metadataReferences">The metadata references.</param>
        /// <returns>A <see cref="Solution"/></returns>
        public static Solution CreateSolution(FileInfo code, CSharpCompilationOptions compilationOptions, IReadOnlyList<MetadataReference> metadataReferences = null)
        {
            if (string.Equals(code.Extension, ".cs", StringComparison.OrdinalIgnoreCase))
            {
                return CreateSolution(new[] { File.ReadAllText(code.FullName) }, compilationOptions, metadataReferences ?? Enumerable.Empty<MetadataReference>());
            }

            if (string.Equals(code.Extension, ".csproj", StringComparison.OrdinalIgnoreCase))
            {
                var projectInfo = ProjectFile.ParseInfo(code);
                return EmptySolution.AddProject(projectInfo)
                                    .WithProjectCompilationOptions(
                                        projectInfo.Id,
                                        compilationOptions)
                                    .AddMetadataReferences(
                                        projectInfo.Id,
                                        metadataReferences ?? Enumerable.Empty<MetadataReference>());
            }

            if (string.Equals(code.Extension, ".sln", StringComparison.OrdinalIgnoreCase))
            {
                var solutionInfo = SolutionFile.ParseInfo(code);
                var solution = EmptySolution;

                foreach (var projectInfo in solutionInfo.Projects)
                {
                    solution = solution.AddProject(projectInfo)
                                       .WithProjectCompilationOptions(
                                           projectInfo.Id,
                                           compilationOptions)
                                       .AddMetadataReferences(
                                           projectInfo.Id,
                                           metadataReferences ?? Enumerable.Empty<MetadataReference>());
                }

                return solution;
            }

            throw new NotSupportedException($"Cannot create a solution from {code.FullName}");
        }

        /// <summary>
        /// Searches parent directories for <paramref name="assembly"/> the first file matching *.sln
        /// </summary>
        /// <param name="assembly">The assembly</param>
        /// <param name="sln">The <see cref="File"/> if found.</param>
        /// <returns>A value indicating if a file was found.</returns>
        [Obsolete("Moved to SolutionFile.TryFind")]
        public static bool TryFindSolutionFile(Assembly assembly, out FileInfo sln) => SolutionFile.TryFind(assembly, out sln);

        /// <summary>
        /// Searches parent directories for <paramref name="name"/> the first file matching Foo.sln
        /// </summary>
        /// <param name="name">The assembly</param>
        /// <returns>The solution file.</returns>
        [Obsolete("Moved to SolutionFile.Find")]
        public static FileInfo FindSolutionFile(string name) => SolutionFile.Find(name);

        /// <summary>
        /// Searches parent directories for <paramref name="name"/> the first file matching Foo.sln
        /// </summary>
        /// <param name="name">The assembly</param>
        /// <param name="sln">The <see cref="File"/> if found.</param>
        /// <returns>A value indicating if a file was found.</returns>
        [Obsolete("Moved to SolutionFile.TryFind")]
        public static bool TryFindSolutionFile(string name, out FileInfo sln) => SolutionFile.TryFind(name, out sln);

        /// <summary>
        /// Searches parent directories for <paramref name="dllFile"/>
        /// </summary>
        /// <param name="dllFile">Ex Foo.dll</param>
        /// <param name="result">The <see cref="File"/> if found.</param>
        /// <returns>A value indicating if a file was found.</returns>
        [Obsolete("Moved to ProjectFile.TryFind")]
        public static bool TryFindProjectFile(FileInfo dllFile, out FileInfo result) => ProjectFile.TryFind(dllFile, out result);

        /// <summary>
        /// Searches parent directories for <paramref name="name"/>
        /// </summary>
        /// <param name="name">Ex Foo.csproj</param>
        /// <returns>The project file.</returns>
        [Obsolete("Moved to ProjectFile.Find")]
        public static FileInfo FindProjectFile(string name) => ProjectFile.Find(name);

        /// <summary>
        /// Searches parent directories for <paramref name="name"/>
        /// </summary>
        /// <param name="name">Ex Foo.csproj</param>
        /// <param name="result">The <see cref="File"/> if found.</param>
        /// <returns>A value indicating if a file was found.</returns>
        [Obsolete("Moved to ProjectFile.TryFind")]
        public static bool TryFindProjectFile(string name, out FileInfo result) => ProjectFile.TryFind(name, out result);

        /// <summary>
        /// Searches parent directories for <paramref name="fileName"/>
        /// </summary>
        /// <param name="directory">The directory to start in.</param>
        /// <param name="fileName">Ex Foo.csproj</param>
        /// <param name="result">The <see cref="File"/> if found.</param>
        /// <returns>A value indicating if a file was found.</returns>
        public static bool TryFindFileInParentDirectory(DirectoryInfo directory, string fileName, out FileInfo result)
        {
            if (directory.EnumerateFiles(fileName).TrySingle(out result))
            {
                return true;
            }

            if (directory.Parent != null)
            {
                return TryFindFileInParentDirectory(directory.Parent, fileName, out result);
            }

            result = null;
            return false;
        }

        /// <summary>
        /// Create diagnostic options that at least warns for <paramref name="analyzer"/>
        /// AD0001 is reported as error.
        /// </summary>
        /// <param name="analyzer">The analyzers to report warning or error for.</param>
        /// <param name="suppressed">The analyzer IDs to suppress.</param>
        /// <returns>A collection to pass in as argument when creating compilation options.</returns>
        public static IReadOnlyCollection<KeyValuePair<string, ReportDiagnostic>> CreateSpecificDiagnosticOptions(DiagnosticAnalyzer analyzer, IEnumerable<string> suppressed)
        {
            return CreateSpecificDiagnosticOptions(new[] { analyzer }, suppressed);
        }

        /// <summary>
        /// Create diagnostic options that at least warns for <paramref name="analyzers"/>
        /// AD0001 is reported as error.
        /// </summary>
        /// <param name="analyzers">The analyzers to report warning or error for.</param>
        /// <param name="suppressed">The analyzer IDs to suppress.</param>
        /// <returns>A collection to pass in as argument when creating compilation options.</returns>
        public static IReadOnlyCollection<KeyValuePair<string, ReportDiagnostic>> CreateSpecificDiagnosticOptions(IEnumerable<DiagnosticAnalyzer> analyzers, IEnumerable<string> suppressed)
        {
            return CreateSpecificDiagnosticOptions(analyzers?.SelectMany(x => x.SupportedDiagnostics).Distinct(), suppressed);
        }

        /// <summary>
        /// Create diagnostic options that at least warns for <paramref name="enabled"/>
        /// AD0001 is reported as error.
        /// </summary>
        /// <param name="enabled">The analyzers to report warning or error for.</param>
        /// <param name="suppressed">The diagnostic IDs to suppress.</param>
        /// <returns>A collection to pass in as argument when creating compilation options.</returns>
        public static IReadOnlyCollection<KeyValuePair<string, ReportDiagnostic>> CreateSpecificDiagnosticOptions(IEnumerable<DiagnosticDescriptor> enabled, IEnumerable<string> suppressed)
        {
            ReportDiagnostic WarnOrError(DiagnosticSeverity severity)
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
                        throw new ArgumentOutOfRangeException();
                }
            }

            var diagnosticOptions = new Dictionary<string, ReportDiagnostic>();
            if (enabled != null)
            {
                foreach (var descriptor in enabled)
                {
                    diagnosticOptions.Add(descriptor.Id, WarnOrError(descriptor.DefaultSeverity));
                }
            }

            diagnosticOptions.Add("AD0001", ReportDiagnostic.Error);
            if (suppressed != null)
            {
                foreach (var id in suppressed)
                {
                    diagnosticOptions[id] = ReportDiagnostic.Suppress;
                }
            }

            return diagnosticOptions;
        }

        [System.Diagnostics.DebuggerDisplay("{FileName}")]
        private struct SourceMetadata
        {
            public SourceMetadata(string code)
            {
                this.Code = code;
                this.FileName = CodeReader.FileName(code);
                this.Namespace = CodeReader.Namespace(code);
            }

            internal string Code { get; }

            internal string FileName { get; }

            internal string Namespace { get; }
        }

        [System.Diagnostics.DebuggerDisplay("{Name}")]
        private struct ProjectMetadata
        {
            public ProjectMetadata(string name, ProjectId id, IReadOnlyList<SourceMetadata> sources)
            {
                this.Name = name;
                this.Id = id;
                this.Sources = sources;
            }

            internal string Name { get; }

            internal ProjectId Id { get; }

            internal IReadOnlyList<SourceMetadata> Sources { get; }
        }
    }
}
