namespace Gu.Roslyn.Asserts.Analyzers.Tests.GURA09UseStandardNames
{
    using NUnit.Framework;

    public static partial class CodeFix
    {
        public static class Property
        {
            [TestCase("public int ↓Foo { get; }", "public int P { get; }")]
            [TestCase("public int ↓A { get; }", "public int P { get; }")]
            [TestCase("public int ↓Foo { get; set; }", "public int P { get; set; }")]
            public static void Single(string declarationBefore, string declarationAfter)
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
        public static void M()
        {
            var c = @""
namespace N
{
    class C
    {
        public int ↓Foo { get; }
    }
}"";
            RoslynAssert.Valid(Analyzer, c);
        }
    }
}".AssertReplace("public int ↓Foo { get; }", declarationBefore);

                var after = @"
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
            var c = @""
namespace N
{
    class C
    {
        public int P { get; }
    }
}"";
            RoslynAssert.Valid(Analyzer, c);
        }
    }
}".AssertReplace("public int P { get; }", declarationAfter);

                RoslynAssert.CodeFix(Analyzer, Fix, ExpectedDiagnostic, new[] { Code.PlaceholderAnalyzer, before }, new[] { Code.PlaceholderAnalyzer, after });
            }

            [Test]
            public static void NextProperty()
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
        public static void M()
        {
            var c = @""
namespace N
{
    class C
    {
        public int P1 { get; }

        public int ↓Foo { get; }
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
        public static void M()
        {
            var c = @""
namespace N
{
    class C
    {
        public int P1 { get; }

        public int P2 { get; }
    }
}"";
            RoslynAssert.Valid(Analyzer, c);
        }
    }
}";

                RoslynAssert.CodeFix(Analyzer, Fix, ExpectedDiagnostic, new[] { Code.PlaceholderAnalyzer, before }, new[] { Code.PlaceholderAnalyzer, after });
            }

            [Test]
            public static void ClassNamedFooWithPropertyNamedC()
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
        public static void M()
        {
            var foo = @""
namespace N
{
    public class ↓Foo
    {
        public int ↓C => 1;
    }
}"";
            RoslynAssert.Valid(Analyzer, foo);
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
        public static void M()
        {
            var foo = @""
namespace N
{
    public class Foo
    {
        public int P => 1;
    }
}"";
            RoslynAssert.Valid(Analyzer, foo);
        }
    }
}";
                RoslynAssert.CodeFix(Analyzer, Fix, ExpectedDiagnostic, new[] { Code.PlaceholderAnalyzer, before }, after);
            }

            [Test]
            public static void ClassNamedFooWithExpressionBodyPropertyNamedC()
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
        public static void M()
        {
            var foo = @""
namespace N
{
    public class ↓Foo
    {
        public int ↓C => 1;
    }
}"";
            RoslynAssert.Valid(Analyzer, foo);
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
        public static void M()
        {
            var foo = @""
namespace N
{
    public class Foo
    {
        public int P => 1;
    }
}"";
            RoslynAssert.Valid(Analyzer, foo);
        }
    }
}";
                RoslynAssert.CodeFix(Analyzer, Fix, ExpectedDiagnostic, new[] { Code.PlaceholderAnalyzer, before }, after);
            }

            [Test]
            public static void PropertyInBaseClass()
            {
                var before = @"
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
            var c1 = @""
namespace N
{
    public class C1
    {
        public C1(int a, params int[] values)
        {
            this.A = a;
            this.Values = values;
        }

        public int ↓A { get; }

        public int[] Values { get; }
    }
}"";
            var c2 = @""
namespace N
{
    public class C2 : C1
    {
        public C2(int a, int b, int c, int d)
            : base(a, b, c, d)
        {
        }
    }
}"";

            RoslynAssert.Valid(Analyzer, c1, c2);
        }
    }
}";

                var after = @"
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
            var c1 = @""
namespace N
{
    public class C1
    {
        public C1(int a, params int[] values)
        {
            this.P1 = a;
            this.Values = values;
        }

        public int P1 { get; }

        public int[] Values { get; }
    }
}"";
            var c2 = @""
namespace N
{
    public class C2 : C1
    {
        public C2(int a, int b, int c, int d)
            : base(a, b, c, d)
        {
        }
    }
}"";

            RoslynAssert.Valid(Analyzer, c1, c2);
        }
    }
}";
                RoslynAssert.CodeFix(Analyzer, Fix, ExpectedDiagnostic, new[] { Code.PlaceholderAnalyzer, before }, after);
            }
        }
    }
}
