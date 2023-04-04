namespace Gu.Roslyn.Asserts;

/// <summary>
/// Controls serialization using <see cref="SyntaxFactoryWriter"/>.
/// </summary>
public sealed class SyntaxFactoryWriterSettings
{
    /// <summary>
    /// The default instance.
    /// </summary>
    public static readonly SyntaxFactoryWriterSettings Default = new(defaultTrivia: false);

    /// <summary>
    /// Initializes a new instance of the <see cref="SyntaxFactoryWriterSettings"/> class.
    /// </summary>
    /// <param name="defaultTrivia">Controls if default trivia should be used.</param>
    public SyntaxFactoryWriterSettings(bool defaultTrivia)
    {
        this.DefaultTrivia = defaultTrivia;
    }

    /// <summary>
    /// Gets a value indicating whether default trivia should be used.
    /// True means that no calls for creating trivia is generated.
    /// </summary>
    public bool DefaultTrivia { get; }
}
