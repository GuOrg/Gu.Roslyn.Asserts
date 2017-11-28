#### 1.1.0
* FEATURE: handle error indicated in code with expected diagnostic
* FEATURE: AnalyzerAssert.CodeFix<TAnalyzer, TCodeFix>with expected diagnostic

#### 0.4.0
* BUGFIX: Better heuristics for determining if a csproj is new format
* FEATURE: CodeFactory.CreateSolutionWithOneProject
* FEATURE: CodeComparer
* FEATURE ExpectedDiagnostic.
* BREAKING: Change signature of AnalyzerAssert.DiagnosticsWithMetadataAsync
* BREAKING: Move DiagnosticsAndSources to separate class.

#### 0.3.6
* BUGFIX: Parse filenames with error indicators.
* FEATURE Benchmark API.

#### 0.3.5
* BUGFIX: FixAll when multiple projects.

#### 0.3.4
* FEATURE: Keep format for untouched parts in code fix.

#### 0.3.3
* FEATURE: FixAllInDocument
* FEATURE: FindProjectFile & FindSolutionFile

#### 0.3.2
* FEATURE: Figure out intra project dependencies

#### 0.3.1
* FEATURE: Add transitive dependencies.

#### 0.3.0
* BREAKING: Remove obsolete NoDiagnostics
* FEATURE: AnalyzerAssert.SuppressedDiagnostics
* FEATURE: overloads with CSharpCompilationOptions.
* FEATURE: Shallower stacks in exceptions.

