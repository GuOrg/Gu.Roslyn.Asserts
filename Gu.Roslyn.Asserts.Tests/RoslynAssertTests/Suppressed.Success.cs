﻿// ReSharper disable RedundantNameQualifier
namespace Gu.Roslyn.Asserts.Tests.RoslynAssertTests
{
    using Gu.Roslyn.Asserts.Tests.TestHelpers.Suppressors;
    using Microsoft.CodeAnalysis.Diagnostics;
    using NUnit.Framework;

    public static partial class VisibleMagicFieldIsAllowed
    {
        public static class Success
        {
            private static readonly DiagnosticSuppressor Suppressor = new AllowUnassignedMagicMembers();

            [Test]
            public static void DoesNotSuppressAllDiagnostics()
            {
                const string code = @"  
namespace N
{
    public class C
    {
        public string F;
    }
}";
                RoslynAssert.NotSuppressed(Suppressor, code);
            }

            [Test]
            public static void DoesSuppressDiagnosticAboutMagicField()
            {
                const string code = @"  
namespace N
{
    public class C
    {
        public string Magic;
    }
}";
                RoslynAssert.Suppressed(Suppressor, code);
            }

            [Test]
            public static void DoesSuppressDiagnosticAboutMagicFieldWithSuppressionDescriptor()
            {
                const string code = @"  
namespace N
{
    public class C
    {
        public string Magic;
    }
}";
                RoslynAssert.SuppressedBy(Suppressor, AllowUnassignedMagicMembers.FieldNameIsMagic, code);
            }

            [Test]
            public static void DoesSuppressDiagnosticAboutMagicProperties()
            {
                const string code = @"  
namespace N
{
    public class C
    {
        public string Magic { get; set; }
    }
}";
                RoslynAssert.Suppressed(Suppressor, code);
            }

            [Test]
            public static void DoesSuppressDiagnosticAboutMagicPropertiesWithSuppressionDescriptor()
            {
                const string code = @"  
namespace N
{
    public class C
    {
        public string Magic { get; set; }
    }
}";
                RoslynAssert.SuppressedBy(Suppressor, AllowUnassignedMagicMembers.PropertyNameIsMagic, code);
            }
        }
    }
}
