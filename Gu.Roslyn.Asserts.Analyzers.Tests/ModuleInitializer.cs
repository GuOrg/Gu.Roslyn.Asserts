namespace Gu.Roslyn.Asserts.Analyzers.Tests
{
    using System.Runtime.CompilerServices;

    using Gu.Roslyn.Asserts;

    internal static class ModuleInitializer
    {
        [ModuleInitializer]
        internal static void Initialize()
        {
            Settings.Default = Settings.Default
                .WithCompilationOptions(x => x.WithSuppressed("CS1701", "CS1702", "CS0281"))
                .WithMetadataReferences(MetadataReferences.Transitive(typeof(ModuleInitializer)));
        }
    }
}
