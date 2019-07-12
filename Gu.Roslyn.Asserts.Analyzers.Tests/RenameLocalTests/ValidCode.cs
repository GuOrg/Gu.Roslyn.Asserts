namespace Gu.Roslyn.Asserts.Analyzers.Tests.RenameLocalTests
{
    using Microsoft.CodeAnalysis.Diagnostics;
    using NUnit.Framework;

    public static class ValidCode
    {
        private static readonly DiagnosticAnalyzer Analyzer = new ArgumentAnalyzer();

        [Test]
        public static void WhenAnalyzerCorrectName()
        {
            var code = @"
namespace RoslynSandbox
{
    using Gu.Roslyn.Asserts;
    using NUnit.Framework;

    public static class C
    {
        private static readonly PlaceholderAnalyzer Analyzer = new PlaceholderAnalyzer();

        [Test]
        public static void M()
        {
            var code = ""class ↓C { }"";
            RoslynAssert.Valid(Analyzer, code);
        }
    }
}";
            RoslynAssert.Valid(Analyzer, Code.PlaceholderAnalyzer, code);
        }

        [Test]
        public static void DoNotWarnWhenTwoParams()
        {
            var code = @"
namespace RoslynSandbox
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
            var code2 = ""class C { }"";
            RoslynAssert.Valid(Analyzer, code1, code2);
        }
    }
}";

            RoslynAssert.Valid(Analyzer, Code.PlaceholderAnalyzer, code);
        }

        [Test]
        public static void CodeFixOneParamWithPosition()
        {
            var code = @"
namespace RoslynSandbox
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
            RoslynAssert.Valid(Analyzer, Code.PlaceholderAnalyzer, code);
        }

        [Test]
        public static void CodeFixTwoParamsWithOnePosition()
        {
            var code = @"
namespace RoslynSandbox
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
            RoslynAssert.Diagnostics(Analyzer, code1, code);
        }
    }
}";
            RoslynAssert.Valid(Analyzer, Code.PlaceholderAnalyzer, code);
        }

        [Test]
        public static void CodeFixTwoParamsWithOnePositionConst()
        {
            var code = @"
namespace RoslynSandbox
{
    using Gu.Roslyn.Asserts;
    using NUnit.Framework;

    public static class C
    {
        private static readonly PlaceholderAnalyzer Analyzer = new PlaceholderAnalyzer();
        const string code1 = ""class C { }"";

        [Test]
        public static void M()
        {
            var code = ""↓class C { }"";
            RoslynAssert.Diagnostics(Analyzer, code1, code);
        }
    }
}";
            RoslynAssert.Valid(Analyzer, Code.PlaceholderAnalyzer, code);
        }

        [Test]
        public static void CodeFixTwoParamsWithOnePositionInstanceField()
        {
            var code = @"
namespace RoslynSandbox
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
            RoslynAssert.Valid(Analyzer, Code.PlaceholderAnalyzer, code);
        }

        [Test]
        public static void CodeFixArrayWithOnePosition()
        {
            var code = @"
namespace RoslynSandbox
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
            RoslynAssert.Valid(Analyzer, Code.PlaceholderAnalyzer, code);
        }

        [Test]
        public static void CodeFixOneBefore()
        {
            var code = @"
namespace RoslynSandbox
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
            RoslynAssert.Valid(Analyzer, Code.PlaceholderAnalyzer, Code.PlaceholderFix, code);
        }

        [Test]
        public static void Refactoring()
        {
            var code = @"
namespace RoslynSandbox
{
    using Gu.Roslyn.Asserts;
    using NUnit.Framework;

    public static class C
    {
        private static readonly PlaceholderRefactoring Refactoring = new PlaceholderRefactoring();

        [Test]
        public static void M()
        {
            var before = @""
namespace RoslynSandbox
{
    public static class C
    {
        public static void Test()
        {
            var text = """"↓\na"""";
        }
    }
}"";

            var after = @""
namespace RoslynSandbox
{
    public static class C
    {
        public static void Test()
        {
            var text = """"\n"""" +
                       """"a"""";
        }
    }
}"";
            RoslynAssert.Refactoring(Refactoring, before, after);
        }
    }
}";
            RoslynAssert.Valid(Analyzer, Code.PlaceholderRefactoring, code);
        }
    }
}
