namespace Gu.Roslyn.Asserts;

using System;
using System.Collections.Generic;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

public class Settings
{
    private static Settings? @default;

    /// <summary>
    /// Create an instance of the class <see cref="Settings"/>.
    /// </summary>
    /// <param name="compilationOptions">The <see cref="CSharpCompilationOptions"/>.</param>
    /// <param name="metadataReferences">A collection of <see cref="MetadataReference"/> to use when compiling. Default is <see langword="null" /> meaning <see cref="MetadataReferences"/> are used.</param>
    /// <param name="allowCompilationErrors">Specify if compilation errors are accepted in the fixed code. This can be for example syntax errors. Default value is <see cref="AllowCompilationErrors.No"/>.</param>
    /// <param name="errorOnAD0001">Compiler error if AD0001 is reported.</param>
    public Settings(CSharpCompilationOptions compilationOptions, IReadOnlyList<MetadataReference>? metadataReferences, AllowCompilationErrors allowCompilationErrors = AllowCompilationErrors.No, bool errorOnAD0001 = true)
    {
        this.CompilationOptions = compilationOptions;
        this.MetadataReferences = metadataReferences;
        this.AllowCompilationErrors = allowCompilationErrors;
        this.ErrorOnAD0001 = errorOnAD0001;
    }

    public static Settings Default
    {
        get => @default ??= new Settings(CodeFactory.DefaultCompilationOptions(specificDiagnosticOptions: null), null);
        set => @default = value;
    }

    /// <summary>
    /// The <see cref="CSharpCompilationOptions"/>.
    /// </summary>
    public CSharpCompilationOptions CompilationOptions { get; }

    /// <summary>
    /// A collection of <see cref="MetadataReference"/> to use when compiling. Default is <see langword="null" /> meaning <see cref="MetadataReferences"/> are used.
    /// </summary>
    public IReadOnlyList<MetadataReference>? MetadataReferences { get; }

    /// <summary>
    /// Specify if compilation errors are accepted in the fixed code. This can be for example syntax errors. Default value is <see cref="AllowCompilationErrors.No"/>.
    /// </summary>
    public AllowCompilationErrors AllowCompilationErrors { get; }

    /// <summary>
    /// Compiler error if AD0001 is reported.
    /// </summary>
    public bool ErrorOnAD0001 { get; }

    /// <summary>
    /// Create a new instance with new <see cref="CSharpCompilationOptions"/>.
    /// </summary>
    /// <param name="compilationOptions">The <see cref="CSharpCompilationOptions"/>.</param>
    /// <returns>A new instance of <see cref="Settings"/>.</returns>
    public Settings WithCompilationOptions(CSharpCompilationOptions compilationOptions) => new(compilationOptions, this.MetadataReferences, this.AllowCompilationErrors, this.ErrorOnAD0001);

    /// <summary>
    /// Create a new instance with new <see cref="CSharpCompilationOptions"/>.
    /// </summary>
    /// <param name="update">The update of current <see cref="CompilationOptions"/>.</param>
    /// <returns>A new instance of <see cref="Settings"/>.</returns>
    public Settings WithCompilationOptions(Func<CSharpCompilationOptions, CSharpCompilationOptions> update)
    {
        if (update is null)
        {
            throw new ArgumentNullException(nameof(update));
        }

        return new(update(this.CompilationOptions), this.MetadataReferences, this.AllowCompilationErrors, this.ErrorOnAD0001);
    }

    /// <summary>
    /// Create a new instance with new <see cref="IReadOnlyList{MetadataReference}"/>.
    /// </summary>
    /// <param name="metadataReferences">The <see cref="IReadOnlyList{MetadataReference}"/>.</param>
    /// <returns>A new instance of <see cref="Settings"/>.</returns>
    public Settings WithMetadataReferences(IReadOnlyList<MetadataReference>? metadataReferences) => new(this.CompilationOptions, metadataReferences, this.AllowCompilationErrors, this.ErrorOnAD0001);

    /// <summary>
    /// Create a new instance with new <see cref="IReadOnlyList{MetadataReference}"/>.
    /// </summary>
    /// <param name="update">The update of current <see cref="IReadOnlyList{MetadataReference}"/>.</param>
    /// <returns>A new instance of <see cref="Settings"/>.</returns>
    public Settings WithCompilationOptions(Func<IReadOnlyList<MetadataReference>?, IReadOnlyList<MetadataReference>?> update)
    {
        if (update is null)
        {
            throw new ArgumentNullException(nameof(update));
        }

        return new(this.CompilationOptions, update(this.MetadataReferences), this.AllowCompilationErrors, this.ErrorOnAD0001);
    }

    /// <summary>
    /// Create a new instance with new <see cref="AllowCompilationErrors"/>.
    /// </summary>
    /// <param name="allowCompilationErrors">The <see cref="AllowCompilationErrors"/>.</param>
    /// <returns>A new instance of <see cref="Settings"/>.</returns>
    public Settings WithAllowCompilationErrors(AllowCompilationErrors allowCompilationErrors) => new(this.CompilationOptions, this.MetadataReferences, allowCompilationErrors, this.ErrorOnAD0001);
}
