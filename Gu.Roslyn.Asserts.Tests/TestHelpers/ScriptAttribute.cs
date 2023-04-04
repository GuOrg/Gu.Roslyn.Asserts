namespace Gu.Roslyn.Asserts.Tests;

using NUnit.Framework;

public sealed class ScriptAttribute : ExplicitAttribute
{
    public ScriptAttribute()
     : base("SCRIPT")
    {
    }
}
