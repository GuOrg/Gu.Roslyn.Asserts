// ReSharper disable RedundantNameQualifier
namespace Gu.Roslyn.Asserts.Tests.RoslynAssertTests;

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
            var code = """
                namespace N
                {
                    class C
                    {
                    }
                }
                """;
            RoslynAssert.NoCompilerDiagnostics(code);
        }

        [Test]
        public static void WhenCodeNoCompilerErrorsCollectionInitializer()
        {
            var code = """
                namespace N
                {
                    using System.Collections.Generic;

                    class C
                    {
                        public C()
                        {
                            var xs = new List<int> { 1, 2, 3 };
                        }
                    }
                }
                """;
            RoslynAssert.NoCompilerDiagnostics(code);
        }

        [Test]
        public static void WhenCodeHasCompilerErrors()
        {
            var code = """
                namespace N
                {
                    class C
                    {
                        public event EventHandler E;
                    }
                }
                """;
            var exception = Assert.Throws<AssertException>(() => RoslynAssert.NoCompilerDiagnostics(code, Settings.Default.WithMetadataReferences(Enumerable.Empty<MetadataReference>())));
            var expected = """
                Expected no diagnostics, found:
                CS0518 Predefined type 'System.Object' is not defined or imported
                  at line 2 and character 10 in file C.cs | class ↓C
                CS0518 Predefined type 'System.Object' is not defined or imported
                  at line 4 and character 21 in file C.cs | public event ↓EventHandler E;
                CS0246 The type or namespace name 'EventHandler' could not be found (are you missing a using directive or an assembly reference?)
                  at line 4 and character 21 in file C.cs | public event ↓EventHandler E;
                CS0518 Predefined type 'System.Void' is not defined or imported
                  at line 4 and character 34 in file C.cs | public event EventHandler ↓E;
                CS0518 Predefined type 'System.Void' is not defined or imported
                  at line 4 and character 34 in file C.cs | public event EventHandler ↓E;
                CS1729 'object' does not contain a constructor that takes 0 arguments
                  at line 2 and character 10 in file C.cs | class ↓C
                CS0067 The event 'C.E' is never used
                  at line 4 and character 34 in file C.cs | public event EventHandler ↓E;

                """;
            Assert.AreEqual(expected, exception.Message);
        }
    }
}
