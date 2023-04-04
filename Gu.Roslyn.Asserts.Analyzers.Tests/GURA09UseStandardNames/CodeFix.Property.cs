namespace Gu.Roslyn.Asserts.Analyzers.Tests.GURA09UseStandardNames;

using NUnit.Framework;

public static partial class CodeFix
{
    public static class Property
    {
        [TestCase("Foo")]
        [TestCase("Bar")]
        [TestCase("Baz")]
        [TestCase("Lol")]
        public static void WithName(string name)
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
    class C1
    {
        public int ↓Foo { get; }
    }
}"";
            RoslynAssert.Valid(Analyzer, c);
        }
    }
}".AssertReplace("Foo", name);

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
    class C1
    {
        public int P { get; }
    }
}"";
            RoslynAssert.Valid(Analyzer, c);
        }
    }
}";

            Assert.CodeFix(new[] { Code.PlaceholderAnalyzer, before }, new[] { Code.PlaceholderAnalyzer, after }, fixTitle: $"Replace {name} with P");
        }

        [TestCase("public int ↓Foo { get; }", "public int P { get; }")]
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

            Assert.CodeFix(new[] { Code.PlaceholderAnalyzer, before }, new[] { Code.PlaceholderAnalyzer, after }, fixTitle: $"Replace Foo with P");
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

            Assert.CodeFix(new[] { Code.PlaceholderAnalyzer, before }, new[] { Code.PlaceholderAnalyzer, after }, fixTitle: $"Replace Foo with P2");
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
            this.Foo = a;
            this.Values = values;
        }

        public int ↓Foo { get; }

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
            Assert.CodeFix(new[] { Code.PlaceholderAnalyzer, before }, after, fixTitle: $"Replace Foo with P1");
        }

        [Test]
        public static void WithBackingField()
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
        private int foo;

        public C(int ↓foo)
        {
            this.foo = foo;
        }

        public int ↓Foo { get => this.foo; set => this.foo = value; }
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
        private int p;

        public C(int p)
        {
            this.p = p;
        }

        public int P { get => this.p; set => this.p = value; }
    }
}"";
            RoslynAssert.Valid(Analyzer, c);
        }
    }
}";

            Assert.CodeFix(new[] { Code.PlaceholderAnalyzer, before }, new[] { Code.PlaceholderAnalyzer, after }, fixTitle: $"Replace Foo with P");
        }

        [Test]
        public static void UpdatesStringLiteral()
        {
            var before = @"
namespace N
{
    using Gu.Roslyn.Asserts;
    using NUnit.Framework;

    public static class Valid
    {
        private static readonly PlaceholderAnalyzer Analyzer = new PlaceholderAnalyzer();
        private static readonly PlaceholderFix Fix = new PlaceholderFix();

        [Test]
        public static void M()
        {
            var before = @""
namespace N
{
    class C
    {
        public int ↓Foo { get; }
    }
}"";

            var after = @""
namespace N
{
    class C
    {
        public int Value { get; }
    }
}"";
            RoslynAssert.CodeFix(Analyzer, Fix, before, after, fixTitle: ""Foo"");
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
        private static readonly PlaceholderFix Fix = new PlaceholderFix();

        [Test]
        public static void M()
        {
            var before = @""
namespace N
{
    class C
    {
        public int P { get; }
    }
}"";

            var after = @""
namespace N
{
    class C
    {
        public int Value { get; }
    }
}"";
            RoslynAssert.CodeFix(Analyzer, Fix, before, after, fixTitle: ""P"");
        }
    }
}";

            Assert.CodeFix(new[] { Code.PlaceholderAnalyzer, Code.PlaceholderFix, before }, new[] { Code.PlaceholderAnalyzer, Code.PlaceholderFix, after }, fixTitle: $"Replace Foo with P");
        }
    }
}
