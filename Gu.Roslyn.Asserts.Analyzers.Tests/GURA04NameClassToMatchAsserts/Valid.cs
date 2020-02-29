namespace Gu.Roslyn.Asserts.Analyzers.Tests.GURA04NameClassToMatchAsserts
{
    using NUnit.Framework;

    public static class Valid
    {
        private static readonly DiagnosticAssert Assert = RoslynAssert.Create<ClassDeclarationAnalyzer>();

        [Test]
        public static void WhenNoAsserts()
        {
            var code = @"
namespace N
{
    using System;
    using NUnit.Framework;

    public static class C
    {
        private static readonly PlaceholderAnalyzer Analyzer = new PlaceholderAnalyzer();

        [Test]
        public static void M()
        {
            var c = ""class C { }"";
            Console.WriteLine(c);
        }
    }
}";
            Assert.Valid(Code.PlaceholderAnalyzer, code);
        }

        [Test]
        public static void WhenOneValid()
        {
            var code = @"
namespace N
{
    using Gu.Roslyn.Asserts;
    using NUnit.Framework;

    public static class Valid
    {
        private static readonly PlaceholderAnalyzer Analyzer = new PlaceholderAnalyzer();

        [Test]
        public static void M()
        {
            var c = ""class C { }"";
            RoslynAssert.Valid(Analyzer, c);
        }
    }
}";
            Assert.Valid(Code.PlaceholderAnalyzer, code);
        }

        [Test]
        public static void WhenOneValidInNestedClass()
        {
            var code = @"
namespace N
{
    using Gu.Roslyn.Asserts;
    using NUnit.Framework;

    public static class Valid
    {
        private static readonly PlaceholderAnalyzer Analyzer = new PlaceholderAnalyzer();

        public static class Class
        {
            [Test]
            public static void M()
            {
                var c = ""class C { }"";
                RoslynAssert.Valid(Analyzer, c);
            }
        }
    }
}";
            Assert.Valid(Code.PlaceholderAnalyzer, code);
        }

        [Test]
        public static void WhenTwoValid()
        {
            var code = @"
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
            var c = ""class C { }"";
            RoslynAssert.Valid(Analyzer, c);
        }

        [Test]
        public static void M2()
        {
            var c = ""class C { }"";
            RoslynAssert.Valid(Analyzer, c);
        }
    }
}";
            Assert.Valid(Code.PlaceholderAnalyzer, code);
        }

        [Test]
        public static void WhenMix()
        {
            var code = @"
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
            var c = ""class C { }"";
            RoslynAssert.Valid(Analyzer, c);
        }

        [Test]
        public static void M2()
        {
            var c = ""class C { }"";
            RoslynAssert.Diagnostics(Analyzer, c);
        }
    }
}";
            Assert.Valid(Code.PlaceholderAnalyzer, code);
        }
    }
}
