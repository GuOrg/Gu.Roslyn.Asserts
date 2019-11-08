namespace Gu.Roslyn.Asserts.Analyzers.Tests.GURA09UseStandardNames
{
    using NUnit.Framework;

    public static partial class CodeFix
    {
        public static class Class
        {
            [TestCase("Foo")]
            [TestCase("Bar")]
            public static void SingleClassNamed(string name)
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
            var foo = ""class ↓Foo { }"";
            RoslynAssert.Valid(Analyzer, foo);
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
            var foo = ""class C { }"";
            RoslynAssert.Valid(Analyzer, foo);
        }
    }
}";

                RoslynAssert.CodeFix(Analyzer, Fix, ExpectedDiagnostic, new[] { Code.PlaceholderAnalyzer, before }, new[] { Code.PlaceholderAnalyzer, after }, fixTitle: $"Replace {name} with C");
            }

            [TestCase("Foo")]
            [TestCase("Bar")]
            public static void SingleMultilineClassNamed(string name)
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
class ↓Foo
{
}"";
            RoslynAssert.Valid(Analyzer, foo);
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
            var foo = @""
class C
{
}"";
            RoslynAssert.Valid(Analyzer, foo);
        }
    }
}";

                RoslynAssert.CodeFix(Analyzer, Fix, ExpectedDiagnostic, new[] { Code.PlaceholderAnalyzer, before }, new[] { Code.PlaceholderAnalyzer, after }, fixTitle: $"Replace {name} with C");
            }

            [TestCase("Foo")]
            [TestCase("Bar")]
            public static void GenericClassNamed(string name)
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
            var foo = ""class ↓Foo<T> { }"";
            RoslynAssert.Valid(Analyzer, foo);
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
        public static void M1()
        {
            var foo = ""class C<T> { }"";
            RoslynAssert.Valid(Analyzer, foo);
        }
    }
}";

                RoslynAssert.CodeFix(Analyzer, Fix, ExpectedDiagnostic, new[] { Code.PlaceholderAnalyzer, before }, new[] { Code.PlaceholderAnalyzer, after }, fixTitle: $"Replace {name} with C");
            }

            [Ignore("Not sure if we want to touch this.")]
            [Test]
            public static void FooException()
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
            var foo = ""class ↓FooException : Exception { }"";
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
        public static void M1()
        {
            var foo = ""class C : Exception { }"";
            RoslynAssert.Valid(Analyzer, foo);
        }
    }
}";

                RoslynAssert.CodeFix(Analyzer, Fix, ExpectedDiagnostic, new[] { Code.PlaceholderAnalyzer, before }, new[] { Code.PlaceholderAnalyzer, after });
            }

            [Test]
            public static void FooWithFactoryMethod()
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
            var foo = @""
class ↓Foo
{
    public static Foo M(Foo ↓foo) => foo;
    public static Foo M() => new C();
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
        public static void M1()
        {
            var foo = @""
class C
{
    public static C M(C foo) => foo;
    public static C M() => new C();
}"";
            RoslynAssert.Valid(Analyzer, foo);
        }
    }
}";

                RoslynAssert.CodeFix(Analyzer, Fix, ExpectedDiagnostic, new[] { Code.PlaceholderAnalyzer, before }, new[] { Code.PlaceholderAnalyzer, after }, fixTitle: $"Replace Foo with C");
            }

            [Test]
            public static void DoesNotTouchWordInTestMethodName()
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
        public static void FooTest()
        {
            var foo = @""
class ↓Foo
{
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
        public static void FooTest()
        {
            var foo = @""
class C
{
}"";
            RoslynAssert.Valid(Analyzer, foo);
        }
    }
}";

                RoslynAssert.CodeFix(Analyzer, Fix, ExpectedDiagnostic, new[] { Code.PlaceholderAnalyzer, before }, new[] { Code.PlaceholderAnalyzer, after }, fixTitle: $"Replace Foo with C");
            }

            [Test]
            public static void DoesNotTouchWordPart()
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
        public static void FooTest()
        {
            var foo = @""
class ↓Foo
{
}"";
            var text = ""FooWord"";
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
        public static void FooTest()
        {
            var foo = @""
class C
{
}"";
            var text = ""FooWord"";
            RoslynAssert.Valid(Analyzer, foo);
        }
    }
}";

                RoslynAssert.CodeFix(Analyzer, Fix, ExpectedDiagnostic, new[] { Code.PlaceholderAnalyzer, before }, new[] { Code.PlaceholderAnalyzer, after }, fixTitle: $"Replace Foo with C");
            }

            [Test]
            public static void C1AndBarInSameDocument()
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
            var code = @""
class C1 { }
class ↓Bar { }"";
            RoslynAssert.Valid(Analyzer, code);
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
            var code = @""
class C1 { }
class C2 { }"";
            RoslynAssert.Valid(Analyzer, code);
        }
    }
}";

                RoslynAssert.CodeFix(Analyzer, Fix, ExpectedDiagnostic, new[] { Code.PlaceholderAnalyzer, before }, new[] { Code.PlaceholderAnalyzer, after }, fixTitle: $"Replace Bar with C2");
            }

            [Test]
            public static void C1AndBarInDifferentDocuments()
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
            var c1 = ""class C1 { }"";
            var bar = ""class ↓Bar { }"";
            RoslynAssert.Valid(Analyzer, c1, bar);
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
            var c1 = ""class C1 { }"";
            var bar = ""class C2 { }"";
            RoslynAssert.Valid(Analyzer, c1, bar);
        }
    }
}";

                RoslynAssert.CodeFix(Analyzer, Fix, ExpectedDiagnostic, new[] { Code.PlaceholderAnalyzer, before }, new[] { Code.PlaceholderAnalyzer, after }, fixTitle: $"Replace Bar with C2");
            }

            [Test]
            public static void C1C2AndBaz()
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
            var c1 = ""class C1 { }"";
            var c2 = ""class C2 { }"";
            var baz = ""class ↓Baz { }"";
            RoslynAssert.Valid(Analyzer, c1, c2, baz);
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
            var c1 = ""class C1 { }"";
            var c2 = ""class C2 { }"";
            var baz = ""class C3 { }"";
            RoslynAssert.Valid(Analyzer, c1, c2, baz);
        }
    }
}";

                RoslynAssert.CodeFix(Analyzer, Fix, ExpectedDiagnostic, new[] { Code.PlaceholderAnalyzer, before }, new[] { Code.PlaceholderAnalyzer, after }, fixTitle: $"Replace Baz with C3");
            }
        }
    }
}
