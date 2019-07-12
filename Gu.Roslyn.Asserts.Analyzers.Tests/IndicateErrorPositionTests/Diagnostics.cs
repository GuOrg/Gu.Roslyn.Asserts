namespace Gu.Roslyn.Asserts.Analyzers.Tests.IndicateErrorPositionTests
{
    using Microsoft.CodeAnalysis.CodeFixes;
    using Microsoft.CodeAnalysis.Diagnostics;
    using NUnit.Framework;

    public static class Diagnostics
    {
        private static readonly DiagnosticAnalyzer Analyzer = new ArgumentAnalyzer();
        private static readonly CodeFixProvider Fix = new IndicateErrorPositionFix();
        private static readonly ExpectedDiagnostic ExpectedDiagnostic = ExpectedDiagnostic.Create(GURA02IndicateErrorPosition.Descriptor);

        private static readonly string AnalyzerCode = @"
namespace RoslynSandbox
{
    using System.Collections.Immutable;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.Diagnostics;

    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    internal class PlaceHolderAnalyzer : DiagnosticAnalyzer
    {
        internal const string DiagnosticId = ""NoError"";

        internal static readonly DiagnosticDescriptor Descriptor = new DiagnosticDescriptor(
            id: DiagnosticId,
            title: ""This analyzer never reports an error."",
            messageFormat: ""Message format."",
            category: ""Category"",
            defaultSeverity: DiagnosticSeverity.Warning,
            isEnabledByDefault: true);

        /// <inheritdoc/>
        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get; } = ImmutableArray.Create(Descriptor);

        public override void Initialize(AnalysisContext context)
        {
            if (context == null)
            {
                throw new System.ArgumentNullException(nameof(context));
            }

            context.EnableConcurrentExecution();
            context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.Analyze | GeneratedCodeAnalysisFlags.ReportDiagnostics);
            context.RegisterSyntaxNodeAction(Handle, SyntaxKind.IdentifierName);
        }

        private static void Handle(SyntaxNodeAnalysisContext context)
        {
        }
    }
}";

        private static readonly string FixCode = @"
namespace RoslynSandbox
{
    using System.Collections.Immutable;
    using System.Threading.Tasks;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CodeFixes;

    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(PlaceHolderFix))]
    internal class PlaceHolderFix : CodeFixProvider
    {
        public override ImmutableArray<string> FixableDiagnosticIds { get; } = ImmutableArray.Create(PlaceHolderAnalyzer.DiagnosticId);

        public override FixAllProvider GetFixAllProvider() => null;

        public override Task RegisterCodeFixesAsync(CodeFixContext context) => Task.FromResult(true);
    }
}";

        [Test]
        public static void RoslynAssertDiagnosticsTwoParams()
        {
            var code = @"
namespace RoslynSandbox
{
    using Gu.Roslyn.Asserts;
    using NUnit.Framework;

    public static class C
    {
        private static readonly PlaceHolderAnalyzer Analyzer = new PlaceHolderAnalyzer();

        [Test]
        public static void M()
        {
            var code1 = ""class Foo { }"";
            var code2 = ""class Foo { }"";
            RoslynAssert.Diagnostics(Analyzer, ↓code1, ↓code2);
        }
    }
}";

            RoslynAssert.NoFix(Analyzer, Fix, ExpectedDiagnostic, AnalyzerCode, code);
        }

        [Test]
        public static void RoslynAssertDiagnosticsArray()
        {
            var code = @"
namespace RoslynSandbox
{
    using Gu.Roslyn.Asserts;
    using NUnit.Framework;

    public static class C
    {
        private static readonly PlaceHolderAnalyzer Analyzer = new PlaceHolderAnalyzer();

        [Test]
        public static void M()
        {
            var code1 = ""class Foo { }"";
            var code2 = ""class Foo { }"";
            RoslynAssert.Diagnostics(Analyzer, ↓new [] { code1, code2 });
        }
    }
}";

            RoslynAssert.NoFix(Analyzer, Fix, ExpectedDiagnostic, AnalyzerCode, code);
        }

        [Test]
        public static void RoslynAssertCodeFixOneBefore()
        {
            var code = @"
namespace RoslynSandbox
{
    using Gu.Roslyn.Asserts;
    using NUnit.Framework;

    public static class C
    {
        private static readonly PlaceHolderAnalyzer Analyzer = new PlaceHolderAnalyzer();
        private static readonly PlaceHolderFix Fix = new PlaceHolderFix();

        [Test]
        public static void M()
        {
            var code1 = ""class Foo { }"";
            var code2 = ""class Foo { }"";
            RoslynAssert.CodeFix(Analyzer, Fix, ↓new [] { code1, code2 }, string.Empty);
        }
    }
}";

            RoslynAssert.NoFix(Analyzer, Fix, ExpectedDiagnostic, AnalyzerCode, FixCode, code);
        }
    }
}
