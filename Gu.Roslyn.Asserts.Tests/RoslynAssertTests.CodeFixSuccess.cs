// ReSharper disable RedundantNameQualifier
namespace Gu.Roslyn.Asserts.Tests
{
    using System;
    using System.IO;
    using Gu.Roslyn.Asserts.Tests.CodeFixes;
    using Microsoft.CodeAnalysis;
    using NUnit.Framework;

    [TestFixture]
    public static partial class RoslynAssertTests
    {
        public static class CodeFixSuccess
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
                RoslynAssert.CodeFix(analyzer, fix, expectedDiagnostic, before, after, "Rename to: value");
                RoslynAssert.CodeFix(analyzer, fix, expectedDiagnostic, new[] { before }, after, "Rename to: value");
                RoslynAssert.CodeFix(analyzer, fix, new[] { before }, after, fixTitle: "Rename to: value");
                RoslynAssert.CodeFix(analyzer, fix, new[] { before }, new[] { after }, fixTitle: "Rename to: value");
                RoslynAssert.CodeFix(analyzer, fix, expectedDiagnostic, new[] { before }, after, fixTitle: "Rename to: value");
                RoslynAssert.CodeFix(analyzer, fix, expectedDiagnostic, new[] { before }, new[] { after }, fixTitle: "Rename to: value");
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
                RoslynAssert.CodeFix(analyzer, fix, expectedDiagnostic, before, after, "Rename to: value");
                RoslynAssert.CodeFix(analyzer, fix, expectedDiagnostic, new[] { before }, after, "Rename to: value");
                RoslynAssert.CodeFix(analyzer, fix, expectedDiagnostic, new[] { before }, after, fixTitle: "Rename to: value");
                RoslynAssert.CodeFix(analyzer, fix, expectedDiagnostic, new[] { before }, new[] { after }, fixTitle: "Rename to: value");
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
                RoslynAssert.CodeFix(analyzer, fix, expectedDiagnostic, before, after, "Rename to: value");
                RoslynAssert.CodeFix(analyzer, fix, expectedDiagnostic, new[] { before }, after, "Rename to: value");
                RoslynAssert.CodeFix(analyzer, fix, expectedDiagnostic, new[] { before }, after, fixTitle: "Rename to: value");
                RoslynAssert.CodeFix(analyzer, fix, expectedDiagnostic, new[] { before }, new[] { after }, fixTitle: "Rename to: value");
            }

            [TestCase("Rename to: value1", "value1")]
            [TestCase("Rename to: value2", "value2")]
            public static void SingleDocumentOneErrorTwoFixes(string title, string expected)
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
                after = after.AssertReplace("value", expected);
                var analyzer = new FieldNameMustNotBeginWithUnderscore();
                var fix = new DontUseUnderscoreManyCodeFixProvider();
                RoslynAssert.CodeFix(analyzer, fix, before, after, title);
                RoslynAssert.CodeFix(analyzer, fix, new[] { before }, after, fixTitle: title);
                RoslynAssert.CodeFix(analyzer, fix, new[] { before }, new[] { after }, fixTitle: title);
                var expectedDiagnostic = ExpectedDiagnostic.Create(FieldNameMustNotBeginWithUnderscore.Descriptor);
                RoslynAssert.CodeFix(analyzer, fix, expectedDiagnostic, before, after, title);
                RoslynAssert.CodeFix(analyzer, fix, expectedDiagnostic, new[] { before }, after, fixTitle: title);
                RoslynAssert.CodeFix(analyzer, fix, expectedDiagnostic, new[] { before }, new[] { after }, fixTitle: title);
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
        public event EventHandler ↓Bar;
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
                RoslynAssert.MetadataReferences.Add(MetadataReference.CreateFromFile(typeof(EventHandler).Assembly.Location));
                var expectedDiagnostic = ExpectedDiagnostic.Create("CS0067");
                var fix = new RemoveUnusedFixProvider();
                RoslynAssert.CodeFix(fix, expectedDiagnostic, before, after);
                RoslynAssert.CodeFix(fix, expectedDiagnostic, new[] { before }, after);
                RoslynAssert.CodeFix(fix, expectedDiagnostic, new[] { before }, new[] { after });
                RoslynAssert.CodeFix(fix, expectedDiagnostic, before, after, fixTitle: "Remove public event EventHandler Bar;");
                RoslynAssert.CodeFix(fix, expectedDiagnostic, new[] { before }, after, fixTitle: "Remove public event EventHandler Bar;");
                RoslynAssert.CodeFix(fix, expectedDiagnostic, new[] { before }, new[] { after }, fixTitle: "Remove public event EventHandler Bar;");
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
        public event EventHandler ↓Bar;
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
                RoslynAssert.MetadataReferences.Add(MetadataReference.CreateFromFile(typeof(EventHandler).Assembly.Location));
                var expectedDiagnostic = ExpectedDiagnostic.Create("CS0067");
                var fix = new RemoveUnusedFixProvider();
                RoslynAssert.CodeFix(fix, expectedDiagnostic, new[] { before2, before1 }, after);
                RoslynAssert.CodeFix(fix, expectedDiagnostic, new[] { before2, before1 }, new[] { after, before2 });
                RoslynAssert.CodeFix(fix, expectedDiagnostic, new[] { before1, before2 }, after);
                RoslynAssert.CodeFix(fix, expectedDiagnostic, new[] { before1, before2 }, new[] { after, before2 });
            }

            [Test]
            public static void PartialTwoDocumentsCodeFixOnly()
            {
                var part1 = @"
namespace N
{
    using System;

    public partial class C
    {
        public event EventHandler ↓Bar;
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
                RoslynAssert.MetadataReferences.Add(MetadataReference.CreateFromFile(typeof(EventHandler).Assembly.Location));
                var expectedDiagnostic = ExpectedDiagnostic.Create("CS0067");
                var fix = new RemoveUnusedFixProvider();
                RoslynAssert.CodeFix(fix, expectedDiagnostic, new[] { part2, part1 }, after);
                RoslynAssert.CodeFix(fix, expectedDiagnostic, new[] { part2, part1 }, new[] { after, part2 });
                RoslynAssert.CodeFix(fix, expectedDiagnostic, new[] { part1, part2 }, after);
                RoslynAssert.CodeFix(fix, expectedDiagnostic, new[] { part1, part2 }, new[] { part2, after });
            }

            [Test]
            public static void PartialTwoDocumentsOneFix()
            {
                var part1 = @"
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
                RoslynAssert.MetadataReferences.Add(MetadataReference.CreateFromFile(typeof(EventHandler).Assembly.Location));
                var expectedDiagnostic = ExpectedDiagnostic.Create(FieldNameMustNotBeginWithUnderscore.Descriptor);
                var analyzer = new FieldNameMustNotBeginWithUnderscore();
                var fix = new DoNotUseUnderscoreFix();
                RoslynAssert.CodeFix(analyzer, fix, new[] { part1, part2 }, after);
                RoslynAssert.CodeFix(analyzer, fix, new[] { part2, part1 }, after);
                RoslynAssert.CodeFix(analyzer, fix, new[] { part2, part1 }, new[] { after, part2 });
                RoslynAssert.CodeFix(analyzer, fix, new[] { part2, part1 }, new[] { part2, after });
                RoslynAssert.CodeFix(analyzer, fix, expectedDiagnostic, new[] { part2, part1 }, after);
                RoslynAssert.CodeFix(analyzer, fix, expectedDiagnostic, new[] { part1, part2 }, after);
                RoslynAssert.CodeFix(analyzer, fix, expectedDiagnostic, new[] { part2, part1 }, new[] { after, part2 });
                RoslynAssert.CodeFix(analyzer, fix, expectedDiagnostic, new[] { part2, part1 }, new[] { part2, after });
                RoslynAssert.CodeFix(analyzer, fix, expectedDiagnostic, new[] { part1, part2 }, after, fixTitle: "Rename to: value");
                RoslynAssert.CodeFix(analyzer, fix, expectedDiagnostic, new[] { part1, part2 }, new[] { part2, after }, fixTitle: "Rename to: value");
                RoslynAssert.CodeFix(analyzer, fix, expectedDiagnostic, new[] { part1, part2 }, new[] { after, part2 }, fixTitle: "Rename to: value");
            }

            [Test]
            public static void PartialTwoDocumentsOneFixWhenSpansMatch()
            {
                var part1 = @"
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
                RoslynAssert.MetadataReferences.Add(MetadataReference.CreateFromFile(typeof(EventHandler).Assembly.Location));
                var expectedDiagnostic = ExpectedDiagnostic.Create(FieldNameMustNotBeginWithUnderscore.Descriptor);
                var analyzer = new FieldNameMustNotBeginWithUnderscore();
                var fix = new DoNotUseUnderscoreFix();
                RoslynAssert.CodeFix(analyzer, fix, new[] { part1, part2 }, after);
                RoslynAssert.CodeFix(analyzer, fix, new[] { part2, part1 }, after);
                RoslynAssert.CodeFix(analyzer, fix, new[] { part2, part1 }, new[] { after, part2 });
                RoslynAssert.CodeFix(analyzer, fix, new[] { part2, part1 }, new[] { part2, after });
                RoslynAssert.CodeFix(analyzer, fix, expectedDiagnostic, new[] { part2, part1 }, after);
                RoslynAssert.CodeFix(analyzer, fix, expectedDiagnostic, new[] { part1, part2 }, after);
                RoslynAssert.CodeFix(analyzer, fix, expectedDiagnostic, new[] { part2, part1 }, new[] { after, part2 });
                RoslynAssert.CodeFix(analyzer, fix, expectedDiagnostic, new[] { part2, part1 }, new[] { part2, after });
                RoslynAssert.CodeFix(analyzer, fix, expectedDiagnostic, new[] { part1, part2 }, after, fixTitle: "Rename to: value1");
                RoslynAssert.CodeFix(analyzer, fix, expectedDiagnostic, new[] { part1, part2 }, new[] { part2, after }, fixTitle: "Rename to: value1");
                RoslynAssert.CodeFix(analyzer, fix, expectedDiagnostic, new[] { part1, part2 }, new[] { after, part2 }, fixTitle: "Rename to: value1");
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
        public event EventHandler ↓Bar;
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
                RoslynAssert.MetadataReferences.Add(MetadataReference.CreateFromFile(typeof(EventHandler).Assembly.Location));
                var expectedDiagnostic = ExpectedDiagnostic.Create("CS0067");
                var fix = new RemoveUnusedFixProvider();
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
        public event EventHandler ↓Bar;
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
                RoslynAssert.MetadataReferences.Add(MetadataReference.CreateFromFile(typeof(EventHandler).Assembly.Location));
                var expectedDiagnostic = ExpectedDiagnostic.Create("CS0067");
                var fix = new RemoveUnusedFixProvider();
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
        public event EventHandler ↓Bar;
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
                RoslynAssert.MetadataReferences.Add(MetadataReference.CreateFromFile(typeof(EventHandler).Assembly.Location));
                var expectedDiagnostic = ExpectedDiagnostic.Create("CS0067");
                var fix = new RemoveUnusedFixProvider();
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
                var fix = new RenameToValueCodeFixProvider();
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
                var fix = new InsertEventFixProvider();
                var expectedDiagnostic = ExpectedDiagnostic.Create(ClassMustHaveEventAnalyzer.DiagnosticId);
                RoslynAssert.CodeFix(analyzer, fix, before, after, allowCompilationErrors: AllowCompilationErrors.Yes);
                RoslynAssert.CodeFix(analyzer, fix, new[] { before }, after, allowCompilationErrors: AllowCompilationErrors.Yes);
                RoslynAssert.CodeFix(analyzer, fix, new[] { before }, new[] { after }, allowCompilationErrors: AllowCompilationErrors.Yes);
                RoslynAssert.CodeFix(analyzer, fix, expectedDiagnostic, before, after, allowCompilationErrors: AllowCompilationErrors.Yes);
                RoslynAssert.CodeFix(analyzer, fix, expectedDiagnostic, new[] { before }, after, allowCompilationErrors: AllowCompilationErrors.Yes);
                RoslynAssert.CodeFix(analyzer, fix, expectedDiagnostic, new[] { before }, new[] { after }, allowCompilationErrors: AllowCompilationErrors.Yes);
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
                var fix = new RenameToValueCodeFixProvider();
                RoslynAssert.CodeFix(analyzer, fix, expectedDiagnostic, before, after);
                RoslynAssert.CodeFix(analyzer, fix, expectedDiagnostic, new[] { before }, after);
                RoslynAssert.CodeFix(analyzer, fix, expectedDiagnostic, new[] { before }, new[] { after });
            }

            [Test]
            public static void WithMetadataReferences()
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
                RoslynAssert.MetadataReferences.Add(MetadataReference.CreateFromFile(typeof(EventHandler).Assembly.Location));
                var analyzer = new ClassMustHaveEventAnalyzer();
                var fix = new InsertEventFixProvider();
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
                var solution = CodeFactory.CreateSolution(
                    csproj,
                    analyzer,
                    expectedDiagnostic,
                    metadataReferences: new[] { MetadataReference.CreateFromFile(typeof(int).Assembly.Location) });
                RoslynAssert.CodeFix(analyzer, new DoNotUseUnderscoreFix(), expectedDiagnostic, solution, after);
            }
        }
    }
}
