namespace Gu.Roslyn.Asserts.Tests;

using System.Runtime.CompilerServices;

internal static class ModuleInitializer
{
    [ModuleInitializer]
    internal static void Initialize()
    {
        Settings.Default = Settings.Default.WithMetadataReferences(Asserts.MetadataReferences.Transitive(typeof(ModuleInitializer)));
    }
}
