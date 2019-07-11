// ReSharper disable RedundantNameQualifier
// ReSharper disable AssignNullToNotNullAttribute
namespace Gu.Roslyn.Asserts.Tests
{
    using System.Linq;
    using Microsoft.CodeAnalysis;
    using NUnit.Framework;

    [TestFixture]
    public static partial class RoslynAssertValidTests
    {
        public static class Success
        {
            [OneTimeSetUp]
            public static void OneTimeSetUp()
            {
                RoslynAssert.MetadataReferences.Add(MetadataReference.CreateFromFile(typeof(int).Assembly.Location));
            }

            [OneTimeTearDown]
            public static void OneTimeTearDown()
            {
                RoslynAssert.ResetAll();
            }

            [Test]
            public static void WithSingleMetadataReference()
            {
                var code = @"
namespace RoslynSandbox
{
    class Foo
    {
    }
}";
                var analyzer = new NoErrorAnalyzer();
                RoslynAssert.Valid(analyzer, code, metadataReferences: new[] { Gu.Roslyn.Asserts.MetadataReferences.CreateFromAssembly(typeof(object).Assembly) }, compilationOptions: CodeFactory.DefaultCompilationOptions(analyzer, RoslynAssert.SuppressedDiagnostics));
            }

            [Test]
            public static void WithTransitiveMetadataReference()
            {
                var code = @"
namespace RoslynSandbox
{
    class Foo
    {
    }
}";
                var analyzer = new NoErrorAnalyzer();
                var metadataReferences = Gu.Roslyn.Asserts.MetadataReferences.Transitive(typeof(Microsoft.CodeAnalysis.CSharp.CSharpCompilation)).ToArray();
                RoslynAssert.Valid(analyzer, code, metadataReferences: metadataReferences, compilationOptions: CodeFactory.DefaultCompilationOptions(analyzer, RoslynAssert.SuppressedDiagnostics));
            }

            [Test]
            public static void SingleDocumentNoErrorAnalyzer()
            {
                var code = @"
namespace RoslynSandbox
{
    class Foo
    {
    }
}";
                var analyzer = new NoErrorAnalyzer();
                RoslynAssert.Valid(analyzer, code);
                RoslynAssert.Valid(typeof(NoErrorAnalyzer), code);

                var descriptor = NoErrorAnalyzer.Descriptor;
                RoslynAssert.Valid(analyzer, descriptor, code);
                RoslynAssert.Valid(typeof(NoErrorAnalyzer), descriptor, code);
            }

            [Test]
            public static void SevenPointThreeFeature()
            {
                var code = @"
namespace RoslynSandbox
{
    class Foo<T>
        where T : struct, System.Enum
    {
    }
}";
                var analyzer = new NoErrorAnalyzer();
                RoslynAssert.Valid(analyzer, code);
                RoslynAssert.Valid(typeof(NoErrorAnalyzer), code);

                var descriptor = NoErrorAnalyzer.Descriptor;
                RoslynAssert.Valid(analyzer, descriptor, code);
                RoslynAssert.Valid(typeof(NoErrorAnalyzer), descriptor, code);
            }

            [Test]
            public static void ProjectFileNoErrorAnalyzer()
            {
                var code = ProjectFile.Find("Gu.Roslyn.Asserts.csproj");
                var analyzer = new NoErrorAnalyzer();

                var metadataReferences = Gu.Roslyn.Asserts.MetadataReferences.Transitive(
                    typeof(Microsoft.CodeAnalysis.CSharp.CSharpCompilation),
                    typeof(Microsoft.CodeAnalysis.CodeFixes.CodeFixProvider));
                RoslynAssert.Valid(analyzer, code, metadataReferences: metadataReferences);
                RoslynAssert.Valid(typeof(NoErrorAnalyzer), code, metadataReferences: metadataReferences);

                var descriptor = NoErrorAnalyzer.Descriptor;
                RoslynAssert.Valid(analyzer, descriptor, code, metadataReferences: metadataReferences);
                RoslynAssert.Valid(typeof(NoErrorAnalyzer), descriptor, code, metadataReferences: metadataReferences);
            }

            [Test]
            public static void TwoDocumentsNoErrorAnalyzer()
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
                var analyzer = new NoErrorAnalyzer();
                RoslynAssert.Valid(analyzer, code1, code2);
                RoslynAssert.Valid(analyzer, code2, code1);
                RoslynAssert.Valid(typeof(NoErrorAnalyzer), code1, code2);
            }

            [Test]
            public static void TwoProjectsNoErrorAnalyzer()
            {
                var code1 = @"
namespace Project1
{
    class Code1
    {
    }
}";
                var code2 = @"
namespace Project2
{
    class Code2
    {
    }
}";
                var analyzer = new NoErrorAnalyzer();
                RoslynAssert.Valid(analyzer, code1, code2);
                RoslynAssert.Valid(analyzer, code2, code1);
                RoslynAssert.Valid(typeof(NoErrorAnalyzer), code1, code2);
            }

            [Test]
            public static void WithExpectedDiagnostic()
            {
                var code = @"
namespace RoslynSandbox
{
    class Foo
    {
        private readonly int value1;
    }
}";
                var analyzer = new FieldNameMustNotBeginWithUnderscore();
                var descriptor = FieldNameMustNotBeginWithUnderscore.Descriptor;
                RoslynAssert.Valid(analyzer, descriptor, code);
                RoslynAssert.Valid(typeof(FieldNameMustNotBeginWithUnderscore), descriptor, code);
            }

            [Test]
            public static void WithExpectedDiagnosticWhenOtherReportsError()
            {
                var code = @"
namespace RoslynSandbox
{
    class Foo
    {
        private readonly int foo;
        
        public int WrongName { get; set; }
    }
}";

                var descriptor = FieldAndPropertyMustBeNamedFooAnalyzer.FieldDescriptor;
                var analyzer = new FieldAndPropertyMustBeNamedFooAnalyzer();
                RoslynAssert.Valid(analyzer, descriptor, code);
                RoslynAssert.Valid(typeof(FieldAndPropertyMustBeNamedFooAnalyzer), descriptor, code);
            }

            [Test]
            public static void WithExpectedDiagnosticWhenAnalyzerSupportsTwoDiagnostics()
            {
                var code = @"
namespace RoslynSandbox
{
    class Foo
    {
        private readonly int value1;
    }
}";

                var descriptor = FieldNameMustNotBeginWithUnderscoreReportsTwo.Descriptor1;
                var analyzer = new FieldNameMustNotBeginWithUnderscoreReportsTwo();
                RoslynAssert.Valid(analyzer, descriptor, code);
                RoslynAssert.Valid(typeof(FieldNameMustNotBeginWithUnderscoreReportsTwo), descriptor, code);
            }

            [Test]
            public static void Issue53()
            {
                var resourcesCode = @"
namespace RoslynSandbox.Properties
{
    public class Resources
    {
    }
}";

                var code = @"
namespace RoslynSandbox
{
    using RoslynSandbox.Properties;

    public class Foo
    {
    }
}";
                var analyzer = new FieldNameMustNotBeginWithUnderscoreReportsTwo();
                RoslynAssert.Valid(analyzer, resourcesCode, code);
                RoslynAssert.Valid(analyzer, code, resourcesCode);
                RoslynAssert.Valid(typeof(FieldNameMustNotBeginWithUnderscoreReportsTwo), resourcesCode, code);
            }

            [Test]
            public static void AnalyzerWithTwoDiagnostics()
            {
                var code = @"
namespace RoslynSandbox
{
    public class Foo
    {
        private int foo;
    }
}";
                var analyzer = new FieldAndPropertyMustBeNamedFooAnalyzer();
                RoslynAssert.Valid(analyzer, code);
            }

            [Test]
            public static void BinaryStrings()
            {
                var binaryReferencedCode = @"
namespace BinaryReferencedAssembly
{
    public class Base
    {
        private int _fieldName;
    }
}";
                var code = @"
namespace RoslynSandbox
{
    using System.Reflection;

    public class C : BinaryReferencedAssembly.Base
    {
        private int f;
    }
}";
                var analyzer = new FieldNameMustNotBeginWithUnderscore();
                RoslynAssert.Valid(analyzer, code, metadataReferences: RoslynAssert.MetadataReferences.Append(Asserts.MetadataReferences.CreateBinary(binaryReferencedCode)), compilationOptions: CodeFactory.DefaultCompilationOptions(new[] { analyzer }));
                RoslynAssert.Valid(analyzer, code, metadataReferences: RoslynAssert.MetadataReferences.Append(Asserts.MetadataReferences.CreateBinary(binaryReferencedCode)));
            }

            [Test]
            public static void BinarySolution()
            {
                var binaryReferencedCode = @"
namespace BinaryReferencedAssembly
{
    public class Base
    {
        private int _fieldName;
    }
}";
                var code = @"
namespace RoslynSandbox
{
    using System.Reflection;

    public class C : BinaryReferencedAssembly.Base
    {
        private int f;
    }
}";
                var analyzer = new FieldNameMustNotBeginWithUnderscore();
                var solution = CodeFactory.CreateSolution(
                    code,
                    CodeFactory.DefaultCompilationOptions(new[] { analyzer }),
                    RoslynAssert.MetadataReferences.Append(Asserts.MetadataReferences.CreateBinary(binaryReferencedCode)));

                RoslynAssert.Valid(analyzer, solution);
            }
        }
    }
}
