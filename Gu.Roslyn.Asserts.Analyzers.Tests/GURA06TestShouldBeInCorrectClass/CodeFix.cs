namespace Gu.Roslyn.Asserts.Analyzers.Tests.GURA06TestShouldBeInCorrectClass;

using NUnit.Framework;

public static class CodeFix
{
    private static readonly DiagnosticFixAssert Assert = RoslynAssert.Create<MethodDeclarationAnalyzer, MoveFix>(
        ExpectedDiagnostic.Create(Descriptors.GURA06TestShouldBeInCorrectClass));

    [Test]
    public static void WhenMixCreatingClass()
    {
        var before = """
            namespace N
            {
                using Gu.Roslyn.Asserts;
                using NUnit.Framework;

                public static class Valid
                {
                    private static readonly PlaceholderAnalyzer Analyzer = new PlaceholderAnalyzer();

                    [Test]
                    public static void M1()
                    {
                        var c = "class C { }";
                        RoslynAssert.Valid(Analyzer, c);
                    }

                    [Test]
                    public static void ↓M2()
                    {
                        var c = "class C { }";
                        RoslynAssert.Diagnostics(Analyzer, c);
                    }
                }
            }
            """;

        var valid = """
            namespace N
            {
                using Gu.Roslyn.Asserts;
                using NUnit.Framework;

                public static class Valid
                {
                    private static readonly PlaceholderAnalyzer Analyzer = new PlaceholderAnalyzer();

                    [Test]
                    public static void M1()
                    {
                        var c = "class C { }";
                        RoslynAssert.Valid(Analyzer, c);
                    }
                }
            }
            """;

        var diagnostics = """
            namespace N
            {
                using Gu.Roslyn.Asserts;
                using NUnit.Framework;

                public static class Diagnostics
                {
                    private static readonly PlaceholderAnalyzer Analyzer = new PlaceholderAnalyzer();

                    [Test]
                    public static void M2()
                    {
                        var c = "class C { }";
                        RoslynAssert.Diagnostics(Analyzer, c);
                    }
                }
            }
            """;
        Assert.CodeFix(new[] { Code.PlaceholderAnalyzer, before }, new[] { Code.PlaceholderAnalyzer, valid, diagnostics });
    }

    [Test]
    public static void WhenMixAndClassExists()
    {
        var before = """
            namespace N
            {
                using Gu.Roslyn.Asserts;
                using NUnit.Framework;

                public static class Valid
                {
                    private static readonly PlaceholderAnalyzer Analyzer = new PlaceholderAnalyzer();

                    [Test]
                    public static void M1()
                    {
                        var c = "class C { }";
                        RoslynAssert.Valid(Analyzer, c);
                    }

                    [Test]
                    public static void ↓M2()
                    {
                        var c = "class C { }";
                        RoslynAssert.Diagnostics(Analyzer, c);
                    }
                }
            }
            """;

        var diagnosticsBefore = """
            namespace N
            {
                using Gu.Roslyn.Asserts;
                using NUnit.Framework;

                public static class Diagnostics
                {
                    private static readonly PlaceholderAnalyzer Analyzer = new PlaceholderAnalyzer();

                    [Test]
                    public static void M1()
                    {
                        var c = "class C { }";
                        RoslynAssert.Diagnostics(Analyzer, c);
                    }
                }
            }
            """;

        var valid = """
            namespace N
            {
                using Gu.Roslyn.Asserts;
                using NUnit.Framework;

                public static class Valid
                {
                    private static readonly PlaceholderAnalyzer Analyzer = new PlaceholderAnalyzer();

                    [Test]
                    public static void M1()
                    {
                        var c = "class C { }";
                        RoslynAssert.Valid(Analyzer, c);
                    }
                }
            }
            """;

        var diagnosticsAfter = """
            namespace N
            {
                using Gu.Roslyn.Asserts;
                using NUnit.Framework;

                public static class Diagnostics
                {
                    private static readonly PlaceholderAnalyzer Analyzer = new PlaceholderAnalyzer();

                    [Test]
                    public static void M1()
                    {
                        var c = "class C { }";
                        RoslynAssert.Diagnostics(Analyzer, c);
                    }

                    [Test]
                    public static void M2()
                    {
                        var c = "class C { }";
                        RoslynAssert.Diagnostics(Analyzer, c);
                    }
                }
            }
            """;
        Assert.CodeFix(new[] { Code.PlaceholderAnalyzer, before, diagnosticsBefore }, new[] { Code.PlaceholderAnalyzer, valid, diagnosticsAfter });
    }
}
