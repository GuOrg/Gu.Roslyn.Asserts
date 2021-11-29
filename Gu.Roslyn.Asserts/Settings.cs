namespace Gu.Roslyn.Asserts
{
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
        /// <param name="parseOptions">The <see cref="CSharpParseOptions"/>.</param>
        /// <param name="metadataReferences">A collection of <see cref="MetadataReference"/> to use when compiling. Default is <see langword="null" /> meaning <see cref="MetadataReferences"/> are used.</param>
        /// <param name="allowCompilationErrors">Specify if compilation errors are accepted in the fixed code. This can be for example syntax errors. Default value is <see cref="AllowCompilationErrors.No"/>.</param>
        public Settings(CSharpCompilationOptions compilationOptions, CSharpParseOptions parseOptions, MetadataReferencesCollection? metadataReferences, AllowCompilationErrors allowCompilationErrors = AllowCompilationErrors.No)
        {
            this.CompilationOptions = compilationOptions;
            this.ParseOptions = parseOptions;
            this.MetadataReferences = metadataReferences;
            this.AllowCompilationErrors = allowCompilationErrors;
        }

        public static Settings Default
        {
            get => @default ??= new Settings(CodeFactory.DefaultCompilationOptions(new[] { new KeyValuePair<string, ReportDiagnostic>("AD0001", ReportDiagnostic.Error) }), CSharpParseOptions.Default, null);
            set => @default = value;
        }

        /// <summary>
        /// The <see cref="CSharpCompilationOptions"/>.
        /// </summary>
        public CSharpCompilationOptions CompilationOptions { get; }

        /// <summary>
        /// The <see cref="CSharpParseOptions"/>.
        /// </summary>
        public CSharpParseOptions ParseOptions { get; }

        /// <summary>
        /// A collection of <see cref="MetadataReference"/> to use when compiling. Default is <see langword="null" /> meaning <see cref="MetadataReferences"/> are used.
        /// </summary>
        public MetadataReferencesCollection? MetadataReferences { get; }

        /// <summary>
        /// Specify if compilation errors are accepted in the fixed code. This can be for example syntax errors. Default value is <see cref="AllowCompilationErrors.No"/>.
        /// </summary>
        public AllowCompilationErrors AllowCompilationErrors { get; }

        /// <summary>
        /// Create a new instance with new <see cref="CSharpCompilationOptions"/>.
        /// </summary>
        /// <param name="compilationOptions">The <see cref="CSharpCompilationOptions"/>.</param>
        /// <returns>A new instance of <see cref="Settings"/>.</returns>
        public Settings WithCompilationOptions(CSharpCompilationOptions compilationOptions) => new(compilationOptions, this.ParseOptions, this.MetadataReferences, this.AllowCompilationErrors);

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

            return new(update(this.CompilationOptions), this.ParseOptions, this.MetadataReferences, this.AllowCompilationErrors);
        }

        /// <summary>
        /// Create a new instance with new <see cref="CSharpCompilationOptions"/>.
        /// </summary>
        /// <param name="parseOptions">The <see cref="CSharpParseOptions"/>.</param>
        /// <returns>A new instance of <see cref="Settings"/>.</returns>
        public Settings WithParseOption(CSharpParseOptions parseOptions) => new(this.CompilationOptions, parseOptions, this.MetadataReferences, this.AllowCompilationErrors);

        /// <summary>
        /// Create a new instance with new <see cref="IReadOnlyList{MetadataReference}"/>.
        /// </summary>
        /// <param name="metadataReferences">The <see cref="IEnumerable{MetadataReference}"/>.</param>
        /// <returns>A new instance of <see cref="Settings"/>.</returns>
        public Settings WithMetadataReferences(IEnumerable<MetadataReference>? metadataReferences) => new(
            this.CompilationOptions,
            this.ParseOptions,
            metadataReferences is null ? null : new MetadataReferencesCollection(metadataReferences),
            this.AllowCompilationErrors);

        /// <summary>
        /// Create a new instance with new <see cref="IReadOnlyList{MetadataReference}"/>.
        /// </summary>
        /// <param name="update">The update of current <see cref="IReadOnlyList{MetadataReference}"/>.</param>
        /// <returns>A new instance of <see cref="Settings"/>.</returns>
        public Settings WithMetadataReferences(Func<IEnumerable<MetadataReference>?, IEnumerable<MetadataReference>?> update)
        {
            if (update is null)
            {
                throw new ArgumentNullException(nameof(update));
            }

            return new(
                this.CompilationOptions,
                this.ParseOptions,
                update(this.MetadataReferences) is { } metadataReferences ? new MetadataReferencesCollection(metadataReferences) : null,
                this.AllowCompilationErrors);
        }

        /// <summary>
        /// Create a new instance with new <see cref="AllowCompilationErrors"/>.
        /// </summary>
        /// <param name="allowCompilationErrors">The <see cref="AllowCompilationErrors"/>.</param>
        /// <returns>A new instance of <see cref="Settings"/>.</returns>
        public Settings WithAllowCompilationErrors(AllowCompilationErrors allowCompilationErrors) => new(this.CompilationOptions, this.ParseOptions, this.MetadataReferences, allowCompilationErrors);
    }
}
