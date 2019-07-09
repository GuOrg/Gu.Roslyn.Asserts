namespace Gu.Roslyn.Asserts.Tests
{
    using System;
    using System.Collections.Immutable;
    using System.Globalization;
    using System.Linq;
    using System.Text.RegularExpressions;
    using System.Threading;
    using Gu.Roslyn.Asserts.Internals;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CodeFixes;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.Diagnostics;
    using NUnit.Framework;

    public static partial class RoslynAssertTests
    {
        public static class ApiTests
        {
            private static readonly Project Project = CodeFactory.CreateSolution(
                                                                     ProjectFile.Find("Gu.Roslyn.Asserts.csproj"),
                                                                     Asserts.MetadataReferences.Transitive(typeof(CSharpCompilation)))
                                                                 .Projects
                                                                 .Single();

            private static readonly INamedTypeSymbol RoslynAssertType = Project
                                                                        .GetCompilationAsync(CancellationToken.None)
                                                                        .Result
                                                                        .GetTypeByMetadataName(typeof(RoslynAssert).FullName);

            private static readonly ImmutableArray<IMethodSymbol> CodeFixMethods = GetMethods(RoslynAssertType, nameof(RoslynAssert.CodeFix));
            private static readonly ImmutableArray<IMethodSymbol> DiagnosticsMethods = GetMethods(RoslynAssertType, nameof(RoslynAssert.Diagnostics));
            private static readonly ImmutableArray<IMethodSymbol> FixAllMethods = GetMethods(RoslynAssertType, nameof(RoslynAssert.FixAll));
            private static readonly ImmutableArray<IMethodSymbol> ValidMethods = GetMethods(RoslynAssertType, nameof(RoslynAssert.Valid));

            [TestCaseSource(nameof(CodeFixMethods))]
            [TestCaseSource(nameof(DiagnosticsMethods))]
            [TestCaseSource(nameof(FixAllMethods))]
            [TestCaseSource(nameof(ValidMethods))]
            public static void AnalyzerParameter(IMethodSymbol method)
            {
                if (TryFindByType<DiagnosticAnalyzer>(method.Parameters, out var parameter))
                {
                    Assert.AreEqual(0, parameter.Ordinal);
                    Assert.AreEqual(false, parameter.IsOptional);
                    Assert.AreEqual("analyzer", parameter.MetadataName);
                    StringAssert.IsMatch("The <see cref=\"T:Microsoft.CodeAnalysis.Diagnostics.DiagnosticAnalyzer\"/> to check <paramref name=\"\\w+\"/> with.", GetComment(parameter));
                }
                else if (TryFindByType<Type>(method.Parameters, out parameter))
                {
                    Assert.AreEqual(0, parameter.Ordinal);
                    Assert.AreEqual(false, parameter.IsOptional);
                    Assert.AreEqual("analyzerType", parameter.MetadataName);
                    StringAssert.IsMatch("The type of <see cref=\"T:Microsoft.CodeAnalysis.Diagnostics.DiagnosticAnalyzer\"/> to check <paramref name=\"\\w+\"/> with.", GetComment(parameter));
                }
                else
                {
                    Assert.AreEqual(typeof(CodeFixProvider).Name, method.Parameters[0].Type.MetadataName);
                }
            }

            [TestCaseSource(nameof(CodeFixMethods))]
            [TestCaseSource(nameof(DiagnosticsMethods))]
            [TestCaseSource(nameof(FixAllMethods))]
            public static void AllowCompilationErrorsParameter(IMethodSymbol method)
            {
                if (TryFindByType<AllowCompilationErrors>(method.Parameters, out var parameter))
                {
                    Assert.AreEqual(true, parameter.IsOptional);
                    Assert.AreEqual(AllowCompilationErrors.No, (AllowCompilationErrors)parameter.ExplicitDefaultValue);
                    Assert.AreEqual("allowCompilationErrors", parameter.MetadataName);
                    Assert.AreEqual("Specify if compilation errors are accepted in the fixed code. This can be for example syntax errors. Default value is <see cref=\"F:Gu.Roslyn.Asserts.AllowCompilationErrors.No\"/>.", GetComment(parameter));
                }
                else
                {
                    Assert.AreEqual(true, method.Parameters.Last().IsParams);
                }
            }

            private static ImmutableArray<IMethodSymbol> GetMethods(INamedTypeSymbol containingType, string name)
            {
                return ImmutableArray.CreateRange(RoslynAssertType
                                                  .GetMembers(name)
                                                  .Cast<IMethodSymbol>()
                                                  .Where(x => x.DeclaredAccessibility == Accessibility.Public &&
                                                              !x.IsGenericMethod));
            }

            private static bool TryFindByType<T>(ImmutableArray<IParameterSymbol> parameters, out IParameterSymbol parameter)
            {
                return parameters.TrySingle(x => x.Type.MetadataName == typeof(T).Name, out parameter);
            }

            private static string GetComment(IParameterSymbol parameter)
            {
                var xml = parameter.ContainingSymbol.GetDocumentationCommentXml(CultureInfo.InvariantCulture);
                return Regex.Match(xml, $"<param name=\"{parameter.Name}\">(?<text>.+)</param>").Groups["text"].Value;
            }
        }
    }
}
