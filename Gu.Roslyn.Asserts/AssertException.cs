namespace Gu.Roslyn.Asserts;

using System;
using System.Runtime.Serialization;

/// <summary>
/// Thrown when an assertion failed.
/// </summary>
[Serializable]
public class AssertException : Exception
{
    /// <summary>
    /// Initializes a new instance of the <see cref="AssertException"/> class.
    /// </summary>
    /// <param name="message"> The error message that explains the reason for the exception.</param>
    public AssertException(string message)
        : base(message)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="AssertException"/> class.
    /// </summary>
    /// <param name="message"> The error message that explains the reason for the exception.</param>
    /// <param name="innerException"> The exception that caused the current exception.</param>
    public AssertException(string message, Exception innerException)
        : base(message, innerException)
    {
    }

    /// <summary>Initializes a new instance of the <see cref="AssertException" /> class with serialized data.</summary>
    /// <param name="info">The <see cref="SerializationInfo" /> that holds the serialized object data about the exception being thrown.</param>
    /// <param name="context">The <see cref="StreamingContext" /> that contains contextual information about the source or destination.</param>
    /// <exception cref="ArgumentNullException">
    /// <paramref name="info" /> is <see langword="null" />.</exception>
    protected AssertException(SerializationInfo info, StreamingContext context)
        : base(info, context)
    {
    }

    private AssertException()
    {
    }

    /// <inheritdoc />
    public override string ToString() => this.Message;
}
