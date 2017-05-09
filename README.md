# Gu.Roslyn.Asserts

[![Build status](https://ci.appveyor.com/api/projects/status/a0976a1dmtcx387r/branch/master?svg=true)](https://ci.appveyor.com/project/JohanLarsson/gu-roslyn-asserts/branch/master)

Asserts for testing Roslyn analyzers.

#Samples

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


