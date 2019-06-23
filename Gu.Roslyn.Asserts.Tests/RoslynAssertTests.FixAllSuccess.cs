namespace Gu.Roslyn.Asserts.Tests
{
    using System;
    using Gu.Roslyn.Asserts.Tests.CodeFixes;
    using Microsoft.CodeAnalysis;
    using NUnit.Framework;

    [TestFixture]
    public partial class RoslynAssertTests
    {
        public class FixAllSuccess
        {
            [TearDown]
            public void TearDown()
            {
                RoslynAssert.MetadataReferences.Clear();
            }

            [Test]
            public void OneError()
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
                RoslynAssert.MetadataReferences.Add(MetadataReference.CreateFromFile(typeof(int).Assembly.Location));
                RoslynAssert.FixAll<FieldNameMustNotBeginWithUnderscore, DontUseUnderscoreCodeFixProvider>(code, fixedCode);
            }

            [TestCase("Rename to: value1", "value1")]
            [TestCase("Rename to: value2", "value2")]
            public void SingleDocumentOneErrorTwoFixes(string title, string expected)
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
                RoslynAssert.MetadataReferences.Add(MetadataReference.CreateFromFile(typeof(int).Assembly.Location));
                RoslynAssert.FixAll<FieldNameMustNotBeginWithUnderscore, DontUseUnderscoreManyCodeFixProvider>(code, fixedCode, title);
            }

            [Test]
            public void TwoErrors()
            {
                var code = @"
namespace RoslynSandbox
{
    class Foo
    {
        private readonly int ↓_value1;
        private readonly int ↓_value2;
    }
}";

                var fixedCode = @"
namespace RoslynSandbox
{
    class Foo
    {
        private readonly int value1;
        private readonly int value2;
    }
}";
                RoslynAssert.MetadataReferences.Add(MetadataReference.CreateFromFile(typeof(int).Assembly.Location));
                RoslynAssert.FixAll<FieldNameMustNotBeginWithUnderscore, DontUseUnderscoreCodeFixProvider>(code, fixedCode);
            }

            [Test]
            public void FixAllInDocumentTwoErrors()
            {
                var code = @"
namespace RoslynSandbox
{
    class Foo
    {
        private readonly int ↓_value1;
        private readonly int ↓_value2;
    }
}";

                var fixedCode = @"
namespace RoslynSandbox
{
    class Foo
    {
        private readonly int value1;
        private readonly int value2;
    }
}";
                RoslynAssert.MetadataReferences.Add(MetadataReference.CreateFromFile(typeof(int).Assembly.Location));
                RoslynAssert.FixAllInDocument<FieldNameMustNotBeginWithUnderscore, DontUseUnderscoreCodeFixProvider>(code, fixedCode);
            }

            [Test]
            public void FixAllOneByOneTwoErrors()
            {
                var code = @"
namespace RoslynSandbox
{
    class Foo
    {
        private readonly int ↓_value1;
        private readonly int ↓_value2;
    }
}";

                var fixedCode = @"
namespace RoslynSandbox
{
    class Foo
    {
        private readonly int value1;
        private readonly int value2;
    }
}";
                RoslynAssert.MetadataReferences.Add(MetadataReference.CreateFromFile(typeof(int).Assembly.Location));
                RoslynAssert.FixAllOneByOne<FieldNameMustNotBeginWithUnderscore, DontUseUnderscoreCodeFixProvider>(code, fixedCode);
            }

            [Test]
            public void SingleDocumentOneErrorCorrectFixExplicitTitleExpectedDiagnosticWithPositionAnalyzerSupportsTwoDiagnostics1()
            {
                var code = @"
namespace RoslynSandbox
{
    class Foo
    {
        public readonly int ↓_value;
    }
}";

                var fixedCode = @"
namespace RoslynSandbox
{
    class Foo
    {
        public readonly int value;
    }
}";
                RoslynAssert.MetadataReferences.Add(MetadataReference.CreateFromFile(typeof(int).Assembly.Location));
                var expectedDiagnostic = ExpectedDiagnostic.Create(FieldNameMustNotBeginWithUnderscoreDifferentDiagnosticsForPublic.Id1);
                RoslynAssert.FixAll<FieldNameMustNotBeginWithUnderscoreDifferentDiagnosticsForPublic, DontUseUnderscoreCodeFixProvider>(expectedDiagnostic, code, fixedCode);
                RoslynAssert.FixAll<FieldNameMustNotBeginWithUnderscoreDifferentDiagnosticsForPublic, DontUseUnderscoreCodeFixProvider>(expectedDiagnostic, new[] { code }, fixedCode);
                RoslynAssert.FixAll(new FieldNameMustNotBeginWithUnderscoreDifferentDiagnosticsForPublic(), new DontUseUnderscoreCodeFixProvider(), expectedDiagnostic, code, fixedCode);
                RoslynAssert.FixAll(new FieldNameMustNotBeginWithUnderscoreDifferentDiagnosticsForPublic(), new DontUseUnderscoreCodeFixProvider(), expectedDiagnostic, new[] { code }, fixedCode);
                RoslynAssert.FixAll<FieldNameMustNotBeginWithUnderscoreDifferentDiagnosticsForPublic, DontUseUnderscoreCodeFixProvider>(expectedDiagnostic, code, fixedCode, "Rename to: value");
                RoslynAssert.FixAll<FieldNameMustNotBeginWithUnderscoreDifferentDiagnosticsForPublic, DontUseUnderscoreCodeFixProvider>(expectedDiagnostic, new[] { code }, fixedCode, "Rename to: value");
                RoslynAssert.FixAll(new FieldNameMustNotBeginWithUnderscoreDifferentDiagnosticsForPublic(), new DontUseUnderscoreCodeFixProvider(), expectedDiagnostic, code, fixedCode, fixTitle: "Rename to: value");
                RoslynAssert.FixAll(new FieldNameMustNotBeginWithUnderscoreDifferentDiagnosticsForPublic(), new DontUseUnderscoreCodeFixProvider(), expectedDiagnostic, new[] { code }, fixedCode, fixTitle: "Rename to: value");
            }

            [Test]
            public void SingleDocumentOneErrorCorrectFixExplicitTitleExpectedDiagnosticWithPositionAnalyzerSupportsTwoDiagnostics2()
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
                RoslynAssert.MetadataReferences.Add(MetadataReference.CreateFromFile(typeof(int).Assembly.Location));
                var expectedDiagnostic = ExpectedDiagnostic.Create(FieldNameMustNotBeginWithUnderscoreDifferentDiagnosticsForPublic.Id2);
                RoslynAssert.FixAll<FieldNameMustNotBeginWithUnderscoreDifferentDiagnosticsForPublic, DontUseUnderscoreCodeFixProvider>(expectedDiagnostic, code, fixedCode);
                RoslynAssert.FixAll<FieldNameMustNotBeginWithUnderscoreDifferentDiagnosticsForPublic, DontUseUnderscoreCodeFixProvider>(expectedDiagnostic, new[] { code }, fixedCode);
                RoslynAssert.FixAll(new FieldNameMustNotBeginWithUnderscoreDifferentDiagnosticsForPublic(), new DontUseUnderscoreCodeFixProvider(), expectedDiagnostic, code, fixedCode);
                RoslynAssert.FixAll(new FieldNameMustNotBeginWithUnderscoreDifferentDiagnosticsForPublic(), new DontUseUnderscoreCodeFixProvider(), expectedDiagnostic, new[] { code }, fixedCode);
                RoslynAssert.FixAll<FieldNameMustNotBeginWithUnderscoreDifferentDiagnosticsForPublic, DontUseUnderscoreCodeFixProvider>(expectedDiagnostic, code, fixedCode, "Rename to: value");
                RoslynAssert.FixAll<FieldNameMustNotBeginWithUnderscoreDifferentDiagnosticsForPublic, DontUseUnderscoreCodeFixProvider>(expectedDiagnostic, new[] { code }, fixedCode, "Rename to: value");
                RoslynAssert.FixAll(new FieldNameMustNotBeginWithUnderscoreDifferentDiagnosticsForPublic(), new DontUseUnderscoreCodeFixProvider(), expectedDiagnostic, code, fixedCode, fixTitle: "Rename to: value");
                RoslynAssert.FixAll(new FieldNameMustNotBeginWithUnderscoreDifferentDiagnosticsForPublic(), new DontUseUnderscoreCodeFixProvider(), expectedDiagnostic, new[] { code }, fixedCode, fixTitle: "Rename to: value");
            }

            [Test]
            public void TwoDocumentsTwoErrorsTwoFixes()
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
    using System;

    public class Foo2
    {
        public event EventHandler ↓Bar;
    }
}";

                var fixed1 = @"
namespace RoslynSandbox
{
    using System;

    public class Foo1
    {
    }
}";

                var fixed2 = @"
namespace RoslynSandbox
{
    using System;

    public class Foo2
    {
    }
}";
                RoslynAssert.MetadataReferences.Add(MetadataReference.CreateFromFile(typeof(EventHandler).Assembly.Location));
                var expectedDiagnostic = ExpectedDiagnostic.Create("CS0067");
                RoslynAssert.FixAll<RemoveUnusedFixProvider>(expectedDiagnostic, new[] { code1, code2 }, new[] { fixed1, fixed2 });
                RoslynAssert.FixAll<RemoveUnusedFixProvider>(expectedDiagnostic, new[] { code2, code1 }, new[] { fixed2, fixed1 });
            }

            [TestCase("Rename to: value1", "value1")]
            [TestCase("Rename to: value2", "value2")]
            public void TwoDocumentsOneErrorWhenCodeFixProviderHasManyFixes(string title, string expected)
            {
                var code1 = @"
namespace RoslynSandbox
{
    class Foo1
    {
        private readonly int ↓_value;
    }
}";
                var code2 = @"
namespace RoslynSandbox
{
    class Foo2
    {
        private readonly int value;
    }
}";

                var fixedCode = @"
namespace RoslynSandbox
{
    class Foo1
    {
        private readonly int value;
    }
}";

                fixedCode = fixedCode.AssertReplace("value", expected);
                RoslynAssert.MetadataReferences.Add(MetadataReference.CreateFromFile(typeof(int).Assembly.Location));
                var expectedDiagnostic = ExpectedDiagnostic.Create(FieldNameMustNotBeginWithUnderscore.DiagnosticId);
                RoslynAssert.FixAll<FieldNameMustNotBeginWithUnderscore, DontUseUnderscoreManyCodeFixProvider>(new[] { code1, code2 }, new[] { fixedCode, code2 }, title);
                RoslynAssert.FixAll<FieldNameMustNotBeginWithUnderscore, DontUseUnderscoreManyCodeFixProvider>(new[] { code1, code2 }, fixedCode, title);
                RoslynAssert.FixAll<FieldNameMustNotBeginWithUnderscore, DontUseUnderscoreManyCodeFixProvider>(expectedDiagnostic, new[] { code1, code2 }, new[] { fixedCode, code2 }, title);
                RoslynAssert.FixAll<FieldNameMustNotBeginWithUnderscore, DontUseUnderscoreManyCodeFixProvider>(expectedDiagnostic, new[] { code1, code2 }, fixedCode, title);
            }

            [TestCase("Rename to: value1", "value1")]
            [TestCase("Rename to: value2", "value2")]
            public void TwoDocumentsOneFixCorrectFixPassOnlyFixedCode(string title, string expected)
            {
                var code1 = @"
namespace RoslynSandbox
{
    class Foo1
    {
        private readonly int ↓_value;
    }
}";
                var code2 = @"
namespace RoslynSandbox
{
    class Foo2
    {
        private readonly int value;
    }
}";

                var fixedCode = @"
namespace RoslynSandbox
{
    class Foo1
    {
        private readonly int value;
    }
}";

                fixedCode = fixedCode.AssertReplace("value", expected);
                RoslynAssert.MetadataReferences.Add(MetadataReference.CreateFromFile(typeof(int).Assembly.Location));
                RoslynAssert.FixAll<FieldNameMustNotBeginWithUnderscore, DontUseUnderscoreManyCodeFixProvider>(new[] { code1, code2 }, fixedCode, title);
            }

            [TestCase("Rename to: value1", "value1")]
            [TestCase("Rename to: value2", "value2")]
            public void TwoDocumentsTwoFixes(string title, string expected)
            {
                var code1 = @"
namespace RoslynSandbox
{
    class Foo1
    {
        private readonly int ↓_value;
    }
}";
                var code2 = @"
namespace RoslynSandbox
{
    class Foo2
    {
        private readonly int ↓_value;
    }
}";

                var fixedCode1 = @"
namespace RoslynSandbox
{
    class Foo1
    {
        private readonly int value;
    }
}";

                var fixedCode2 = @"
namespace RoslynSandbox
{
    class Foo2
    {
        private readonly int value;
    }
}";
                fixedCode1 = fixedCode1.AssertReplace("value", expected);
                fixedCode2 = fixedCode2.AssertReplace("value", expected);
                RoslynAssert.MetadataReferences.Add(MetadataReference.CreateFromFile(typeof(int).Assembly.Location));
                RoslynAssert.FixAll<FieldNameMustNotBeginWithUnderscore, DontUseUnderscoreManyCodeFixProvider>(new[] { code1, code2 }, new[] { fixedCode1, fixedCode2 }, title);
            }

            [Test]
            public void TwoDocumentsDifferentProjectsCodeFixOnlyOneFix()
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
                RoslynAssert.MetadataReferences.Add(MetadataReference.CreateFromFile(typeof(EventHandler).Assembly.Location));
                var expectedDiagnostic = ExpectedDiagnostic.CreateFromCodeWithErrorsIndicated("CS0067", code1, out code1);
                RoslynAssert.FixAll<RemoveUnusedFixProvider>(expectedDiagnostic, new[] { code1, code2 }, new[] { fixedCode, code2 });
                RoslynAssert.FixAll<RemoveUnusedFixProvider>(expectedDiagnostic, new[] { code2, code1 }, new[] { code2, fixedCode });
            }

            [Test]
            public void TwoDocumentsDifferentProjectsCodeFixOnlyOneFixPassingOnlyFixedCode()
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
                RoslynAssert.MetadataReferences.Add(MetadataReference.CreateFromFile(typeof(EventHandler).Assembly.Location));
                var expectedDiagnostic = ExpectedDiagnostic.CreateFromCodeWithErrorsIndicated("CS0067", code1, out code1);
                RoslynAssert.FixAll<RemoveUnusedFixProvider>(expectedDiagnostic, new[] { code1, code2 }, fixedCode);
                RoslynAssert.FixAll<RemoveUnusedFixProvider>(expectedDiagnostic, new[] { code2, code1 }, fixedCode);
                RoslynAssert.FixAll(new RemoveUnusedFixProvider(), expectedDiagnostic, new[] { code2, code1 }, fixedCode);
            }

            [Test]
            public void TwoDocumentsDifferentProjectsCodeFixOnly()
            {
                var code1 = @"
namespace RoslynSandbox.Core
{
    using System;

    public class FooCore
    {
        public event EventHandler ↓Bar;
    }
}";

                var code2 = @"
namespace RoslynSandbox.Client
{
    public class FooClient
    {
    }
}";

                var fixedCode = @"
namespace RoslynSandbox.Core
{
    using System;

    public class FooCore
    {
    }
}";
                RoslynAssert.MetadataReferences.Add(MetadataReference.CreateFromFile(typeof(EventHandler).Assembly.Location));
                var expectedDiagnostic = ExpectedDiagnostic.CreateFromCodeWithErrorsIndicated("CS0067", code1, out code1);
                RoslynAssert.FixAll<RemoveUnusedFixProvider>(expectedDiagnostic, new[] { code1, code2 }, fixedCode);
                RoslynAssert.FixAll<RemoveUnusedFixProvider>(expectedDiagnostic, new[] { code2, code1 }, fixedCode);
                RoslynAssert.FixAll(new RemoveUnusedFixProvider(), expectedDiagnostic, new[] { code2, code1 }, fixedCode);
            }

            [Test]
            public void TwoDocumentsDifferentProjectsInheritingCodeFixOnly()
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
                RoslynAssert.MetadataReferences.Add(MetadataReference.CreateFromFile(typeof(EventHandler).Assembly.Location));
                var expectedDiagnostic = ExpectedDiagnostic.CreateFromCodeWithErrorsIndicated("CS0067", code1, out code1);
                RoslynAssert.FixAll<RemoveUnusedFixProvider>(expectedDiagnostic, new[] { code1, code2 }, fixedCode);
                RoslynAssert.FixAll<RemoveUnusedFixProvider>(expectedDiagnostic, new[] { code2, code1 }, fixedCode);
                RoslynAssert.FixAll(new RemoveUnusedFixProvider(), expectedDiagnostic, new[] { code2, code1 }, fixedCode);
            }

            [Test]
            public void TwoDocumentsDifferentProjectsInheritingCodeFixOnlyCorrectFix2()
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
                RoslynAssert.MetadataReferences.Add(MetadataReference.CreateFromFile(typeof(EventHandler).Assembly.Location));
                var expectedDiagnostic = ExpectedDiagnostic.CreateFromCodeWithErrorsIndicated("CS0067", code2, out code2);
                RoslynAssert.FixAll<RemoveUnusedFixProvider>(expectedDiagnostic, new[] { code1, code2 }, fixedCode);
                RoslynAssert.FixAll<RemoveUnusedFixProvider>(expectedDiagnostic, new[] { code2, code1 }, fixedCode);
                RoslynAssert.FixAll(new RemoveUnusedFixProvider(), expectedDiagnostic, new[] { code2, code1 }, fixedCode);
            }

            [Test]
            public void SingleDocumentCodeFixOnly()
            {
                var code = @"
namespace RoslynSandbox
{
    using System;

    public class Foo
    {
        public event EventHandler ↓Bar;
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
                RoslynAssert.MetadataReferences.Add(MetadataReference.CreateFromFile(typeof(EventHandler).Assembly.Location));
                var expectedDiagnostic = ExpectedDiagnostic.CreateFromCodeWithErrorsIndicated("CS0067", code, out code);
                RoslynAssert.FixAll<RemoveUnusedFixProvider>(expectedDiagnostic, code, fixedCode);
                RoslynAssert.FixAll(new RemoveUnusedFixProvider(), expectedDiagnostic, code, fixedCode);
            }

            [Test]
            public void TwoClassOneError()
            {
                var barCode = @"
namespace RoslynSandbox
{
    class Bar
    {
        private readonly int value;
    }
}";

                var testCode = @"
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
                RoslynAssert.MetadataReferences.Add(MetadataReference.CreateFromFile(typeof(int).Assembly.Location));
                var expectedDiagnostic = ExpectedDiagnostic.Create(FieldNameMustNotBeginWithUnderscore.DiagnosticId);
                RoslynAssert.FixAll<FieldNameMustNotBeginWithUnderscore, DontUseUnderscoreCodeFixProvider>(new[] { barCode, testCode }, new[] { barCode, fixedCode });
                RoslynAssert.FixAll<FieldNameMustNotBeginWithUnderscore, DontUseUnderscoreCodeFixProvider>(new[] { barCode, testCode }, fixedCode);
                RoslynAssert.FixAll(new FieldNameMustNotBeginWithUnderscore(), new DontUseUnderscoreCodeFixProvider(), new[] { barCode, testCode }, fixedCode);
                RoslynAssert.FixAll(new FieldNameMustNotBeginWithUnderscore(), new DontUseUnderscoreCodeFixProvider(), new[] { barCode, testCode }, new[] { barCode, fixedCode });
                RoslynAssert.FixAll(new FieldNameMustNotBeginWithUnderscore(), new DontUseUnderscoreCodeFixProvider(), expectedDiagnostic, new[] { barCode, testCode }, fixedCode);
                RoslynAssert.FixAll(new FieldNameMustNotBeginWithUnderscore(), new DontUseUnderscoreCodeFixProvider(), expectedDiagnostic, new[] { barCode, testCode }, new[] { barCode, fixedCode });
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
                RoslynAssert.FixAll<ClassMustHaveEventAnalyzer, InsertEventFixProvider>(code, fixedCode, allowCompilationErrors: AllowCompilationErrors.Yes);
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
                RoslynAssert.MetadataReferences.Add(MetadataReference.CreateFromFile(typeof(int).Assembly.Location));
                var expectedDiagnostic = ExpectedDiagnostic.Create(FieldAndPropertyMustBeNamedFooAnalyzer.FieldDiagnosticId);
                RoslynAssert.FixAll<FieldAndPropertyMustBeNamedFooAnalyzer, RenameToFooCodeFixProvider>(expectedDiagnostic, code, fixedCode);
                RoslynAssert.FixAll(new FieldAndPropertyMustBeNamedFooAnalyzer(), new RenameToFooCodeFixProvider(), expectedDiagnostic, code, fixedCode);
            }
        }
    }
}
