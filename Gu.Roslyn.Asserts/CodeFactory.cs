namespace Gu.Roslyn.Asserts
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.IO;
    using System.Linq;

    using Gu.Roslyn.Asserts.Internals;

    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.Diagnostics;
    using Microsoft.CodeAnalysis.Text;

    /// <summary>
    /// A helper for creating projects and solutions from strings of code.
    /// </summary>
    public static class CodeFactory
    {
        /// <summary>
        /// Compilation options for a dll.
        /// </summary>
        public static readonly CSharpCompilationOptions DllCompilationOptions = DefaultCompilationOptions(null);

        /// <summary>
        /// The workspace used when creating solutions.
        /// </summary>
        public static readonly AdhocWorkspace Workspace = new();

        /// <summary>
        /// An empty solution.
        /// </summary>
        public static readonly Solution EmptySolution = Workspace.CurrentSolution;

        /// <summary>
        /// Create a <see cref="Solution"/> for <paramref name="code"/>
        /// Each unique namespace in <paramref name="code"/> is added as a project.
        /// </summary>
        /// <param name="code">The code to create the solution from.</param>
        /// <param name="parseOptions">The <see cref="CSharpParseOptions"/>.</param>
        /// <param name="compilationOptions">The <see cref="CSharpCompilationOptions"/>.</param>
        /// <param name="metadataReferences">The metadata references.</param>
        /// <returns>A <see cref="Solution"/>.</returns>
        public static Solution CreateSolution(IEnumerable<string> code, CSharpParseOptions parseOptions, CSharpCompilationOptions compilationOptions, IEnumerable<MetadataReference>? metadataReferences)
        {
            if (code is null)
            {
                throw new ArgumentNullException(nameof(code));
            }

            if (compilationOptions is null)
            {
                throw new ArgumentNullException(nameof(compilationOptions));
            }

            if (parseOptions is null)
            {
                throw new ArgumentNullException(nameof(parseOptions));
            }

            var solutionInfo = SolutionInfo.Create(
                SolutionId.CreateNewId("Test.sln"),
                VersionStamp.Default,
                projects: GetProjectInfos());
            var solution = EmptySolution;
            foreach (var projectInfo in solutionInfo.Projects)
            {
                solution = solution.AddProject(projectInfo.WithProjectReferences(FindReferences(projectInfo)));
            }

            return solution;

            IEnumerable<ProjectInfo> GetProjectInfos()
            {
                var byNamespace = new SortedDictionary<string, List<string>>();
                foreach (var document in code)
                {
                    var ns = CodeReader.Namespace(document);
                    if (byNamespace.TryGetValue(ns, out var doc))
                    {
                        doc.Add(document);
                    }
                    else
                    {
                        byNamespace[ns] = new List<string> { document };
                    }
                }

                var byProject = new SortedDictionary<string, List<KeyValuePair<string, List<string>>>>();
                foreach (var kvp in byNamespace)
                {
                    var last = byProject.Keys.LastOrDefault();
                    var ns = kvp.Key;
                    if (last != null &&
                        ns.Contains(last))
                    {
                        byProject[last].Add(kvp);
                    }
                    else
                    {
                        byProject.Add(ns, new List<KeyValuePair<string, List<string>>> { kvp });
                    }
                }

                foreach (var kvp in byProject)
                {
                    var assemblyName = kvp.Key;
                    var projectId = ProjectId.CreateNewId(assemblyName);
                    yield return ProjectInfo.Create(
                                                projectId,
                                                VersionStamp.Default,
                                                assemblyName,
                                                assemblyName,
                                                LanguageNames.CSharp,
                                                compilationOptions: compilationOptions,
                                                metadataReferences: metadataReferences,
                                                documents: kvp.Value.SelectMany(x => x.Value)
                                                              .Select(
                                                                  x =>
                                                                  {
                                                                      var documentName = CodeReader.FileName(x);
                                                                      return DocumentInfo.Create(
                                                                          DocumentId.CreateNewId(projectId, documentName),
                                                                          documentName,
                                                                          sourceCodeKind: SourceCodeKind.Regular,
                                                                          loader: new StringLoader(x));
                                                                  }))
                                            .WithParseOptions(parseOptions);
                }
            }

            IEnumerable<ProjectReference> FindReferences(ProjectInfo projectInfo)
            {
                var references = new List<ProjectReference>();
                foreach (var other in solutionInfo.Projects.Where(x => x.Id != projectInfo.Id))
                {
                    if (projectInfo.Documents.Any(x => x.TextLoader is StringLoader stringLoader &&
                                                 (stringLoader.Code.Contains($"using {other.Name};") ||
                                                  stringLoader.Code.Contains($"{other.Name}."))))
                    {
                        references.Add(new ProjectReference(other.Id));
                    }
                }

                return references;
            }
        }

        /// <summary>
        /// Create a Solution.
        /// </summary>
        /// <param name="code">
        /// The code to create the solution from.
        /// Can be a .cs, .csproj or .sln file.
        /// </param>
        /// <param name="parseOptions">The <see cref="CSharpParseOptions"/>.</param>
        /// <param name="compilationOptions">The <see cref="CompilationOptions"/> to use when compiling.</param>
        /// <param name="metadataReferences">The metadata references.</param>
        /// <returns>A <see cref="Solution"/>.</returns>
        public static Solution CreateSolution(FileInfo code, CSharpParseOptions parseOptions, CSharpCompilationOptions compilationOptions, IEnumerable<MetadataReference>? metadataReferences)
        {
            if (code is null)
            {
                throw new ArgumentNullException(nameof(code));
            }

            if (compilationOptions is null)
            {
                throw new ArgumentNullException(nameof(compilationOptions));
            }

            if (string.Equals(code.Extension, ".cs", StringComparison.OrdinalIgnoreCase))
            {
                return CreateSolution(
                    new[] { File.ReadAllText(code.FullName) },
                    parseOptions,
                    compilationOptions,
                    metadataReferences ?? Enumerable.Empty<MetadataReference>());
            }

            if (string.Equals(code.Extension, ".csproj", StringComparison.OrdinalIgnoreCase))
            {
                return EmptySolution.AddProject(
                    ProjectFile.ParseInfo(code)
                               .WithParseOptions(parseOptions)
                               .WithCompilationOptions(compilationOptions)
                               .WithMetadataReferences(metadataReferences));
            }

            if (string.Equals(code.Extension, ".sln", StringComparison.OrdinalIgnoreCase))
            {
                var solution = EmptySolution;
                var solutionInfo = SolutionFile.ParseInfo(code);
                foreach (var projectInfo in solutionInfo.Projects)
                {
                    solution = solution.AddProject(
                        projectInfo
                            .WithParseOptions(parseOptions)
                            .WithCompilationOptions(compilationOptions)
                            .WithMetadataReferences(metadataReferences));
                }

                return solution;
            }

            throw new NotSupportedException($"Cannot create a solution from {code.FullName}");
        }

        /// <summary>
        /// Create a <see cref="Solution"/> for <paramref name="code"/>
        /// Each unique namespace in <paramref name="code"/> is added as a project.
        /// </summary>
        /// <param name="code">The code to create the solution from.</param>
        /// <param name="parseOptions">The <see cref="CSharpParseOptions"/>.</param>
        /// <param name="compilationOptions">The <see cref="CSharpCompilationOptions"/>.</param>
        /// <param name="metadataReferences">The metadata references.</param>
        /// <returns>A <see cref="Solution"/>.</returns>
        public static Solution CreateSolutionWithOneProject(IEnumerable<string> code, CSharpParseOptions parseOptions, CSharpCompilationOptions compilationOptions, IEnumerable<MetadataReference>? metadataReferences)
        {
            if (code is null)
            {
                throw new ArgumentNullException(nameof(code));
            }

            if (parseOptions is null)
            {
                throw new ArgumentNullException(nameof(parseOptions));
            }

            if (compilationOptions is null)
            {
                throw new ArgumentNullException(nameof(compilationOptions));
            }

            var projectInfo = GetProjectInfo().WithParseOptions(parseOptions);
            return EmptySolution.AddProject(projectInfo);

            ProjectInfo GetProjectInfo()
            {
                var projectName = ProjectName();

                var projectId = ProjectId.CreateNewId(projectName);
                return ProjectInfo.Create(
                    projectId,
                    VersionStamp.Default,
                    projectName,
                    projectName,
                    LanguageNames.CSharp,
                    metadataReferences: metadataReferences,
                    compilationOptions: compilationOptions,
                    documents: code.Select(
                        x =>
                        {
                            var documentName = CodeReader.FileName(x);
                            return DocumentInfo.Create(
                                DocumentId.CreateNewId(projectId, documentName),
                                documentName,
                                sourceCodeKind: SourceCodeKind.Regular,
                                loader: TextLoader.From(
                                    TextAndVersion.Create(
                                        SourceText.From(x, null, SourceHashAlgorithm.Sha1),
                                        VersionStamp.Default)));
                        }));

                string ProjectName()
                {
                    string? projectName = null;
                    foreach (var doc in code)
                    {
                        if (projectName is null)
                        {
                            projectName = CodeReader.Namespace(doc);
                        }
                        else
                        {
                            var ns = CodeReader.Namespace(doc);
                            var indexOf = ns.IndexOf('.');
                            if (indexOf > 0)
                            {
                                ns = ns.Substring(0, indexOf);
                            }

                            if (ns.Length < projectName.Length)
                            {
                                projectName = ns;
                            }
                        }
                    }

                    return projectName ?? throw new InvalidOperationException("Could not find project name.");
                }
            }
        }

        /// <summary>
        /// Create a Solution by cloning a remote git repository.
        /// </summary>
        /// <param name="githubUrl">
        /// The url to the code to create the solution from.
        /// Can be a .cs, .csproj or .sln file.
        /// Sample URL: https://github.com/GuOrg/Gu.Roslyn.Asserts/blob/master/Gu.Roslyn.Asserts.sln.
        /// </param>
        /// <param name="parseOptions">The <see cref="CSharpParseOptions"/>.</param>
        /// <param name="compilationOptions">The <see cref="CSharpCompilationOptions"/>.</param>
        /// <param name="metadataReferences">The metadata references.</param>
        /// <returns>>A <see cref="Solution"/>.</returns>
        public static Solution CreateSolution(Uri githubUrl, CSharpParseOptions parseOptions, CSharpCompilationOptions compilationOptions, IEnumerable<MetadataReference>? metadataReferences)
        {
            if (githubUrl is null)
            {
                throw new ArgumentNullException(nameof(githubUrl));
            }

            if (parseOptions is null)
            {
                throw new ArgumentNullException(nameof(parseOptions));
            }

            if (compilationOptions is null)
            {
                throw new ArgumentNullException(nameof(compilationOptions));
            }

            var git = new GitClient();
            var gitFile = GitRepositoryProvider.ParseUrl(githubUrl);
            var tempDirectory = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
            var directoryInfo = Directory.CreateDirectory(tempDirectory);
            git.Clone(
                gitFile.RepositoryUrl,
                directoryInfo.FullName,
                GitClient.CloneFlags.Shallow,
                gitFile.Branch);
            var slnFileInfo = new FileInfo(Path.Combine(directoryInfo.FullName, gitFile.Path));
            return CreateSolution(slnFileInfo, parseOptions, compilationOptions, metadataReferences);
        }

        /// <summary>
        /// Create a <see cref="Solution"/> for <paramref name="code"/>
        /// Each unique namespace in <paramref name="code"/> is added as a project.
        /// </summary>
        /// <param name="code">The code to create the solution from.</param>
        /// <param name="settings">The <see cref="Settings"/>.</param>
        /// <returns>A <see cref="Solution"/>.</returns>
        public static Solution CreateSolution(string code, Settings? settings = null)
        {
            settings ??= Settings.Default;
            return CreateSolution(new[] { code }, settings.ParseOptions, settings.CompilationOptions, settings.MetadataReferences);
        }

        /// <summary>
        /// Create a <see cref="Solution"/> for <paramref name="code"/>
        /// Each unique namespace in <paramref name="code"/> is added as a project.
        /// </summary>
        /// <param name="code">The code to create the solution from.</param>
        /// <param name="settings">The <see cref="Settings"/>.</param>
        /// <returns>A <see cref="Solution"/>.</returns>
        public static Solution CreateSolution(IEnumerable<string> code, Settings? settings = null)
        {
            settings ??= Settings.Default;
            return CreateSolution(code, settings.ParseOptions, settings.CompilationOptions, settings.MetadataReferences);
        }

        /// <summary>
        /// Create a <see cref="Solution"/> for <paramref name="code"/>
        /// Each unique namespace in <paramref name="code"/> is added as a project.
        /// </summary>
        /// <param name="code">The code to create the solution from.</param>
        /// <param name="settings">The <see cref="Settings"/>.</param>
        /// <returns>A <see cref="Solution"/>.</returns>
        public static Solution CreateSolutionWithOneProject(string code, Settings? settings = null)
        {
            settings ??= Settings.Default;
            return CreateSolutionWithOneProject(new[] { code }, settings.ParseOptions, settings.CompilationOptions, settings.MetadataReferences);
        }

        /// <summary>
        /// Create a <see cref="Solution"/> for <paramref name="code"/>
        /// Each unique namespace in <paramref name="code"/> is added as a project.
        /// </summary>
        /// <param name="code">The code to create the solution from.</param>
        /// <param name="settings">The <see cref="Settings"/>.</param>
        /// <returns>A <see cref="Solution"/>.</returns>
        public static Solution CreateSolutionWithOneProject(IEnumerable<string> code, Settings? settings = null)
        {
            settings ??= Settings.Default;
            return CreateSolutionWithOneProject(code, settings.ParseOptions, settings.CompilationOptions, settings.MetadataReferences);
        }

        /// <summary>
        /// Create a <see cref="Solution"/> for <paramref name="code"/>
        /// Each unique namespace in <paramref name="code"/> is added as a project.
        /// </summary>
        /// <param name="code">The code to create the solution from.</param>
        /// <param name="settings">The <see cref="Settings"/>.</param>
        /// <returns>A <see cref="Solution"/>.</returns>
        public static Solution CreateSolution(FileInfo code, Settings? settings = null)
        {
            settings ??= Settings.Default;
            return CreateSolution(code, settings.ParseOptions, settings.CompilationOptions, settings.MetadataReferences);
        }

        /// <summary>
        /// Create a Solution by cloning a remote git repository.
        /// </summary>
        /// <param name="githubUrl">
        /// The url to the code to create the solution from.
        /// Can be a .cs, .csproj or .sln file.
        /// Sample URL: https://github.com/GuOrg/Gu.Roslyn.Asserts/blob/master/Gu.Roslyn.Asserts.sln.
        /// </param>
        /// <param name="settings">The <see cref="Settings"/>.</param>
        /// <returns>>A <see cref="Solution"/>.</returns>
        public static Solution CreateSolution(Uri githubUrl, Settings? settings = null)
        {
            if (githubUrl is null)
            {
                throw new ArgumentNullException(nameof(githubUrl));
            }

            settings ??= Settings.Default;
            return CreateSolution(githubUrl, settings.ParseOptions, settings.CompilationOptions, settings.MetadataReferences);
        }

        /// <summary>
        /// Create default compilation options for <paramref name="specificDiagnosticOptions"/>
        /// AD0001 is reported as error.
        /// </summary>
        /// <param name="specificDiagnosticOptions">A list of ids and <see cref="ReportDiagnostic"/>.</param>
        /// <returns>An instance of <see cref="CSharpCompilationOptions"/>.</returns>
        public static CSharpCompilationOptions DefaultCompilationOptions(IEnumerable<KeyValuePair<string, ReportDiagnostic>>? specificDiagnosticOptions = null)
        {
            // All arguments needed here to disambiguate. There was a breaking change between 3.0 and 3.3.1
            return new CSharpCompilationOptions(
                outputKind: OutputKind.DynamicallyLinkedLibrary,
                reportSuppressedDiagnostics: false,
                moduleName: null,
                mainTypeName: null,
                scriptClassName: null,
                usings: null,
                optimizationLevel: OptimizationLevel.Debug,
                checkOverflow: false,
                allowUnsafe: true,
                cryptoKeyContainer: null,
                cryptoKeyFile: null,
                cryptoPublicKey: default,
                delaySign: null,
                platform: Platform.AnyCpu,
                generalDiagnosticOption: ReportDiagnostic.Default,
                warningLevel: 4,
                specificDiagnosticOptions: specificDiagnosticOptions,
                concurrentBuild: true,
                deterministic: false,
                xmlReferenceResolver: null,
                sourceReferenceResolver: null,
                metadataReferenceResolver: null,
                assemblyIdentityComparer: null,
                strongNameProvider: null,
                publicSign: false,
                metadataImportOptions: MetadataImportOptions.Public,
                nullableContextOptions: NullableContextOptions.Enable);
        }

        /// <summary>
        /// Searches parent directories for <paramref name="fileName"/>.
        /// </summary>
        /// <param name="directory">The directory to start in.</param>
        /// <param name="fileName">Ex Foo.csproj.</param>
        /// <param name="result">The <see cref="File"/> if found.</param>
        /// <returns>A value indicating if a file was found.</returns>
        public static bool TryFindFileInParentDirectory(DirectoryInfo directory, string fileName, [NotNullWhen(true)] out FileInfo? result)
        {
            if (directory is null)
            {
                throw new ArgumentNullException(nameof(directory));
            }

            if (fileName is null)
            {
                throw new ArgumentNullException(nameof(fileName));
            }

            if (directory.EnumerateFiles(fileName).TryFirst(out result))
            {
#pragma warning disable CS8762 // Parameter must have a non-null value when exiting in some condition.
                return true;
#pragma warning restore CS8762 // Parameter must have a non-null value when exiting in some condition.
            }

            if (directory.Parent != null)
            {
                return TryFindFileInParentDirectory(directory.Parent, fileName, out result);
            }

            result = null;
            return false;
        }

        /// <summary>
        /// Create a <see cref="Solution"/> for <paramref name="diagnosticsAndSources"/>.
        /// </summary>
        /// <param name="diagnosticsAndSources">The code to create the solution from with .</param>
        /// <param name="analyzer">The <see cref="DiagnosticAnalyzer"/> to check <paramref name="diagnosticsAndSources"/> with.</param>
        /// <param name="settings">The <see cref="Settings"/>.</param>
        /// <returns>A <see cref="Solution"/>.</returns>
        internal static Solution CreateSolution(DiagnosticAnalyzer analyzer, DiagnosticsAndSources diagnosticsAndSources, Settings settings)
        {
            return CreateSolution(
                diagnosticsAndSources.Code,
                settings.ParseOptions,
                settings.CompilationOptions.WithSpecific(analyzer.SupportedDiagnostics, diagnosticsAndSources.ExpectedDiagnostics),
                settings.MetadataReferences);
        }

        /// <summary>
        /// Create a <see cref="Solution"/> for <paramref name="code"/>.
        /// </summary>
        /// <param name="code">The code to create the solution from with.</param>
        /// <param name="analyzer">The <see cref="DiagnosticAnalyzer"/> to check <paramref name="code"/> with.</param>
        /// <param name="settings">The <see cref="Settings"/>.</param>
        /// <returns>A <see cref="Solution"/>.</returns>
        internal static Solution CreateSolution(DiagnosticAnalyzer analyzer, IEnumerable<string> code,  Settings settings)
        {
            return CreateSolution(
                code,
                settings.ParseOptions,
                settings.CompilationOptions.WithWarningOrError(analyzer.SupportedDiagnostics),
                settings.MetadataReferences);
        }

        /// <summary>
        /// Create a <see cref="Solution"/> for <paramref name="code"/>.
        /// </summary>
        /// <param name="code">The code to create the solution from with.</param>
        /// <param name="analyzer">The <see cref="DiagnosticAnalyzer"/> to check <paramref name="code"/> with.</param>
        /// <param name="descriptor">The <see cref="DiagnosticDescriptor"/> to check <paramref name="code"/> with.</param>
        /// <param name="settings">The <see cref="Settings"/>.</param>
        /// <returns>A <see cref="Solution"/>.</returns>
        internal static Solution CreateSolution(DiagnosticAnalyzer analyzer, DiagnosticDescriptor descriptor, IEnumerable<string> code, Settings settings)
        {
            return CreateSolution(
                code,
                settings.ParseOptions,
                settings.CompilationOptions.WithSpecific(analyzer.SupportedDiagnostics, descriptor),
                settings.MetadataReferences);
        }

        /// <summary>
        /// Create a <see cref="Solution"/> for <paramref name="code"/>.
        /// </summary>
        /// <param name="code">The code to create the solution from with.</param>
        /// <param name="analyzer">The <see cref="DiagnosticAnalyzer"/> to check <paramref name="code"/> with.</param>
        /// <param name="settings">The <see cref="Settings"/>.</param>
        /// <returns>A <see cref="Solution"/>.</returns>
        internal static Solution CreateSolution(FileInfo code, DiagnosticAnalyzer analyzer, Settings settings)
        {
            return CreateSolution(
                code,
                settings.ParseOptions,
                settings.CompilationOptions.WithWarningOrError(analyzer.SupportedDiagnostics),
                settings.MetadataReferences);
        }

        /// <summary>
        /// Create a <see cref="Solution"/> for <paramref name="code"/>.
        /// </summary>
        /// <param name="code">The code to create the solution from with.</param>
        /// <param name="analyzer">The <see cref="DiagnosticAnalyzer"/> to check <paramref name="code"/> with.</param>
        /// <param name="descriptor">The <see cref="DiagnosticDescriptor"/> to check <paramref name="code"/> with.</param>
        /// <param name="settings">The <see cref="Settings"/>.</param>
        /// <returns>A <see cref="Solution"/>.</returns>
        internal static Solution CreateSolution(FileInfo code, DiagnosticAnalyzer analyzer, DiagnosticDescriptor descriptor, Settings settings)
        {
            return CreateSolution(
                code,
                settings.ParseOptions,
                settings.CompilationOptions.WithSpecific(analyzer.SupportedDiagnostics, descriptor),
                settings.MetadataReferences);
        }
    }
}
