// ReSharper disable RedundantNameQualifier
// ReSharper disable AssignNullToNotNullAttribute
namespace Gu.Roslyn.Asserts.Tests
{
    using System.Collections.Generic;
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
                RoslynAssert.Valid<NoErrorAnalyzer>(code);
                RoslynAssert.Valid(typeof(NoErrorAnalyzer), code);
                RoslynAssert.Valid(analyzer, code);
                RoslynAssert.Valid(analyzer, code, CodeFactory.DefaultCompilationOptions(analyzer, RoslynAssert.SuppressedDiagnostics), RoslynAssert.MetadataReferences);
                RoslynAssert.Valid(analyzer, new[] { code }, CodeFactory.DefaultCompilationOptions(analyzer, RoslynAssert.SuppressedDiagnostics), RoslynAssert.MetadataReferences);

                var descriptor = NoErrorAnalyzer.Descriptor;
                RoslynAssert.Valid<NoErrorAnalyzer>(descriptor, code);
                RoslynAssert.Valid(typeof(NoErrorAnalyzer), descriptor, code);
                RoslynAssert.Valid(analyzer, descriptor, code);

                RoslynAssert.Valid<NoErrorAnalyzer>(new[] { descriptor }, code);
                RoslynAssert.Valid(typeof(NoErrorAnalyzer), new[] { descriptor }, code);
                RoslynAssert.Valid(analyzer, new[] { descriptor }, code);
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
                RoslynAssert.Valid<NoErrorAnalyzer>(code);
                RoslynAssert.Valid(typeof(NoErrorAnalyzer), code);
                RoslynAssert.Valid(analyzer, code);
                RoslynAssert.Valid(analyzer, code, CodeFactory.DefaultCompilationOptions(analyzer, RoslynAssert.SuppressedDiagnostics), RoslynAssert.MetadataReferences);
                RoslynAssert.Valid(analyzer, new[] { code }, CodeFactory.DefaultCompilationOptions(analyzer, RoslynAssert.SuppressedDiagnostics), RoslynAssert.MetadataReferences);

                var descriptor = NoErrorAnalyzer.Descriptor;
                RoslynAssert.Valid<NoErrorAnalyzer>(descriptor, code);
                RoslynAssert.Valid(typeof(NoErrorAnalyzer), descriptor, code);
                RoslynAssert.Valid(analyzer, descriptor, code);

                RoslynAssert.Valid<NoErrorAnalyzer>(new[] { descriptor }, code);
                RoslynAssert.Valid(typeof(NoErrorAnalyzer), new[] { descriptor }, code);
                RoslynAssert.Valid(analyzer, new[] { descriptor }, code);
            }

            [Test]
            public void ProjectFileNoErrorAnalyzer()
            {
                var csproj = ProjectFile.Find("Gu.Roslyn.Asserts.csproj");
                var analyzer = new NoErrorAnalyzer();

                RoslynAssert.Valid<NoErrorAnalyzer>(csproj);
                RoslynAssert.Valid(typeof(NoErrorAnalyzer), csproj);
                RoslynAssert.Valid(analyzer, csproj);

                var descriptor = NoErrorAnalyzer.Descriptor;
                RoslynAssert.Valid<NoErrorAnalyzer>(descriptor, csproj);
                RoslynAssert.Valid(typeof(NoErrorAnalyzer), descriptor, csproj);
                RoslynAssert.Valid(analyzer, descriptor, csproj);
                RoslynAssert.Valid(analyzer, csproj, CodeFactory.DefaultCompilationOptions(analyzer, descriptor, null), RoslynAssert.MetadataReferences);
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
                RoslynAssert.Valid<NoErrorAnalyzer>(code1, code2);
                RoslynAssert.Valid(typeof(NoErrorAnalyzer), code1, code2);
                var analyzer = new NoErrorAnalyzer();
                RoslynAssert.Valid(analyzer, code1, code2);

                RoslynAssert.Valid(analyzer, new List<string> { code1, code2 });
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
                RoslynAssert.Valid<NoErrorAnalyzer>(code1, code2);
                RoslynAssert.Valid(typeof(NoErrorAnalyzer), code1, code2);
                RoslynAssert.Valid(new NoErrorAnalyzer(), code1, code2);
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
                RoslynAssert.Valid<FieldNameMustNotBeginWithUnderscore>(descriptor, code);
                RoslynAssert.Valid(typeof(FieldNameMustNotBeginWithUnderscore), descriptor, code);
                RoslynAssert.Valid(analyzer, descriptor, code);
                RoslynAssert.Valid<FieldNameMustNotBeginWithUnderscore>(new[] { descriptor }, code);
                RoslynAssert.Valid(typeof(FieldNameMustNotBeginWithUnderscore), new[] { descriptor }, code);
                RoslynAssert.Valid(analyzer, new[] { descriptor }, code);
                RoslynAssert.Valid(analyzer, descriptor, new List<string> { code });
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
                RoslynAssert.Valid<FieldAndPropertyMustBeNamedFooAnalyzer>(descriptor, code);
                RoslynAssert.Valid(typeof(FieldAndPropertyMustBeNamedFooAnalyzer), descriptor, code);
                RoslynAssert.Valid(new FieldAndPropertyMustBeNamedFooAnalyzer(), descriptor, code);
                RoslynAssert.Valid<FieldAndPropertyMustBeNamedFooAnalyzer>(new[] { descriptor }, code);
                RoslynAssert.Valid(typeof(FieldAndPropertyMustBeNamedFooAnalyzer), new[] { descriptor }, code);
                RoslynAssert.Valid(new FieldAndPropertyMustBeNamedFooAnalyzer(), new[] { descriptor }, code);
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
                RoslynAssert.Valid<FieldNameMustNotBeginWithUnderscoreReportsTwo>(descriptor, code);
                RoslynAssert.Valid(typeof(FieldNameMustNotBeginWithUnderscoreReportsTwo), descriptor, code);
                RoslynAssert.Valid(new FieldNameMustNotBeginWithUnderscoreReportsTwo(), descriptor, code);
                RoslynAssert.Valid<FieldNameMustNotBeginWithUnderscoreReportsTwo>(new[] { descriptor }, code);
                RoslynAssert.Valid(typeof(FieldNameMustNotBeginWithUnderscoreReportsTwo), new[] { descriptor }, code);
                RoslynAssert.Valid(new FieldNameMustNotBeginWithUnderscoreReportsTwo(), new[] { descriptor }, code);
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
                RoslynAssert.Valid<FieldNameMustNotBeginWithUnderscoreReportsTwo>(resourcesCode, testCode);
                RoslynAssert.Valid(new FieldNameMustNotBeginWithUnderscoreReportsTwo(), resourcesCode, testCode);
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
                RoslynAssert.Valid(new FieldAndPropertyMustBeNamedFooAnalyzer(), testCode);
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
