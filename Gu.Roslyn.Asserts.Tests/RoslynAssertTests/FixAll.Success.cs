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
            public static void OneDiagnostic()
            {
                var before = @"
namespace N
{
    class C
    {
        private readonly int ↓_f = 1;

        public int M() => _f;
    }
}";

                var after = @"
namespace N
{
    class C
    {
        private readonly int f = 1;

        public int M() => f;
    }
}";
                var analyzer = new FieldNameMustNotBeginWithUnderscore();
                var fix = new DoNotUseUnderscoreFix();
                RoslynAssert.FixAll(analyzer, fix, before, after);
            }

            [TestCase("Rename to: 'f1'", "f1")]
            [TestCase("Rename to: 'f2'", "f2")]
            public static void SingleDocumentOneDiagnosticTwoFixes(string fixTitle, string expected)
            {
                var before = @"
namespace N
{
    class C
    {
        private readonly int ↓_f = 1;

        public int M() => _f;
    }
}";

                var after = @"
namespace N
{
    class C
    {
        private readonly int f = 1;

        public int M() => f;
    }
}".AssertReplace("f", expected);

                var analyzer = new FieldNameMustNotBeginWithUnderscore();
                var fix = new DontUseUnderscoreManyFix();
                RoslynAssert.FixAll(analyzer, fix, before, after, fixTitle);
            }

            [Test]
            public static void TwoDiagnostics()
            {
                var before = @"
namespace N
{
    class C
    {
        private readonly int ↓_f1 = 1;
        private readonly int ↓_f2 = 2;

        public int M() => _f1 + _f2;
    }
}";

                var after = @"
namespace N
{
    class C
    {
        private readonly int f1 = 1;
        private readonly int f2 = 2;

        public int M() => f1 + f2;
    }
}";
                var analyzer = new FieldNameMustNotBeginWithUnderscore();
                var fix = new DoNotUseUnderscoreFix();
                RoslynAssert.FixAll(analyzer, fix, before, after);
            }

            [Test]
            public static void FixAllInDocumentTwoDiagnostics()
            {
                var before = @"
namespace N
{
    class C
    {
        private readonly int ↓_f1 = 1;
        private readonly int ↓_f2 = 2;

        public int M() => _f1 + _f2;
    }
}";

                var after = @"
namespace N
{
    class C
    {
        private readonly int f1 = 1;
        private readonly int f2 = 2;

        public int M() => f1 + f2;
    }
}";
                var analyzer = new FieldNameMustNotBeginWithUnderscore();
                var fix = new DoNotUseUnderscoreFix();
                RoslynAssert.FixAllInDocument(analyzer, fix, before, after);
            }

            [Test]
            public static void FixAllOneByOneTwoDiagnostics()
            {
                var before = @"
namespace N
{
    class C
    {
        private readonly int ↓_f1 = 1;
        private readonly int ↓_f2 = 2;

        public int M() => _f1 + _f2;
    }
}";

                var after = @"
namespace N
{
    class C
    {
        private readonly int f1 = 1;
        private readonly int f2 = 2;

        public int M() => f1 + f2;
    }
}";
                var analyzer = new FieldNameMustNotBeginWithUnderscore();
                var fix = new DoNotUseUnderscoreFix();
                RoslynAssert.FixAllOneByOne(analyzer, fix, before, after);
            }

            [Test]
            public static void SingleDocumentOneDiagnosticCorrectFixExplicitTitleExpectedDiagnosticWithPositionAnalyzerSupportsTwoDiagnostics1()
            {
                var before = @"
namespace N
{
    class C
    {
        public readonly int ↓_f = 1;

        public int M() => _f;
    }
}";

                var after = @"
namespace N
{
    class C
    {
        public readonly int f = 1;

        public int M() => f;
    }
}";
                var expectedDiagnostic = ExpectedDiagnostic.Create(FieldNameMustNotBeginWithUnderscoreDifferentDiagnosticsForPublic.Id1);
                var analyzer = new FieldNameMustNotBeginWithUnderscoreDifferentDiagnosticsForPublic();
                var fix = new DoNotUseUnderscoreFix();
                RoslynAssert.FixAll(analyzer, fix, expectedDiagnostic, before, after);
                RoslynAssert.FixAll(analyzer, fix, expectedDiagnostic, new[] { before }, after);
                RoslynAssert.FixAll(analyzer, fix, expectedDiagnostic, before, after, fixTitle: "Rename to: 'f'");
                RoslynAssert.FixAll(analyzer, fix, expectedDiagnostic, new[] { before }, after, fixTitle: "Rename to: 'f'");
            }

            [Test]
            public static void SingleDocumentOneDiagnosticCorrectFixExplicitTitleExpectedDiagnosticWithPositionAnalyzerSupportsTwoDiagnostics2()
            {
                var before = @"
namespace N
{
    class C
    {
        private readonly int ↓_f = 1;

        public int M() => _f;
    }
}";

                var after = @"
namespace N
{
    class C
    {
        private readonly int f = 1;

        public int M() => f;
    }
}";
                var expectedDiagnostic = ExpectedDiagnostic.Create(FieldNameMustNotBeginWithUnderscoreDifferentDiagnosticsForPublic.Id2);
                var analyzer = new FieldNameMustNotBeginWithUnderscoreDifferentDiagnosticsForPublic();
                var fix = new DoNotUseUnderscoreFix();
                RoslynAssert.FixAll(analyzer, fix, expectedDiagnostic, before, after);
                RoslynAssert.FixAll(analyzer, fix, expectedDiagnostic, new[] { before }, after);
                RoslynAssert.FixAll(analyzer, fix, expectedDiagnostic, before, after, fixTitle: "Rename to: 'f'");
                RoslynAssert.FixAll(analyzer, fix, expectedDiagnostic, new[] { before }, after, fixTitle: "Rename to: 'f'");
            }

            [Test]
            public static void TwoDocumentsTwoDiagnosticsTwoFixes()
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
            public static void TwoDocumentsOneDiagnosticWhenCodeFixProviderHasManyFixes(string fixTitle, string expected)
            {
                var before1 = @"
namespace N
{
    class C1
    {
        private readonly int ↓_value = 1;

        public int M() => _value;
    }
}";
                var before2 = @"
namespace N
{
    class C2
    {
        private readonly int value2 = 2;

        public int M() => this.value2;
    }
}";

                var after = @"
namespace N
{
    class C1
    {
        private readonly int value = 1;

        public int M() => value;
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
        private readonly int ↓_value = 1;

        public int M1() => _value;
    }
}";
                var before2 = @"
namespace N
{
    class C2
    {
        private readonly int f2 = 2;

        public int M2() => this.f2;
    }
}";

                var after = @"
namespace N
{
    class C1
    {
        private readonly int value = 1;

        public int M1() => value;
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
        private readonly int ↓_value = 1;

        public int M1() => _value;
    }
}";
                var before2 = @"
namespace N
{
    class C2
    {
        private readonly int ↓_value = 2;

        public int M2() => _value;
    }
}";

                var after1 = @"
namespace N
{
    class C1
    {
        private readonly int value = 1;

        public int M1() => value;
    }
}".AssertReplace("value", expected);

                var after2 = @"
namespace N
{
    class C2
    {
        private readonly int value = 2;

        public int M2() => value;
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
        public event EventHandler? ↓E;
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
            public static void TwoClassOneDiagnostic()
            {
                var barCode = @"
namespace N
{
    class C1
    {
        private readonly int f1 = 1;

        public int M1() => this.f1;
    }
}";

                var before = @"
namespace N
{
    class C2
    {
        private readonly int ↓_value = 2;

        public int M2() => _value;
    }
}";

                var after = @"
namespace N
{
    class C2
    {
        private readonly int value = 2;

        public int M2() => value;
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
        public EventHandler? M() => null;
    }
}";
                var analyzer = new ClassMustHaveMethodAnalyzer();
                var fix = InsertMethodFix.ReturnEventHandler;
                RoslynAssert.FixAll(analyzer, fix, before, after, settings: Settings.Default.WithAllowedCompilerDiagnostics(AllowedCompilerDiagnostics.WarningsAndErrors));
            }

            [Test]
            public static void WithExpectedDiagnosticWhenOneReportsDiagnostic()
            {
                var before = @"
namespace N
{
    class Value
    {
        private readonly int ↓wrongName = 1;
        
        public int WrongName => this.wrongName;
    }
}";

                var after = @"
namespace N
{
    class Value
    {
        private readonly int value = 1;
        
        public int WrongName => this.value;
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
