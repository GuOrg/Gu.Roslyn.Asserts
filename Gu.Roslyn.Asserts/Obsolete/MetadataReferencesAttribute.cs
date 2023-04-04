namespace Gu.Roslyn.Asserts;

using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;

/// <summary>
/// Specify what default metadata reference to use when compiling the code in asserts.
/// </summary>
[AttributeUsage(AttributeTargets.Assembly)]
[Obsolete("Use Settings.Default")]
public sealed class MetadataReferencesAttribute : Attribute
{
    /// <summary>
    /// Initializes a new instance of the <see cref="MetadataReferencesAttribute"/> class.
    /// </summary>
    /// <param name="types">Specify types in assemblies for which metadata references will be included.</param>
#pragma warning disable CA1019 // Define accessors for attribute arguments
    public MetadataReferencesAttribute(params Type[] types)
#pragma warning restore CA1019 // Define accessors for attribute arguments
    {
        this.MetadataReferences = types.Select(x => Gu.Roslyn.Asserts.MetadataReferences.CreateFromAssembly(x.Assembly))
                                       .ToArray();

        Settings.Default = Settings.Default.WithMetadataReferences(x => x.Concat(this.MetadataReferences));
    }

    /// <summary>
    /// Gets the metadata references to include in the workspaces used in tests.
    /// </summary>
    public IEnumerable<MetadataReference> MetadataReferences { get; }
}
