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
            public static void SingleDocumentOneError()
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
                RoslynAssert.CodeFix(analyzer, fix, new[] { before }, after);
                RoslynAssert.CodeFix(analyzer, fix, new[] { before }, new[] { after });
                var expectedDiagnostic = ExpectedDiagnostic.Create(FieldNameMustNotBeginWithUnderscore.Descriptor);
                RoslynAssert.CodeFix(analyzer, fix, expectedDiagnostic, new[] { before }, after);
                RoslynAssert.CodeFix(analyzer, fix, expectedDiagnostic, new[] { before }, new[] { after });
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
                var expectedDiagnostic = ExpectedDiagnostic.Create(FieldNameMustNotBeginWithUnderscore.Descriptor);
                var analyzer = new FieldNameMustNotBeginWithUnderscore();
                var fix = new DoNotUseUnderscoreFix();
                RoslynAssert.CodeFix(analyzer, fix, expectedDiagnostic, before, after, "Rename to: 'value'");
                RoslynAssert.CodeFix(analyzer, fix, expectedDiagnostic, new[] { before }, after, "Rename to: 'value'");
                RoslynAssert.CodeFix(analyzer, fix, new[] { before }, after, fixTitle: "Rename to: 'value'");
                RoslynAssert.CodeFix(analyzer, fix, new[] { before }, new[] { after }, fixTitle: "Rename to: 'value'");
                RoslynAssert.CodeFix(analyzer, fix, expectedDiagnostic, new[] { before }, after, fixTitle: "Rename to: 'value'");
                RoslynAssert.CodeFix(analyzer, fix, expectedDiagnostic, new[] { before }, new[] { after }, fixTitle: "Rename to: 'value'");
            }

            [Test]
            public static void SingleDocumentOneErrorCorrectFixExplicitTitleExpectedDiagnosticWithPosition()
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
                var expectedDiagnostic = ExpectedDiagnostic.CreateFromCodeWithErrorsIndicated(FieldNameMustNotBeginWithUnderscore.DiagnosticId, before, out before);
                var analyzer = new FieldNameMustNotBeginWithUnderscore();
                var fix = new DoNotUseUnderscoreFix();
                RoslynAssert.CodeFix(analyzer, fix, expectedDiagnostic, before, after, "Rename to: 'value'");
                RoslynAssert.CodeFix(analyzer, fix, expectedDiagnostic, new[] { before }, after, "Rename to: 'value'");
                RoslynAssert.CodeFix(analyzer, fix, expectedDiagnostic, new[] { before }, after, fixTitle: "Rename to: 'value'");
                RoslynAssert.CodeFix(analyzer, fix, expectedDiagnostic, new[] { before }, new[] { after }, fixTitle: "Rename to: 'value'");
            }

            [Test]
            public static void SingleDocumentOneErrorCorrectFixExplicitTitleExpectedDiagnosticWithPositionAnalyzerSupportsTwoDiagnostics()
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
                RoslynAssert.CodeFix(analyzer, fix, expectedDiagnostic, before, after, "Rename to: 'value'");
                RoslynAssert.CodeFix(analyzer, fix, expectedDiagnostic, new[] { before }, after, "Rename to: 'value'");
                RoslynAssert.CodeFix(analyzer, fix, expectedDiagnostic, new[] { before }, after, fixTitle: "Rename to: 'value'");
                RoslynAssert.CodeFix(analyzer, fix, expectedDiagnostic, new[] { before }, new[] { after }, fixTitle: "Rename to: 'value'");
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
        private readonly int _value2;
    }
}";
                var analyzer = new FieldNameMustNotBeginWithUnderscore();
                var fix = new DoNotUseUnderscoreFix();
                RoslynAssert.CodeFix(analyzer, fix, new[] { before }, after, fixTitle: "Rename to: 'value1'");

                after = @"
namespace N
{
    class C
    {
        private readonly int _value1;
        private readonly int value2;
    }
}";
                RoslynAssert.CodeFix(analyzer, fix, new[] { before }, after, fixTitle: "Rename to: 'value2'");
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
        private int ↓_value;
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
        private int value;
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
                RoslynAssert.CodeFix(analyzer, fix, expectedDiagnostic, new[] { before, part2 }, after, fixTitle: "Rename to: 'value'");
                RoslynAssert.CodeFix(analyzer, fix, expectedDiagnostic, new[] { before, part2 }, new[] { part2, after }, fixTitle: "Rename to: 'value'");
                RoslynAssert.CodeFix(analyzer, fix, expectedDiagnostic, new[] { before, part2 }, new[] { after, part2 }, fixTitle: "Rename to: 'value'");
            }

            [Test]
            public static void PartialTwoDocumentsOneFixWhenSpansMatch()
            {
                var before = @"
namespace N
{
    public partial class C
    {
        private int ↓_value1;
    }
}";

                var part2 = @"
namespace N
{
    public partial class C
    {
        private int value2;
    }
}";

                var after = @"
namespace N
{
    public partial class C
    {
        private int value1;
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
                RoslynAssert.CodeFix(analyzer, fix, expectedDiagnostic, new[] { before, part2 }, after, fixTitle: "Rename to: 'value1'");
                RoslynAssert.CodeFix(analyzer, fix, expectedDiagnostic, new[] { before, part2 }, new[] { part2, after }, fixTitle: "Rename to: 'value1'");
                RoslynAssert.CodeFix(analyzer, fix, expectedDiagnostic, new[] { before, part2 }, new[] { after, part2 }, fixTitle: "Rename to: 'value1'");
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
            public static void TwoDocumentsOneError()
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
                var expectedDiagnostic = ExpectedDiagnostic.Create(ClassMustHaveEventAnalyzer.DiagnosticId);
                var settings = Settings.Default.WithAllowedCompilationDiagnostics(AllowCompilationDiagnostics.WarningsAndErrors);
                RoslynAssert.CodeFix(analyzer, fix, before, after, settings: settings);
                RoslynAssert.CodeFix(analyzer, fix, new[] { before }, after, settings: settings);
                RoslynAssert.CodeFix(analyzer, fix, new[] { before }, new[] { after }, settings: settings);
                RoslynAssert.CodeFix(analyzer, fix, expectedDiagnostic, before, after, settings: settings);
                RoslynAssert.CodeFix(analyzer, fix, expectedDiagnostic, new[] { before }, after, settings: settings);
                RoslynAssert.CodeFix(analyzer, fix, expectedDiagnostic, new[] { before }, new[] { after }, settings: settings);
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
                RoslynAssert.CodeFix(analyzer, fix, expectedDiagnostic, before, after);
                RoslynAssert.CodeFix(analyzer, fix, expectedDiagnostic, new[] { before }, after);
                RoslynAssert.CodeFix(analyzer, fix, expectedDiagnostic, new[] { before }, new[] { after });
            }

            [Test]
            public static void InsertEventFix()
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
        public event EventHandler E;
    }
}";
                var analyzer = new ClassMustHaveEventAnalyzer();
                var fix = new InsertEventFix();
                RoslynAssert.CodeFix(analyzer, fix, before, after);
                RoslynAssert.CodeFix(analyzer, fix, new[] { before }, after);
                RoslynAssert.CodeFix(analyzer, fix, new[] { before }, new[] { after });
            }

            [Test]
            public static void InsertFullyQualifiedEventFix()
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
        public event System.EventHandler E;
    }
}";
                var analyzer = new ClassMustHaveEventAnalyzer();
                var fix = new InsertFullyQualifiedEventFix();
                RoslynAssert.CodeFix(analyzer, fix, before, after);
                RoslynAssert.CodeFix(analyzer, fix, new[] { before }, after);
                RoslynAssert.CodeFix(analyzer, fix, new[] { before }, new[] { after });
            }

            [Test]
            public static void InsertFullyQualifiedSimplifiedEventFix()
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
        public event System.EventHandler E;
    }
}";
                var analyzer = new ClassMustHaveEventAnalyzer();
                var fix = new InsertFullyQualifiedSimplifiedEventFix();
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
        public event EventHandler E;
    }
}";
                var analyzer = new ClassMustHaveEventAnalyzer();
                var fix = new InsertFullyQualifiedSimplifiedEventFix();
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
