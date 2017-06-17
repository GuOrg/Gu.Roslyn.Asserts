# Gu.Roslyn.Asserts

[![License](https://img.shields.io/badge/license-MIT-blue.svg)](LICENSE.md)
[![NuGet](https://img.shields.io/nuget/v/Gu.Roslyn.Asserts.svg)](https://www.nuget.org/packages/Gu.Roslyn.Asserts/)
[![Build status](https://ci.appveyor.com/api/projects/status/a0976a1dmtcx387r/branch/master?svg=true)](https://ci.appveyor.com/project/JohanLarsson/gu-roslyn-asserts/branch/master)

<!--
[![Build status](https://ci.appveyor.com/api/projects/status/a0976a1dmtcx387r/branch/master?svg=true)](https://ci.appveyor.com/project/JohanLarsson/gu-roslyn-asserts/branch/master)
-->

Asserts for testing Roslyn analyzers.
As of now MetaDataReferences must be added to the static field `AnalyzerAssert.References` not super nice.
A halper like this can be used.
```c#
private static IReadOnlyList<MetadataReference> CreateMetaDataReferences(params Type[] types)
{
    return types.Select(type => type.GetTypeInfo().Assembly)
                .Distinct()
                .Select(assembly => MetadataReference.CreateFromFile(assembly.Location))
                .ToArray();
}
```

# Samples

## No diagnostic, happy path.

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
    AnalyzerAssert.NoDiagnostics<NoErrorAnalyzer>(code);
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
    AnalyzerAssert.NoDiagnostics(type, code);
}
```

## Diagnostic

Indicate error position with ↓
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

## Diagnostic and code fix

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


## Code fix only

```c#
[Test]
public void TestThatCodeFixProducesExpectedCode()
{
    AnalyzerAssert.References.Add(MetadataReference.CreateFromFile(typeof(object).Assembly.Location).WithAliases(ImmutableArray.Create("global", "corlib")));
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

## Diagnostic and code fix when expecting the fix to not do anything

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

## Analyze a project on disk

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
