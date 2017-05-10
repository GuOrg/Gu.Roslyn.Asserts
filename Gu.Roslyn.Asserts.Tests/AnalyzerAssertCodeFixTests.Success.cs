// ReSharper disable RedundantNameQualifier
namespace Gu.Roslyn.Asserts.Tests
{
    using System.Collections.Immutable;
    using Gu.Roslyn.Asserts.Tests.CodeFixes;
    using Microsoft.CodeAnalysis;
    using NUnit.Framework;

    [TestFixture]
    public partial class AnalyzerAssertCodeFixTests
    {
        public class Success
        {
            [Test]
            public void SingleClassOneErrorCorrectFix()
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
                AnalyzerAssert.CodeFix<FieldNameMustNotBeginWithUnderscore, DontUseUnderscoreCodeFixProvider>(code, fixedCode);
            }

            [Test]
            public void SingleClassCodeFixOnlyCorrectFix()
            {
                AnalyzerAssert.MetadataReference.Add(MetadataReference.CreateFromFile(typeof(object).Assembly.Location).WithAliases(ImmutableArray.Create("global", "corlib")));
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
                AnalyzerAssert.CodeFix<RemoveUnusedFixProvider>("CS0067", code, fixedCode);
            }

            [Test]
            public void TwoClassesCodeFixOnlyCorrectFix()
            {
                AnalyzerAssert.MetadataReference.Add(MetadataReference.CreateFromFile(typeof(object).Assembly.Location).WithAliases(ImmutableArray.Create("global", "corlib")));
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
    public class Foo2
    {
    }
}";

                var fixedCode = @"
namespace RoslynSandbox
{
    using System;

    public class Foo1
    {
    }
}";
                AnalyzerAssert.CodeFix<RemoveUnusedFixProvider>("CS0067", new[] { code1, code2 }, new[] { fixedCode, code2 });
                AnalyzerAssert.CodeFix<RemoveUnusedFixProvider>("CS0067", new[] { code2, code1 }, new[] { code2, fixedCode });
            }

            [Test]
            public void TwoClassesOneErrorCorrectFix()
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
                AnalyzerAssert.CodeFix<FieldNameMustNotBeginWithUnderscore, DontUseUnderscoreCodeFixProvider>(new[] { barCode, code }, new[] { barCode, fixedCode });
            }
        }
    }
}