namespace Gu.Roslyn.Asserts.Analyzers.Tests.IndicateErrorPositionTests
{
    using Microsoft.CodeAnalysis.Diagnostics;
    using NUnit.Framework;

    public static class Valid
    {
        private static readonly DiagnosticAnalyzer Analyzer = new ArgumentAnalyzer();

        [Test]
        public static void DiagnosticsOneParamWithPosition()
        {
            var code = @"
namespace RoslynSandbox
{
    using Gu.Roslyn.Asserts;
    using NUnit.Framework;

    public static class C
    {
        private static readonly PlaceholderAnalyzer Analyzer = new PlaceholderAnalyzer();

        [Test]
        public static void M()
        {
            var code = ""↓class C { }"";
            RoslynAssert.Diagnostics(Analyzer, code);
        }
    }
}";
            RoslynAssert.Valid(Analyzer, Code.PlaceholderAnalyzer, code);
        }

        [Test]
        public static void DiagnosticsOneParamWithPositionAssertReplace()
        {
            var code = @"
namespace RoslynSandbox
{
    using Gu.Roslyn.Asserts;
    using NUnit.Framework;

    public static class C
    {
        private static readonly PlaceholderAnalyzer Analyzer = new PlaceholderAnalyzer();

        [TestCase(""C { }"")]
        public static void M(string declaration)
        {
            var code = ""↓class C { }"".AssertReplace(""C { }"", declaration);
            RoslynAssert.Diagnostics(Analyzer, code);
        }
    }
}";
            RoslynAssert.Valid(Analyzer, Code.PlaceholderAnalyzer, code);
        }

        [Test]
        public static void DiagnosticsTwoParamsWithOnePosition()
        {
            var code = @"
namespace RoslynSandbox
{
    using Gu.Roslyn.Asserts;
    using NUnit.Framework;

    public static class C
    {
        private static readonly PlaceholderAnalyzer Analyzer = new PlaceholderAnalyzer();

        [Test]
        public static void M()
        {
            var c1 = ""class C1 { }"";
            var code = ""↓class C { }"";
            RoslynAssert.Diagnostics(Analyzer, c1, code);
        }
    }
}";
            RoslynAssert.Valid(Analyzer, Code.PlaceholderAnalyzer, code);
        }

        [Test]
        public static void DiagnosticsTwoParamsWithOnePositionAndAssertReplace()
        {
            var code = @"
namespace RoslynSandbox
{
    using Gu.Roslyn.Asserts;
    using NUnit.Framework;

    public static class C
    {
        private static readonly PlaceholderAnalyzer Analyzer = new PlaceholderAnalyzer();

        [TestCase(""C2 { }"")]
        public static void M(string declaration)
        {
            var c1 = ""class C1 { }"";
            var code = ""↓class C2 { }"".AssertReplace(""C2 { }"", declaration);
            RoslynAssert.Diagnostics(Analyzer, c1, code);
        }
    }
}";
            RoslynAssert.Valid(Analyzer, Code.PlaceholderAnalyzer, code);
        }

        [Test]
        public static void DiagnosticsTwoParamsWithOnePositionAndAssertReplaceWithPosition()
        {
            var code = @"
namespace RoslynSandbox
{
    using Gu.Roslyn.Asserts;
    using NUnit.Framework;

    public static class C
    {
        private static readonly PlaceholderAnalyzer Analyzer = new PlaceholderAnalyzer();

        [TestCase(""↓C { }"")]
        public static void M(string declaration)
        {
            var c1 = ""class C1 { }"";
            var code = ""class C { }"".AssertReplace(""C { }"", declaration);
            RoslynAssert.Diagnostics(Analyzer, c1, code);
        }
    }
}";
            RoslynAssert.Valid(Analyzer, Code.PlaceholderAnalyzer, code);
        }

        [Test]
        public static void DiagnosticsTwoParamsWithOnePositionConst()
        {
            var code = @"
namespace RoslynSandbox
{
    using Gu.Roslyn.Asserts;
    using NUnit.Framework;

    public static class C
    {
        private static readonly PlaceholderAnalyzer Analyzer = new PlaceholderAnalyzer();
        const string C1 = ""class C1 { }"";

        [Test]
        public static void M()
        {
            var code = ""↓class C2 { }"";
            RoslynAssert.Diagnostics(Analyzer, C1, code);
        }
    }
}";
            RoslynAssert.Valid(Analyzer, Code.PlaceholderAnalyzer, code);
        }

        [Test]
        public static void DiagnosticsTwoParamsWithOnePositionInstanceField()
        {
            var code = @"
namespace RoslynSandbox
{
    using Gu.Roslyn.Asserts;
    using NUnit.Framework;

    public class C
    {
        private static readonly PlaceholderAnalyzer Analyzer = new PlaceholderAnalyzer();
        private readonly string code1 = ""class C { }"";

        [Test]
        public void M()
        {
            var code = ""↓class C { }"";
            RoslynAssert.Diagnostics(Analyzer, this.code1, code);
        }
    }
}";
            RoslynAssert.Valid(Analyzer, Code.PlaceholderAnalyzer, code);
        }

        [Test]
        public static void DiagnosticsArrayWithOnePosition()
        {
            var code = @"
namespace RoslynSandbox
{
    using Gu.Roslyn.Asserts;
    using NUnit.Framework;

    public static class C
    {
        private static readonly PlaceholderAnalyzer Analyzer = new PlaceholderAnalyzer();

        [Test]
        public static void M()
        {
            var code1 = ""class C { }"";
            var code = ""↓class C { }"";
            RoslynAssert.Diagnostics(Analyzer, new [] { code1, code });
        }
    }
}";
            RoslynAssert.Valid(Analyzer, Code.PlaceholderAnalyzer, code);
        }

        [Test]
        public static void CodeFixOneBefore()
        {
            var code = @"
namespace RoslynSandbox
{
    using Gu.Roslyn.Asserts;
    using NUnit.Framework;

    public static class C
    {
        private static readonly PlaceholderAnalyzer Analyzer = new PlaceholderAnalyzer();
        private static readonly PlaceholderFix Fix = new PlaceholderFix();

        [Test]
        public static void M()
        {
            var before = ""↓class C { }"";
            RoslynAssert.CodeFix(Analyzer, Fix, before, string.Empty);
        }
    }
}";
            RoslynAssert.Valid(Analyzer, Code.PlaceholderAnalyzer, Code.PlaceholderFix, code);
        }

        [Test]
        public static void CodeFixAssertReplace()
        {
            var code = @"
namespace RoslynSandbox
{
    using Gu.Roslyn.Asserts;
    using NUnit.Framework;

    public static class C
    {
        private static readonly PlaceholderAnalyzer Analyzer = new PlaceholderAnalyzer();
        private static readonly PlaceholderFix Fix = new PlaceholderFix();

        [TestCase(""int"")]
        [TestCase(""string"")]
        public static void M(string type)
        {
            var before = @""
namespace RoslynSandbox
{
    public class C
    {
        /// <summary>
        /// Summary
        /// </summary>
        public static void M(string ↓text)
        {
        }
    }
}"".AssertReplace(""string"", type);

            var after = @""
namespace RoslynSandbox
{
    public class C
    {
        /// <summary>
        /// Summary
        /// </summary>
        /// <param name=""""text""""></param>
        public static void M(string text)
        {
        }
    }
}"".AssertReplace(""string"", type);
            RoslynAssert.CodeFix(Analyzer, Fix, before, after);
        }
        }
}";
            RoslynAssert.Valid(Analyzer, Code.PlaceholderAnalyzer, Code.PlaceholderFix, code);
        }
    }
}
