// ReSharper disable RedundantNameQualifier
namespace Gu.Roslyn.Asserts.Tests.RoslynAssertTests
{
    using System.Linq;
    using Microsoft.CodeAnalysis;
    using NUnit.Framework;

    public static class NoCompilerErrors
    {
        public static class Success
        {
            [Test]
            public static void WhenCodeNoCompilerErrors()
            {
                var code = @"
namespace N
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
namespace N
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
namespace N
{
    class C
    {
        public event EventHandler E;
    }
}";
                var exception = Assert.Throws<AssertException>(() => RoslynAssert.NoCompilerErrors(Enumerable.Empty<MetadataReference>(), code));
                var expected = "Found errors.\r\n" +
                               "CS0518 Predefined type 'System.Object' is not defined or imported\r\n" +
                               "  at line 3 and character 10 in file C.cs | class ↓C\r\n" +
                               "CS0518 Predefined type 'System.Object' is not defined or imported\r\n" +
                               "  at line 5 and character 21 in file C.cs | public event ↓EventHandler E;\r\n" +
                               "CS0246 The type or namespace name 'EventHandler' could not be found (are you missing a using directive or an assembly reference?)\r\n" +
                               "  at line 5 and character 21 in file C.cs | public event ↓EventHandler E;\r\n" +
                               "CS0518 Predefined type 'System.Void' is not defined or imported\r\n" +
                               "  at line 5 and character 34 in file C.cs | public event EventHandler ↓E;\r\n" +
                               "CS0518 Predefined type 'System.Void' is not defined or imported\r\n" +
                               "  at line 5 and character 34 in file C.cs | public event EventHandler ↓E;\r\n" +
                               "CS1729 'object' does not contain a constructor that takes 0 arguments\r\n" +
                               "  at line 3 and character 10 in file C.cs | class ↓C\r\n";
                Assert.AreEqual(expected, exception.Message);
            }
        }
    }
}
