// ReSharper disable RedundantNameQualifier
namespace Gu.Roslyn.Asserts.Tests
{
    using System.Collections.Generic;
    using Gu.Roslyn.Asserts.Tests.CodeFixes;
    using NUnit.Framework;

    [TestFixture]
    public static partial class RoslynAssertTests
    {
        public static class NoFixSuccess
        {
            [Test]
            public static void SingleDocumentOneErrorNoFix()
            {
                var code = @"
namespace RoslynSandbox
{
    class Foo
    {
        private readonly int ↓_value;
    }
}";
                var analyzer = new FieldNameMustNotBeginWithUnderscore();
                var expectedDiagnostic = ExpectedDiagnostic.Create(FieldNameMustNotBeginWithUnderscore.DiagnosticId);
                RoslynAssert.NoFix<FieldNameMustNotBeginWithUnderscore, NoCodeFixProvider>(code);
                RoslynAssert.NoFix<FieldNameMustNotBeginWithUnderscore, NoCodeFixProvider>(expectedDiagnostic, code);
                RoslynAssert.NoFix<FieldNameMustNotBeginWithUnderscore, NoCodeFixProvider>((IReadOnlyList<string>)new[] { code });
                RoslynAssert.NoFix(analyzer, new NoCodeFixProvider(), code);
                RoslynAssert.NoFix(analyzer, new NoCodeFixProvider(), expectedDiagnostic, code);
                RoslynAssert.NoFix(analyzer, new NoCodeFixProvider(), expectedDiagnostic, new List<string> { code });
                RoslynAssert.NoFix(analyzer, new NoCodeFixProvider(), new[] { code }, CodeFactory.DefaultCompilationOptions(analyzer, expectedDiagnostic, RoslynAssert.SuppressedDiagnostics), RoslynAssert.MetadataReferences);
            }

            [Test]
            public static void TwoClassOneErrorNoFix()
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

                RoslynAssert.NoFix<FieldNameMustNotBeginWithUnderscore, NoCodeFixProvider>(barCode, code);
                RoslynAssert.NoFix<FieldNameMustNotBeginWithUnderscore, NoCodeFixProvider>(new List<string> { barCode, code });
                RoslynAssert.NoFix(new FieldNameMustNotBeginWithUnderscore(), new NoCodeFixProvider(), barCode, code);

                var expectedDiagnostic = ExpectedDiagnostic.Create(FieldNameMustNotBeginWithUnderscore.DiagnosticId);
                RoslynAssert.NoFix<FieldNameMustNotBeginWithUnderscore, NoCodeFixProvider>(expectedDiagnostic, barCode, code);
                RoslynAssert.NoFix(new FieldNameMustNotBeginWithUnderscore(), new NoCodeFixProvider(), expectedDiagnostic, barCode, code);
            }
        }
    }
}
