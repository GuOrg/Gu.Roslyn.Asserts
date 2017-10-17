# Gu.Roslyn.Asserts

[![License](https://img.shields.io/badge/license-MIT-blue.svg)](LICENSE.md)
[![NuGet](https://img.shields.io/nuget/v/Gu.Roslyn.Asserts.svg)](https://www.nuget.org/packages/Gu.Roslyn.Asserts/)
[![Build status](https://ci.appveyor.com/api/projects/status/a0976a1dmtcx387r/branch/master?svg=true)](https://ci.appveyor.com/project/JohanLarsson/gu-roslyn-asserts/branch/master)

<!--
[![Build status](https://ci.appveyor.com/api/projects/status/a0976a1dmtcx387r/branch/master?svg=true)](https://ci.appveyor.com/project/JohanLarsson/gu-roslyn-asserts/branch/master)
-->

Asserts for testing Roslyn analyzers.

- [Valid](#valid)
- [Diagnostics](#diagnostics)
- [CodeFix](#codefix)
  - [Code fix only](#code-fix-only)
- [FixAll](#fixall)
- [NoFix](#nofix)
- [Attributes](#attributes)
  - [MetadataReferenceAttribute](#metadatareferenceattribute)
  - [MetadataReferencesAttribute](#metadatareferencesattribute)
    - [Sample AssemblyInfo.cs (for the test project.)](#sample-assemblyinfocs-for-the-test-project)
  - [Exlicit set AnalyzerAssert.MetadataReferences](#exlicit-set-analyzerassertmetadatareferences)
  - [IgnoredErrorsAttribute](#ignorederrorsattribute)
  - [AllowedDiagnosticsAttribute](#alloweddiagnosticsattribute)
- [Analyze](#analyze)
  - [GetDiagnosticsAsync](#getdiagnosticsasync)
- [CodeFactory](#codefactory)
  - [CreateSolution](#createsolution)
    - [Create a Microsoft.CodeAnalysis.AdhocWorkspace, a Roslyn Solution from code.](#create-a-microsoftcodeanalysisadhocworkspace--a-roslyn-solution-from-code)
    - [Create a Microsoft.CodeAnalysis.AdhocWorkspace, a Roslyn Solution from a file on disk.](#create-a-microsoftcodeanalysisadhocworkspace--a-roslyn-solution-from-a-file-on-disk)
- [SyntaxNodeExt](#syntaxnodeext)
- [Usage with different test project types](#usage-with-different-test-project-types)
  - [Net461 new project type.](#net461-new-project-type)
  - [NetCoreApp2.0](#netcoreapp20)


# Valid

Use `AnalyzerAssert.Valid<NoErrorAnalyzer>(code)` to test that an analyzer does not report errors for valid code.

```c#
[Test]
public void TestThatNoDiagnosticsAreReportedForTheCode()
{
    var code = @"
namespace Gu.Roslyn.Asserts.Tests
{
    class Foo
    {
    }
}";
    AnalyzerAssert.Valid<NoErrorAnalyzer>(code);
}
```

```c#
[TestCase(typeof(NoErrorAnalyzer))]
public void SingleClassNoErrorType(Type type)
{
    var code = @"
namespace RoslynSandbox
{
    class Foo
    {
    }
}";
    AnalyzerAssert.Valid(type, code);
}
```

# Diagnostics

Use `AnalyzerAssert.Diagnostics<FieldNameMustNotBeginWithUnderscore>(code)` to test that the analyzer reports error or warning at position indicated with ↓

```c#
[Test]
public void JustCheckPositionAndDiagnosticId()
{
    var code = @"
namespace RoslynSandbox
{
    class Foo
    {
        private readonly int ↓_value;
    }
}";
    AnalyzerAssert.Diagnostics<FieldNameMustNotBeginWithUnderscore>(code);
}

[Test]
public void CheckMessageAlso()
{
    var code = @"
namespace RoslynSandbox
{
    class Foo
    {
        private readonly int ↓_value;
    }
}";
    AnalyzerAssert.Diagnostics<FieldNameMustNotBeginWithUnderscore>(ExpectedMessage.Create("Field '_value' must not begin with an underscore"), code);
}

[Test]
public void CheckMessageViaArgsPassedToFormatString()
{
    var code = @"
namespace RoslynSandbox
{
    class Foo
    {
        private readonly int ↓_value;
    }
}";
    AnalyzerAssert.Diagnostics<FieldNameMustNotBeginWithUnderscore>(ExpectedMessage.Create(new[] { "_value" }), code);
}
```

# CodeFix
Test that the analyzer reports an error or warning at position indicated with ↓ and that the codefix fixes it and produces the expected code.

```c#
[Test]
public void TestThatAnalyzerWarnsOnCorrectPositionAndThatCodeFixProducesExpectedCode()
{
    var code = @"
namespace RoslynSandbox
{
    class Foo
    {
        private readonly int ↓_value;
    }
}";

    var fixedCode = @"
namespace RoslynSandbox
{
    class Foo
    {
        private readonly int value;
    }
}";
    AnalyzerAssert.CodeFix<FieldNameMustNotBeginWithUnderscore, SA1309CodeFixProvider>(code, fixedCode);
}
```

With explicit title for the fix to apply. Useful when there are many candidate fixes.

```c#
[Test]
public void TestThatAnalyzerWarnsOnCorrectPositionAndThatCodeFixProducesExpectedCode()
{
    var code = @"
namespace RoslynSandbox
{
    class Foo
    {
        private readonly int ↓_value;
    }
}";

    var fixedCode = @"
namespace RoslynSandbox
{
    class Foo
    {
        private readonly int value;
    }
}";
    AnalyzerAssert.CodeFix<FieldNameMustNotBeginWithUnderscore, SA1309CodeFixProvider>(code, fixedCode, "Rename to value");
}
```

## Code fix only

When the code fix is for a warning produced by an analyzer that you do not own, for example a built in analyzer in Visual Studio.
```c#
[Test]
public void TestThatCodeFixProducesExpectedCode()
{
    var code = @"
namespace RoslynSandbox
{
    using System;

    public class Foo
    {
        public event EventHandler ↓Bar;
    }
}";

    var fixedCode = @"
namespace RoslynSandbox
{
    using System;

    public class Foo
    {
    }
}";
    AnalyzerAssert.CodeFix<RemoveUnusedFixProvider>("CS0067", code, fixedCode);
}
```

# FixAll

When there are many isses that will be fixed:

```c#
[Test]
public void TestThatAnalyzerWarnsOnCorrectPositionAndThatCodeFixProducesExpectedCode()
{
    var code = @"
namespace RoslynSandbox
{
    class Foo
    {
        private readonly int ↓_value1;
        private readonly int ↓_value2;
    }
}";

    var fixedCode = @"
namespace RoslynSandbox
{
    class Foo
    {
        private readonly int value1;
        private readonly int value2;
    }
}";
    AnalyzerAssert.FixAll<FieldNameMustNotBeginWithUnderscore, SA1309CodeFixProvider>(code, fixedCode);
}
```

# NoFix

Test that the analyzer reports an error or warning at position indicated with ↓ and that the codefix does not change the code.
This can happen if for example it is decided to not support rare edge cases with the code fix.

```c#
[Test]
public void TestThatAnalyzerWarnsOnCorrectPositionAndThatCodeFixDoesNothing()
{
    var code = @"
namespace RoslynSandbox
{
    class Foo
    {
        private readonly int ↓_value;
    }
}";

    AnalyzerAssert.NoFix<FieldNameMustNotBeginWithUnderscore, SA1309CodeFixProvider>(code);
}
```

# Attributes

When creating the workspace to analyze metadata references need to be added. There are a couple of ways to provide them using this library.
Some overloads of the asserts allow passing explicit references but it will be verbose to do that everywhere.

## MetadataReferenceAttribute
For specifying a metadata reference to be used in the tests, with or without aliases.
```c#
[assembly: MetadataReference(typeof(object), new[] { "global", "corlib" })]
```

## MetadataReferencesAttribute

For specifying a batch of metadata references to be used in the tests.
```c#
[assembly: MetadataReferences(
    typeof(System.Linq.Enumerable),
    typeof(System.Net.WebClient),
    typeof(System.Reactive.Disposables.SerialDisposable),
    typeof(System.Reactive.Disposables.ICancelable),
    typeof(System.Reactive.Linq.Observable),
    typeof(Gu.Reactive.Condition),
    typeof(Gu.Wpf.Reactive.ConditionControl),
    typeof(System.Xml.Serialization.XmlSerializer),
    typeof(System.Windows.Media.Matrix),
    typeof(Microsoft.CodeAnalysis.CSharp.CSharpCompilation),
    typeof(Microsoft.CodeAnalysis.Compilation),
    typeof(NUnit.Framework.Assert))]
```

Calling `AnalyzerAssert.ResetMetadataReferences()` resets `AnalyzerAssert.MetadataReferences` to the list provided via the attribute or clears it if no attribute is provided.

## MetadataReferences

For getting all metadata references specified with attributes use:

```cs
var compilation = CSharpCompilation.Create(
    "TestProject",
    new[] { syntaxTree },
    MetadataReferences.FromAttributes());
```

### Sample AssemblyInfo.cs (for the test project.)

```c#
using System.Reflection;
using System.Runtime.InteropServices;
using Gu.Roslyn.Asserts;

...
[assembly: AssemblyVersion("1.0.0.0")]
[assembly: AssemblyFileVersion("1.0.0.0")]

[assembly: MetadataReference(typeof(object), new[] { "global", "corlib" })]
[assembly: MetadataReference(typeof(System.Diagnostics.Debug), new[] { "global", "system" })]
[assembly: MetadataReferences(
    typeof(System.Linq.Enumerable),
    typeof(System.Net.WebClient),
    typeof(System.Data.Common.DbConnection),
    typeof(System.Reactive.Disposables.SerialDisposable),
    typeof(System.Reactive.Disposables.ICancelable),
    typeof(System.Reactive.Linq.Observable),
    typeof(System.Xml.Serialization.XmlSerializer),
    typeof(System.Windows.Media.Brush),
    typeof(System.Windows.Controls.Control),
    typeof(System.Windows.Media.Matrix),
    typeof(System.Xaml.XamlLanguage),
    typeof(Microsoft.CodeAnalysis.CSharp.CSharpCompilation),
    typeof(Microsoft.CodeAnalysis.Compilation),
    typeof(NUnit.Framework.Assert))]
```
## Exlicit set AnalyzerAssert.MetadataReferences

```c#
AnalyzerAssert.MetadataReferences.Add(MetadataReference.CreateFromFile(typeof(int).Assembly.Location));
```

A helper like this can be used.
```c#
private static IReadOnlyList<MetadataReference> CreateMetadataReferences(params Type[] types)
{
    return types.Select(type => type.GetTypeInfo().Assembly)
                .Distinct()
                .Select(assembly => MetadataReference.CreateFromFile(assembly.Location))
                .ToArray();
}
```

## IgnoredErrorsAttribute

For globally ignoring compiler warnings and errors introduced by code fixes when calling calling AnalyzerAssert.CodeFix and AnalyzerAssert.FixAll.

```c#
[assembly: IgnoredErrors("CS1569", ...)]
```

## AllowedDiagnosticsAttribute

For globally ignoring compiler warnings and errors introduced by code fixes when calling calling AnalyzerAssert.CodeFix and AnalyzerAssert.FixAll.

```c#
[assembly: AllowedDiagnostics(AllowedDiagnostics.Warnings)]
```

# Analyze

## GetDiagnosticsAsync

Analyze a cs, csproj or sln file on disk.

```c#
[Test]
public async Task GetDiagnosticsFromProjectOnDisk()
{
    var dllFile = new Uri(Assembly.GetExecutingAssembly().CodeBase, UriKind.Absolute).LocalPath;
    Assert.AreEqual(true, CodeFactory.TryFindProjectFile(new FileInfo(dllFile), out FileInfo projectFile));
    var diagnostics = await Analyze.GetDiagnosticsAsync(new FieldNameMustNotBeginWithUnderscore(), projectFile, MetadataReferences)
                                    .ConfigureAwait(false);
    ...
}
```` 

# CodeFactory

## CreateSolution

### Create a Microsoft.CodeAnalysis.AdhocWorkspace, a Roslyn Solution from code.

```c#
        [Test]
        public void CreateSolutionFromSources()
        {
            var code = @"
namespace RoslynSandbox
{
    class Foo
    {
        private readonly int _value;
    }
}";
            var sln = CodeFactory.CreateSolution(code, new[] { new FieldNameMustNotBeginWithUnderscore() });
            Assert.AreEqual("RoslynSandbox", sln.Projects.Single().Name);
            Assert.AreEqual("Foo.cs", sln.Projects.Single().Documents.Single().Name);
        }

        [Test]
        public void CreateSolutionFromSources()
        {
            var code1 = @"
namespace Project1
{
    class Foo1
    {
        private readonly int _value;
    }
}";

            var code2 = @"
namespace Project2
{
    class Foo2
    {
        private readonly int _value;
    }
}";
            var sln = CodeFactory.CreateSolution(new[] { code1, code2 }, new[] { new FieldNameMustNotBeginWithUnderscore() });
            CollectionAssert.AreEqual(new[] { "Project1", "Project2" }, sln.Projects.Select(x => x.Name));
            Assert.AreEqual(new[] { "Foo1.cs", "Foo2.cs" }, sln.Projects.Select(x => x.Documents.Single().Name));
        }
```

### Create a Microsoft.CodeAnalysis.AdhocWorkspace, a Roslyn Solution from a file on disk.

```c#
[Test]
public void CreateSolutionFromProjectFile()
{
    Assert.AreEqual(
        true,
        CodeFactory.TryFindProjectFile(
            new FileInfo(new Uri(Assembly.GetExecutingAssembly().CodeBase, UriKind.Absolute).LocalPath),
            out FileInfo projectFile));
    var solution = CodeFactory.CreateSolution(
        projectFile,
        new[] { new FieldNameMustNotBeginWithUnderscore(), },
        CreateMetadataReferences(typeof(object)));
}

[Test]
public void CreateSolutionFromSolutionFile()
{
    Assert.AreEqual(
        true,
        CodeFactory.TryFindFileInParentDirectory(
            new FileInfo(new Uri(Assembly.GetExecutingAssembly().CodeBase, UriKind.Absolute).LocalPath).Directory, "Gu.Roslyn.Asserts.sln",
            out FileInfo solutionFile));
    var solution = CodeFactory.CreateSolution(
        solutionFile,
        new[] { new FieldNameMustNotBeginWithUnderscore(), },
        CreateMetadataReferences(typeof(object)));
}
```

# Benchmark

Sample benchmark using BenchmarkDotNet.

```
public class FieldNameMustNotBeginWithUnderscoreBenchmark
{
    private static readonly Solution Solution = CodeFactory.CreateSolution(
        CodeFactory.FindSolutionFile("Gu.Roslyn.Asserts.sln"),
        MetadataReferences.Transitive(typeof(Benchmark).Assembly).ToArray());

    private static readonly Benchmark Benchmark = Benchmark.Create(Solution, new FieldNameMustNotBeginWithUnderscore());

    [BenchmarkDotNet.Attributes.Benchmark]
    public void RunOnGuRoslynAssertsSln()
    {
        Benchmark.Run();
    }
}
```

# SyntaxNodeExt
```cs
        [Test]
        public void FindAssignmentExpressionDemo()
        {
            var syntaxTree = CSharpSyntaxTree.ParseText(
                @"
namespace RoslynSandbox
{
    internal class Foo
    {
        internal Foo()
        {
            var temp = 1;
            temp = 2;
        }
    }
}");
            var compilation = CSharpCompilation.Create("test", new[] { syntaxTree }, new[] { MetadataReference.CreateFromFile(typeof(object).Assembly.Location), });
            var semanticModel = compilation.GetSemanticModel(syntaxTree);
            var assignment = syntaxTree.FindAssignmentExpression("temp = 2");
            Assert.AreEqual("temp = 2", assignment.ToString());
            Assert.AreEqual("int", semanticModel.GetTypeInfo(assignment.Right).Type.ToDisplayString());
        }
```

# Usage with different test project types
## Net461 new project type.
```xml
<PropertyGroup>
  <AutoGenerateBindingRedirects>true</AutoGenerateBindingRedirects>
  <GenerateBindingRedirectsOutputType>true</GenerateBindingRedirectsOutputType>
</PropertyGroup>
```

## NetCoreApp2.0
TODO figure out what is needed here.

