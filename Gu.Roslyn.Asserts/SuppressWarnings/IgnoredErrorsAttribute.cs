namespace Gu.Roslyn.Asserts
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Specify what compiler errors to ignore when calling RoslynAssert.CodeFix and RoslynAssert.FixAll.
    /// </summary>
    [AttributeUsage(AttributeTargets.Assembly, AllowMultiple = false)]
    [Obsolete("Use " + nameof(SuppressWarningsAttribute))]
    public sealed class IgnoredErrorsAttribute : SuppressWarningsAttribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="IgnoredErrorsAttribute"/> class.
        /// </summary>
        /// <param name="errorIds">Specify ids of compiler errors to ignore when checking if a fix introduced compiler errors.</param>
        public IgnoredErrorsAttribute(params string[] errorIds)
            : base(errorIds)
        {
            this.ErrorIds = errorIds ?? Array.Empty<string>();
        }

        /// <summary>
        /// Gets the ids of compiler errors to ignore when checking if a fix introduced compiler errors.
        /// </summary>
        public IReadOnlyList<string> ErrorIds { get; }
    }
}
