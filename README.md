# Gu.Roslyn.Asserts

[![License](https://img.shields.io/badge/license-MIT-blue.svg)](LICENSE)
[![NuGet](https://img.shields.io/nuget/v/Gu.Roslyn.Asserts.svg)](https://www.nuget.org/packages/Gu.Roslyn.Asserts/)
[![Build status](https://ci.appveyor.com/api/projects/status/a0976a1dmtcx387r/branch/master?svg=true)](https://ci.appveyor.com/project/JohanLarsson/gu-roslyn-asserts/branch/master)
[![Gitter](https://badges.gitter.im/GuOrg/Gu.Roslyn.Asserts.svg)](https://gitter.im/GuOrg/Gu.Roslyn.Asserts?utm_source=badge&utm_medium=badge&utm_campaign=pr-badge)

Microsoft are working on an official package for testing analyzers: [Microsoft.CodeAnalysis.CSharp.CodeFix.Testing](https://dotnet.myget.org/feed/roslyn-analyzers/package/nuget/Microsoft.CodeAnalysis.CSharp.CodeFix.Testing.XUnit).

Hopefully this library will not be needed in the future.

<!--
[![Build status](https://ci.appveyor.com/api/projects/status/a0976a1dmtcx387r/branch/master?svg=true)](https://ci.appveyor.com/project/JohanLarsson/gu-roslyn-asserts/branch/master)
-->

Asserts for testing Roslyn analyzers.

Use 1.x for Microsoft.CodeAnalysis 1.x

- [RoslynAssert.Valid](#roslynassertvalid)
- [RoslynAssert.Diagnostics](#roslynassertdiagnostics)
- [CodeFix](#codefix)
  - [Code fix only](#code-fix-only)
- [FixAll](#fixall)
- [NoFix](#nofix)
- [Refactoring](#refactoring)
- [AST](#ast)
  - [SyntaxFactoryWriter](#syntaxfactorywriter)
- [Attributes](#attributes)
  - [MetadataReferenceAttribute](#metadatareferenceattribute)
  - [MetadataReferencesAttribute](#metadatareferencesattribute)
  - [MetadataReferences](#metadatareferences)
    - [Sample AssemblyInfo.cs (for the test project.)](#sample-assemblyinfocs-for-the-test-project)
  - [Exlicit set RoslynAssert.MetadataReferences](#exlicit-set-roslynassertmetadatareferences)
  - [IgnoredErrorsAttribute](#ignorederrorsattribute)
  - [AllowedDiagnosticsAttribute](#alloweddiagnosticsattribute)
- [Analyze](#analyze)
  - [GetDiagnosticsAsync](#getdiagnosticsasync)
- [Fix](#fix)
- [CodeFactory](#codefactory)
  - [CreateSolution](#createsolution)
    - [Create a Microsoft.CodeAnalysis.AdhocWorkspace, a Roslyn Solution from code.](#create-a-microsoftcodeanalysisadhocworkspace--a-roslyn-solution-from-code)
    - [Create a Microsoft.CodeAnalysis.AdhocWorkspace, a Roslyn Solution from a file on disk.](#create-a-microsoftcodeanalysisadhocworkspace--a-roslyn-solution-from-a-file-on-disk)
- [Benchmark](#benchmark)
- [SyntaxNodeExt](#syntaxnodeext)
- [AstView](#astview)
- [Usage with different test project types](#usage-with-different-test-project-types)
  - [Net472 new project type.](#net472-new-project-type)
  - [NetCoreApp2.0](#netcoreapp20)


# RoslynAssert.Valid

Use `RoslynAssert.Valid<NoErrorAnalyzer>(code)` to test that an analyzer does not report errors for valid code.
The code is checked so that it does not have any compiler errors either.
A typical test fixture looks like:

```c#
public class ValidCode
{
    private static readonly DiagnosticAnalyzer Analyzer = new YourAnalyzer();

    [Test]
    public void SomeTest()
    {
        var code = @"
namespace TestCode
{
    class Foo
    {
    }
}";
        RoslynAssert.Valid(Analyzer, code);
    }
    ...
}
```

If the analyzer produces many diagnostics you can pass in a descriptor so that only diagnostics matching it are checked.

```c#
public class ValidCode
{
    private static readonly DiagnosticAnalyzer Analyzer = new YourAnalyzer();
    private static readonly DiagnosticDescriptor Descriptor = YourAnalyzer.SomeDescriptor;

    [Test]
    public void SomeTest()
    {
        var code = @"
namespace TestCode
{
    class Foo
    {
    }
}";
        RoslynAssert.Valid(Analyzer, Descriptor, code);
    }
    ...
}
```

When testing all analyzers something like this can be used:

```c#
public class ValidCodeWithAllAnalyzers
{
    private static readonly IReadOnlyList<DiagnosticAnalyzer> AllAnalyzers = typeof(KnownSymbol)
                                                                                .Assembly.GetTypes()
                                                                                .Where(typeof(DiagnosticAnalyzer).IsAssignableFrom)
                                                                                .Select(t => (DiagnosticAnalyzer)Activator.CreateInstance(t))
                                                                                .ToArray();


    private static readonly Solution ValidCodeProjectSln = CodeFactory.CreateSolution(
        ProjectFile.Find("ValidCode.csproj"),
        AllAnalyzers,
        RoslynAssert.MetadataReferences);

    [TestCaseSource(nameof(AllAnalyzers))]
    public void ValidCodeProject(DiagnosticAnalyzer analyzer)
    {
        RoslynAssert.Valid(analyzer, ValidCodeProjectSln);
    }
}
```

# RoslynAssert.Diagnostics

Use `RoslynAssert.Diagnostics<FieldNameMustNotBeginWithUnderscore>(code)` to test that the analyzer reports error or warning at position indicated with ↓
With an aplhanumeric keyboard `alt + 25` writes `↓`.

A typical test fixture looks like:

```c#
public class Diagnostics
{
    private static readonly DiagnosticAnalyzer Analyzer = new YourAnalyzer();
    private static readonly ExpectedDiagnostic ExpectedDiagnostic = ExpectedDiagnostic.Create(YourAnalyzer.Descriptor);

    [Test]
    public void SomeTest()
    {
        var code = @"
namespace TestCode
{
    class ↓Foo
    {
    }
}";
        RoslynAssert.Diagnostics(Analyzer, code);
    }

    [Test]
    public void CheckMessageAlso()
    {
        var code = @"
namespace TestCode
{
    class ↓Foo
    {
    }
}";
        RoslynAssert.Diagnostics(Analyzer, ExpectedDiagnostic.WithMessage("Don't name it foo"), code);
    }
    ...
}
```

If the analyzer produces many diagnostics you can pass in a descriptor so that only diagnostics matching it are checked.

```c#
public class Diagnostics
{
    private static readonly DiagnosticAnalyzer Analyzer = new YourAnalyzer();
    private static readonly ExpectedDiagnostic ExpectedDiagnostic = ExpectedDiagnostic.Create(YourAnalyzer.Descriptor);

    [Test]
    public void SomeTest()
    {
        var code = @"
namespace TestCode
{
    class ↓Foo
    {
    }
}";
        RoslynAssert.Diagnostics(Analyzer, ExpectedDiagnostic, code);
    }
    ...
}
```

If the analyzer supports many diagnostics the overload with `ExpectedDiagnostic` must be used. This suppresses all diagnsics other than the expected.

# CodeFix
Test that the analyzer reports an error or warning at position indicated with ↓ and that the codefix fixes it and produces the expected code.
With an aplhanumeric keyboard `alt + 25` writes `↓`.

```c#
public class CodeFixTests
{
    private static readonly DiagnosticAnalyzer Analyzer = new FieldNameMustNotBeginWithUnderscore();
    private static readonly CodeFixProvider Fix = new SA1309CodeFixProvider();

	[Test]
	public void TestThatAnalyzerWarnsOnCorrectPositionAndThatCodeFixProducesExpectedCode()
	{
		var before = @"
	namespace N
	{
		class Foo
		{
			private readonly int ↓_value;
		}
	}";

		var after = @"
	namespace N
	{
		class Foo
		{
			private readonly int value;
		}
	}";
		RoslynAssert.CodeFix(Analyzer, Fix, before, after);
	}
}

```

A typical test fixture looks like:

```c#
public class CodeFixTests
{
    private static readonly DiagnosticAnalyzer Analyzer = new YourAnalyzer();
    private static readonly CodeFixProvider Fix = new YorCodeFixProvider();
    private static readonly ExpectedDiagnostic ExpectedDiagnostic = ExpectedDiagnostic.Create(YourAnalyzer.Descriptor);

    [Test]
    public void SomeTest()
    {
        var before = @"
namespace N
{
    class Foo
    {
        private readonly int ↓_value;
    }
}";

        var after = @"
namespace N
{
    class Foo
    {
        private readonly int value;
    }
}";
        RoslynAssert.CodeFix(Analyzer, Fix, before, after);
    }

    [Test]
    public void ExplicitFixTitle()
    {
        var before = @"
namespace N
{
    class Foo
    {
        private readonly int ↓_value;
    }
}";

        var after = @"
namespace N
{
    class Foo
    {
        private readonly int value;
    }
}";
        RoslynAssert.CodeFix(Analyzer, Fix, before, after, fixTitle: "Don't use underscore prefix");
    }
    ...
}
```

With explicit title for the fix to apply. Useful when there are many candidate fixes.


If the analyzer supports many diagnostics the overload with `ExpectedDiagnostic` must be used. This suppresses all diagnsics other than the expected.

## Code fix only

When the code fix is for a warning produced by an analyzer that you do not own, for example a built in analyzer in Visual Studio.
```c#
public class CodeFixTests
{
    private static readonly CodeFixProvider Fix = new RemoveUnusedFixProvider();
    private static readonly ExpectedDiagnostic ExpectedDiagnostic = ExpectedDiagnostic.Create("CS0067");

	[Test]
	public void TestThatCodeFixProducesExpectedCode()
	{
		var before = @"
	namespace N
	{
		using System;

		public class Foo
		{
			public event EventHandler ↓Bar;
		}
	}";

		var after = @"
	namespace N
	{
		using System;

		public class Foo
		{
		}
	}";
		RoslynAssert.CodeFix(Fix, ExpectedDiagnostic, before, after);
	}
}
```

# FixAll

When there are many isses that will be fixed:
RoslynAssert.FixAll does:
- Fix all diagnostcs one by one
- Fix all diagnostics in all supported scopes.

```c#
public class CodeFixTests
{
    private static readonly DiagnosticAnalyzer Analyzer = new FieldNameMustNotBeginWithUnderscore();
    private static readonly CodeFixProvider Fix = new SA1309CodeFixProvider();

	[Test]
	public void TestThatAnalyzerWarnsOnCorrectPositionAndThatCodeFixProducesExpectedCode()
	{
		var before = @"
	namespace N
	{
		class Foo
		{
			private readonly int ↓_value1;
			private readonly int ↓_value2;
		}
	}";

		var after = @"
	namespace N
	{
		class Foo
		{
			private readonly int value1;
			private readonly int value2;
		}
	}";
		RoslynAssert.FixAll(Analyzer, Fix, before, after);
	}
}
```

# NoFix

Test that the analyzer reports an error or warning at position indicated with ↓ and that the codefix does not change the code.
With an aplhanumeric keyboard `alt + 25` writes `↓`.
This can happen if for example it is decided to not support rare edge cases with the code fix.

```c#
public class CodeFixTests
{
    private static readonly DiagnosticAnalyzer Analyzer = new FieldNameMustNotBeginWithUnderscore();
    private static readonly CodeFixProvider Fix = new SA1309CodeFixProvider();

	[Test]
	public void TestThatAnalyzerWarnsOnCorrectPositionAndThatCodeFixDoesNothing()
	{
		var code = @"
	namespace N
	{
		class Foo
		{
			private readonly int ↓_value;
		}
	}";

		RoslynAssert.NoFix(Analyzer, Fix, code);
	}
}
```

# Refactoring
```cs
public class CodeFixTests
{
	private static readonly CodeRefactoringProvider Refactoring = new ClassNameToUpperCaseRefactoringProvider();

    [Test]
    public void WithPositionIndicated()
    {
        var before = @"
class ↓Foo
{
}";

        var after = @"
class FOO
{
}";
        RoslynAssert.Refactoring(Refactoring, before, after);
		// Or if you want to assert on title also
		RoslynAssert.Refactoring(Refactoring, before, after, title: "Change to uppercase.");
    }
}
```

# AST

For checking every node and token in the tree.

```cs
[Test]
public void CheckAst()
{
    var actual = SyntaxFactory.AssignmentExpression(SyntaxKind.SimpleAssignmentExpression, SyntaxFactory.IdentifierName("a"), SyntaxFactory.IdentifierName("b"));
    var expected = CSharpSyntaxTree.ParseText("var c = a + b").FindAssignmentExpression("a + b");
    RoslynAssert.Ast(expected, actual);
}
```

## SyntaxFactoryWriter

Get a string with a call to SyntaxFactory for generating the code passed in.

```cs
var code = @"namespace A.B
{
    public class C
    {
    }
}";
var call = SyntaxFactoryWriter.Serialize(code);
```

# Attributes

When creating the workspace to analyze metadata references need to be added. There are a couple of ways to provide them using this library.
Some overloads of the asserts allow passing explicit references but it will be verbose to do that everywhere.

In most scenarios something like this in the test project is what you want:

```c#
using Gu.Roslyn.Asserts;

[assembly: TransitiveMetadataReferences(
    typeof(Microsoft.EntityFrameworkCore.DbContext),
    typeof(Microsoft.AspNetCore.Mvc.Controller))]
```

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

Calling `RoslynAssert.ResetMetadataReferences()` resets `RoslynAssert.MetadataReferences` to the list provided via the attribute or clears it if no attribute is provided.

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
## Exlicit set RoslynAssert.MetadataReferences

```c#
RoslynAssert.MetadataReferences.Add(MetadataReference.CreateFromFile(typeof(int).Assembly.Location));
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

For globally ignoring compiler warnings and errors introduced by code fixes when calling calling RoslynAssert.CodeFix and RoslynAssert.FixAll.

```c#
[assembly: IgnoredErrors("CS1569", ...)]
```

## AllowedDiagnosticsAttribute

For globally ignoring compiler warnings and errors introduced by code fixes when calling calling RoslynAssert.CodeFix and RoslynAssert.FixAll.

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
```

# Fix

When dropping down to manual mode `Analyze` & `Fix` can be used like this:

```cs
[Test]
public void SingleClassOneErrorCorrectFix()
{
    var code = @"
namespace N
{
    class Foo
    {
        private readonly int _value;
    }
}";

    var after = @"
namespace N
{
    class Foo
    {
        private readonly int value;
    }
}";
    var analyzer = new FieldNameMustNotBeginWithUnderscore();
    var cSharpCompilationOptions = CodeFactory.DefaultCompilationOptions(analyzer);
    var metadataReferences = new[] { MetadataReference.CreateFromFile(typeof(int).Assembly.Location) };
    var sln = CodeFactory.CreateSolution(code, cSharpCompilationOptions, metadataReferences);
    var diagnostics = Analyze.GetDiagnostics(sln, analyzer);
    var fixedSln = Fix.Apply(sln, new DontUseUnderscoreCodeFixProvider(), diagnostics);
    CodeAssert.AreEqual(after, fixedSln.Projects.Single().Documents.Single());
}
```

# CodeFactory

## CreateSolution

### Create a Microsoft.CodeAnalysis.AdhocWorkspace, a Roslyn Solution from code.

```c#
[Test]
public void CreateSolutionFromSources()
{
    var code = @"
namespace N
{
    class Foo
    {
        private readonly int _value;
    }
}";
    var sln = CodeFactory.CreateSolution(code, new[] { new FieldNameMustNotBeginWithUnderscore() });
    Assert.AreEqual("N", sln.Projects.Single().Name);
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

```cs
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
namespace N
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

# AstView
![Animation](https://user-images.githubusercontent.com/1640096/60766676-77ba5f80-a0ad-11e9-95c2-1b789d5490be.gif)

# Usage with different test project types
## Net472 new project type.
```xml
<PropertyGroup>
  <AutoGenerateBindingRedirects>true</AutoGenerateBindingRedirects>
  <GenerateBindingRedirectsOutputType>true</GenerateBindingRedirectsOutputType>
</PropertyGroup>
```

## NetCoreApp2.0
TODO figure out what is needed here.

