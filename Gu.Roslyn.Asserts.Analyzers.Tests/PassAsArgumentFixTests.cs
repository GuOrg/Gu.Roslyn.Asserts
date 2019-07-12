namespace Gu.Roslyn.Asserts.Analyzers.Tests
{
    using Microsoft.CodeAnalysis.CodeFixes;
    using NUnit.Framework;

    public static class PassAsArgumentFixTests
    {
        private static readonly CodeFixProvider Fix = new PassAsArgumentFix();
        private static readonly ExpectedDiagnostic ExpectedDiagnostic = ExpectedDiagnostic.Create("CS0618");

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
        public static void RoslynAssertValidCreateField()
        {
            var before = @"
namespace RoslynSandbox
{
    using Gu.Roslyn.Asserts;
    using NUnit.Framework;

    public static class C
    {
        [Test]
        public static void M()
        {
            var code = ""class Foo { }"";
            ↓RoslynAssert.Valid<PlaceHolderAnalyzer>(code);
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
            var code = ""class Foo { }"";
            RoslynAssert.Valid(Analyzer, code);
        }
    }
}";
            RoslynAssert.CodeFix(Fix, ExpectedDiagnostic, new[] { AnalyzerCode, before }, after, fixTitle: "Create and use field 'Analyzer'.", suppressedDiagnostics: new[] { "CS1701" });
        }

        [Explicit("Need to test how this works.")]
        [Test]
        public static void RoslynAssertValidCreateFieldFixAll()
        {
            var before = @"
namespace RoslynSandbox
{
    using Gu.Roslyn.Asserts;
    using NUnit.Framework;

    public static class C
    {
        [Test]
        public static void M1()
        {
            var code = ""class Foo { }"";
            ↓RoslynAssert.Valid<PlaceHolderAnalyzer>(code);
        }

        [Test]
        public static void M2()
        {
            var code = ""class Foo { }"";
            ↓RoslynAssert.Valid<PlaceHolderAnalyzer>(code);
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
        public static void M1()
        {
            var code = ""class Foo { }"";
            RoslynAssert.Valid(Analyzer, code);
        }

        [Test]
        public static void M2()
        {
            var code = ""class Foo { }"";
            RoslynAssert.Valid(Analyzer, code);
        }
    }
}";
            RoslynAssert.FixAll(Fix, ExpectedDiagnostic, new[] { AnalyzerCode, before }, after, fixTitle: "Create and use field 'Analyzer'.", suppressedDiagnostics: new[] { "CS1701" });
        }

        [Test]
        public static void RoslynAssertValidUseExistingStaticField()
        {
            var before = @"
namespace RoslynSandbox
{
    using Microsoft.CodeAnalysis.Diagnostics;
    using Gu.Roslyn.Asserts;
    using NUnit.Framework;

    public static class C
    {
        private static readonly DiagnosticAnalyzer Analyzer = new PlaceHolderAnalyzer();

        [Test]
        public static void M()
        {
            var code = ""class Foo { }"";
            ↓RoslynAssert.Valid<PlaceHolderAnalyzer>(code);
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
        private static readonly DiagnosticAnalyzer Analyzer = new PlaceHolderAnalyzer();

        [Test]
        public static void M()
        {
            var code = ""class Foo { }"";
            RoslynAssert.Valid(Analyzer, code);
        }
    }
}";
            RoslynAssert.CodeFix(Fix, ExpectedDiagnostic, new[] { AnalyzerCode, before }, after, fixTitle: "Use 'Analyzer'.", suppressedDiagnostics: new[] { "CS1701" });
        }

        [Test]
        public static void RoslynAssertValidUseExistingInstanceField()
        {
            var before = @"
namespace RoslynSandbox
{
    using Microsoft.CodeAnalysis.Diagnostics;
    using Gu.Roslyn.Asserts;
    using NUnit.Framework;

    public class C
    {
        private readonly DiagnosticAnalyzer Analyzer = new PlaceHolderAnalyzer();

        [Test]
        public void M()
        {
            var code = ""class Foo { }"";
            ↓RoslynAssert.Valid<PlaceHolderAnalyzer>(code);
        }
    }
}";

            var after = @"
namespace RoslynSandbox
{
    using Microsoft.CodeAnalysis.Diagnostics;
    using Gu.Roslyn.Asserts;
    using NUnit.Framework;

    public class C
    {
        private readonly DiagnosticAnalyzer Analyzer = new PlaceHolderAnalyzer();

        [Test]
        public void M()
        {
            var code = ""class Foo { }"";
            RoslynAssert.Valid(this.Analyzer, code);
        }
    }
}";
            RoslynAssert.CodeFix(Fix, ExpectedDiagnostic, new[] { AnalyzerCode, before }, after, fixTitle: "Use 'Analyzer'.", suppressedDiagnostics: new[] { "CS1701" });
        }

        [Test]
        public static void RoslynAssertCodeFixOnlyUseExistingStaticField()
        {
            var before = @"
namespace RoslynSandbox
{
    using Microsoft.CodeAnalysis.CodeFixes;
    using Gu.Roslyn.Asserts;
    using NUnit.Framework;

    public static class C
    {
        private static readonly CodeFixProvider Fix = new PlaceHolderFix();

        [Test]
        public static void M()
        {
            var before = ""class Foo { }"";
            var after = ""class Foo { }"";
            ↓RoslynAssert.CodeFix<PlaceHolderFix>(ExpectedDiagnostic.Create(""123""), before, after);
        }
    }
}";

            var after = @"
namespace RoslynSandbox
{
    using Microsoft.CodeAnalysis.CodeFixes;
    using Gu.Roslyn.Asserts;
    using NUnit.Framework;

    public static class C
    {
        private static readonly CodeFixProvider Fix = new PlaceHolderFix();

        [Test]
        public static void M()
        {
            var before = ""class Foo { }"";
            var after = ""class Foo { }"";
            RoslynAssert.CodeFix(Fix, ExpectedDiagnostic.Create(""123""), before, after);
        }
    }
}";
            RoslynAssert.CodeFix(Fix, ExpectedDiagnostic, new[] { AnalyzerCode, FixCode, before }, after, fixTitle: "Use 'Fix'.", suppressedDiagnostics: new[] { "CS1701", "CS1702" });
        }

        [Test]
        public static void RoslynAssertCodeFixUseExistingStaticFields()
        {
            var before = @"
namespace RoslynSandbox
{
    using Microsoft.CodeAnalysis.Diagnostics;
    using Microsoft.CodeAnalysis.CodeFixes;
    using Gu.Roslyn.Asserts;
    using NUnit.Framework;

    public static class C
    {
        private static readonly DiagnosticAnalyzer Analyzer = new PlaceHolderAnalyzer();
        private static readonly CodeFixProvider Fix = new PlaceHolderFix();

        [Test]
        public static void M()
        {
            var before = ""class Foo { }"";
            var after = ""class Foo { }"";
            ↓RoslynAssert.CodeFix<PlaceHolderAnalyzer, PlaceHolderFix>(ExpectedDiagnostic.Create(""123""), before, after);
        }
    }
}";

            var after = @"
namespace RoslynSandbox
{
    using Microsoft.CodeAnalysis.Diagnostics;
    using Microsoft.CodeAnalysis.CodeFixes;
    using Gu.Roslyn.Asserts;
    using NUnit.Framework;

    public static class C
    {
        private static readonly DiagnosticAnalyzer Analyzer = new PlaceHolderAnalyzer();
        private static readonly CodeFixProvider Fix = new PlaceHolderFix();

        [Test]
        public static void M()
        {
            var before = ""class Foo { }"";
            var after = ""class Foo { }"";
            RoslynAssert.CodeFix(Analyzer, Fix, ExpectedDiagnostic.Create(""123""), before, after);
        }
    }
}";
            RoslynAssert.CodeFix(Fix, ExpectedDiagnostic, new[] { FixCode, AnalyzerCode, before }, after, fixTitle: "Use 'Analyzer' and 'Fix.", suppressedDiagnostics: new[] { "CS1701", "CS1702" });
        }

        [Test]
        public static void RoslynAssertCodeFixCreateAndUseFields()
        {
            var before = @"
namespace RoslynSandbox
{
    using Gu.Roslyn.Asserts;
    using NUnit.Framework;

    public static class C
    {
        [Test]
        public static void M()
        {
            var before = ""class Foo { }"";
            var after = ""class Foo { }"";
            ↓RoslynAssert.CodeFix<PlaceHolderAnalyzer, PlaceHolderFix>(ExpectedDiagnostic.Create(""123""), before, after);
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
        private static readonly PlaceHolderFix Fix = new PlaceHolderFix();

        [Test]
        public static void M()
        {
            var before = ""class Foo { }"";
            var after = ""class Foo { }"";
            RoslynAssert.CodeFix(Analyzer, Fix, ExpectedDiagnostic.Create(""123""), before, after);
        }
    }
}";
            RoslynAssert.CodeFix(Fix, ExpectedDiagnostic, new[] { FixCode, AnalyzerCode, before }, after, fixTitle: "Create and use fields 'Analyzer' and 'Fix'.", suppressedDiagnostics: new[] { "CS1701", "CS1702" });
        }
    }
}
