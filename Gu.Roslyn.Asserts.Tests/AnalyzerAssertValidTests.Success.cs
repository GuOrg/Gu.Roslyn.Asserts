﻿// ReSharper disable RedundantNameQualifier
// ReSharper disable AssignNullToNotNullAttribute
namespace Gu.Roslyn.Asserts.Tests
{
    using System;
    using System.IO;
    using System.Reflection;
    using NUnit.Framework;

    [TestFixture]
    public partial class AnalyzerAssertValidTests
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
                AnalyzerAssert.Valid<NoErrorAnalyzer>(code);
            }

            [Test]
            public void ProjectFileNoErrorGeneric()
            {
                var dllFile = new Uri(Assembly.GetExecutingAssembly().CodeBase, UriKind.Absolute).LocalPath;
                Assert.AreEqual(true, CodeFactory.TryFindFileInParentDirectory(new DirectoryInfo(Path.GetDirectoryName(dllFile)), Path.GetFileNameWithoutExtension(dllFile) + ".csproj", out FileInfo projectFile));
                AnalyzerAssert.Valid<NoErrorAnalyzer>(projectFile);
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
                AnalyzerAssert.Valid(type, code);
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
                AnalyzerAssert.Valid(new NoErrorAnalyzer(), code);
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
                AnalyzerAssert.Valid<NoErrorAnalyzer>(code1, code2);
            }
        }
    }
}