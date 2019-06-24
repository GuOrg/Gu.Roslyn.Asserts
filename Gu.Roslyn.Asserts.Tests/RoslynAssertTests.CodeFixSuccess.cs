// ReSharper disable RedundantNameQualifier
namespace Gu.Roslyn.Asserts.Tests
{
    using System;
    using System.IO;
    using Gu.Roslyn.Asserts.Tests.CodeFixes;
    using Microsoft.CodeAnalysis;
    using NUnit.Framework;

    [TestFixture]
    public partial class RoslynAssertTests
    {
        public class CodeFixSuccess
        {
            [TearDown]
            public void TearDown()
            {
                RoslynAssert.ResetAll();
            }

            [Test]
            public void SingleDocumentOneError()
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
                RoslynAssert.CodeFix<FieldNameMustNotBeginWithUnderscore, DontUseUnderscoreCodeFixProvider>(code, fixedCode);
                RoslynAssert.CodeFix<FieldNameMustNotBeginWithUnderscore, DontUseUnderscoreCodeFixProvider>(new[] { code }, fixedCode);
                var analyzer = new FieldNameMustNotBeginWithUnderscore();
                var fix = new DontUseUnderscoreCodeFixProvider();
                RoslynAssert.CodeFix(analyzer, fix, new[] { code }, fixedCode);
                RoslynAssert.CodeFix(analyzer, fix, new[] { code }, new[] { fixedCode });
                var expectedDiagnostic = ExpectedDiagnostic.Create(FieldNameMustNotBeginWithUnderscore.Descriptor);
                RoslynAssert.CodeFix(analyzer, fix, expectedDiagnostic, new[] { code }, fixedCode);
                RoslynAssert.CodeFix(analyzer, fix, expectedDiagnostic, new[] { code }, new[] { fixedCode });
            }

            [Test]
            public void MakeSealed()
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
                RoslynAssert.MetadataReferences.Add(MetadataReference.CreateFromFile(typeof(int).Assembly.Location));
                RoslynAssert.CodeFix<ClassMustBeSealedAnalyzer, MakeSealedFixProvider>(code, fixedCode);
                RoslynAssert.CodeFix<ClassMustBeSealedAnalyzer, MakeSealedFixProvider>(new[] { code }, fixedCode);
                var analyzer = new ClassMustBeSealedAnalyzer();
                var fix = new MakeSealedFixProvider();
                RoslynAssert.CodeFix(analyzer, fix, new[] { code }, fixedCode);
                RoslynAssert.CodeFix(analyzer, fix, new[] { code }, new[] { fixedCode });
                var expectedDiagnostic = ExpectedDiagnostic.Create(ClassMustBeSealedAnalyzer.DiagnosticId);
                RoslynAssert.CodeFix(analyzer, fix, expectedDiagnostic, new[] { code }, fixedCode);
                RoslynAssert.CodeFix(analyzer, fix, expectedDiagnostic, new[] { code }, new[] { fixedCode });
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
                RoslynAssert.MetadataReferences.Add(MetadataReference.CreateFromFile(typeof(int).Assembly.Location));
                RoslynAssert.CodeFix<ClassMustBeSealedAnalyzer, MakeSealedFixProvider>(code, fixedCode);
                var analyzer = new ClassMustBeSealedAnalyzer();
                var fix = new MakeSealedFixProvider();
                RoslynAssert.CodeFix(analyzer, fix, new[] { code }, fixedCode);
                RoslynAssert.CodeFix(analyzer, fix, new[] { code }, new[] { fixedCode });
                var expectedDiagnostic = ExpectedDiagnostic.Create(ClassMustBeSealedAnalyzer.DiagnosticId);
                RoslynAssert.CodeFix(analyzer, fix, expectedDiagnostic, new[] { code }, fixedCode);
                RoslynAssert.CodeFix(analyzer, fix, expectedDiagnostic, new[] { code }, new[] { fixedCode });
            }

            [Test]
            public void SingleDocumentOneErrorCorrectFixExplicitTitle()
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
                RoslynAssert.CodeFix<FieldNameMustNotBeginWithUnderscore, DontUseUnderscoreCodeFixProvider>(code, fixedCode, "Rename to: value");
                var analyzer = new FieldNameMustNotBeginWithUnderscore();
                var fix = new DontUseUnderscoreCodeFixProvider();
                RoslynAssert.CodeFix(analyzer, fix, new[] { code }, fixedCode);
                RoslynAssert.CodeFix(analyzer, fix, new[] { code }, new[] { fixedCode });
                var expectedDiagnostic = ExpectedDiagnostic.Create(FieldNameMustNotBeginWithUnderscore.Descriptor);
                RoslynAssert.CodeFix(analyzer, fix, expectedDiagnostic, new[] { code }, fixedCode);
                RoslynAssert.CodeFix(analyzer, fix, expectedDiagnostic, new[] { code }, new[] { fixedCode });
            }

            [Test]
            public void SingleDocumentOneErrorCorrectFixExplicitTitleExpectedDiagnosticWithErrorsIndicated()
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
                var expectedDiagnostic = ExpectedDiagnostic.Create(FieldNameMustNotBeginWithUnderscore.Descriptor);
                RoslynAssert.CodeFix<FieldNameMustNotBeginWithUnderscore, DontUseUnderscoreCodeFixProvider>(expectedDiagnostic, code, fixedCode, "Rename to: value");
                RoslynAssert.CodeFix<FieldNameMustNotBeginWithUnderscore, DontUseUnderscoreCodeFixProvider>(expectedDiagnostic, new[] { code }, fixedCode, "Rename to: value");
                var analyzer = new FieldNameMustNotBeginWithUnderscore();
                var fix = new DontUseUnderscoreCodeFixProvider();
                RoslynAssert.CodeFix(analyzer, fix, expectedDiagnostic, code, fixedCode, "Rename to: value");
                RoslynAssert.CodeFix(analyzer, fix, expectedDiagnostic, new[] { code }, fixedCode, "Rename to: value");
                RoslynAssert.CodeFix(analyzer, fix, new[] { code }, fixedCode, fixTitle: "Rename to: value");
                RoslynAssert.CodeFix(analyzer, fix, new[] { code }, new[] { fixedCode }, fixTitle: "Rename to: value");
                RoslynAssert.CodeFix(analyzer, fix, expectedDiagnostic, new[] { code }, fixedCode, fixTitle: "Rename to: value");
                RoslynAssert.CodeFix(analyzer, fix, expectedDiagnostic, new[] { code }, new[] { fixedCode }, fixTitle: "Rename to: value");
            }

            [Test]
            public void SingleDocumentOneErrorCorrectFixExplicitTitleExpectedDiagnosticWithPosition()
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
                var expectedDiagnostic = ExpectedDiagnostic.CreateFromCodeWithErrorsIndicated(FieldNameMustNotBeginWithUnderscore.DiagnosticId, code, out code);
                RoslynAssert.CodeFix<FieldNameMustNotBeginWithUnderscore, DontUseUnderscoreCodeFixProvider>(expectedDiagnostic, code, fixedCode, "Rename to: value");
                RoslynAssert.CodeFix<FieldNameMustNotBeginWithUnderscore, DontUseUnderscoreCodeFixProvider>(expectedDiagnostic, new[] { code }, fixedCode, "Rename to: value");
                var analyzer = new FieldNameMustNotBeginWithUnderscore();
                var fix = new DontUseUnderscoreCodeFixProvider();
                RoslynAssert.CodeFix(analyzer, fix, expectedDiagnostic, code, fixedCode, "Rename to: value");
                RoslynAssert.CodeFix(analyzer, fix, expectedDiagnostic, new[] { code }, fixedCode, "Rename to: value");
                RoslynAssert.CodeFix(analyzer, fix, expectedDiagnostic, new[] { code }, fixedCode, fixTitle: "Rename to: value");
                RoslynAssert.CodeFix(analyzer, fix, expectedDiagnostic, new[] { code }, new[] { fixedCode }, fixTitle: "Rename to: value");
            }

            [Test]
            public void SingleDocumentOneErrorCorrectFixExplicitTitleExpectedDiagnosticWithPositionAnalyzerSupportsTwoDiagnostics()
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
                RoslynAssert.CodeFix<FieldNameMustNotBeginWithUnderscoreDifferentDiagnosticsForPublic, DontUseUnderscoreCodeFixProvider>(expectedDiagnostic, code, fixedCode, "Rename to: value");
                RoslynAssert.CodeFix<FieldNameMustNotBeginWithUnderscoreDifferentDiagnosticsForPublic, DontUseUnderscoreCodeFixProvider>(expectedDiagnostic, new[] { code }, fixedCode, "Rename to: value");
                var analyzer = new FieldNameMustNotBeginWithUnderscoreDifferentDiagnosticsForPublic();
                var fix = new DontUseUnderscoreCodeFixProvider();
                RoslynAssert.CodeFix(analyzer, fix, expectedDiagnostic, code, fixedCode, "Rename to: value");
                RoslynAssert.CodeFix(analyzer, fix, expectedDiagnostic, new[] { code }, fixedCode, "Rename to: value");
                RoslynAssert.CodeFix(analyzer, fix, expectedDiagnostic, new[] { code }, fixedCode, fixTitle: "Rename to: value");
                RoslynAssert.CodeFix(analyzer, fix, expectedDiagnostic, new[] { code }, new[] { fixedCode }, fixTitle: "Rename to: value");
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
                RoslynAssert.CodeFix<FieldNameMustNotBeginWithUnderscore, DontUseUnderscoreManyCodeFixProvider>(code, fixedCode, title);
                RoslynAssert.CodeFix<FieldNameMustNotBeginWithUnderscore, DontUseUnderscoreManyCodeFixProvider>(new[] { code }, fixedCode, title);
                var analyzer = new FieldNameMustNotBeginWithUnderscore();
                var fix = new DontUseUnderscoreManyCodeFixProvider();
                RoslynAssert.CodeFix(analyzer, fix, code, fixedCode, title);
                RoslynAssert.CodeFix(analyzer, fix, new[] { code }, fixedCode, fixTitle: title);
                RoslynAssert.CodeFix(analyzer, fix, new[] { code }, new[] { fixedCode }, fixTitle: title);
                var expectedDiagnostic = ExpectedDiagnostic.Create(FieldNameMustNotBeginWithUnderscore.Descriptor);
                RoslynAssert.CodeFix(analyzer, fix, expectedDiagnostic, code, fixedCode, title);
                RoslynAssert.CodeFix(analyzer, fix, expectedDiagnostic, new[] { code }, fixedCode, fixTitle: title);
                RoslynAssert.CodeFix(analyzer, fix, expectedDiagnostic, new[] { code }, new[] { fixedCode }, fixTitle: title);
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
                RoslynAssert.MetadataReferences.Add(MetadataReference.CreateFromFile(typeof(EventHandler).Assembly.Location));
                var expectedDiagnostic = ExpectedDiagnostic.Create("CS0067");
                RoslynAssert.CodeFix<RemoveUnusedFixProvider>(expectedDiagnostic, code, fixedCode);
                RoslynAssert.CodeFix<RemoveUnusedFixProvider>(expectedDiagnostic, new[] { code }, fixedCode);
                var fix = new RemoveUnusedFixProvider();
                RoslynAssert.CodeFix(fix, expectedDiagnostic, code, fixedCode);
                RoslynAssert.CodeFix(fix, expectedDiagnostic, new[] { code }, fixedCode);
                RoslynAssert.CodeFix(fix, expectedDiagnostic, new[] { code }, new[] { fixedCode });
                RoslynAssert.CodeFix(fix, expectedDiagnostic, code, fixedCode, fixTitle: "Remove public event EventHandler Bar;");
                RoslynAssert.CodeFix(fix, expectedDiagnostic, new[] { code }, fixedCode, fixTitle: "Remove public event EventHandler Bar;");
                RoslynAssert.CodeFix(fix, expectedDiagnostic, new[] { code }, new[] { fixedCode }, fixTitle: "Remove public event EventHandler Bar;");
            }

            [Test]
            public void TwoDocumentsCodeFixOnly()
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
                RoslynAssert.MetadataReferences.Add(MetadataReference.CreateFromFile(typeof(EventHandler).Assembly.Location));
                var expectedDiagnostic = ExpectedDiagnostic.Create("CS0067");
                RoslynAssert.CodeFix<RemoveUnusedFixProvider>(expectedDiagnostic, new[] { code1, code2 }, fixedCode);
                RoslynAssert.CodeFix<RemoveUnusedFixProvider>(expectedDiagnostic, new[] { code2, code1 }, fixedCode);
                var fix = new RemoveUnusedFixProvider();
                RoslynAssert.CodeFix(fix, expectedDiagnostic, new[] { code2, code1 }, fixedCode);
                RoslynAssert.CodeFix(fix, expectedDiagnostic, new[] { code2, code1 }, new[] { fixedCode, code2 });
                RoslynAssert.CodeFix(fix, expectedDiagnostic, new[] { code1, code2 }, fixedCode);
                RoslynAssert.CodeFix(fix, expectedDiagnostic, new[] { code1, code2 }, new[] { fixedCode, code2 });
            }

            [Test]
            public void PartialTwoDocumentsCodeFixOnly()
            {
                var part1 = @"
namespace RoslynSandbox
{
    using System;

    public partial class Foo
    {
        public event EventHandler ↓Bar;
    }
}";

                var part2 = @"
namespace RoslynSandbox
{
    public partial class Foo
    {
    }
}";

                var fixedCode = @"
namespace RoslynSandbox
{
    using System;

    public partial class Foo
    {
    }
}";
                RoslynAssert.MetadataReferences.Add(MetadataReference.CreateFromFile(typeof(EventHandler).Assembly.Location));
                var expectedDiagnostic = ExpectedDiagnostic.Create("CS0067");
                RoslynAssert.CodeFix<RemoveUnusedFixProvider>(expectedDiagnostic, new[] { part1, part2 }, fixedCode);
                RoslynAssert.CodeFix<RemoveUnusedFixProvider>(expectedDiagnostic, new[] { part2, part1 }, fixedCode);
                var fix = new RemoveUnusedFixProvider();
                RoslynAssert.CodeFix(fix, expectedDiagnostic, new[] { part2, part1 }, fixedCode);
                RoslynAssert.CodeFix(fix, expectedDiagnostic, new[] { part2, part1 }, new[] { fixedCode, part2 });
                RoslynAssert.CodeFix(fix, expectedDiagnostic, new[] { part1, part2 }, fixedCode);
                RoslynAssert.CodeFix(fix, expectedDiagnostic, new[] { part1, part2 }, new[] { part2, fixedCode });
            }

            [Test]
            public void PartialTwoDocumentsOneFix()
            {
                var part1 = @"
namespace RoslynSandbox
{
    public partial class Foo
    {
        private int ↓_value;
    }
}";

                var part2 = @"
namespace RoslynSandbox
{
    public partial class Foo
    {
    }
}";

                var fixedCode = @"
namespace RoslynSandbox
{
    public partial class Foo
    {
        private int value;
    }
}";
                RoslynAssert.MetadataReferences.Add(MetadataReference.CreateFromFile(typeof(EventHandler).Assembly.Location));
                var expectedDiagnostic = ExpectedDiagnostic.Create(FieldNameMustNotBeginWithUnderscore.Descriptor);
                RoslynAssert.CodeFix<FieldNameMustNotBeginWithUnderscore, DontUseUnderscoreCodeFixProvider>(expectedDiagnostic, new[] { part1, part2 }, fixedCode);
                RoslynAssert.CodeFix<FieldNameMustNotBeginWithUnderscore, DontUseUnderscoreCodeFixProvider>(expectedDiagnostic, new[] { part2, part1 }, fixedCode);
                var analyzer = new FieldNameMustNotBeginWithUnderscore();
                var fix = new DontUseUnderscoreCodeFixProvider();
                RoslynAssert.CodeFix(analyzer, fix, new[] { part1, part2 }, fixedCode);
                RoslynAssert.CodeFix(analyzer, fix, new[] { part2, part1 }, fixedCode);
                RoslynAssert.CodeFix(analyzer, fix, new[] { part2, part1 }, new[] { fixedCode, part2 });
                RoslynAssert.CodeFix(analyzer, fix, new[] { part2, part1 }, new[] { part2, fixedCode });
                RoslynAssert.CodeFix(analyzer, fix, expectedDiagnostic, new[] { part2, part1 }, fixedCode);
                RoslynAssert.CodeFix(analyzer, fix, expectedDiagnostic, new[] { part1, part2 }, fixedCode);
                RoslynAssert.CodeFix(analyzer, fix, expectedDiagnostic, new[] { part2, part1 }, new[] { fixedCode, part2 });
                RoslynAssert.CodeFix(analyzer, fix, expectedDiagnostic, new[] { part2, part1 }, new[] { part2, fixedCode });
                RoslynAssert.CodeFix(analyzer, fix, expectedDiagnostic, new[] { part1, part2 }, fixedCode, fixTitle: "Rename to: value");
                RoslynAssert.CodeFix(analyzer, fix, expectedDiagnostic, new[] { part1, part2 }, new[] { part2, fixedCode }, fixTitle: "Rename to: value");
                RoslynAssert.CodeFix(analyzer, fix, expectedDiagnostic, new[] { part1, part2 }, new[] { fixedCode, part2 }, fixTitle: "Rename to: value");
            }

            [Test]
            public void TwoDocumentsDifferentProjectsCodeFixOnly()
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
                var expectedDiagnostic = ExpectedDiagnostic.Create("CS0067");
                RoslynAssert.CodeFix<RemoveUnusedFixProvider>(expectedDiagnostic, new[] { code1, code2 }, fixedCode);
                RoslynAssert.CodeFix<RemoveUnusedFixProvider>(expectedDiagnostic, new[] { code2, code1 }, fixedCode);
                var fix = new RemoveUnusedFixProvider();
                RoslynAssert.CodeFix(fix, expectedDiagnostic, new[] { code2, code1 }, fixedCode);
                RoslynAssert.CodeFix(fix, expectedDiagnostic, new[] { code2, code1 }, new[] { fixedCode, code2 });
                RoslynAssert.CodeFix(fix, expectedDiagnostic, new[] { code1, code2 }, fixedCode);
                RoslynAssert.CodeFix(fix, expectedDiagnostic, new[] { code1, code2 }, new[] { fixedCode, code2 });

                expectedDiagnostic = ExpectedDiagnostic.CreateFromCodeWithErrorsIndicated("CS0067", code1, out code1);
                RoslynAssert.CodeFix<RemoveUnusedFixProvider>(expectedDiagnostic, new[] { code1, code2 }, fixedCode);
                RoslynAssert.CodeFix<RemoveUnusedFixProvider>(expectedDiagnostic, new[] { code2, code1 }, fixedCode);
                RoslynAssert.CodeFix(fix, expectedDiagnostic, new[] { code2, code1 }, fixedCode);
                RoslynAssert.CodeFix(fix, expectedDiagnostic, new[] { code2, code1 }, new[] { fixedCode, code2 });
                RoslynAssert.CodeFix(fix, expectedDiagnostic, new[] { code1, code2 }, fixedCode);
                RoslynAssert.CodeFix(fix, expectedDiagnostic, new[] { code1, code2 }, new[] { fixedCode, code2 });
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
                var expectedDiagnostic = ExpectedDiagnostic.Create("CS0067");
                RoslynAssert.CodeFix<RemoveUnusedFixProvider>(expectedDiagnostic, new[] { code1, code2 }, fixedCode);
                RoslynAssert.CodeFix<RemoveUnusedFixProvider>(expectedDiagnostic, new[] { code2, code1 }, fixedCode);
                var fix = new RemoveUnusedFixProvider();
                RoslynAssert.CodeFix(fix, expectedDiagnostic, new[] { code2, code1 }, fixedCode);
                RoslynAssert.CodeFix(fix, expectedDiagnostic, new[] { code2, code1 }, new[] { fixedCode, code2 });
                RoslynAssert.CodeFix(fix, expectedDiagnostic, new[] { code1, code2 }, fixedCode);
                RoslynAssert.CodeFix(fix, expectedDiagnostic, new[] { code1, code2 }, new[] { fixedCode, code2 });

                expectedDiagnostic = ExpectedDiagnostic.CreateFromCodeWithErrorsIndicated("CS0067", code1, out code1);
                RoslynAssert.CodeFix<RemoveUnusedFixProvider>(expectedDiagnostic, new[] { code1, code2 }, fixedCode);
                RoslynAssert.CodeFix<RemoveUnusedFixProvider>(expectedDiagnostic, new[] { code2, code1 }, fixedCode);
                RoslynAssert.CodeFix(fix, expectedDiagnostic, new[] { code2, code1 }, fixedCode);
                RoslynAssert.CodeFix(fix, expectedDiagnostic, new[] { code2, code1 }, new[] { fixedCode, code2 });
                RoslynAssert.CodeFix(fix, expectedDiagnostic, new[] { code1, code2 }, fixedCode);
                RoslynAssert.CodeFix(fix, expectedDiagnostic, new[] { code1, code2 }, new[] { fixedCode, code2 });
            }

            [Test]
            public void TwoDocumentsDifferentProjectsInheritingCodeFixOnly2()
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
                var expectedDiagnostic = ExpectedDiagnostic.Create("CS0067");
                RoslynAssert.CodeFix<RemoveUnusedFixProvider>(expectedDiagnostic, new[] { code1, code2 }, fixedCode);
                RoslynAssert.CodeFix<RemoveUnusedFixProvider>(expectedDiagnostic, new[] { code2, code1 }, fixedCode);
                var fix = new RemoveUnusedFixProvider();
                RoslynAssert.CodeFix(fix, expectedDiagnostic, new[] { code2, code1 }, fixedCode);
                RoslynAssert.CodeFix(fix, expectedDiagnostic, new[] { code1, code2 }, fixedCode);
                RoslynAssert.CodeFix(fix, expectedDiagnostic, new[] { code2, code1 }, new[] { fixedCode, code1 });
                RoslynAssert.CodeFix(fix, expectedDiagnostic, new[] { code2, code1 }, new[] { code1, fixedCode });
                RoslynAssert.CodeFix(fix, expectedDiagnostic, new[] { code1, code2 }, new[] { fixedCode, code1 });
                RoslynAssert.CodeFix(fix, expectedDiagnostic, new[] { code1, code2 }, new[] { code1, fixedCode });
            }

            [Test]
            public void TwoDocumentsOneError()
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
                RoslynAssert.MetadataReferences.Add(MetadataReference.CreateFromFile(typeof(int).Assembly.Location));
                RoslynAssert.CodeFix<FieldNameMustNotBeginWithUnderscore, DontUseUnderscoreCodeFixProvider>(new[] { barCode, code }, fixedCode);
                var analyzer = new FieldNameMustNotBeginWithUnderscore();
                var fix = new DontUseUnderscoreCodeFixProvider();
                RoslynAssert.CodeFix(analyzer, fix, new[] { barCode, code }, fixedCode);
                RoslynAssert.CodeFix(analyzer, fix, new[] { barCode, code }, new[] { barCode, fixedCode });
            }

            [Test]
            public void TwoDocumentsOneErrorFixTouchingBothDocuments()
            {
                var code1 = @"
namespace RoslynSandbox
{
    class C1
    {
        public int ↓WrongName { get; }
    }
}";

                var code2 = @"
namespace RoslynSandbox
{
    class C2
    {
        public C2(C1 c1)
        {
            var x = c1.WrongName;
        }
    }
}";

                var fixedCode1 = @"
namespace RoslynSandbox
{
    class C1
    {
        public int Foo { get; }
    }
}";

                var fixedCode2 = @"
namespace RoslynSandbox
{
    class C2
    {
        public C2(C1 c1)
        {
            var x = c1.Foo;
        }
    }
}";
                RoslynAssert.MetadataReferences.Add(MetadataReference.CreateFromFile(typeof(int).Assembly.Location));
                var analyzer = new PropertyMustBeNamedFooAnalyzer();
                var fix = new RenameToFooCodeFixProvider();
                RoslynAssert.CodeFix(analyzer, fix, new[] { code1, code2 }, new[] { fixedCode1, fixedCode2 });
                RoslynAssert.CodeFix(analyzer, fix, new[] { code2, code1 }, new[] { fixedCode1, fixedCode2 });
                RoslynAssert.CodeFix(analyzer, fix, new[] { code1, code2 }, new[] { fixedCode2, fixedCode1 });
                RoslynAssert.CodeFix(analyzer, fix, new[] { code2, code1 }, new[] { fixedCode2, fixedCode1 });

                RoslynAssert.CodeFix(analyzer, fix, new[] { code1, code2 }, new[] { fixedCode1, fixedCode2 }, fixTitle: "Rename to: Foo");
                RoslynAssert.CodeFix(analyzer, fix, new[] { code2, code1 }, new[] { fixedCode1, fixedCode2 }, fixTitle: "Rename to: Foo");
                RoslynAssert.CodeFix(analyzer, fix, new[] { code1, code2 }, new[] { fixedCode2, fixedCode1 }, fixTitle: "Rename to: Foo");
                RoslynAssert.CodeFix(analyzer, fix, new[] { code2, code1 }, new[] { fixedCode2, fixedCode1 }, fixTitle: "Rename to: Foo");
            }

            [Test]
            public void CodeFixAddingDocument()
            {
                var code = @"
namespace RoslynSandbox
{
    class C
    {
        public static C Create() => ↓new C();
    }
}";

                var fixedCode = @"
namespace RoslynSandbox
{
    class C
    {
        public static C Create() => new C().Id();
    }
}";

                var extensionMethodCode = @"namespace RoslynSandbox
{
    public static class Extensions
    {
        public static T Id<T>(this T t) => t;
    }
}";
                RoslynAssert.MetadataReferences.Add(MetadataReference.CreateFromFile(typeof(int).Assembly.Location));
                var analyzer = new CallIdAnalyzer();
                var fix = new CallIdFix();
                RoslynAssert.CodeFix(analyzer, fix, new[] { code }, new[] { fixedCode, extensionMethodCode });
                RoslynAssert.CodeFix(analyzer, fix, new[] { code }, new[] { fixedCode, extensionMethodCode }, fixTitle: "Call ID()");
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
                RoslynAssert.CodeFix<ClassMustHaveEventAnalyzer, InsertEventFixProvider>(code, fixedCode, null, AllowCompilationErrors.Yes);
                var analyzer = new ClassMustHaveEventAnalyzer();
                var fix = new InsertEventFixProvider();
                var expectedDiagnostic = ExpectedDiagnostic.Create(ClassMustHaveEventAnalyzer.DiagnosticId);
                RoslynAssert.CodeFix(analyzer, fix, code, fixedCode, allowCompilationErrors: AllowCompilationErrors.Yes);
                RoslynAssert.CodeFix(analyzer, fix, new[] { code }, fixedCode, allowCompilationErrors: AllowCompilationErrors.Yes);
                RoslynAssert.CodeFix(analyzer, fix, new[] { code }, new[] { fixedCode }, allowCompilationErrors: AllowCompilationErrors.Yes);
                RoslynAssert.CodeFix(analyzer, fix, expectedDiagnostic, code, fixedCode, allowCompilationErrors: AllowCompilationErrors.Yes);
                RoslynAssert.CodeFix(analyzer, fix, expectedDiagnostic, new[] { code }, fixedCode, allowCompilationErrors: AllowCompilationErrors.Yes);
                RoslynAssert.CodeFix(analyzer, fix, expectedDiagnostic, new[] { code }, new[] { fixedCode }, allowCompilationErrors: AllowCompilationErrors.Yes);
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
                RoslynAssert.CodeFix<FieldAndPropertyMustBeNamedFooAnalyzer, RenameToFooCodeFixProvider>(expectedDiagnostic, code, fixedCode);
                var analyzer = new FieldAndPropertyMustBeNamedFooAnalyzer();
                var fix = new RenameToFooCodeFixProvider();
                RoslynAssert.CodeFix(analyzer, fix, expectedDiagnostic, code, fixedCode);
                RoslynAssert.CodeFix(analyzer, fix, expectedDiagnostic, new[] { code }, fixedCode);
                RoslynAssert.CodeFix(analyzer, fix, expectedDiagnostic, new[] { code }, new[] { fixedCode });
            }

            [Test]
            public void WithMetadataReferences()
            {
                var code = @"
namespace RoslynSandbox
{
    using System;

    ↓class Foo
    {
    }
}";

                var fixedCode = @"
namespace RoslynSandbox
{
    using System;

    class Foo
    {
        public event EventHandler SomeEvent;
    }
}";
                RoslynAssert.MetadataReferences.Add(MetadataReference.CreateFromFile(typeof(EventHandler).Assembly.Location));
                RoslynAssert.CodeFix<ClassMustHaveEventAnalyzer, InsertEventFixProvider>(code, fixedCode);
                var analyzer = new ClassMustHaveEventAnalyzer();
                var fix = new InsertEventFixProvider();
                RoslynAssert.CodeFix(analyzer, fix, code, fixedCode);
                RoslynAssert.CodeFix(analyzer, fix, new[] { code }, fixedCode);
                RoslynAssert.CodeFix(analyzer, fix, new[] { code }, new[] { fixedCode });
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
                RoslynAssert.CodeFix(analyzer, new DontUseUnderscoreCodeFixProvider(), expectedDiagnostic, sln, fixedCode);
            }
        }
    }
}
