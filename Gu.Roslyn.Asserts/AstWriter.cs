namespace Gu.Roslyn.Asserts
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;

    public class AstWriter
    {
        private readonly StringBuilder builder = new StringBuilder();
        private readonly AstWriterSettings settings;
        private readonly Indentation indentation = new Indentation();

        private AstWriter(AstWriterSettings settings)
        {
            this.settings = settings;
        }

        /// <summary>
        /// Dump the node recursively to a json format for diffing.
        /// </summary>
        /// <param name="node">The <see cref="SyntaxNode"/></param>
        /// <param name="settings">The <see cref="AstWriterSettings"/></param>
        /// <returns>The node serialized into a string format.</returns>
        public static string Serialize(SyntaxNode node, AstWriterSettings settings = null)
        {
            var writer = new AstWriter(settings ?? AstWriterSettings.EveryThing).Write(node);
            return writer.ToString();
        }

        /// <inheritdoc />
        public override string ToString() => this.builder.ToString();

        private AstWriter Write(SyntaxNode node)
        {
            return this.WriteStartElement()
                       .Write(" ")
                       .WriteProperty("Kind", node.Kind().ToString())
                       .WriteTrivia("LeadingTrivia", node.GetLeadingTrivia())
                       .WriteTrivia("TrailingTrivia", node.GetTrailingTrivia())
                       .WriteChildTokens(node.ChildTokens().ToList())
                       .WriteChildNodes(node.ChildNodes().ToList())
                       .Write(" ")
                       .WriteEndElement();
        }

        private AstWriter WriteChildNodes(IReadOnlyList<SyntaxNode> children)
        {
            if (children.Any())
            {
                this.WriteLine(",")
                    .Write(this.indentation)
                    .Write("\"ChildNodes\": [");
                this.indentation.PushChars(17);
                for (var i = 0; i < children.Count; i++)
                {
                    this.Write(children[i]);
                    if (i == children.Count - 1)
                    {
                        this.Write(" ]");
                    }
                    else
                    {
                        this.WriteLine(",")
                            .Write(this.indentation);
                    }
                }

                this.indentation.Pop();
            }

            return this;
        }

        private AstWriter WriteChildTokens(IReadOnlyList<SyntaxToken> children)
        {
            if (children.Any())
            {
                this.WriteLine(",")
                    .Write(this.indentation)
                    .Write("\"ChildTokens\": [ ");
                this.indentation.PushChars(17);
                for (var i = 0; i < children.Count; i++)
                {
                    var token = children[i];
                    this.Write(token);
                    if (i == children.Count - 1)
                    {
                        this.Write(" ]");
                    }
                    else
                    {
                        this.WriteLine(",")
                            .Write(this.indentation);
                    }
                }

                this.indentation.Pop();
            }

            return this;
        }

        private AstWriter Write(SyntaxToken token)
        {
            return this.WriteStartElement()
                       .Write(" ")
                       .WriteProperty("Kind", token.Kind().ToString())
                       .Write(", ")
                       .WriteProperty("Text", token.Text.Replace("\r", "\\r").Replace("\n", "\\n"))
                       .Write(", ")
                       .WriteProperty("ValueText", token.ValueText.Replace("\r", "\\r").Replace("\n", "\\n"))
                       .WriteTrivia("LeadingTrivia", token.LeadingTrivia)
                       .WriteTrivia("TrailingTrivia", token.TrailingTrivia)
                       .Write(" ")
                       .WriteEndElement();
        }

        private AstWriter WriteTrivia(string name, SyntaxTriviaList triviaList)
        {
            if (triviaList.Any())
            {
                this.Write($", \"{name}\": [ ");
                for (var i = 0; i < triviaList.Count; i++)
                {
                    var trivia = triviaList[i];
                    this.Write(trivia)
                        .Write(i == triviaList.Count - 1 ? " ]" : ", ");
                }
            }

            return this;
        }

        private AstWriter Write(SyntaxTrivia trivia)
        {
            return this.Write("{ ")
                         .WriteProperty("Kind", trivia.Kind().ToString())
                         .Write(", ")
                         .WriteProperty("Text", trivia.ToString().Replace("\r", "\\r").Replace("\n", "\\n"))
                         .Write(" }");
        }

        private AstWriter WriteProperty(string name, string value)
        {
            this.builder.Append('"')
                .Append(name)
                .Append('"')
                .Append(": ")
                .Append('"')
                .Append(value)
                .Append('"');
            return this;
        }

        private AstWriter Write(Indentation indentation)
        {
            this.builder.Append(indentation.Current);
            return this;
        }

        private AstWriter Write(char c)
        {
            this.builder.Append(c);
            return this;
        }

        private AstWriter Write(string text)
        {
            this.builder.Append(text);
            return this;
        }

        private AstWriter WriteLine()
        {
            this.builder.AppendLine();
            return this;
        }

        private AstWriter WriteLine(string text)
        {
            this.builder.AppendLine(text);
            return this;
        }

        private AstWriter WriteStartElement()
        {
            this.builder.Append("{");
            return this;
        }

        private AstWriter WriteEndElement()
        {
            this.builder.Append("}");
            return this;
        }

        private class Indentation
        {
            private readonly Stack<string> stack = new Stack<string>(new[] { "  " });

            public string Current => this.stack.Peek();

            public void Push() => this.stack.Push(this.stack.Peek() + "  ");

            public void PushChars(int n) => this.stack.Push(this.stack.Peek() + new string(' ', n));

            public void Pop(int count = 1)
            {
                for (var i = 0; i < count; i++)
                {
                    _ = this.stack.Pop();
                }
            }
        }
    }
}
