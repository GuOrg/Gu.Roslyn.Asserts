namespace Gu.Roslyn.Asserts.Analyzers.Tests.GURA09UseStandardNames;

using NUnit.Framework;

public static partial class CodeFix
{
    public static class Method
    {
        [Test]
        public static void Bar()
        {
            var before = @"
namespace N
{
    using Gu.Roslyn.Asserts;
    using NUnit.Framework;

    public static class Valid
    {
        private static readonly PlaceholderAnalyzer Analyzer = new PlaceholderAnalyzer();

        [Test]
        public static void M1()
        {
            var c = @""
namespace N
{
    class C
    {
        void ↓Bar(int i) { }
    }
}"";
            RoslynAssert.Valid(Analyzer, c);
        }
    }
}";

            var after = @"
namespace N
{
    using Gu.Roslyn.Asserts;
    using NUnit.Framework;

    public static class Valid
    {
        private static readonly PlaceholderAnalyzer Analyzer = new PlaceholderAnalyzer();

        [Test]
        public static void M1()
        {
            var c = @""
namespace N
{
    class C
    {
        void M(int i) { }
    }
}"";
            RoslynAssert.Valid(Analyzer, c);
        }
    }
}";

            Assert.CodeFix(new[] { Code.PlaceholderAnalyzer, before }, new[] { Code.PlaceholderAnalyzer, after }, fixTitle: $"Replace Bar with M");
        }

        [Test]
        public static void BarGeneric()
        {
            var before = @"
namespace N
{
    using Gu.Roslyn.Asserts;
    using NUnit.Framework;

    public static class Valid
    {
        private static readonly PlaceholderAnalyzer Analyzer = new PlaceholderAnalyzer();

        [Test]
        public static void M1()
        {
            var c = @""
namespace N
{
    class C
    {
        void ↓Bar<T>(T i) { }
    }
}"";
            RoslynAssert.Valid(Analyzer, c);
        }
    }
}";

            var after = @"
namespace N
{
    using Gu.Roslyn.Asserts;
    using NUnit.Framework;

    public static class Valid
    {
        private static readonly PlaceholderAnalyzer Analyzer = new PlaceholderAnalyzer();

        [Test]
        public static void M1()
        {
            var c = @""
namespace N
{
    class C
    {
        void M<T>(T i) { }
    }
}"";
            RoslynAssert.Valid(Analyzer, c);
        }
    }
}";

            Assert.CodeFix(new[] { Code.PlaceholderAnalyzer, before }, new[] { Code.PlaceholderAnalyzer, after }, fixTitle: $"Replace Bar with M");
        }

        [Test]
        public static void BarOverloads()
        {
            var before = @"
namespace N
{
    using Gu.Roslyn.Asserts;
    using NUnit.Framework;

    public static class Valid
    {
        private static readonly PlaceholderAnalyzer Analyzer = new PlaceholderAnalyzer();

        [Test]
        public static void M1()
        {
            var c = @""
namespace N
{
    class C
    {
        void ↓Bar(int _) { }
        void ↓Bar(double _) { }
    }
}"";
            RoslynAssert.Valid(Analyzer, c);
        }
    }
}";

            var after = @"
namespace N
{
    using Gu.Roslyn.Asserts;
    using NUnit.Framework;

    public static class Valid
    {
        private static readonly PlaceholderAnalyzer Analyzer = new PlaceholderAnalyzer();

        [Test]
        public static void M1()
        {
            var c = @""
namespace N
{
    class C
    {
        void M(int _) { }
        void M(double _) { }
    }
}"";
            RoslynAssert.Valid(Analyzer, c);
        }
    }
}";

            Assert.FixAll(new[] { Code.PlaceholderAnalyzer, before }, new[] { Code.PlaceholderAnalyzer, after }, fixTitle: $"Replace Bar with M");
        }
    }
}
