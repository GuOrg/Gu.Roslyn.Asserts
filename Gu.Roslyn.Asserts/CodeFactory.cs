namespace Gu.Roslyn.Asserts
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Text;
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
        public static readonly AdhocWorkspace Workspace = new AdhocWorkspace();

        /// <summary>
        /// An empty solution.
        /// </summary>
        public static readonly Solution EmptySolution = Workspace.CurrentSolution;

        /// <summary>
        /// Creates a solution for <paramref name="code"/>.
        /// </summary>
        /// <param name="code">The sources as strings.</param>
        /// <param name="metadataReferences">The <see cref="MetadataReference"/> to use when compiling.</param>
        /// <returns>A list with diagnostics per document.</returns>
        public static Solution CreateSolution(string code, params MetadataReference[] metadataReferences)
        {
            return CreateSolution(new[] { code }, (IEnumerable<MetadataReference>)metadataReferences);
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
            return CreateSolution(code, (IEnumerable<MetadataReference>)metadataReferences);
        }

        /// <summary>
        /// Creates a solution for <paramref name="code"/>.
        /// </summary>
        /// <param name="code">The sources as strings.</param>
        /// <param name="metadataReferences">The <see cref="MetadataReference"/> to use when compiling.</param>
        /// <returns>A list with diagnostics per document.</returns>
        public static Solution CreateSolution(string code, IEnumerable<MetadataReference> metadataReferences)
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
        public static Solution CreateSolution(IReadOnlyList<string> code, IEnumerable<MetadataReference> metadataReferences)
        {
            return CreateSolution(code, DefaultCompilationOptions((IReadOnlyList<DiagnosticAnalyzer>)null, null), metadataReferences);
        }

        /// <summary>
        /// Create a <see cref="Solution"/> for <paramref name="code"/>.
        /// </summary>
        /// <param name="code">The code to create the solution from.</param>
        /// <param name="compilationOptions">The <see cref="CSharpCompilationOptions"/>.</param>
        /// <param name="metadataReferences">The metadata references.</param>
        /// <returns>A <see cref="Solution"/>.</returns>
        public static Solution CreateSolution(string code, CSharpCompilationOptions compilationOptions, IEnumerable<MetadataReference> metadataReferences = null)
        {
            return CreateSolution(new[] { code }, compilationOptions, metadataReferences);
        }

        /// <summary>
        /// Create a Solution with diagnostic options set to warning for all supported diagnostics in <paramref name="analyzers"/>.
        /// </summary>
        /// <param name="code">The code to create the solution from.</param>
        /// <param name="analyzers">The analyzers to add diagnostic options for.</param>
        /// <param name="metadataReferences">The metadata references.</param>
        /// <returns>A <see cref="Solution"/>.</returns>
        public static Solution CreateSolution(string code, IReadOnlyList<DiagnosticAnalyzer> analyzers, IEnumerable<MetadataReference> metadataReferences = null)
        {
            return CreateSolution(new[] { code }, analyzers, metadataReferences);
        }

        /// <summary>
        /// Create a Solution with diagnostic options set to warning for all supported diagnostics in <paramref name="analyzers"/>
        /// Each unique namespace in <paramref name="code"/> is added as a project.
        /// </summary>
        /// <param name="code">The code to create the solution from.</param>
        /// <param name="analyzers">The analyzers to add diagnostic options for.</param>
        /// <param name="metadataReferences">The metadata references.</param>
        /// <returns>A <see cref="Solution"/>.</returns>
        public static Solution CreateSolution(IReadOnlyList<string> code, IReadOnlyList<DiagnosticAnalyzer> analyzers, IEnumerable<MetadataReference> metadataReferences = null)
        {
            return CreateSolution(code, DefaultCompilationOptions(analyzers, null), metadataReferences);
        }

        /// <summary>
        /// Create a <see cref="Solution"/> for <paramref name="code"/>
        /// Each unique namespace in <paramref name="code"/> is added as a project.
        /// </summary>
        /// <param name="code">The code to create the solution from.</param>
        /// <param name="compilationOptions">The <see cref="CSharpCompilationOptions"/>.</param>
        /// <param name="metadataReferences">The metadata references.</param>
        /// <param name="languageVersion">The <see cref="LanguageVersion"/>.</param>
        /// <returns>A <see cref="Solution"/>.</returns>
        public static Solution CreateSolution(IEnumerable<string> code, CSharpCompilationOptions compilationOptions, IEnumerable<MetadataReference> metadataReferences = null, LanguageVersion languageVersion = LanguageVersion.Latest)
        {
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
                                            .WithParseOptions(CSharpParseOptions.Default.WithLanguageVersion(languageVersion));
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
        /// Create a Solution with diagnostic options set to warning for all supported diagnostics in <paramref name="analyzers"/>.
        /// </summary>
        /// <param name="code">The code to create the solution from.</param>
        /// <param name="analyzers">The analyzers to add diagnostic options for.</param>
        /// <param name="metadataReferences">The metadata references.</param>
        /// <returns>A <see cref="Solution"/>.</returns>
        public static Solution CreateSolutionWithOneProject(string code, IReadOnlyList<DiagnosticAnalyzer> analyzers, IEnumerable<MetadataReference> metadataReferences = null)
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
        /// <returns>A <see cref="Solution"/>.</returns>
        public static Solution CreateSolutionWithOneProject(IReadOnlyList<string> code, IReadOnlyList<DiagnosticAnalyzer> analyzers, IEnumerable<MetadataReference> metadataReferences = null)
        {
            return CreateSolutionWithOneProject(code, DefaultCompilationOptions(analyzers, null), metadataReferences);
        }

        /// <summary>
        /// Create a <see cref="Solution"/> for <paramref name="code"/>.
        /// </summary>
        /// <param name="code">The code to create the solution from.</param>
        /// <param name="compilationOptions">The <see cref="CSharpCompilationOptions"/>.</param>
        /// <param name="metadataReferences">The metadata references.</param>
        /// <returns>A <see cref="Solution"/>.</returns>
        public static Solution CreateSolutionWithOneProject(string code, CSharpCompilationOptions compilationOptions, IEnumerable<MetadataReference> metadataReferences = null)
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
        /// <returns>A <see cref="Solution"/>.</returns>
        public static Solution CreateSolutionWithOneProject(IEnumerable<string> code, CSharpCompilationOptions compilationOptions, IEnumerable<MetadataReference> metadataReferences = null)
        {
            var projectInfo = GetProjectInfo();
            return EmptySolution.AddProject(projectInfo);

            ProjectInfo GetProjectInfo()
            {
                string projectName = null;
                foreach (var doc in code)
                {
                    if (projectName == null)
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
                                        SourceText.From(x, (Encoding)null, SourceHashAlgorithm.Sha1),
                                        VersionStamp.Default)));
                        }));
            }
        }

        /// <summary>
        /// Create default compilation options for <paramref name="analyzer"/>
        /// AD0001 is reported as error.
        /// </summary>
        /// <param name="analyzer">The analyzer to report warning or error for.</param>
        /// <param name="suppressed">The analyzer IDs to suppress.</param>
        /// <returns>An instance of <see cref="CSharpCompilationOptions"/>.</returns>
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
        /// <returns>An instance of <see cref="CSharpCompilationOptions"/>.</returns>
        public static CSharpCompilationOptions DefaultCompilationOptions(IReadOnlyList<DiagnosticAnalyzer> analyzers, IEnumerable<string> suppressed = null)
        {
            return DefaultCompilationOptions(CreateSpecificDiagnosticOptions(analyzers, suppressed));
        }

        /// <summary>
        /// Create default compilation options for <paramref name="descriptor"/>
        /// AD0001 is reported as error.
        /// </summary>
        /// <param name="descriptor">The analyzers to report warning or error for.</param>
        /// <param name="suppressed">The analyzer IDs to suppress.</param>
        /// <returns>An instance of <see cref="CSharpCompilationOptions"/>.</returns>
        public static CSharpCompilationOptions DefaultCompilationOptions(DiagnosticDescriptor descriptor, IEnumerable<string> suppressed = null)
        {
            return DefaultCompilationOptions(CreateSpecificDiagnosticOptions(new[] { descriptor }, suppressed));
        }

        /// <summary>
        /// Create default compilation options for <paramref name="analyzer"/>
        /// AD0001 is reported as error.
        /// </summary>
        /// <param name="analyzer">The analyzers to report warning or error for.</param>
        /// <param name="expectedDiagnostic">The diagnostics to check for.</param>
        /// <param name="suppressWarnings">The analyzer IDs to suppress.</param>
        /// <returns>An instance of <see cref="CSharpCompilationOptions"/>.</returns>
        public static CSharpCompilationOptions DefaultCompilationOptions(DiagnosticAnalyzer analyzer, ExpectedDiagnostic expectedDiagnostic, IEnumerable<string> suppressWarnings)
        {
            return DefaultCompilationOptions(analyzer, expectedDiagnostic.Id, suppressWarnings);
        }

        /// <summary>
        /// Create default compilation options for <paramref name="analyzer"/>
        /// AD0001 is reported as error.
        /// </summary>
        /// <param name="analyzer">The analyzers to report warning or error for.</param>
        /// <param name="descriptor">The diagnostics to check for.</param>
        /// <param name="suppressWarnings">The analyzer IDs to suppress.</param>
        /// <returns>An instance of <see cref="CSharpCompilationOptions"/>.</returns>
        public static CSharpCompilationOptions DefaultCompilationOptions(DiagnosticAnalyzer analyzer, DiagnosticDescriptor descriptor, IEnumerable<string> suppressWarnings)
        {
            return DefaultCompilationOptions(analyzer, descriptor.Id, suppressWarnings);
        }

        /// <summary>
        /// Create default compilation options for <paramref name="analyzer"/>
        /// AD0001 is reported as error.
        /// </summary>
        /// <param name="analyzer">The analyzers to report warning or error for.</param>
        /// <param name="expectedDiagnostics">The diagnostics to check for.</param>
        /// <param name="suppressWarnings">The analyzer IDs to suppress.</param>
        /// <returns>An instance of <see cref="CSharpCompilationOptions"/>.</returns>
        public static CSharpCompilationOptions DefaultCompilationOptions(DiagnosticAnalyzer analyzer, IReadOnlyList<ExpectedDiagnostic> expectedDiagnostics, IEnumerable<string> suppressWarnings)
        {
            RoslynAssert.VerifyAnalyzerSupportsDiagnostics(analyzer, expectedDiagnostics);
            var descriptors = analyzer.SupportedDiagnostics.Where(x => expectedDiagnostics.Any(e => e.Id == x.Id)).ToArray();
            suppressWarnings ??= Enumerable.Empty<string>();
            return DefaultCompilationOptions(descriptors, suppressWarnings.Concat(analyzer.SupportedDiagnostics.Where(x => expectedDiagnostics.All(e => e.Id != x.Id)).Select(x => x.Id)));
        }

        /// <summary>
        /// Create default compilation options for <paramref name="analyzer"/>
        /// AD0001 is reported as error.
        /// </summary>
        /// <param name="analyzer">The analyzers to report warning or error for.</param>
        /// <param name="descriptors">The diagnostics to check for.</param>
        /// <param name="suppressWarnings">The analyzer IDs to suppress.</param>
        /// <returns>An instance of <see cref="CSharpCompilationOptions"/>.</returns>
        public static CSharpCompilationOptions DefaultCompilationOptions(DiagnosticAnalyzer analyzer, IReadOnlyList<DiagnosticDescriptor> descriptors, IEnumerable<string> suppressWarnings)
        {
            RoslynAssert.VerifyAnalyzerSupportsDiagnostics(analyzer, descriptors);
            suppressWarnings ??= Enumerable.Empty<string>();
            return DefaultCompilationOptions(descriptors, suppressWarnings.Concat(analyzer.SupportedDiagnostics.Where(x => descriptors.All(e => e.Id != x.Id)).Select(x => x.Id)));
        }

        /// <summary>
        /// Create default compilation options for <paramref name="descriptors"/>
        /// AD0001 is reported as error.
        /// </summary>
        /// <param name="descriptors">The analyzers to report warning or error for.</param>
        /// <param name="suppressed">The analyzer IDs to suppress.</param>
        /// <returns>An instance of <see cref="CSharpCompilationOptions"/>.</returns>
        public static CSharpCompilationOptions DefaultCompilationOptions(IReadOnlyList<DiagnosticDescriptor> descriptors, IEnumerable<string> suppressed = null)
        {
            return DefaultCompilationOptions(CreateSpecificDiagnosticOptions(descriptors, suppressed));
        }

        /// <summary>
        /// Create default compilation options for <paramref name="specificDiagnosticOptions"/>
        /// AD0001 is reported as error.
        /// </summary>
        /// <param name="specificDiagnosticOptions">A list of ids and <see cref="ReportDiagnostic"/>.</param>
        /// <returns>An instance of <see cref="CSharpCompilationOptions"/>.</returns>
        public static CSharpCompilationOptions DefaultCompilationOptions(IEnumerable<KeyValuePair<string, ReportDiagnostic>> specificDiagnosticOptions)
        {
            return new CSharpCompilationOptions(
                outputKind: OutputKind.DynamicallyLinkedLibrary,
                reportSuppressedDiagnostics: false,
                moduleName: null,
                mainTypeName: null,
                scriptClassName: null,
                usings: null,
                optimizationLevel: OptimizationLevel.Debug,
                checkOverflow: false,
                allowUnsafe: false,
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
                metadataImportOptions: MetadataImportOptions.Public);
        }

        /// <summary>
        /// Create a Solution.
        /// </summary>
        /// <param name="code">
        /// The code to create the solution from.
        /// Can be a .cs, .csproj or .sln file.
        /// </param>
        /// <param name="metadataReferences">The metadata references.</param>
        /// <returns>A <see cref="Solution"/>.</returns>
        public static Solution CreateSolution(FileInfo code, IEnumerable<MetadataReference> metadataReferences = null)
        {
            return CreateSolution(
                code,
                DefaultCompilationOptions((IReadOnlyList<DiagnosticAnalyzer>)null, null),
                metadataReferences);
        }

        /// <summary>
        /// Create a Solution with diagnostic options set to warning for all supported diagnostics in <paramref name="analyzers"/>.
        /// </summary>
        /// <param name="code">
        /// The code to create the solution from.
        /// Can be a .cs, .csproj or .sln file.
        /// </param>
        /// <param name="analyzers">The analyzers to add diagnostic options for.</param>
        /// <param name="metadataReferences">The metadata references.</param>
        /// <returns>A <see cref="Solution"/>.</returns>
        public static Solution CreateSolution(FileInfo code, IReadOnlyList<DiagnosticAnalyzer> analyzers, IEnumerable<MetadataReference> metadataReferences = null)
        {
            var compilationOptions = DefaultCompilationOptions(analyzers, null);
            return CreateSolution(code, compilationOptions, metadataReferences);
        }

        /// <summary>
        /// Create a Solution with diagnostic options set to warning for all supported diagnostics in <paramref name="analyzer"/>.
        /// </summary>
        /// <param name="code">
        /// The code to create the solution from.
        /// Can be a .cs, .csproj or .sln file.
        /// </param>
        /// <param name="analyzer">The analyzer to add diagnostic options for.</param>
        /// <param name="expectedDiagnostic">The <see cref="ExpectedDiagnostic"/> with information about the expected <see cref="Diagnostic"/>. If <paramref name="analyzer"/> supports more than one <see cref="DiagnosticDescriptor.Id"/> this must be provided.</param>
        /// <param name="suppressWarnings">The suppressed diagnostics.</param>
        /// <param name="metadataReferences">The metadata references.</param>
        /// <returns>A <see cref="Solution"/>.</returns>
        public static Solution CreateSolution(FileInfo code, DiagnosticAnalyzer analyzer, ExpectedDiagnostic expectedDiagnostic, IEnumerable<string> suppressWarnings = null, IEnumerable<MetadataReference> metadataReferences = null)
        {
            return CreateSolution(code, DefaultCompilationOptions(analyzer, expectedDiagnostic, suppressWarnings), metadataReferences);
        }

        /// <summary>
        /// Create a Solution.
        /// </summary>
        /// <param name="code">
        /// The code to create the solution from.
        /// Can be a .cs, .csproj or .sln file.
        /// </param>
        /// <param name="compilationOptions">The <see cref="CompilationOptions"/> to use when compiling.</param>
        /// <param name="metadataReferences">The metadata references.</param>
        /// <returns>A <see cref="Solution"/>.</returns>
        public static Solution CreateSolution(FileInfo code, CSharpCompilationOptions compilationOptions, IEnumerable<MetadataReference> metadataReferences = null)
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
                var solution = EmptySolution;
                var solutionInfo = SolutionFile.ParseInfo(code);
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
        /// Create a Solution by cloning a remote git repository.
        /// </summary>
        /// <param name="githubUrl">
        /// The url to the code to create the solution from.
        /// Can be a .cs, .csproj or .sln file.
        /// Sample URL: https://github.com/GuOrg/Gu.Roslyn.Asserts/blob/master/Gu.Roslyn.Asserts.sln.
        /// </param>
        /// <param name="analyzers">The analyzers to add diagnostic options for.</param>
        /// <param name="metadataReferences">The metadata references.</param>
        /// <returns>>A <see cref="Solution"/>.</returns>
        public static Solution CreateSolution(Uri githubUrl, IReadOnlyList<DiagnosticAnalyzer> analyzers, IEnumerable<MetadataReference> metadataReferences = null)
        {
            var git = new GitClient();
            var gitFile = GitRepositoryProvider.ParseUrl(githubUrl);
            string tempDirectory = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
            var directoryInfo = Directory.CreateDirectory(tempDirectory);
            git.Clone(
                gitFile.RepositoryUrl,
                directoryInfo.FullName,
                GitClient.CloneFlags.Shallow,
                gitFile.Branch);
            var slnFileInfo = new FileInfo(Path.Combine(directoryInfo.FullName, gitFile.Path));
            return CreateSolution(slnFileInfo, analyzers, metadataReferences);
        }

        /// <summary>
        /// Searches parent directories for <paramref name="fileName"/>.
        /// </summary>
        /// <param name="directory">The directory to start in.</param>
        /// <param name="fileName">Ex Foo.csproj.</param>
        /// <param name="result">The <see cref="File"/> if found.</param>
        /// <returns>A value indicating if a file was found.</returns>
        public static bool TryFindFileInParentDirectory(DirectoryInfo directory, string fileName, out FileInfo result)
        {
            if (directory.EnumerateFiles(fileName).TryFirst(out result))
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
                        throw new ArgumentOutOfRangeException();
                }
            }
        }

        /// <summary>
        /// Create a <see cref="Solution"/> for <paramref name="diagnosticsAndSources"/>.
        /// </summary>
        /// <param name="diagnosticsAndSources">The code to create the solution from with .</param>
        /// <param name="analyzer">The <see cref="DiagnosticAnalyzer"/> to check <paramref name="diagnosticsAndSources"/> with.</param>
        /// <param name="compilationOptions">The <see cref="CSharpCompilationOptions"/>.</param>
        /// <param name="suppressWarnings">The explicitly suppressed diagnostics.</param>
        /// <param name="metadataReferences">The metadata references.</param>
        /// <returns>A <see cref="Solution"/>.</returns>
        internal static Solution CreateSolution(DiagnosticsAndSources diagnosticsAndSources, DiagnosticAnalyzer analyzer, CSharpCompilationOptions compilationOptions, IEnumerable<string> suppressWarnings, IEnumerable<MetadataReference> metadataReferences)
        {
            return CreateSolution(
                diagnosticsAndSources.Code,
                compilationOptions ?? DefaultCompilationOptions(analyzer, diagnosticsAndSources.ExpectedDiagnostics, suppressWarnings),
                metadataReferences);
        }

        private static CSharpCompilationOptions DefaultCompilationOptions(DiagnosticAnalyzer analyzer, string expectedId, IEnumerable<string> suppressWarnings)
        {
            RoslynAssert.VerifyAnalyzerSupportsDiagnostic(analyzer, expectedId);
            var descriptor = analyzer.SupportedDiagnostics.Single(x => x.Id == expectedId);
            suppressWarnings ??= Enumerable.Empty<string>();
            return DefaultCompilationOptions(descriptor, suppressWarnings.Concat(analyzer.SupportedDiagnostics.Select(x => x.Id).Where(x => x != expectedId)));
        }
    }
}
