namespace Gu.Roslyn.Asserts
{
    using System;
    using System.Collections.Generic;
    using System.Collections.Immutable;
    using System.Linq;

    /// <summary>
    /// For getting all warnings to suppress specified with [SuppressWarnings].
    /// </summary>
    [Obsolete("Use settings.")]
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
                var suppressAttributes = Attribute.GetCustomAttributes(assembly, typeof(SuppressWarningsAttribute));
                foreach (var attribute in suppressAttributes.Cast<SuppressWarningsAttribute>())
                {
                    set.UnionWith(attribute.Ids);
                }

#pragma warning disable CS0618 // Type or member is obsolete
                var errorsAttributes = Attribute.GetCustomAttributes(assembly, typeof(IgnoredErrorsAttribute));
                foreach (var attribute in errorsAttributes.Cast<IgnoredErrorsAttribute>())
#pragma warning restore CS0618 // Type or member is obsolete
                {
                    set.UnionWith(attribute.ErrorIds);
                }
            }

            fromAttributes = ImmutableArray.CreateRange(set);
            return fromAttributes;
        }
    }
}
