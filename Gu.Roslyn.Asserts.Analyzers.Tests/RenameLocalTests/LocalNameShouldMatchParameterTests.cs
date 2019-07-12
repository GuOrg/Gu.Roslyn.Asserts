namespace Gu.Roslyn.Asserts.Analyzers.Tests.RenameLocalTests
{
    using Microsoft.CodeAnalysis.CodeFixes;
    using Microsoft.CodeAnalysis.Diagnostics;
    using NUnit.Framework;

    public static class CodeFix
    {
        private static readonly DiagnosticAnalyzer Analyzer = new ArgumentAnalyzer();
        private static readonly CodeFixProvider Fix = new RenameLocalFix();
        private static readonly ExpectedDiagnostic ExpectedDiagnostic = ExpectedDiagnostic.Create(GURA01NameOfLocalShouldMatchParameter.Descriptor);

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
        public static void WhenAnalyzerWrongName()
        {
            var before = @"
namespace RoslynSandbox
{
    using Microsoft.CodeAnalysis.Diagnostics;
    using Gu.Roslyn.Asserts;
    using NUnit.Framework;

    public static class C
    {
        [Test]
        public static void M()
        {
            var code = ""class C { }"";
            var wrong = (DiagnosticAnalyzer)null;
            RoslynAssert.Valid(↓wrong, code);
        }
    }
}";

            var after = @"
namespace RoslynSandbox
{
    using Microsoft.CodeAnalysis.Diagnostics;
    using Gu.Roslyn.Asserts;
    using NUnit.Framework;

    public static class C
    {
        [Test]
        public static void M()
        {
            var code = ""class C { }"";
            var analyzer = (DiagnosticAnalyzer)null;
            RoslynAssert.Valid(analyzer, code);
        }
    }
}";
            var expectedDiagnostic = ExpectedDiagnostic.WithMessage("Name of 'wrong' should be 'analyzer'.");
            RoslynAssert.CodeFix(Analyzer, Fix, expectedDiagnostic, before, after, fixTitle: "Rename to 'analyzer'.");
        }

        [Test]
        public static void WhenOneParam()
        {
            var before = @"
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
            var C = ""class C { }"";
            RoslynAssert.Valid(Analyzer, ↓C);
        }
    }
}";

            var after = @"
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
            var code = ""class C { }"";
            RoslynAssert.Valid(Analyzer, code);
        }
    }
}";
            var expectedDiagnostic = ExpectedDiagnostic.WithMessage("Name of 'C' should be 'code'.");
            RoslynAssert.CodeFix(Analyzer, Fix, expectedDiagnostic, new[] { AnalyzerCode, before }, after);
        }

        [Test]
        public static void WhenOnlyOneBeforeHasPosition()
        {
            var before = @"
namespace RoslynSandbox
{
    using Microsoft.CodeAnalysis.CodeFixes;
    using Microsoft.CodeAnalysis.Diagnostics;
    using Gu.Roslyn.Asserts;
    using NUnit.Framework;

    public static class C
    {
        [Test]
        public static void M()
        {
            var code1 = ""class C1 { }"";
            var code2 = ""class ↓C2 { }"";
            var after = ""class C2 { }"";
            RoslynAssert.CodeFix((DiagnosticAnalyzer)null, (CodeFixProvider)null, new [] { code1, code2 }, after);
        }
    }
}";

            var after = @"
namespace RoslynSandbox
{
    using Microsoft.CodeAnalysis.CodeFixes;
    using Microsoft.CodeAnalysis.Diagnostics;
    using Gu.Roslyn.Asserts;
    using NUnit.Framework;

    public static class C
    {
        [Test]
        public static void M()
        {
            var code1 = ""class C1 { }"";
            var before = ""class ↓C2 { }"";
            var after = ""class C2 { }"";
            RoslynAssert.CodeFix((DiagnosticAnalyzer)null, (CodeFixProvider)null, new [] { code1, before }, after);
        }
    }
}";
            var expectedDiagnostic = ExpectedDiagnostic.WithMessage("Name of 'code2' should be 'before'.");
            var diagnosticsAndSources = new DiagnosticsAndSources(new[] { expectedDiagnostic }, new[] { before });
            RoslynAssert.CodeFix(Analyzer, Fix, diagnosticsAndSources, new[] { after }, fixTitle: "Rename to 'before'.");
        }

        [Test]
        public static void WhenOnlyOneCodeHasPosition()
        {
            var before = @"
namespace RoslynSandbox
{
    using Microsoft.CodeAnalysis.Diagnostics;
    using Gu.Roslyn.Asserts;
    using NUnit.Framework;

    public static class C
    {
        [Test]
        public static void M()
        {
            var code1 = ""class C1 { }"";
            var code2 = ""class ↓C2 { }"";
            var after = ""class C2 { }"";
            RoslynAssert.Diagnostics((DiagnosticAnalyzer)null, new [] { code1, code2 });
        }
    }
}";

            var after = @"
namespace RoslynSandbox
{
    using Microsoft.CodeAnalysis.Diagnostics;
    using Gu.Roslyn.Asserts;
    using NUnit.Framework;

    public static class C
    {
        [Test]
        public static void M()
        {
            var code1 = ""class C1 { }"";
            var code = ""class ↓C2 { }"";
            var after = ""class C2 { }"";
            RoslynAssert.Diagnostics((DiagnosticAnalyzer)null, new [] { code1, code });
        }
    }
}";
            var expectedDiagnostic = ExpectedDiagnostic.WithMessage("Name of 'code2' should be 'code'.");
            var diagnosticsAndSources = new DiagnosticsAndSources(new[] { expectedDiagnostic }, new[] { before });
            RoslynAssert.CodeFix(Analyzer, Fix, diagnosticsAndSources, new[] { after }, fixTitle: "Rename to 'code'.");
        }

        [Explicit("Temp ignore.")]
        [Test]
        public static void RunOnTestProject()
        {
            var sln = CodeFactory.CreateSolution(
                ProjectFile.Find("Gu.Roslyn.Asserts.Tests.csproj"),
                MetadataReferences.FromAttributes());
            RoslynAssert.Valid(Analyzer, sln);
        }
    }
}
