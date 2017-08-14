namespace Gu.Roslyn.Asserts
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Microsoft.CodeAnalysis;

    internal static class MetadataReferences
    {
#pragma warning disable 169
        private static List<MetadataReference> metadataReferences;
#pragma warning restore 169

        /// <summary>
        /// Get the metadata references specified in the test assembly.
        /// </summary>
        public static List<MetadataReference> GetMetadataReferences()
        {
            if (metadataReferences != null)
            {
                return new List<MetadataReference>(metadataReferences);
            }

            metadataReferences = new List<MetadataReference>();
            var assemblies = AppDomain.CurrentDomain.GetAssemblies();
            foreach (var assembly in assemblies)
            {
                var attributes = Attribute.GetCustomAttributes(assembly, typeof(MetadataReferenceAttribute));
                foreach (var attribute in attributes.Cast<MetadataReferenceAttribute>())
                {
                    metadataReferences.Add(attribute.MetadataReference);
                }
            }

            foreach (var assembly in assemblies)
            {
                var attribute = (MetadataReferencesAttribute)Attribute.GetCustomAttribute(assembly, typeof(MetadataReferencesAttribute));
                if (attribute != null)
                {
                    metadataReferences.AddRange(attribute.MetadataReferences);
                }
            }

            ////var displayNames = metadataReferences.Select(x => x.Display).ToArray();
            ////var distinct = displayNames.Distinct().OrderBy(x => x).ToArray();
            ////if (distinct.Length != displayNames.Length)
            ////{
            ////    var dupes = displayNames.Where(x => displayNames.Count(y => x == y) > 1);
            ////    throw AssertException.Create(
            ////        "Expected metadata references to be unique assemblies.\r\n" +
            ////        "The following appear more than once.\r\n" +
            ////        $"{string.Join(Environment.NewLine, dupes)}");
            ////}

            return new List<MetadataReference>(metadataReferences);
        }
    }
}