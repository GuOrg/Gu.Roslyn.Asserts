// ReSharper disable RedundantNameQualifier
namespace Gu.Roslyn.Asserts.Tests.RoslynAssertTests
{
    using System.IO;
    using Gu.Roslyn.Asserts.Tests.CodeFixes;
    using NUnit.Framework;

    [TestFixture]
    public static partial class CodeFix
    {
        public static class Success
        {
            [Test]
            public static void SingleDocumentOneDiagnostic()
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
                RoslynAssert.CodeFix(analyzer, fix, new[] { before }, after);
                RoslynAssert.CodeFix(analyzer, fix, new[] { before }, new[] { after });
                var expectedDiagnostic = ExpectedDiagnostic.Create(FieldNameMustNotBeginWithUnderscore.Descriptor);
                RoslynAssert.CodeFix(analyzer, fix, expectedDiagnostic, new[] { before }, after);
                RoslynAssert.CodeFix(analyzer, fix, expectedDiagnostic, new[] { before }, new[] { after });
            }

            [Test]
            public static void SingleDocumentOneDiagnosticSuppressedWarnings()
            {
                var before = @"
namespace N
{
    class C
    {
        private readonly int ↓_f;
    }
}";

                var after = @"
namespace N
{
    class C
    {
        private readonly int f;
    }
}";
                var analyzer = new FieldNameMustNotBeginWithUnderscore();
                var fix = new DoNotUseUnderscoreFix();
                var settings = Settings.Default.WithCompilationOptions(x => x.WithSuppressedDiagnostics("CS0169"));
                RoslynAssert.CodeFix(analyzer, fix, new[] { before }, after, settings: settings);
                RoslynAssert.CodeFix(analyzer, fix, new[] { before }, new[] { after }, settings: settings);
                var expectedDiagnostic = ExpectedDiagnostic.Create(FieldNameMustNotBeginWithUnderscore.Descriptor);
                RoslynAssert.CodeFix(analyzer, fix, expectedDiagnostic, new[] { before }, after, settings: settings);
                RoslynAssert.CodeFix(analyzer, fix, expectedDiagnostic, new[] { before }, new[] { after }, settings: settings);
            }

            [Test]
            public static void SingleDocumentOneDiagnosticAllowWarnings()
            {
                var before = @"
namespace N
{
    class C
    {
        private readonly int ↓_f;
    }
}";

                var after = @"
namespace N
{
    class C
    {
        private readonly int f;
    }
}";
                var analyzer = new FieldNameMustNotBeginWithUnderscore();
                var fix = new DoNotUseUnderscoreFix();
                var settings = Settings.Default.WithAllowedCompilationDiagnostics(AllowCompilationDiagnostics.Warnings);
                RoslynAssert.CodeFix(analyzer, fix, new[] { before }, after, settings: settings);
                RoslynAssert.CodeFix(analyzer, fix, new[] { before }, new[] { after }, settings: settings);
                var expectedDiagnostic = ExpectedDiagnostic.Create(FieldNameMustNotBeginWithUnderscore.Descriptor);
                RoslynAssert.CodeFix(analyzer, fix, expectedDiagnostic, new[] { before }, after, settings: settings);
                RoslynAssert.CodeFix(analyzer, fix, expectedDiagnostic, new[] { before }, new[] { after }, settings: settings);
            }

            [Test]
            public static void MakeSealed()
            {
                var before = @"
namespace N
{
    ↓public class C
    {
    }
}";

                var after = @"
namespace N
{
    public sealed class C
    {
    }
}";
                var analyzer = new ClassMustBeSealedAnalyzer();
                var fix = new MakeSealedFixProvider();
                RoslynAssert.CodeFix(analyzer, fix, new[] { before }, after);
                RoslynAssert.CodeFix(analyzer, fix, new[] { before }, new[] { after });
                var expectedDiagnostic = ExpectedDiagnostic.Create(ClassMustBeSealedAnalyzer.DiagnosticId);
                RoslynAssert.CodeFix(analyzer, fix, expectedDiagnostic, new[] { before }, after);
                RoslynAssert.CodeFix(analyzer, fix, expectedDiagnostic, new[] { before }, new[] { after });
            }

            [Test]
            public static void MakeSealedCorrectFixKeepsPoorFormat()
            {
                var before = @"
namespace    N
{
    ↓public class C
    {
private readonly int value;
                
            public C(int value)
{
        this.value  =  value;
        }
    }
}";

                var after = @"
namespace    N
{
    public sealed class C
    {
private readonly int value;
                
            public C(int value)
{
        this.value  =  value;
        }
    }
}";
                var analyzer = new ClassMustBeSealedAnalyzer();
                var fix = new MakeSealedFixProvider();
                RoslynAssert.CodeFix(analyzer, fix, new[] { before }, after);
                RoslynAssert.CodeFix(analyzer, fix, new[] { before }, new[] { after });
                var expectedDiagnostic = ExpectedDiagnostic.Create(ClassMustBeSealedAnalyzer.DiagnosticId);
                RoslynAssert.CodeFix(analyzer, fix, expectedDiagnostic, new[] { before }, after);
                RoslynAssert.CodeFix(analyzer, fix, expectedDiagnostic, new[] { before }, new[] { after });
            }

            [Test]
            public static void SingleDocumentOneErrorCorrectFixExplicitTitle()
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
                RoslynAssert.CodeFix(analyzer, fix, new[] { before }, after);
                RoslynAssert.CodeFix(analyzer, fix, new[] { before }, new[] { after });
                var expectedDiagnostic = ExpectedDiagnostic.Create(FieldNameMustNotBeginWithUnderscore.Descriptor);
                RoslynAssert.CodeFix(analyzer, fix, expectedDiagnostic, new[] { before }, after);
                RoslynAssert.CodeFix(analyzer, fix, expectedDiagnostic, new[] { before }, new[] { after });
            }

            [Test]
            public static void SingleDocumentOneErrorCorrectFixExplicitTitleExpectedDiagnosticWithErrorsIndicated()
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
                var expectedDiagnostic = ExpectedDiagnostic.Create(FieldNameMustNotBeginWithUnderscore.Descriptor);
                var analyzer = new FieldNameMustNotBeginWithUnderscore();
                var fix = new DoNotUseUnderscoreFix();
                RoslynAssert.CodeFix(analyzer, fix, expectedDiagnostic, before, after, "Rename to: 'f'");
                RoslynAssert.CodeFix(analyzer, fix, expectedDiagnostic, new[] { before }, after, "Rename to: 'f'");
                RoslynAssert.CodeFix(analyzer, fix, new[] { before }, after, fixTitle: "Rename to: 'f'");
                RoslynAssert.CodeFix(analyzer, fix, new[] { before }, new[] { after }, fixTitle: "Rename to: 'f'");
                RoslynAssert.CodeFix(analyzer, fix, expectedDiagnostic, new[] { before }, after, fixTitle: "Rename to: 'f'");
                RoslynAssert.CodeFix(analyzer, fix, expectedDiagnostic, new[] { before }, new[] { after }, fixTitle: "Rename to: 'f'");
            }

            [Test]
            public static void SingleDocumentOneErrorCorrectFixExplicitTitleExpectedDiagnosticWithPosition()
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
                var expectedDiagnostic = ExpectedDiagnostic.CreateFromCodeWithErrorsIndicated(FieldNameMustNotBeginWithUnderscore.DiagnosticId, before, out before);
                var analyzer = new FieldNameMustNotBeginWithUnderscore();
                var fix = new DoNotUseUnderscoreFix();
                RoslynAssert.CodeFix(analyzer, fix, expectedDiagnostic, before, after, "Rename to: 'f'");
                RoslynAssert.CodeFix(analyzer, fix, expectedDiagnostic, new[] { before }, after, "Rename to: 'f'");
                RoslynAssert.CodeFix(analyzer, fix, expectedDiagnostic, new[] { before }, after, fixTitle: "Rename to: 'f'");
                RoslynAssert.CodeFix(analyzer, fix, expectedDiagnostic, new[] { before }, new[] { after }, fixTitle: "Rename to: 'f'");
            }

            [Test]
            public static void SingleDocumentOneErrorCorrectFixExplicitTitleExpectedDiagnosticWithPositionAnalyzerSupportsTwoDiagnostics()
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
                RoslynAssert.CodeFix(analyzer, fix, expectedDiagnostic, before, after, "Rename to: 'f'");
                RoslynAssert.CodeFix(analyzer, fix, expectedDiagnostic, new[] { before }, after, "Rename to: 'f'");
                RoslynAssert.CodeFix(analyzer, fix, expectedDiagnostic, new[] { before }, after, fixTitle: "Rename to: 'f'");
                RoslynAssert.CodeFix(analyzer, fix, expectedDiagnostic, new[] { before }, new[] { after }, fixTitle: "Rename to: 'f'");
            }

            [TestCase("Rename to: 'f1'", "f1")]
            [TestCase("Rename to: 'f2'", "f2")]
            public static void SingleDocumentOneErrorTwoFixes(string fixTitle, string expected)
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
                RoslynAssert.CodeFix(analyzer, fix, before, after, fixTitle);
                RoslynAssert.CodeFix(analyzer, fix, new[] { before }, after, fixTitle: fixTitle);
                RoslynAssert.CodeFix(analyzer, fix, new[] { before }, new[] { after }, fixTitle: fixTitle);
                var expectedDiagnostic = ExpectedDiagnostic.Create(FieldNameMustNotBeginWithUnderscore.Descriptor);
                RoslynAssert.CodeFix(analyzer, fix, expectedDiagnostic, before, after, fixTitle);
                RoslynAssert.CodeFix(analyzer, fix, expectedDiagnostic, new[] { before }, after, fixTitle: fixTitle);
                RoslynAssert.CodeFix(analyzer, fix, expectedDiagnostic, new[] { before }, new[] { after }, fixTitle: fixTitle);
            }

            [Test]
            public static void SingleDocumentTwoFixableErrorsFilterByTitle()
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
        private readonly int _f2 = 2;

        public int M() => f1 + _f2;
    }
}";
                var analyzer = new FieldNameMustNotBeginWithUnderscore();
                var fix = new DoNotUseUnderscoreFix();
                RoslynAssert.CodeFix(analyzer, fix, new[] { before }, after, fixTitle: "Rename to: 'f1'");

                after = @"
namespace N
{
    class C
    {
        private readonly int _f1 = 1;
        private readonly int f2 = 2;

        public int M() => _f1 + f2;
    }
}";
                RoslynAssert.CodeFix(analyzer, fix, new[] { before }, after, fixTitle: "Rename to: 'f2'");
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
                var expectedDiagnostic = ExpectedDiagnostic.Create("CS0067");
                var fix = new RemoveUnusedFix();
                RoslynAssert.CodeFix(fix, expectedDiagnostic, before, after);
                RoslynAssert.CodeFix(fix, expectedDiagnostic, new[] { before }, after);
                RoslynAssert.CodeFix(fix, expectedDiagnostic, new[] { before }, new[] { after });
                RoslynAssert.CodeFix(fix, expectedDiagnostic, before, after, fixTitle: "Remove public event EventHandler? E;");
                RoslynAssert.CodeFix(fix, expectedDiagnostic, new[] { before }, after, fixTitle: "Remove public event EventHandler? E;");
                RoslynAssert.CodeFix(fix, expectedDiagnostic, new[] { before }, new[] { after }, fixTitle: "Remove public event EventHandler? E;");
            }

            [Test]
            public static void TwoDocumentsCodeFixOnly()
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
    public class C2
    {
    }
}";

                var after = @"
namespace N
{
    using System;

    public class C1
    {
    }
}";
                var expectedDiagnostic = ExpectedDiagnostic.Create("CS0067");
                var fix = new RemoveUnusedFix();
                RoslynAssert.CodeFix(fix, expectedDiagnostic, new[] { before2, before1 }, after);
                RoslynAssert.CodeFix(fix, expectedDiagnostic, new[] { before2, before1 }, new[] { after, before2 });
                RoslynAssert.CodeFix(fix, expectedDiagnostic, new[] { before1, before2 }, after);
                RoslynAssert.CodeFix(fix, expectedDiagnostic, new[] { before1, before2 }, new[] { after, before2 });
            }

            [Test]
            public static void PartialTwoDocumentsCodeFixOnly()
            {
                var before = @"
namespace N
{
    using System;

    public partial class C
    {
        public event EventHandler? ↓E;
    }
}";

                var part2 = @"
namespace N
{
    public partial class C
    {
    }
}";

                var after = @"
namespace N
{
    using System;

    public partial class C
    {
    }
}";
                var expectedDiagnostic = ExpectedDiagnostic.Create("CS0067");
                var fix = new RemoveUnusedFix();
                RoslynAssert.CodeFix(fix, expectedDiagnostic, new[] { part2, before }, after);
                RoslynAssert.CodeFix(fix, expectedDiagnostic, new[] { part2, before }, new[] { after, part2 });
                RoslynAssert.CodeFix(fix, expectedDiagnostic, new[] { before, part2 }, after);
                RoslynAssert.CodeFix(fix, expectedDiagnostic, new[] { before, part2 }, new[] { part2, after });
            }

            [Test]
            public static void PartialTwoDocumentsOneFix()
            {
                var before = @"
namespace N
{
    public partial class C
    {
        private int ↓_f = 1;

        public int M() => _f;
    }
}";

                var part2 = @"
namespace N
{
    public partial class C
    {
    }
}";

                var after = @"
namespace N
{
    public partial class C
    {
        private int f = 1;

        public int M() => f;
    }
}";
                var expectedDiagnostic = ExpectedDiagnostic.Create(FieldNameMustNotBeginWithUnderscore.Descriptor);
                var analyzer = new FieldNameMustNotBeginWithUnderscore();
                var fix = new DoNotUseUnderscoreFix();
                RoslynAssert.CodeFix(analyzer, fix, new[] { before, part2 }, after);
                RoslynAssert.CodeFix(analyzer, fix, new[] { part2, before }, after);
                RoslynAssert.CodeFix(analyzer, fix, new[] { part2, before }, new[] { after, part2 });
                RoslynAssert.CodeFix(analyzer, fix, new[] { part2, before }, new[] { part2, after });
                RoslynAssert.CodeFix(analyzer, fix, expectedDiagnostic, new[] { part2, before }, after);
                RoslynAssert.CodeFix(analyzer, fix, expectedDiagnostic, new[] { before, part2 }, after);
                RoslynAssert.CodeFix(analyzer, fix, expectedDiagnostic, new[] { part2, before }, new[] { after, part2 });
                RoslynAssert.CodeFix(analyzer, fix, expectedDiagnostic, new[] { part2, before }, new[] { part2, after });
                RoslynAssert.CodeFix(analyzer, fix, expectedDiagnostic, new[] { before, part2 }, after, fixTitle: "Rename to: 'f'");
                RoslynAssert.CodeFix(analyzer, fix, expectedDiagnostic, new[] { before, part2 }, new[] { part2, after }, fixTitle: "Rename to: 'f'");
                RoslynAssert.CodeFix(analyzer, fix, expectedDiagnostic, new[] { before, part2 }, new[] { after, part2 }, fixTitle: "Rename to: 'f'");
            }

            [Test]
            public static void PartialTwoDocumentsOneFixWhenSpansMatch()
            {
                var before = @"
namespace N
{
    public partial class C
    {
        private int ↓_f1 = 1;

        public int M() => _f1 + this.f2;
    }
}";

                var part2 = @"
namespace N
{
    public partial class C
    {
        private int f2 = 2;
    }
}";

                var after = @"
namespace N
{
    public partial class C
    {
        private int f1 = 1;

        public int M() => f1 + this.f2;
    }
}";
                var expectedDiagnostic = ExpectedDiagnostic.Create(FieldNameMustNotBeginWithUnderscore.Descriptor);
                var analyzer = new FieldNameMustNotBeginWithUnderscore();
                var fix = new DoNotUseUnderscoreFix();
                RoslynAssert.CodeFix(analyzer, fix, new[] { before, part2 }, after);
                RoslynAssert.CodeFix(analyzer, fix, new[] { part2, before }, after);
                RoslynAssert.CodeFix(analyzer, fix, new[] { part2, before }, new[] { after, part2 });
                RoslynAssert.CodeFix(analyzer, fix, new[] { part2, before }, new[] { part2, after });
                RoslynAssert.CodeFix(analyzer, fix, expectedDiagnostic, new[] { part2, before }, after);
                RoslynAssert.CodeFix(analyzer, fix, expectedDiagnostic, new[] { before, part2 }, after);
                RoslynAssert.CodeFix(analyzer, fix, expectedDiagnostic, new[] { part2, before }, new[] { after, part2 });
                RoslynAssert.CodeFix(analyzer, fix, expectedDiagnostic, new[] { part2, before }, new[] { part2, after });
                RoslynAssert.CodeFix(analyzer, fix, expectedDiagnostic, new[] { before, part2 }, after, fixTitle: "Rename to: 'f1'");
                RoslynAssert.CodeFix(analyzer, fix, expectedDiagnostic, new[] { before, part2 }, new[] { part2, after }, fixTitle: "Rename to: 'f1'");
                RoslynAssert.CodeFix(analyzer, fix, expectedDiagnostic, new[] { before, part2 }, new[] { after, part2 }, fixTitle: "Rename to: 'f1'");
            }

            [Test]
            public static void TwoDocumentsDifferentProjectsCodeFixOnly()
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
                var expectedDiagnostic = ExpectedDiagnostic.Create("CS0067");
                var fix = new RemoveUnusedFix();
                RoslynAssert.CodeFix(fix, expectedDiagnostic, new[] { before2, before1 }, after);
                RoslynAssert.CodeFix(fix, expectedDiagnostic, new[] { before2, before1 }, new[] { after, before2 });
                RoslynAssert.CodeFix(fix, expectedDiagnostic, new[] { before1, before2 }, after);
                RoslynAssert.CodeFix(fix, expectedDiagnostic, new[] { before1, before2 }, new[] { after, before2 });

                expectedDiagnostic = ExpectedDiagnostic.CreateFromCodeWithErrorsIndicated("CS0067", before1, out before1);
                RoslynAssert.CodeFix(fix, expectedDiagnostic, new[] { before2, before1 }, after);
                RoslynAssert.CodeFix(fix, expectedDiagnostic, new[] { before2, before1 }, new[] { after, before2 });
                RoslynAssert.CodeFix(fix, expectedDiagnostic, new[] { before1, before2 }, after);
                RoslynAssert.CodeFix(fix, expectedDiagnostic, new[] { before1, before2 }, new[] { after, before2 });
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
                var expectedDiagnostic = ExpectedDiagnostic.Create("CS0067");
                var fix = new RemoveUnusedFix();
                RoslynAssert.CodeFix(fix, expectedDiagnostic, new[] { before2, before1 }, after);
                RoslynAssert.CodeFix(fix, expectedDiagnostic, new[] { before2, before1 }, new[] { after, before2 });
                RoslynAssert.CodeFix(fix, expectedDiagnostic, new[] { before1, before2 }, after);
                RoslynAssert.CodeFix(fix, expectedDiagnostic, new[] { before1, before2 }, new[] { after, before2 });

                expectedDiagnostic = ExpectedDiagnostic.CreateFromCodeWithErrorsIndicated("CS0067", before1, out before1);
                RoslynAssert.CodeFix(fix, expectedDiagnostic, new[] { before2, before1 }, after);
                RoslynAssert.CodeFix(fix, expectedDiagnostic, new[] { before2, before1 }, new[] { after, before2 });
                RoslynAssert.CodeFix(fix, expectedDiagnostic, new[] { before1, before2 }, after);
                RoslynAssert.CodeFix(fix, expectedDiagnostic, new[] { before1, before2 }, new[] { after, before2 });
            }

            [Test]
            public static void TwoDocumentsDifferentProjectsInheritingCodeFixOnly2()
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
        public event EventHandler? ↓E;
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
                var expectedDiagnostic = ExpectedDiagnostic.Create("CS0067");
                var fix = new RemoveUnusedFix();
                RoslynAssert.CodeFix(fix, expectedDiagnostic, new[] { before2, before1 }, after);
                RoslynAssert.CodeFix(fix, expectedDiagnostic, new[] { before1, before2 }, after);
                RoslynAssert.CodeFix(fix, expectedDiagnostic, new[] { before2, before1 }, new[] { after, before1 });
                RoslynAssert.CodeFix(fix, expectedDiagnostic, new[] { before2, before1 }, new[] { before1, after });
                RoslynAssert.CodeFix(fix, expectedDiagnostic, new[] { before1, before2 }, new[] { after, before1 });
                RoslynAssert.CodeFix(fix, expectedDiagnostic, new[] { before1, before2 }, new[] { before1, after });
            }

            [Test]
            public static void TwoDocumentsOneDiagnostic()
            {
                var barCode = @"
namespace N
{
    class C1
    {
        private readonly int f1 = 1;

        public int M() => f1;
    }
}";

                var before = @"
namespace N
{
    class C2
    {
        private readonly int ↓_f2 = 2;

        public int M() => _f2;
    }
}";

                var after = @"
namespace N
{
    class C2
    {
        private readonly int f2 = 2;

        public int M() => f2;
    }
}";
                var analyzer = new FieldNameMustNotBeginWithUnderscore();
                var fix = new DoNotUseUnderscoreFix();
                RoslynAssert.CodeFix(analyzer, fix, new[] { barCode, before }, after);
                RoslynAssert.CodeFix(analyzer, fix, new[] { barCode, before }, new[] { barCode, after });
            }

            [Test]
            public static void TwoDocumentsOneErrorFixTouchingBothDocuments()
            {
                var before1 = @"
namespace N
{
    class C1
    {
        public int ↓WrongName { get; }
    }
}";

                var before2 = @"
namespace N
{
    class C2
    {
        public C2(C1 c1)
        {
            var x = c1.WrongName;
        }
    }
}";

                var after1 = @"
namespace N
{
    class C1
    {
        public int Value { get; }
    }
}";

                var after2 = @"
namespace N
{
    class C2
    {
        public C2(C1 c1)
        {
            var x = c1.Value;
        }
    }
}";
                var analyzer = new PropertyMustBeNamedValueAnalyzer();
                var fix = new RenameToValueFix();
                RoslynAssert.CodeFix(analyzer, fix, new[] { before1, before2 }, new[] { after1, after2 });
                RoslynAssert.CodeFix(analyzer, fix, new[] { before2, before1 }, new[] { after1, after2 });
                RoslynAssert.CodeFix(analyzer, fix, new[] { before1, before2 }, new[] { after2, after1 });
                RoslynAssert.CodeFix(analyzer, fix, new[] { before2, before1 }, new[] { after2, after1 });

                RoslynAssert.CodeFix(analyzer, fix, new[] { before1, before2 }, new[] { after1, after2 }, fixTitle: "Rename to: Value");
                RoslynAssert.CodeFix(analyzer, fix, new[] { before2, before1 }, new[] { after1, after2 }, fixTitle: "Rename to: Value");
                RoslynAssert.CodeFix(analyzer, fix, new[] { before1, before2 }, new[] { after2, after1 }, fixTitle: "Rename to: Value");
                RoslynAssert.CodeFix(analyzer, fix, new[] { before2, before1 }, new[] { after2, after1 }, fixTitle: "Rename to: Value");
            }

            [Test]
            public static void CodeFixAddingDocument()
            {
                var before = @"
namespace N
{
    class C
    {
        public static C Create() => ↓new C();
    }
}";

                var after = @"
namespace N
{
    class C
    {
        public static C Create() => new C().Id();
    }
}";

                var extensionMethodCode = @"namespace N
{
    public static class Extensions
    {
        public static T Id<T>(this T t) => t;
    }
}";
                var analyzer = new CallIdAnalyzer();
                var fix = new CallIdFix();
                RoslynAssert.CodeFix(analyzer, fix, new[] { before }, new[] { after, extensionMethodCode });
                RoslynAssert.CodeFix(analyzer, fix, new[] { before }, new[] { after, extensionMethodCode }, fixTitle: "Call ID()");
            }

            [Test]
            public static void WhenFixIntroducesCompilerErrorsThatAreAccepted()
            {
                var before = @"
namespace N
{
    ↓class C
    {
    }
}";

                var after = @"
namespace N
{
    class C
    {
        public EventHandler? M() => null;
    }
}";
                var analyzer = new ClassMustHaveMethodAnalyzer();
                var fix = CodeFixes.InsertMethodFix.ReturnEventHandler;
                var expectedDiagnostic = ExpectedDiagnostic.Create(ClassMustHaveMethodAnalyzer.DiagnosticId);
                var settings = Settings.Default.WithAllowedCompilationDiagnostics(AllowCompilationDiagnostics.WarningsAndErrors);
                RoslynAssert.CodeFix(analyzer, fix, before, after, settings: settings);
                RoslynAssert.CodeFix(analyzer, fix, new[] { before }, after, settings: settings);
                RoslynAssert.CodeFix(analyzer, fix, new[] { before }, new[] { after }, settings: settings);
                RoslynAssert.CodeFix(analyzer, fix, expectedDiagnostic, before, after, settings: settings);
                RoslynAssert.CodeFix(analyzer, fix, expectedDiagnostic, new[] { before }, after, settings: settings);
                RoslynAssert.CodeFix(analyzer, fix, expectedDiagnostic, new[] { before }, new[] { after }, settings: settings);
            }

            [Test]
            public static void WithExpectedDiagnosticWhenOneReportsDiagnostic()
            {
                var before = @"
namespace N
{
    class C
    {
        private readonly int ↓wrongName = 1;
        
        public int WrongName => this.wrongName;
    }
}";

                var after = @"
namespace N
{
    class C
    {
        private readonly int value = 1;
        
        public int WrongName => this.value;
    }
}";
                var expectedDiagnostic = ExpectedDiagnostic.Create(FieldAndPropertyMustBeNamedValueAnalyzer.FieldDiagnosticId);
                var analyzer = new FieldAndPropertyMustBeNamedValueAnalyzer();
                var fix = new RenameToValueFix();
                RoslynAssert.CodeFix(analyzer, fix, expectedDiagnostic, before, after);
                RoslynAssert.CodeFix(analyzer, fix, expectedDiagnostic, new[] { before }, after);
                RoslynAssert.CodeFix(analyzer, fix, expectedDiagnostic, new[] { before }, new[] { after });
            }

            [Test]
            public static void InsertMethodFix()
            {
                var before = @"
namespace N
{
    using System;

    ↓class C
    {
    }
}";

                var after = @"
namespace N
{
    using System;

    class C
    {
        public EventHandler? M() => null;
    }
}";
                var analyzer = new ClassMustHaveMethodAnalyzer();
                var fix = CodeFixes.InsertMethodFix.ReturnEventHandler;
                RoslynAssert.CodeFix(analyzer, fix, before, after);
                RoslynAssert.CodeFix(analyzer, fix, new[] { before }, after);
                RoslynAssert.CodeFix(analyzer, fix, new[] { before }, new[] { after });
            }

            [Test]
            public static void InsertFullyQualifiedMethodFix()
            {
                var before = @"
namespace N
{
    ↓class C
    {
    }
}";

                var after = @"
namespace N
{
    class C
    {
        public System.EventHandler? M() => null;
    }
}";
                var analyzer = new ClassMustHaveMethodAnalyzer();
                var fix = CodeFixes.InsertMethodFix.ReturnFullyQualifiedEventHandler;
                RoslynAssert.CodeFix(analyzer, fix, before, after);
                RoslynAssert.CodeFix(analyzer, fix, new[] { before }, after);
                RoslynAssert.CodeFix(analyzer, fix, new[] { before }, new[] { after });
            }

            [Test]
            public static void InsertFullyQualifiedSimplifiedMethodFix()
            {
                var before = @"
namespace N
{
    ↓class C
    {
    }
}";

                var after = @"
namespace N
{
    class C
    {
        public System.EventHandler? M() => null;
    }
}";
                var analyzer = new ClassMustHaveMethodAnalyzer();
                var fix = CodeFixes.InsertMethodFix.ReturnFullyQualifiedSimplifiedEventHandler;
                RoslynAssert.CodeFix(analyzer, fix, before, after);
                RoslynAssert.CodeFix(analyzer, fix, new[] { before }, after);
                RoslynAssert.CodeFix(analyzer, fix, new[] { before }, new[] { after });
            }

            [Test]
            public static void InsertFullyQualifiedSimplifiedEventFixWhenUsing()
            {
                var before = @"
namespace N
{
    using System;

    ↓class C
    {
    }
}";

                var after = @"
namespace N
{
    using System;

    class C
    {
        public System.EventHandler? M() => null;
    }
}";
                var analyzer = new ClassMustHaveMethodAnalyzer();
                var fix = CodeFixes.InsertMethodFix.ReturnFullyQualifiedEventHandler;
                RoslynAssert.CodeFix(analyzer, fix, before, after);
                RoslynAssert.CodeFix(analyzer, fix, new[] { before }, after);
                RoslynAssert.CodeFix(analyzer, fix, new[] { before }, new[] { after });
            }

            [Test]
            public static void ProjectFromDisk()
            {
                var after = @"// ReSharper disable All
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
                    5,
                    20);
                var solution = CodeFactory.CreateSolution(csproj);
                RoslynAssert.CodeFix(analyzer, new DoNotUseUnderscoreFix(), expectedDiagnostic, solution, after);
            }
        }
    }
}
