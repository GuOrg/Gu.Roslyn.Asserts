// ReSharper disable RedundantNameQualifier
// ReSharper disable AssignNullToNotNullAttribute
namespace Gu.Roslyn.Asserts.Tests
{
    using System.Linq;
    using Microsoft.CodeAnalysis;
    using NUnit.Framework;

    [TestFixture]
    public static partial class RoslynAssertTests
    {
        public static class ValidSuccess
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
namespace N
{
    class C
    {
    }
}";
                var analyzer = new NopAnalyzer();
                RoslynAssert.Valid(analyzer, code, metadataReferences: new[] { Gu.Roslyn.Asserts.MetadataReferences.CreateFromAssembly(typeof(object).Assembly) }, compilationOptions: CodeFactory.DefaultCompilationOptions(analyzer, RoslynAssert.SuppressedDiagnostics));
            }

            [Test]
            public static void WithTransitiveMetadataReference()
            {
                var code = @"
namespace N
{
    class C
    {
    }
}";
                var analyzer = new NopAnalyzer();
                var metadataReferences = Gu.Roslyn.Asserts.MetadataReferences.Transitive(typeof(Microsoft.CodeAnalysis.CSharp.CSharpCompilation)).ToArray();
                RoslynAssert.Valid(analyzer, code, metadataReferences: metadataReferences, compilationOptions: CodeFactory.DefaultCompilationOptions(analyzer, RoslynAssert.SuppressedDiagnostics));
            }

            [Test]
            public static void SingleDocumentNopAnalyzer()
            {
                var code = @"
namespace N
{
    class C
    {
    }
}";
                var descriptor = Descriptors.Id1;
                var analyzer = new NopAnalyzer(descriptor);
                RoslynAssert.Valid(analyzer, code);
                RoslynAssert.Valid(typeof(NopAnalyzer), code);

                RoslynAssert.Valid(analyzer, descriptor, code);
                RoslynAssert.Valid(typeof(NopAnalyzer), descriptor, code);
            }

            [Test]
            public static void SevenPointThreeFeature()
            {
                var code = @"
namespace N
{
    class C<T>
        where T : struct, System.Enum
    {
    }
}";
                var descriptor = Descriptors.Id1;
                var analyzer = new NopAnalyzer(descriptor);
                RoslynAssert.Valid(analyzer, code);
                RoslynAssert.Valid(typeof(NopAnalyzer), code);

                RoslynAssert.Valid(analyzer, descriptor, code);
                RoslynAssert.Valid(typeof(NopAnalyzer), descriptor, code);
            }

            [Test]
            public static void ProjectFileNopAnalyzer()
            {
                var code = ProjectFile.Find("Gu.Roslyn.Asserts.csproj");
                var metadataReferences = Gu.Roslyn.Asserts.MetadataReferences.Transitive(
                    typeof(Microsoft.CodeAnalysis.CSharp.CSharpCompilation),
                    typeof(Microsoft.CodeAnalysis.CodeFixes.CodeFixProvider));
                var descriptor = Descriptors.Id1;
                var analyzer = new NopAnalyzer(descriptor);
                RoslynAssert.Valid(analyzer, code, metadataReferences: metadataReferences);
                RoslynAssert.Valid(typeof(NopAnalyzer), code, metadataReferences: metadataReferences);

                RoslynAssert.Valid(analyzer, descriptor, code, metadataReferences: metadataReferences);
                RoslynAssert.Valid(typeof(NopAnalyzer), descriptor, code, metadataReferences: metadataReferences);
            }

            [Test]
            public static void TwoDocumentsNopAnalyzer()
            {
                var code1 = @"
namespace N
{
    class Code1
    {
    }
}";
                var code2 = @"
namespace N
{
    class Code2
    {
    }
}";
                var analyzer = new NopAnalyzer();
                RoslynAssert.Valid(analyzer, code1, code2);
                RoslynAssert.Valid(analyzer, code2, code1);
                RoslynAssert.Valid(typeof(NopAnalyzer), code1, code2);
            }

            [Test]
            public static void TwoProjectsNopAnalyzer()
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
                var analyzer = new NopAnalyzer();
                RoslynAssert.Valid(analyzer, code1, code2);
                RoslynAssert.Valid(analyzer, code2, code1);
                RoslynAssert.Valid(typeof(NopAnalyzer), code1, code2);
            }

            [Test]
            public static void WithExpectedDiagnostic()
            {
                var code = @"
namespace N
{
    class C
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
namespace N
{
    class Value
    {
        private readonly int value;
        
        public int WrongName { get; set; }
    }
}";

                var descriptor = FieldAndPropertyMustBeNamedValueAnalyzer.FieldDescriptor;
                var analyzer = new FieldAndPropertyMustBeNamedValueAnalyzer();
                RoslynAssert.Valid(analyzer, descriptor, code);
                RoslynAssert.Valid(typeof(FieldAndPropertyMustBeNamedValueAnalyzer), descriptor, code);
            }

            [Test]
            public static void WithExpectedDiagnosticWhenAnalyzerSupportsTwoDiagnostics()
            {
                var code = @"
namespace N
{
    class C
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
namespace N.Properties
{
    public class Resources
    {
    }
}";

                var code = @"
namespace N
{
    using N.Properties;

    public class C
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
namespace N
{
    public class C
    {
        private int value;
    }
}";
                var analyzer = new FieldAndPropertyMustBeNamedValueAnalyzer();
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
namespace N
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
namespace N
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
