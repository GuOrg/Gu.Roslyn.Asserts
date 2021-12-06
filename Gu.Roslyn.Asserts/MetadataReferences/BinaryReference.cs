namespace Gu.Roslyn.Asserts;

using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.CodeAnalysis;

/// <summary>
/// Helper for getting meta data reference from code.
/// </summary>
public static class BinaryReference
{
    /// <summary>
    /// Create a binary reference from strings.
    /// This is useful when testing for example deriving from a base class not in source.
    /// </summary>
    /// <param name="code">The code to create a dll project from.</param>
    /// <returns>A <see cref="MetadataReference"/>.</returns>
    public static MetadataReference Compile(params string[] code)
    {
        return Compile(code, Settings.Default.WithCompilationOptions(CodeFactory.DllCompilationOptions));
    }

    /// <summary>
    /// Create a binary reference from strings.
    /// This is useful when testing for example deriving from a base class not in source.
    /// </summary>
    /// <param name="code">The code to create a dll project from.</param>
    /// <param name="settings">The <see cref="Settings"/>.</param>
    /// <returns>A <see cref="MetadataReference"/>.</returns>
    public static MetadataReference Compile(IEnumerable<string> code, Settings? settings = null)
    {
        settings ??= Settings.Default;
        var sln = CodeFactory.CreateSolutionWithOneProject(
            code,
            settings.ParseOptions,
            settings.CompilationOptions,
            settings.MetadataReferences);
        RoslynAssert.NoCompilerDiagnostics(sln);

        using var ms = new MemoryStream();
        _ = sln.Projects.Single().GetCompilationAsync().GetAwaiter().GetResult()!.Emit(ms);
        ms.Position = 0;
        return MetadataReference.CreateFromStream(ms);
    }
}
