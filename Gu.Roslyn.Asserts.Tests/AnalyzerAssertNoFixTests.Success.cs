// ReSharper disable RedundantNameQualifier
namespace Gu.Roslyn.Asserts.Tests
{
    using System.Collections.Generic;
    using Gu.Roslyn.Asserts.Tests.CodeFixes;
    using NUnit.Framework;

    [TestFixture]
    public partial class AnalyzerAssertNoFixTests
    {
        public class Success
        {
            [Test]
            public void SingleClassOneErrorNoFix()
            {
                var code = @"
namespace RoslynSandbox
{
    class Foo
    {
        private readonly int ↓_value;
    }
}";

                AnalyzerAssert.NoFix<FieldNameMustNotBeginWithUnderscore, NoCodeFixProvider>(code);
            }

            [Test]
            public void SingleClassOneErrorEmptyFix()
            {
                var code = @"
namespace RoslynSandbox
{
    class Foo
    {
        private readonly int ↓_value;
    }
}";

                AnalyzerAssert.NoFix<FieldNameMustNotBeginWithUnderscore, EmptyCodeFixProvider>(code);
            }

            [Test]
            public void TwoClassOneErrorNoFix()
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

                AnalyzerAssert.NoFix<FieldNameMustNotBeginWithUnderscore, NoCodeFixProvider>(barCode, code);
                AnalyzerAssert.NoFix<FieldNameMustNotBeginWithUnderscore, NoCodeFixProvider>((IReadOnlyList<string>)new[] { barCode, code });
            }

            [Test]
            public void TwoClassOneErrorEmptyFix()
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

                AnalyzerAssert.NoFix<FieldNameMustNotBeginWithUnderscore, EmptyCodeFixProvider>(barCode, code);
                AnalyzerAssert.NoFix<FieldNameMustNotBeginWithUnderscore, EmptyCodeFixProvider>((IReadOnlyList<string>)new[] { barCode, code });
            }
        }
    }
}