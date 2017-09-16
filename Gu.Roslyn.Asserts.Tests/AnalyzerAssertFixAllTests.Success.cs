namespace Gu.Roslyn.Asserts.Tests
{
    using System;
    using Gu.Roslyn.Asserts.Tests.CodeFixes;
    using Microsoft.CodeAnalysis;
    using NUnit.Framework;

    [TestFixture]
    public partial class AnalyzerAssertFixAllTests
    {
        public class Success
        {
            [TearDown]
            public void TearDown()
            {
                AnalyzerAssert.MetadataReferences.Clear();
            }

            [Test]
            public void OneErrorCorrectFix()
            {
                var code = @"
namespace RoslynSandbox
{
    class Foo
    {
        private readonly int ↓_value;
    }
}";

                var fixedCode = @"
namespace RoslynSandbox
{
    class Foo
    {
        private readonly int value;
    }
}";
                AnalyzerAssert.MetadataReferences.Add(MetadataReference.CreateFromFile(typeof(int).Assembly.Location));
                AnalyzerAssert.FixAll<FieldNameMustNotBeginWithUnderscore, DontUseUnderscoreCodeFixProvider>(code, fixedCode);
            }

            [TestCase("Rename to: value1", "value1")]
            [TestCase("Rename to: value2", "value2")]
            public void SingleClassOneErrorTwoFixesCorrectFix(string title, string expected)
            {
                var code = @"
namespace RoslynSandbox
{
    class Foo
    {
        private readonly int ↓_value;
    }
}";

                var fixedCode = @"
namespace RoslynSandbox
{
    class Foo
    {
        private readonly int value;
    }
}";
                fixedCode = fixedCode.AssertReplace("value", expected);
                AnalyzerAssert.MetadataReferences.Add(MetadataReference.CreateFromFile(typeof(int).Assembly.Location));
                AnalyzerAssert.FixAll<FieldNameMustNotBeginWithUnderscore, DontUseUnderscoreManyCodeFixProvider>(code, fixedCode, title);
            }

            [Test]
            public void TwoErrorsCorrectFix()
            {
                var code = @"
namespace RoslynSandbox
{
    class Foo
    {
        private readonly int ↓_value1;
        private readonly int ↓_value2;
    }
}";

                var fixedCode = @"
namespace RoslynSandbox
{
    class Foo
    {
        private readonly int value1;
        private readonly int value2;
    }
}";
                AnalyzerAssert.MetadataReferences.Add(MetadataReference.CreateFromFile(typeof(int).Assembly.Location));
                AnalyzerAssert.FixAll<FieldNameMustNotBeginWithUnderscore, DontUseUnderscoreCodeFixProvider>(code, fixedCode);
            }

            [Test]
            public void TwoClassesCodeFixOnlyCorrectFix()
            {
                var code1 = @"
namespace RoslynSandbox
{
    using System;

    public class Foo1
    {
        public event EventHandler ↓Bar;
    }
}";

                var code2 = @"
namespace RoslynSandbox
{
    using System;

    public class Foo2
    {
        public event EventHandler ↓Bar;
    }
}";

                var fixed1 = @"
namespace RoslynSandbox
{
    using System;

    public class Foo1
    {
    }
}";

                var fixed2 = @"
namespace RoslynSandbox
{
    using System;

    public class Foo2
    {
    }
}";
                AnalyzerAssert.MetadataReferences.Add(MetadataReference.CreateFromFile(typeof(EventHandler).Assembly.Location));
                AnalyzerAssert.FixAll<RemoveUnusedFixProvider>("CS0067", new[] { code1, code2 }, new[] { fixed1, fixed2 });
                AnalyzerAssert.FixAll<RemoveUnusedFixProvider>("CS0067", new[] { code2, code1 }, new[] { fixed2, fixed1 });
            }

            [TestCase("Rename to: value1", "value1")]
            [TestCase("Rename to: value2", "value2")]
            public void TwoClassesOneFixCorrectFix(string title, string expected)
            {
                var code1 = @"
namespace RoslynSandbox
{
    class Foo1
    {
        private readonly int ↓_value;
    }
}";
                var code2 = @"
namespace RoslynSandbox
{
    class Foo2
    {
        private readonly int value;
    }
}";

                var fixedCode = @"
namespace RoslynSandbox
{
    class Foo1
    {
        private readonly int value;
    }
}";

                fixedCode = fixedCode.AssertReplace("value", expected);
                AnalyzerAssert.MetadataReferences.Add(MetadataReference.CreateFromFile(typeof(int).Assembly.Location));
                AnalyzerAssert.FixAll<FieldNameMustNotBeginWithUnderscore, DontUseUnderscoreManyCodeFixProvider>(new[] { code1, code2 }, new[] { fixedCode, code2 }, title);
            }

            [TestCase("Rename to: value1", "value1")]
            [TestCase("Rename to: value2", "value2")]
            public void TwoClassesOneFixCorrectFixPassOnlyFixedCode(string title, string expected)
            {
                var code1 = @"
namespace RoslynSandbox
{
    class Foo1
    {
        private readonly int ↓_value;
    }
}";
                var code2 = @"
namespace RoslynSandbox
{
    class Foo2
    {
        private readonly int value;
    }
}";

                var fixedCode = @"
namespace RoslynSandbox
{
    class Foo1
    {
        private readonly int value;
    }
}";

                fixedCode = fixedCode.AssertReplace("value", expected);
                AnalyzerAssert.MetadataReferences.Add(MetadataReference.CreateFromFile(typeof(int).Assembly.Location));
                AnalyzerAssert.FixAll<FieldNameMustNotBeginWithUnderscore, DontUseUnderscoreManyCodeFixProvider>(new[] { code1, code2 }, fixedCode, title);
            }

            [TestCase("Rename to: value1", "value1")]
            [TestCase("Rename to: value2", "value2")]
            public void TwoClassesTwoFixesCorrectFix(string title, string expected)
            {
                var code1 = @"
namespace RoslynSandbox
{
    class Foo1
    {
        private readonly int ↓_value;
    }
}";
                var code2 = @"
namespace RoslynSandbox
{
    class Foo2
    {
        private readonly int ↓_value;
    }
}";

                var fixedCode1 = @"
namespace RoslynSandbox
{
    class Foo1
    {
        private readonly int value;
    }
}";

                var fixedCode2 = @"
namespace RoslynSandbox
{
    class Foo2
    {
        private readonly int value;
    }
}";
                fixedCode1 = fixedCode1.AssertReplace("value", expected);
                fixedCode2 = fixedCode2.AssertReplace("value", expected);
                AnalyzerAssert.MetadataReferences.Add(MetadataReference.CreateFromFile(typeof(int).Assembly.Location));
                AnalyzerAssert.FixAll<FieldNameMustNotBeginWithUnderscore, DontUseUnderscoreManyCodeFixProvider>(new[] { code1, code2 }, new[] { fixedCode1, fixedCode2 }, title);
            }

            [Test]
            public void SingleClassCodeFixOnlyCorrectFix()
            {
                var code = @"
namespace RoslynSandbox
{
    using System;

    public class Foo
    {
        public event EventHandler ↓Bar;
    }
}";

                var fixedCode = @"
namespace RoslynSandbox
{
    using System;

    public class Foo
    {
    }
}";
                AnalyzerAssert.MetadataReferences.Add(MetadataReference.CreateFromFile(typeof(EventHandler).Assembly.Location));
                AnalyzerAssert.FixAll<RemoveUnusedFixProvider>("CS0067", code, fixedCode);
            }

            [Test]
            public void TwoClassOneErrorCorrectFix()
            {
                var barCode = @"
namespace RoslynSandbox
{
    class Bar
    {
        private readonly int value;
    }
}";

                var code = @"
namespace RoslynSandbox
{
    class Foo
    {
        private readonly int ↓_value;
    }
}";

                var fixedCode = @"
namespace RoslynSandbox
{
    class Foo
    {
        private readonly int value;
    }
}";
                AnalyzerAssert.MetadataReferences.Add(MetadataReference.CreateFromFile(typeof(int).Assembly.Location));
                AnalyzerAssert.FixAll<FieldNameMustNotBeginWithUnderscore, DontUseUnderscoreCodeFixProvider>(new[] { barCode, code }, new[] { barCode, fixedCode });
            }

            [Test]
            public void WhenFixIntroducesCompilerErrorsThatAreAccepted()
            {
                var code = @"
namespace RoslynSandbox
{
    ↓class Foo
    {
    }
}";

                var fixedCode = @"
namespace RoslynSandbox
{
    class Foo
    {
        public event EventHandler SomeEvent;
    }
}";
                AnalyzerAssert.FixAll<ClassMustHaveEventAnalyzer, InsertEventFixProvider>(code, fixedCode, allowCompilationErrors: AllowCompilationErrors.Yes);
            }
        }
    }
}