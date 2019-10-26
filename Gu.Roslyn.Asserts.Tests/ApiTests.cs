namespace Gu.Roslyn.Asserts.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Collections.Immutable;
    using System.Diagnostics.CodeAnalysis;
    using System.Globalization;
    using System.Linq;
    using System.Threading;
    using Gu.Roslyn.Asserts.Internals;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CodeFixes;
    using Microsoft.CodeAnalysis.CodeRefactorings;
    using Microsoft.CodeAnalysis.Diagnostics;
    using Microsoft.CodeAnalysis.Text;
    using NUnit.Framework;

    public static class ApiTests
    {
        private static readonly Project Project = CodeFactory.CreateSolution(
                                                                 ProjectFile.Find("Gu.Roslyn.Asserts.csproj"),
                                                                 Asserts.MetadataReferences.Transitive(typeof(CodeFixProvider)))
                                                             .Projects
                                                             .Single();

        private static readonly INamedTypeSymbol RoslynAssertType = Project
                                                                    .GetCompilationAsync(CancellationToken.None)
                                                                    .Result!
                                                                    .GetTypeByMetadataName(typeof(RoslynAssert).FullName);

        private static readonly ImmutableArray<IMethodSymbol> CodeFixMethods = GetMethods(RoslynAssertType, nameof(RoslynAssert.CodeFix));
        private static readonly ImmutableArray<IMethodSymbol> DiagnosticsMethods = GetMethods(RoslynAssertType, nameof(RoslynAssert.Diagnostics));
        private static readonly ImmutableArray<IMethodSymbol> FixAllMethods = GetMethods(RoslynAssertType, nameof(RoslynAssert.FixAll), nameof(RoslynAssert.FixAllInDocument), nameof(RoslynAssert.FixAllByScope), nameof(RoslynAssert.FixAllOneByOne));
        private static readonly ImmutableArray<IMethodSymbol> NoCompilerErrorsMethods = GetMethods(RoslynAssertType, nameof(RoslynAssert.NoCompilerErrors));
        private static readonly ImmutableArray<IMethodSymbol> NoAnalyzerDiagnosticsMethods = GetMethods(RoslynAssertType, nameof(RoslynAssert.NoAnalyzerDiagnostics));
        private static readonly ImmutableArray<IMethodSymbol> NoFixMethods = GetMethods(RoslynAssertType, nameof(RoslynAssert.NoFix));
        private static readonly ImmutableArray<IMethodSymbol> NoRefactoringMethods = GetMethods(RoslynAssertType, nameof(RoslynAssert.NoRefactoring));
        private static readonly ImmutableArray<IMethodSymbol> RefactoringMethods = GetMethods(RoslynAssertType, nameof(RoslynAssert.Refactoring));
        private static readonly ImmutableArray<IMethodSymbol> ValidMethods = GetMethods(RoslynAssertType, nameof(RoslynAssert.Valid));

        [TestCaseSource(nameof(CodeFixMethods))]
        [TestCaseSource(nameof(DiagnosticsMethods))]
        [TestCaseSource(nameof(FixAllMethods))]
        [TestCaseSource(nameof(NoCompilerErrorsMethods))]
        [TestCaseSource(nameof(NoAnalyzerDiagnosticsMethods))]
        [TestCaseSource(nameof(NoFixMethods))]
        [TestCaseSource(nameof(ValidMethods))]
        public static void AnalyzerParameter(IMethodSymbol method)
        {
            if (method.TryFindParameterByType<DiagnosticAnalyzer>(out var parameter))
            {
                Assert.AreEqual(0, parameter!.Ordinal);
                Assert.AreEqual(false, parameter.IsOptional, "Optional.");
                Assert.AreEqual("analyzer", parameter.MetadataName);
                StringAssert.IsMatch("The <see cref=\"T:Microsoft.CodeAnalysis.Diagnostics.DiagnosticAnalyzer\"/> to check <paramref name=\"\\w+\"/> with.", parameter.DocComment());
            }
            else if (method.TryFindParameterByType<Type>(out parameter))
            {
                Assert.AreEqual(0, parameter!.Ordinal);
                Assert.AreEqual(false, parameter.IsOptional);
                Assert.AreEqual("analyzerType", parameter.MetadataName);
                StringAssert.IsMatch("The type of <see cref=\"T:Microsoft.CodeAnalysis.Diagnostics.DiagnosticAnalyzer\"/> to check <paramref name=\"\\w+\"/> with.", parameter.DocComment());
            }
            else if (method.Name != nameof(RoslynAssert.NoCompilerErrors))
            {
                Assert.AreEqual(typeof(CodeFixProvider).Name, method.Parameters[0].Type.MetadataName);
            }
        }

        [TestCaseSource(nameof(CodeFixMethods))]
        [TestCaseSource(nameof(FixAllMethods))]
        [TestCaseSource(nameof(NoFixMethods))]
        public static void CodeFixParameter(IMethodSymbol method)
        {
            Assert.AreEqual(true, method.TryFindParameterByType<CodeFixProvider>(out var parameter), "Missing.");
            switch (parameter!.Ordinal)
            {
                case 0:
                    Assert.AreEqual(typeof(ExpectedDiagnostic).Name, method.Parameters[1].Type.MetadataName);
                    break;
                case 1:
                    Assert.AreEqual(typeof(DiagnosticAnalyzer).Name, method.Parameters[0].Type.MetadataName);
                    break;
                default:
                    Assert.Fail("Position");
                    break;
            }

            Assert.AreEqual(false, parameter.IsOptional, "Optional.");
            Assert.AreEqual("fix", parameter.MetadataName);
            Assert.AreEqual("The <see cref=\"T:Microsoft.CodeAnalysis.CodeFixes.CodeFixProvider\"/> to apply on the <see cref=\"T:Microsoft.CodeAnalysis.Diagnostic\"/> reported.", parameter.DocComment());
        }

        [TestCaseSource(nameof(CodeFixMethods))]
        [TestCaseSource(nameof(DiagnosticsMethods))]
        [TestCaseSource(nameof(FixAllMethods))]
        [TestCaseSource(nameof(NoFixMethods))]
        public static void ExpectedDiagnosticParameter(IMethodSymbol method)
        {
            if (method.TryFindParameterByType<ExpectedDiagnostic>(out var parameter))
            {
                Assert.AreEqual(false, parameter!.IsOptional, "Optional.");
                Assert.AreEqual("expectedDiagnostic", parameter.MetadataName);
                if (method.TryFindParameterByType<DiagnosticAnalyzer>(out _))
                {
                    Assert.AreEqual("The <see cref=\"T:Gu.Roslyn.Asserts.ExpectedDiagnostic\"/> with information about the expected <see cref=\"T:Microsoft.CodeAnalysis.Diagnostic\"/>. If <paramref name=\"analyzer\"/> supports more than one <see cref=\"P:Microsoft.CodeAnalysis.DiagnosticDescriptor.Id\"/> this must be provided.", parameter.DocComment());
                }
                else
                {
                    Assert.AreEqual("The <see cref=\"T:Gu.Roslyn.Asserts.ExpectedDiagnostic\"/> with information about the expected <see cref=\"T:Microsoft.CodeAnalysis.Diagnostic\"/>.", parameter.DocComment());
                }
            }
            else
            {
                Assert.AreEqual(true, method.TryFindParameterByType<DiagnosticAnalyzer>(out _));
            }
        }

        [TestCaseSource(nameof(ValidMethods))]
        public static void DiagnosticDescriptorParameter(IMethodSymbol method)
        {
            if (method.TryFindParameterByType<DiagnosticDescriptor>(out var parameter))
            {
                Assert.AreEqual("descriptor", parameter!.MetadataName);
                Assert.AreEqual(false, parameter.IsOptional, "Optional.");
                string expected = $"The <see cref=\"T:Microsoft.CodeAnalysis.DiagnosticDescriptor\"/> with information about the expected <see cref=\"T:Microsoft.CodeAnalysis.Diagnostic\"/>. If <paramref name=\"{method.Parameters[0].Name}\"/> supports more than one <see cref=\"P:Microsoft.CodeAnalysis.DiagnosticDescriptor.Id\"/> this must be provided.";
                Assert.AreEqual(expected, parameter.DocComment());
            }
        }

        [TestCaseSource(nameof(CodeFixMethods))]
        [TestCaseSource(nameof(FixAllMethods))]
        [TestCaseSource(nameof(RefactoringMethods))]
        public static void BeforeParameter(IMethodSymbol method)
        {
            if (!method.TryFindParameterByType<DiagnosticsAndSources>(out _) &&
                !method.TryFindParameterByType<Solution>(out _))
            {
                Assert.AreEqual(true, method.Parameters.TrySingle(x => x.Name == "before", out var parameter));
                Assert.AreEqual(false, parameter!.IsOptional, "Optional.");
                if (method.TryFindParameterByType<DiagnosticAnalyzer>(out var analyzerParameter))
                {
                    Assert.AreEqual($"The code to analyze with <paramref name=\"{analyzerParameter!.Name}\"/>. Indicate error position with ↓ (alt + 25).", parameter.DocComment());
                }
                else if (method.TryFindParameterByType<CodeRefactoringProvider>(out var refactoringParameter))
                {
                    if (method.TryFindParameterByType<TextSpan>(out var spanParameter))
                    {
                        Assert.AreEqual($"The code to analyze with <paramref name=\"{refactoringParameter!.Name}\"/>. Position is provided by <paramref name=\"{spanParameter!.Name}\"/>.", parameter.DocComment());
                    }
                    else
                    {
                        Assert.AreEqual($"The code to analyze with <paramref name=\"{refactoringParameter!.Name}\"/>. Indicate position with ↓ (alt + 25).", parameter.DocComment());
                    }
                }
                else
                {
                    Assert.AreEqual("The code to analyze for <paramref name=\"expectedDiagnostic\"/>. Indicate error position with ↓ (alt + 25).", parameter.DocComment());
                }
            }
        }

        [TestCaseSource(nameof(CodeFixMethods))]
        [TestCaseSource(nameof(FixAllMethods))]
        [TestCaseSource(nameof(RefactoringMethods))]
        public static void AfterParameter(IMethodSymbol method)
        {
            Assert.AreEqual(true, method.Parameters.TrySingle(x => x.Name == "after", out var parameter));
            Assert.AreEqual(false, parameter!.IsOptional, "Optional.");
            if (method.TryFindParameterByType<CodeFixProvider>(out var fix))
            {
                Assert.AreEqual($"The expected code produced by applying <paramref name=\"{fix!.Name}\"/>.", parameter.DocComment());
            }
            else if (method.TryFindParameterByType<CodeRefactoringProvider>(out var refactoring))
            {
                Assert.AreEqual($"The expected code produced by <paramref name=\"{refactoring!.Name}\"/>.", parameter.DocComment());
            }
        }

        [TestCaseSource(nameof(ValidMethods))]
        [TestCaseSource(nameof(NoAnalyzerDiagnosticsMethods))]
        public static void ValidCodeParameter(IMethodSymbol method)
        {
            if (method.TryFindParameterByType<Solution>(out var sln))
            {
                Assert.AreEqual("solution", sln!.MetadataName);
                Assert.AreEqual(false, sln.IsOptional, "Optional.");
            }
            else
            {
                Assert.AreEqual(true, method.Parameters.TrySingle(x => x.Name == "code", out var parameter));
                Assert.AreEqual(false, parameter!.IsOptional, "Optional.");
                switch (parameter.Type.Name)
                {
                    case "FileInfo":
                    case "Solution":
                        break;
                    default:
                        Assert.AreEqual($"The code to analyze using <paramref name=\"{method.Parameters[0].Name}\"/>. Analyzing the code is expected to produce no errors or warnings.", parameter.DocComment());
                        break;
                }
            }
        }

        [TestCaseSource(nameof(NoRefactoringMethods))]
        public static void NoRefactoringCodeParameter(IMethodSymbol method)
        {
            Assert.AreEqual(true, method.Parameters.TrySingle(x => x.Name == "code", out var parameter));
            Assert.AreEqual(false, parameter!.IsOptional, "Optional.");
            if (method.TryFindParameterByType<TextSpan>(out var spanParameter))
            {
                Assert.AreEqual($"The code to analyze with <paramref name=\"refactoring\"/>. Position is provided by <paramref name=\"{spanParameter!.Name}\"/>.", parameter.DocComment());
            }
            else
            {
                Assert.AreEqual($"The code to analyze with <paramref name=\"refactoring\"/>. Indicate position with ↓ (alt + 25).", parameter.DocComment());
            }
        }

        [TestCaseSource(nameof(DiagnosticsMethods))]
        [TestCaseSource(nameof(NoCompilerErrorsMethods))]
        [TestCaseSource(nameof(NoFixMethods))]
        public static void CodeParameter(IMethodSymbol method)
        {
            if (method.TryFindParameterByType<Solution>(out var sln))
            {
                Assert.AreEqual("solution", sln!.MetadataName);
                Assert.AreEqual(false, sln.IsOptional, "Optional.");
            }
            else if (method.TryFindParameterByType<DiagnosticsAndSources>(out _))
            {
            }
            else
            {
                Assert.AreEqual(true, method.Parameters.TrySingle(x => x.Name == "code", out var parameter));
                Assert.AreEqual(false, parameter!.IsOptional, "Optional.");
                switch (parameter.Type.Name)
                {
                    case "FileInfo":
                    case "Solution":
                        break;
                    default:
                        switch (method.Parameters[0].Name)
                        {
                            case "analyzer":
                                Assert.AreEqual($"The code to analyze with <paramref name=\"analyzer\"/>. Indicate error position with ↓ (alt + 25).", parameter.DocComment());
                                break;
                            case "analyzerType":
                                Assert.AreEqual($"The code to analyze with <paramref name=\"analyzerType\"/>. Indicate error position with ↓ (alt + 25).", parameter.DocComment());
                                break;
                            case "fix":
                                Assert.AreEqual($"The code to analyze. Indicate error position with ↓ (alt + 25).", parameter.DocComment());
                                break;
                            default:
                                Assert.Inconclusive($"Not handling {method.Parameters[0]}");
                                break;
                        }

                        break;
                }
            }
        }

        [TestCaseSource(nameof(CodeFixMethods))]
        [TestCaseSource(nameof(DiagnosticsMethods))]
        [TestCaseSource(nameof(FixAllMethods))]
        [TestCaseSource(nameof(NoFixMethods))]
        public static void AllowCompilationErrorsParameter(IMethodSymbol method)
        {
            if (method.TryFindParameterByType<AllowCompilationErrors>(out var parameter))
            {
                Assert.AreEqual(true, parameter!.IsOptional, "Not optional.");
                Assert.AreEqual(AllowCompilationErrors.No, (AllowCompilationErrors)parameter.ExplicitDefaultValue);
                Assert.AreEqual("allowCompilationErrors", parameter.MetadataName);
                Assert.AreEqual("Specify if compilation errors are accepted in the fixed code. This can be for example syntax errors. Default value is <see cref=\"F:Gu.Roslyn.Asserts.AllowCompilationErrors.No\"/>.", parameter.DocComment());
            }
            else
            {
                Assert.AreEqual(true, method.Parameters.Last().IsParams || method.GetAttributes().Any(), "Missing.");
            }
        }

        [TestCaseSource(nameof(CodeFixMethods))]
        [TestCaseSource(nameof(DiagnosticsMethods))]
        [TestCaseSource(nameof(FixAllMethods))]
        [TestCaseSource(nameof(NoFixMethods))]
        public static void SuppressWarningsParameter(IMethodSymbol method)
        {
            if (method.TryFindParameterByType<IEnumerable<string>>(out var parameter))
            {
                Assert.AreEqual(true, parameter!.IsOptional, "Not optional.");
                Assert.AreEqual(null, parameter.ExplicitDefaultValue);
                Assert.AreEqual("suppressWarnings", parameter.MetadataName);
                Assert.AreEqual("A collection of <see cref=\"P:Microsoft.CodeAnalysis.DiagnosticDescriptor.Id\"/> to suppress when analyzing the code. Default is <see langword=\"null\" /> meaning <see cref=\"F:Gu.Roslyn.Asserts.RoslynAssert.SuppressedDiagnostics\"/> are used.", parameter.DocComment());
                Assert.AreEqual("allowCompilationErrors", method.Parameters[parameter.Ordinal - 1].Name);
            }
            else
            {
                Assert.AreEqual(true, method.Parameters.Any(x => x.Type.MetadataName == typeof(Solution).Name) || method.Parameters.Last().IsParams || method.GetAttributes().Any(), "Missing.");
            }
        }

        [TestCaseSource(nameof(CodeFixMethods))]
        [TestCaseSource(nameof(DiagnosticsMethods))]
        [TestCaseSource(nameof(FixAllMethods))]
        [TestCaseSource(nameof(NoCompilerErrorsMethods))]
        [TestCaseSource(nameof(NoFixMethods))]
        [TestCaseSource(nameof(ValidMethods))]
        public static void MetadataReferencesParameter(IMethodSymbol method)
        {
            if (method.TryFindParameterByType<IEnumerable<MetadataReference>>(out var parameter))
            {
                if (!method.Parameters.Last().IsParams)
                {
                    Assert.AreEqual(true, parameter!.IsOptional, "Not optional.");
                    Assert.AreEqual(null, parameter.ExplicitDefaultValue);
                    Assert.AreEqual("suppressWarnings", method.Parameters[parameter.Ordinal - 1].Name);
                }

                Assert.AreEqual("metadataReferences", parameter!.MetadataName);
                Assert.AreEqual("A collection of <see cref=\"T:Microsoft.CodeAnalysis.MetadataReference\"/> to use when compiling. Default is <see langword=\"null\" /> meaning <see cref=\"F:Gu.Roslyn.Asserts.RoslynAssert.MetadataReferences\"/> are used.", parameter.DocComment());
            }
            else
            {
                Assert.AreEqual(true, method.Parameters.Any(x => x.Type.MetadataName == typeof(Solution).Name) || method.Parameters.Last().IsParams || method.GetAttributes().Any(), "Missing.");
            }
        }

        private static ImmutableArray<IMethodSymbol> GetMethods(INamedTypeSymbol containingType, string name, params string[] names)
        {
            names = new[] { name }.Concat(names ?? Enumerable.Empty<string>()).ToArray();
            return ImmutableArray.CreateRange(
                names.SelectMany(x => containingType.GetMembers(x))
                     .Cast<IMethodSymbol>()
                     .Where(x => x.DeclaredAccessibility == Accessibility.Public &&
                                 !x.IsGenericMethod &&
                                 !IsObsolete(x)));
        }

        private static bool TryFindParameterByType<T>(this IMethodSymbol method, out IParameterSymbol? parameter)
        {
            if (typeof(T).IsGenericType)
            {
                return method.Parameters.TrySingle(
                    x => x!.Type is INamedTypeSymbol namedType &&
                         namedType.IsGenericType &&
                         namedType.MetadataName == typeof(T).Name &&
                         namedType.TypeArguments[0].MetadataName == typeof(T).GenericTypeArguments[0].Name,
                    out parameter);
            }

            return method.Parameters.TrySingle(x => x!.Type.MetadataName == typeof(T).Name, out parameter);
        }

        private static string DocComment(this IParameterSymbol parameter)
        {
            var xml = parameter.ContainingSymbol.GetDocumentationCommentXml(CultureInfo.InvariantCulture);
            var prefix = $"<param name=\"{parameter.Name}\">";
            var start = xml.IndexOf(prefix, StringComparison.Ordinal);
            if (start > 0)
            {
                var end = xml.IndexOf("</param>", start, StringComparison.Ordinal);
                return xml.Substring(start + prefix.Length, end - start - prefix.Length);
            }

            throw new InvalidOperationException($"Did not find a comment for {parameter.Name}");
        }

        private static bool IsObsolete(IMethodSymbol method) => method.GetAttributes().Any(x => x.AttributeClass.MetadataName == "ObsoleteAttribute");
    }
}
