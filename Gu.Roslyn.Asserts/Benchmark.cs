namespace Gu.Roslyn.Asserts
{
    using System;
    using System.Collections.Generic;
    using System.Collections.Immutable;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using Microsoft.CodeAnalysis.Diagnostics;

    /// <summary>
    /// A benchmark runner for a <see cref="DiagnosticAnalyzer"/>
    /// </summary>
    public class Benchmark
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Benchmark"/> class.
        /// </summary>
        /// <param name="analyzer">The <see cref="DiagnosticAnalyzer"/></param>
        /// <param name="contextAndActions">The contexts and actions to invoke in the benchmark.</param>
        public Benchmark(DiagnosticAnalyzer analyzer, IReadOnlyList<ContextAndAction> contextAndActions)
        {
            this.Analyzer = analyzer;
            this.ContextAndActions = contextAndActions;
        }

        /// <summary>
        /// Gets the analyzer to run benchmarks for.
        /// </summary>
        public DiagnosticAnalyzer Analyzer { get; }

        /// <summary>
        /// Gets the contexts to invoke the analyzer on.
        /// </summary>
        public IReadOnlyList<ContextAndAction> ContextAndActions { get; }

        /// <summary>
        /// Creates a new instance of the <see cref="Benchmark"/> class.
        /// </summary>
        public static Benchmark Create(Solution solution, DiagnosticAnalyzer analyzer) => CreateAsync(solution, analyzer).GetAwaiter().GetResult();

        /// <summary>
        /// Creates a new instance of the <see cref="Benchmark"/> class.
        /// </summary>
        public static Benchmark Create(Project project, DiagnosticAnalyzer analyzer) => CreateAsync(project, analyzer).GetAwaiter().GetResult();

        /// <summary>
        /// Creates a new instance of the <see cref="Benchmark"/> class.
        /// </summary>
        public static async Task<Benchmark> CreateAsync(Solution solution, DiagnosticAnalyzer analyzer)
        {
            var context = new BenchmarkAnalysisContext(analyzer);
            var contextAndActions = await Walker.GetContextAndActionsAsync(solution, context.Actions);
            return new Benchmark(analyzer, contextAndActions);
        }

        /// <summary>
        /// Creates a new instance of the <see cref="Benchmark"/> class.
        /// </summary>
        public static async Task<Benchmark> CreateAsync(Project project, DiagnosticAnalyzer analyzer)
        {
            var context = new BenchmarkAnalysisContext(analyzer);
            var contextAndActions = await Walker.GetContextAndActionsAsync(project, context.Actions);
            return new Benchmark(analyzer, contextAndActions);
        }

        /// <summary>
        /// Run the benchmark. This invokes all actions in <see cref="ContextAndActions"/>
        /// </summary>
        public void Run()
        {
            foreach (var contextAndAction in this.ContextAndActions)
            {
                contextAndAction.Action(contextAndAction.Context);
            }
        }

        /// <inheritdoc/>
        public override string ToString()
        {
            return $"{nameof(this.Analyzer)}: {this.Analyzer.GetType().Name}";
        }

        /// <summary>
        /// An instance where the analyzer registered and action.
        /// </summary>
        public class ContextAndAction
        {
            /// <summary>
            /// Initializes a new instance of the <see cref="ContextAndAction"/> class.
            /// </summary>
            public ContextAndAction(SyntaxNodeAnalysisContext context, Action<SyntaxNodeAnalysisContext> action)
            {
                this.Context = context;
                this.Action = action;
            }

            /// <summary>
            /// Gets the <see cref="SyntaxNodeAnalysisContext"/> to pass in when invoking <see cref="Action"/>
            /// </summary>
            public SyntaxNodeAnalysisContext Context { get; }

            /// <summary>
            /// Gets the action registered for <see cref="Context"/> by BenchmarkAnalysisContext.RegisterSyntaxNodeAction(action, syntaxKinds)"/>
            /// </summary>
            public Action<SyntaxNodeAnalysisContext> Action { get; }
        }

        private class Walker : CSharpSyntaxWalker
        {
            private static readonly Action<Diagnostic> ReportDiagnostic = _ => { };
            private static readonly Func<Diagnostic, bool> IsSupportedDiagnostic = _ => true;

            private readonly IReadOnlyDictionary<int, Action<SyntaxNodeAnalysisContext>> actions;
            private readonly List<ContextAndAction> contextAndActions = new List<ContextAndAction>();

            private SemanticModel semanticModel;
            private ISymbol symbol;

            private Walker(IReadOnlyDictionary<int, Action<SyntaxNodeAnalysisContext>> actions)
                : base(SyntaxWalkerDepth.Token)
            {
                this.actions = actions;
            }

            public static async Task<IReadOnlyList<ContextAndAction>> GetContextAndActionsAsync(Solution solution, IReadOnlyDictionary<int, Action<SyntaxNodeAnalysisContext>> contextActions)
            {
                var walker = new Walker(contextActions);
                foreach (var project in solution.Projects)
                {
                    foreach (var document in project.Documents)
                    {
                        walker.semanticModel = await document.GetSemanticModelAsync().ConfigureAwait(false);
                        if (document.TryGetSyntaxRoot(out var root))
                        {
                            walker.Visit(root);
                        }
                    }
                }

                return walker.contextAndActions;
            }

            public static async Task<IReadOnlyList<ContextAndAction>> GetContextAndActionsAsync(Project project, IReadOnlyDictionary<int, Action<SyntaxNodeAnalysisContext>> contextActions)
            {
                var walker = new Walker(contextActions);
                foreach (var document in project.Documents)
                {
                    walker.semanticModel = await document.GetSemanticModelAsync().ConfigureAwait(false);
                    if (document.TryGetSyntaxRoot(out var root))
                    {
                        walker.Visit(root);
                    }
                }

                return walker.contextAndActions;
            }

            public override void DefaultVisit(SyntaxNode node)
            {
                if (node is BaseFieldDeclarationSyntax field)
                {
                    foreach (var declarator in field.Declaration.Variables)
                    {
                        this.symbol = this.semanticModel.GetDeclaredSymbol(declarator, CancellationToken.None);
                        if (this.actions.TryGetValue(node.RawKind, out var action))
                        {
                            this.contextAndActions.Add(
                                new ContextAndAction(
                                    new SyntaxNodeAnalysisContext(
                                        node,
                                        this.symbol,
                                        this.semanticModel,
                                        null,
                                        ReportDiagnostic,
                                        IsSupportedDiagnostic,
                                        CancellationToken.None),
                                    action));
                        }

                        base.DefaultVisit(node);
                    }
                }
                else
                {
                    this.symbol = this.GetDeclaredSymbolOrDefault(node, CancellationToken.None) ?? this.symbol;
                    if (this.actions.TryGetValue(node.RawKind, out var action))
                    {
                        this.contextAndActions.Add(
                            new ContextAndAction(
                                new SyntaxNodeAnalysisContext(
                                    node,
                                    this.symbol,
                                    this.semanticModel,
                                    null,
                                    ReportDiagnostic,
                                    IsSupportedDiagnostic,
                                    CancellationToken.None),
                                action));
                    }

                    base.DefaultVisit(node);
                }
            }

            private ISymbol GetDeclaredSymbolOrDefault(SyntaxNode node, CancellationToken cancellationToken)
            {
                // http://source.roslyn.codeplex.com/#Microsoft.CodeAnalysis.CSharp/Compilation/CSharpSemanticModel.cs,4633
                switch (node)
                {
                    case AccessorDeclarationSyntax accessor:
                        return ModelExtensions.GetDeclaredSymbol(this.semanticModel, accessor, cancellationToken);
                    case BaseTypeDeclarationSyntax type:
                        return ModelExtensions.GetDeclaredSymbol(this.semanticModel, type, cancellationToken);
                    case QueryClauseSyntax clause:
                        return ModelExtensions.GetDeclaredSymbol(this.semanticModel, clause, cancellationToken);
                    case MemberDeclarationSyntax member:
                        return ModelExtensions.GetDeclaredSymbol(this.semanticModel, member, cancellationToken);
                }

                switch (node.RawKind)
                {
                    case 8830: // SyntaxKind.LocalFunctionStatement:
                        return this.semanticModel.GetDeclaredSymbol(node, cancellationToken);
                    case (int)SyntaxKind.LabeledStatement:
                        return this.semanticModel.GetDeclaredSymbol(node, cancellationToken);
                    case (int)SyntaxKind.CaseSwitchLabel:
                    case (int)SyntaxKind.DefaultSwitchLabel:
                        return this.semanticModel.GetDeclaredSymbol(node, cancellationToken);
                    case (int)SyntaxKind.AnonymousObjectCreationExpression:
                        return this.semanticModel.GetDeclaredSymbol(node, cancellationToken);
                    case (int)SyntaxKind.AnonymousObjectMemberDeclarator:
                        return this.semanticModel.GetDeclaredSymbol(node, cancellationToken);
                    case 8926: // SyntaxKind.TupleExpression:
                        return this.semanticModel.GetDeclaredSymbol(node, cancellationToken);
                    case (int)SyntaxKind.Argument:
                        return this.semanticModel.GetDeclaredSymbol(node, cancellationToken);
                    case (int)SyntaxKind.VariableDeclarator:
                        return this.semanticModel.GetDeclaredSymbol(node, cancellationToken);
                    case 8927: // SyntaxKind.SingleVariableDesignation:
                        return this.semanticModel.GetDeclaredSymbol(node, cancellationToken);
                    case 8925: // SyntaxKind.TupleElement:
                        return this.semanticModel.GetDeclaredSymbol(node, cancellationToken);
                    case (int)SyntaxKind.NamespaceDeclaration:
                        return this.semanticModel.GetDeclaredSymbol(node, cancellationToken);
                    case (int)SyntaxKind.Parameter:
                        return this.semanticModel.GetDeclaredSymbol(node, cancellationToken);
                    case (int)SyntaxKind.TypeParameter:
                        return this.semanticModel.GetDeclaredSymbol(node, cancellationToken);
                    case (int)SyntaxKind.UsingDirective:
                        var usingDirective = (UsingDirectiveSyntax)node;
                        if (usingDirective.Alias == null)
                        {
                            break;
                        }

                        return ModelExtensions.GetDeclaredSymbol(this.semanticModel, usingDirective, cancellationToken);
                    case (int)SyntaxKind.ForEachStatement:
                        return this.semanticModel.GetDeclaredSymbol(node, cancellationToken);
                    case (int)SyntaxKind.CatchDeclaration:
                        return this.semanticModel.GetDeclaredSymbol(node, cancellationToken);
                    case (int)SyntaxKind.JoinIntoClause:
                        return this.semanticModel.GetDeclaredSymbol(node, cancellationToken);
                    case (int)SyntaxKind.QueryContinuation:
                        return this.semanticModel.GetDeclaredSymbol(node, cancellationToken);
                }

                return null;
            }
        }

        private class BenchmarkAnalysisContext : AnalysisContext
        {
            private readonly Dictionary<int, Action<SyntaxNodeAnalysisContext>> actions = new Dictionary<int, Action<SyntaxNodeAnalysisContext>>();

            public BenchmarkAnalysisContext(DiagnosticAnalyzer analyzer)
            {
                analyzer.Initialize(this);
            }

            public IReadOnlyDictionary<int, Action<SyntaxNodeAnalysisContext>> Actions => this.actions;

            public override void EnableConcurrentExecution()
            {
            }

            public override void ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags analysisMode)
            {
            }

            public override void RegisterSyntaxNodeAction<TLanguageKindEnum>(Action<SyntaxNodeAnalysisContext> action, ImmutableArray<TLanguageKindEnum> syntaxKinds)
            {
                foreach (var kind in syntaxKinds)
                {
                    // Hack using GetHashCode here
                    this.actions.Add(kind.GetHashCode(), action);
                }
            }

            public override void RegisterCompilationStartAction(Action<CompilationStartAnalysisContext> action)
            {
                throw new NotImplementedException();
            }

            public override void RegisterCompilationAction(Action<CompilationAnalysisContext> action)
            {
                throw new NotImplementedException();
            }

            public override void RegisterSemanticModelAction(Action<SemanticModelAnalysisContext> action)
            {
                throw new NotImplementedException();
            }

            public override void RegisterSymbolAction(Action<SymbolAnalysisContext> action, ImmutableArray<SymbolKind> symbolKinds)
            {
                throw new NotImplementedException();
            }

            public override void RegisterCodeBlockStartAction<TLanguageKindEnum>(Action<CodeBlockStartAnalysisContext<TLanguageKindEnum>> action)
            {
                throw new NotImplementedException();
            }

            public override void RegisterCodeBlockAction(Action<CodeBlockAnalysisContext> action)
            {
                throw new NotImplementedException();
            }

            public override void RegisterSyntaxTreeAction(Action<SyntaxTreeAnalysisContext> action)
            {
                throw new NotImplementedException();
            }
        }
    }
}
