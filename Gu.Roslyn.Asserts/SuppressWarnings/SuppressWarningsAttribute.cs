namespace Gu.Roslyn.Asserts
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Specify what compiler warnings to ignore when when compiling the code in asserts.
    /// Example: 
    /// </summary>
    [AttributeUsage(AttributeTargets.Assembly, AllowMultiple = false)]
    public class SuppressWarningsAttribute : Attribute
    {
        public SuppressWarningsAttribute(params string[] ids)
        {
            this.Ids = ids;
        }

        public IReadOnlyList<string> Ids { get; }
    }
}
