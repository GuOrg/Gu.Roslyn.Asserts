namespace Gu.Roslyn.Asserts.Tests
{
    using System;
    using Gu.Roslyn.Asserts.Tests.CodeFixes;
    using Microsoft.CodeAnalysis;
    using NUnit.Framework;

    [TestFixture]
    public partial class RoslynAssertTests
    {
        public static class FixAllSuccess
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
            public static void OneError()
            {
                var before = @"
namespace RoslynSandbox
{
    class Foo
    {
        private readonly int ↓_value;
    }
}";

                var after = @"
namespace RoslynSandbox
{
    class Foo
    {
        private readonly int value;
    }
}";
                var analyzer = new FieldNameMustNotBeginWithUnderscore();
                var fix = new DontUseUnderscoreCodeFixProvider();
                RoslynAssert.FixAll(analyzer, fix, before, after);
            }

            [TestCase("Rename to: value1", "value1")]
            [TestCase("Rename to: value2", "value2")]
            public static void SingleDocumentOneErrorTwoFixes(string title, string expected)
            {
                var before = @"
namespace RoslynSandbox
{
    class Foo
    {
        private readonly int ↓_value;
    }
}";

                var after = @"
namespace RoslynSandbox
{
    class Foo
    {
        private readonly int value;
    }
}".AssertReplace("value", expected);

                var analyzer = new FieldNameMustNotBeginWithUnderscore();
                var fix = new DontUseUnderscoreManyCodeFixProvider();
                RoslynAssert.FixAll(analyzer, fix, before, after, title);
            }

            [Test]
            public static void TwoErrors()
            {
                var before = @"
namespace RoslynSandbox
{
    class Foo
    {
        private readonly int ↓_value1;
        private readonly int ↓_value2;
    }
}";

                var after = @"
namespace RoslynSandbox
{
    class Foo
    {
        private readonly int value1;
        private readonly int value2;
    }
}";
                var analyzer = new FieldNameMustNotBeginWithUnderscore();
                var fix = new DontUseUnderscoreCodeFixProvider();
                RoslynAssert.FixAll(analyzer, fix, before, after);
            }

            [Test]
            public static void FixAllInDocumentTwoErrors()
            {
                var before = @"
namespace RoslynSandbox
{
    class Foo
    {
        private readonly int ↓_value1;
        private readonly int ↓_value2;
    }
}";

                var after = @"
namespace RoslynSandbox
{
    class Foo
    {
        private readonly int value1;
        private readonly int value2;
    }
}";
                var analyzer = new FieldNameMustNotBeginWithUnderscore();
                var fix = new DontUseUnderscoreCodeFixProvider();
                RoslynAssert.FixAllInDocument(analyzer, fix, before, after);
            }

            [Test]
            public static void FixAllOneByOneTwoErrors()
            {
                var before = @"
namespace RoslynSandbox
{
    class Foo
    {
        private readonly int ↓_value1;
        private readonly int ↓_value2;
    }
}";

                var after = @"
namespace RoslynSandbox
{
    class Foo
    {
        private readonly int value1;
        private readonly int value2;
    }
}";
                var analyzer = new FieldNameMustNotBeginWithUnderscore();
                var fix = new DontUseUnderscoreCodeFixProvider();
                RoslynAssert.FixAllOneByOne(analyzer, fix, before, after);
            }

            [Test]
            public static void SingleDocumentOneErrorCorrectFixExplicitTitleExpectedDiagnosticWithPositionAnalyzerSupportsTwoDiagnostics1()
            {
                var before = @"
namespace RoslynSandbox
{
    class Foo
    {
        public readonly int ↓_value;
    }
}";

                var after = @"
namespace RoslynSandbox
{
    class Foo
    {
        public readonly int value;
    }
}";
                var expectedDiagnostic = ExpectedDiagnostic.Create(FieldNameMustNotBeginWithUnderscoreDifferentDiagnosticsForPublic.Id1);
                var analyzer = new FieldNameMustNotBeginWithUnderscoreDifferentDiagnosticsForPublic();
                var fix = new DontUseUnderscoreCodeFixProvider();
                RoslynAssert.FixAll(analyzer, fix, expectedDiagnostic, before, after);
                RoslynAssert.FixAll(analyzer, fix, expectedDiagnostic, new[] { before }, after);
                RoslynAssert.FixAll(analyzer, fix, expectedDiagnostic, before, after, fixTitle: "Rename to: value");
                RoslynAssert.FixAll(analyzer, fix, expectedDiagnostic, new[] { before }, after, fixTitle: "Rename to: value");
            }

            [Test]
            public static void SingleDocumentOneErrorCorrectFixExplicitTitleExpectedDiagnosticWithPositionAnalyzerSupportsTwoDiagnostics2()
            {
                var before = @"
namespace RoslynSandbox
{
    class Foo
    {
        private readonly int ↓_value;
    }
}";

                var after = @"
namespace RoslynSandbox
{
    class Foo
    {
        private readonly int value;
    }
}";
                var expectedDiagnostic = ExpectedDiagnostic.Create(FieldNameMustNotBeginWithUnderscoreDifferentDiagnosticsForPublic.Id2);
                var analyzer = new FieldNameMustNotBeginWithUnderscoreDifferentDiagnosticsForPublic();
                var fix = new DontUseUnderscoreCodeFixProvider();
                RoslynAssert.FixAll(analyzer, fix, expectedDiagnostic, before, after);
                RoslynAssert.FixAll(analyzer, fix, expectedDiagnostic, new[] { before }, after);
                RoslynAssert.FixAll(analyzer, fix, expectedDiagnostic, before, after, fixTitle: "Rename to: value");
                RoslynAssert.FixAll(analyzer, fix, expectedDiagnostic, new[] { before }, after, fixTitle: "Rename to: value");
            }

            [Test]
            public static void TwoDocumentsTwoErrorsTwoFixes()
            {
                var before1 = @"
namespace RoslynSandbox
{
    using System;

    public class Foo1
    {
        public event EventHandler ↓Bar;
    }
}";

                var before2 = @"
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
                var fix = new RemoveUnusedFixProvider();
                RoslynAssert.FixAll(fix, expectedDiagnostic, new[] { before1, before2 }, new[] { fixed1, fixed2 });
                RoslynAssert.FixAll(fix, expectedDiagnostic, new[] { before2, before1 }, new[] { fixed2, fixed1 });
            }

            [TestCase("Rename to: value1", "value1")]
            [TestCase("Rename to: value2", "value2")]
            public static void TwoDocumentsOneErrorWhenCodeFixProviderHasManyFixes(string title, string expected)
            {
                var before1 = @"
namespace RoslynSandbox
{
    class Foo1
    {
        private readonly int ↓_value;
    }
}";
                var before2 = @"
namespace RoslynSandbox
{
    class Foo2
    {
        private readonly int value;
    }
}";

                var after = @"
namespace RoslynSandbox
{
    class Foo1
    {
        private readonly int value;
    }
}";

                after = after.AssertReplace("value", expected);
                var expectedDiagnostic = ExpectedDiagnostic.Create(FieldNameMustNotBeginWithUnderscore.DiagnosticId);
                var analyzer = new FieldNameMustNotBeginWithUnderscore();
                var fix = new DontUseUnderscoreManyCodeFixProvider();
                RoslynAssert.FixAll(analyzer, fix, new[] { before1, before2 }, new[] { after, before2 }, title);
                RoslynAssert.FixAll(analyzer, fix, new[] { before1, before2 }, after, title);
                RoslynAssert.FixAll(analyzer, fix, expectedDiagnostic, new[] { before1, before2 }, new[] { after, before2 }, title);
                RoslynAssert.FixAll(analyzer, fix, expectedDiagnostic, new[] { before1, before2 }, after, title);
            }

            [TestCase("Rename to: value1", "value1")]
            [TestCase("Rename to: value2", "value2")]
            public static void TwoDocumentsOneFixCorrectFixPassOnlyFixedCode(string title, string expected)
            {
                var before1 = @"
namespace RoslynSandbox
{
    class Foo1
    {
        private readonly int ↓_value;
    }
}";
                var before2 = @"
namespace RoslynSandbox
{
    class Foo2
    {
        private readonly int value;
    }
}";

                var after = @"
namespace RoslynSandbox
{
    class Foo1
    {
        private readonly int value;
    }
}";

                after = after.AssertReplace("value", expected);
                var analyzer = new FieldNameMustNotBeginWithUnderscore();
                var fix = new DontUseUnderscoreManyCodeFixProvider();
                RoslynAssert.FixAll(analyzer, fix, new[] { before1, before2 }, after, title);
            }

            [TestCase("Rename to: value1", "value1")]
            [TestCase("Rename to: value2", "value2")]
            public static void TwoDocumentsTwoFixes(string title, string expected)
            {
                var before1 = @"
namespace RoslynSandbox
{
    class Foo1
    {
        private readonly int ↓_value;
    }
}";
                var before2 = @"
namespace RoslynSandbox
{
    class Foo2
    {
        private readonly int ↓_value;
    }
}";

                var after1 = @"
namespace RoslynSandbox
{
    class Foo1
    {
        private readonly int value;
    }
}";

                var after2 = @"
namespace RoslynSandbox
{
    class Foo2
    {
        private readonly int value;
    }
}";
                after1 = after1.AssertReplace("value", expected);
                after2 = after2.AssertReplace("value", expected);
                var analyzer = new FieldNameMustNotBeginWithUnderscore();
                var fix = new DontUseUnderscoreManyCodeFixProvider();
                RoslynAssert.FixAll(analyzer, fix, new[] { before1, before2 }, new[] { after1, after2 }, title);
            }

            [Test]
            public static void TwoDocumentsDifferentProjectsCodeFixOnlyOneFix()
            {
                var before1 = @"
namespace RoslynSandbox.Core
{
    using System;

    public class Foo1
    {
        public event EventHandler ↓Bar;
    }
}";

                var before2 = @"
namespace RoslynSandbox.Client
{
    public class Foo2
    {
    }
}";

                var after = @"
namespace RoslynSandbox.Core
{
    using System;

    public class Foo1
    {
    }
}";
                RoslynAssert.MetadataReferences.Add(MetadataReference.CreateFromFile(typeof(EventHandler).Assembly.Location));
                var expectedDiagnostic = ExpectedDiagnostic.CreateFromCodeWithErrorsIndicated("CS0067", before1, out before1);
                var fix = new RemoveUnusedFixProvider();
                RoslynAssert.FixAll(fix, expectedDiagnostic, new[] { before1, before2 }, new[] { after, before2 });
                RoslynAssert.FixAll(fix, expectedDiagnostic, new[] { before2, before1 }, new[] { before2, after });
            }

            [Test]
            public static void TwoDocumentsDifferentProjectsCodeFixOnlyOneFixPassingOnlyFixedCode()
            {
                var before1 = @"
namespace RoslynSandbox.Core
{
    using System;

    public class Foo1
    {
        public event EventHandler ↓Bar;
    }
}";

                var before2 = @"
namespace RoslynSandbox.Client
{
    public class Foo2
    {
    }
}";

                var after = @"
namespace RoslynSandbox.Core
{
    using System;

    public class Foo1
    {
    }
}";
                RoslynAssert.MetadataReferences.Add(MetadataReference.CreateFromFile(typeof(EventHandler).Assembly.Location));
                var expectedDiagnostic = ExpectedDiagnostic.CreateFromCodeWithErrorsIndicated("CS0067", before1, out before1);
                var fix = new RemoveUnusedFixProvider();
                RoslynAssert.FixAll(fix, expectedDiagnostic, new[] { before1, before2 }, after);
                RoslynAssert.FixAll(fix, expectedDiagnostic, new[] { before2, before1 }, after);
                RoslynAssert.FixAll(new RemoveUnusedFixProvider(), expectedDiagnostic, new[] { before2, before1 }, after);
            }

            [Test]
            public static void TwoDocumentsDifferentProjectsCodeFixOnly()
            {
                var before1 = @"
namespace RoslynSandbox.Core
{
    using System;

    public class FooCore
    {
        public event EventHandler ↓Bar;
    }
}";

                var before2 = @"
namespace RoslynSandbox.Client
{
    public class FooClient
    {
    }
}";

                var after = @"
namespace RoslynSandbox.Core
{
    using System;

    public class FooCore
    {
    }
}";
                RoslynAssert.MetadataReferences.Add(MetadataReference.CreateFromFile(typeof(EventHandler).Assembly.Location));
                var expectedDiagnostic = ExpectedDiagnostic.CreateFromCodeWithErrorsIndicated("CS0067", before1, out before1);
                var fix = new RemoveUnusedFixProvider();
                RoslynAssert.FixAll(fix, expectedDiagnostic, new[] { before1, before2 }, after);
                RoslynAssert.FixAll(fix, expectedDiagnostic, new[] { before2, before1 }, after);
            }

            [Test]
            public static void TwoDocumentsDifferentProjectsInheritingCodeFixOnly()
            {
                var before1 = @"
namespace RoslynSandbox.Core
{
    using System;

    public class Foo1
    {
        public event EventHandler ↓Bar;
    }
}";

                var before2 = @"
namespace RoslynSandbox.Client
{
    public class Foo2 : RoslynSandbox.Core.Foo1
    {
    }
}";

                var after = @"
namespace RoslynSandbox.Core
{
    using System;

    public class Foo1
    {
    }
}";
                RoslynAssert.MetadataReferences.Add(MetadataReference.CreateFromFile(typeof(EventHandler).Assembly.Location));
                var expectedDiagnostic = ExpectedDiagnostic.CreateFromCodeWithErrorsIndicated("CS0067", before1, out before1);
                var fix = new RemoveUnusedFixProvider();
                RoslynAssert.FixAll(fix, expectedDiagnostic, new[] { before1, before2 }, after);
                RoslynAssert.FixAll(fix, expectedDiagnostic, new[] { before2, before1 }, after);
            }

            [Test]
            public static void TwoDocumentsDifferentProjectsInheritingCodeFixOnlyCorrectFix2()
            {
                var before1 = @"
namespace RoslynSandbox.Core
{
    public class Foo1
    {
    }
}";

                var before2 = @"
namespace RoslynSandbox.Client
{
    using System;

    public class Foo2 : RoslynSandbox.Core.Foo1
    {
        public event EventHandler ↓Bar;
    }
}";

                var after = @"
namespace RoslynSandbox.Client
{
    using System;

    public class Foo2 : RoslynSandbox.Core.Foo1
    {
    }
}";
                RoslynAssert.MetadataReferences.Add(MetadataReference.CreateFromFile(typeof(EventHandler).Assembly.Location));
                var expectedDiagnostic = ExpectedDiagnostic.CreateFromCodeWithErrorsIndicated("CS0067", before2, out before2);
                var fix = new RemoveUnusedFixProvider();
                RoslynAssert.FixAll(fix, expectedDiagnostic, new[] { before1, before2 }, after);
                RoslynAssert.FixAll(fix, expectedDiagnostic, new[] { before2, before1 }, after);
            }

            [Test]
            public static void SingleDocumentCodeFixOnly()
            {
                var before = @"
namespace RoslynSandbox
{
    using System;

    public class Foo
    {
        public event EventHandler ↓Bar;
    }
}";

                var after = @"
namespace RoslynSandbox
{
    using System;

    public class Foo
    {
    }
}";
                RoslynAssert.MetadataReferences.Add(MetadataReference.CreateFromFile(typeof(EventHandler).Assembly.Location));
                var expectedDiagnostic = ExpectedDiagnostic.CreateFromCodeWithErrorsIndicated("CS0067", before, out before);
                var fix = new RemoveUnusedFixProvider();
                RoslynAssert.FixAll(fix, expectedDiagnostic, before, after);
            }

            [Test]
            public static void TwoClassOneError()
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

                var after = @"
namespace RoslynSandbox
{
    class Foo
    {
        private readonly int value;
    }
}";
                var expectedDiagnostic = ExpectedDiagnostic.Create(FieldNameMustNotBeginWithUnderscore.DiagnosticId);
                var analyzer = new FieldNameMustNotBeginWithUnderscore();
                var fix = new DontUseUnderscoreCodeFixProvider();
                RoslynAssert.FixAll(analyzer, fix, new[] { barCode, testCode }, new[] { barCode, after });
                RoslynAssert.FixAll(analyzer, fix, new[] { barCode, testCode }, after);
                RoslynAssert.FixAll(new FieldNameMustNotBeginWithUnderscore(), new DontUseUnderscoreCodeFixProvider(), new[] { barCode, testCode }, after);
                RoslynAssert.FixAll(new FieldNameMustNotBeginWithUnderscore(), new DontUseUnderscoreCodeFixProvider(), new[] { barCode, testCode }, new[] { barCode, after });
                RoslynAssert.FixAll(new FieldNameMustNotBeginWithUnderscore(), new DontUseUnderscoreCodeFixProvider(), expectedDiagnostic, new[] { barCode, testCode }, after);
                RoslynAssert.FixAll(new FieldNameMustNotBeginWithUnderscore(), new DontUseUnderscoreCodeFixProvider(), expectedDiagnostic, new[] { barCode, testCode }, new[] { barCode, after });
            }

            [Test]
            public static void WhenFixIntroducesCompilerErrorsThatAreAccepted()
            {
                var before = @"
namespace RoslynSandbox
{
    ↓class Value
    {
    }
}";

                var after = @"
namespace RoslynSandbox
{
    class Value
    {
        public event EventHandler SomeEvent;
    }
}";
                var analyzer = new ClassMustHaveEventAnalyzer();
                var fix = new InsertEventFixProvider();
                RoslynAssert.FixAll(analyzer, fix, before, after, allowCompilationErrors: AllowCompilationErrors.Yes);
            }

            [Test]
            public static void WithExpectedDiagnosticWhenOneReportsError()
            {
                var before = @"
namespace RoslynSandbox
{
    class Value
    {
        private readonly int ↓wrongName;
        
        public int WrongName { get; set; }
    }
}";

                var after = @"
namespace RoslynSandbox
{
    class Value
    {
        private readonly int value;
        
        public int WrongName { get; set; }
    }
}";
                var expectedDiagnostic = ExpectedDiagnostic.Create(FieldAndPropertyMustBeNamedValueAnalyzer.FieldDiagnosticId);
                var analyzer = new FieldAndPropertyMustBeNamedValueAnalyzer();
                var fix = new RenameToValueCodeFixProvider();
                RoslynAssert.FixAll(analyzer, fix, expectedDiagnostic, before, after);
            }
        }
    }
}
