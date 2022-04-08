namespace Gu.Roslyn.Asserts.Analyzers.Tests.GURA02IndicateErrorPosition
{
    using NUnit.Framework;

    public static class Valid
    {
        private static readonly DiagnosticAssert Assert = RoslynAssert.Create<InvocationAnalyzer>(Descriptors.GURA02IndicateErrorPosition);

        [Test]
        public static void DiagnosticsOneParamWithPosition()
        {
            var code = @"
namespace N
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
            Assert.Valid(Code.PlaceholderAnalyzer, code);
        }

        [Test]
        public static void SuppressedOneParamWithPosition()
        {
            var code = @"
namespace N
{
    using Gu.Roslyn.Asserts;
    using NUnit.Framework;

    public static class C
    {
        private static readonly PlaceholderSuppressor Suppressor = new PlaceholderSuppressor();

        [Test]
        public static void M()
        {
            var code = ""↓class C { }"";
            RoslynAssert.Suppressed(Suppressor, code);
        }
    }
}";
            Assert.Valid(Code.PlaceholderSuppressor, code);
        }

        [Test]
        public static void NotSuppressedOneParamWithPosition()
        {
            var code = @"
namespace N
{
    using Gu.Roslyn.Asserts;
    using NUnit.Framework;

    public static class C
    {
        private static readonly PlaceholderSuppressor Suppressor = new PlaceholderSuppressor();

        [Test]
        public static void M()
        {
            var code = ""↓class C { }"";
            RoslynAssert.NotSuppressed(Suppressor, code);
        }
    }
}";
            Assert.Valid(Code.PlaceholderSuppressor, code);
        }

        [Test]
        public static void DiagnosticsOneParamWithPositionAssertReplace()
        {
            var code = @"
namespace N
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
            Assert.Valid(Code.PlaceholderAnalyzer, code);
        }

        [Test]
        public static void DiagnosticsTwoParamsWithOnePosition()
        {
            var code = @"
namespace N
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
            Assert.Valid(Code.PlaceholderAnalyzer, code);
        }

        [Test]
        public static void DiagnosticsTwoParamsWithOnePositionAndAssertReplace()
        {
            var code = @"
namespace N
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
            Assert.Valid(Code.PlaceholderAnalyzer, code);
        }

        [Test]
        public static void DiagnosticsTwoParamsWithOnePositionAndAssertReplaceWithPosition()
        {
            var code = @"
namespace N
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
            Assert.Valid(Code.PlaceholderAnalyzer, code);
        }

        [Test]
        public static void DiagnosticsTwoParamsWithOnePositionConst()
        {
            var code = @"
namespace N
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
            Assert.Valid(Code.PlaceholderAnalyzer, code);
        }

        [Test]
        public static void DiagnosticsTwoParamsWithOnePositionInstanceField()
        {
            var code = @"
namespace N
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
            Assert.Valid(Code.PlaceholderAnalyzer, code);
        }

        [Test]
        public static void DiagnosticsArrayWithOnePosition()
        {
            var code = @"
namespace N
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
            Assert.Valid(Code.PlaceholderAnalyzer, code);
        }

        [Test]
        public static void CodeFixOneBefore()
        {
            var code = @"
namespace N
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
            Assert.Valid(Code.PlaceholderAnalyzer, Code.PlaceholderFix, code);
        }

        [Test]
        public static void CodeFixAssertReplace()
        {
            var code = @"
namespace N
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
namespace N
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
namespace N
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
            Assert.Valid(Code.PlaceholderAnalyzer, Code.PlaceholderFix, code);
        }

        [Test]
        public static void WhenAssertReplacingOne()
        {
            var code = @"
namespace N
{
    using Gu.Roslyn.Asserts;
    using NUnit.Framework;

    public static class C
    {
        private static readonly PlaceholderAnalyzer Analyzer = new PlaceholderAnalyzer();

        [TestCase(""↓int"")]
        [TestCase(""↓string"")]
        public static void M(string type)
        {
            var c1 = @""
namespace N
{
    public class C1
    {
    }
}"";

            var code = @""
namespace N
{
    public class C2
    {
        public static void M(string text)
        {
        }
    }
}"".AssertReplace(""string"", type);
            RoslynAssert.Diagnostics(Analyzer, c1, code);
        }
    }
}";
            Assert.Valid(Code.PlaceholderAnalyzer, code);
        }

        [Test]
        public static void WhenAssertReplacingOnePartial()
        {
            var code = @"
namespace N
{
    using Gu.Roslyn.Asserts;
    using NUnit.Framework;

    public static class C
    {
        private static readonly PlaceholderAnalyzer Analyzer = new PlaceholderAnalyzer();

        [TestCase(""↓int"")]
        [TestCase(""↓string"")]
        public static void M(string type)
        {
            var c1Part1 = @""
namespace N
{
    public partial class C1
    {
    }
}"";

            var c1Part2 = @""
namespace N
{
    public partial class C1
    {
        public static void M(string text)
        {
        }
    }
}"".AssertReplace(""string"", type);
            RoslynAssert.Diagnostics(Analyzer, c1Part1, c1Part2);
        }
    }
}";
            Assert.Valid(Code.PlaceholderAnalyzer, code);
        }
    }
}
