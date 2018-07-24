// ReSharper disable RedundantNameQualifier
namespace Gu.Roslyn.Asserts.Tests
{
    using System;
    using System.IO;
    using Gu.Roslyn.Asserts.Tests.CodeFixes;
    using Microsoft.CodeAnalysis;
    using NUnit.Framework;

    [TestFixture]
    public partial class AnalyzerAssertCodeFixTests
    {
        public class Success
        {
            [TearDown]
            public void TearDown()
            {
                AnalyzerAssert.ResetAll();
            }

            [Test]
            public void SingleClassOneErrorCorrectFix()
            {
                var code = @"
namespace RoslynSandbox
{
    class Foo
    {
        private readonly int ↓_value;
    }
}";

                var fixedCode = @"
namespace RoslynSandbox
{
    class Foo
    {
        private readonly int value;
    }
}";
                AnalyzerAssert.MetadataReferences.Add(MetadataReference.CreateFromFile(typeof(int).Assembly.Location));
                AnalyzerAssert.CodeFix<FieldNameMustNotBeginWithUnderscore, DontUseUnderscoreCodeFixProvider>(code, fixedCode);
                AnalyzerAssert.CodeFix<FieldNameMustNotBeginWithUnderscore, DontUseUnderscoreCodeFixProvider>(new[] { code }, fixedCode);
                AnalyzerAssert.CodeFix(new FieldNameMustNotBeginWithUnderscore(), new DontUseUnderscoreCodeFixProvider(), new[] { code }, fixedCode);
            }

            [Test]
            public void MakeSealedCorrectFix()
            {
                var code = @"
namespace RoslynSandbox
{
    ↓public class Foo
    {
    }
}";

                var fixedCode = @"
namespace RoslynSandbox
{
    public sealed class Foo
    {
    }
}";
                AnalyzerAssert.MetadataReferences.Add(MetadataReference.CreateFromFile(typeof(int).Assembly.Location));
                AnalyzerAssert.CodeFix<ClassMustBeSealedAnalyzer, MakeSealedFixProvider>(code, fixedCode);
                AnalyzerAssert.CodeFix<ClassMustBeSealedAnalyzer, MakeSealedFixProvider>(new[] { code }, fixedCode);
                AnalyzerAssert.CodeFix(new ClassMustBeSealedAnalyzer(), new MakeSealedFixProvider(), new[] { code }, fixedCode);
            }

            [Test]
            public void MakeSealedCorrectFixKeepsPoorFormat()
            {
                var code = @"
namespace    RoslynSandbox
{
    ↓public class Foo
    {
private readonly int value;
                
            public Foo(int value)
{
        this.value  =  value;
        }
    }
}";

                var fixedCode = @"
namespace    RoslynSandbox
{
    public sealed class Foo
    {
private readonly int value;
                
            public Foo(int value)
{
        this.value  =  value;
        }
    }
}";
                AnalyzerAssert.MetadataReferences.Add(MetadataReference.CreateFromFile(typeof(int).Assembly.Location));
                AnalyzerAssert.CodeFix<ClassMustBeSealedAnalyzer, MakeSealedFixProvider>(code, fixedCode);
            }

            [Test]
            public void SingleClassOneErrorCorrectFixExplicitTitle()
            {
                var code = @"
namespace RoslynSandbox
{
    class Foo
    {
        private readonly int ↓_value;
    }
}";

                var fixedCode = @"
namespace RoslynSandbox
{
    class Foo
    {
        private readonly int value;
    }
}";
                AnalyzerAssert.MetadataReferences.Add(MetadataReference.CreateFromFile(typeof(int).Assembly.Location));
                AnalyzerAssert.CodeFix<FieldNameMustNotBeginWithUnderscore, DontUseUnderscoreCodeFixProvider>(code, fixedCode, "Rename to: value");
            }

            [Test]
            public void SingleClassOneErrorCorrectFixExplicitTitleExpectedDiagnosticWithErrorsIndicated()
            {
                var code = @"
namespace RoslynSandbox
{
    class Foo
    {
        private readonly int ↓_value;
    }
}";

                var fixedCode = @"
namespace RoslynSandbox
{
    class Foo
    {
        private readonly int value;
    }
}";
                AnalyzerAssert.MetadataReferences.Add(MetadataReference.CreateFromFile(typeof(int).Assembly.Location));
                var expectedDiagnostic = ExpectedDiagnostic.Create(FieldNameMustNotBeginWithUnderscore.DiagnosticId);
                AnalyzerAssert.CodeFix<FieldNameMustNotBeginWithUnderscore, DontUseUnderscoreCodeFixProvider>(expectedDiagnostic, code, fixedCode, "Rename to: value");
                AnalyzerAssert.CodeFix<FieldNameMustNotBeginWithUnderscore, DontUseUnderscoreCodeFixProvider>(expectedDiagnostic, new[] { code }, fixedCode, "Rename to: value");
                AnalyzerAssert.CodeFix(new FieldNameMustNotBeginWithUnderscore(), new DontUseUnderscoreCodeFixProvider(), expectedDiagnostic, code, fixedCode, "Rename to: value");
                AnalyzerAssert.CodeFix(new FieldNameMustNotBeginWithUnderscore(), new DontUseUnderscoreCodeFixProvider(), expectedDiagnostic, new[] { code }, fixedCode, "Rename to: value");
            }

            [Test]
            public void SingleClassOneErrorCorrectFixExplicitTitleExpectedDiagnosticWithPosition()
            {
                var code = @"
namespace RoslynSandbox
{
    class Foo
    {
        private readonly int ↓_value;
    }
}";

                var fixedCode = @"
namespace RoslynSandbox
{
    class Foo
    {
        private readonly int value;
    }
}";
                AnalyzerAssert.MetadataReferences.Add(MetadataReference.CreateFromFile(typeof(int).Assembly.Location));
                var expectedDiagnostic = ExpectedDiagnostic.CreateFromCodeWithErrorsIndicated(FieldNameMustNotBeginWithUnderscore.DiagnosticId, code, out code);
                AnalyzerAssert.CodeFix<FieldNameMustNotBeginWithUnderscore, DontUseUnderscoreCodeFixProvider>(expectedDiagnostic, code, fixedCode, "Rename to: value");
                AnalyzerAssert.CodeFix<FieldNameMustNotBeginWithUnderscore, DontUseUnderscoreCodeFixProvider>(expectedDiagnostic, new[] { code }, fixedCode, "Rename to: value");
                AnalyzerAssert.CodeFix(new FieldNameMustNotBeginWithUnderscore(), new DontUseUnderscoreCodeFixProvider(), expectedDiagnostic, code, fixedCode, "Rename to: value");
                AnalyzerAssert.CodeFix(new FieldNameMustNotBeginWithUnderscore(), new DontUseUnderscoreCodeFixProvider(), expectedDiagnostic, new[] { code }, fixedCode, "Rename to: value");
            }

            [Test]
            public void SingleClassOneErrorCorrectFixExplicitTitleExpectedDiagnosticWithPositionAnalyserSupportsTwoDiagnostics()
            {
                var code = @"
namespace RoslynSandbox
{
    class Foo
    {
        private readonly int ↓_value;
    }
}";

                var fixedCode = @"
namespace RoslynSandbox
{
    class Foo
    {
        private readonly int value;
    }
}";
                AnalyzerAssert.MetadataReferences.Add(MetadataReference.CreateFromFile(typeof(int).Assembly.Location));
                var expectedDiagnostic = ExpectedDiagnostic.Create(FieldNameMustNotBeginWithUnderscoreDifferentDiagnosticsForPublic.Id2);
                AnalyzerAssert.CodeFix<FieldNameMustNotBeginWithUnderscoreDifferentDiagnosticsForPublic, DontUseUnderscoreCodeFixProvider>(expectedDiagnostic, code, fixedCode, "Rename to: value");
                AnalyzerAssert.CodeFix<FieldNameMustNotBeginWithUnderscoreDifferentDiagnosticsForPublic, DontUseUnderscoreCodeFixProvider>(expectedDiagnostic, new[] { code }, fixedCode, "Rename to: value");
                AnalyzerAssert.CodeFix(new FieldNameMustNotBeginWithUnderscoreDifferentDiagnosticsForPublic(), new DontUseUnderscoreCodeFixProvider(), expectedDiagnostic, code, fixedCode, "Rename to: value");
                AnalyzerAssert.CodeFix(new FieldNameMustNotBeginWithUnderscoreDifferentDiagnosticsForPublic(), new DontUseUnderscoreCodeFixProvider(), expectedDiagnostic, new[] { code }, fixedCode, "Rename to: value");
            }

            [TestCase("Rename to: value1", "value1")]
            [TestCase("Rename to: value2", "value2")]
            public void SingleClassOneErrorTwoFixesCorrectFix(string title, string expected)
            {
                var code = @"
namespace RoslynSandbox
{
    class Foo
    {
        private readonly int ↓_value;
    }
}";

                var fixedCode = @"
namespace RoslynSandbox
{
    class Foo
    {
        private readonly int value;
    }
}";
                fixedCode = fixedCode.AssertReplace("value", expected);
                AnalyzerAssert.MetadataReferences.Add(MetadataReference.CreateFromFile(typeof(int).Assembly.Location));
                AnalyzerAssert.CodeFix<FieldNameMustNotBeginWithUnderscore, DontUseUnderscoreManyCodeFixProvider>(code, fixedCode, title);
                AnalyzerAssert.CodeFix<FieldNameMustNotBeginWithUnderscore, DontUseUnderscoreManyCodeFixProvider>(new[] { code }, fixedCode, title);
                AnalyzerAssert.CodeFix(new FieldNameMustNotBeginWithUnderscore(), new DontUseUnderscoreManyCodeFixProvider(), code, fixedCode, title);
                AnalyzerAssert.CodeFix(new FieldNameMustNotBeginWithUnderscore(), new DontUseUnderscoreManyCodeFixProvider(), new[] { code }, fixedCode, title);
            }

            [Test]
            public void SingleClassCodeFixOnlyCorrectFix()
            {
                var code = @"
namespace RoslynSandbox
{
    using System;

    public class Foo
    {
        public event EventHandler Bar;
    }
}";

                var fixedCode = @"
namespace RoslynSandbox
{
    using System;

    public class Foo
    {
    }
}";
                AnalyzerAssert.MetadataReferences.Add(MetadataReference.CreateFromFile(typeof(EventHandler).Assembly.Location));
                var expectedDiagnostic = ExpectedDiagnostic.Create("CS0067");
                AnalyzerAssert.CodeFix<RemoveUnusedFixProvider>(expectedDiagnostic, code, fixedCode);
                AnalyzerAssert.CodeFix<RemoveUnusedFixProvider>(expectedDiagnostic, new[] { code }, fixedCode);
                AnalyzerAssert.CodeFix(new RemoveUnusedFixProvider(), expectedDiagnostic, code, fixedCode);
                AnalyzerAssert.CodeFix(new RemoveUnusedFixProvider(), expectedDiagnostic, new[] { code }, fixedCode);
            }

            [Test]
            public void TwoClassesCodeFixOnlyCorrectFix()
            {
                var code1 = @"
namespace RoslynSandbox
{
    using System;

    public class Foo1
    {
        public event EventHandler ↓Bar;
    }
}";

                var code2 = @"
namespace RoslynSandbox
{
    public class Foo2
    {
    }
}";

                var fixedCode = @"
namespace RoslynSandbox
{
    using System;

    public class Foo1
    {
    }
}";
                AnalyzerAssert.MetadataReferences.Add(MetadataReference.CreateFromFile(typeof(EventHandler).Assembly.Location));
                var expectedDiagnostic = ExpectedDiagnostic.CreateFromCodeWithErrorsIndicated("CS0067", code1, out code1);
                AnalyzerAssert.CodeFix<RemoveUnusedFixProvider>(expectedDiagnostic, new[] { code1, code2 }, fixedCode);
                AnalyzerAssert.CodeFix<RemoveUnusedFixProvider>(expectedDiagnostic, new[] { code2, code1 }, fixedCode);
                AnalyzerAssert.CodeFix(new RemoveUnusedFixProvider(), expectedDiagnostic, new[] { code2, code1 }, fixedCode);
                AnalyzerAssert.CodeFix(new RemoveUnusedFixProvider(), expectedDiagnostic, new[] { code1, code2 }, fixedCode);
            }

            [Test]
            public void TwoClassesDifferentProjectsCodeFixOnlyCorrectFix()
            {
                var code1 = @"
namespace RoslynSandbox.Core
{
    using System;

    public class Foo1
    {
        public event EventHandler ↓Bar;
    }
}";

                var code2 = @"
namespace RoslynSandbox.Client
{
    public class Foo2
    {
    }
}";

                var fixedCode = @"
namespace RoslynSandbox.Core
{
    using System;

    public class Foo1
    {
    }
}";
                AnalyzerAssert.MetadataReferences.Add(MetadataReference.CreateFromFile(typeof(EventHandler).Assembly.Location));
                var expectedDiagnostic = ExpectedDiagnostic.CreateFromCodeWithErrorsIndicated("CS0067", code1, out code1);
                AnalyzerAssert.CodeFix<RemoveUnusedFixProvider>(expectedDiagnostic, new[] { code1, code2 }, fixedCode);
                AnalyzerAssert.CodeFix<RemoveUnusedFixProvider>(expectedDiagnostic, new[] { code2, code1 }, fixedCode);
                AnalyzerAssert.CodeFix(new RemoveUnusedFixProvider(), expectedDiagnostic, new[] { code2, code1 }, fixedCode);
            }

            [Test]
            public void TwoClassesDifferentProjectsInheritingCodeFixOnlyCorrectFix()
            {
                var code1 = @"
namespace RoslynSandbox.Core
{
    using System;

    public class Foo1
    {
        public event EventHandler ↓Bar;
    }
}";

                var code2 = @"
namespace RoslynSandbox.Client
{
    public class Foo2 : RoslynSandbox.Core.Foo1
    {
    }
}";

                var fixedCode = @"
namespace RoslynSandbox.Core
{
    using System;

    public class Foo1
    {
    }
}";
                AnalyzerAssert.MetadataReferences.Add(MetadataReference.CreateFromFile(typeof(EventHandler).Assembly.Location));
                var expectedDiagnostic = ExpectedDiagnostic.CreateFromCodeWithErrorsIndicated("CS0067", code1, out code1);
                AnalyzerAssert.CodeFix<RemoveUnusedFixProvider>(expectedDiagnostic, new[] { code1, code2 }, fixedCode);
                AnalyzerAssert.CodeFix<RemoveUnusedFixProvider>(expectedDiagnostic, new[] { code2, code1 }, fixedCode);
                AnalyzerAssert.CodeFix(new RemoveUnusedFixProvider(), expectedDiagnostic, new[] { code2, code1 }, fixedCode);
            }

            [Test]
            public void TwoClassesDifferentProjectsInheritingCodeFixOnlyCorrectFix2()
            {
                var code1 = @"
namespace RoslynSandbox.Core
{
    public class Foo1
    {
    }
}";

                var code2 = @"
namespace RoslynSandbox.Client
{
    using System;

    public class Foo2 : RoslynSandbox.Core.Foo1
    {
        public event EventHandler ↓Bar;
    }
}";

                var fixedCode = @"
namespace RoslynSandbox.Client
{
    using System;

    public class Foo2 : RoslynSandbox.Core.Foo1
    {
    }
}";
                AnalyzerAssert.MetadataReferences.Add(MetadataReference.CreateFromFile(typeof(EventHandler).Assembly.Location));
                var expectedDiagnostic = ExpectedDiagnostic.Create("CS0067");
                AnalyzerAssert.CodeFix<RemoveUnusedFixProvider>(expectedDiagnostic, new[] { code1, code2 }, fixedCode);
                AnalyzerAssert.CodeFix<RemoveUnusedFixProvider>(expectedDiagnostic, new[] { code2, code1 }, fixedCode);
                AnalyzerAssert.CodeFix(new RemoveUnusedFixProvider(), expectedDiagnostic, new[] { code2, code1 }, fixedCode);
            }

            [Test]
            public void TwoClassesOneErrorCorrectFix()
            {
                var barCode = @"
namespace RoslynSandbox
{
    class Bar
    {
        private readonly int value;
    }
}";

                var code = @"
namespace RoslynSandbox
{
    class Foo
    {
        private readonly int ↓_value;
    }
}";

                var fixedCode = @"
namespace RoslynSandbox
{
    class Foo
    {
        private readonly int value;
    }
}";
                AnalyzerAssert.MetadataReferences.Add(MetadataReference.CreateFromFile(typeof(int).Assembly.Location));
                AnalyzerAssert.CodeFix<FieldNameMustNotBeginWithUnderscore, DontUseUnderscoreCodeFixProvider>(new[] { barCode, code }, fixedCode);
            }

            [Test]
            public void WhenFixIntroducesCompilerErrorsThatAreAccepted()
            {
                var code = @"
namespace RoslynSandbox
{
    ↓class Foo
    {
    }
}";

                var fixedCode = @"
namespace RoslynSandbox
{
    class Foo
    {
        public event EventHandler SomeEvent;
    }
}";
                AnalyzerAssert.CodeFix<ClassMustHaveEventAnalyzer, InsertEventFixProvider>(code, fixedCode, null, AllowCompilationErrors.Yes);
            }

            [Test]
            public void WithExpectedDiagnosticWhenOneReportsError()
            {
                var code = @"
namespace RoslynSandbox
{
    class Foo
    {
        private readonly int ↓wrongName;
        
        public int WrongName { get; set; }
    }
}";

                var fixedCode = @"
namespace RoslynSandbox
{
    class Foo
    {
        private readonly int foo;
        
        public int WrongName { get; set; }
    }
}";
                AnalyzerAssert.MetadataReferences.Add(MetadataReference.CreateFromFile(typeof(int).Assembly.Location));
                var expectedDiagnostic = ExpectedDiagnostic.Create(FieldAndPropertyMustBeNamedFooAnalyzer.FieldDiagnosticId);
                AnalyzerAssert.CodeFix<FieldAndPropertyMustBeNamedFooAnalyzer, RenameToFooCodeFixProvider>(expectedDiagnostic, code, fixedCode);
                AnalyzerAssert.CodeFix(new FieldAndPropertyMustBeNamedFooAnalyzer(), new RenameToFooCodeFixProvider(), expectedDiagnostic, code, fixedCode);
            }

            [Test]
            public void ProjectFromDisk()
            {
                var fixedCode = @"// ReSharper disable InconsistentNaming
// ReSharper disable ArrangeThisQualifier
// ReSharper disable NotAccessedField.Local
#pragma warning disable IDE0009 // Member access should be qualified.
#pragma warning disable IDE0044 // Add readonly modifier
namespace ClassLibrary1
{
    public class ClassLibrary1Class1
    {
        private int value;

        public ClassLibrary1Class1(int value)
        {
            this.value = value;
        }
    }
}
";
                var csproj = ProjectFile.Find("ClassLibrary1.csproj");
                var analyzer = new FieldNameMustNotBeginWithUnderscore();
                var expectedDiagnostic = ExpectedDiagnostic.Create(
                    FieldNameMustNotBeginWithUnderscore.DiagnosticId,
                    "Field '_value' must not begin with an underscore",
                    Path.Combine(csproj.DirectoryName, "ClassLibrary1Class1.cs"),
                    9,
                    20);
                var sln = CodeFactory.CreateSolution(
                    csproj,
                    analyzer,
                    expectedDiagnostic,
                    metadataReferences: new[] { MetadataReference.CreateFromFile(typeof(int).Assembly.Location) });
                AnalyzerAssert.CodeFix(analyzer, new DontUseUnderscoreCodeFixProvider(), expectedDiagnostic, sln, fixedCode);
            }
        }
    }
}
