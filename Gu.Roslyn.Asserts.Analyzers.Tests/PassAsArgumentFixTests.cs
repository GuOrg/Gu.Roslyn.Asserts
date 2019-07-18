namespace Gu.Roslyn.Asserts.Analyzers.Tests
{
    using Microsoft.CodeAnalysis.CodeFixes;
    using NUnit.Framework;

    public static class PassAsArgumentFixTests
    {
        private static readonly CodeFixProvider Fix = new PassAsArgumentFix();
        private static readonly ExpectedDiagnostic ExpectedDiagnostic = ExpectedDiagnostic.Create("CS0618");

        [Test]
        public static void ValidCreateField()
        {
            var before = @"
namespace N
{
    using Gu.Roslyn.Asserts;
    using NUnit.Framework;

    public static class Valid
    {
        [Test]
        public static void M()
        {
            var code = ""class C { }"";
            ↓RoslynAssert.Valid<PlaceholderAnalyzer>(code);
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
            var code = ""class C { }"";
            RoslynAssert.Valid(Analyzer, code);
        }
    }
}";
            RoslynAssert.CodeFix(Fix, ExpectedDiagnostic, new[] { Code.PlaceholderAnalyzer, before }, after, fixTitle: "Create and use field 'Analyzer'.");
        }

        [Test]
        public static void ValidCreateFieldFixAll()
        {
            var before = @"
namespace N
{
    using Gu.Roslyn.Asserts;
    using NUnit.Framework;

    public static class Valid
    {
        [Test]
        public static void M1()
        {
            var code = ""class C { }"";
            ↓RoslynAssert.Valid<PlaceholderAnalyzer>(code);
        }

        [Test]
        public static void M2()
        {
            var code = ""class C { }"";
            ↓RoslynAssert.Valid<PlaceholderAnalyzer>(code);
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
            var code = ""class C { }"";
            RoslynAssert.Valid(Analyzer, code);
        }

        [Test]
        public static void M2()
        {
            var code = ""class C { }"";
            RoslynAssert.Valid(Analyzer, code);
        }
    }
}";
            RoslynAssert.FixAll(Fix, ExpectedDiagnostic, new[] { Code.PlaceholderAnalyzer, before }, after);
        }

        [Test]
        public static void ValidUseExistingStaticField()
        {
            var before = @"
namespace N
{
    using Microsoft.CodeAnalysis.Diagnostics;
    using Gu.Roslyn.Asserts;
    using NUnit.Framework;

    public static class C
    {
        private static readonly DiagnosticAnalyzer Analyzer = new PlaceholderAnalyzer();

        [Test]
        public static void M()
        {
            var code = ""class C { }"";
            ↓RoslynAssert.Valid<PlaceholderAnalyzer>(code);
        }
    }
}";

            var after = @"
namespace N
{
    using Microsoft.CodeAnalysis.Diagnostics;
    using Gu.Roslyn.Asserts;
    using NUnit.Framework;

    public static class C
    {
        private static readonly DiagnosticAnalyzer Analyzer = new PlaceholderAnalyzer();

        [Test]
        public static void M()
        {
            var code = ""class C { }"";
            RoslynAssert.Valid(Analyzer, code);
        }
    }
}";
            RoslynAssert.CodeFix(Fix, ExpectedDiagnostic, new[] { Code.PlaceholderAnalyzer, before }, after, fixTitle: "Use 'Analyzer'.");
        }

        [Test]
        public static void ValidUseExistingInstanceField()
        {
            var before = @"
namespace N
{
    using Microsoft.CodeAnalysis.Diagnostics;
    using Gu.Roslyn.Asserts;
    using NUnit.Framework;

    public class C
    {
        private readonly DiagnosticAnalyzer Analyzer = new PlaceholderAnalyzer();

        [Test]
        public void M()
        {
            var code = ""class C { }"";
            ↓RoslynAssert.Valid<PlaceholderAnalyzer>(code);
        }
    }
}";

            var after = @"
namespace N
{
    using Microsoft.CodeAnalysis.Diagnostics;
    using Gu.Roslyn.Asserts;
    using NUnit.Framework;

    public class C
    {
        private readonly DiagnosticAnalyzer Analyzer = new PlaceholderAnalyzer();

        [Test]
        public void M()
        {
            var code = ""class C { }"";
            RoslynAssert.Valid(this.Analyzer, code);
        }
    }
}";
            RoslynAssert.CodeFix(Fix, ExpectedDiagnostic, new[] { Code.PlaceholderAnalyzer, before }, after, fixTitle: "Use 'Analyzer'.");
        }

        [Test]
        public static void CodeFixOnlyUseExistingStaticField()
        {
            var before = @"
namespace N
{
    using Microsoft.CodeAnalysis.CodeFixes;
    using Gu.Roslyn.Asserts;
    using NUnit.Framework;

    public static class C
    {
        private static readonly CodeFixProvider Fix = new PlaceholderFix();

        [Test]
        public static void M()
        {
            var before = ""class C { }"";
            var after = ""class C { }"";
            ↓RoslynAssert.CodeFix<PlaceholderFix>(ExpectedDiagnostic.Create(""123""), before, after);
        }
    }
}";

            var after = @"
namespace N
{
    using Microsoft.CodeAnalysis.CodeFixes;
    using Gu.Roslyn.Asserts;
    using NUnit.Framework;

    public static class C
    {
        private static readonly CodeFixProvider Fix = new PlaceholderFix();

        [Test]
        public static void M()
        {
            var before = ""class C { }"";
            var after = ""class C { }"";
            RoslynAssert.CodeFix(Fix, ExpectedDiagnostic.Create(""123""), before, after);
        }
    }
}";
            RoslynAssert.CodeFix(Fix, ExpectedDiagnostic, new[] { Code.PlaceholderAnalyzer, Code.PlaceholderFix, before }, after, fixTitle: "Use 'Fix'.");
        }

        [Test]
        public static void CodeFixUseExistingStaticFields()
        {
            var before = @"
namespace N
{
    using Microsoft.CodeAnalysis.Diagnostics;
    using Microsoft.CodeAnalysis.CodeFixes;
    using Gu.Roslyn.Asserts;
    using NUnit.Framework;

    public static class C
    {
        private static readonly DiagnosticAnalyzer Analyzer = new PlaceholderAnalyzer();
        private static readonly CodeFixProvider Fix = new PlaceholderFix();

        [Test]
        public static void M()
        {
            var before = ""class C { }"";
            var after = ""class C { }"";
            ↓RoslynAssert.CodeFix<PlaceholderAnalyzer, PlaceholderFix>(ExpectedDiagnostic.Create(""123""), before, after);
        }
    }
}";

            var after = @"
namespace N
{
    using Microsoft.CodeAnalysis.Diagnostics;
    using Microsoft.CodeAnalysis.CodeFixes;
    using Gu.Roslyn.Asserts;
    using NUnit.Framework;

    public static class C
    {
        private static readonly DiagnosticAnalyzer Analyzer = new PlaceholderAnalyzer();
        private static readonly CodeFixProvider Fix = new PlaceholderFix();

        [Test]
        public static void M()
        {
            var before = ""class C { }"";
            var after = ""class C { }"";
            RoslynAssert.CodeFix(Analyzer, Fix, ExpectedDiagnostic.Create(""123""), before, after);
        }
    }
}";
            RoslynAssert.CodeFix(Fix, ExpectedDiagnostic, new[] { Code.PlaceholderFix, Code.PlaceholderAnalyzer, before }, after, fixTitle: "Use 'Analyzer' and 'Fix.");
        }

        [Test]
        public static void CodeFixUseExistingStaticFieldsInOtherPart()
        {
            var part1 = @"
namespace N
{
    using Microsoft.CodeAnalysis.Diagnostics;
    using Microsoft.CodeAnalysis.CodeFixes;

    public static partial class C
    {
        private static readonly DiagnosticAnalyzer Analyzer = new PlaceholderAnalyzer();
        private static readonly CodeFixProvider Fix = new PlaceholderFix();
    }
}";
            var before = @"
namespace N
{
    using Gu.Roslyn.Asserts;
    using NUnit.Framework;

    public static partial class C
    {
        [Test]
        public static void M()
        {
            var before = ""class C { }"";
            var after = ""class C { }"";
            ↓RoslynAssert.CodeFix<PlaceholderAnalyzer, PlaceholderFix>(ExpectedDiagnostic.Create(""123""), before, after);
        }
    }
}";

            var after = @"
namespace N
{
    using Gu.Roslyn.Asserts;
    using NUnit.Framework;

    public static partial class C
    {
        [Test]
        public static void M()
        {
            var before = ""class C { }"";
            var after = ""class C { }"";
            RoslynAssert.CodeFix(Analyzer, Fix, ExpectedDiagnostic.Create(""123""), before, after);
        }
    }
}";
            RoslynAssert.CodeFix(Fix, ExpectedDiagnostic, new[] { Code.PlaceholderFix, Code.PlaceholderAnalyzer, part1, before }, after, fixTitle: "Use 'Analyzer' and 'Fix.");
        }

        [Test]
        public static void CodeFixUseExistingStaticFieldsInContainingTypeInOtherPart()
        {
            var part1 = @"
namespace N
{
    using Microsoft.CodeAnalysis.Diagnostics;
    using Microsoft.CodeAnalysis.CodeFixes;

    public static partial class C
    {
        private static readonly DiagnosticAnalyzer Analyzer = new PlaceholderAnalyzer();
        private static readonly CodeFixProvider Fix = new PlaceholderFix();
    }
}";
            var before = @"
namespace N
{
    using Gu.Roslyn.Asserts;
    using NUnit.Framework;

    public static partial class C
    {
        public static class C1
        {
            [Test]
            public static void M()
            {
                var before = ""class C { }"";
                var after = ""class C { }"";
                ↓RoslynAssert.CodeFix<PlaceholderAnalyzer, PlaceholderFix>(ExpectedDiagnostic.Create(""123""), before, after);
            }
        }
    }
}";

            var after = @"
namespace N
{
    using Gu.Roslyn.Asserts;
    using NUnit.Framework;

    public static partial class C
    {
        public static class C1
        {
            [Test]
            public static void M()
            {
                var before = ""class C { }"";
                var after = ""class C { }"";
                RoslynAssert.CodeFix(Analyzer, Fix, ExpectedDiagnostic.Create(""123""), before, after);
            }
        }
    }
}";
            RoslynAssert.CodeFix(Fix, ExpectedDiagnostic, new[] { Code.PlaceholderFix, Code.PlaceholderAnalyzer, part1, before }, after, fixTitle: "Use 'Analyzer' and 'Fix.");
        }

        [Test]
        public static void CodeFixCreateAndUseFields()
        {
            var before = @"
namespace N
{
    using Gu.Roslyn.Asserts;
    using NUnit.Framework;

    public static class C
    {
        [Test]
        public static void M()
        {
            var before = ""class C { }"";
            var after = ""class C { }"";
            ↓RoslynAssert.CodeFix<PlaceholderAnalyzer, PlaceholderFix>(ExpectedDiagnostic.Create(""123""), before, after);
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
        private static readonly PlaceholderFix Fix = new PlaceholderFix();

        [Test]
        public static void M()
        {
            var before = ""class C { }"";
            var after = ""class C { }"";
            RoslynAssert.CodeFix(Analyzer, Fix, ExpectedDiagnostic.Create(""123""), before, after);
        }
    }
}";
            RoslynAssert.CodeFix(Fix, ExpectedDiagnostic, new[] { Code.PlaceholderFix, Code.PlaceholderAnalyzer, before }, after, fixTitle: "Create and use fields 'Analyzer' and 'Fix'.");
        }

        [Test]
        public static void CodeFixCreateAndUseFieldsFixAll()
        {
            var before = @"
namespace N
{
    using Gu.Roslyn.Asserts;
    using NUnit.Framework;

    public static class C
    {
        [Test]
        public static void M1()
        {
            var before = ""class C { }"";
            var after = ""class C { }"";
            ↓RoslynAssert.CodeFix<PlaceholderAnalyzer, PlaceholderFix>(ExpectedDiagnostic.Create(""123""), before, after);
        }

        [Test]
        public static void M2()
        {
            var before = ""class C { }"";
            var after = ""class C { }"";
            ↓RoslynAssert.CodeFix<PlaceholderAnalyzer, PlaceholderFix>(ExpectedDiagnostic.Create(""123""), before, after);
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
        private static readonly PlaceholderFix Fix = new PlaceholderFix();

        [Test]
        public static void M1()
        {
            var before = ""class C { }"";
            var after = ""class C { }"";
            RoslynAssert.CodeFix(Analyzer, Fix, ExpectedDiagnostic.Create(""123""), before, after);
        }

        [Test]
        public static void M2()
        {
            var before = ""class C { }"";
            var after = ""class C { }"";
            RoslynAssert.CodeFix(Analyzer, Fix, ExpectedDiagnostic.Create(""123""), before, after);
        }
    }
}";
            RoslynAssert.FixAll(Fix, ExpectedDiagnostic, new[] { Code.PlaceholderFix, Code.PlaceholderAnalyzer, before }, after);
        }
    }
}
