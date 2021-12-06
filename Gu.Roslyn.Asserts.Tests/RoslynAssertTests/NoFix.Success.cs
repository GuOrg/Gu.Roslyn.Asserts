// ReSharper disable RedundantNameQualifier
namespace Gu.Roslyn.Asserts.Tests.RoslynAssertTests
{
    using System.Collections.Generic;
    using NUnit.Framework;

    [TestFixture]
    public static partial class NoFix
    {
        public static class Success
        {
            [Test]
            public static void SingleDocumentOneErrorNoFix()
            {
                var code = @"
namespace N
{
    class C
    {
        private readonly int ↓_f = 1;

        public int M() => _f;
    }
}";
                var analyzer = new FieldNameMustNotBeginWithUnderscore();
                var fix = new CodeFixes.NoFix();
                var expectedDiagnostic = ExpectedDiagnostic.Create(FieldNameMustNotBeginWithUnderscore.DiagnosticId);
                RoslynAssert.NoFix(analyzer, fix, code);
                RoslynAssert.NoFix(analyzer, new CodeFixes.NoFix(), expectedDiagnostic, code);
                RoslynAssert.NoFix(analyzer, new CodeFixes.NoFix(), expectedDiagnostic, new List<string> { code });
            }

            [Test]
            public static void OneDocumentOneErrorNoFix()
            {
                var code = @"
namespace N
{
    class C
    {
        private readonly int ↓_f = 1;

        public int M() => _f;
    }
}";

                var analyzer = new FieldNameMustNotBeginWithUnderscore();
                var fix = new CodeFixes.NoFix();
                RoslynAssert.NoFix(analyzer, fix, code);
                var expectedDiagnostic = ExpectedDiagnostic.Create(FieldNameMustNotBeginWithUnderscore.DiagnosticId);
                RoslynAssert.NoFix(analyzer, fix, expectedDiagnostic, code);
            }

            [Test]
            public static void OneDocumentOneErrorNoFixSuppressedWarnings()
            {
                var code = @"
namespace N
{
    class C
    {
        private readonly int ↓_f;
    }
}";

                var analyzer = new FieldNameMustNotBeginWithUnderscore();
                var fix = new CodeFixes.NoFix();
                var expectedDiagnostic = ExpectedDiagnostic.Create(FieldNameMustNotBeginWithUnderscore.DiagnosticId);
                var settings = Settings.Default.WithCompilationOptions(x => x.WithSuppressedDiagnostics("CS0169"));
                RoslynAssert.NoFix(analyzer, fix, expectedDiagnostic, code, settings);
            }

            [Test]
            public static void OneDocumentOneErrorNoFixAllowWarnings()
            {
                var code = @"
namespace N
{
    class C
    {
        private readonly int ↓_f;
    }
}";

                var analyzer = new FieldNameMustNotBeginWithUnderscore();
                var fix = new CodeFixes.NoFix();
                var expectedDiagnostic = ExpectedDiagnostic.Create(FieldNameMustNotBeginWithUnderscore.DiagnosticId);
                var settings = Settings.Default.WithAllowedCompilerDiagnostics(AllowedCompilerDiagnostics.Warnings);
                RoslynAssert.NoFix(analyzer, fix, expectedDiagnostic, code, settings);
            }

            [Test]
            public static void TwoDocumentsOneErrorNoFix()
            {
                var barCode = @"
namespace N
{
    class C1
    {
        private readonly int value = 1;

        public int M() => this.value;
    }
}";

                var code = @"
namespace N
{
    class C2
    {
        private readonly int ↓_value = 2;

        public int M() => _value;
    }
}";

                var analyzer = new FieldNameMustNotBeginWithUnderscore();
                var fix = new CodeFixes.NoFix();
                RoslynAssert.NoFix(analyzer, fix, barCode, code);
                var expectedDiagnostic = ExpectedDiagnostic.Create(FieldNameMustNotBeginWithUnderscore.DiagnosticId);
                RoslynAssert.NoFix(analyzer, fix, expectedDiagnostic, barCode, code);
            }
        }
    }
}
