// ReSharper disable RedundantNameQualifier
namespace Gu.Roslyn.Asserts.Tests
{
    using System.Collections.Generic;
    using Gu.Roslyn.Asserts.Tests.CodeFixes;
    using Microsoft.CodeAnalysis;
    using NUnit.Framework;

    [TestFixture]
    public static partial class RoslynAssertTests
    {
        public static class NoFixSuccess
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
                var fix = new NoCodeFixProvider();
                var expectedDiagnostic = ExpectedDiagnostic.Create(FieldNameMustNotBeginWithUnderscore.DiagnosticId);
                RoslynAssert.NoFix(analyzer, fix, code);
                RoslynAssert.NoFix(analyzer, new NoCodeFixProvider(), expectedDiagnostic, code);
                RoslynAssert.NoFix(analyzer, new NoCodeFixProvider(), expectedDiagnostic, new List<string> { code });
            }

            [Test]
            public static void OneDocumentOneErrorNoFix()
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
                var fix = new NoCodeFixProvider();
                RoslynAssert.NoFix(analyzer, fix, code);
                var expectedDiagnostic = ExpectedDiagnostic.Create(FieldNameMustNotBeginWithUnderscore.DiagnosticId);
                RoslynAssert.NoFix(analyzer, fix, expectedDiagnostic, code);
            }

            [Test]
            public static void TwoDocumentsOneErrorNoFix()
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

                var analyzer = new FieldNameMustNotBeginWithUnderscore();
                var fix = new NoCodeFixProvider();
                RoslynAssert.NoFix(analyzer, fix, barCode, code);
                var expectedDiagnostic = ExpectedDiagnostic.Create(FieldNameMustNotBeginWithUnderscore.DiagnosticId);
                RoslynAssert.NoFix(analyzer, fix, expectedDiagnostic, barCode, code);
            }
        }
    }
}
