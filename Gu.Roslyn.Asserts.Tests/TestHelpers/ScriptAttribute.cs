namespace Gu.Roslyn.Asserts.Tests
{
    using NUnit.Framework;

    public class ScriptAttribute : ExplicitAttribute
    {
        public ScriptAttribute()
         : base("SCRIPT")
        {
        }
    }
}
