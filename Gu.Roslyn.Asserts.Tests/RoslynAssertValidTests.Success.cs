// ReSharper disable RedundantNameQualifier
// ReSharper disable AssignNullToNotNullAttribute
namespace Gu.Roslyn.Asserts.Tests
{
    using System.Linq;
    using NUnit.Framework;

    [TestFixture]
    public static partial class RoslynAssertValidTests
    {
        public class Success
        {
            [OneTimeSetUp]
            public void OneTimeSetUp()
            {
                RoslynAssert.MetadataReferences.Add(Gu.Roslyn.Asserts.MetadataReferences.CreateFromAssembly(typeof(object).Assembly).WithAliases(new[] { "global", "mscorlib" }));
                RoslynAssert.MetadataReferences.Add(Gu.Roslyn.Asserts.MetadataReferences.CreateFromAssembly(typeof(System.Diagnostics.Debug).Assembly).WithAliases(new[] { "global", "System" }));
                RoslynAssert.MetadataReferences.AddRange(Gu.Roslyn.Asserts.MetadataReferences.Transitive(
                    typeof(Microsoft.CodeAnalysis.CSharp.CSharpCompilation),
                    typeof(Microsoft.CodeAnalysis.CodeFixes.CodeFixProvider),
                    typeof(System.Runtime.CompilerServices.InternalsVisibleToAttribute),
                    typeof(NUnit.Framework.Assert)));
            }

            [OneTimeTearDown]
            public void OneTimeTearDown()
            {
                // Usually this is not needed but we want everything reset when testing the AnalyzerAssert.
                RoslynAssert.MetadataReferences.Clear();
            }

            [Test]
            public void WithSingleMetadataReference()
            {
                var code = @"
namespace RoslynSandbox
{
    class Foo
    {
    }
}";
                var analyzer = new NoErrorAnalyzer();
                RoslynAssert.Valid(analyzer, code, CodeFactory.DefaultCompilationOptions(analyzer, RoslynAssert.SuppressedDiagnostics), new[] { Gu.Roslyn.Asserts.MetadataReferences.CreateFromAssembly(typeof(object).Assembly) });
                RoslynAssert.Valid(analyzer, code, CodeFactory.DefaultCompilationOptions(analyzer, RoslynAssert.SuppressedDiagnostics), Gu.Roslyn.Asserts.MetadataReferences.Transitive(typeof(object).Assembly));
                RoslynAssert.Valid(analyzer, code, CodeFactory.DefaultCompilationOptions(analyzer, RoslynAssert.SuppressedDiagnostics), new[] { Gu.Roslyn.Asserts.MetadataReferences.CreateFromAssembly(typeof(object).Assembly).WithAliases(new[] { "global", "mscorlib" }) });
            }

            [Test]
            public void WithTransitiveMetadataReference()
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
                RoslynAssert.Valid(analyzer, code, CodeFactory.DefaultCompilationOptions(analyzer, RoslynAssert.SuppressedDiagnostics), metadataReferences);
            }

            [Test]
            public void SingleDocumentNoErrorAnalyzer()
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
                RoslynAssert.Valid(analyzer, code, CodeFactory.DefaultCompilationOptions(analyzer, RoslynAssert.SuppressedDiagnostics), RoslynAssert.MetadataReferences);
                RoslynAssert.Valid(analyzer, new[] { code }, CodeFactory.DefaultCompilationOptions(analyzer, RoslynAssert.SuppressedDiagnostics), RoslynAssert.MetadataReferences);
                RoslynAssert.Valid(typeof(NoErrorAnalyzer), code);

                var descriptor = NoErrorAnalyzer.Descriptor;
                RoslynAssert.Valid(analyzer, descriptor, code);
                RoslynAssert.Valid(analyzer, new[] { descriptor }, code);

                RoslynAssert.Valid(typeof(NoErrorAnalyzer), descriptor, code);
                RoslynAssert.Valid(typeof(NoErrorAnalyzer), new[] { descriptor }, code);
            }

            [Test]
            public void SevenPointThreeFeature()
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
                RoslynAssert.Valid(analyzer, code, CodeFactory.DefaultCompilationOptions(analyzer, RoslynAssert.SuppressedDiagnostics), RoslynAssert.MetadataReferences);
                RoslynAssert.Valid(analyzer, new[] { code }, CodeFactory.DefaultCompilationOptions(analyzer, RoslynAssert.SuppressedDiagnostics), RoslynAssert.MetadataReferences);
                RoslynAssert.Valid(typeof(NoErrorAnalyzer), code);

                var descriptor = NoErrorAnalyzer.Descriptor;
                RoslynAssert.Valid(analyzer, descriptor, code);
                RoslynAssert.Valid(typeof(NoErrorAnalyzer), descriptor, code);

                RoslynAssert.Valid(analyzer, new[] { descriptor }, code);
                RoslynAssert.Valid(typeof(NoErrorAnalyzer), new[] { descriptor }, code);
            }

            [Test]
            public void ProjectFileNoErrorAnalyzer()
            {
                var csproj = ProjectFile.Find("Gu.Roslyn.Asserts.csproj");
                var analyzer = new NoErrorAnalyzer();

                RoslynAssert.Valid(analyzer, csproj);
                RoslynAssert.Valid(typeof(NoErrorAnalyzer), csproj);

                var descriptor = NoErrorAnalyzer.Descriptor;
                RoslynAssert.Valid(analyzer, descriptor, csproj);
                RoslynAssert.Valid(analyzer, csproj, CodeFactory.DefaultCompilationOptions(analyzer, descriptor, null), RoslynAssert.MetadataReferences);
                RoslynAssert.Valid(typeof(NoErrorAnalyzer), descriptor, csproj);
            }

            [Test]
            public void TwoDocumentsNoErrorAnalyzer()
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
                RoslynAssert.Valid(analyzer, new [] { code1, code2 });
                RoslynAssert.Valid(typeof(NoErrorAnalyzer), code1, code2);
            }

            [Test]
            public void TwoProjectsNoErrorAnalyzer()
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
            public void WithExpectedDiagnostic()
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
                RoslynAssert.Valid(analyzer, new[] { descriptor }, code);
                RoslynAssert.Valid(analyzer, descriptor, new [] { code });
                RoslynAssert.Valid(typeof(FieldNameMustNotBeginWithUnderscore), descriptor, code);
                RoslynAssert.Valid(typeof(FieldNameMustNotBeginWithUnderscore), new[] { descriptor }, code);
            }

            [Test]
            public void WithExpectedDiagnosticWhenOtherReportsError()
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
                RoslynAssert.Valid(analyzer, new[] { descriptor }, code);
                RoslynAssert.Valid(typeof(FieldAndPropertyMustBeNamedFooAnalyzer), descriptor, code);
                RoslynAssert.Valid(typeof(FieldAndPropertyMustBeNamedFooAnalyzer), new[] { descriptor }, code);
            }

            [Test]
            public void WithExpectedDiagnosticWhenAnalyzerSupportsTwoDiagnostics()
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
                RoslynAssert.Valid(analyzer, new[] { descriptor }, code);
                RoslynAssert.Valid(typeof(FieldNameMustNotBeginWithUnderscoreReportsTwo), descriptor, code);
                RoslynAssert.Valid(typeof(FieldNameMustNotBeginWithUnderscoreReportsTwo), new[] { descriptor }, code);
            }

            [Test]
            public void Issue53()
            {
                var resourcesCode = @"
namespace RoslynSandbox.Properties
{
    public class Resources
    {
    }
}";

                var testCode = @"
namespace RoslynSandbox
{
    using RoslynSandbox.Properties;

    public class Foo
    {
    }
}";
                var analyzer = new FieldNameMustNotBeginWithUnderscoreReportsTwo();
                RoslynAssert.Valid(analyzer, resourcesCode, testCode);
                RoslynAssert.Valid(analyzer, testCode, resourcesCode);
                RoslynAssert.Valid(typeof(FieldNameMustNotBeginWithUnderscoreReportsTwo), resourcesCode, testCode);
            }

            [Test]
            public void AnalyzerWithTwoDiagnostics()
            {
                var testCode = @"
namespace RoslynSandbox
{
    public class Foo
    {
        private int foo;
    }
}";
                var analyzer = new FieldAndPropertyMustBeNamedFooAnalyzer();
                RoslynAssert.Valid(analyzer, testCode);
            }

            [Test]
            public void BinaryStrings()
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
                RoslynAssert.Valid(analyzer, code, CodeFactory.DefaultCompilationOptions(new[] { analyzer }), RoslynAssert.MetadataReferences.Append(Asserts.MetadataReferences.CreateBinary(binaryReferencedCode)));
                RoslynAssert.Valid(analyzer, code, metadataReferences: RoslynAssert.MetadataReferences.Append(Asserts.MetadataReferences.CreateBinary(binaryReferencedCode)));
            }

            [Test]
            public void BinarySolution()
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
