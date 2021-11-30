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
                .WithCompilationOptions(x => x.WithSuppressed("CS0281", "CS1701", "CS1702"))
                .WithMetadataReferences(MetadataReferences.Transitive(typeof(ModuleInitializer)));
        }
    }
}
