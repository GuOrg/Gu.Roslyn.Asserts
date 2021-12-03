namespace Gu.Roslyn.Asserts.Tests.RoslynAssertTests
{
    using Gu.Roslyn.Asserts.Tests.CodeFixes;

    using NUnit.Framework;

    [TestFixture]
    public static partial class FixAll
    {
        public static class Success
        {
            [Test]
            public static void OneError()
            {
                var before = @"
namespace N
{
    class C
    {
        private readonly int ↓_value;
    }
}";

                var after = @"
namespace N
{
    class C
    {
        private readonly int value;
    }
}";
                var analyzer = new FieldNameMustNotBeginWithUnderscore();
                var fix = new DoNotUseUnderscoreFix();
                RoslynAssert.FixAll(analyzer, fix, before, after);
            }

            [TestCase("Rename to: 'value1'", "value1")]
            [TestCase("Rename to: 'value2'", "value2")]
            public static void SingleDocumentOneErrorTwoFixes(string fixTitle, string expected)
            {
                var before = @"
namespace N
{
    class C
    {
        private readonly int ↓_value;
    }
}";

                var after = @"
namespace N
{
    class C
    {
        private readonly int value;
    }
}".AssertReplace("value", expected);

                var analyzer = new FieldNameMustNotBeginWithUnderscore();
                var fix = new DontUseUnderscoreManyFix();
                RoslynAssert.FixAll(analyzer, fix, before, after, fixTitle);
            }

            [Test]
            public static void TwoErrors()
            {
                var before = @"
namespace N
{
    class C
    {
        private readonly int ↓_value1;
        private readonly int ↓_value2;
    }
}";

                var after = @"
namespace N
{
    class C
    {
        private readonly int value1;
        private readonly int value2;
    }
}";
                var analyzer = new FieldNameMustNotBeginWithUnderscore();
                var fix = new DoNotUseUnderscoreFix();
                RoslynAssert.FixAll(analyzer, fix, before, after);
            }

            [Test]
            public static void FixAllInDocumentTwoErrors()
            {
                var before = @"
namespace N
{
    class C
    {
        private readonly int ↓_value1;
        private readonly int ↓_value2;
    }
}";

                var after = @"
namespace N
{
    class C
    {
        private readonly int value1;
        private readonly int value2;
    }
}";
                var analyzer = new FieldNameMustNotBeginWithUnderscore();
                var fix = new DoNotUseUnderscoreFix();
                RoslynAssert.FixAllInDocument(analyzer, fix, before, after);
            }

            [Test]
            public static void FixAllOneByOneTwoErrors()
            {
                var before = @"
namespace N
{
    class C
    {
        private readonly int ↓_value1;
        private readonly int ↓_value2;
    }
}";

                var after = @"
namespace N
{
    class C
    {
        private readonly int value1;
        private readonly int value2;
    }
}";
                var analyzer = new FieldNameMustNotBeginWithUnderscore();
                var fix = new DoNotUseUnderscoreFix();
                RoslynAssert.FixAllOneByOne(analyzer, fix, before, after);
            }

            [Test]
            public static void SingleDocumentOneErrorCorrectFixExplicitTitleExpectedDiagnosticWithPositionAnalyzerSupportsTwoDiagnostics1()
            {
                var before = @"
namespace N
{
    class C
    {
        public readonly int ↓_value;
    }
}";

                var after = @"
namespace N
{
    class C
    {
        public readonly int value;
    }
}";
                var expectedDiagnostic = ExpectedDiagnostic.Create(FieldNameMustNotBeginWithUnderscoreDifferentDiagnosticsForPublic.Id1);
                var analyzer = new FieldNameMustNotBeginWithUnderscoreDifferentDiagnosticsForPublic();
                var fix = new DoNotUseUnderscoreFix();
                RoslynAssert.FixAll(analyzer, fix, expectedDiagnostic, before, after);
                RoslynAssert.FixAll(analyzer, fix, expectedDiagnostic, new[] { before }, after);
                RoslynAssert.FixAll(analyzer, fix, expectedDiagnostic, before, after, fixTitle: "Rename to: 'value'");
                RoslynAssert.FixAll(analyzer, fix, expectedDiagnostic, new[] { before }, after, fixTitle: "Rename to: 'value'");
            }

            [Test]
            public static void SingleDocumentOneErrorCorrectFixExplicitTitleExpectedDiagnosticWithPositionAnalyzerSupportsTwoDiagnostics2()
            {
                var before = @"
namespace N
{
    class C
    {
        private readonly int ↓_value;
    }
}";

                var after = @"
namespace N
{
    class C
    {
        private readonly int value;
    }
}";
                var expectedDiagnostic = ExpectedDiagnostic.Create(FieldNameMustNotBeginWithUnderscoreDifferentDiagnosticsForPublic.Id2);
                var analyzer = new FieldNameMustNotBeginWithUnderscoreDifferentDiagnosticsForPublic();
                var fix = new DoNotUseUnderscoreFix();
                RoslynAssert.FixAll(analyzer, fix, expectedDiagnostic, before, after);
                RoslynAssert.FixAll(analyzer, fix, expectedDiagnostic, new[] { before }, after);
                RoslynAssert.FixAll(analyzer, fix, expectedDiagnostic, before, after, fixTitle: "Rename to: 'value'");
                RoslynAssert.FixAll(analyzer, fix, expectedDiagnostic, new[] { before }, after, fixTitle: "Rename to: 'value'");
            }

            [Test]
            public static void TwoDocumentsTwoErrorsTwoFixes()
            {
                var before1 = @"
namespace N
{
    using System;

    public class C1
    {
        public event EventHandler? ↓E;
    }
}";

                var before2 = @"
namespace N
{
    using System;

    public class C2
    {
        public event EventHandler? ↓E;
    }
}";

                var fixed1 = @"
namespace N
{
    using System;

    public class C1
    {
    }
}";

                var fixed2 = @"
namespace N
{
    using System;

    public class C2
    {
    }
}";
                var expectedDiagnostic = ExpectedDiagnostic.Create("CS0067");
                var fix = new RemoveUnusedFix();
                RoslynAssert.FixAll(fix, expectedDiagnostic, new[] { before1, before2 }, new[] { fixed1, fixed2 });
                RoslynAssert.FixAll(fix, expectedDiagnostic, new[] { before2, before1 }, new[] { fixed2, fixed1 });
            }

            [TestCase("Rename to: 'value1'", "value1")]
            [TestCase("Rename to: 'value2'", "value2")]
            public static void TwoDocumentsOneErrorWhenCodeFixProviderHasManyFixes(string fixTitle, string expected)
            {
                var before1 = @"
namespace N
{
    class C1
    {
        private readonly int ↓_value;
    }
}";
                var before2 = @"
namespace N
{
    class C2
    {
        private readonly int value;
    }
}";

                var after = @"
namespace N
{
    class C1
    {
        private readonly int value;
    }
}".AssertReplace("value", expected);
                var expectedDiagnostic = ExpectedDiagnostic.Create(FieldNameMustNotBeginWithUnderscore.DiagnosticId);
                var analyzer = new FieldNameMustNotBeginWithUnderscore();
                var fix = new DontUseUnderscoreManyFix();
                RoslynAssert.FixAll(analyzer, fix, new[] { before1, before2 }, new[] { after, before2 }, fixTitle);
                RoslynAssert.FixAll(analyzer, fix, new[] { before1, before2 }, after, fixTitle);
                RoslynAssert.FixAll(analyzer, fix, expectedDiagnostic, new[] { before1, before2 }, new[] { after, before2 }, fixTitle);
                RoslynAssert.FixAll(analyzer, fix, expectedDiagnostic, new[] { before1, before2 }, after, fixTitle);
            }

            [TestCase("Rename to: 'value1'", "value1")]
            [TestCase("Rename to: 'value2'", "value2")]
            public static void TwoDocumentsOneFixCorrectFixPassOnlyFixedCode(string fixTitle, string expected)
            {
                var before1 = @"
namespace N
{
    class C1
    {
        private readonly int ↓_value;
    }
}";
                var before2 = @"
namespace N
{
    class C2
    {
        private readonly int value;
    }
}";

                var after = @"
namespace N
{
    class C1
    {
        private readonly int value;
    }
}".AssertReplace("value", expected);
                var analyzer = new FieldNameMustNotBeginWithUnderscore();
                var fix = new DontUseUnderscoreManyFix();
                RoslynAssert.FixAll(analyzer, fix, new[] { before1, before2 }, after, fixTitle);
            }

            [TestCase("Rename to: 'value1'", "value1")]
            [TestCase("Rename to: 'value2'", "value2")]
            public static void TwoDocumentsTwoFixes(string title, string expected)
            {
                var before1 = @"
namespace N
{
    class C1
    {
        private readonly int ↓_value;
    }
}";
                var before2 = @"
namespace N
{
    class C2
    {
        private readonly int ↓_value;
    }
}";

                var after1 = @"
namespace N
{
    class C1
    {
        private readonly int value;
    }
}".AssertReplace("value", expected);

                var after2 = @"
namespace N
{
    class C2
    {
        private readonly int value;
    }
}".AssertReplace("value", expected);
                var analyzer = new FieldNameMustNotBeginWithUnderscore();
                var fix = new DontUseUnderscoreManyFix();
                RoslynAssert.FixAll(analyzer, fix, new[] { before1, before2 }, new[] { after1, after2 }, title);
            }

            [Test]
            public static void TwoDocumentsDifferentProjectsCodeFixOnlyOneFix()
            {
                var before1 = @"
namespace N.Core
{
    using System;

    public class C1
    {
        public event EventHandler? ↓E;
    }
}";

                var before2 = @"
namespace N.Client
{
    public class C2
    {
    }
}";

                var after = @"
namespace N.Core
{
    using System;

    public class C1
    {
    }
}";
                var expectedDiagnostic = ExpectedDiagnostic.CreateFromCodeWithErrorsIndicated("CS0067", before1, out before1);
                var fix = new RemoveUnusedFix();
                RoslynAssert.FixAll(fix, expectedDiagnostic, new[] { before1, before2 }, new[] { after, before2 });
                RoslynAssert.FixAll(fix, expectedDiagnostic, new[] { before2, before1 }, new[] { before2, after });
            }

            [Test]
            public static void TwoDocumentsDifferentProjectsCodeFixOnlyOneFixPassingOnlyFixedCode()
            {
                var before1 = @"
namespace N.Core
{
    using System;

    public class C1
    {
        public event EventHandler? ↓E;
    }
}";

                var before2 = @"
namespace N.Client
{
    public class C2
    {
    }
}";

                var after = @"
namespace N.Core
{
    using System;

    public class C1
    {
    }
}";
                var expectedDiagnostic = ExpectedDiagnostic.CreateFromCodeWithErrorsIndicated("CS0067", before1, out before1);
                var fix = new RemoveUnusedFix();
                RoslynAssert.FixAll(fix, expectedDiagnostic, new[] { before1, before2 }, after);
                RoslynAssert.FixAll(fix, expectedDiagnostic, new[] { before2, before1 }, after);
                RoslynAssert.FixAll(fix, expectedDiagnostic, new[] { before2, before1 }, after);
            }

            [Test]
            public static void TwoDocumentsDifferentProjectsCodeFixOnly()
            {
                var before1 = @"
namespace N.Core
{
    using System;

    public class CCore
    {
        public event EventHandler? ↓Bar;
    }
}";

                var before2 = @"
namespace N.Client
{
    public class CClient
    {
    }
}";

                var after = @"
namespace N.Core
{
    using System;

    public class CCore
    {
    }
}";
                var expectedDiagnostic = ExpectedDiagnostic.CreateFromCodeWithErrorsIndicated("CS0067", before1, out before1);
                var fix = new RemoveUnusedFix();
                RoslynAssert.FixAll(fix, expectedDiagnostic, new[] { before1, before2 }, after);
                RoslynAssert.FixAll(fix, expectedDiagnostic, new[] { before2, before1 }, after);
            }

            [Test]
            public static void TwoDocumentsDifferentProjectsInheritingCodeFixOnly()
            {
                var before1 = @"
namespace N.Core
{
    using System;

    public class C1
    {
        public event EventHandler? ↓E;
    }
}";

                var before2 = @"
namespace N.Client
{
    public class C2 : N.Core.C1
    {
    }
}";

                var after = @"
namespace N.Core
{
    using System;

    public class C1
    {
    }
}";
                var expectedDiagnostic = ExpectedDiagnostic.CreateFromCodeWithErrorsIndicated("CS0067", before1, out before1);
                var fix = new RemoveUnusedFix();
                RoslynAssert.FixAll(fix, expectedDiagnostic, new[] { before1, before2 }, after);
                RoslynAssert.FixAll(fix, expectedDiagnostic, new[] { before2, before1 }, after);
            }

            [Test]
            public static void TwoDocumentsDifferentProjectsInheritingCodeFixOnlyCorrectFix2()
            {
                var before1 = @"
namespace N.Core
{
    public class C1
    {
    }
}";

                var before2 = @"
namespace N.Client
{
    using System;

    public class C2 : N.Core.C1
    {
        public event EventHandler? ↓Bar;
    }
}";

                var after = @"
namespace N.Client
{
    using System;

    public class C2 : N.Core.C1
    {
    }
}";
                var expectedDiagnostic = ExpectedDiagnostic.CreateFromCodeWithErrorsIndicated("CS0067", before2, out before2);
                var fix = new RemoveUnusedFix();
                RoslynAssert.FixAll(fix, expectedDiagnostic, new[] { before1, before2 }, after);
                RoslynAssert.FixAll(fix, expectedDiagnostic, new[] { before2, before1 }, after);
            }

            [Test]
            public static void SingleDocumentCodeFixOnly()
            {
                var before = @"
namespace N
{
    using System;

    public class C
    {
        public event EventHandler? ↓Bar;
    }
}";

                var after = @"
namespace N
{
    using System;

    public class C
    {
    }
}";
                var expectedDiagnostic = ExpectedDiagnostic.CreateFromCodeWithErrorsIndicated("CS0067", before, out before);
                var fix = new RemoveUnusedFix();
                RoslynAssert.FixAll(fix, expectedDiagnostic, before, after);
            }

            [Test]
            public static void TwoClassOneError()
            {
                var barCode = @"
namespace N
{
    class Bar
    {
        private readonly int value;
    }
}";

                var before = @"
namespace N
{
    class C
    {
        private readonly int ↓_value;
    }
}";

                var after = @"
namespace N
{
    class C
    {
        private readonly int value;
    }
}";
                var expectedDiagnostic = ExpectedDiagnostic.Create(FieldNameMustNotBeginWithUnderscore.DiagnosticId);
                var analyzer = new FieldNameMustNotBeginWithUnderscore();
                var fix = new DoNotUseUnderscoreFix();
                RoslynAssert.FixAll(analyzer, fix, new[] { barCode, before }, new[] { barCode, after });
                RoslynAssert.FixAll(analyzer, fix, new[] { barCode, before }, after);
                RoslynAssert.FixAll(analyzer, fix, new[] { barCode, before }, after);
                RoslynAssert.FixAll(analyzer, fix, new[] { barCode, before }, new[] { barCode, after });
                RoslynAssert.FixAll(analyzer, fix, expectedDiagnostic, new[] { barCode, before }, after);
                RoslynAssert.FixAll(analyzer, fix, expectedDiagnostic, new[] { barCode, before }, new[] { barCode, after });
            }

            [Test]
            public static void WhenFixIntroducesCompilerErrorsThatAreAccepted()
            {
                var before = @"
namespace N
{
    ↓class Value
    {
    }
}";

                var after = @"
namespace N
{
    class Value
    {
        public event EventHandler E;
    }
}";
                var analyzer = new ClassMustHaveEventAnalyzer();
                var fix = new InsertEventFix();
                RoslynAssert.FixAll(analyzer, fix, before, after, settings: Settings.Default.WithAllowedCompilationDiagnostics(AllowCompilationDiagnostics.WarningsAndErrors));
            }

            [Test]
            public static void WithExpectedDiagnosticWhenOneReportsError()
            {
                var before = @"
namespace N
{
    class Value
    {
        private readonly int ↓wrongName;
        
        public int WrongName { get; set; }
    }
}";

                var after = @"
namespace N
{
    class Value
    {
        private readonly int value;
        
        public int WrongName { get; set; }
    }
}";
                var expectedDiagnostic = ExpectedDiagnostic.Create(FieldAndPropertyMustBeNamedValueAnalyzer.FieldDiagnosticId);
                var analyzer = new FieldAndPropertyMustBeNamedValueAnalyzer();
                var fix = new RenameToValueFix();
                RoslynAssert.FixAll(analyzer, fix, expectedDiagnostic, before, after);
            }
        }
    }
}
