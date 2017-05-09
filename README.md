# Gu.Roslyn.Asserts

[![Build status](https://ci.appveyor.com/api/projects/status/a0976a1dmtcx387r/branch/master?svg=true)](https://ci.appveyor.com/project/JohanLarsson/gu-roslyn-asserts/branch/master)

Asserts for testing Roslyn analyzers.

# Samples

## No diagnostic, happy path.

```c#
[Test]
public void SingleClassNoError()
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

## Diagnostic

Indicate error position with ↓
```c#
[Test]
public void SingleClassOneErrorGeneric()
{
    var code = @"
namespace Gu.Roslyn.Asserts.Tests
{
    class Foo
    {
        ↓public Foo()
        {
        }
    }
}";
    AnalyzerAssert.Diagnostics<ErrorOnCtorAnalyzer>(code);
}
```

## Diagnostic and code fix

```c#
[Test]
public void SingleClassOneErrorCorrectFix()
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

## Diagnostic and code fix when expecting the fix to not do anything

```c#
[Test]
public void SingleClassOneErrorCorrectFix()
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
