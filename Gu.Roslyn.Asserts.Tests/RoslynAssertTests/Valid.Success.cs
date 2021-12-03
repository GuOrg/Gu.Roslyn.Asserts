// ReSharper disable RedundantNameQualifier
// ReSharper disable AssignNullToNotNullAttribute
namespace Gu.Roslyn.Asserts.Tests.RoslynAssertTests
{
    using System.Linq;

    using NUnit.Framework;

    public static partial class Valid
    {
        public static class Success
        {
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
                var metadataReferences = new[] { Gu.Roslyn.Asserts.MetadataReferences.CreateFromAssembly(typeof(object).Assembly) };
                RoslynAssert.Valid(analyzer, code, Settings.Default.WithMetadataReferences(metadataReferences));
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
                RoslynAssert.Valid(analyzer, code, Settings.Default.WithMetadataReferences(metadataReferences));
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
                Assert.Inconclusive("This has dependency problems in bot net472 and netcoreapp.");
                var code = ProjectFile.Find("Gu.Roslyn.Asserts.csproj");
                var metadataReferences = Gu.Roslyn.Asserts.MetadataReferences.Transitive(
                                               typeof(Microsoft.CodeAnalysis.CSharp.CSharpCompilation),
                                               typeof(Microsoft.CodeAnalysis.CodeFixes.CodeFixProvider))
                                           .ToArray();
                var descriptor = Descriptors.Id1;
                var analyzer = new NopAnalyzer(descriptor);
                RoslynAssert.Valid(analyzer, code, Settings.Default.WithMetadataReferences(metadataReferences));
                RoslynAssert.Valid(typeof(NopAnalyzer), code, Settings.Default.WithMetadataReferences(metadataReferences));

                RoslynAssert.Valid(analyzer, descriptor, code, Settings.Default.WithMetadataReferences(metadataReferences));
                RoslynAssert.Valid(typeof(NopAnalyzer), descriptor, code, Settings.Default.WithMetadataReferences(metadataReferences));
            }

            [Test]
            public static void TwoDocumentsNopAnalyzer()
            {
                var code1 = @"
namespace N
{
    class C1
    {
    }
}";
                var code2 = @"
namespace N
{
    class C2
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
    class C1
    {
    }
}";
                var code2 = @"
namespace Project2
{
    class C2
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
        private readonly int f = 1;

        public int M() => this.f;
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
    class C
    {
        private int value;
        
        public int WrongName
        {
            get => this.value;
            set => this.value = value;
        }
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
        private readonly int f = 1;

        public int M() => this.f;
    }
}";

                var descriptor = FieldNameMustNotBeginWithUnderscoreReportsTwo.Descriptor1;
                var analyzer = new FieldNameMustNotBeginWithUnderscoreReportsTwo();
                RoslynAssert.Valid(analyzer, descriptor, code);
                RoslynAssert.Valid(typeof(FieldNameMustNotBeginWithUnderscoreReportsTwo), descriptor, code);
            }

            [Test]
            public static void Suppressed()
            {
                var code = @"
namespace N
{
    public class C
    {
        private readonly string f;
    }
}";

                var analyzer = new NopAnalyzer();
                var settings = Settings.Default.WithCompilationOptions(x => x.WithSuppressed("CS1823", "CS8618", "CS0169"));
                RoslynAssert.Valid(analyzer, code, settings: settings);
                RoslynAssert.Valid(analyzer, analyzer.SupportedDiagnostics[0], code, settings: settings);
            }

            [Test]
            public static void Pragma()
            {
                var code = @"
namespace N
{
    public class C
    {
#pragma warning disable CA1823 // Avoid unused private fields
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
#pragma warning disable CS0169 // Remove unused private members
        private readonly string f;
#pragma warning restore CS0169 // Remove unused private members
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
#pragma warning restore CA1823 // Avoid unused private fields
    }
}";

                var analyzer = new NopAnalyzer();
                RoslynAssert.Valid(analyzer, code);
                RoslynAssert.Valid(analyzer, analyzer.SupportedDiagnostics[0], code);
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
    public class C
    {
    }
}";
                var analyzer = new FieldNameMustNotBeginWithUnderscoreReportsTwo();
                RoslynAssert.Valid(analyzer, resourcesCode, code);
                RoslynAssert.Valid(analyzer, code, resourcesCode);
                RoslynAssert.Valid(analyzer.GetType(), resourcesCode, code);
            }

            [Test]
            public static void AnalyzerWithTwoDiagnostics()
            {
                var code = @"
namespace N
{
    public class C
    {
        private readonly int value = 1;

        public int M() => this.value;
    }
}";
                var analyzer = new FieldAndPropertyMustBeNamedValueAnalyzer();
                RoslynAssert.Valid(analyzer, code);
            }

            [Test]
            public static void BinaryStrings()
            {
                var code = @"
namespace N
{
    public class C : BinaryReferencedAssembly.Base
    {
        private int f = 1;
        
        public int M() => this.f;
    }
}";
                var analyzer = new FieldNameMustNotBeginWithUnderscore();
                var binaryReference = Asserts.MetadataReferences.CreateBinary(@"
namespace BinaryReferencedAssembly
{
    public class Base
    {
    }
}");

                var settings = Settings.Default.WithMetadataReferences(x => x.Append(binaryReference));
                RoslynAssert.Valid(analyzer, code, settings);
            }

            [Test]
            public static void BinarySolution()
            {
                var code = @"
namespace N
{
    public class C : BinaryReferencedAssembly.Base
    {
        private int f = 1;
        
        public int M() => this.f;
    }
}";
                var binaryReference = Asserts.MetadataReferences.CreateBinary(@"
namespace BinaryReferencedAssembly
{
    public class Base
    {
    }
}");

                var analyzer = new FieldNameMustNotBeginWithUnderscore();
                var settings = Settings.Default.WithMetadataReferences(x => x.Append(binaryReference));
                var solution = CodeFactory.CreateSolution(
                    code,
                    settings);

                RoslynAssert.Valid(analyzer, solution);
            }
        }
    }
}
