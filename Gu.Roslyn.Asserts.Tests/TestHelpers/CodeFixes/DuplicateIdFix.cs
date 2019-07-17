namespace Gu.Roslyn.Asserts.Tests.CodeFixes
{
    using System;
    using System.Collections.Immutable;
    using System.Threading.Tasks;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CodeFixes;

    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(DuplicateIdFix))]
    internal class DuplicateIdFix : CodeFixProvider
    {
        public override ImmutableArray<string> FixableDiagnosticIds { get; } = ImmutableArray.Create(Descriptors.Id1.Id);

        public override FixAllProvider GetFixAllProvider() => CustomFixAllProviders.BatchFixer;

        public override Task RegisterCodeFixesAsync(CodeFixContext context) => throw new NotSupportedException();
    }
}
