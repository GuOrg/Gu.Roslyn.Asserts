namespace Gu.Roslyn.Asserts
{
    using System;
    using System.Collections.Generic;
    using System.Collections.Immutable;
    using System.Linq;

    /// <summary>
    /// For getting all warnings to suppress specified with [SuppressWarnings].
    /// </summary>
    public static class SuppressWarnings
    {
        private static ImmutableArray<string> fromAttributes;

        /// <summary>
        /// Get the warnings to suppress specified with <see cref="SuppressWarningsAttribute"/>.
        /// </summary>
        public static ImmutableArray<string> FromAttributes()
        {
            if (!fromAttributes.IsDefault)
            {
                return fromAttributes;
            }

            var set = new HashSet<string>();
            foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                var attributes = Attribute.GetCustomAttributes(assembly, typeof(SuppressWarningsAttribute));
                foreach (var attribute in attributes.Cast<SuppressWarningsAttribute>())
                {
                    set.UnionWith(attribute.Ids);
                }
            }

            fromAttributes = ImmutableArray.CreateRange(set);
            return fromAttributes;
        }
    }
}
