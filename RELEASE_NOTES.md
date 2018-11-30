#### 2.6.3
* FEATURE: MetadataReferences.CreateBinary()
* BREAKING: Refactor overloads and use optional parameters.

#### 2.6.2
* MetadataReferences.Transitive(type) handle generic types.
* Use reference assemblies.

#### 2.6.0
* Compile valid code once.
* Use DiagnosticDescriptor in Valid. Old API made [Obsolete]
* Better error when two descriptors have the same ID.
* Use LanguageVersion.Latest.

#### 2.5.0
* BREAKING: Don't throw test framework exceptions.
* BREAKING: Require no compiler errors in AnalyzerAssert.Valid.
* FEATURE: CodeAssert better message when differ at end.
* FEATURE: Better message when message differs.

#### 2.4.2
* FEATURE: AnalyzerAssert.Ast.
* FEATURE: AstWriter.

#### 2.4.0
* BREAKING: Mark async API obsolete.
* FEATURE: Create sln from github url.
* FEATURE: Support testing refactorings.
* FEATURE: Multitarget net46 & netstandard2.0
* BREAKING: Probably changed some overload.

#### 2.3.1
* BUGFIX: ExpectedDiagnostic.Create without path should nolt throw.

#### 2.3.1
* BUGFIX: FindExpression

#### 2.3.0
* FEATURE: Limited support for resolving references when parsing project & sln files.
* FEATURE: Add more metadata when parsing files.
* FEATURE: Expose fix methods.
* BUGFIX: The project already transitively references the target project. #53
* BUGFIX: Apply fixes one-by-one in document order. #51
* FEATURE: More overloads to CodeFix & FixAll. #50

#### 2.2.9
* BUGFIX: Find with whitespace.

#### 2.2.8
* FEATURE: Allow code to contain node code in Find methods.

#### 2.2.7
* BUGFIXES: TryFindInvocation when argument is invocation.

#### 2.2.6
* BUGFIXES: TryFind methods.

#### 2.2.5
* FEATURE: Make more Analyze methods public.

#### 2.2.4
* BUGFIX NoFix handles expected diagnostic with error indicated.

#### 2.2.3
* BREAKING: NoFix is stricter now, requires no registered code action
* BUGFIX: Handle suppressing one of the diagnostics the analyzer supports.

#### 2.2.2
* FEATURE: Reuse shared workspace when creating solutions.

#### 2.2.1
* BUGFIX: handle many analyzers for same diagnostic.

#### 2.2.0
* BUGFIX: handle expected diagnostic when analyzer supports many.
* BREAKING: Removed obsolete ErrorMessage

#### 2.1.1
* BUGFIX: remove check for single diagnostic.

#### 2.1.0
* FEATURE: handle error indicated in code with expected diagnostic
* FEATURE: AnalyzerAssert.CodeFix<TAnalyzer, TCodeFix>with expected diagnostic

#### 2.0.0
Use this version for Microsoft.CodeAnalysis.CSharp 2.x

#### 1.0.0
Use this version for Microsoft.CodeAnalysis.CSharp 1.x

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

