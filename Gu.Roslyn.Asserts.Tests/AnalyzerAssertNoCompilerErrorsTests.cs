// ReSharper disable RedundantNameQualifier
namespace Gu.Roslyn.Asserts.Tests
{
    using Microsoft.CodeAnalysis;
    using NUnit.Framework;

    public class AnalyzerAssertNoCompilerErrorsTests
    {
        [TearDown]
        public void TearDown()
        {
            AnalyzerAssert.MetadataReferences.Clear();
        }

        [Test]
        public void WhenCodeNoCompilerErrors()
        {
            var code = @"
namespace RoslynSandbox
{
    class Foo
    {
    }
}";
            AnalyzerAssert.MetadataReferences.Add(MetadataReference.CreateFromFile(typeof(object).Assembly.Location));
            AnalyzerAssert.NoCompilerErrors(code);
        }

        [Test]
        public void WhenCodeNoCompilerErrorsCollectionInitializer()
        {
            var code = @"
namespace RoslynSandbox
{
    using System.Collections.Generic;

    class Foo
    {
        public Foo()
        {
            var ints = new List<int> { 1, 2, 3 };
        }
    }
}";
            AnalyzerAssert.MetadataReferences.Add(MetadataReference.CreateFromFile(typeof(object).Assembly.Location));
            AnalyzerAssert.NoCompilerErrors(code);
        }

        [Test]
        public void WhenCodeHasCompilerErrors()
        {
            var code = @"
namespace RoslynSandbox
{
    class Foo
    {
        public event EventHandler SomeEvent;
    }
}";
            var exception = Assert.Throws<NUnit.Framework.AssertionException>(() => AnalyzerAssert.NoCompilerErrors(code));
            var expected = "Found errors.\r\n" +
                           "CS0518 Predefined type 'System.Object' is not defined or imported\r\n" +
                           "  at line 3 and character 10 in file Foo.cs | class ↓Foo\r\n" +
                           "CS0518 Predefined type 'System.Object' is not defined or imported\r\n" +
                           "  at line 5 and character 21 in file Foo.cs | public event ↓EventHandler SomeEvent;\r\n" +
                           "CS0246 The type or namespace name 'EventHandler' could not be found (are you missing a using directive or an assembly reference?)\r\n" +
                           "  at line 5 and character 21 in file Foo.cs | public event ↓EventHandler SomeEvent;\r\n" +
                           "CS0518 Predefined type 'System.Void' is not defined or imported\r\n" +
                           "  at line 5 and character 34 in file Foo.cs | public event EventHandler ↓SomeEvent;\r\n" +
                           "CS0518 Predefined type 'System.Void' is not defined or imported\r\n" +
                           "  at line 5 and character 34 in file Foo.cs | public event EventHandler ↓SomeEvent;\r\n" +
                           "CS1729 'object' does not contain a constructor that takes 0 arguments\r\n" +
                           "  at line 3 and character 10 in file Foo.cs | class ↓Foo\r\n";
            Assert.AreEqual(expected, exception.Message);
        }
    }
}