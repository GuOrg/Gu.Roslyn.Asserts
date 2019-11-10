namespace Gu.Roslyn.Asserts.Analyzers.Tests.GURA03NameShouldMatchCode
{
    using Microsoft.CodeAnalysis.CodeFixes;
    using Microsoft.CodeAnalysis.Diagnostics;
    using NUnit.Framework;

    public static class CodeFix
    {
        private static readonly DiagnosticAnalyzer Analyzer = new InvocationAnalyzer();
        private static readonly CodeFixProvider Fix = new RenameFix();
        private static readonly ExpectedDiagnostic ExpectedDiagnostic = ExpectedDiagnostic.Create(Descriptors.GURA03NameShouldMatchCode);

        [TestCase("class C1 { }", "private const", "C1")]
        [TestCase("class C1 { }", "private static readonly", "C1")]
        [TestCase("class C1 { }", "const", "C1")]
        [TestCase("public class C1 { }", "private const", "C1")]
        public static void FieldClassWhenSingleArgument(string declaration, string modifiers, string name)
        {
            var before = @"
namespace N
{
    using Gu.Roslyn.Asserts;
    using NUnit.Framework;

    public static class C
    {
        private static readonly PlaceholderAnalyzer Analyzer = new PlaceholderAnalyzer();

        private const string ↓WRONG = @""
namespace N
{
    public class C1 { }
}"";

        [Test]
        public static void M()
        {
            RoslynAssert.Valid(Analyzer, WRONG);
        }
    }
}".AssertReplace("public class C1 { }", declaration)
  .AssertReplace("private const", modifiers);

            var after = @"
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
            RoslynAssert.Valid(Analyzer, C1);
        }
    }
}".AssertReplace("public class C1 { }", declaration)
  .AssertReplace("private const", modifiers)
  .AssertReplace("C1", name);
            RoslynAssert.CodeFix(Analyzer, Fix, ExpectedDiagnostic, new[] { Code.PlaceholderAnalyzer, before }, after);
        }

        [Test]
        public static void LocalWhenSingleArgument()
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
            var ↓wrong = ""class C { }"";
            RoslynAssert.Valid(Analyzer, wrong);
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
            var c = ""class C { }"";
            RoslynAssert.Valid(Analyzer, c);
        }
    }
}";
            RoslynAssert.CodeFix(Analyzer, Fix, ExpectedDiagnostic, new[] { Code.PlaceholderAnalyzer, before }, after);
        }

        [Test]
        public static void LocalWhenAssertReplace()
        {
            var before = @"
namespace N
{
    using Gu.Roslyn.Asserts;
    using NUnit.Framework;

    public static class C
    {
        private static readonly PlaceholderAnalyzer Analyzer = new PlaceholderAnalyzer();

        [TestCase(""int"")]
        public static void M(string type)
        {
            var ↓wrong = ""class C { private int f }"".AssertReplace(""int"", type);
            RoslynAssert.Valid(Analyzer, wrong);
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

        [TestCase(""int"")]
        public static void M(string type)
        {
            var c = ""class C { private int f }"".AssertReplace(""int"", type);
            RoslynAssert.Valid(Analyzer, c);
        }
    }
}";
            RoslynAssert.CodeFix(Analyzer, Fix, ExpectedDiagnostic, new[] { Code.PlaceholderAnalyzer, before }, after);
        }

        [Test]
        public static void FieldClassWhenParams()
        {
            var before = @"
namespace N
{
    using Gu.Roslyn.Asserts;
    using NUnit.Framework;

    public static class C
    {
        private static readonly PlaceholderAnalyzer Analyzer = new PlaceholderAnalyzer();

        private const string ↓WRONG = @""
namespace N
{
    public class C1 { }
}"";

        [Test]
        public static void M()
        {
            var c2 = ""class C2 { }"";
            RoslynAssert.Valid(Analyzer, WRONG, c2);
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
}";
            RoslynAssert.CodeFix(Analyzer, Fix, ExpectedDiagnostic, new[] { Code.PlaceholderAnalyzer, before }, after);
        }

        [Test]
        public static void FieldClassWhenTwoParams()
        {
            var before = @"
namespace N
{
    using Gu.Roslyn.Asserts;
    using NUnit.Framework;

    public static class C
    {
        private static readonly PlaceholderAnalyzer Analyzer = new PlaceholderAnalyzer();

        private const string ↓WRONG = @""
namespace N
{
    public class C1 { }
}"";

        [Test]
        public static void M1()
        {
            var c2 = ""class C2 { }"";
            RoslynAssert.Valid(Analyzer, WRONG, c2);
        }

        [Test]
        public static void M2()
        {
            var c2 = ""class C2 { }"";
            RoslynAssert.Valid(Analyzer, WRONG, c2);
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

        private const string C1 = @""
namespace N
{
    public class C1 { }
}"";

        [Test]
        public static void M1()
        {
            var c2 = ""class C2 { }"";
            RoslynAssert.Valid(Analyzer, C1, c2);
        }

        [Test]
        public static void M2()
        {
            var c2 = ""class C2 { }"";
            RoslynAssert.Valid(Analyzer, C1, c2);
        }
    }
}";
            RoslynAssert.FixAll(Analyzer, Fix, ExpectedDiagnostic, new[] { Code.PlaceholderAnalyzer, before }, after);
        }

        [Test]
        public static void ClassWhenArray()
        {
            var before = @"
namespace N
{
    using Gu.Roslyn.Asserts;
    using NUnit.Framework;

    public static class C
    {
        private static readonly PlaceholderAnalyzer Analyzer = new PlaceholderAnalyzer();

        private const string ↓WRONG = @""
namespace N
{
    public class C1 { }
}"";

        [Test]
        public static void M()
        {
            var c2 = ""class C2 { }"";
            RoslynAssert.Valid(Analyzer, new [] { WRONG, c2 });
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

        private const string C1 = @""
namespace N
{
    public class C1 { }
}"";

        [Test]
        public static void M()
        {
            var c2 = ""class C2 { }"";
            RoslynAssert.Valid(Analyzer, new [] { C1, c2 });
        }
    }
}";
            RoslynAssert.CodeFix(Analyzer, Fix, ExpectedDiagnostic, new[] { Code.PlaceholderAnalyzer, before }, after);
        }
    }
}
