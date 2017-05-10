﻿// ReSharper disable RedundantNameQualifier
namespace Gu.Roslyn.Asserts.Tests
{
    using System;
    using NUnit.Framework;

    [TestFixture]
    public partial class AnalyzerAssertNoDiagnosticsTests
    {
        public class Success
        {
            [Test]
            public void SingleClassNoErrorGeneric()
            {
                var code = @"
namespace RoslynSandbox
{
    class Foo
    {
    }
}";
                AnalyzerAssert.NoDiagnostics<NoErrorAnalyzer>(code);
            }

            [TestCase(typeof(NoErrorAnalyzer))]
            public void SingleClassNoErrorType(Type type)
            {
                var code = @"
namespace RoslynSandbox
{
    class Foo
    {
    }
}";
                AnalyzerAssert.NoDiagnostics(type, code);
            }

            [Test]
            public void SingleClassNoErrorPassingAnalyzer()
            {
                var code = @"
namespace RoslynSandbox
{
    class Foo
    {
    }
}";
                AnalyzerAssert.NoDiagnostics(new NoErrorAnalyzer(), code);
            }

            [Test]
            public void TwoClassesNoError()
            {
                var code1 = @"
namespace RoslynSandbox
{
    class Code1
    {
    }
}";
                var code2 = @"
namespace RoslynSandbox
{
    class Code2
    {
    }
}";
                AnalyzerAssert.NoDiagnostics<NoErrorAnalyzer>(code1, code2);
            }
        }
    }
}