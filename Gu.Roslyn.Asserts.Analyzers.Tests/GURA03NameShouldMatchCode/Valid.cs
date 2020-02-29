namespace Gu.Roslyn.Asserts.Analyzers.Tests.GURA03NameShouldMatchCode
{
    using NUnit.Framework;

    public static class Valid
    {
        private static readonly DiagnosticAssert Assert = RoslynAssert.Create<InvocationAnalyzer>(Descriptors.GURA03NameShouldMatchCode);

        [TestCase("class C1 { }", "private const string C1")]
        [TestCase("class C1 { }", "private static readonly string C1")]
        [TestCase("class C1 { }", "const string C1")]
        [TestCase("public class C1 { }", "private const string C1")]
        [TestCase("public partial class C1 { }", "private const string C1")]
        public static void Class(string declaration, string field)
        {
            var code = @"
namespace N
{
    using Gu.Roslyn.Asserts;
    using NUnit.Framework;

    public static class C
    {
        private static readonly PlaceholderAnalyzer Analyzer = new PlaceholderAnalyzer();

        private const string C1 = @""
namespace N
{
    public class C1 { }
}"";

        [Test]
        public static void M()
        {
            var c2 = ""class C2 { }"";
            RoslynAssert.Valid(Analyzer, C1, c2);
        }
    }
}".AssertReplace("public class C1 { }", declaration)
  .AssertReplace("private const string C1", field);
            Assert.Valid(Code.PlaceholderAnalyzer, code);
        }

        [Test]
        public static void GenericClassOneTypeParameters()
        {
            var code = @"
namespace N
{
    using Gu.Roslyn.Asserts;
    using NUnit.Framework;

    public static class C
    {
        private static readonly PlaceholderAnalyzer Analyzer = new PlaceholderAnalyzer();

        private const string COfT = @""
namespace N
{
    public partial class C<T> { }
}"";

        [Test]
        public static void M()
        {
            var c2OfT = ""class C2<T> { }"";
            RoslynAssert.Valid(Analyzer, COfT, c2OfT);
        }
    }
}";
            Assert.Valid(Code.PlaceholderAnalyzer, code);
        }

        [Test]
        public static void GenericClassWithTwoTypeParameters()
        {
            var code = @"
namespace N
{
    using Gu.Roslyn.Asserts;
    using NUnit.Framework;

    public static class C
    {
        private static readonly PlaceholderAnalyzer Analyzer = new PlaceholderAnalyzer();

        private const string COfT1T2 = @""
namespace N
{
    public partial class C1<T1, T2> { }
}"";

        [Test]
        public static void M()
        {
            var c2OfT1T2 = ""class C2<T1, T2> { }"";
            RoslynAssert.Valid(Analyzer, COfT1T2, c2OfT1T2);
        }
    }
}";
            Assert.Valid(Code.PlaceholderAnalyzer, code);
        }

        [Test]
        public static void Struct()
        {
            var code = @"
namespace N
{
    using Gu.Roslyn.Asserts;
    using NUnit.Framework;

    public static class C
    {
        private static readonly PlaceholderAnalyzer Analyzer = new PlaceholderAnalyzer();

        private const string S = @""
namespace N
{
    public struct S { }
}"";

        [Test]
        public static void M()
        {
            var c2 = ""class C2 { }"";
            RoslynAssert.Valid(Analyzer, S, c2);
        }
    }
}";
            Assert.Valid(Code.PlaceholderAnalyzer, code);
        }

        [Test]
        public static void IgnorePartialClass()
        {
            var code = @"
namespace N
{
    using Gu.Roslyn.Asserts;
    using NUnit.Framework;

    public static class C
    {
        private static readonly PlaceholderAnalyzer Analyzer = new PlaceholderAnalyzer();

        private const string C1Part1 = @""
namespace N
{
    public partial class C1 { }
}"";

        private const string C1Part2 = @""
namespace N
{
    public partial class C1 { }
}"";

        [Test]
        public static void M()
        {
            var c2 = ""class C2 { }"";
            RoslynAssert.Valid(Analyzer, C1Part1, C1Part2, c2);
        }
    }
}";
            Assert.Valid(Code.PlaceholderAnalyzer, code);
        }

        [Test]
        public static void Inline()
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
            RoslynAssert.Valid(Analyzer, ""class C2 { }"");
        }
    }
}";
            Assert.Valid(Code.PlaceholderAnalyzer, code);
        }

        [Test]
        public static void OneParam()
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
            RoslynAssert.Valid(Analyzer, c1);
        }
    }
}";

            Assert.Valid(Code.PlaceholderAnalyzer, code);
        }

        [Test]
        public static void TwoParams()
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
            var c2 = ""class C2 { }"";
            RoslynAssert.Valid(Analyzer, c1, c2);
        }
    }
}";

            Assert.Valid(Code.PlaceholderAnalyzer, code);
        }

        [Test]
        public static void TwoParamsExplicitArray()
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
            var c2 = ""class C2 { }"";
            RoslynAssert.Valid(Analyzer, new []{ c1, c2 });
        }
    }
}";

            Assert.Valid(Code.PlaceholderAnalyzer, code);
        }

        [Test]
        public static void InlineStringEmptyParam()
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
            RoslynAssert.Valid(Analyzer, string.Empty);
        }
    }
}";

            Assert.Valid(Code.PlaceholderAnalyzer, code);
        }

        [TestCase("string.Empty")]
        [TestCase("\"\"")]
        [TestCase("\"SYNTAX_ERROR\"")]
        public static void WhenNoNameInCode(string expression)
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
            var empty = string.Empty;
            RoslynAssert.Valid(Analyzer, empty);
        }
    }
}".AssertReplace("string.Empty", expression);

            Assert.Valid(Code.PlaceholderAnalyzer, code);
        }

        [Test]
        public static void WhenLocalNameMatchesParameter()
        {
            var code = @"
namespace N
{
    using Gu.Roslyn.Asserts;
    using NUnit.Framework;

    public static class C
    {
        private static readonly PlaceholderAnalyzer Analyzer = new PlaceholderAnalyzer();

        private const string C1 = @""
namespace N
{
    public class C1 { }
}"";

        [Test]
        public static void M()
        {
            var code = ""class C2 { }"";
            RoslynAssert.Valid(Analyzer, C1, code);
        }
    }
}";

            Assert.Valid(new[] { Code.PlaceholderAnalyzer, code });
        }
    }
}
