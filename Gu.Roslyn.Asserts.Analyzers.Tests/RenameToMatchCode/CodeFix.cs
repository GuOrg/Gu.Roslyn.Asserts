namespace Gu.Roslyn.Asserts.Analyzers.Tests.RenameToMatchCode
{
    using Microsoft.CodeAnalysis.CodeFixes;
    using Microsoft.CodeAnalysis.Diagnostics;
    using NUnit.Framework;

    public static class CodeFix
    {
        private static readonly DiagnosticAnalyzer Analyzer = new InvocationAnalyzer();
        private static readonly CodeFixProvider Fix = new RenameFix();
        private static readonly ExpectedDiagnostic ExpectedDiagnostic = ExpectedDiagnostic.Create(Descriptors.NameShouldMatchCode);

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


        [Test]
        public static void WhenLocalNameMatchesParameter()
        {
            var before = @"
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
            var ↓code = ""class C2 { }"";
            RoslynAssert.Valid(Analyzer, C1, code);
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
    }
}
