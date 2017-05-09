using System.Collections.Immutable;
using Microsoft.CodeAnalysis;

namespace Gu.Roslyn.Asserts.Tests
{
    using Gu.Roslyn.Asserts.Tests.CodeFixes;
    using NUnit.Framework;

    public partial class AnalyzerAssertTests
    {
        public class CodeFix
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
    AnalyzerAssert.References.Add(MetadataReference.CreateFromFile(typeof(object).Assembly.Location).WithAliases(ImmutableArray.Create("global", "corlib")));
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
                AnalyzerAssert.CodeFix<FieldNameMustNotBeginWithUnderscore, DontUseUnderscoreCodeFixProvider>(new[] { barCode, code }, new[] { barCode, fixedCode });
            }

            [Test]
            public void SingleClassOneErrorErrorInFix()
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
        private readonly int bar;
    }
}";
                var exception = Assert.Throws<NUnit.Framework.AssertionException>(() => AnalyzerAssert.CodeFix<FieldNameMustNotBeginWithUnderscore, DontUseUnderscoreCodeFixProvider>(code, fixedCode));
                var expected = "Mismatch on line 6 of file Foo.cs\r\n" +
                               "Expected:         private readonly int bar;\r\n" +
                               "Actual:           private readonly int value;\r\n" +
                               "                                       ^\r\n";
                Assert.AreEqual(expected, exception.Message);
            }

            [Test]
            public void TwoClassesOneErrorErrorInFix()
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
        private readonly int bar;
    }
}";
                var exception = Assert.Throws<NUnit.Framework.AssertionException>(() => AnalyzerAssert.CodeFix<FieldNameMustNotBeginWithUnderscore, DontUseUnderscoreCodeFixProvider>(new[] { barCode, code }, new[] { barCode, fixedCode }));
                var expected = "Mismatch on line 6 of file Foo.cs\r\n" +
                               "Expected:         private readonly int bar;\r\n" +
                               "Actual:           private readonly int value;\r\n" +
                               "                                       ^\r\n";
                Assert.AreEqual(expected, exception.Message);
            }
        }
    }
}