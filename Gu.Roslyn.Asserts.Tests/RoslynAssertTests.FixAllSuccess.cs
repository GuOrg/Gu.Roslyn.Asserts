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
                RoslynAssert.MetadataReferences.Add(MetadataReference.CreateFromFile(typeof(int).Assembly.Location));
                RoslynAssert.FixAll<FieldNameMustNotBeginWithUnderscore, DontUseUnderscoreCodeFixProvider>(before, after);
            }

            [TestCase("Rename to: value1", "value1")]
            [TestCase("Rename to: value2", "value2")]
            public void SingleDocumentOneErrorTwoFixes(string title, string expected)
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
                after = after.AssertReplace("value", expected);
                RoslynAssert.MetadataReferences.Add(MetadataReference.CreateFromFile(typeof(int).Assembly.Location));
                RoslynAssert.FixAll<FieldNameMustNotBeginWithUnderscore, DontUseUnderscoreManyCodeFixProvider>(before, after, title);
            }

            [Test]
            public void TwoErrors()
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
                RoslynAssert.MetadataReferences.Add(MetadataReference.CreateFromFile(typeof(int).Assembly.Location));
                RoslynAssert.FixAll<FieldNameMustNotBeginWithUnderscore, DontUseUnderscoreCodeFixProvider>(before, after);
            }

            [Test]
            public void FixAllInDocumentTwoErrors()
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
                RoslynAssert.MetadataReferences.Add(MetadataReference.CreateFromFile(typeof(int).Assembly.Location));
                RoslynAssert.FixAllInDocument<FieldNameMustNotBeginWithUnderscore, DontUseUnderscoreCodeFixProvider>(before, after);
            }

            [Test]
            public void FixAllOneByOneTwoErrors()
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
                RoslynAssert.MetadataReferences.Add(MetadataReference.CreateFromFile(typeof(int).Assembly.Location));
                RoslynAssert.FixAllOneByOne<FieldNameMustNotBeginWithUnderscore, DontUseUnderscoreCodeFixProvider>(before, after);
            }

            [Test]
            public void SingleDocumentOneErrorCorrectFixExplicitTitleExpectedDiagnosticWithPositionAnalyzerSupportsTwoDiagnostics1()
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
                RoslynAssert.MetadataReferences.Add(MetadataReference.CreateFromFile(typeof(int).Assembly.Location));
                var expectedDiagnostic = ExpectedDiagnostic.Create(FieldNameMustNotBeginWithUnderscoreDifferentDiagnosticsForPublic.Id1);
                RoslynAssert.FixAll<FieldNameMustNotBeginWithUnderscoreDifferentDiagnosticsForPublic, DontUseUnderscoreCodeFixProvider>(expectedDiagnostic, before, after);
                RoslynAssert.FixAll<FieldNameMustNotBeginWithUnderscoreDifferentDiagnosticsForPublic, DontUseUnderscoreCodeFixProvider>(expectedDiagnostic, new[] { before }, after);
                RoslynAssert.FixAll(new FieldNameMustNotBeginWithUnderscoreDifferentDiagnosticsForPublic(), new DontUseUnderscoreCodeFixProvider(), expectedDiagnostic, before, after);
                RoslynAssert.FixAll(new FieldNameMustNotBeginWithUnderscoreDifferentDiagnosticsForPublic(), new DontUseUnderscoreCodeFixProvider(), expectedDiagnostic, new[] { before }, after);
                RoslynAssert.FixAll<FieldNameMustNotBeginWithUnderscoreDifferentDiagnosticsForPublic, DontUseUnderscoreCodeFixProvider>(expectedDiagnostic, before, after, "Rename to: value");
                RoslynAssert.FixAll<FieldNameMustNotBeginWithUnderscoreDifferentDiagnosticsForPublic, DontUseUnderscoreCodeFixProvider>(expectedDiagnostic, new[] { before }, after, "Rename to: value");
                RoslynAssert.FixAll(new FieldNameMustNotBeginWithUnderscoreDifferentDiagnosticsForPublic(), new DontUseUnderscoreCodeFixProvider(), expectedDiagnostic, before, after, fixTitle: "Rename to: value");
                RoslynAssert.FixAll(new FieldNameMustNotBeginWithUnderscoreDifferentDiagnosticsForPublic(), new DontUseUnderscoreCodeFixProvider(), expectedDiagnostic, new[] { before }, after, fixTitle: "Rename to: value");
            }

            [Test]
            public void SingleDocumentOneErrorCorrectFixExplicitTitleExpectedDiagnosticWithPositionAnalyzerSupportsTwoDiagnostics2()
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
                RoslynAssert.MetadataReferences.Add(MetadataReference.CreateFromFile(typeof(int).Assembly.Location));
                var expectedDiagnostic = ExpectedDiagnostic.Create(FieldNameMustNotBeginWithUnderscoreDifferentDiagnosticsForPublic.Id2);
                RoslynAssert.FixAll<FieldNameMustNotBeginWithUnderscoreDifferentDiagnosticsForPublic, DontUseUnderscoreCodeFixProvider>(expectedDiagnostic, before, after);
                RoslynAssert.FixAll<FieldNameMustNotBeginWithUnderscoreDifferentDiagnosticsForPublic, DontUseUnderscoreCodeFixProvider>(expectedDiagnostic, new[] { before }, after);
                RoslynAssert.FixAll(new FieldNameMustNotBeginWithUnderscoreDifferentDiagnosticsForPublic(), new DontUseUnderscoreCodeFixProvider(), expectedDiagnostic, before, after);
                RoslynAssert.FixAll(new FieldNameMustNotBeginWithUnderscoreDifferentDiagnosticsForPublic(), new DontUseUnderscoreCodeFixProvider(), expectedDiagnostic, new[] { before }, after);
                RoslynAssert.FixAll<FieldNameMustNotBeginWithUnderscoreDifferentDiagnosticsForPublic, DontUseUnderscoreCodeFixProvider>(expectedDiagnostic, before, after, "Rename to: value");
                RoslynAssert.FixAll<FieldNameMustNotBeginWithUnderscoreDifferentDiagnosticsForPublic, DontUseUnderscoreCodeFixProvider>(expectedDiagnostic, new[] { before }, after, "Rename to: value");
                RoslynAssert.FixAll(new FieldNameMustNotBeginWithUnderscoreDifferentDiagnosticsForPublic(), new DontUseUnderscoreCodeFixProvider(), expectedDiagnostic, before, after, fixTitle: "Rename to: value");
                RoslynAssert.FixAll(new FieldNameMustNotBeginWithUnderscoreDifferentDiagnosticsForPublic(), new DontUseUnderscoreCodeFixProvider(), expectedDiagnostic, new[] { before }, after, fixTitle: "Rename to: value");
            }

            [Test]
            public void TwoDocumentsTwoErrorsTwoFixes()
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
                RoslynAssert.FixAll<RemoveUnusedFixProvider>(expectedDiagnostic, new[] { before1, before2 }, new[] { fixed1, fixed2 });
                RoslynAssert.FixAll<RemoveUnusedFixProvider>(expectedDiagnostic, new[] { before2, before1 }, new[] { fixed2, fixed1 });
            }

            [TestCase("Rename to: value1", "value1")]
            [TestCase("Rename to: value2", "value2")]
            public void TwoDocumentsOneErrorWhenCodeFixProviderHasManyFixes(string title, string expected)
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
                RoslynAssert.MetadataReferences.Add(MetadataReference.CreateFromFile(typeof(int).Assembly.Location));
                var expectedDiagnostic = ExpectedDiagnostic.Create(FieldNameMustNotBeginWithUnderscore.DiagnosticId);
                RoslynAssert.FixAll<FieldNameMustNotBeginWithUnderscore, DontUseUnderscoreManyCodeFixProvider>(new[] { before1, before2 }, new[] { after, before2 }, title);
                RoslynAssert.FixAll<FieldNameMustNotBeginWithUnderscore, DontUseUnderscoreManyCodeFixProvider>(new[] { before1, before2 }, after, title);
                RoslynAssert.FixAll<FieldNameMustNotBeginWithUnderscore, DontUseUnderscoreManyCodeFixProvider>(expectedDiagnostic, new[] { before1, before2 }, new[] { after, before2 }, title);
                RoslynAssert.FixAll<FieldNameMustNotBeginWithUnderscore, DontUseUnderscoreManyCodeFixProvider>(expectedDiagnostic, new[] { before1, before2 }, after, title);
            }

            [TestCase("Rename to: value1", "value1")]
            [TestCase("Rename to: value2", "value2")]
            public void TwoDocumentsOneFixCorrectFixPassOnlyFixedCode(string title, string expected)
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
                RoslynAssert.MetadataReferences.Add(MetadataReference.CreateFromFile(typeof(int).Assembly.Location));
                RoslynAssert.FixAll<FieldNameMustNotBeginWithUnderscore, DontUseUnderscoreManyCodeFixProvider>(new[] { before1, before2 }, after, title);
            }

            [TestCase("Rename to: value1", "value1")]
            [TestCase("Rename to: value2", "value2")]
            public void TwoDocumentsTwoFixes(string title, string expected)
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
                RoslynAssert.MetadataReferences.Add(MetadataReference.CreateFromFile(typeof(int).Assembly.Location));
                RoslynAssert.FixAll<FieldNameMustNotBeginWithUnderscore, DontUseUnderscoreManyCodeFixProvider>(new[] { before1, before2 }, new[] { after1, after2 }, title);
            }

            [Test]
            public void TwoDocumentsDifferentProjectsCodeFixOnlyOneFix()
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
                RoslynAssert.FixAll<RemoveUnusedFixProvider>(expectedDiagnostic, new[] { before1, before2 }, new[] { after, before2 });
                RoslynAssert.FixAll<RemoveUnusedFixProvider>(expectedDiagnostic, new[] { before2, before1 }, new[] { before2, after });
            }

            [Test]
            public void TwoDocumentsDifferentProjectsCodeFixOnlyOneFixPassingOnlyFixedCode()
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
                RoslynAssert.FixAll<RemoveUnusedFixProvider>(expectedDiagnostic, new[] { before1, before2 }, after);
                RoslynAssert.FixAll<RemoveUnusedFixProvider>(expectedDiagnostic, new[] { before2, before1 }, after);
                RoslynAssert.FixAll(new RemoveUnusedFixProvider(), expectedDiagnostic, new[] { before2, before1 }, after);
            }

            [Test]
            public void TwoDocumentsDifferentProjectsCodeFixOnly()
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
                RoslynAssert.FixAll<RemoveUnusedFixProvider>(expectedDiagnostic, new[] { before1, before2 }, after);
                RoslynAssert.FixAll<RemoveUnusedFixProvider>(expectedDiagnostic, new[] { before2, before1 }, after);
                RoslynAssert.FixAll(new RemoveUnusedFixProvider(), expectedDiagnostic, new[] { before2, before1 }, after);
            }

            [Test]
            public void TwoDocumentsDifferentProjectsInheritingCodeFixOnly()
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
                RoslynAssert.FixAll<RemoveUnusedFixProvider>(expectedDiagnostic, new[] { before1, before2 }, after);
                RoslynAssert.FixAll<RemoveUnusedFixProvider>(expectedDiagnostic, new[] { before2, before1 }, after);
                RoslynAssert.FixAll(new RemoveUnusedFixProvider(), expectedDiagnostic, new[] { before2, before1 }, after);
            }

            [Test]
            public void TwoDocumentsDifferentProjectsInheritingCodeFixOnlyCorrectFix2()
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
                RoslynAssert.FixAll<RemoveUnusedFixProvider>(expectedDiagnostic, new[] { before1, before2 }, after);
                RoslynAssert.FixAll<RemoveUnusedFixProvider>(expectedDiagnostic, new[] { before2, before1 }, after);
                RoslynAssert.FixAll(new RemoveUnusedFixProvider(), expectedDiagnostic, new[] { before2, before1 }, after);
            }

            [Test]
            public void SingleDocumentCodeFixOnly()
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
                RoslynAssert.FixAll<RemoveUnusedFixProvider>(expectedDiagnostic, before, after);
                RoslynAssert.FixAll(new RemoveUnusedFixProvider(), expectedDiagnostic, before, after);
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

                var after = @"
namespace RoslynSandbox
{
    class Foo
    {
        private readonly int value;
    }
}";
                RoslynAssert.MetadataReferences.Add(MetadataReference.CreateFromFile(typeof(int).Assembly.Location));
                var expectedDiagnostic = ExpectedDiagnostic.Create(FieldNameMustNotBeginWithUnderscore.DiagnosticId);
                RoslynAssert.FixAll<FieldNameMustNotBeginWithUnderscore, DontUseUnderscoreCodeFixProvider>(new[] { barCode, testCode }, new[] { barCode, after });
                RoslynAssert.FixAll<FieldNameMustNotBeginWithUnderscore, DontUseUnderscoreCodeFixProvider>(new[] { barCode, testCode }, after);
                RoslynAssert.FixAll(new FieldNameMustNotBeginWithUnderscore(), new DontUseUnderscoreCodeFixProvider(), new[] { barCode, testCode }, after);
                RoslynAssert.FixAll(new FieldNameMustNotBeginWithUnderscore(), new DontUseUnderscoreCodeFixProvider(), new[] { barCode, testCode }, new[] { barCode, after });
                RoslynAssert.FixAll(new FieldNameMustNotBeginWithUnderscore(), new DontUseUnderscoreCodeFixProvider(), expectedDiagnostic, new[] { barCode, testCode }, after);
                RoslynAssert.FixAll(new FieldNameMustNotBeginWithUnderscore(), new DontUseUnderscoreCodeFixProvider(), expectedDiagnostic, new[] { barCode, testCode }, new[] { barCode, after });
            }

            [Test]
            public void WhenFixIntroducesCompilerErrorsThatAreAccepted()
            {
                var before = @"
namespace RoslynSandbox
{
    ↓class Foo
    {
    }
}";

                var after = @"
namespace RoslynSandbox
{
    class Foo
    {
        public event EventHandler SomeEvent;
    }
}";
                RoslynAssert.FixAll<ClassMustHaveEventAnalyzer, InsertEventFixProvider>(before, after, allowCompilationErrors: AllowCompilationErrors.Yes);
            }

            [Test]
            public void WithExpectedDiagnosticWhenOneReportsError()
            {
                var before = @"
namespace RoslynSandbox
{
    class Foo
    {
        private readonly int ↓wrongName;
        
        public int WrongName { get; set; }
    }
}";

                var after = @"
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
                RoslynAssert.FixAll<FieldAndPropertyMustBeNamedFooAnalyzer, RenameToFooCodeFixProvider>(expectedDiagnostic, before, after);
                RoslynAssert.FixAll(new FieldAndPropertyMustBeNamedFooAnalyzer(), new RenameToFooCodeFixProvider(), expectedDiagnostic, before, after);
            }
        }
    }
}
