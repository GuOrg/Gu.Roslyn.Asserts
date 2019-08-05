namespace Gu.Roslyn.Asserts.Analyzers.Tests.GURA08bShouldBePublic
{
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.Diagnostics;
    using NUnit.Framework;

    public static class Valid
    {
        private static readonly DiagnosticAnalyzer Analyzer = new ObjectCreationAnalyzer();
        private static readonly DiagnosticDescriptor Descriptor = Descriptors.GURA08bShouldBePublic;

        [Test]
        public static void DiagnosticAnalyzer()
        {
            var analyzer = @"
namespace N
{
    using System.Collections.Immutable;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.Diagnostics;

    public class Analyzer : DiagnosticAnalyzer
    {
        /// <inheritdoc/>
        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get; }

        public override void Initialize(AnalysisContext context)
        {
        }

        private static void Handle(SyntaxNodeAnalysisContext context)
        {
        }
    }
}";

            var diagnostics = @"
namespace N
{
    using Gu.Roslyn.Asserts;
    using NUnit.Framework;

    public static class Diagnostics
    {
        private static readonly Analyzer Analyzer = new Analyzer();

        [TestCase(""C2 { }"")]
        public static void M(string declaration)
        {
            var c1 = ""class C1 { }"";
            var code = ""↓class C2 { }"".AssertReplace(""C2 { }"", declaration);
            RoslynAssert.Diagnostics(Analyzer, c1, code);
        }
    }
}";
            RoslynAssert.Valid(Analyzer, Descriptor, analyzer, diagnostics);
        }

        [Test]
        public static void CodeFixProvider()
        {
            var analyzer = @"
namespace N
{
    using System.Collections.Immutable;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.Diagnostics;

    public class Analyzer : DiagnosticAnalyzer
    {
        /// <inheritdoc/>
        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get; }

        public override void Initialize(AnalysisContext context)
        {
        }

        private static void Handle(SyntaxNodeAnalysisContext context)
        {
        }
    }
}";

            var fix = @"
namespace N
{
    using System.Collections.Immutable;
    using System.Threading.Tasks;
    using Microsoft.CodeAnalysis.CodeFixes;

    public class Fix : CodeFixProvider
    {
        public override ImmutableArray<string> FixableDiagnosticIds { get; }

        public override FixAllProvider GetFixAllProvider() => null;

        public override Task RegisterCodeFixesAsync(CodeFixContext context) => Task.CompletedTask;
    }
}";

            var diagnostics = @"
namespace N
{
    using Gu.Roslyn.Asserts;
    using NUnit.Framework;

    public static class Diagnostics
    {
        private static readonly Analyzer Analyzer = new Analyzer();
        private static readonly Fix Fix = new Fix();

        [TestCase(""C2 { }"")]
        public static void M(string declaration)
        {
            var c1 = ""class C1 { }"";
            var code = ""↓class C2 { }"".AssertReplace(""C2 { }"", declaration);
            RoslynAssert.Diagnostics(Analyzer, c1, code);
        }
    }
}";
            RoslynAssert.Valid(Analyzer, Descriptor, analyzer, fix, diagnostics);
        }

        [Test]
        public static void CodeRefactoringProvider()
        {
            var placeholderRefactoring = @"
namespace N
{
    using System.Threading.Tasks;
    using Microsoft.CodeAnalysis.CodeRefactorings;

    public class PlaceholderRefactoring : CodeRefactoringProvider
    {
        public override Task ComputeRefactoringsAsync(CodeRefactoringContext context) => Task.CompletedTask;
    }
}";
            RoslynAssert.Valid(Analyzer, Descriptor, placeholderRefactoring);
        }

        [Test]
        public static void RandomClass()
        {
            var c = @"
namespace N
{
    using System.Collections.Immutable;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.Diagnostics;

    public class C
    {
    }
}";
            RoslynAssert.Valid(Analyzer, Descriptor, c);
        }

        [Test]
        public static void RoslynAssertDiagnostics()
        {
            var analyzer = @"
namespace N
{
    using System.Collections.Immutable;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.Diagnostics;

    public class Analyzer : DiagnosticAnalyzer
    {
        /// <inheritdoc/>
        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get; }

        public override void Initialize(AnalysisContext context)
        {
        }

        private static void Handle(SyntaxNodeAnalysisContext context)
        {
        }
    }
}";
            var diagnostics = @"
namespace N
{
    using Gu.Roslyn.Asserts;
    using NUnit.Framework;

    public static class Diagnostics
    {
        private static readonly Analyzer Analyzer = new Analyzer();

        [TestCase(""C2 { }"")]
        public static void M(string declaration)
        {
            var c1 = ""class C1 { }"";
            var code = ""class C2 { }"".AssertReplace(""C2 { }"", declaration);
            RoslynAssert.Diagnostics(Analyzer, c1, code);
        }
    }
}";
            RoslynAssert.Valid(Analyzer, Descriptor, analyzer, diagnostics);
        }

        [Test]
        public static void DiagnosticGenericAnalyzer()
        {
            var analyzer = @"
namespace N
{
    using System.Collections.Immutable;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.Diagnostics;

    public class Analyzer : DiagnosticAnalyzer
    {
        /// <inheritdoc/>
        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get; }

        public override void Initialize(AnalysisContext context)
        {
        }

        private static void Handle(SyntaxNodeAnalysisContext context)
        {
        }
    }
}";

            var diagnostics = @"
namespace N
{
    using Microsoft.CodeAnalysis.Diagnostics;

    using Gu.Roslyn.Asserts;
    using NUnit.Framework;

    [TestFixture(typeof(Analyzer))]
    public static class Diagnostics<T>
        where T : DiagnosticAnalyzer, new()
    {
        private static readonly DiagnosticAnalyzer Analyzer = new T();

        [TestCase(""C2 { }"")]
        public static void M(string declaration)
        {
            var c1 = ""class C1 { }"";
            var code = ""class C2 { }"".AssertReplace(""C2 { }"", declaration);
            RoslynAssert.Diagnostics(Analyzer, c1, code);
        }
    }
}";
            RoslynAssert.Valid(Analyzer, Descriptor, analyzer, diagnostics);
        }
    }
}
