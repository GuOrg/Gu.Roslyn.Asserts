namespace Gu.Roslyn.Asserts;

using System;
using System.Collections.Generic;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

public class Settings
{
    private static Settings? @default;

    public Settings(CSharpCompilationOptions compilationOptions, IReadOnlyList<MetadataReference>? metadataReferences, AllowCompilationErrors allowCompilationErrors = AllowCompilationErrors.No)
    {
        this.CompilationOptions = compilationOptions;
        this.MetadataReferences = metadataReferences;
        this.AllowCompilationErrors = allowCompilationErrors;
    }

    public static Settings Default
    {
        get => @default ??= new Settings(CodeFactory.DefaultCompilationOptions(specificDiagnosticOptions: null), null);
        set => @default = value;
    }

    public CSharpCompilationOptions CompilationOptions { get; }

    public IReadOnlyList<MetadataReference>? MetadataReferences { get; }

    public AllowCompilationErrors AllowCompilationErrors { get; }

    public Settings WithCompilationOptions(CSharpCompilationOptions compilationOptions) => new(compilationOptions, this.MetadataReferences, this.AllowCompilationErrors);

    public Settings WithCompilationOptions(Func<CSharpCompilationOptions, CSharpCompilationOptions> update)
    {
        if (update is null)
        {
            throw new ArgumentNullException(nameof(update));
        }

        return new(update(this.CompilationOptions), this.MetadataReferences, this.AllowCompilationErrors);
    }

    public Settings WithMetadataReferences(IReadOnlyList<MetadataReference>? metadataReferences) => new(this.CompilationOptions, metadataReferences, this.AllowCompilationErrors);

    public Settings WithCompilationOptions(Func<IReadOnlyList<MetadataReference>?, IReadOnlyList<MetadataReference>?> update)
    {
        if (update is null)
        {
            throw new ArgumentNullException(nameof(update));
        }

        return new(this.CompilationOptions, update(this.MetadataReferences), this.AllowCompilationErrors);
    }

    public Settings WithAllowCompilationErrors(AllowCompilationErrors allowCompilationErrors) => new(this.CompilationOptions, this.MetadataReferences, allowCompilationErrors);
}
