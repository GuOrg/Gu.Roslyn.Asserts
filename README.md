# Gu.Roslyn.Asserts
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


