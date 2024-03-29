﻿namespace Gu.Roslyn.Asserts.Analyzers.Tests.GURA08aShouldBeInternal;

using NUnit.Framework;

public static class CodeFix
{
    private static readonly DiagnosticFixAssert Assert = RoslynAssert.Create<ObjectCreationAnalyzer, AccessibilityFix>(
        ExpectedDiagnostic.Create(Descriptors.GURA08aShouldBeInternal));

    [Test]
    public static void DiagnosticAnalyzer()
    {
        var before = """
            namespace N
            {
                using System.Collections.Immutable;
                using Microsoft.CodeAnalysis;
                using Microsoft.CodeAnalysis.Diagnostics;

                public class Analyzer : DiagnosticAnalyzer
                {
                    /// <inheritdoc/>
                    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get; }

                    public override void Initialize(AnalysisContext context)
                    {
                    }

                    private static void Handle(SyntaxNodeAnalysisContext context)
                    {
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
                    private static readonly Analyzer Analyzer = new ↓Analyzer();

                    [TestCase("C2 { }")]
                    public static void M(string declaration)
                    {
                        var c1 = "class C1 { }";
                        var code = "class C2 { }".AssertReplace("C2 { }", declaration);
                        RoslynAssert.Diagnostics(Analyzer, c1, code);
                    }
                }
            }
            """;

        var after = """
            namespace N
            {
                using System.Collections.Immutable;
                using Microsoft.CodeAnalysis;
                using Microsoft.CodeAnalysis.Diagnostics;

                internal class Analyzer : DiagnosticAnalyzer
                {
                    /// <inheritdoc/>
                    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get; }

                    public override void Initialize(AnalysisContext context)
                    {
                    }

                    private static void Handle(SyntaxNodeAnalysisContext context)
                    {
                    }
                }
            }
            """;
        Assert.CodeFix(new[] { before, diagnostics }, new[] { after, diagnostics.AssertReplace("↓", string.Empty) });
    }
}
