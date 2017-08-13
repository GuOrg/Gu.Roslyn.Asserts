namespace Gu.Roslyn.Asserts
{
    using System;
    using System.Collections.Generic;
    using Microsoft.CodeAnalysis;

    /// <summary>
    /// Specify a default metadata reference to use.
    /// </summary>
    [AttributeUsage(AttributeTargets.Assembly, AllowMultiple = true)]
    public class MetadataReferenceAttribute : Attribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MetadataReferenceAttribute"/> class.
        /// </summary>
        /// <param name="type">A type in the assembly.</param>
        public MetadataReferenceAttribute(Type type)
            : this(type, new string[0])
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MetadataReferenceAttribute"/> class.
        /// </summary>
        /// <param name="type">A type in the assembly.</param>
        /// <param name="aliases">Aliases: ex {"global", "corlib"} can be null</param>
        public MetadataReferenceAttribute(Type type, string[] aliases)
        {
            this.Type = type;
            this.Aliases = aliases ?? new string[0];
        }

        /// <summary>
        /// Get the type in the assembly
        /// </summary>
        public Type Type { get; }

        /// <summary>
        /// Get the aliases
        /// </summary>
        public IReadOnlyList<string> Aliases { get; }

        public MetadataReference MetadataReference
        {
            get
            {
#if NET46
                if (this.Aliases == null || this.Aliases.Count == 0)
                {
                    return MetadataReference.CreateFromFile(typeof(object).Assembly.Location);
                }

                return MetadataReference.CreateFromFile(typeof(object).Assembly.Location).WithAliases(this.Aliases);
#else
                return null;
#endif
            }
        }
    }
}