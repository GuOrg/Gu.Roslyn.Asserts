namespace Gu.Roslyn.Asserts.Tests
{
    using System.Collections.Immutable;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using Microsoft.CodeAnalysis.Diagnostics;

    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    internal class ThrowingAnalyzer : DiagnosticAnalyzer
    {
        public const string DiagnosticId = "Throws";

        internal static readonly DiagnosticDescriptor Descriptor = new DiagnosticDescriptor(
            id: DiagnosticId,
            title: "This analyzer throws an NRE.",
            messageFormat: "This analyzer throws an NRE.",
            category: "This analyzer throws an NRE.",
            defaultSeverity: DiagnosticSeverity.Warning,
            isEnabledByDefault: true);

        /// <inheritdoc/>
        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get; } = ImmutableArray.Create(Descriptor);

        public override void Initialize(AnalysisContext context)
        {
            context.RegisterSyntaxNodeAction(Handle, SyntaxKind.IdentifierName);
        }

        private static void Handle(SyntaxNodeAnalysisContext context)
        {
            // ReSharper disable once PossibleNullReferenceException
            SyntaxToken syntaxToken = (context.Node as ClassDeclarationSyntax).Identifier;
        }
    }
}