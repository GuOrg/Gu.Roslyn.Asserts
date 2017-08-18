namespace Gu.Roslyn.Asserts
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Helper for getting ignored errors from <see cref="IgnoredErrorsAttribute"/>.
    /// </summary>
    internal static class IgnoredErrors
    {
#pragma warning disable 169
        private static IReadOnlyList<string> staticErrors;
#pragma warning restore 169

        /// <summary>
        /// Get the metadata references specified with <see cref="MetadataReferenceAttribute"/> and <see cref="MetadataReferencesAttribute"/> in the test assembly.
        /// </summary>
        public static IReadOnlyList<string> Get()
        {
            if (staticErrors != null)
            {
                return staticErrors;
            }

            var errors = new List<string>();
            staticErrors = errors;
            var assemblies = AppDomain.CurrentDomain.GetAssemblies();
            foreach (var assembly in assemblies)
            {
                var attribute = (IgnoredErrorsAttribute)Attribute.GetCustomAttribute(assembly, typeof(IgnoredErrorsAttribute));
                if (attribute?.ErrorIds != null)
                {
                    errors.AddRange(attribute.ErrorIds);
                }
            }

            return staticErrors;
        }
    }
}