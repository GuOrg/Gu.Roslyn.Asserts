// ReSharper disable RedundantNameQualifier
namespace Gu.Roslyn.Asserts.Tests.RoslynAssertTests
{
    using Gu.Roslyn.Asserts.Tests.TestHelpers.Suppressors;
    using Microsoft.CodeAnalysis.Diagnostics;
    using NUnit.Framework;

    public static partial class VisibleMagicFieldIsAllowed
    {
        public static class Fail
        {
            private static readonly DiagnosticSuppressor Suppressor = new AllowUnassignedMagicMembers();

            [Test]
            public static void FailsINothingToSuppress()
            {
                const string code = @"  
namespace N
{
    public class C
    {
        public string? F;
    }
}";
                var exception = Assert.Throws<AssertException>(() => RoslynAssert.Suppressed(Suppressor, code));
                CodeAssert.AreEqual(exception.Message, "Found no errors to suppress");
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
            return Magic;
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
        public string F;
    }
}";
                var exception = Assert.Throws<AssertException>(() => RoslynAssert.Suppressed(Suppressor, code));
                Assert.That(exception.Message, Does.Contain(AllowUnassignedMagicMembers.FieldNameIsMagic.SuppressedDiagnosticId));
            }

            [Test]
            public static void DoesFailIfSuppressedByWrongSuppressionDescriptor()
            {
                const string code = @"  
namespace N
{
    public class C
    {
        public string Magic;
    }
}";
                var expected = "Expected diagnostic to be suppressed by AllowUnassignedMagicMembers:Property is called Magic but was:\r\n" +
                               "AllowUnassignedMagicMembers:Field is called Magic\r\n";
                var exception = Assert.Throws<AssertException>(() =>
                    RoslynAssert.SuppressedBy(Suppressor, AllowUnassignedMagicMembers.PropertyNameIsMagic, code));
                CodeAssert.AreEqual(expected, exception.Message);
            }

            [Test]
            public static void DoesSuppressDiagnosticAboutMagicPropertiesWithWrongSuppressionDescriptor()
            {
                const string code = @"  
namespace N
{
    public class C
    {
        public string Magic { get; set; }
    }
}";
                var expected = "Expected diagnostic to be suppressed by AllowUnassignedMagicMembers:Field is called Magic but was:\r\n" +
                               "AllowUnassignedMagicMembers:Property is called Magic\r\n";
                var exception = Assert.Throws<AssertException>(() =>
                    RoslynAssert.SuppressedBy(Suppressor, AllowUnassignedMagicMembers.FieldNameIsMagic, code));
                CodeAssert.AreEqual(expected, exception.Message);
            }
        }
    }
}
