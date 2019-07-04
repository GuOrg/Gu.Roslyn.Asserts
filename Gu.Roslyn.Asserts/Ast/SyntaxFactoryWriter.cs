namespace Gu.Roslyn.Asserts
{
    using System;
    using System.Collections.Concurrent;
    using System.Diagnostics;
    using System.Linq;
    using System.Reflection;
    using System.Text;
    using Gu.Roslyn.Asserts.Internals;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;

    /// <summary>
    /// For transforming code into the corresponding SyntaxFactory call.
    /// </summary>
    [Obsolete("WIP not finished yet.")]
    public class SyntaxFactoryWriter
    {
        private static readonly ConcurrentDictionary<ParameterInfo, Action<SyntaxFactoryWriter, SyntaxNode, bool>> ArgumentWriters = new ConcurrentDictionary<ParameterInfo, Action<SyntaxFactoryWriter, SyntaxNode, bool>>();
        private readonly Writer writer = new Writer();

        /// <summary>
        /// Transforms the code passed in to a call to SyntaxFactory that creates the same code.
        /// </summary>
        /// <param name="code">For example class C { }. </param>
        /// <returns>SyntaxFactory.Compilation(...)</returns>
        public static string Serialize(string code)
        {
            var compilationUnit = SyntaxFactory.ParseCompilationUnit(code);
            return new SyntaxFactoryWriter()
                   .Write(compilationUnit)
                   .ToString();
        }

        /// <summary>
        /// Transforms the code passed in to a call to SyntaxFactory that creates the same code.
        /// </summary>
        /// <param name="node">For example class C { }. </param>
        /// <returns>SyntaxFactory.Xxx(...)</returns>
        public static string Serialize(SyntaxNode node)
        {
            return new SyntaxFactoryWriter()
                   .Write(node)
                   .ToString();
        }

        public override string ToString() => this.writer.ToString();

        private static Action<SyntaxFactoryWriter, SyntaxNode, bool> CreateArgumentWriter(ParameterInfo parameter)
        {
            switch (parameter.Name)
            {
                case "quoteKind":
                case "kind":
                    return (factoryWriter, syntaxNode, closeArgumentList) => factoryWriter.WriteArgument(parameter.Name, syntaxNode.Kind(), closeArgumentList);
            }

            var method = (MethodInfo)parameter.Member;
            var property = method.ReturnType.GetProperty(parameter.Name.Substring(0, 1).ToUpper() + parameter.Name.Substring(1), BindingFlags.Public | BindingFlags.Instance);
            if (property == null)
            {
                return null;
            }

            if (property.PropertyType.IsGenericType)
            {
                foreach (var candidate in (typeof(SyntaxFactoryWriter)).GetMethods(BindingFlags.NonPublic | BindingFlags.Instance)
                                                                       .Where(x => x.Name == "WriteArgument" && x.IsGenericMethod))
                {
                    var parameters = candidate.GetParameters();
                    if (parameters.Length == 3 &&
                        parameters[0].ParameterType == typeof(string) &&
                        parameters[1].ParameterType.Name == property.PropertyType.GetGenericTypeDefinition().Name &&
                        parameters[2].ParameterType == typeof(bool))
                    {
                        var writeMethod = candidate.MakeGenericMethod(property.PropertyType.GenericTypeArguments[0]);
                        return (factoryWriter, syntaxNode, closeArgumentList) => writeMethod.Invoke(factoryWriter, new[] { parameter.Name, property.GetValue(syntaxNode), closeArgumentList });
                    }
                }
            }
            else
            {
                var writeMethod = (typeof(SyntaxFactoryWriter))
                    .GetMethod(
                        nameof(WriteArgument),
                        BindingFlags.NonPublic | BindingFlags.Instance,
                        null,
                        CallingConventions.Any,
                        new[] { typeof(string), property.PropertyType, typeof(bool) },
                        null);

                if (writeMethod != null)
                {
                    return (factoryWriter, syntaxNode, closeArgumentList) => writeMethod.Invoke(factoryWriter, new[] { parameter.Name, property.GetValue(syntaxNode), closeArgumentList });
                }
            }

            return null;
        }

        private SyntaxFactoryWriter Write(SyntaxNode node)
        {
            var method = typeof(SyntaxFactory)
                         .GetMethods(BindingFlags.Public | BindingFlags.Static)
                         .Where(x => !x.Name.StartsWith("Parse") &&
                                     x.ReturnType == node.GetType() &&
                                     x.GetParameters().All(p => ArgumentWriters.GetOrAdd(p, CreateArgumentWriter) != null) &&
                                     x.ReturnType.Name.StartsWith(x.Name))
                         .MaxBy(x => x.GetParameters().Length);
            this.writer.Append("SyntaxFactory.").Append(method.Name).AppendLine("(")
                .PushIndent();
            var parameters = method.GetParameters();
            for (var i = 0; i < parameters.Length; i++)
            {
                ArgumentWriters[parameters[i]].Invoke(this, node, i == parameters.Length - 1);
            }

            this.writer.PopIndent();
            return this;
        }

        private SyntaxFactoryWriter Write(SyntaxToken token)
        {
            if (token.Kind() == SyntaxKind.None)
            {
                this.writer.Append("default");
                return this;
            }

            switch (token.Kind())
            {
                case SyntaxKind.BadToken:
                    return this.AppendLine("SyntaxFactory.BadToken(")
                               .PushIndent()
                               .WriteArgument("leading", token.LeadingTrivia)
                               .WriteArgument("text", token.Text, escape: true)
                               .WriteArgument("trailing", token.TrailingTrivia, closeArgumentList: true)
                               .PopIndent();
                case SyntaxKind.IdentifierToken:
                    if (token.IsContextualKeyword())
                    {
                        return this.AppendLine("SyntaxFactory.Identifier(")
                                   .PushIndent()
                                   .WriteArgument("leading", token.LeadingTrivia)
                                   .WriteArgument("contextualKind", token.Kind())
                                   .WriteArgument("text", token.Text, escape: true)
                                   .WriteArgument("valueText", token.ValueText, escape: true)
                                   .WriteArgument("trailing", token.TrailingTrivia, closeArgumentList: true)
                                   .PopIndent();
                    }

                    return this.AppendLine("SyntaxFactory.Identifier(")
                               .PushIndent()
                               .WriteArgument("leading", token.LeadingTrivia)
                               .WriteArgument("text", token.Text, escape: true)
                               .WriteArgument("trailing", token.TrailingTrivia, closeArgumentList: true)
                               .PopIndent();

                case SyntaxKind.CharacterLiteralToken:
                case SyntaxKind.NumericLiteralToken:
                case SyntaxKind.StringLiteralToken:
                    if (!token.HasLeadingTrivia &&
                        !token.HasTrailingTrivia)
                    {
                        return this.AppendLine("SyntaxFactory.Literal(")
                                   .PushIndent()
                                   .WriteArgument("text", token.Text, escape: true)
                                   .WriteArgument("value", token.ValueText, escape: true, closeArgumentList: true)
                                   .PopIndent();
                    }

                    return this.AppendLine("SyntaxFactory.Literal(")
                               .PushIndent()
                               .WriteArgument("leading", token.LeadingTrivia)
                               .WriteArgument("text", token.Text, escape: true)
                               .WriteArgument("value", token.ValueText, escape: true)
                               .WriteArgument("trailing", token.TrailingTrivia, closeArgumentList: true)
                               .PopIndent();
                case SyntaxKind.XmlTextLiteralToken when token.HasLeadingTrivia || token.HasTrailingTrivia:
                    if (!token.HasLeadingTrivia &&
                        !token.HasTrailingTrivia)
                    {
                        return this.AppendLine("SyntaxFactory.XmlTextLiteral(")
                                   .PushIndent()
                                   .WriteArgument("leading", token.LeadingTrivia)
                                   .WriteArgument("text", token.Text, escape: true)
                                   .WriteArgument("value", token.ValueText, escape: true)
                                   .WriteArgument("trailing", token.TrailingTrivia, closeArgumentList: true)
                                   .PopIndent();
                    }

                    return this.AppendLine("SyntaxFactory.XmlEntity(")
                               .PushIndent()
                               .WriteArgument("leading", token.LeadingTrivia)
                               .WriteArgument("text", token.Text, escape: true)
                               .WriteArgument("value", token.ValueText, escape: true)
                               .WriteArgument("trailing", token.TrailingTrivia, closeArgumentList: true)
                               .PopIndent();

                case SyntaxKind.XmlTextLiteralToken:
                    if (token.Text != token.ValueText)
                    {
                        return this.AppendLine("SyntaxFactory.XmlTextLiteral(")
                                   .PushIndent()
                                   .WriteArgument("text", token.Text, escape: true)
                                   .WriteArgument("value", token.ValueText, escape: true, closeArgumentList: true)
                                   .PopIndent();
                    }

                    return this.Append($"SyntaxFactory.XmlTextLiteral(\"{token.Value}\")");
                case SyntaxKind.XmlTextLiteralNewLineToken:
                    if (token.Text.Length == 1)
                    {
                        return this.AppendLine("SyntaxFactory.XmlTextNewLine(")
                                   .PushIndent()
                                   .WriteArgument("leading", token.LeadingTrivia)
                                   .WriteArgument("text", "\\n", escape: false)
                                   .WriteArgument("value", "\\n", escape: false)
                                   .WriteArgument("trailing", token.TrailingTrivia, closeArgumentList: true)
                                   .PopIndent();
                    }

                    return this.AppendLine("SyntaxFactory.XmlTextNewLine(")
                               .PushIndent()
                               .WriteArgument("leading", token.LeadingTrivia)
                               .WriteArgument("text", "\\r\\n", escape: false)
                               .WriteArgument("value", "\\r\\n", escape: false)
                               .WriteArgument("trailing", token.TrailingTrivia, closeArgumentList: true)
                               .PopIndent();

                default:
                    if (token.Text != token.ValueText)
                    {
                        return this.AppendLine("SyntaxFactory.Token(")
                                   .PushIndent()
                                   .WriteArgument("leading", token.LeadingTrivia)
                                   .WriteArgument("kind", token.Kind())
                                   .WriteArgument("text", token.Text, escape: true)
                                   .WriteArgument("valueText", token.ValueText, escape: true)
                                   .WriteArgument("trailing", token.TrailingTrivia, closeArgumentList: true)
                                   .PopIndent();
                    }
                    else if (token.HasLeadingTrivia || token.HasTrailingTrivia)
                    {
                        return this.AppendLine("SyntaxFactory.Token(")
                                   .PushIndent()
                                   .WriteArgument("leading", token.LeadingTrivia)
                                   .WriteArgument("kind", token.Kind())
                                   .WriteArgument("trailing", token.TrailingTrivia, closeArgumentList: true)
                                   .PopIndent();
                    }

                    return this.Append($"SyntaxFactory.Token(SyntaxKind.{token.Kind()})");
            }
        }

        private SyntaxFactoryWriter Write(SyntaxTrivia trivia)
        {
            if (trivia.HasStructure)
            {
                return this.AppendLine("SyntaxFactory.Trivia(")
                    .PushIndent()
                    .Write(trivia.GetStructure())
                    .PopIndent()
                    .Append(")");
            }

            switch (trivia.Kind())
            {
                case SyntaxKind.None:
                    return this.Append("default");
                case SyntaxKind.SingleLineCommentTrivia:
                case SyntaxKind.MultiLineCommentTrivia:
                    return this.Append($"SyntaxFactory.Comment(\"{trivia.ToString()}\")");
                case SyntaxKind.DisabledTextTrivia:
                    return this.Append($"SyntaxFactory.DisabledText(\"{trivia.ToString()}\")");
                case SyntaxKind.DocumentationCommentExteriorTrivia:
                    return this.Append($"SyntaxFactory.DocumentationCommentExterior(\"{trivia.ToString()}\")");
                case SyntaxKind.WhitespaceTrivia:
                    if (trivia.Span.Length == 1)
                    {
                        return this.Append("SyntaxFactory.Space");
                    }

                    return this.Append($"SyntaxFactory.Whitespace(\"{trivia.ToString()}\")");
                case SyntaxKind.EndOfLineTrivia:
                    return this.Append("SyntaxFactory.LineFeed");
                default:
                    throw new NotImplementedException($"Not handling {trivia.Kind()} yet.");
            }
        }

        private SyntaxFactoryWriter WriteArgument(string parameter, SyntaxNode node, bool closeArgumentList = false)
        {
            _ = this.writer
                    .WriteArgumentStart(parameter);
            if (node == null)
            {
                _ = this.writer.Append("default")
                               .WriteArgumentEnd(closeArgumentList);
                return this;
            }

            return this.Write(node).WriteArgumentEnd(closeArgumentList);
        }

        private SyntaxFactoryWriter WriteArgument(string parameter, SyntaxToken token, bool closeArgumentList = false)
        {
            _ = this.writer
                    .WriteArgumentStart(parameter);
            return this.Write(token)
                       .WriteArgumentEnd(closeArgumentList);
        }

        private SyntaxFactoryWriter WriteArgument<T>(string parameter, SyntaxList<T> syntaxList, bool closeArgumentList = false)
            where T : SyntaxNode
        {
            _ = this.writer
                    .WriteArgumentStart(parameter);
            switch (syntaxList.Count)
            {
                case 0:
                    this.Append("default").WriteArgumentEnd(closeArgumentList);
                    return this;
                case 1:
                    return this.AppendLine($"SyntaxFactory.SingletonList<{typeof(T).Name}>(")
                               .PushIndent()
                               .Write(syntaxList[0])
                               .Append(")")
                               .WriteArgumentEnd(closeArgumentList)
                               .PopIndent();
                default:
                    this.AppendLine($"SyntaxFactory.List(")
                        .PushIndent()
                        .AppendLine($"new {typeof(T).Name}[]")
                        .AppendLine("{")
                        .PushIndent();
                    foreach (var node in syntaxList)
                    {
                        _ = this.Write(node).AppendLine(",");
                    }

                    return this.PopIndent()
                               .Append("})")
                               .WriteArgumentEnd(closeArgumentList)
                               .PopIndent();
            }
        }

        private SyntaxFactoryWriter WriteArgument<T>(string parameter, SeparatedSyntaxList<T> syntaxList, bool closeArgumentList = false)
            where T : SyntaxNode
        {
            _ = this.writer
                    .WriteArgumentStart(parameter);
            switch (syntaxList.Count)
            {
                case 0:
                    this.Append("default").WriteArgumentEnd(closeArgumentList);
                    return this;
                case 1 when syntaxList.SeparatorCount == 0:
                    return this.AppendLine($"SyntaxFactory.SingletonSeparatedList<{typeof(T).Name}>(")
                               .PushIndent()
                               .Write(syntaxList[0])
                               .Append(")")
                               .WriteArgumentEnd(closeArgumentList)
                               .PopIndent();
                default:
                    this.AppendLine("SyntaxFactory.SeparatedList(")
                        .PushIndent()
                        .AppendLine($"new {typeof(T).Name}[]")
                        .AppendLine("{")
                        .PushIndent();
                    foreach (var node in syntaxList)
                    {
                        _ = this.Write(node)
                                .AppendLine(",");
                    }

                    this.PopIndent()
                        .AppendLine("},")
                        .AppendLine($"new SyntaxToken[]")
                        .AppendLine("{")
                        .PushIndent();

                    foreach (var token in syntaxList.GetSeparators())
                    {
                        _ = this.Write(token)
                                .AppendLine(",");
                    }

                    return this.PopIndent()
                               .Append("})")
                               .WriteArgumentEnd(closeArgumentList)
                               .PopIndent();
            }
        }

        private SyntaxFactoryWriter WriteArgument(string parameter, SyntaxTokenList tokenList, bool closeArgumentList = false)
        {
            _ = this.writer
                    .WriteArgumentStart(parameter);
            switch (tokenList.Count)
            {
                case 0:
                    this.Append("default").WriteArgumentEnd(closeArgumentList);
                    return this;
                case 1:
                    return this.AppendLine($"SyntaxFactory.TokenList(")
                               .PushIndent()
                               .Write(tokenList[0])
                               .Append(")")
                               .WriteArgumentEnd(closeArgumentList)
                               .PopIndent();
                default:
                    this.AppendLine($"SyntaxFactory.TokenList(")
                        .PushIndent();
                    for (var i = 0; i < tokenList.Count; i++)
                    {
                        _ = this.Write(tokenList[i]);
                        if (i < tokenList.Count - 1)
                        {
                            this.AppendLine(",");
                        }
                    }

                    this.writer.Append(")");
                    return this.WriteArgumentEnd(closeArgumentList)
                               .PopIndent();
            }
        }

        private SyntaxFactoryWriter WriteArgument(string parameter, SyntaxTriviaList triviaList, bool closeArgumentList = false)
        {
            _ = this.writer
                    .WriteArgumentStart(parameter);
            switch (triviaList.Count)
            {
                case 0:
                    this.Append("default").WriteArgumentEnd(closeArgumentList);
                    return this;
                case 1 when TryGetSingleLine(out var text):
                    return this.Append(text)
                               .WriteArgumentEnd(closeArgumentList);
                default:
                    this.AppendLine($"SyntaxFactory.TriviaList(")
                        .PushIndent();
                    for (var i = 0; i < triviaList.Count; i++)
                    {
                        _ = this.Write(triviaList[i]);
                        if (i < triviaList.Count - 1)
                        {
                            this.AppendLine(",");
                        }
                    }

                    this.writer.Append(")");
                    return this.WriteArgumentEnd(closeArgumentList)
                               .PopIndent();
            }

            bool TryGetSingleLine(out string result)
            {
                if (!triviaList.Any())
                {
                    result = "default";
                    return true;
                }

                if (triviaList.TrySingle(out var trivia))
                {
                    switch (trivia.Kind())
                    {
                        case SyntaxKind.None:
                            result = "default";
                            return true;
                        case SyntaxKind.WhitespaceTrivia:
                            result = trivia.Span.Length == 1
                                ? "SyntaxFactory.TriviaList(SyntaxFactory.Space)"
                                : $"SyntaxFactory.TriviaList(SyntaxFactory.Whitespace(\"{trivia.ToString()}\"))";
                            return true;
                        case SyntaxKind.EndOfLineTrivia:
                            result = "SyntaxFactory.TriviaList(SyntaxFactory.LineFeed)";
                            return true;
                    }
                }

                result = null;
                return false;
            }
        }

        private SyntaxFactoryWriter WriteArgument(string parameter, SyntaxKind kind, bool closeArgumentList = false)
        {
            _ = this.writer
                    .WriteArgumentStart(parameter)
                       .Append("SyntaxKind.")
                       .Append(kind.ToString())
                       .WriteArgumentEnd(closeArgumentList);
            return this;
        }

        private SyntaxFactoryWriter WriteArgument(string parameter, string text, bool escape, bool closeArgumentList = false)
        {
            _ = this.writer
                    .WriteArgumentStart(parameter)
                    .Append("\"");
            foreach (var c in text)
            {
                if (escape &&
                    (c == '\\' ||
                     c == '"'))
                {
                    this.writer.Append('\\');
                }

                this.writer.Append(c);
            }

            return this.Append("\"")
                      .WriteArgumentEnd(closeArgumentList);
        }

        private SyntaxFactoryWriter WriteArgument(string parameter, bool value, bool closeArgumentList = false)
        {
            _ = this.writer
                    .WriteArgumentStart(parameter)
                    .Append("SyntaxKind.")
                    .Append(value ? "true" : "false")
                    .WriteArgumentEnd(closeArgumentList);
            return this;
        }

        private SyntaxFactoryWriter Append(string text)
        {
            this.writer.Append(text);
            return this;
        }

        private SyntaxFactoryWriter AppendLine(string text)
        {
            this.writer.AppendLine(text);
            return this;
        }

        private SyntaxFactoryWriter WriteArgumentEnd(bool closeArgumentList)
        {
            _ = this.writer.WriteArgumentEnd(closeArgumentList);
            return this;
        }

        private SyntaxFactoryWriter PushIndent()
        {
            _ = this.writer.PushIndent();
            return this;
        }

        private SyntaxFactoryWriter PopIndent()
        {
            _ = this.writer.PopIndent();
            return this;
        }

        private class Writer
        {
            private readonly StringBuilder builder = new StringBuilder();
            private string indentation = string.Empty;
            private bool newLine = true;

            public Writer AppendLine(string text)
            {
                Debug.Assert(text != ")", "Probably a bug here, don't think we ever want newline after )");
                if (this.newLine)
                {
                    this.builder.Append(this.indentation);
                }

                this.builder.AppendLine(text);
                this.newLine = true;
                return this;
            }

            public Writer Append(string text)
            {
                if (this.newLine)
                {
                    this.builder.Append(this.indentation);
                }

                this.builder.Append(text);
                this.newLine = false;
                return this;
            }

            public Writer Append(char c)
            {
                if (this.newLine)
                {
                    this.builder.Append(this.indentation);
                }

                this.builder.Append(c);
                this.newLine = false;
                return this;
            }

            public Writer WriteArgumentStart(string parameterName)
            {
                switch (SyntaxFacts.GetKeywordKind(parameterName))
                {
                    case SyntaxKind.None:
                        break;
                    case SyntaxKind.DefaultKeyword:
                    case SyntaxKind.ElseKeyword:
                        this.Append("@");
                        break;
                    default:
                        this.Append("@");
                        break;
                }

                return this.Append(parameterName)
                           .Append(": ");
            }

            public Writer WriteArgumentEnd(bool closeArgumentList)
            {
                if (closeArgumentList)
                {
                    return this.Append(")");
                }
                else
                {
                    return this.AppendLine(",");
                }
            }

            public Writer PushIndent()
            {
                this.indentation += "    ";
                return this;
            }

            public Writer PopIndent()
            {
                this.indentation = this.indentation.Substring(0, this.indentation.Length - 4);
                return this;
            }

            public override string ToString() => this.builder.ToString();
        }
    }
}
