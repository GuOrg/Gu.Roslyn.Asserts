// ReSharper disable RedundantNameQualifier
#pragma warning disable IDE0079 // Remove unnecessary suppression
namespace Gu.Roslyn.Asserts.Tests.RoslynAssertTests;

using System;
using Gu.Roslyn.Asserts.Tests.TestHelpers.Suppressors;
using Microsoft.CodeAnalysis.Diagnostics;
using NUnit.Framework;

public static partial class VisibleMagicFieldIsAllowed
{
    public static class Fail
    {
        private static readonly DiagnosticSuppressor Suppressor = new AllowUnassignedMagicMembers();

        [Test]
        public static void FailsIfNothingToSuppress()
        {
#pragma warning disable GURA02 // Indicate position
            const string code = @"  
namespace N
{
    public class C
    {
        public string? F;
    }
}";
#pragma warning restore GURA02 // Indicate position
            var expected = "Expected code to have at least one error position indicated with '↓'";
            var exception = Assert.Throws<InvalidOperationException>(() => RoslynAssert.Suppressed(Suppressor, code));
            CodeAssert.AreEqual(expected, exception.Message);
        }

        [Test]
        public static void FailsIfDiagnosticIsNotSuppressableBySuppressor()
        {
            const string code = @"  
namespace N
{
    public class C
    {
        public string M()
        {
            string Magic;
            return ↓Magic;
        }
    }
}";
            var exception = Assert.Throws<AssertException>(() => RoslynAssert.Suppressed(Suppressor, code));
            Assert.That(exception.Message, Does.Contain("CS0165 Use of unassigned local variable 'Magic'"));
        }

        [Test]
        public static void DoesNotSuppressNonMagicField()
        {
            const string code = @"  
namespace N
{
    public class C
    {
        public string ↓F;
    }
}";
            var exception = Assert.Throws<AssertException>(() => RoslynAssert.Suppressed(Suppressor, code));
            Assert.That(exception.Message, Does.Contain(AllowUnassignedMagicMembers.FieldNameIsMagic.SuppressedDiagnosticId));
        }
    }
}
