namespace Gu.Roslyn.Asserts
{
    using System;
    using System.Collections.Generic;
    using Microsoft.CodeAnalysis;

    /// <summary>
    /// Specify what compiler warnings to ignore when when compiling the code in asserts.
    /// Example: [assembly: SuppressWarnings("CS1701", "CS1702")].
    /// </summary>
    [Obsolete("Use settings.")]
    [AttributeUsage(AttributeTargets.Assembly, AllowMultiple = false)]
    public sealed class SuppressWarningsAttribute : Attribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SuppressWarningsAttribute"/> class.
        /// </summary>
        /// <param name="ids">A collection of <see cref="Diagnostic.Id"/> to suppress.</param>
        public SuppressWarningsAttribute(params string[] ids)
        {
            this.Ids = ids;
        }

        /// <summary>
        /// Gets a collection of <see cref="Diagnostic.Id"/> to suppress.
        /// </summary>
        public IReadOnlyList<string> Ids { get; }
    }
}
