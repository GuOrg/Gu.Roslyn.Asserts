namespace Gu.Roslyn.Asserts.Analyzers.Tests.GURA01NameShouldMatchParameter
{
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.Diagnostics;
    using NUnit.Framework;

    public static class Valid
    {
        private static readonly DiagnosticAnalyzer Analyzer = new InvocationAnalyzer();
        private static readonly DiagnosticDescriptor Descriptor = Descriptors.GURA01NameShouldMatchParameter;

        [Test]
        public static void ValidAnalyzerAndCode()
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
            var code = ""class ↓C { }"";
            RoslynAssert.Valid(Analyzer, code);
        }
    }
}";
            RoslynAssert.Valid(Analyzer, Descriptor, Code.PlaceholderAnalyzer, code);
        }

        [Test]
        public static void ValidAnalyzerAndCodeAndMetadataReferences()
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
            var code = ""class ↓C { }"";
            var metadataReferences = new[] { Gu.Roslyn.Asserts.MetadataReferences.CreateFromAssembly(typeof(object).Assembly) };
            RoslynAssert.Valid(Analyzer, code, metadataReferences: metadataReferences);
        }
    }
}";
            RoslynAssert.Valid(Analyzer, Descriptor, Code.PlaceholderAnalyzer, code);
        }

        [Test]
        public static void WhenTypeOfAnalyzer()
        {
            var code = @"
namespace N
{
    using Gu.Roslyn.Asserts;
    using NUnit.Framework;

    public static class C
    {
        [Test]
        public static void M()
        {
            var code = ""class ↓C { }"";
            RoslynAssert.Valid(typeof(PlaceholderAnalyzer), code);
        }
    }
}";
            RoslynAssert.Valid(Analyzer, Descriptor, Code.PlaceholderAnalyzer, code);
        }

        [Test]
        public static void WhenAnalyzerGetType()
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
            var code = ""class ↓C { }"";
            RoslynAssert.Valid(Analyzer.GetType(), code);
        }
    }
}";
            RoslynAssert.Valid(Analyzer, Descriptor, Code.PlaceholderAnalyzer, code);
        }

        [TestCase("Analyzer")]
        [TestCase("PlaceholderAnalyzer")]
        public static void WhenFieldAnalyzer(string name)
        {
            var code = @"
namespace N
{
    using Gu.Roslyn.Asserts;
    using NUnit.Framework;

    public static class C
    {
        private static readonly PlaceholderAnalyzer Name = new PlaceholderAnalyzer();

        [Test]
        public static void M()
        {
            var code = ""class C { }"";
            RoslynAssert.Valid(Name, code);
        }
    }
}".AssertReplace("Name", name);
            RoslynAssert.Valid(Analyzer, Descriptor, Code.PlaceholderAnalyzer, code);
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
            var code = ""class C1 { }"";
            RoslynAssert.Valid(Analyzer, code);
        }
    }
}";

            RoslynAssert.Valid(Analyzer, Descriptor, Code.PlaceholderAnalyzer, code);
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

            RoslynAssert.Valid(Analyzer, Descriptor, Code.PlaceholderAnalyzer, code);
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

            RoslynAssert.Valid(Analyzer, Descriptor, Code.PlaceholderAnalyzer, code);
        }

        [Test]
        public static void InlineStringEmptyParams()
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

            RoslynAssert.Valid(Analyzer, Descriptor, Code.PlaceholderAnalyzer, code);
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
            var code = string.Empty;
            RoslynAssert.Valid(Analyzer, code);
        }
    }
}".AssertReplace("string.Empty", expression);

            RoslynAssert.Valid(Analyzer, Descriptor, Code.PlaceholderAnalyzer, code);
        }

        [Test]
        public static void CodeFixOneParamWithPosition()
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
            var code = ""↓class C { }"";
            RoslynAssert.Diagnostics(Analyzer, code);
        }
    }
}";
            RoslynAssert.Valid(Analyzer, Descriptor, Code.PlaceholderAnalyzer, code);
        }

        [Test]
        public static void CodeFixTwoParamsWithOnePosition()
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
            var code = ""↓class C2 { }"";
            RoslynAssert.Diagnostics(Analyzer, c1, code);
        }
    }
}";
            RoslynAssert.Valid(Analyzer, Descriptor, Code.PlaceholderAnalyzer, code);
        }

        [Test]
        public static void CodeFixTwoParamsWithOnePositionConst()
        {
            var code = @"
namespace N
{
    using Gu.Roslyn.Asserts;
    using NUnit.Framework;

    public static class C
    {
        private static readonly PlaceholderAnalyzer Analyzer = new PlaceholderAnalyzer();
        const string C1 = ""class C1 { }"";

        [Test]
        public static void M()
        {
            var code = ""↓class C2 { }"";
            RoslynAssert.Diagnostics(Analyzer, C1, code);
        }
    }
}";
            RoslynAssert.Valid(Analyzer, Descriptor, Code.PlaceholderAnalyzer, code);
        }

        [Test]
        public static void CodeFixTwoParamsWithOnePositionInstanceField()
        {
            var code = @"
namespace N
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
            RoslynAssert.Valid(Analyzer, Descriptor, Code.PlaceholderAnalyzer, code);
        }

        [Test]
        public static void CodeFixArrayWithOnePosition()
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
            var code = ""↓class C2 { }"";
            RoslynAssert.Diagnostics(Analyzer, new [] { c1, code });
        }
    }
}";
            RoslynAssert.Valid(Analyzer, Descriptor, Code.PlaceholderAnalyzer, code);
        }

        [Test]
        public static void CodeFixOneBefore()
        {
            var code = @"
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
            var before = ""↓class C { }"";
            RoslynAssert.CodeFix(Analyzer, Fix, before, string.Empty);
        }
    }
}";
            RoslynAssert.Valid(Analyzer, Descriptor, Code.PlaceholderAnalyzer, Code.PlaceholderFix, code);
        }

        [Test]
        public static void CodeFixAllowCompilationErrors()
        {
            var code = @"
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
            var before = ""↓class C { }"";
            var after = ""class C { }"";
            RoslynAssert.CodeFix(Analyzer, Fix, before, after, allowCompilationErrors: AllowCompilationErrors.Yes);
        }
    }
}";
            RoslynAssert.Valid(Analyzer, Descriptor, Code.PlaceholderAnalyzer, Code.PlaceholderFix, code);
        }

        [Test]
        public static void Refactoring()
        {
            var code = @"
namespace N
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
namespace N
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
namespace N
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
            RoslynAssert.Valid(Analyzer, Descriptor, Code.PlaceholderRefactoring, code);
        }

        [Test]
        public static void FixAllMany()
        {
            var code = @"
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
            var before1 = ""↓class C1 { }"";
            var before2 = ""↓class C2 { }"";
            var after1 = ""class C1 { }"";
            var after2 = ""class C2 { }"";
            RoslynAssert.FixAll(Analyzer, Fix, new [] { before1, before2 }, new [] { after1, after2 });
        }
    }
}";
            RoslynAssert.Valid(Analyzer, Descriptor, Code.PlaceholderAnalyzer, Code.PlaceholderFix, code);
        }

        [Test]
        public static void WhenTwoWithPosition()
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
            var c1 = ""↓class C1 { }"";
            var c2 = ""↓class C2 { }"";
            RoslynAssert.Diagnostics(Analyzer, c1, c2);
        }
    }
}";

            RoslynAssert.Valid(Analyzer, Code.PlaceholderAnalyzer, code);
        }

        [Test]
        public static void WhenMetadataReferencesInOtherClass()
        {
            var code = @"
namespace N
{
    using Gu.Roslyn.Asserts;
    using Microsoft.CodeAnalysis;
    using NUnit.Framework;

    public static class SpecialMetadataReferences
    {
        public static readonly MetadataReference[] Corlib = new[] { Gu.Roslyn.Asserts.MetadataReferences.CreateFromAssembly(typeof(object).Assembly) };
    }

    public static class C
    {
        private static readonly PlaceholderAnalyzer Analyzer = new PlaceholderAnalyzer();

        [Test]
        public static void M()
        {
            var code = ""class ↓C { }"";
            RoslynAssert.Valid(Analyzer, code, metadataReferences: SpecialMetadataReferences.Corlib);
        }
    }
}";
            RoslynAssert.Valid(Analyzer, Descriptor, Code.PlaceholderAnalyzer, code);
        }
    }
}
