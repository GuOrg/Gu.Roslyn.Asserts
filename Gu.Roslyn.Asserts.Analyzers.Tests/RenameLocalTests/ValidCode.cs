namespace Gu.Roslyn.Asserts.Analyzers.Tests.RenameLocalTests
{
    using Microsoft.CodeAnalysis.Diagnostics;
    using NUnit.Framework;

    public static class ValidCode
    {
        private static readonly DiagnosticAnalyzer Analyzer = new ArgumentAnalyzer();

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
        public static void WhenAnalyzerCorrectName()
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
            var code = ""class ↓C { }"";
            RoslynAssert.Valid(Analyzer, code);
        }
    }
}";
            RoslynAssert.Valid(Analyzer, AnalyzerCode, code);
        }

        [Test]
        public static void DoNotWarnWhenTwoParams()
        {
            var code = @"
namespace RoslynSandbox
{
    using Microsoft.CodeAnalysis.Diagnostics;
    using Gu.Roslyn.Asserts;
    using NUnit.Framework;

    public static class C
    {
        private static readonly PlaceHolderAnalyzer Analyzer = new PlaceHolderAnalyzer();

        [Test]
        public static void M()
        {
            var code1 = ""class C { }"";
            var code2 = ""class C { }"";
            RoslynAssert.Valid(Analyzer, code1, code2);
        }
    }
}";

            RoslynAssert.Valid(Analyzer, AnalyzerCode, code);
        }

        [Test]
        public static void RoslynAssertCodeFixOneParamWithPosition()
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
            var code = ""↓class C { }"";
            RoslynAssert.Diagnostics(Analyzer, code);
        }
    }
}";
            RoslynAssert.Valid(Analyzer, AnalyzerCode, code);
        }

        [Test]
        public static void RoslynAssertCodeFixTwoParamsWithOnePosition()
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
            var code1 = ""class C { }"";
            var code = ""↓class C { }"";
            RoslynAssert.Diagnostics(Analyzer, code1, code);
        }
    }
}";
            RoslynAssert.Valid(Analyzer, AnalyzerCode, code);
        }

        [Test]
        public static void RoslynAssertCodeFixTwoParamsWithOnePositionConst()
        {
            var code = @"
namespace RoslynSandbox
{
    using Gu.Roslyn.Asserts;
    using NUnit.Framework;

    public static class C
    {
        private static readonly PlaceHolderAnalyzer Analyzer = new PlaceHolderAnalyzer();
        const string code1 = ""class C { }"";

        [Test]
        public static void M()
        {
            var code = ""↓class C { }"";
            RoslynAssert.Diagnostics(Analyzer, code1, code);
        }
    }
}";
            RoslynAssert.Valid(Analyzer, AnalyzerCode, code);
        }

        [Test]
        public static void RoslynAssertCodeFixTwoParamsWithOnePositionInstanceField()
        {
            var code = @"
namespace RoslynSandbox
{
    using Gu.Roslyn.Asserts;
    using NUnit.Framework;

    public class C
    {
        private static readonly PlaceHolderAnalyzer Analyzer = new PlaceHolderAnalyzer();
        private readonly string code1 = ""class C { }"";

        [Test]
        public void M()
        {
            var code = ""↓class C { }"";
            RoslynAssert.Diagnostics(Analyzer, this.code1, code);
        }
    }
}";
            RoslynAssert.Valid(Analyzer, AnalyzerCode, code);
        }

        [Test]
        public static void RoslynAssertCodeFixArrayWithOnePosition()
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
            var code1 = ""class C { }"";
            var code = ""↓class C { }"";
            RoslynAssert.Diagnostics(Analyzer, new [] { code1, code });
        }
    }
}";
            RoslynAssert.Valid(Analyzer, AnalyzerCode, code);
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
            var before = ""↓class C { }"";
            RoslynAssert.CodeFix(Analyzer, Fix, before, string.Empty);
        }
    }
}";
            RoslynAssert.Valid(Analyzer, AnalyzerCode, FixCode, code);
        }
    }
}
