// ReSharper disable RedundantNameQualifier
namespace Gu.Roslyn.Asserts.Tests
{
    using Microsoft.CodeAnalysis;
    using NUnit.Framework;

    public partial class RoslynAssertTests
    {
        public static class NoCompilerErrors
        {
            [SetUp]
            public static void SetUp()
            {
                RoslynAssert.MetadataReferences.Add(MetadataReference.CreateFromFile(typeof(int).Assembly.Location));
            }

            [TearDown]
            public static void TearDown()
            {
                RoslynAssert.ResetAll();
            }

            [Test]
            public static void WhenCodeNoCompilerErrors()
            {
                var code = @"
namespace RoslynSandbox
{
    class C
    {
    }
}";
                RoslynAssert.NoCompilerErrors(code);
            }

            [Test]
            public static void WhenCodeNoCompilerErrorsCollectionInitializer()
            {
                var code = @"
namespace RoslynSandbox
{
    using System.Collections.Generic;

    class C
    {
        public C()
        {
            var ints = new List<int> { 1, 2, 3 };
        }
    }
}";
                RoslynAssert.NoCompilerErrors(code);
            }

            [Test]
            public static void WhenCodeHasCompilerErrors()
            {
                var code = @"
namespace RoslynSandbox
{
    class C
    {
        public event EventHandler SomeEvent;
    }
}";
                RoslynAssert.ResetAll();
                var exception = Assert.Throws<AssertException>(() => RoslynAssert.NoCompilerErrors(code));
                var expected = "Found errors.\r\n" +
                               "CS0518 Predefined type 'System.Object' is not defined or imported\r\n" +
                               "  at line 3 and character 10 in file C.cs | class ↓C\r\n" +
                               "CS0518 Predefined type 'System.Object' is not defined or imported\r\n" +
                               "  at line 5 and character 21 in file C.cs | public event ↓EventHandler SomeEvent;\r\n" +
                               "CS0246 The type or namespace name 'EventHandler' could not be found (are you missing a using directive or an assembly reference?)\r\n" +
                               "  at line 5 and character 21 in file C.cs | public event ↓EventHandler SomeEvent;\r\n" +
                               "CS0518 Predefined type 'System.Void' is not defined or imported\r\n" +
                               "  at line 5 and character 34 in file C.cs | public event EventHandler ↓SomeEvent;\r\n" +
                               "CS0518 Predefined type 'System.Void' is not defined or imported\r\n" +
                               "  at line 5 and character 34 in file C.cs | public event EventHandler ↓SomeEvent;\r\n" +
                               "CS1729 'object' does not contain a constructor that takes 0 arguments\r\n" +
                               "  at line 3 and character 10 in file C.cs | class ↓C\r\n";
                Assert.AreEqual(expected, exception.Message);
            }
        }
    }
}
