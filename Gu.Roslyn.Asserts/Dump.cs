namespace Gu.Roslyn.Asserts
{
    using System.CodeDom.Compiler;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;

    public static class Dump
    {
        public static string Ast(SyntaxNode node, AstWriterSettings settings)
        {
            var writer = new Writer(settings).Write(node);
            return writer.ToString();
        }

        private static IndentedTextWriter WriteProperty(this IndentedTextWriter writer, string name, string value)
        {
            writer.Write($" \"{name}\": \"{value}\"");
            return writer;
        }

        public class Writer
        {
            private readonly StringBuilder builder = new StringBuilder();
            private readonly AstWriterSettings settings;
            private int indentation;

            public Writer(AstWriterSettings settings)
            {
                this.settings = settings;
            }

            public Writer Write(SyntaxNode node)
            {
                this.Write("{")
                    .WriteProperty("Kind", node.Kind().ToString())
                    .Write(",")
                    .WriteProperty("Text", node.ToString().Replace("\r", "\\r").Replace("\n", "\\n"))
                    .WriteTrivia("LeadingTrivia", node.GetLeadingTrivia())
                    .WriteChildTokens(node.ChildTokens().ToList())
                    .WriteTrivia("TrailingTrivia", node.GetTrailingTrivia());

                var childNodes = node.ChildNodes().ToArray();
                if (childNodes.Any())
                {
                    this.indentation++;
                    this.WriteLine(", \"ChildNodes\": [");
                    this.indentation++;
                    for (var i = 0; i < childNodes.Length; i++)
                    {
                        var child = childNodes[i];
                        this.Write(child)
                            .WriteLine(i == childNodes.Length - 1 ? "]}" : ",");
                    }

                    this.indentation--;
                    this.indentation--;
                }

                return this;
            }

            public Writer WriteProperty(string name, string value)
            {
                this.builder.Append(this.builder[this.builder.Length - 1] == '{' ? " " : ", ")
                            .Append('"')
                            .Append(name)
                            .Append('"')
                            .Append(": ")
                            .Append('"')
                            .Append(value)
                            .Append('"');
                return this;
            }

            public Writer WriteChildTokens(IReadOnlyList<SyntaxToken> tokens)
            {
                if (tokens.Any())
                {
                    this.Write(", \"ChildTokens\": [ ");
                    for (var i = 0; i < tokens.Count; i++)
                    {
                        var token = tokens[i];
                        this.Write(token)
                            .WriteLine(i == tokens.Count - 1 ? " ]}" : ", ");
                    }
                }

                return this;
            }

            public Writer Write(SyntaxToken token)
            {
                return this.WriteStartElement()
                           .WriteProperty("Kind", token.Kind().ToString())
                           .Write(",")
                           .WriteProperty("Text", token.Text.Replace("\r", "\\r").Replace("\n", "\\n"))
                           .Write(",")
                           .WriteProperty("ValueText", token.ValueText.Replace("\r", "\\r").Replace("\n", "\\n"))
                           .WriteTrivia("LeadingTrivia", token.LeadingTrivia)
                           .WriteTrivia("TrailingTrivia", token.TrailingTrivia)
                           .WriteEndElement();
            }

            public Writer WriteTrivia(string name, SyntaxTriviaList triviaList)
            {
                if (triviaList.Any())
                {
                    this.Write($", \"{name}\": [ ");
                    for (var i = 0; i < triviaList.Count; i++)
                    {
                        var trivia = triviaList[i];
                        this.Write(trivia)
                            .WriteLine(i == triviaList.Count - 1 ? " ]}" : ", ");
                    }
                }

                return this;
            }

            public Writer Write(SyntaxTrivia trivia)
            {
                return this.Write("{ ")
                             .WriteProperty("Kind", trivia.Kind().ToString())
                             .Write(",")
                             .WriteProperty("Text", trivia.ToString().Replace("\r", "\\r").Replace("\n", "\\n"))
                             .Write(" }");
            }

            public Writer Write(char c)
            {
                this.builder.Append(c);
                return this;
            }

            public Writer Write(string text)
            {
                this.builder.Append(text);
                return this;
            }

            public Writer WriteLine()
            {
                this.builder.AppendLine();
                return this;
            }

            public Writer WriteLine(string text)
            {
                this.builder.AppendLine(text);
                return this;
            }

            public Writer WriteStartElement()
            {
                this.builder.Append("{");
                return this;
            }

            public Writer WriteEndElement()
            {
                this.builder.Append("}");
                return this;
            }
        }
    }
}
