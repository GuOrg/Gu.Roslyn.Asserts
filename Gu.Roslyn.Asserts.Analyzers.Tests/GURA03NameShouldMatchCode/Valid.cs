namespace Gu.Roslyn.Asserts.Analyzers.Tests.GURA03NameShouldMatchCode
{
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.Diagnostics;
    using NUnit.Framework;

    public static class Valid
    {
        private static readonly DiagnosticAnalyzer Analyzer = new InvocationAnalyzer();
        private static readonly DiagnosticDescriptor Descriptor = Descriptors.GURA03NameShouldMatchCode;

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
            RoslynAssert.Valid(Analyzer, Descriptor, Code.PlaceholderAnalyzer, code);
        }

        [Test]
        public static void GenericClass()
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
            var c2 = ""class C2 { }"";
            RoslynAssert.Valid(Analyzer, COfT, c2);
        }
    }
}";
            RoslynAssert.Valid(Analyzer, Descriptor, Code.PlaceholderAnalyzer, code);
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
            RoslynAssert.Valid(Analyzer, Descriptor, Code.PlaceholderAnalyzer, code);
        }

        [Explicit("Not sure this is worth handling.")]
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
            RoslynAssert.Valid(Analyzer, Descriptor, Code.PlaceholderAnalyzer, code);
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
            RoslynAssert.Valid(Analyzer, Descriptor, Code.PlaceholderAnalyzer, code);
        }

        [Test]
        public static void WhenAssertReplace()
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
            var anything = ""class C { }"".AssertReplace(""class C { }"", declaration);
            RoslynAssert.Valid(Analyzer, anything);
        }
    }
}";

            RoslynAssert.Valid(Analyzer, Descriptor, Code.PlaceholderAnalyzer, Code.PlaceholderFix, code);
        }
    }
}
