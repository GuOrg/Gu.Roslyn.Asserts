// ReSharper disable RedundantNameQualifier
// ReSharper disable AssignNullToNotNullAttribute
namespace Gu.Roslyn.Asserts.Tests
{
    using System.Collections.Generic;
    using System.Linq;
    using NUnit.Framework;

    [TestFixture]
    public partial class AnalyzerAssertValidTests
    {
        public class Success
        {
            [OneTimeSetUp]
            public void OneTimeSetUp()
            {
                AnalyzerAssert.MetadataReferences.Add(Gu.Roslyn.Asserts.MetadataReferences.CreateFromAssembly(typeof(object).Assembly).WithAliases(new[] { "global", "mscorlib" }));
                AnalyzerAssert.MetadataReferences.Add(Gu.Roslyn.Asserts.MetadataReferences.CreateFromAssembly(typeof(System.Diagnostics.Debug).Assembly).WithAliases(new[] { "global", "System" }));
                AnalyzerAssert.MetadataReferences.AddRange(Gu.Roslyn.Asserts.MetadataReferences.Transitive(
                    typeof(Microsoft.CodeAnalysis.CSharp.CSharpCompilation),
                    typeof(Microsoft.CodeAnalysis.CodeFixes.CodeFixProvider),
                    typeof(System.Runtime.CompilerServices.InternalsVisibleToAttribute),
                    typeof(NUnit.Framework.Assert)));
            }

            [OneTimeTearDown]
            public void OneTimeTearDown()
            {
                // Usually this is not needed but we want everything reset when testing the AnalyzerAssert.
                AnalyzerAssert.MetadataReferences.Clear();
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
                AnalyzerAssert.Valid(analyzer, code, CodeFactory.DefaultCompilationOptions(analyzer, AnalyzerAssert.SuppressedDiagnostics), new[] { Gu.Roslyn.Asserts.MetadataReferences.CreateFromAssembly(typeof(object).Assembly) });
                AnalyzerAssert.Valid(analyzer, code, CodeFactory.DefaultCompilationOptions(analyzer, AnalyzerAssert.SuppressedDiagnostics), Gu.Roslyn.Asserts.MetadataReferences.Transitive(typeof(object).Assembly));
                AnalyzerAssert.Valid(analyzer, code, CodeFactory.DefaultCompilationOptions(analyzer, AnalyzerAssert.SuppressedDiagnostics), new[] { Gu.Roslyn.Asserts.MetadataReferences.CreateFromAssembly(typeof(object).Assembly).WithAliases(new[] { "global", "mscorlib" }) });
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
                AnalyzerAssert.Valid(analyzer, code, CodeFactory.DefaultCompilationOptions(analyzer, AnalyzerAssert.SuppressedDiagnostics), metadataReferences);
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
                AnalyzerAssert.Valid<NoErrorAnalyzer>(code);
                AnalyzerAssert.Valid(typeof(NoErrorAnalyzer), code);
                AnalyzerAssert.Valid(analyzer, code);
                AnalyzerAssert.Valid(analyzer, code, CodeFactory.DefaultCompilationOptions(analyzer, AnalyzerAssert.SuppressedDiagnostics), AnalyzerAssert.MetadataReferences);
                AnalyzerAssert.Valid(analyzer, new[] { code }, CodeFactory.DefaultCompilationOptions(analyzer, AnalyzerAssert.SuppressedDiagnostics), AnalyzerAssert.MetadataReferences);

                var descriptor = NoErrorAnalyzer.Descriptor;
                AnalyzerAssert.Valid<NoErrorAnalyzer>(descriptor, code);
                AnalyzerAssert.Valid(typeof(NoErrorAnalyzer), descriptor, code);
                AnalyzerAssert.Valid(analyzer, descriptor, code);

                AnalyzerAssert.Valid<NoErrorAnalyzer>(new[] { descriptor }, code);
                AnalyzerAssert.Valid(typeof(NoErrorAnalyzer), new[] { descriptor }, code);
                AnalyzerAssert.Valid(analyzer, new[] { descriptor }, code);
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
                AnalyzerAssert.Valid<NoErrorAnalyzer>(code);
                AnalyzerAssert.Valid(typeof(NoErrorAnalyzer), code);
                AnalyzerAssert.Valid(analyzer, code);
                AnalyzerAssert.Valid(analyzer, code, CodeFactory.DefaultCompilationOptions(analyzer, AnalyzerAssert.SuppressedDiagnostics), AnalyzerAssert.MetadataReferences);
                AnalyzerAssert.Valid(analyzer, new[] { code }, CodeFactory.DefaultCompilationOptions(analyzer, AnalyzerAssert.SuppressedDiagnostics), AnalyzerAssert.MetadataReferences);

                var descriptor = NoErrorAnalyzer.Descriptor;
                AnalyzerAssert.Valid<NoErrorAnalyzer>(descriptor, code);
                AnalyzerAssert.Valid(typeof(NoErrorAnalyzer), descriptor, code);
                AnalyzerAssert.Valid(analyzer, descriptor, code);

                AnalyzerAssert.Valid<NoErrorAnalyzer>(new[] { descriptor }, code);
                AnalyzerAssert.Valid(typeof(NoErrorAnalyzer), new[] { descriptor }, code);
                AnalyzerAssert.Valid(analyzer, new[] { descriptor }, code);
            }

            [Test]
            public void ProjectFileNoErrorAnalyzer()
            {
                var csproj = ProjectFile.Find("Gu.Roslyn.Asserts.csproj");
                var analyzer = new NoErrorAnalyzer();

                AnalyzerAssert.Valid<NoErrorAnalyzer>(csproj);
                AnalyzerAssert.Valid(typeof(NoErrorAnalyzer), csproj);
                AnalyzerAssert.Valid(analyzer, csproj);

                var descriptor = NoErrorAnalyzer.Descriptor;
                AnalyzerAssert.Valid<NoErrorAnalyzer>(descriptor, csproj);
                AnalyzerAssert.Valid(typeof(NoErrorAnalyzer), descriptor, csproj);
                AnalyzerAssert.Valid(analyzer, descriptor, csproj);
                AnalyzerAssert.Valid(analyzer, csproj, CodeFactory.DefaultCompilationOptions(analyzer, descriptor, null), AnalyzerAssert.MetadataReferences);
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
                AnalyzerAssert.Valid<NoErrorAnalyzer>(code1, code2);
                AnalyzerAssert.Valid(typeof(NoErrorAnalyzer), code1, code2);
                var analyzer = new NoErrorAnalyzer();
                AnalyzerAssert.Valid(analyzer, code1, code2);

                AnalyzerAssert.Valid(analyzer, new List<string> { code1, code2 });
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
                AnalyzerAssert.Valid<NoErrorAnalyzer>(code1, code2);
                AnalyzerAssert.Valid(typeof(NoErrorAnalyzer), code1, code2);
                AnalyzerAssert.Valid(new NoErrorAnalyzer(), code1, code2);
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
                AnalyzerAssert.Valid<FieldNameMustNotBeginWithUnderscore>(descriptor, code);
                AnalyzerAssert.Valid(typeof(FieldNameMustNotBeginWithUnderscore), descriptor, code);
                AnalyzerAssert.Valid(analyzer, descriptor, code);
                AnalyzerAssert.Valid<FieldNameMustNotBeginWithUnderscore>(new[] { descriptor }, code);
                AnalyzerAssert.Valid(typeof(FieldNameMustNotBeginWithUnderscore), new[] { descriptor }, code);
                AnalyzerAssert.Valid(analyzer, new[] { descriptor }, code);
                AnalyzerAssert.Valid(analyzer, descriptor, new List<string> { code });
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
                AnalyzerAssert.Valid<FieldAndPropertyMustBeNamedFooAnalyzer>(descriptor, code);
                AnalyzerAssert.Valid(typeof(FieldAndPropertyMustBeNamedFooAnalyzer), descriptor, code);
                AnalyzerAssert.Valid(new FieldAndPropertyMustBeNamedFooAnalyzer(), descriptor, code);
                AnalyzerAssert.Valid<FieldAndPropertyMustBeNamedFooAnalyzer>(new[] { descriptor }, code);
                AnalyzerAssert.Valid(typeof(FieldAndPropertyMustBeNamedFooAnalyzer), new[] { descriptor }, code);
                AnalyzerAssert.Valid(new FieldAndPropertyMustBeNamedFooAnalyzer(), new[] { descriptor }, code);
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
                AnalyzerAssert.Valid<FieldNameMustNotBeginWithUnderscoreReportsTwo>(descriptor, code);
                AnalyzerAssert.Valid(typeof(FieldNameMustNotBeginWithUnderscoreReportsTwo), descriptor, code);
                AnalyzerAssert.Valid(new FieldNameMustNotBeginWithUnderscoreReportsTwo(), descriptor, code);
                AnalyzerAssert.Valid<FieldNameMustNotBeginWithUnderscoreReportsTwo>(new[] { descriptor }, code);
                AnalyzerAssert.Valid(typeof(FieldNameMustNotBeginWithUnderscoreReportsTwo), new[] { descriptor }, code);
                AnalyzerAssert.Valid(new FieldNameMustNotBeginWithUnderscoreReportsTwo(), new[] { descriptor }, code);
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
                AnalyzerAssert.Valid<FieldNameMustNotBeginWithUnderscoreReportsTwo>(resourcesCode, testCode);
                AnalyzerAssert.Valid(new FieldNameMustNotBeginWithUnderscoreReportsTwo(), resourcesCode, testCode);
                AnalyzerAssert.Valid(typeof(FieldNameMustNotBeginWithUnderscoreReportsTwo), resourcesCode, testCode);
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
                AnalyzerAssert.Valid(new FieldAndPropertyMustBeNamedFooAnalyzer(), testCode);
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
                AnalyzerAssert.Valid(analyzer, code, CodeFactory.DefaultCompilationOptions(new[] { analyzer }), AnalyzerAssert.MetadataReferences.Append(Asserts.MetadataReferences.CreateBinary(binaryReferencedCode)));
                AnalyzerAssert.Valid(analyzer, code, metadataReferences: AnalyzerAssert.MetadataReferences.Append(Asserts.MetadataReferences.CreateBinary(binaryReferencedCode)));
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
                    AnalyzerAssert.MetadataReferences.Append(Asserts.MetadataReferences.CreateBinary(binaryReferencedCode)));

                AnalyzerAssert.Valid(analyzer, solution);
            }
        }
    }
}
