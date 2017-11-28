namespace Gu.Roslyn.Asserts
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Specify what compiler errors to ignore when calling AnalyzerAssert.CodeFix and AnalyzerAssert.FixAll
    /// </summary>
    [AttributeUsage(AttributeTargets.Assembly, AllowMultiple = false)]
    public class IgnoredErrorsAttribute : Attribute
    {
        private static readonly IReadOnlyList<string> Empty = new string[0];

        /// <summary>
        /// Initializes a new instance of the <see cref="IgnoredErrorsAttribute"/> class.
        /// </summary>
        /// <param name="errorIds">Specify ids of compiler errors to ignore when checking if a fix introduced compiler errors.</param>
        public IgnoredErrorsAttribute(params string[] errorIds)
        {
            this.ErrorIds = errorIds ?? Empty;
        }

        /// <summary>
        /// Gets the ids of compiler errors to ignore when checking if a fix introduced compiler errors.
        /// </summary>
        public IReadOnlyList<string> ErrorIds { get; }
    }
}